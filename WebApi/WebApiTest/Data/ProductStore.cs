using WebApi.Models.DataModels;
using WebApiTest.Unit;

namespace WebApiTest.Data
{
    public static class ProductStore
    {
        private static readonly Product[] products = new Product[]
        {
            new Product { Id = (Guid)ProductStoreUnitTests.Good.First().First(), Name = "test1"},
            new Product { Id = (Guid)ProductStoreUnitTests.Good.Last().First(), Name = "test2"},
        };

        public static IEnumerable<Product> GetProducts()
        {
            return products;
        }

        public static Product? GetProduct(Guid id)
        {
            foreach (var product in products)
            {
                if (product.Id == id)
                    return product;
            }
            return null;
        }
    }
}
