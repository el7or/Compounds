namespace SendNotification.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public partial class NotificationContext : DbContext
    {
        public NotificationContext() : base("name=NotificationContext")
        {
        }

        public NotificationContext(string connectionString) : base(connectionString)
        {

        }

        public virtual DbSet<NotificationSchedule> NotificationSchedules { get; set; }
        public List<RegisterModel> GetTokensForSchedual(Guid id)
        {
            var param = new SqlParameter("@notificationSchedualId", id);
            var result = this.Database.SqlQuery<RegisterModel>("GetSchedualTokens @notificationSchedualId", param).ToList();
            return result;
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
