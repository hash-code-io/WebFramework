namespace HashCode.SharedKernel.Domain;

public interface IPageableQuery
{
    int Skip { get; }
    int Take { get; }
}