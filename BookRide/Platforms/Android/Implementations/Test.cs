using BookRide.Interfaces;
using BookRide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  
namespace BookRide.Platforms.Android.Implementations
{
    public class Test : ITest
    {
        public string GetValue()
        {
            return "This is a test implementation for Android";
        }
    }
}
