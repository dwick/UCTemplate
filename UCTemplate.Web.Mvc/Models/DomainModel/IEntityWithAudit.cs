namespace UCTemplate.Web.Mvc.Models.DomainModel
{
    #region using

    using System;

    #endregion

    public interface IEntityWithAudit
    {
        DateTimeOffset Created { get; set; }
        DateTimeOffset Modified { get; set; }
    }
}