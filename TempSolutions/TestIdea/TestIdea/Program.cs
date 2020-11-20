using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestIdea
{
	class Program
	{
		//https://pixabay.com/api/docs/
		static void Main(string[] args)
		{
			LoadImages().Wait();
		}

		private static async Task LoadImages()
		{
			try
			{
				var assembly = typeof(Program).Assembly;

				var resource = assembly.GetManifestResourceNames();
				using var mStream = assembly.GetManifestResourceStream("TestIdea.keys.json");
				using var reader = new StreamReader(mStream);
				var json = reader.ReadToEnd();
				var jObj = JObject.Parse(json);
				var key = jObj["pixabay_key"].ToString();
				var client = new HttpClient();
				var request = new HttpRequestMessage(HttpMethod.Get, $"https://pixabay.com/api/?key={key}&per_page=3");
				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine(content);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION LoadImages() : {ex.Message}\n{ex}");
			}
		}
	}
}
