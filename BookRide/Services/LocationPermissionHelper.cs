using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public static class LocationPermissionHelper
    {
        public static async Task<bool> EnsurePermissionsAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationAlways>();

            return status == PermissionStatus.Granted;
        }
    }
}
