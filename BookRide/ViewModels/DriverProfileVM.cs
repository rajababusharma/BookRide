using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.ViewModels
{
    public partial class DriverProfileVM : ObservableObject, IQueryAttributable
    {
        public DriverProfileVM() { }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            throw new NotImplementedException();
        }
    }
}
