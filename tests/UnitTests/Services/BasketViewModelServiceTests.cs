using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Web.Services;
using Moq;
using Xunit;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Specifications;
using Microsoft.eShopWeb.UnitTests.Services.Scenarios;
using Shouldly;

namespace Microsoft.eShopWeb.UnitTests.Services
{
    // Notes:
    // All examples are using Moq
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
            
            Mock<Basket> basket = new(userName)
            {
                CallBase = true
            };
            basket.Setup(x => x.Id).Returns(basketId);
            
            var basketRepositoryMock = new Mock<IAsyncRepository<Basket>>();
            basketRepositoryMock.Setup(x => 
                    x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), CancellationToken.None))
                .ReturnsAsync(basket.Object);
            
            var testSubject = new BasketViewModelService(
                basketRepositoryMock.Object,
                new Mock<IAsyncRepository<CatalogItem>>().Object, 
                new Mock<IUriComposer>().Object);
            
            // Act
            var result = await testSubject.GetOrCreateBasketForUser(userName); 
            
            // Assert
            Assert.Equal(basketId, result.Id);
            Assert.Equal(userName, result.BuyerId);
            Assert.False(result.Items.Any());
            
            basketRepositoryMock.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once);
            
            basketRepositoryMock.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == userName),default), Times.Never);
        }
        
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserDoesNotHaveAnExistingBasket_ThenCreateANewBasketAndViewModel() 
        {
            // Arrange
            const string userName = "blah";
            const int basketId = 234;
            
            Mock<Basket> basket = new(userName)
            {
                CallBase = true
            };
            basket.Setup(x => x.Id).Returns(basketId);
            
            var basketRepositoryMock = new Mock<IAsyncRepository<Basket>>();
            basketRepositoryMock.Setup(x => 
                    x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), CancellationToken.None))
                .ReturnsAsync((Basket) null);
            
            basketRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Basket>(), default))
                .ReturnsAsync(basket.Object);
            
            var testSubject = new BasketViewModelService(
                basketRepositoryMock.Object,
                new Mock<IAsyncRepository<CatalogItem>>().Object, 
                new Mock<IUriComposer>().Object);
            
            // Act
            var result = await testSubject.GetOrCreateBasketForUser(userName); 
            
            // Assert
            Assert.Equal(basketId, result.Id);
            Assert.Equal(userName, result.BuyerId);
            Assert.False(result.Items.Any());
            
            basketRepositoryMock.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once);
            
            basketRepositoryMock.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == userName),default), Times.Once);
        }
    }
    
    // First iteration, shared mocks and constants, no mock scenarios

    public class BasketViewModelServiceTestsV1
    { 
        [Fact]
        public async Task GetOrCreateBasketForUser_WhenTheUserHasAnExistingBasket_ThenFetchBasketAndCreateViewModel() 
        {
            var testSubject = new BasketViewModelService(
                Given.ABasketRepository.Object, 
                Given.AnItemRepository.Object, 
                Given.AnUriComposer.Object);

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
                Given.ABasketRepository.WhichReturnsABasket(Given.AUserName, Given.ABasketId).Object,
                Given.AnItemRepository.Object, 
                Given.AnUriComposer.Object);

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
                basketRepositoryMock.Object,
                Given.AnItemRepository.Object, 
                Given.AnUriComposer.Object);

            var result = await testSubject.GetOrCreateBasketForUser(Given.AUserName); 
            
            result.Id.ShouldBe(Given.ABasketId);
            result.BuyerId.ShouldBe(Given.AUserName);
            result.Items.ShouldBeEmpty();
            
            basketRepositoryMock.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once);
            
            basketRepositoryMock.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == Given.AUserName),default), Times.Never); 
        }
    }
    
    // Fourth iteration, include a builder 
    // missing verification of matching invocations

    public class BasketViewModelServiceBuilder
    {
        public Mock<IAsyncRepository<Basket>> BasketRepository { get; private set; }

        public BasketViewModelServiceBuilder With(Mock<IAsyncRepository<Basket>> basketRepositoryMock)
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
                BasketRepository.Object,
                Given.AnItemRepository.Object, 
                Given.AnUriComposer.Object);
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
            
            builder.BasketRepository.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once); 
            
            builder.BasketRepository.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == Given.AUserName),default), Times.Never); 
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
            
            builder.BasketRepository.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once); 
            
            builder.BasketRepository.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == Given.AUserName),default), Times.Never); 
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
            
            builder.BasketRepository.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once); 
            
            builder.BasketRepository.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == Given.AUserName),default), Times.Once); 
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
            
            builder.BasketRepository.Verify(x => 
                x.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(),default), Times.Once); 
            
            builder.BasketRepository.Verify(x => 
                x.AddAsync(It.Is<Basket>(b => b.BuyerId == Given.AUserName),default), 
                basketExists? Times.Never : Times.Once); 
        }
    }
}