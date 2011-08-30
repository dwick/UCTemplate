namespace UCTemplate.Web.Mvc.Models.DomainModel
{
    public interface IEntityWithTypedId<T>
    {
        T Id { get; }
    }
}