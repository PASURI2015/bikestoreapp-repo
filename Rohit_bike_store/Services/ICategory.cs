using Rohit_bike_store.DTO;


namespace Rohit_bike_store.Service
{
    public interface ICategory
    {
        public Task<CategoryDto> Put(int id, CategoryDto category);
        public Task<CategoryDto> Post(CategoryDto category);
        public Task<List<CategoryDto>> Get();
        public Task<List<CategoryDto>> GetCategoryByCategoryName(string categoryName);
    }

}