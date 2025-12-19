using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class RealtimeDatabaseService
    {
        private readonly FirebaseClient _firebase;

        public RealtimeDatabaseService()
        {
            _firebase = new FirebaseClient(
                "https://bookride-f497d-default-rtdb.firebaseio.com/");
        }

        // Save or Update
        public async Task SaveAsync<T>(string path, T data)
        {
            await _firebase
                .Child(path)
                .PutAsync(data);
        }

        // Get all records under a node
        public async Task<List<T>> GetAllAsync<T>(string path)
        {
            var items = await _firebase
                .Child(path)
                .OnceAsync<T>();

            return items.Select(i => i.Object).ToList();
        }


        // Get a single record by key
        // 🔹 Retrieve Single Node
        public async Task<T> GetAsync<T>(string path)
        {
            return await _firebase
                .Child(path)
                .OnceSingleAsync<T>();
        }

        // 🔹 Delete
        public async Task DeleteAsync(string path)
        {
            await _firebase
                .Child(path)
                .DeleteAsync();
        }
    }
}
