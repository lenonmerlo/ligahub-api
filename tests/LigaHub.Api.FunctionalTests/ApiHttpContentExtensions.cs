using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LigaHub.Api.FunctionalTests;

internal static class ApiHttpContentExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions =
        CreateSerializerOptions();

    public static Task<T?> ReadFromApiJsonAsync<T>(
        this HttpContent content,
        CancellationToken cancellationToken = default)
    {
        return content.ReadFromJsonAsync<T>(
            SerializerOptions,
            cancellationToken);
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web);

        options.Converters.Add(
            new JsonStringEnumConverter(
                JsonNamingPolicy.CamelCase));

        return options;
    }
}
