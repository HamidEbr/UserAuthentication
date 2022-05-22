using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserAuthentication.Application.Handlers.Queries;
using UserAuthentication.Application.ViewModels;

namespace UserAuthentication.Application.Handlers.Commands
{
    /// <summary>
    /// this command is called from gateway
    /// </summary>
    public class RegisterUserCommand : BaseCommand<long>
    {
        public RegisterUserCommand(string mobile, string email, string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Mobile = mobile;
            Email = email;
        }

        public string Mobile { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }

    public class RegisterUserCommandHandler : BaseCommandHandler<RegisterUserCommand, long>
    {
        private readonly ILogger<GetUserAuthQueryHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private string _userApiUrl = @"http://localhost:23833/";
        private string _autApiUrl = @"http://localhost:23834/";
        private HttpClient _httpClient;

        private readonly RetryPolicy _retryPolicy;
        private static TimeoutPolicy _timeoutPolicy;
        private readonly FallbackPolicy<string> _fallbackPolicy;
        private static CircuitBreakerPolicy _circuitBreakerPolicy;
        private static BulkheadPolicy _bulkheadPolicy;

        /// <summary>
        /// initilize retry policy using polly
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="httpClientFactory"></param>
        public RegisterUserCommandHandler(ILogger<GetUserAuthQueryHandler> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;

            _retryPolicy = Policy
                .Handle<Exception>()
                .Retry(2);

            _timeoutPolicy = Policy.Timeout(20, TimeoutStrategy.Pessimistic);

            _fallbackPolicy = Policy<string>
                                .Handle<Exception>()
                                .Fallback("Customer Name Not Available - Please retry later");

            if (_circuitBreakerPolicy == null)
            {
                _circuitBreakerPolicy = Policy.Handle<Exception>()
                                              .CircuitBreaker(2, TimeSpan.FromMinutes(1));
            }

            _bulkheadPolicy = Policy.Bulkhead(3, 6);
        }

        /// <summary>
        /// combine two api call results into one
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<long> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_autApiUrl);
            var uri = "/api/authentication/register-user/";
            var authContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new AuthenticationViewModel()
            {
                Email = request.Email,
                Mobile = request.Mobile
            }), Encoding.UTF8, "application/json");

            var authResult = _bulkheadPolicy.Execute(() => _httpClient.PostAsync(uri, authContent).Result);
            var authString = await authResult.Content.ReadAsStringAsync(cancellationToken);
            var auth = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticationViewModel>(authString);

            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_userApiUrl);
            uri = "/api/authentication/register-user/";
            var userContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(new UserViewModel()
            {
                AuthenticationId = auth.Id,
                FirstName = request.FirstName,
                LastName = request.LastName
            }), Encoding.UTF8, "application/json");
            var result = _bulkheadPolicy.Execute(() => _httpClient.PostAsync(uri, userContent).Result);
            
            return auth.Id;
        }
    }
}