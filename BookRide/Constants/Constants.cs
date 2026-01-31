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

        public const string Firebase_WebApi_key = "AIzaSyBrQSvoldFIOvbfbrY25qBbp6jRa2Z1vfM";
        public const string Firebase_App_id = "1:512183391871:android:8316cf26c46ede9bf3514a";
        public const string Firebase_project_id = "taxi-ride-e2fb5";
        public const string Firebase_Bucket = "taxi-ride-e2fb5.firebasestorage.app";   // for debug/release mode
        public const string Firebase_Realtime_Storage_url = "https://taxi-ride-e2fb5-default-rtdb.firebaseio.com/";
        public const string Firebase_AadharLocation = "AadharCards";
        public const string Firebase_ProfileImageLocation = "ProfileImages";
        public const string Firebase_PaymentImageLocation = "PayReciept";
        public const int LocationService_Timer = 15;  // minutes
        public const int CreditPointService_Timer = 24;  // Hours
        public const string Firebase_TokenKeyValue = "firebase_token";
        public const string Firebase_UIDKeyValue = "firebase_uid";
        public const string Firebase_RefreshTokenKeyValue = "refresh_token";
        public const string Firebase_TokenTimeKeyValue = "token_time";
        public const string Firebase_UserId = "rekharajababu7124@gmail.com";
        public const string Firebase_Userpwd = "Rekha@7124";
        public const string LoggedInUser = "LoggedInUser";
        public const string SessionStartTime = "SessionStartTime";
    }
}
