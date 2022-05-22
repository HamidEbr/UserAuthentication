using UserAuthentication.Domain.Users;

namespace User.Application.Handlers.Commands
{
    public class StoreUserCommand : BaseCommand<long>
    {
        public StoreUserCommand(long authenticationId, string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            AuthenticationId = authenticationId;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public long AuthenticationId { get; }
    }

    public class StoreUserCommandHandler : BaseCommandHandler<StoreUserCommand, long>
    {
        private readonly IUserRepository _repository;

        public StoreUserCommandHandler(IUserRepository userRepository)
        {
            _repository = userRepository;
        }

        public override async Task<long> Handle(StoreUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _repository.Add(new UserModel()
            {
                AuthenticationId = request.AuthenticationId,
                FirstName = request.FirstName,
                LastName = request.LastName
            });
            return result.Id;
        }
    }
}
