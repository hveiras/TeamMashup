using System;

namespace TeamMashup.Core.Domain
{
    public interface IScheduleableEntity
    {
        DateTime From { get; set; }

        DateTime To { get; set; }
    }
}
