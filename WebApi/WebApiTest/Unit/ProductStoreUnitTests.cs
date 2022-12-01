using WebApi.Models.DataModels;
using WebApiTest.Data;

namespace WebApiTest.Unit
{
    public class ProductStoreUnitTests
    {
        [Fact]
        public void GetProducts_WhenExecuted_ReturnsListOfProducts()
        {
            var products = ProductStore.GetProducts();
            Assert.IsAssignableFrom<IEnumerable<Product>>(products);
        }

        [Theory, MemberData(nameof(Good))]
        public void GetProduct_WithExistingId_ReturnsAProduct(Guid id)
        {
            var product = ProductStore.GetProduct(id);
            Assert.NotNull(product);
            Assert.Equal(id, product?.Id);
            Assert.IsAssignableFrom<Product>(product);
        }

        [Theory, MemberData(nameof(Bad))]
        public void GetAProduct_WithNonExistingId_ReturnsNull(Guid id)
        {
            var product = ProductStore.GetProduct(id);
            Assert.Null(product);
        }

        static readonly Guid a = Guid.NewGuid();
        static readonly Guid b = Guid.NewGuid();
        static readonly Guid c = Guid.NewGuid();
        static readonly Guid d = Guid.NewGuid();

        public static IEnumerable<object[]> Good
        {
            get
            {
                yield return new object[] { a };
                yield return new object[] { b };
            }
        }
        public static IEnumerable<object[]> Bad
        {
            get
            {
                yield return new object[] { c };
                yield return new object[] { d };
            }
        }
    }
}
