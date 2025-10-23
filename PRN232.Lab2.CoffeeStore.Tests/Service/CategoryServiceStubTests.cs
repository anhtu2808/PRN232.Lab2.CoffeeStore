using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PRN232.Lab2.CoffeeStore.Models.Exception;
using PRN232.Lab2.CoffeeStore.Models.Response.Category;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Service;

namespace CoffeeStore.Tests.Service;

/// <summary>
/// Test class cho CategoryService sử dụng Stub pattern
/// Stub: Cung cấp dữ liệu giả cố định, không verify behavior
/// </summary>
public class CategoryServiceStubTests
{
    
    private readonly IMapper _mapper;

    
    public CategoryServiceStubTests()
    {
        // Tạo AutoMapper configuration
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Category, CategoryResponse>();
        }, NullLoggerFactory.Instance);

        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsMappedCategory()
    {
        // ===== ARRANGE =====
        // Chuẩn bị dữ liệu và stub các dependencies

        // 1. Tạo dữ liệu fake - đây là dữ liệu mà Repository stub sẽ trả về
        var stubCategory = new Category 
        { 
            CategoryId = 1, 
            Name = "Coffee",
            Description = "Coffee beverages"
        };

        // 2. Tạo STUB cho ICategoryRepository
        // Stub này sẽ giả lập database và trả về dữ liệu cố định
        var stubCategoryRepo = new Mock<ICategoryRepository>();
        stubCategoryRepo
            .Setup(repo => repo.GetByIdAsync(1))  // Khi gọi GetByIdAsync với id = 1
            .ReturnsAsync(stubCategory);           // Luôn trả về stubCategory

        // 3. Tạo STUB cho IUnitOfWork
        // Stub này sẽ trả về stubCategoryRepo khi truy cập property Categories
        var stubUnitOfWork = new Mock<IUnitOfWork>();
        stubUnitOfWork
            .Setup(uow => uow.Categories)          // Khi truy cập property Categories
            .Returns(stubCategoryRepo.Object);     // Trả về stub repository

        // 4. Tạo instance của CategoryService với các dependencies đã stub
        // Service này sẽ sử dụng dữ liệu giả thay vì database thật
        var categoryService = new CategoryService(stubUnitOfWork.Object, _mapper);

        // ===== ACT =====
        // Thực thi method cần test
        var result = await categoryService.GetByIdAsync(1);

        // ===== ASSERT =====
        // Kiểm tra kết quả có đúng như mong đợi không
        Assert.NotNull(result);                    // Kết quả không null
        Assert.Equal(1, result.CategoryId);        // CategoryId được map đúng
        Assert.Equal("Coffee", result.Name);       // Name được map đúng
        Assert.Equal("Coffee beverages", result.Description); // Description được map đúng

        // LƯU Ý: Với Stub, chúng ta KHÔNG verify xem GetByIdAsync có được gọi hay không
        // Stub chỉ quan tâm đến OUTPUT, không quan tâm đến BEHAVIOR
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ThrowsAppException()
    {
        // ===== ARRANGE =====
    
        // 1. Stub trả về null khi tìm category không tồn tại
        var stubCategoryRepo = new Mock<ICategoryRepository>();
        stubCategoryRepo
            .Setup(repo => repo.GetByIdAsync(999))  // Id không tồn tại
            .ReturnsAsync((Category?)null);          // Trả về null

        // 2. Tạo stub UnitOfWork
        var stubUnitOfWork = new Mock<IUnitOfWork>();
        stubUnitOfWork
            .Setup(uow => uow.Categories)
            .Returns(stubCategoryRepo.Object);

        // 3. Tạo service instance
        var categoryService = new CategoryService(stubUnitOfWork.Object, _mapper);

        // ===== ACT & ASSERT =====
        // Verify rằng AppException được throw với message đúng
        var exception = await Assert.ThrowsAsync<AppException>(
            async () => await categoryService.GetByIdAsync(999)
        );
    
        // Kiểm tra message của exception
        Assert.Equal("Category not found.", exception.Message);
    }


    [Fact]
    public async Task GetByIdAsync_WithMultipleCalls_AlwaysReturnsSameStubData()
    {
        // ===== ARRANGE =====
        // Test này chứng minh đặc điểm của Stub: luôn trả về dữ liệu cố định

        var stubCategory = new Category 
        { 
            CategoryId = 1, 
            Name = "Coffee" 
        };

        // Stub luôn trả về cùng một object
        var stubCategoryRepo = new Mock<ICategoryRepository>();
        stubCategoryRepo
            .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))  // Với bất kỳ id nào
            .ReturnsAsync(stubCategory);                         // Đều trả về stubCategory

        var stubUnitOfWork = new Mock<IUnitOfWork>();
        stubUnitOfWork
            .Setup(uow => uow.Categories)
            .Returns(stubCategoryRepo.Object);

        var categoryService = new CategoryService(stubUnitOfWork.Object, _mapper);

        // ===== ACT =====
        // Gọi method nhiều lần
        var result1 = await categoryService.GetByIdAsync(1);
        var result2 = await categoryService.GetByIdAsync(2);
        var result3 = await categoryService.GetByIdAsync(99);

        // ===== ASSERT =====
        // Tất cả đều trả về cùng dữ liệu vì sử dụng stub
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.Equal("Coffee", result1.Name);
        Assert.Equal("Coffee", result2.Name);  // Giống result1
        Assert.Equal("Coffee", result3.Name);  // Giống result1

        // Lưu ý: Đây là đặc điểm của Stub - dữ liệu cố định
        // Trong thực tế, id khác nhau nên trả về category khác nhau
    }

    [Fact]
    public async Task GetByIdAsync_CategoryWithNullDescription_MapsCorrectly()
    {
        // ===== ARRANGE =====
        // Test mapping với dữ liệu có field null

        var stubCategory = new Category 
        { 
            CategoryId = 5, 
            Name = "Tea",
            Description = null  // Description null
        };

        var stubCategoryRepo = new Mock<ICategoryRepository>();
        stubCategoryRepo
            .Setup(repo => repo.GetByIdAsync(5))
            .ReturnsAsync(stubCategory);

        var stubUnitOfWork = new Mock<IUnitOfWork>();
        stubUnitOfWork
            .Setup(uow => uow.Categories)
            .Returns(stubCategoryRepo.Object);

        var categoryService = new CategoryService(stubUnitOfWork.Object, _mapper);

        // ===== ACT =====
        var result = await categoryService.GetByIdAsync(5);

        // ===== ASSERT =====
        Assert.NotNull(result);
        Assert.Equal(5, result.CategoryId);
        Assert.Equal("Tea", result.Name);
        Assert.Null(result.Description);  // Description vẫn null sau khi map
    }
}