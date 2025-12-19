using BookRide.Models;
using BookRide.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class TravellerProfileVM : ObservableObject, IQueryAttributable
    {
        private readonly RealtimeDatabaseService _db;

        [ObservableProperty]
        public ObservableCollection<Users> usersList = new();

        
   

        public TravellerProfileVM()
        {
            _db = new RealtimeDatabaseService();
            LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            UsersList = await _db.GetAllAsync<Users>("Users").ContinueWith(t =>
            {
                var userList = t.Result;
                return new ObservableCollection<Users>(userList);
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            var usr = query["CurrentUser"] as Users;
        }
    }
}
