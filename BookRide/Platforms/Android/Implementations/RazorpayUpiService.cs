using Android.Content;
using BookRide.Interfaces;
using BookRide.Models;
using Android.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uri = Android.Net.Uri;

namespace BookRide.Platforms.Android.Implementations
{
    public class RazorpayUpiService : IUpiPaymentService
    {
        public Task StartUpiPaymentAsync(OrderResponse order,Users user)
        {
            var uri = Uri.Parse(
            $"upi://pay" +
            $"?pa=8693849475-2@ybl" +
            $"&pn=Order Payment{user.Mobile}" +
            $"&tr={order.OrderId}" +
            $"&am={order.Amount}" +
            $"&cu=INR");

            Intent intent = new(Intent.ActionView);
            intent.SetData(uri);

            Intent chooser = Intent.CreateChooser(intent, "Pay using UPI");
            Platform.CurrentActivity.StartActivityForResult(chooser, 101);

            return Task.CompletedTask;
        }
    }
}
