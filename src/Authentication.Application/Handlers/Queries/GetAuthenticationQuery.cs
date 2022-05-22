using Authentication.Application.ViewModels;
using UserAuthentication.Domain.Authentications;

namespace Authentication.Application.Handlers.Queries
{
    public class GetAuthenticationQuery : BaseQuery<AuthenticationViewModel>
    {
        public long Id { get; }

        public GetAuthenticationQuery(long id)
        {
            Id = id;
        }
    }

    public class GetAuthenticationQueryHandler : BaseQueryHandler<GetAuthenticationQuery, AuthenticationViewModel>
    {
        private readonly IAuthenticationRepository _repository;

        public GetAuthenticationQueryHandler(IAuthenticationRepository repository)
        {
            _repository = repository;
        }

        public override async Task<AuthenticationViewModel> Handle(GetAuthenticationQuery request, CancellationToken cancellationToken)
        {
            var result = await _repository.FindByIdAsync(request.Id);
            return new AuthenticationViewModel()
            {
                Email = result.Email,
                Mobile = result.Mobile,
            };
        }
    }
}
