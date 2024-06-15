using Microsoft.AspNetCore.Http;

namespace WebApplication1.Extensions
{
    public static class SessionExtensions
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetInt32(key, value ? 1 : 0);
        }

        public static bool? GetBool(this ISession session, string key)
        {
            var value = session.GetInt32(key);
            if (value.HasValue)
            {
                return value.Value == 1;
            }
            return null;
        }
    }
}
