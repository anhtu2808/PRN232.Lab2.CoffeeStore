using AutoMapper;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Request.Common;
using PRN232.Lab2.CoffeeStore.Models.Request.Product;
using PRN232.Lab2.CoffeeStore.Models.Response.Common;
using PRN232.Lab2.CoffeeStore.Models.Response.Product;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.IService;

namespace PRN232.Lab2.CoffeeStore.Services.Service;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PageResponse<ProductResponse>> GetProductsAsync(ProductFilter filter)
    {
        var pagedProducts = await _unitOfWork.Products.GetPagedProductsAsync(
            filter
        );

        var productResponses = _mapper.Map<List<ProductResponse>>(pagedProducts.Items);

        return new PageResponse<ProductResponse>(
            productResponses,
            pagedProducts.TotalCount,
            pagedProducts.Page,
            pagedProducts.PageSize
        );
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        var product =
            await _unitOfWork.Products.GetFirstOrDefaultAsync(p => p.ProductId == id, includeProperties: "Category");
        if (product is null)
        {
            throw new AppException(ErrorCode.ProductNotFound);
        }

        return _mapper.Map<ProductResponse>(product);
    }

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var productEntity = _mapper.Map<Product>(request);
        productEntity.IsActive = true;
        var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
        if (category is null)
        {
            throw new AppException(ErrorCode.CategoryNotFound);
        }

        await _unitOfWork.Products.AddAsync(productEntity);

        await _unitOfWork.CompleteAsync();

        return _mapper.Map<ProductResponse>(productEntity);
    }

    public async Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
        if (request.CategoryId != null)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId.Value);
            if (category is null)
            {
                throw new AppException(ErrorCode.CategoryNotFound);
            }
        }
       
        if (existingProduct is null)
        {
            throw new AppException(ErrorCode.ProductNotFound);
        }

        _mapper.Map(request, existingProduct);

        await _unitOfWork.Products.UpdateAsync(existingProduct);
        await _unitOfWork.CompleteAsync();
        return _mapper.Map<ProductResponse>(existingProduct);
    }

    public async Task DeleteProductAsync(int id)
    {
        var productToDelete = await _unitOfWork.Products.GetByIdAsync(id);
        if (productToDelete is null)
        {
            throw new AppException(ErrorCode.ProductNotFound);
        }

        if (productToDelete.IsActive == false)
        {
            throw new AppException(ErrorCode.ProductAlreadyInactive);
        }

        await _unitOfWork.Products.DeleteAsync(productToDelete);
        await _unitOfWork.CompleteAsync();
    }
}