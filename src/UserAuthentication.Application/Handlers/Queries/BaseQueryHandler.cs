using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace UserAuthentication.Application.Handlers.Queries
{
    /// <summary>
    /// for general use in dependancy injection we use it using reflection
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public abstract class BaseQuery<TResponse> : IRequest<TResponse>
    {
    }

    public abstract class BaseQueryHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : IRequest<TResponse>
    {
        public abstract Task<TResponse> Handle(TCommand request, CancellationToken cancellationToken);
    }
}
