using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using TeamMashup.Core.Enums;

namespace TeamMashup.Models.Internal
{
    public class RoleModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum length is {1}")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        public AssetScope Scope { get; set; }
    }

    public class RoleMembersModel
    {
        public long RoleId { get; set; }

        public string RoleName { get; set; }

        public MultiSelectList Members { get; set; }

        public RoleMembersModel()
        {
            this.Members = new MultiSelectList(new List<SelectListItem>());
        }
    }

    public class RoleClaimsModel
    {
        public long RoleId { get; set; }

        public string RoleName { get; set; }

        public MultiSelectList Claims { get; set; }

        public MultiSelectList AvailableClaims { get; set; }

        public RoleClaimsModel()
        {
            this.Claims = new MultiSelectList(new List<SelectListItem>());
            this.AvailableClaims = new MultiSelectList(new List<SelectListItem>());
        }
    }

    public class UserRolesModel
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public MultiSelectList Roles { get; set; }

        public MultiSelectList AvailableRoles { get; set; }

        public UserRolesModel()
        {
            this.Roles = new MultiSelectList(new List<SelectListItem>());
            this.AvailableRoles = new MultiSelectList(new List<SelectListItem>());
        }
    }

    public class UserSearchModel
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class ClaimModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public AssetScope Scope { get; set; }
    }
}