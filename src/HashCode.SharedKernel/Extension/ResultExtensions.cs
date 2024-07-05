// ReSharper disable once CheckNamespace
namespace Ardalis.Result;

internal static class ResultExtensions
{
    public static async Task<Result<T>> SuccessOrNotFound<T>(this Task<T?> resultOrNull) where T : class
    {
        T? result = await resultOrNull;
        return result is null ? Result.NotFound() : Result.Success(result);
    }
}