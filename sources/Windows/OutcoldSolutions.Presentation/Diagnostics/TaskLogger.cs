﻿// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Diagnostics
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    /// <summary>
    /// The task logger.
    /// </summary>
    public static class TaskLogger
    {
        /// <summary>
        /// The log task.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="task">
        /// The task.
        /// </param>
        public static void LogTask(this ILogger logger, Task task)
        {
            Debug.Assert(logger != null, "logger != null");
            Debug.Assert(task != null, "task != null");
            if (logger != null && task != null)
            {
                if (task.IsFaulted)
                {
                    logger.Error(task.Exception, "Task failed");
                }
                else if (task.IsCanceled)
                {
                    logger.Debug("Task is cancelled.");
                }
                else if (task.IsCompleted)
                {
                    logger.Debug("Task is completed.");
                }
            }
        }

        /// <summary>
        /// The log task.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="task">
        /// The task.
        /// </param>
        /// <typeparam name="TResult">
        /// Result type of task.
        /// </typeparam>
        public static void LogTask<TResult>(this ILogger logger, Task<TResult> task)
        {
            Debug.Assert(logger != null, "logger != null");
            Debug.Assert(task != null, "task != null");
            if (logger != null && task != null)
            {
                if (task.IsFaulted)
                {
                    logger.Error(task.Exception, "Task failed");
                }
                else if (task.IsCanceled)
                {
                    logger.Warning("Task is cancelled.");
                }
                else if (task.IsCompleted)
                {
                    logger.Debug("Task is completed. Result {0}.", task.Result);
                }
            }
        }
    }
}