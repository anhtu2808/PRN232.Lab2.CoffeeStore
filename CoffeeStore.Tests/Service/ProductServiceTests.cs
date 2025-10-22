using AutoMapper;
using FluentAssertions;
using Moq;
using PRN232.Lab2.CoffeeStore.Models.Response.Product;
using PRN232.Lab2.CoffeeStore.Repositories.Entity;
using PRN232.Lab2.CoffeeStore.Repositories.IRepository;
using PRN232.Lab2.CoffeeStore.Repositories.UnitOfWork;
using PRN232.Lab2.CoffeeStore.Services.Service;

namespace CoffeeStore.Tests.Service
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly ProductService _productService;
        private readonly Mock<IMapper> _mapperMock;

        public ProductServiceTests()
        {
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            _productRepoMock = new Mock<IProductRepository>();
            _mapperMock = new Mock<IMapper>(); // ✅ Khởi tạo mock trước khi setup

            _mapperMock
                .Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
                .Returns((Product src) => new ProductResponse
                {
                    ProductId = src.ProductId,
                    Name = src.Name,
                    Price = src.Price,
                    Description = src.Description,
                    IsActive = src.IsActive
                });

            unitOfWorkMock.Setup(u => u.Products).Returns(_productRepoMock.Object);

            _productService = new ProductService(unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductResponse_WhenProductExists()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 1,
                Name = "Latte",
                Price = 50,
                IsActive = true,
                Description = "Milk Coffee"
            };

            _productRepoMock
                .Setup(r => r.GetFirstOrDefaultAsync(p => p.ProductId == 1, "Category"))
                .ReturnsAsync(product);

            // Act
            var result = await _productService.GetProductByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductId, result.ProductId);
            Assert.Equal(product.Name, result.Name);
        }
    }
}
