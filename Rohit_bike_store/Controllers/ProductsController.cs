using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProduct p;
        public ProductsController(IProduct p)
        {
            this.p = p;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetAllProducts()
        {
            try
            {
                var result = await p.GetAllProducts();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("bybrandname/{BrandName}")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProductByBrandName(string BrandName)
        {
            try
            {
                var product = await p.GetProductByBrandName(BrandName);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("bycategoryname/{CategoryName}")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetProductByCategoryName(string CategoryName)
        {
            try
            {
                var products = await p.GetProductByCategoryName(CategoryName);
                if (products == null || !products.Any())
                {
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("bymodelyear/{ModelYear}")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetProductByModelyear(int ModelYear)
        {
            try
            {
                var products = await p.GetProductByModelYear(ModelYear);
                if (products == null || !products.Any())
                {
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("purchasedbycustomer/{customerid}")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetProductByCustomerId(int customerid)
        {
            try
            {
                var products = await p.GetProductByCustomerId(customerid);
                if (products == null || !products.Any())
                {
                    return NotFound();
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductDetails")]
        [Authorize]
        public async Task<ActionResult> GetProductNameCategoryNameBrandName()
        {
            try
            {
                var result = await p.GetProductNameCategoryNameBrandName();
                if (result == null || !result.Any())
                {
                    return NotFound("No product details found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ProductPurchasedByMaximumCustomer")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProductPurchasedByMaxCustomer()
        {
            try
            {
                var product = await p.ProductPurchasedByMaximumCustomer();
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("numberofproductssoldbyeachstore")]
        [Authorize]
        public async Task<IEnumerable<Object>> GetnumberofProductsSoldByeachStore()
        {
            try
            {
                return await p.GetNumberOfProductsByEachStore();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddProduct(Product product)
        {
            try
            {
                await p.AddProduct(product);
                return CreatedAtAction(nameof(GetAllProducts), new { id = product.ProductId }, new { Message = "Record added successfully!", InsertedRecord = product });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("edit/{productid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateProduct(int productid, Product product)
        {
            try
            {
                if (productid != product.ProductId)
                {
                    return BadRequest("Product ID mismatch.");
                }

                var updatedProduct = await p.UpdateProduct(product);
                if (updatedProduct == null)
                {
                    return NotFound();
                }
                return Ok(updatedProduct);
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        [HttpPatch("edit/{productid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> UpdateParticularInfo(ProductDto productdto)
        {
            try
            {
                var updatedProduct = await p.UpdateParticularProductInfo(productdto);
                if (updatedProduct == null)
                {
                    return NotFound();
                }
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
