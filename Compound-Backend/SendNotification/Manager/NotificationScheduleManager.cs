using log4net;
using SendNotification.Data;
using SendNotification.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace SendNotification.Manager
{
    public class NotificationScheduleManager
    {
        static object scheduleLock = new object();
        private NotificationContext dbContext;

        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public NotificationScheduleManager()
        {
            dbContext = new NotificationContext();
        }

        public void HandleScheduleNotifications()
        {
            NotificationSchedule schedule = null;
            try
            {
                lock (scheduleLock)
                {
                    schedule = dbContext.NotificationSchedules.Where(x => !x.HasSend && x.IsActive & !x.IsDeleted
                    && DateTime.Now >= x.ScheduleDateTime).OrderBy(x => x.PrioritySend).FirstOrDefault();
                    if (schedule != null)
                    {
                        schedule.IsActive = false;
                        dbContext.SaveChanges();
                    }
                }
                if (schedule != null)
                {
                    var regs = RegIds(schedule);
                    ProcessRegs(regs);
                    schedule.HasSend = true;
                }
            }
            catch (Exception ex)
            {
                if (schedule != null)
                {
                    schedule.IsActive = true;
                    schedule.HasError = true;
                    schedule.HasSend = false;
                }
                log.Error("Error Happen", ex);
            }
            dbContext.SaveChanges();
            dbContext.Dispose();
        }

        private List<DataForSend> RegIds(NotificationSchedule schedule)
        {
            List<DataForSend> regIds = dbContext.GetTokensForSchedual(schedule.Id).ToList()
                          .Select(x => new DataForSend
                          {
                              Device = x.RegisterType,
                              RegisterId = x.RegisterId,
                          }).ToList();

            if (regIds != null && regIds.Count > 0)
            {
                regIds.ForEach(x =>
                {
                    x.RecordId = schedule.RecordId;
                    x.NotificationText = schedule.NotificationText;
                    x.Title = schedule.NotificationText;
                    x.Global = schedule.GlobalType;
                    x.NotificationData = schedule.NotificationData;
                });
            }
            return regIds;
        }

        private void ProcessRegs(List<DataForSend> list)
        {
            var size = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
            var pages = list.Count / size;
            pages += list.Count % size > 0 ? 1 : 0;
            while (pages > 0)
            {
                Task.Run(() => SendNotification(list.Skip(size * (pages - 1)).Take(size).ToList()));
                pages--;
            }
        }

        private void SendNotification(List<DataForSend> list)
        {
            foreach (var item in list)
            {
                var notificationManager = new NotificationManager();
                notificationManager.Send(item);
            }
        }
    }
}
