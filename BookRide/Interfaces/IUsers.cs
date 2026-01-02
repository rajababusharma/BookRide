using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface IUsers
    {
        Task<ObservableCollection<Users>> GetUsers();
    }
}
