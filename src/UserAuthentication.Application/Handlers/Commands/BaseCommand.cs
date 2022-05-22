using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace UserAuthentication.Application.Handlers.Commands
{
    /// <summary>
    /// for general use in dependancy injection we use it using reflection
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class BaseCommand<TResponse> : IRequest<TResponse>
    {

    }

    public abstract class BaseCommandHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
    }
}
