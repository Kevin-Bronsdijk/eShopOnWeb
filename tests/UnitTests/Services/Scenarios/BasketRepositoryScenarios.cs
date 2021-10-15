using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Moq;

namespace Microsoft.eShopWeb.UnitTests.Services.Scenarios
{
    public static class BasketRepositoryScenarios
    {
        public static Mock<IAsyncRepository<Basket>> WhichReturnsABasket(this Mock<IAsyncRepository<Basket>> @this,
            string buyerId, int id)
        {
            @this.Setup(x =>
                    x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), default))
                .ReturnsAsync(Given.ABasket.WhichIsBasedOnProvidedData(buyerId, id).Object);

            return @this;
        }

        public static Mock<IAsyncRepository<Basket>> WhichReturnsABasket(this Mock<IAsyncRepository<Basket>> @this,
            bool hasABasket)
        {
            if (hasABasket)
            {
                @this.Setup(x =>
                        x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), default))
                    .ReturnsAsync(Given.ABasket.WhichIsBasedOnProvidedData(Given.AUserName, Given.ABasketId).Object);
            }
            else
            {
                @this.Setup(x =>
                        x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), default))
                    .ReturnsAsync(null as Basket);
            }

            return @this;
        }

        public static Mock<IAsyncRepository<Basket>> WhichCanCreateABasket(this Mock<IAsyncRepository<Basket>> @this)
        {
            @this.Setup(x => x.AddAsync(It.IsAny<Basket>(), default))
                .ReturnsAsync(Given.ABasket.WhichIsBasedOnProvidedData(Given.AUserName, Given.ABasketId).Object);

            return @this;
        }
    }
}