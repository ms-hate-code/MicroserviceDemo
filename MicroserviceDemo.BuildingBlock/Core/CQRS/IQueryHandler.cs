using MediatR;

namespace MicroserviceDemo.BuildingBlock.Core.CQRS;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TResponse : notnull
    where TQuery : IQuery<TResponse>
{
}