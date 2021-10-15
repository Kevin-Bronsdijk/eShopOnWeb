using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Moq;

namespace Microsoft.eShopWeb.UnitTests.Services.Scenarios
{
    public static class Given
    {
        public const string AUserName = "john";
        public const int ABasketId = 1;

        public static readonly Mock<IAsyncRepository<Basket>> ABasketRepository = new();
        public static readonly Mock<IUriComposer> AnUriComposer = new();
        public static readonly Mock<IAsyncRepository<CatalogItem>> AnItemRepository = new();
        public static readonly Mock<Basket> ABasket = new();
    }
}