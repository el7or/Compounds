namespace Puzzle.Compound.Common.Enums
{
    public enum PushNotificationType
    {
        VisitRequestedOnGate = 1,
        VisitApprove,
        VisitCanceled,
        ServiceAccepted,
        ServiceAcceptedWithComment,
        ServiceComment,
        ServiceCanceled,
        IssueAccepted,
        IssueAcceptedWithComment,
        IssueComment,
        IssueCanceled,
        Advertise,
        Notification,
        News,
        RegisteredUserApproved,
        SubAccountDeleted,
        SubAccountCanceled,
        SubAccountActive,
        ContractEnd
    }

    public enum SendPriority
    {
        High = 1,
        Medium,
        Low
    }
}