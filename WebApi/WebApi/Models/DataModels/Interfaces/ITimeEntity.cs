namespace WebApi.Models.DataModels.Interfaces
{
    public interface ITimeEntity
    {
        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
