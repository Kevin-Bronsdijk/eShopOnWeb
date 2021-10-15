using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using NSubstitute;

namespace Microsoft.eShopWeb.UnitTests.Services.Scenarios.NSubstitute
{
    public static class BasketRepositoryScenarios
    {
        public static IAsyncRepository<Basket> WhichReturnsABasket(this IAsyncRepository<Basket> @this,
            string buyerId, int id)
        {
            var bucket = Given.ABasket.WhichIsBasedOnProvidedData(buyerId, id);
            @this.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>())
                .Returns(bucket);

            return @this;
        }

        public static IAsyncRepository<Basket> WhichReturnsABasket(this IAsyncRepository<Basket> @this,
            bool hasABasket)
        {
            
            if (hasABasket)
            {
                var bucket = Given.ABasket.WhichIsBasedOnProvidedData(Given.AUserName, Given.ABasketId);
                @this.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>())
                    .Returns(bucket);
            }
            else
            {
                @this.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>())
                    .Returns((Basket) null);
            }

            return @this;
        }

        public static IAsyncRepository<Basket> WhichCanCreateABasket(this IAsyncRepository<Basket> @this)
        {
            var bucket = Given.ABasket.WhichIsBasedOnProvidedData(Given.AUserName, Given.ABasketId);
            @this.AddAsync(Arg.Any<Basket>())
                .Returns(bucket);
            
            return @this;
        }
    }
    
    public static class BasketScenarios
    {
        public static Basket WhichIsBasedOnProvidedData(this Basket @this, string buyerId, int id)
        {
            @this = Substitute.For<Basket>(buyerId);
            @this.Id.Returns(id);
            
            return  @this;
        }
    }
}