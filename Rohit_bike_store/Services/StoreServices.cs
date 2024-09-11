using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;

namespace Rohit_bike_store.Services
{
    public class GetStoreInEachState
    {
        public String State { get; set; }
        public int StoreCount { get; set; }
    }

    public class StoreServices : IStore
    {
        private readonly RohitBikeStoreContext _context;

        public StoreServices(RohitBikeStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Store>> GetAllStores()
        {
            try
            {
                return await _context.Stores.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all stores", ex);
            }
        }

        public async Task<Store> CreateStore(Store store)
        {
            try
            {
                var result = await _context.Stores.AddAsync(store);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on adding store", ex);
            }
        }

        public async Task<Store> UpdateStore(Store store)
        {
            try
            {
                var result = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == store.StoreId);
                if (result != null)
                {
                    result.StoreName = store.StoreName;
                    result.StoreId = store.StoreId;
                    result.Phone = store.Phone;
                    result.Email = store.Email;
                    result.City = store.City;
                    result.State = store.State;
                    result.ZipCode = store.ZipCode;
                    result.Street = store.Street;
                    await _context.SaveChangesAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating store", ex);
            }
        }

        public async Task<List<Store>> GetStoreByCity(String city)
        {
            try
            {
                var result = await _context.Stores.Where(p => p.City.Equals(city)).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting stores by city: {city}", ex);
            }
        }

        public async Task<Store> Delete(int id)
        {
            try
            {
                var del = _context.Stores.FirstOrDefault(d => d.StoreId == id);
                if (del != null)
                {
                    _context.Stores.Remove(del);
                    await _context.SaveChangesAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting store with id: {id}", ex);
            }
        }

        public async Task<List<GetStoreInEachState>> GetNumberOfStoresInEachState()
        {
            try
            {
                var res = await _context.Stores
                    .GroupBy(o => o.State)
                    .Select(s => new GetStoreInEachState
                    {
                        State = s.Key,
                        StoreCount = s.Count()
                    }).ToListAsync();
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting number of stores in each state", ex);
            }
        }

        public async Task<string?> GetMaximumCustomers()
        {
            try
            {
                int ans = await _context.Orders.GroupBy(o => o.StoreId)
                    .Select(g => new { storeId = g.Key, CustomerCount = g.Select(x => x.CustomerId).Distinct().Count() })
                    .OrderByDescending(x => x.CustomerCount)
                    .Select(x => x.storeId).FirstAsync();
                var res = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == ans);
                return res?.StoreName ?? "store not found";
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting store with maximum customers", ex);
            }
        }

        public async Task<string> GetHighestSale()
        {
            try
            {
                int topOrderId = await _context.OrderItems
                    .GroupBy(o => o.OrderId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key).FirstOrDefaultAsync();

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.OrderId == topOrderId);

                var store = await _context.Stores
                    .FirstOrDefaultAsync(o => o.StoreId == order.StoreId);

                return store?.StoreName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting store with highest sale", ex);
            }
        }

        public async Task<bool> PatchUpdateStore(int storeId, Store updatedStore)
        {
            try
            {
                var store = await _context.Stores.FindAsync(storeId);

                if (store == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(updatedStore.StoreName))
                    store.StoreName = updatedStore.StoreName;
                if (!string.IsNullOrEmpty(updatedStore.Phone))
                    store.Phone = updatedStore.Phone;
                if (!string.IsNullOrEmpty(updatedStore.Email))
                    store.Email = updatedStore.Email;
                if (!string.IsNullOrEmpty(updatedStore.Street))
                    store.Street = updatedStore.Street;
                if (!string.IsNullOrEmpty(updatedStore.City))
                    store.City = updatedStore.City;
                if (!string.IsNullOrEmpty(updatedStore.State))
                    store.State = updatedStore.State;
                if (!string.IsNullOrEmpty(updatedStore.ZipCode))
                    store.ZipCode = updatedStore.ZipCode;

                _context.Stores.Update(store);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error patch updating store with id: {storeId}", ex);
            }
        }
    }
}