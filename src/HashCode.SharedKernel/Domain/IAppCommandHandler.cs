using Ardalis.Result;
using MediatR;

namespace HashCode.SharedKernel.Domain;

public interface IAppCommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : IAppCommand<TResponse>;

public interface IAppCommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : IAppCommand;