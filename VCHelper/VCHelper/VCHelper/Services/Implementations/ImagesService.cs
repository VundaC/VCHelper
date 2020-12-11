using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using VCHelper.Models;
using VCHelper.Services.Interfaces;

namespace VCHelper.Services.Implementations
{
    public class ImagesService : IImagesService
    {
        public ImagesService()
        {
        }

        public async Task<IEnumerable<PixabayHitsResult>> GetImagesAsync(int pageSize)
        {
            try
            {
                using var client = new HttpClient();
                var key = Config.Config.Instance.PixabayKey;
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://pixabay.com/api/?key={key}&per_page={pageSize}&order=latest");
                var response = await Task.Run(async() => await client.SendAsync(request));
                var content = await response.Content.ReadAsStringAsync();
                var jObj = JObject.Parse(content);

                var images = jObj["hits"].ToObject<List<PixabayHitsResult>>();
                return images;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
