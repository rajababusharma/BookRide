using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface IWhatsAppConnect
    {
      void  WhatsappConnect(string phoneNumber, string message);
    }
}
