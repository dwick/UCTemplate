namespace UCTemplate.Web.Mvc.Infrastructure.Commands
{
    #region using

    using System;

    #endregion

    /// <summary>
    /// Command contract.
    /// </summary>
    public interface ICommand
    {
        Guid Id { get; }
        void Execute();
        bool Successful { get; }
    }
    public interface ICommand<T>
    {
        Guid Id { get; }
        T Execute();
        bool Successful { get; }
    }
}
