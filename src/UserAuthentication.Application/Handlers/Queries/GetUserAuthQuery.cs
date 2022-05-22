using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UserAuthentication.Application.ViewModels;

namespace UserAuthentication.Application.Handlers.Queries
{
    public class GetUserAuthQuery : BaseQuery<UserAuthViewModel>
    {
        public long Id { get; }

        public GetUserAuthQuery(long id)
        {
            Id = id;
        }
    }
    public class GetUserAuthQueryHandler : BaseQueryHandler<GetUserAuthQuery, UserAuthViewModel>
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
        public GetUserAuthQueryHandler(ILogger<GetUserAuthQueryHandler> logger, IHttpClientFactory httpClientFactory)
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

        public override async Task<UserAuthViewModel> Handle(GetUserAuthQuery request, CancellationToken cancellationToken)
        {
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_autApiUrl);
            var uri = "/api/get-authentication/" + request.Id;
            var result = _bulkheadPolicy.Execute(() => _httpClient.GetStringAsync(uri).Result);
            var authModel = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticationViewModel>(result);

            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_userApiUrl);
            uri = "/api/get-user/" + request.Id;
            result = _bulkheadPolicy.Execute(() => _httpClient.GetStringAsync(uri).Result);
            var userModel = Newtonsoft.Json.JsonConvert.DeserializeObject<UserViewModel>(result);

            return new UserAuthViewModel()
            {
                Email = authModel.Email,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Mobile = authModel.Mobile,
            };
        }
    }
}
