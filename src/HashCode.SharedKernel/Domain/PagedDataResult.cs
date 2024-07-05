namespace HashCode.SharedKernel.Domain;

public record PagedDataResult<T>(long Total, IReadOnlyList<T> Data);