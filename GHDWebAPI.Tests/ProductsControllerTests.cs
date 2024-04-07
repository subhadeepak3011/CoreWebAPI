using Castle.Core.Logging;
using GHDWebAPI.Controllers;
using GHDWebAPI.Data;
using GHDWebAPI.Model;
using GHDWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace GHDWebAPI.Tests
{
    /// <summary>
    /// Class to run units tests
    /// </summary>
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> productServiceMock;
        private readonly Mock<ILogger<ProductsController>> mockLogger;

        /// <summary>
        /// List of products for mock testing
        /// </summary>
        private List<Product> GetProducts()
        {
            List<Product>  products = new List<Product>()
            {
                new Product { Id = 1, Name ="Television", Brand ="Sony", Price=2600},
                new Product { Id = 2, Name = "Washing Machine", Brand = "LG", Price = 700},
                new Product { Id = 3, Name ="Heater", Brand ="Havells", Price=89.90},
                new Product { Id = 4, Name ="Dryer", Brand ="Dyson", Price=67},
                new Product { Id = 5, Name ="Shoes", Brand ="Nike", Price=145},
                new Product { Id = 6, Name = "Sofa", Brand = "Harvey Norman", Price = 2500},
                new Product { Id = 7, Name = "Chair", Brand = "Recliner", Price = 1456.56},
                new Product { Id = 8, Name = "Microwave", Brand = "LG", Price = 700.45},
                new Product { Id = 9, Name = "Refrigerator", Brand = "Samsung", Price = 1300},
                new Product { Id = 10, Name = "Solar", Brand = "Havells", Price = 5600},
                new Product { Id = 11, Name = "Laptop", Brand = "HP", Price = 1999},
            };

            return products;
        }

        /// <summary>
        /// Initialise
        /// </summary>
        public ProductsControllerTests()
        {
            productServiceMock= new Mock<IProductService>();
            mockLogger = new Mock<ILogger<ProductsController>>();
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [Fact]
        public void GetProducts_ReturnAllProducts()
        {
            //... arrange
            var allProducts = GetProducts();
            productServiceMock.Setup(x => x.GetAll()).Returns(allProducts); 
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);  

            //... act
            var productResult = productController.GetAllProducts();

            //assert
            Assert.NotNull(productResult);
            Assert.Equal(GetProducts().Count(), productResult.Count());
            Assert.Equal(GetProducts().ToString(), productResult.ToString());
            Assert.True(allProducts.Equals(productResult));
        }

        /// <summary>
        /// Get product by Id
        /// </summary>
        [Fact]
        public void GetProductByID_ReturnSingleProduct()
        {
            //... arrange
            var productList = GetProducts();
            productServiceMock.Setup(x => x.GetById(2)).Returns(productList[1]);
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            //... act
            var productResult = productController.GetProduct(2);

            //... assert
            Assert.NotNull(productResult.Value);
            Assert.Equal(productList[1].Id, productResult.Value.Id);
            Assert.True(productList[1].Id == productResult.Value.Id);
        }

        /// <summary>
        /// Failure to add as product is not unique
        /// </summary>
        [Fact]
        public void AddProduct_FailedToAdd()
        {
            //... arrange
            var productList = GetProducts();
 
            productServiceMock.Setup(x => x.GetById(5)).Returns(productList[4]);
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            //... act
            var productResult = productController.PostProduct(productList[4]);
            var createdProduct = productResult as ActionResult<Product>;

            //... assert
            Assert.True(createdProduct.Result is BadRequestObjectResult);
        }

        /// <summary>
        /// Deletion successful
        /// </summary>
        [Fact]
        public void DeleteProduct_Success()
        {
            //... arrange
            int validId = 5;

            var productList = GetProducts();

            productServiceMock.Setup(x => x.GetById(5)).Returns(productList[4]);
           
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            //... act
            var successResult = productController.DeleteProduct(validId);

            //... assert
            Assert.True(successResult is NoContentResult);   

        }

        /// <summary>
        /// Deletion failed
        /// </summary>
        [Fact]
        public void DeleteProduct_Failure()
        {
            //... arrange
            int invalidId = 56;

            var productList = GetProducts();

            productServiceMock.Setup(x => x.GetById(5)).Returns(productList[4]);

            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            //... act
            var errorResult = productController.DeleteProduct(invalidId);

            //... assert
            Assert.True(errorResult is BadRequestObjectResult);
        }

        /// <summary>
        /// Check if product exists by name
        /// </summary>
        /// <param name="productName"></param>
        [Theory]
        [InlineData("Television")]
        public void CheckProductExistOrNotByProductName_Product(string productName)
        {
            //... arrange
            var productList = GetProducts();
            productServiceMock.Setup(x => x.GetAll())
                .Returns(productList);
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            //... act
            var productResult = productController.GetAllProducts();
            var expectedProductName = productResult.ToList()[0].Name;

            //... assert
            Assert.Equal(productName, expectedProductName);
        }

        /// <summary>
        /// Update single product by id
        /// </summary>
        [Fact]
        public void UpdateProductByID_UpdatesSingleProduct()
        {
            //... arrange
            var productList = GetProducts();
            productServiceMock.Setup(x => x.GetById(2)).Returns(productList[1]);
            var productController = new ProductsController(productServiceMock.Object, mockLogger.Object);

            productList[1].Name = "ChangedName";
            //... act
            var productResult = productController.PutProduct(2, productList[1]);

            //... assert
            Assert.True(productResult.Result is NoContentResult);

        }

    }
}