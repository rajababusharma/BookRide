using BookRide.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Services
{
    public class WhatsappConnectService : IWhatsAppConnect
    {
        public async void WhatsappConnect(string phoneNumber, string message)
        {
            var uri = new Uri($"whatsapp://send?phone={phoneNumber}&text={Uri.EscapeDataString(message)}");
            await Launcher.OpenAsync(uri);
        }
    }
}
