using Microsoft.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeamMashup.Core.Contracts;
using TeamMashup.Core.Domain;
using TeamMashup.Core.Enums;
using TeamMashup.Core.Security;
using TeamMashup.Membership;
using TeamMashup.Models;
using TeamMashup.Models.Internal;
using TeamMashup.Server.Filters;

namespace TeamMashup.Server.Areas.Internal.Controllers
{
    [WebAuthorize(Claims="manage-security")]
    [NoCache]
    public class SecurityController : InternalBaseController
    {
        public ActionResult Index()
        {
            return this.RedirectToAction<SecurityController>(x => x.Roles());
        }

        public ActionResult Roles()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Claims()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult NavTabs(string activeTab)
        {
            ViewBag.ActiveTab = activeTab ?? string.Empty;
            return PartialView("_NavTabs");
        }

        [ChildActionOnly]
        public ActionResult AddUserInline()
        {
            return PartialView("_AddUserInline");
        }

        [ChildActionOnly]
        public ActionResult AddClaimInline()
        {
            return PartialView("_AddClaimInline");
        }

        [ChildActionOnly]
        public ActionResult AddRoleInline()
        {
            return PartialView("_AddRoleInline");
        }

        public ActionResult RoleMembers(long roleId)
        {
            var role = Context.Roles.GetById(roleId);

            if (role == null)
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));

