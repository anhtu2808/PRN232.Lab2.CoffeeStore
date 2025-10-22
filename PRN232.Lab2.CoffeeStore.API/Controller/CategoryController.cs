using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.Lab2.CoffeeStore.Models.Request.Category;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.API.Controller;

[ApiController]
[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] RequestParameters parameters)
    {
        var result = await _categoryService.GetAllAsync(parameters);
        var response = new ApiResponse<PageResponse<CategoryResponse>>
        {
            StatusCode = 200,
            Message = "Categories retrieved successfully.",
            Data = result
        };
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category == null)
        {
            return NotFound(new ApiResponse<object>
            {
                StatusCode = 404,
                Message = $"Category with ID {id} not found.",
                Data = null
            });
        }

        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Category retrieved successfully.",
            Data = category
        };
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var created = await _categoryService.CreateAsync(request);
        var response = new ApiResponse<object>
        {
            StatusCode = 201,
            Message = "Category created successfully.",
            Data = created
        };
        return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
       var category = await _categoryService.UpdateAsync(id, request);
        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Category updated successfully.",
            Data = category
        };
        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        var response = new ApiResponse<object>
        {
            StatusCode = 200,
            Message = "Category deleted successfully.",
            Data = null
        };
        return Ok(response);
    }
}
