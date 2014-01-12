using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using TeamMashup.Core.Domain;

namespace TeamMashup.Membership
{
    public class WebRoleProvider : RoleProvider
    {
        private string applicationName;

        public override string ApplicationName
        {
            get
            {
                return applicationName;
            }
            set
            {
                applicationName = value;
            }
        }

        public override void AddUsersToRoles(string[] userEmails, string[] roleNames)
        {
            using (var context = new DatabaseContext())
            {
                var roles = context.Roles.GetByNames(roleNames);

                foreach (var email in userEmails)
                {
                    User user;
                    if (context.Users.TryGetByEmail(email, out user))
                    {
                        var rolesToAdd = roles.Except(user.Roles);
                        foreach (var role in rolesToAdd)
                        {
                            user.Roles.Add(role);
                        }
                    }
                    else
                    {
                        //TODO: Log exception.
                    }
                }

                context.SaveChanges();
            }
        }

        public override void CreateRole(string roleName)
        {
            using (var context = new DatabaseContext())
            {
                var role = new Role(roleName);
                context.Roles.Add(role);
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            using (var context = new DatabaseContext())
            {
                Role role;
                if (context.Roles.TryGetByName(roleName, out role))
                {
                    if(role.Users.Any() && throwOnPopulatedRole)
                        throw new InvalidOperationException("Cannot delete a role that contains users");

                    context.Roles.Remove(role);
                    context.SaveChanges();

                    return true;
                }
                else
                {
                    //TODO: Log role not found.
                    return false;
                }
            }
        }

        public override string[] GetAllRoles()
        {
            using (var context = new DatabaseContext())
            {
                return context.Roles.Select(x => x.Name).ToArray();
            }
        }

        public override string[] GetRolesForUser(string userEmail)
        {
            using (var context = new DatabaseContext())
            {
                User user;
                if (context.Users.TryGetByEmail(userEmail, out user))
                {
                    return user.Roles.Select(x => x.Name).ToArray();
                }
                else
                {
                    //TODO: log user not found.
                    return new List<string>().ToArray();
                }
            }
        }

        public override bool IsUserInRole(string userEmail, string roleName)
        {
            using (var context = new DatabaseContext())
            {
                User user;
                if (context.Users.TryGetByEmail(userEmail, out user))
                {
                    return user.Roles.Any(x => x.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
                }
                else
                {
                    //TODO: log user not found.
                    return false;
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] userEmails, string[] roleNames)
        {
            using (var context = new DatabaseContext())
            {
                var roles = context.Roles.GetByNames(roleNames);

                foreach (var role in roles)
                {
                    foreach (var email in userEmails)
                    {
                        User user;
                        if (context.Users.TryGetByEmail(email, out user))
                        {
                            role.Users.Remove(user);
                        }
                    }
                }

                context.SaveChanges();
            }
        }

        public override bool RoleExists(string roleName)
        {
            using (var context = new DatabaseContext())
            {
                return context.Roles.Any(x => x.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public bool UserHasClaims(long userId, params string[] claims)
        {
            var userClaims = new List<string>();

            using (var context = new DatabaseContext())
            {
                User user;
                if (!context.Users.TryGetById(userId, out user))
                    return false;

                userClaims = (from r in user.Roles
                              from c in r.Claims
                              select c.Name).ToList();
            }

            foreach (var claim in claims)
            {
                if (!userClaims.Contains(claim))
                    return false;
            }

            return true;
        }

        #region NotSupported Operations

        [Obsolete("This method is just to mantain compatibility with RoleProvider base class")]
        public override string[] FindUsersInRole(string roleName, string userEmailToMatch)
        {
            throw new NotSupportedException();
        }

        [Obsolete("This method is just to mantain compatibility with RoleProvider base class")]
        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
