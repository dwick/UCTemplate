namespace UCTemplate.Web.Mvc.Models.DomainModel
{
    #region using

    using System;

    #endregion

    public class Aggregate : Entity, IEntityWithTombstone
    {
        public DateTimeOffset? Deleted { get; protected set; }

        public bool IsDeleted { get; protected set; }

        public void Delete()
        {
            if (IsDeleted) return; 

            IsDeleted = true;
            Deleted = DateTimeOffset.Now;
        }
        public void Recover()
        {
            IsDeleted = false;
            Deleted = null;
        }
    }
}