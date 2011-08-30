namespace UCTemplate.Web.Mvc.Infrastructure.Commands
{
    #region using

    using System;
    using System.Threading.Tasks;

    using log4net;

    #endregion

    /// <summary>
    /// Async command execution.
    /// </summary>
    public static class CommandExecutor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandExecutor));
        /// <summary>
        /// Executes the supplied command asynchronously.
        /// </summary>
        /// <param name="command">The command to execute.</param>
        public static void Excute(ICommand command)
        {
            Task.Factory
                .StartNew(() =>
                              {
                                  Log.Debug(string.Format("Executing '{0}'.", command.GetType()));
                                  command.Execute();
                              }, TaskCreationOptions.LongRunning)
                .ContinueWith(task => Log.Fatal(string.Format("'{0}', failed.", command.GetType()), task.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }

    public static class CommandExecutor<T>
    {
// ReSharper disable StaticFieldInGenericType
        private static readonly ILog Log = LogManager.GetLogger(typeof(CommandExecutor));
// ReSharper restore StaticFieldInGenericType

        public static void Excute(ICommand<T> command, Action<T> success = null)
        {
            var task = Task.Factory
                .StartNew(() =>
                              {
                                  Log.Debug(string.Format("Executing '{0}'.", command.GetType()));
                                  return command.Execute();
                              }, TaskCreationOptions.LongRunning);

            task.ContinueWith(t =>
                                  {
                                      if (success != null) success(t.Result);
                                  }, TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => Log.Fatal(string.Format("'{0}', failed.", command.GetType()), t.Exception), TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
