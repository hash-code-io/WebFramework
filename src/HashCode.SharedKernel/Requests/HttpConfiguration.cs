using HashCode.SharedKernel.Auth;

namespace HashCode.SharedKernel.Requests;

public sealed record HttpConfiguration(HttpMethod HttpMethod, string RouteTemplate, bool HasBody, AppPermission? Permission)
{
    public static HttpConfiguration Get(string routeTemplate, AppPermission permission) => new(HttpMethod.Get, routeTemplate, false, permission);
    public static HttpConfiguration Delete(string routeTemplate, AppPermission permission) => new(HttpMethod.Delete, routeTemplate, false, permission);
    public static HttpConfiguration Post(string routeTemplate, AppPermission permission) => new(HttpMethod.Post, routeTemplate, true, permission);
    public static HttpConfiguration Put(string routeTemplate, AppPermission permission) => new(HttpMethod.Put, routeTemplate, true, permission);

    public static HttpConfiguration GetAnonymous(string routeTemplate) => new(HttpMethod.Get, routeTemplate, false, null);
    public static HttpConfiguration DeleteAnonymous(string routeTemplate) => new(HttpMethod.Delete, routeTemplate, false, null);
    public static HttpConfiguration PostAnonymous(string routeTemplate) => new(HttpMethod.Post, routeTemplate, true, null);
    public static HttpConfiguration PutAnonymous(string routeTemplate) => new(HttpMethod.Put, routeTemplate, true, null);
}