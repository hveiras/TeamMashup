using System.Collections.Generic;
using System.Web.Mvc;

namespace TeamMashup.Models.Admin
{
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
}