namespace UCTemplate.Web.Mvc.Models.DomainModel
{
    #region using

    using System;

    #endregion

    public interface IEntityWithTombstone
    {
        DateTimeOffset? Deleted { get; }
        bool IsDeleted { get; }

        void Delete();
        void Recover();
    }
}