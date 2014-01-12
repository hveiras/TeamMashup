using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeamMashup.Core.Enums
{
    public enum SystemGroups
    {
        [Description("Admin")]
        Admin = 1,

        [Description("Contributor")]
        Contributor,

        [Description("Reader")]
        Reader
    }

    public enum IssueType
    {
        [Display(Name = "UserStory", ResourceType = typeof(I18n.Enums))]
        UserStory = 1,

        [Display(Name = "Defect", ResourceType = typeof(I18n.Enums))]
        Defect
    }

    public enum ScheduleState
    {
        Backlog = 1,
        Defined,
        InProgress,
        Done,
        Accepted,
        Blocked
    }

    public enum ReleaseState
    {
        [Description("Planning")]
        Planning = 1,
        
        [Description("Active")]
        Active,
        
        [Description("Accepted")]
        Accepted
    }

    public enum IterationState
    {
        Planning = 1,
        Commited,
        Accepted
    }

    public enum TaskState
    {
        Defined = 1,
        InProgress,
        Completed,
        Blocked
    }

    public enum SubscriptionState
    {
        ActivationPending = 1,
        Active,
        PaymentFailed,
        PastDue,
        Cancelled
    }

    public enum LogEntryLevel
    {
        Information = 1,
        Warning,
        Error,
        Critical,
        All
    }

    public enum CommentScope
    {
        Tenant,
        Internal
    }

    public enum AssetScope
    {
        Platform,
        Tenant
    }

    public enum BackupType
    {
        Full = 1,
        Incremental
    }

    public enum BackupState
    {
        Pending = 1,
        InProgress,
        Completed
    }

    public enum BackupSchedule
    {
        Automatic = 1,
        OnDemand
    }

    public enum IssueProgressType
    {
        IterationStarted = 1,
        StatusChanged,
        IterationCompleted
    }
}