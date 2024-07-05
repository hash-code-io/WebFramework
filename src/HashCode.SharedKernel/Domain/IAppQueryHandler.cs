using Ardalis.Result;
using MediatR;

namespace HashCode.SharedKernel.Domain;

public interface IAppQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IAppQuery<TResponse>;