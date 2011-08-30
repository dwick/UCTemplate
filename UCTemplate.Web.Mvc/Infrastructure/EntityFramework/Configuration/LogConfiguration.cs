namespace UCTemplate.Web.Mvc.Infrastructure.EntityFramework.Configuration
{
    #region using

    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.ModelConfiguration;

    using Models;

    #endregion

    /// <summary>
    /// Entity configuration for the Log entity.
    /// </summary>
    public class LogConfiguration : EntityTypeConfiguration<Log>
    {
        public LogConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Date)
                .IsRequired();

            Property(x => x.Exception)
                .IsOptional()
                .IsVariableLength().IsUnicode(false).HasColumnType("varchar(MAX)");

            Property(x => x.Level)
                .IsRequired()
                .IsVariableLength().IsUnicode(false).HasMaxLength(20);

            Property(x => x.Logger)
                .IsRequired()
                .IsVariableLength().IsUnicode(false).HasMaxLength(255);

            Property(x => x.Message)
                .IsRequired()
                .IsVariableLength().IsUnicode(false).HasColumnType("varchar(MAX)");

            ToTable("Logs");
        }

    }
}