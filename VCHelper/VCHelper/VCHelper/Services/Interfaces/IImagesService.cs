using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VCHelper.Models;

namespace VCHelper.Services.Interfaces
{
    public interface IImagesService
    {
        Task<IEnumerable<PixabayHitsResult>> GetImagesAsync(int pageSize);
    }
}
