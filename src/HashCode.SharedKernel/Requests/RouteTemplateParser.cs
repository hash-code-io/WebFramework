using System.Text;

namespace HashCode.SharedKernel.Requests;

internal record Replacements(string ReplacementString, IReadOnlyList<string> OrderedParamNames);

internal static class RouteTemplateParser
{
    private static InvalidOperationException OpenBracketException(string routeTemplate) => new($"Found 2 or more open brackets without a closing bracket in between them in [{routeTemplate}]");
    private static InvalidOperationException ClosingBracketException(string routeTemplate) => new($"Found 2 or more open brackets without a closing bracket in between them in [{routeTemplate}]");
    private static InvalidOperationException EmptyBracketException(string routeTemplate) => new($"Found empty bracket in [{routeTemplate}]");
    private static InvalidOperationException EmptyRouteTemplate() => new("Route Template may not be empty");

    public static Replacements Parse(string routeTemplate)
    {
        if (string.IsNullOrEmpty(routeTemplate)) throw EmptyRouteTemplate();

        var replacementStringBuilder = new StringBuilder(routeTemplate.Length);
        var paramNameStringBuilder = new StringBuilder();
        bool inOpenBracket = false;

        List<string> orderedParamNames = [];

        for (int i = 0; i < routeTemplate.Length; i++)
        {
            char current = routeTemplate[i];

            switch (current)
            {
                case '{':
                    if (inOpenBracket)
                        throw OpenBracketException(routeTemplate);

                    inOpenBracket = true;
                    paramNameStringBuilder.Clear();
                    replacementStringBuilder.Append($"{{{orderedParamNames.Count}}}");

                    break;
                case '}':
                    if (!inOpenBracket)
                        throw ClosingBracketException(routeTemplate);

                    inOpenBracket = false;
                    string paramName = paramNameStringBuilder.ToString();
                    if (string.IsNullOrEmpty(paramName))
                        throw EmptyBracketException(routeTemplate);
                    orderedParamNames.Add(paramName.ToLowerInvariant());

                    break;
                case ':':
                    while (routeTemplate[i + 1] != '}')
                    {
                        i++;
                        if (i >= routeTemplate.Length) throw ClosingBracketException(routeTemplate);
                    }

                    break;
                default:
                    if (inOpenBracket && current == ' ') // we don't care about space in params
                        break;
                    StringBuilder stringBuilder = GetCorrectBuilder();
                    stringBuilder.Append(current);

                    break;
            }
        }
        
        return inOpenBracket
            ? throw OpenBracketException(routeTemplate)
            : new Replacements(replacementStringBuilder.ToString(), orderedParamNames.AsReadOnly());

        StringBuilder GetCorrectBuilder()
        {
            return inOpenBracket
                ? paramNameStringBuilder
                : replacementStringBuilder;
        }
    }
}