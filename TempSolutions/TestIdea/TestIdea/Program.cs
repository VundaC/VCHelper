using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
				var request = new HttpRequestMessage(HttpMethod.Get, $"https://pixabay.com/api/?key={key}&per_page=50&order=latest");
				var response = await client.SendAsync(request);
				var content = await response.Content.ReadAsStringAsync();
				jObj = JObject.Parse(content);

				var images = jObj["hits"].ToObject<List<PixabayHitsResult>>();

				Console.WriteLine("start");
				var watch = new Stopwatch();
				watch.Start();

				List<Task> tasks = new List<Task>();
				foreach(var image in images)
				{
					using var webclient = new WebClient();
					var task = Task.Run(async () => await webclient.DownloadFileTaskAsync(image.LargeImageURL, $"{image.Id}-1.jpg"));
					tasks.Add(task);
				}
				await Task.WhenAll(tasks);

				watch.Stop();
				var result1 = watch.Elapsed.Ticks.ToString("00000000000");

				var tcs = new TaskCompletionSource<bool>();
				watch.Reset();
				watch.Start();
				Task.Run(async () =>
				{
					using var webclient = new WebClient();
					foreach (var image in images)
					{
						await webclient.DownloadFileTaskAsync(image.LargeImageURL, $"{image.Id}-2.jpg");
					}
					tcs.TrySetResult(true);
				});
				await tcs.Task;
				watch.Stop();
				var result2 = watch.Elapsed.Ticks.ToString("00000000000");


				Console.WriteLine($"1: {result1}");
				Console.WriteLine($"2: {result2}");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION LoadImages() : {ex.Message}\n{ex}");
			}
		}
	}


	public class PixabayHitsResult
	{
		public string Id { get; set; }
		public string PreviewURL { get; set; }
		public string WebformatURL { get; set; }
		public string LargeImageURL { get; set; }
	}
}
