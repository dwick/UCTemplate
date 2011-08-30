namespace UCTemplate.Web.Mvc.Models.DomainModel
{
    #region using

    using System;

    #endregion

    public class Entity : IEntityWithTypedId<Guid>, IEntityWithAudit
    {
        public Guid Id { get; protected set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Modified { get; set; }
    }
}