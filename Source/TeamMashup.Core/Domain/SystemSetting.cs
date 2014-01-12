using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class SystemSetting
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }

        public SystemSetting()
        {

        }

        public SystemSetting(string key, string value) : this()
        {
            Key = key;
            Value = value;
        }
    }

    public class SystemSettingConfiguration : EntityTypeConfiguration<SystemSetting>
    {
        public SystemSettingConfiguration()
        {
            Property(x => x.Value).IsRequired();
        }
    }
}
