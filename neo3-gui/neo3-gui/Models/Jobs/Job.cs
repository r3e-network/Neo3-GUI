using System;
using System.Threading.Tasks;

namespace Neo.Models.Jobs
{
    /// <summary>
    /// Base class for background notification jobs
    /// </summary>
    public abstract class Job
    {
        /// <summary>
        /// Job start time
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Last successful trigger time
        /// </summary>
        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Interval between job executions
        /// </summary>
        public TimeSpan IntervalTime { get; set; } = TimeSpan.MaxValue;

        /// <summary>
        /// Next scheduled trigger time
        /// </summary>
        public DateTime NextTriggerTime => LastTriggerTime + IntervalTime;

        /// <summary>
        /// Count of consecutive errors
        /// </summary>
        public int ConsecutiveErrors { get; set; }

        /// <summary>
        /// Whether the job is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Execute the job and return a message to push
        /// </summary>
        public abstract Task<WsMessage> Invoke();
    }
}
