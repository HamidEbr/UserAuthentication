using User.Application.ViewModels;
using UserAuthentication.Domain.Users;

namespace User.Application.Handlers.Queries
{
    public class GetUserQuery : BaseQuery<UserViewModel>
    {
        public long Id { get; }

        public GetUserQuery(long id)
        {
            Id = id;
        }
    }

    public class GetUserQueryHandler : BaseQueryHandler<GetUserQuery, UserViewModel>
    {
        private readonly IUserRepository _repository;

        public GetUserQueryHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public override async Task<UserViewModel> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.FindByIdAsync(request.Id);
            return new UserViewModel()
            {
                FirstName = result.FirstName,
                LastName = result.LastName,
            };
        }
    }
}
