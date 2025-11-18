namespace OnionTemplate.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        void UpdateStock(Guid catgoeryId, Guid productId, int quantity);
        Task<bool> Delete(Guid categoryId);
    }
}
