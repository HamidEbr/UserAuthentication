using UserAuthentication.Domain.Authentications;

namespace Authentication.Application.Handlers.Commands
{
    public class StoreAuthenticationCommand : BaseCommand<long>
    {
        public StoreAuthenticationCommand(string mobile, string email)
        {
            Mobile = mobile;
            Email = email;
        }

        public string Mobile { get; }
        public string Email { get; }
    }

    public class StoreAuthenticationCommandHandler : BaseCommandHandler<StoreAuthenticationCommand, long>
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public StoreAuthenticationCommandHandler(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public override async Task<long> Handle(StoreAuthenticationCommand request, CancellationToken cancellationToken)
        {
            var result = await _authenticationRepository.Add(new AuthenticationModel()
            {
                Email = request.Email,
                Mobile = request.Mobile
            });
            return result.Id;
        }
    }
}
