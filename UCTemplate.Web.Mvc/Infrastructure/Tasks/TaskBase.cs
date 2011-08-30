namespace UCTemplate.Web.Mvc.Infrastructure.Tasks
{
    #region using

    using System;

    using log4net;

    #endregion
    
    public abstract class TaskBase : ITask
    {
        protected readonly ILog Log;

        public DateTime ExecutionTime { get; private set; }

        protected TaskBase(DateTime executionTime)
        {
            Log = LogManager.GetLogger(GetType());
            ExecutionTime = executionTime;
        }

        public abstract void Execute();
    }
}