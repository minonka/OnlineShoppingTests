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

        /// <summary>
        /// Get a single user by key (e.g., "admin")
        /// </summary>
        public static User GetUser(string key)
        {
            if (!users.TryGetValue(key, out var user))
                throw new KeyNotFoundException($"User '{key}' not found in users.json");

            return user;
        }

        /// <summary>
        /// Get all users as a dictionary.
        /// </summary>
        public static IReadOnlyDictionary<string, User> GetAllUsers() => users;
    }
}