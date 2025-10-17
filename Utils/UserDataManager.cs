using System.Text.Json;
using OnlineShoppingTests.TestData.Models;

namespace OnlineShoppingTests.Utils
{
    public static class UserDataManager
    {
        private static readonly string filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\TestData\users.json"));

        private static readonly Dictionary<string, User> users;

        static UserDataManager()
        {
            var json = File.ReadAllText(filePath);
            users = JsonSerializer.Deserialize<Dictionary<string, User>>(json)
                ?? throw new InvalidOperationException("Could not load users.json");
        }

        public static User GetUser(string key)
        {
            if (!users.TryGetValue(key, out var user))
                throw new KeyNotFoundException($"User '{key}' not found in users.json");

            return user;
        }
    }
}