namespace WebApi.Models.DataModels.Interfaces
{
    public interface IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
