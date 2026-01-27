using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Models
{
    public class FirebaseRegisterResponse
    {
        public string IdToken { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string LocalId { get; set; }
        public int ExpiresIn { get; set; }
    }
}
