using Rohit_bike_store.DTO;
namespace Rohit_bike_store.Services
{
    public interface IBrand
    {
        public Task<IEnumerable<BrandDto>> GetAllBrands();

        public Task<BrandDto> Put(int id, BrandDto brandDto);

        public Task<BrandDto> Post(BrandDto brandDto);
    }
}
