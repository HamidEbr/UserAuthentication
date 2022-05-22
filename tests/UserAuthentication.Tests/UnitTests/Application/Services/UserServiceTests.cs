using MediatR;
using Microsoft.AspNetCore.Http;
using Moq;
using OpenTracing;
using OpenTracing.Mock;
using User.Application.Handlers.Commands;
using User.Application.ViewModels;
using UserAuthentication.Domain.Users;
using UserAuthentication.Tests.UnitTests.Helpers;
using Xunit;

namespace UserAuthentication.Tests.UnitTests.Application.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockTaskRepository = new Mock<IUserRepository>();
        private readonly Mock<ITracer> _mockITracer = new Mock<ITracer>();
        private readonly Mock<IMediator> _mockIMediator = new Mock<IMediator>();
        private static readonly Mock<IHttpContextAccessor> _mockIHttpContextAccessor = new Mock<IHttpContextAccessor>();

        //private readonly UserViewModelMapper _mockUserViewModelMapper = new UserViewModelMapper(_mockIHttpContextAccessor.Object);

        [Fact]
        public async System.Threading.Tasks.Task CreateUser_Success()
        {
            //Arrange
            _mockITracer.Setup(x => x.BuildSpan(It.IsAny<string>())).Returns(() => new MockSpanBuilder(new MockTracer(), ""));
            _mockIMediator.Setup(x => x.SendAsync<UserViewModel>(It.IsAny<StoreUserCommand>(), null))
                .Returns(System.Threading.Tasks.Task.FromResult(UserHelper.GetUser()));
            _mockIHttpContextAccessor.Setup(x => x.HttpContext).Returns(HttpContextHelper.GetHttpContext());

            //Act

            var userService = new UserService(_mockTaskRepository.Object, _mockUserViewModelMapper, _mockITracer.Object, _mockUserFactory.Object, _mockIMediator.Object);
            var result = await userService.Create(UserViewModelHelper.GetTaskViewModel());

            //Assert
            Assert.NotNull(result);

            Assert.Equal("FirstName", result.FirstName);
            Assert.Equal("LastName", result.LastName);

            Assert.NotNull(result.Id);
            Assert.NotNull(result.FirstName);
            Assert.NotNull(result.LastName);

            _mockITracer.Verify(x => x.BuildSpan(It.IsAny<string>()), Times.Once);
            _mockIMediator.Verify(x => x.SendAsync<UserViewModel>(It.IsAny<StoreUserCommand>(), null), Times.Once);
        }
    }
}
