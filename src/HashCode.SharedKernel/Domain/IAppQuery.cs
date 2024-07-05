using Ardalis.Result;
using MediatR;

namespace HashCode.SharedKernel.Domain;

public interface IAppQuery<TResponse> : IRequest<Result<TResponse>>;