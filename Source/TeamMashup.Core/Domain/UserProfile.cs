using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class UserProfile
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public long LanguageId { get; set; }

        public virtual Language Language { get; set; }

        public virtual ICollection<Survey> CompletedSurveys { get; set; }

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

        public UserProfile()
        {

        }

        public UserProfile(long userId, long languageId) : this()
        {
            UserId = userId;
            LanguageId = languageId;
        }
    }

    public class UserProfileConfiguration : EntityTypeConfiguration<UserProfile>
    {
        public UserProfileConfiguration()
        {
        }
    }
}