            var model = new RoleMembersModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Members = new MultiSelectList(role.Users, "Id", "Name")
            };

            return View(model);
        }

        public ActionResult GetRoleItems(AssetScope scope, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Roles.Where(x => x.ScopeValue == (int)scope)
                                         .OrderByDescending(x => x.Name);

                var totalRecords = query.Count();

                var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

                var model = new DataTablePage
                {
                    sEcho = sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

                var roles = (from x in page
                             select new RoleModel
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             }).ToList();

                foreach (var role in roles)
                {
                    var item = new Dictionary<string, string>
                    {
                        {"Name", role.Name},
                        {"DT_RowId", "roleItem_" + role.Id}
                    };

                    model.aaData.Add(item);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetClaimItems(AssetScope scope, int iDisplayStart, int iDisplayLength, string sEcho)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Claims.Where(x => x.ScopeValue == (int)scope)
                                          .OrderByDescending(x => x.Name);

                var totalRecords = query.Count();

                var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

                var model = new DataTablePage
                {
                    sEcho = sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

                var roles = (from x in page
                             select new ClaimModel
                             {
                                 Id = x.Id,
                                 Name = x.Name
                             }).ToList();

                foreach (var role in roles)
                {
                    var item = new Dictionary<string, string>
                    {
                        {"Name", role.Name},
                        {"DT_RowId", "claimItem_" + role.Id}
                    };

                    model.aaData.Add(item);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SearchUsers(string searchTerm)
        {
            var model = (from u in Context.Users
                         where u.Name.Contains(searchTerm)
                         select new TypeaheadDatum
                         {
                             value = u.Id,
                             name = u.Name
                         }).ToList();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserItems(int iDisplayStart, int iDisplayLength, string sEcho)
        {
            using (var context = new DatabaseContext())
            {
                var query = context.Users.NonDeleted()
                                         .PlatformUsers()
                                         .OrderByDescending(x => x.CreatedDate);

                var totalRecords = query.Count();

                var page = query.Skip(iDisplayStart).Take(iDisplayLength).ToList();

                var model = new DataTablePage
                {
                    sEcho = sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords
                };

                var users = (from x in page
                             select new UserModel
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 Email = x.Email
                             }).ToList();

                foreach (var user in users)
                {
                    var item = new Dictionary<string, string>
                    {
                        {"Name", user.Name},
                        {"Email", user.Email},
                        {"DT_RowId", "userItem_" + user.Id}
                    };

                    model.aaData.Add(item);
                }

                return Json(model, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RoleClaims(long roleId)
        {
            var role = Context.Roles.GetById(roleId);

            if (role == null)
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));

            var availableClaims = Context.Claims.PlatformClaims()
                                                .AsEnumerable()
                                                .Except(role.Claims, new EntityComparer())
                                                .ToList();
                                                

            var model = new RoleClaimsModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Claims = new MultiSelectList(role.Claims, "Id", "Name"),
                AvailableClaims = new MultiSelectList(availableClaims, "Id", "Name")
            };

            return View(model);
        }

        public ActionResult UserRoles(long userId)
        {
            User user;
            if (Context.Users.TryGetById(userId, out user) && !user.Deleted)
            {
                var availableRoles = Context.Roles.PlatformRoles()
                                                  .AsEnumerable()
                                                  .Except(user.Roles, new EntityComparer())
                                                  .ToList();

                var model = new UserRolesModel
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    Roles = new MultiSelectList(user.Roles, "Id", "Name"),
                    AvailableRoles = new MultiSelectList(availableRoles, "Id", "Name")
                };

                return View(model);
            }

            return this.RedirectToAction<SecurityController>(x => x.Users());
        }

        [HttpPost]
        public ActionResult UpdateRoleUsers(long roleId, long[] members)
        {
            if (members == null)
                throw new ArgumentNullException("members");

            Role role;
            if (!Context.Roles.TryGetById(roleId, out role))
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));


            foreach (var userId in members)
            {
                User user;
                if (!Context.Users.TryGetById(userId, out user))
                    throw new InvalidOperationException(string.Format("user with id {0} was not found", userId));

                role.Users.Add(user);
            }

            Context.SaveChanges();


            return this.RedirectToAction<SecurityController>(x => x.Roles());
        }

        [HttpPost]
        public ActionResult AddUserInline(UserModel model)
        {
            if (ModelState.IsValid)
            {
                Context.Users.Add(new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = SecurityManager.Hash("123") //TODO: replace this for an autogenerated password.
                });

                Context.SaveChanges();

                //TODO: send activation email.
            }

            return PartialView("_AddUserInline", model);
        }

        [HttpPost]
        public ActionResult AddClaimInline(ClaimModel model)
        {
            if (ModelState.IsValid)
            {
                Context.Claims.Add(new Claim
                {
                    Name = model.Name,
                    Scope = model.Scope
                });

                Context.SaveChanges();
            }

            return PartialView("_AddClaimInline", model);
        }

        [HttpPost]
        public ActionResult AddRoleInline(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                Context.Roles.Add(new Role
                {
                    Name = model.Name,
                    Scope = model.Scope
                });

                Context.SaveChanges();
            }

            return PartialView("_AddRoleInline", model);
        }

        [HttpPost]
        public ActionResult UpdateRoleMembers(long roleId, IEnumerable<long> userIds)
        {
            if (userIds == null)
            {
                userIds = new List<long>();
            }

            Role role;
            if (!Context.Roles.TryGetById(roleId, out role))
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));

            var users = Context.Users.FilterByIds(userIds).ToList();
            role.Users.Clear();

            foreach (var user in users)
            {
                role.Users.Add(user);
            }

            Context.SaveChanges();

            return JsonSuccess(Url.Action("Roles", "Security"));
        }

        [HttpPost]
        public ActionResult UpdateRoleClaims(long roleId, IEnumerable<long> claimIds)
        {
            if (claimIds == null)
            {
                claimIds = new List<long>();
            }

            Role role;
            if (!Context.Roles.TryGetById(roleId, out role))
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));

            var claims = Context.Claims.FilterByIds(claimIds).ToList();
            role.Claims.Clear();

            foreach (var claim in claims)
            {
                role.Claims.Add(claim);
            }

            Context.SaveChanges();

            return JsonSuccess(Url.Action("Roles", "Security"));
        }

        [HttpPost]
        public ActionResult UpdateUserRoles(long userId, IEnumerable<long> roleIds)
        {
            if (roleIds == null)
            {
                roleIds = new List<long>();
            }

            User user;
            if (!Context.Users.TryGetById(userId, out user))
                throw new InvalidOperationException(string.Format("user with id {0} was not found", userId));

            var roles = Context.Roles.FilterByIds(roleIds).ToList();
            user.Roles.Clear();

            foreach (var role in roles)
            {
                user.Roles.Add(role);
            }

            Context.SaveChanges();

            return JsonSuccess(Url.Action("Users", "Security"));
        }

        public ActionResult DeleteRoles(long roleId)
        {
            return PartialView("_DeleteModalOld");
        }

        public ActionResult DeleteClaims(long claimId)
        {
            return PartialView("_DeleteModalOld");
        }

        public ActionResult DeleteUsers(long userId)
        {
            return PartialView("_DeleteModalOld");
        }

        [HttpPost]
        public ActionResult DeleteRoles(long[] ids)
        {
            var roles = Context.Roles.FilterByIds(ids).ToList();
            var notDeletableRoles = roles.Where(x => x.IsSystemRole).ToList();

            var rolesToDelete = roles.Where(x => !x.IsSystemRole).ToList();

            foreach (var role in rolesToDelete)
            {
                Context.Roles.Remove(role);
            }

            Context.SaveChanges();

            if (notDeletableRoles.Any())
                return JsonError("Some roles were not deleted because they are system protected roles.");

            return JsonSuccess();
        }

        [HttpPost]
        public ActionResult DeleteClaims(long[] ids)
        {
            var claims = Context.Claims.FilterByIds(ids).ToList();
            var notDeletableClaims = claims.Where(x => x.IsSystemClaim).ToList();

            var claimsToDelete = claims.Where(x => !x.IsSystemClaim).ToList();

            foreach (var claim in claimsToDelete)
            {
                Context.Claims.Remove(claim);
            }

            Context.SaveChanges();

            if (notDeletableClaims.Any())
                return JsonError("Some claims were not deleted because they are system protected claims.");

            return JsonSuccess();
        }

        [HttpPost]
        public ActionResult DeleteUsers(long[] ids)
        {
            var users = Context.Users.FilterByIds(ids).ToList();
            var notDeletableUsers = users.Where(x => x.Id == WebSecurity.CurrentUserId).ToList();

            var usersToDelete = users.Where(x => x.Id != WebSecurity.CurrentUserId).ToList();

            foreach (var user in usersToDelete)
            {
                Context.Users.Remove(user);
            }

            Context.SaveChanges();

            if (notDeletableUsers.Any())
                return JsonError("You cannot delete your own user.");

            return JsonSuccess();
        }

        public ActionResult EditRole(long roleId)
        {
            Role role;
            if(!Context.Roles.TryGetById(roleId, out role))
                throw new InvalidOperationException(string.Format("role with id {0} was not found", roleId));

            var model = new RoleModel
            {
                Id = role.Id,
                Name = role.Name,
                Scope = role.Scope
            };

            return PartialView("_EditRole", model);
        }

        public ActionResult EditClaim(long claimId)
        {
            Claim claim;
            if (!Context.Claims.TryGetById(claimId, out claim))
                throw new InvalidOperationException(string.Format("claim with id {0} was not found", claimId));

            var model = new ClaimModel
            {
                Id = claim.Id,
                Name = claim.Name,
                Scope = claim.Scope
            };

            return PartialView("_EditClaim", model);
        }

        public ActionResult EditUser(long userId)
        {
            User user;
            if (!Context.Users.TryGetById(userId, out user))
                throw new InvalidOperationException(string.Format("user with id {0} was not found", userId));

            var model = new UserModel
            {
                Id = user.Id,
                Name = user.Name
            };

            return PartialView("_EditUser", model);
        }

        [HttpPost]
        public ActionResult EditRole(RoleModel model)
        {
            if (ModelState.IsValid)
            {
                Role role;
                if (!Context.Roles.TryGetById(model.Id, out role))
                    throw new InvalidOperationException(string.Format("role with id {0} was not found", model.Id));

                role.Name = model.Name;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditRole", model);
        }

        [HttpPost]
        public ActionResult EditClaim(ClaimModel model)
        {
            if (ModelState.IsValid)
            {
                Claim claim;
                if (!Context.Claims.TryGetById(model.Id, out claim))
                    throw new InvalidOperationException(string.Format("claim with id {0} was not found", model.Id));

                claim.Name = model.Name;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditClaim", model);
        }

        [HttpPost]
        public ActionResult EditUser(UserModel model)
        {
            if (ModelState.IsValid)
            {
                User user;
                if (!Context.Users.TryGetById(model.Id, out user))
                    throw new InvalidOperationException(string.Format("user with id {0} was not found", model.Id));

                user.Name = model.Name;
                Context.SaveChanges();
            }

            return JsonView(ModelState.IsValid, "_EditUser", model);
        }
    }
}