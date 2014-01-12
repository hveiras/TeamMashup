using System.Data.Entity.ModelConfiguration;

namespace TeamMashup.Core.Domain
{
    public class Language : IEntitySet, IUniqueNamedEntity
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public Language() { }

        public Language(string code, string name) : this()
        {
            Code = code;
            Name = name;
        }
    }

    public class LanguageConfiguration : EntityTypeConfiguration<Language>
    {
        public LanguageConfiguration()
        {
            Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(5);

            Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}