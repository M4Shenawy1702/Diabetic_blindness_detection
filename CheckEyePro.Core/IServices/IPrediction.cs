using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckEyePro.Core.IServices
{
    public interface IPrediction
    {
       Task<string> PredectApi(IFormFile Img);
    }
}
