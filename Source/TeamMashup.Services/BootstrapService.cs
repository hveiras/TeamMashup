using Quartz;
using Quartz.Impl;
using System.ServiceProcess;
using System.Threading.Tasks;
using TeamMashup.Core.Tracking;

namespace TeamMashup.Services
{
    partial class BootstrapService : ServiceBase, IServiceBase, IServiceEvents
    {
        private volatile bool isStarted;

        public BootstrapService()
        {
            InitializeComponent();
        }

        public void Start(bool asService)
        {
            if (asService)
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { this };
                ServiceBase.Run(ServicesToRun);
            }
            else
            {
                var task = Task.Factory.StartNew(() => StartSync(new string[] { }));
                task.Wait();
            }
        }

        private void StartSync(string[] args)
        {
            OnStart(args);

            while (isStarted)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        #region Internal Overrides for Testing

        protected override void OnStart(string[] args)
        {
            isStarted = true;
            ((IServiceEvents)this).OnStart(args);
        }

        protected override void OnStop()
        {
            ((IServiceEvents)this).OnStop();
            isStarted = false;
        }

        protected override void OnPause()
        {
            ((IServiceEvents)this).OnPause();
        }

        protected override void OnContinue()
        {
            ((IServiceEvents)this).OnContinue();
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return ((IServiceEvents)this).OnPowerEvent(powerStatus);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            ((IServiceEvents)this).OnSessionChange(changeDescription);
        }

        protected override void OnShutdown()
        {
            ((IServiceEvents)this).OnShutdown();
            isStarted = true;
        }

        protected override void OnCustomCommand(int command)
        {
            ((IServiceEvents)this).OnCustomCommand(command);
        }

        #endregion Internal Overrides for Testing

        #region Service Lifecycle

        private IScheduler scheduler;

        void IServiceEvents.OnStart(string[] args)
        {
            // Construct a scheduler factory
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            // Get and Start a scheduler
            scheduler = schedulerFactory.GetScheduler();
            scheduler.Start();
            scheduler.SpawnBillingJobs();
            scheduler.SpawnBackupJobs();

            LogHelper.Info("TeamMashup services started");
        }

        void IServiceEvents.OnContinue()
        {
            scheduler.ResumeAll();
            LogHelper.Info("TeamMashup services successfully continued.");
        }

        void IServiceEvents.OnPause()
        {
            scheduler.PauseAll();
            LogHelper.Info("TeamMashup services successfully paused.");
        }

        void IServiceEvents.OnStop()
        {
            scheduler.Shutdown(true);
            LogHelper.Info("SampleProject services successfully stoped.");
        }

        void IServiceEvents.OnShutdown()
        {
            scheduler.Shutdown(true);
        }

        bool IServiceEvents.OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            return base.OnPowerEvent(powerStatus);
        }

        void IServiceEvents.OnSessionChange(SessionChangeDescription changeDescription)
        { }

        void IServiceEvents.OnCustomCommand(int command)
        { }

        #endregion Service Lifecycle
    }
}
