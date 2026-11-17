using System.Text.Json;

namespace ArtConnect.Web.UI.Helpers
{
    /// <summary>Shopping cart stored in the session (a list of artwork ids).</summary>
    public static class SessionCart
    {
        private const string Key = "ArtConnect.Cart";

        public static List<int> Get(ISession session)
        {
            var json = session.GetString(Key);
            if (string.IsNullOrEmpty(json))
                return new List<int>();

            return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
        }

        public static int Count(ISession session) => Get(session).Count;

        public static bool Add(ISession session, int artworkId)
        {
            var ids = Get(session);
            if (ids.Contains(artworkId))
                return false;

            ids.Add(artworkId);
            Save(session, ids);
            return true;
        }

        public static void Remove(ISession session, int artworkId)
        {
            var ids = Get(session);
            ids.Remove(artworkId);
            Save(session, ids);
        }

        public static void Clear(ISession session) => session.Remove(Key);

        private static void Save(ISession session, List<int> ids)
        {
            session.SetString(Key, JsonSerializer.Serialize(ids));
        }
    }
}
