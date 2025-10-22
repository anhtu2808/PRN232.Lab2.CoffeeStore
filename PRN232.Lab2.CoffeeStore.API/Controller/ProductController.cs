using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Services.IService;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilter filter)
    {
        var pagedProducts = await _productService.GetProductsAsync(filter);
        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Products retrieved successfully.",
            Data = pagedProducts
        };
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);

        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Product retrieved successfully.",
            Data = product
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var newProduct = await _productService.CreateProductAsync(request);
        var response = new ApiResponse<object>
        {
            StatusCode = 201,
            Message = "Product created successfully.",
            Data = newProduct
        };
        return CreatedAtAction(nameof(GetProductById), new { id = newProduct.ProductId }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
       var product =  await _productService.UpdateProductAsync(id, request);
        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Product updated successfully.",
            Data = product
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);
        return Ok(new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Product deleted successfully.",
            Data = null
        });
    }
}