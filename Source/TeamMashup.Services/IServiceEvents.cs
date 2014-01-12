using System.ServiceProcess;

namespace TeamMashup.Services
{
    interface IServiceEvents
    {
        /// <summary>
        ///    When implemented in a derived class, executes when a Start command is sent
        ///    to the service by the Service Control Manager (SCM) or when the operating
        ///    system starts (for a service that starts automatically). Specifies actions
        ///    to take when the service starts.
        ///    <param name="args">Data passed by the start command. </param>
        /// </summary>
        void OnStart(string[] args);

        /// <summary>
        //     When implemented in a derived class, executes when a Pause command is sent
        //     to the service by the Service Control Manager (SCM). Specifies actions to
        //     take when a service pauses.
        /// </summary>
        void OnPause();

        /// <summary>
        ///     When implemented in a derived class, System.ServiceProcess.ServiceBase.OnContinue()
        ///     runs when a Continue command is sent to the service by the Service Control
        ///     Manager (SCM). Specifies actions to take when a service resumes normal functioning
        ///     after being paused.
        /// </summary>
        void OnContinue();

        /// <summary>
        ///     When implemented in a derived class, executes when a Stop command is sent
        ///     to the service by the Service Control Manager (SCM). Specifies actions to
        ///     take when a service stops running.
        /// </summary>
        void OnStop();

        /// <summary>
        ///     When implemented in a derived class, executes when the system is shutting
        ///     down. Specifies what should occur immediately prior to the system shutting
        ///     down.
        /// </summary>
        void OnShutdown();

        /// <summary>
        ///     When implemented in a derived class, System.ServiceProcess.ServiceBase.OnCustomCommand(System.Int32)
        ///     executes when the Service Control Manager (SCM) passes a custom command to
        ///     the service. Specifies actions to take when a command with the specified
        ///     parameter value occurs.
        ///     <param name="command">The command message sent to the service.</param>
        /// </summary>
        void OnCustomCommand(int command);

        /// <summary>
        ///     When implemented in a derived class, executes when the computer's power status
        ///     has changed. This applies to laptop computers when they go into suspended
        ///     mode, which is not the same as a system shutdown.
        ///     <param name="powerStatus">
        ///     A System.ServiceProcess.PowerBroadcastStatus that indicates a notification
        ///     from the system about its power status.
        ///     </param>
        ///     <returns>
        ///     When implemented in a derived class, the needs of your application determine
        ///     what value to return. For example, if a QuerySuspend broadcast status is
        ///     passed, you could cause your application to reject the query by returning
        ///     false.
        ///     </returns>
        /// </summary>
        bool OnPowerEvent(PowerBroadcastStatus powerStatus);

        /// <summary>
        ///     Executes when a change event is received from a Terminal Server session.
        ///     <param name="changeDescription">A System.ServiceProcess.SessionChangeDescription structure that identifies the change type. </param>
        /// </summary>
        void OnSessionChange(SessionChangeDescription changeDescription);
    }
}
