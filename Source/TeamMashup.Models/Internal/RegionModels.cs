using System.ComponentModel.DataAnnotations;

namespace TeamMashup.Models.Internal
{
    public class CountryModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }

    public class LanguageModel
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(5)]
        public string Code { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}