namespace UCTemplate.Web.Mvc.Infrastructure.EntityFramework
{
    #region using

    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;

    using Configuration;
    using Models;
    using Models.DomainModel;

    #endregion

    /// <summary>
    /// Accounts db
    /// </summary>
    public class UCTemplateContext : DbContext
    {
        public UCTemplateContext()
            : base("UCTemplate")
        {
            Database.SetInitializer<UCTemplateContext>(null);
        }

        /// <summary>
        /// Read-only application logs.
        /// </summary>
        public DbSet<Log> Logs
        {
            get { return Set<Log>(); }
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new LogConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Automatically updates audit information
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            var entriesToAudit = ChangeTracker.Entries()
                .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified)
                    && typeof(IEntityWithAudit).IsAssignableFrom(x.Entity.GetType()));

            foreach (var entry in entriesToAudit)
            {
                var entity = entry.Entity as IEntityWithAudit;
                if (entity != null && entry.State == EntityState.Added)
                {
                    entity.Created = DateTimeOffset.Now;
                    entity.Modified = DateTimeOffset.Now;
                }
                else if (entity != null && entry.State == EntityState.Modified)
                {
                    entity.Modified = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
    }
}