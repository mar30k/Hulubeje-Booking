using Newtonsoft.Json;

namespace HulubejeBooking.Helpers
{
    public static class SessionExtensions
    {
        // Set object as JSON in session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        // Get object from session by deserializing JSON
        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }
    }
}
