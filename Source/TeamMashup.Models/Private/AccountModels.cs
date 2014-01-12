using System;
using System.Web.Mvc;

namespace TeamMashup.Models.Private
{
    public class UserProfileModel
    {
        public long LanguageId { get; set; }

        public SelectList Languages { get; set; }

        public string Bio { get; set; }

        public DateTime? BirthDay { get; set; }

        public string Department { get; set; }

        public string Expertise { get; set; }

        public string Interests { get; set; }

        public string JobTitle { get; set; }

        public string Location { get; set; }

        public string MobilePhone { get; set; }

        public string SkypeName { get; set; }

        public string FacebookProfile { get; set; }

        public string LinkedinProfile { get; set; }

        public string TwitterUserName { get; set; }

        public string WorkPhone { get; set; }

        public string WorkPhoneExtension { get; set; }
    }
}