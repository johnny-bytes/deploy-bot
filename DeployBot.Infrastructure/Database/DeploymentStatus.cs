namespace DeployBot.Infrastructure.Database
{
    public enum DeploymentStatus
    {
        Success = 0,
        Failed = 1,
        InProgress = 2,
        Enqueued = 3,
        Pending = 4
    }
}
