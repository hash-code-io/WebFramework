using Ardalis.Result;
using MediatR;

namespace HashCode.SharedKernel.Domain;

public interface IAppBaseCommand;
public interface IAppCommand<TResponse> : IAppBaseCommand, IRequest<Result<TResponse>>;
public interface IAppCommand : IAppBaseCommand, IRequest<Result>;