namespace WebApi.Models.DataModels.Interfaces
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
