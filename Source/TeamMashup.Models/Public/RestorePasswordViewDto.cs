using System;

namespace TeamMashup.Models.Public
{
    public class RestorePasswordViewDto
    {
        public Guid Token { get; set; }

        public string NewPassword { get; set; }
    }
}