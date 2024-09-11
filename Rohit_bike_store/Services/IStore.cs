using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public interface IStore
    {
        Task<List<Store>> GetAllStores();
        Task<Store> CreateStore(Store store);
        Task<Store> UpdateStore(Store store);
        Task<List<Store>> GetStoreByCity(string city);
        Task<List<GetStoreInEachState>> GetNumberOfStoresInEachState();
        Task<string> GetMaximumCustomers();
        Task<string> GetHighestSale();
        Task<bool> PatchUpdateStore(int storeId, Store store);
        Task<Store> Delete(int id);
    }
}
