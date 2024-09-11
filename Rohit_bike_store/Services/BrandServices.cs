using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public class BrandServices : IBrand
    {
        private readonly RohitBikeStoreContext _context;
        private readonly IMapper _mapper;

        public BrandServices(RohitBikeStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Get
        public async Task<IEnumerable<BrandDto>> GetAllBrands()
        {
            var brands = await _context.Brands.ToListAsync();


            var brandDtos = _mapper.Map<IEnumerable<BrandDto>>(brands);


            return brandDtos;
        }


        //Put
        public async Task<BrandDto> Put(int id, BrandDto brandDto)
        {
            var brand = await _context.Brands.FindAsync(id);


            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found.");
            }


            _mapper.Map(brandDto, brand);

            _context.Brands.Update(brand);


            await _context.SaveChangesAsync();

            return _mapper.Map<BrandDto>(brand);
        }


        //Post
        public async Task<BrandDto> Post(BrandDto brandDto)
        {
            // Check for duplicate BrandId
            if (_context.Brands.Any(b => b.BrandId == brandDto.BrandId))
            {
                throw new InvalidOperationException($"A brand with the ID {brandDto.BrandId} already exists.");
            }

            var brand = _mapper.Map<Brand>(brandDto);
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return _mapper.Map<BrandDto>(brand);
        }

    }
}
