using AutoMapper;
using Rohit_bike_store.Models;
using Rohit_bike_store.Service;
using Rohit_bike_store.DTO;
using Microsoft.EntityFrameworkCore;

namespace Rohit_bike_store.Service
{
    public class CategoryServices : ICategory
    {
        private readonly RohitBikeStoreContext _context;
        private readonly IMapper _mapper;

        public CategoryServices(RohitBikeStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // HttpsPost
        public async Task<CategoryDto> Post(CategoryDto categoryDto)

        {
                var category = _mapper.Map<Category>(categoryDto);
                var result = _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return _mapper.Map<CategoryDto>(result.Entity);
           
        }

        //HttpsGet
        public async Task<List<CategoryDto>> Get()
        {            
                var categories = await _context.Categories.ToListAsync();
                return _mapper.Map<List<CategoryDto>>(categories);           
        }

        //HttpsPut
        public async Task<CategoryDto> Put(int id, CategoryDto category)
        {
            try
            {
                var result = await _context.Categories.FindAsync(id);
                if (result != null)
                {
                    result.CategoryName = category.CategoryName;
                    await _context.SaveChangesAsync();
                    return _mapper.Map<CategoryDto>(result);
                }
                return null;
            }

            catch (Exception ex)
            {
                throw new Exception("Errors on update the data in category");
            }
        }

        //HttpGetByCategoryName
        public async Task<List<CategoryDto>> GetCategoryByCategoryName(string categoryName)
        {
            try
            {
                var categories = await _context.Categories.Where(c => c.CategoryName == categoryName).ToListAsync();
                return _mapper.Map<List<CategoryDto>>(categories);
            }

            catch (Exception ex)
            {
                throw new Exception("Erors on retrive the category name");
            }
        }
    }
}

