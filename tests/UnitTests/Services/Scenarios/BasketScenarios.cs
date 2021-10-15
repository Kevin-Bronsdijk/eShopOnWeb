using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Moq;

namespace Microsoft.eShopWeb.UnitTests.Services.Scenarios
{
    public static class BasketScenarios
    {
        public static Mock<Basket> WhichIsBasedOnProvidedData(this Mock<Basket> @this, string buyerId, int id)
        {
            Mock<Basket> basket = new(buyerId)
            {
                CallBase = true
            };

            basket.Setup(x => x.Id).Returns(id);

            return basket;
        }
    }
}