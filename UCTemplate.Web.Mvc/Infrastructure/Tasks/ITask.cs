namespace UCTemplate.Web.Mvc.Infrastructure.Tasks
{
    #region using

    using System;

    #endregion

    /// <summary>
    /// Task contract.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// When to execute the task.
        /// </summary>
        DateTime ExecutionTime { get; }
        void Execute();
    }
}