
namespace TeamMashup.Services
{
    interface IServiceBase
    {
        /// <summary>
        ///     Indicates whether to report Start, Stop, Pause, and Continue commands in
        ///     the event log.
        ///     <returns>
        ///         true to report information in the event log; otherwise, false.
        ///     </returns>
        /// </summary>
        bool AutoLog { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the service can handle notifications
        ///     of computer power status changes.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     This property is modified after the service was started.
        /// </exception>
        bool CanHandlePowerEvent { get; }

        /// <summary>
        ///     Gets or sets a value that indicates whether the service can handle session
        ///     change events received from a Terminal Server session.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     This property is modified after the service was started.
        /// </exception>
        bool CanHandleSessionChangeEvent { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the service can be paused and resumed.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The service has already been started. The CanPauseAndContinue property cannot be changed once the service has started.
        /// </exception>
        bool CanPauseAndContinue { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the service should be notified when the system is shutting down.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The service has already been started. The CanShutdown property cannot be changed once the service has started.
        /// </exception>
        bool CanShutdown { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the service can be stopped once it has started.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        ///     The service has already been started. The CanStop property cannot be changed once the service has started.
        /// </exception>
        bool CanStop { get; }

        /// <summary>
        /// Gets or sets the short name used to identify the service to the system.
        /// </summary>
        string ServiceName { get; set; }

        /// <summary>
        /// Starts the executing service.
        /// </summary>
        void Start(bool asService);

        /// <summary>
        /// Stops the executing service.
        /// </summary>
        void Stop();
    }
}
