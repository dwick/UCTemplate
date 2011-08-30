namespace UCTemplate.Web.Mvc.Models
{
    #region using

    using System;

    using DomainModel;

    #endregion

    /// <summary>
    /// Log entity
    /// </summary>
    public class Log : IEntityWithTypedId<Guid>
    {
        public Guid Id { get; protected set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}