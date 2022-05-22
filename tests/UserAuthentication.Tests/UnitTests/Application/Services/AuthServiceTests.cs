using Authentication.Application.Handlers.Commands;
using Authentication.Application.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using OpenTracing;
using OpenTracing.Mock;
using UserAuthentication.Domain.Authentications;
using UserAuthentication.Tests.UnitTests.Helpers;
using Xunit;

namespace UserAuthentication.Tests.UnitTests.Application.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthenticationRepository> _mockTaskRepository = new Mock<IAuthenticationRepository>();
        private readonly Mock<ITracer> _mockITracer = new Mock<ITracer>();
        private readonly Mock<IMediator> _mockIMediator = new Mock<IMediator>();
        private static readonly Mock<IHttpContextAccessor> _mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();

        //private readonly UserViewModelMapper _mockUserViewModelMapper = new UserViewModelMapper(_mockIHttpContextAccessor.Object);

        [Fact]
        public async System.Threading.Tasks.Task CreateAuth_Success()
        {
            //Arrange
            _mockITracer.Setup(x => x.BuildSpan(It.IsAny<string>())).Returns(() => new MockSpanBuilder(new MockTracer(), ""));
            _mockIMediator.Setup(x => x.SendAsync<AuthenticationViewModel>(It.IsAny<StoreAuthenticationCommand>(), null))
                .Returns(System.Threading.Tasks.Task.FromResult(AuthenticationHelper.GetAuthentication()));
            _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(HttpContextHelper.GetHttpContext());

            //Act

            var userService = new AuthenticationService(_mockTaskRepository.Object, _mockAuthViewModelMapper, _mockITracer.Object, _mockAuthFactory.Object, _mockIMediator.Object);
            var result = await userService.Create(AuthenticationViewModelHelper.GetTaskViewModel());

            //Assert
            Assert.NotNull(result);

            Assert.Equal("Email", result.Email);
            Assert.Equal("Mobile", result.Mobile);

            Assert.NotNull(result.Id);
            Assert.NotNull(result.Email);
            Assert.NotNull(result.Mobile);

            _mockITracer.Verify(x => x.BuildSpan(It.IsAny<string>()), Times.Once);
            _mockIMediator.Verify(x => x.SendAsync<AuthenticationViewModel>(It.IsAny<StoreAuthenticationCommand>(), null), Times.Once);
        }
    }
}
