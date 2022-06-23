using Puzzle.Compound.Core.Models;

namespace Puzzle.Compound.Data.Repositories
{

    public class NotificationScheduleRepository : RepositoryBase<NotificationSchedule>, INotificationScheduleRepository
    {
        public NotificationScheduleRepository(CompoundDbContext context) : base(context)
        {

        }
    }

    public interface INotificationScheduleRepository : IRepository<NotificationSchedule>
    {

    }
}