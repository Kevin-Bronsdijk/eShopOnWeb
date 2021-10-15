using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using NSubstitute;

namespace Microsoft.eShopWeb.UnitTests.Services.Scenarios.NSubstitute
{
    public static class Given
    {
        public const string AUserName = "john";
        public const int ABasketId = 1;
        
        public static IAsyncRepository<Basket> ABasketRepository => Substitute.For<IAsyncRepository<Basket>>();
        public static IUriComposer AnUriComposer => Substitute.For<IUriComposer>();
        public static IAsyncRepository<CatalogItem> AnItemRepository => Substitute.For<IAsyncRepository<CatalogItem>>();
        public static Basket ABasket => Substitute.For<Basket>(string.Empty);
    }
}