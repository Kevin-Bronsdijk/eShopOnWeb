using System.Linq;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.UnitTests.Services.Scenarios.NSubstitute;
using Microsoft.eShopWeb.Web.Services;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Microsoft.eShopWeb.UnitTests.Services.NSubstitute
{
    // Notes:
    // All examples are using NSubstitute
    // Using Shouldly instead of Assert
    
    // Test how we often see them
    
    public class BasketViewModelServiceTests
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel()
        {            
            // Arrange
            const string userName = "blah";
            const int basketId = 234;
            
            var basket = Substitute.For<Basket>(userName);
            basket.Id.Returns(basketId);
            
            var basketRepositoryMock = Substitute.For<IAsyncRepository<Basket>>();
            
            basketRepositoryMock.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>())
                .Returns(basket);
            
            var testSubject = new BasketViewModelService(
                basketRepositoryMock,
                Substitute.For<IAsyncRepository<CatalogItem>>(), 
                Substitute.For<IUriComposer>());
            
            // Act
            var result = await testSubject.GetOrCreateBasketForUser(userName); 
            
            // Assert
            Assert.Equal(basketId, result.Id);
            Assert.Equal(userName, result.BuyerId);
            Assert.False(result.Items.Any());
            
            await basketRepositoryMock.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await basketRepositoryMock.Received(0).AddAsync(Arg.Is<Basket>(b => b.BuyerId == userName));
        }
        
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserDoesNotHaveAnExistingBasket_ThenCreateANewBasketAndViewModel() 
        {
            // Arrange
            const string userName = "blah";
            const int basketId = 234;
            
            var basket = Substitute.For<Basket>(userName);
            basket.Id.Returns(basketId);
            
            var basketRepositoryMock = Substitute.For<IAsyncRepository<Basket>>();
            
            basketRepositoryMock.FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>())
                .Returns((Basket) null);
            
            basketRepositoryMock.AddAsync(Arg.Any<Basket>())
                .Returns(basket);
            
            var testSubject = new BasketViewModelService(
                basketRepositoryMock,
                Substitute.For<IAsyncRepository<CatalogItem>>(), 
                Substitute.For<IUriComposer>());
            
            // Act
            var result = await testSubject.GetOrCreateBasketForUser(userName); 
            
            // Assert
            Assert.Equal(basketId, result.Id);
            Assert.Equal(userName, result.BuyerId);
            Assert.False(result.Items.Any());
            
            await basketRepositoryMock.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await basketRepositoryMock.Received(1).AddAsync(Arg.Is<Basket>(b => b.BuyerId == userName));
        }
    }
    
    // First iteration, shared mocks and constants, no mock scenarios

    public class BasketViewModelServiceTestsV1
    { 
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel() 
        {
            var testSubject = new BasketViewModelService(
                Given.ABasketRepository, 
                Given.AnItemRepository, 
                Given.AnUriComposer);

            var result = await testSubject.GetOrCreateBasketForUser("username");
            
            Assert.Equal(Given.ABasketId, result.Id);
            Assert.Equal(Given.AUserName, result.BuyerId);
            Assert.False(result.Items.Any());
        }
    }
    
    // Second iteration, using Shoudly, include test scenario
    // missing verification of matching invocations
    
    public class BasketViewModelServiceTestsV2
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel() 
        {
            var testSubject = new BasketViewModelService(
                Given.ABasketRepository.WhichReturnsABasket(Given.AUserName, Given.ABasketId),
                Given.AnItemRepository, 
                Given.AnUriComposer);

            var result = await testSubject.GetOrCreateBasketForUser(Given.AUserName);
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
        }
    }
    
    // Third iteration, include verification of matching invocations
    // cluttered and less reuse due to mock variables and test subject creation 
    
    public class BasketViewModelServiceTestsV3
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel()
        {
            var basketRepositoryMock = Given.ABasketRepository.WhichReturnsABasket(Given.AUserName, Given.ABasketId);
            
            var testSubject = new BasketViewModelService(
                basketRepositoryMock,
                Given.AnItemRepository, 
                Given.AnUriComposer);

            var result = await testSubject.GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();

            await basketRepositoryMock.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await basketRepositoryMock.Received(0).AddAsync(Arg.Is<Basket>(b => b.BuyerId == Given.AUserName));
        }
    }
    
    // Fourth iteration, include a builder 
    // missing verification of matching invocations

    public class BasketViewModelServiceBuilder
    {
        public IAsyncRepository<Basket> BasketRepository { get; private set; }

        public BasketViewModelServiceBuilder With(IAsyncRepository<Basket> basketRepositoryMock)
        {
            BasketRepository = basketRepositoryMock;
            return this;
        }
        
        public BasketViewModelService BuildTestSubject()
        {
            return Build();
        }

        public BasketViewModelService Build()
        {
            return new(
                BasketRepository,
                Given.AnItemRepository, 
                Given.AnUriComposer);
        }
    }

    public class BasketViewModelServiceTestsV4
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel()
        {
            var testSubject = new BasketViewModelServiceBuilder().With(
                Given.ABasketRepository.WhichReturnsABasket(Given.AUserName, Given.ABasketId)).Build();
            
            var result = await testSubject.GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            //basketRepositoryMock.Verify(x => 
            //    x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once); //
        }
    }
    
    // Fifth iteration, include verification of matching invocations
    // builder will make more sense as soon as we start adding more tests
    
    public class BasketViewModelServiceTestsV5
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel()
        {
            var builder = new BasketViewModelServiceBuilder().With(
                Given.ABasketRepository.WhichReturnsABasket(Given.AUserName, Given.ABasketId));
            
            var result = await builder
                .BuildTestSubject()
                .GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            await builder.BasketRepository.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await builder.BasketRepository.Received(0).AddAsync(Arg.Is<Basket>(b => b.BuyerId == Given.AUserName));
        }
    }
    
    // Sixth iteration, adding a second test
    // can clearly see the duplication
    
    public class BasketViewModelServiceTestsV6
    {
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel()
        {
            var builder = new BasketViewModelServiceBuilder().With(
                Given.ABasketRepository
                    .WhichReturnsABasket(true));
            
            var result = await builder
                .BuildTestSubject()
                .GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            await builder.BasketRepository.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await builder.BasketRepository.Received(0).AddAsync(Arg.Is<Basket>(b => b.BuyerId == Given.AUserName));
        }
        
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserDoesNotHaveAnExistingBasket_ThenCreateANewBasketAndViewModel()
        {
            var builder = new BasketViewModelServiceBuilder().With(
                Given.ABasketRepository
                    .WhichReturnsABasket(false)
                    .WhichCanCreateABasket());
            
            var result = await builder
                .BuildTestSubject()
                .GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            await builder.BasketRepository.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await builder.BasketRepository.Received(1).AddAsync(Arg.Is<Basket>(b => b.BuyerId == Given.AUserName));
        }
    }
    
    // Seventh iteration, merge into a single parameterised test
    
    public class BasketViewModelServiceTestsV7
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetOrCreateBasketForUser_WhenBasedOnScenario_ThenShouldExecuteAsExpected(bool basketExists)
        {
            var builder = new BasketViewModelServiceBuilder().With(
                Given.ABasketRepository
                    .WhichReturnsABasket(basketExists)
                    .WhichCanCreateABasket());
            
            var result = await builder
                .BuildTestSubject()
                .GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            
            await builder.BasketRepository.Received(1).FirstOrDefaultAsync(Arg.Any<BasketWithItemsSpecification>());
            await builder.BasketRepository.Received(basketExists? 0 : 1)
                .AddAsync(Arg.Is<Basket>(b => b.BuyerId == Given.AUserName));
        }
    }
}