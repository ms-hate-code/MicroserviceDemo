using MediatR;

namespace MicroserviceDemo.BuildingBlock.Core.CQRS;

public interface IQuery<TQuery> : IRequest<TQuery>
    where TQuery : notnull
{
}