using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace BookRide.Constants
{
    public static class Constants
    {
        public const string Firebase_project_id = "reserve-a-taxi-d161e";
        public const string Firebase_Bucket = "reserve-a-taxi-d161e.firebasestorage.app";
        public const string Firebase_Realtime_Storage_url = "https://reserve-a-taxi-d161e-default-rtdb.firebaseio.com/";
        public const string Firebase_AadharLocation = "AadharCards";
        public const string Firebase_ProfileImageLocation = "ProfileImages";
        public const string Firebase_PaymentImageLocation = "PayReciept";
        public const int LocationService_Timer = 15;  // minutes
        public const int CreditPointService_Timer = 24;  // Hours
    }
}
