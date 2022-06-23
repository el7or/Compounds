using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using log4net.Config;
using SendNotification.Manager;
using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace SendNotification
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            XmlConfigurator.Configure();
            Timer timer = new Timer();
            timer.Interval = 1000; // 1 seconds
            timer.Elapsed += new ElapsedEventHandler(this.OnTimer);
            timer.Start();

            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "circle360-firebase-adminsdk.json");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(filePath)
            });
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            var manager = new NotificationScheduleManager();
            manager.HandleScheduleNotifications();
        }

        protected override void OnStop()
        { }
    }
}
