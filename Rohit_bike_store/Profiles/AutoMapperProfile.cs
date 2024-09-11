using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using AutoMapper;

namespace Rohit_bike_store.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Brand, BrandDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Stock, StockDto>().ReverseMap();
            CreateMap<OrderItem, AddOrderItemDto>().ReverseMap();
            
        }
    }
    
}
