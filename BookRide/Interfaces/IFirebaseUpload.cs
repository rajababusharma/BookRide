using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookRide.Interfaces
{
    public interface IFirebaseUpload
    {
        Task<string> UploadProfieImagesToCloud(Stream filestream,string filename);
        Task<string> UploadAadharImagesToCloud(Stream filestream, string filename);
        Task<string> UploadPaymentImagesToCloud(Stream filestream, string filename);
    }
}
