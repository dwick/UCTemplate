namespace UCTemplate.Web.Mvc.Infrastructure.Commands
{
    #region using

    using System;

    using log4net;

    #endregion
    
    public abstract class CommandBase : ICommand
    {
        protected readonly ILog Log;

        protected CommandBase()
        {
            Log  = LogManager.GetLogger(GetType());
        }

        public abstract void Execute();

        public Guid Id { get; protected set; }
        public bool Successful { get; protected set; }
    }

    public abstract class CommandBase<T> : ICommand<T>
    {
        protected readonly ILog Log;

        protected CommandBase()
        {
            Log = LogManager.GetLogger(GetType());
        }

        public abstract T Execute();

        public Guid Id { get; protected set; }
        public bool Successful { get; protected set; }
    }
}