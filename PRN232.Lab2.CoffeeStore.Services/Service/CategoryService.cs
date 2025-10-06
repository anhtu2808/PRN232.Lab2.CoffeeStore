using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.Category;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.Services.Service;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PageResponse<CategoryResponse>> GetAllAsync(RequestParameters parameters)
    {
        var categories = await _unitOfWork.Categories.GetPagedListAsync(parameters);
        return _mapper.Map<PageResponse<CategoryResponse>>(categories);
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            throw new AppException(ErrorCode.CategoryNotFound);
        }

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            throw new AppException(ErrorCode.CategoryNotFound);
        }
        _mapper.Map(request, category);
        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null)
        {
            throw new AppException(ErrorCode.CategoryNotFound);
        }
        await _unitOfWork.Categories.DeleteAsync(category);
        await _unitOfWork.CompleteAsync();
    }
}