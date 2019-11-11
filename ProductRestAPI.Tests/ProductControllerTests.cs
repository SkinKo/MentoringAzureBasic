using Moq;
using NUnit.Framework;
using ProductRestAPI.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class ProductControllerTests
    {
        private Mock<IProductsRepository> _repository;
        private ProductRestAPI.Controllers.ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IProductsRepository>();
            _repository.Setup(x => x.Get()).Returns(new List<ProductRestAPI.Models.Product>
            {
                new ProductRestAPI.Models.Product { Id = 1, Name = "First Product" },
                new ProductRestAPI.Models.Product { Id = 2, Name = "Second Product" },
                new ProductRestAPI.Models.Product { Id = 3, Name = "Last Product" },
            });
            _repository.Setup(x => x.Get(2)).Returns(new ProductRestAPI.Models.Product { Id = 2, Name = "Second Product" });

            _controller = new ProductRestAPI.Controllers.ProductsController(_repository.Object);
        }

        [Test]
        public void GetAllProducts()
        {
            var products = _controller.Get().Value;

            Assert.AreEqual(products.Count(), 3);
        }

        [Test]
        public void GetSecondProduct()
        {
            var product = _controller.Get(2).Value;

            Assert.AreEqual(product.Name, "Second Product");
        }

        [Test]
        public void TryGetZeroProduct()
        {
            Assert.Throws<KeyNotFoundException>(() => _controller.Get(0));
        }

        [Test]
        public void AddProduct()
        {
            _controller.Post(new ProductRestAPI.Models.Product { Id = 556 });

            _repository.Verify(x => x.Add(It.Is<ProductRestAPI.Models.Product>(y => y.Id == 556)), Times.Once);
        }

        [Test]
        public void UpdateProduct()
        {
            _controller.Put(4, new ProductRestAPI.Models.Product { Id = 4 });

            _repository.Verify(x => x.Update(4, It.Is<ProductRestAPI.Models.Product>(y => y.Id == 4)), Times.Once);
        }

        [Test]
        public void DeleteProduct()
        {
            _controller.Delete(4);

            _repository.Verify(x => x.Delete(4), Times.Once);
        }
    }
}