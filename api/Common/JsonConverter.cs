using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace Common;

public static class JsonConverter
{
    public static string ToJson(this object? item) => item != null ? JsonConvert.SerializeObject(item) : "{}";

    public static T? FromJson<T>(this string source, bool catchError = false)
    {
        if (string.IsNullOrEmpty(source))
            return default(T);
        if (!catchError)
            return JsonConvert.DeserializeObject<T>(source);
        return JsonConvert.DeserializeObject<T>(source,
            new JsonSerializerSettings() { Error = (EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs>)((sender, args) => args.ErrorContext.Handled = true) });
    }

    public static T? FromJson<T>(this Stream stream) where T : class => new DataContractJsonSerializer(typeof(T)).ReadObject(stream) as T;
}
