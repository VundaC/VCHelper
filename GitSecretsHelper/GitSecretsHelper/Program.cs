using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GitSecretsHelper
{
	class Program
	{
		private const string relative = "../../../../../";

		static void Main(string[] args)
		{
			var command = Console.ReadLine();
			switch (command)
			{
				case "encrypt":
				case "hide": EncodeFilesAsync().Wait(); break;
				case "decode":
				case "reveal" : DecodeFilesAsync().Wait(); break;
				default: break;
			}
			if (command.StartsWith("addfile "))
			{
				//addfile to mapping.cfg.
				//addfile to gitignore;
			}
#if DEBUG
			Console.ReadLine();
#endif
		}

		private static async Task DecodeFilesAsync()
		{
			try
			{
				var path_to_mapping = ".gitsecret/paths/mapping.cfg";
#if DEBUG
			path_to_mapping = relative + path_to_mapping;
#endif
				var files = await File.ReadAllLinesAsync(path_to_mapping);
				if (files?.Any() == true)
				{
					foreach (var filePath in files)
					{
						var path = filePath.Substring(0, filePath.IndexOf(":"));
						Console.WriteLine(path);
#if DEBUG
						path = relative + path;
#endif
						if (File.Exists(path))
							File.Delete(path);
						Process.Start("cmd", $"/c \" gpg --output {path} --decrypt {path}.secret \"");
					}
				}

				Console.WriteLine("Finish decoding");
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION DecodeFilesAsync() : {ex.Message}\n{ex}");
				Console.WriteLine($"EXCEPTION: {ex.Message}\n{ex}");
			}
		}


		private static async Task EncodeFilesAsync()
		{
			try
			{
				var path_to_mapping = ".gitsecret/paths/mapping.cfg";
#if DEBUG
				path_to_mapping = relative + path_to_mapping;
#endif


				var assembly = typeof(Program).Assembly;
				// json file contains emails property with array of emails for recipients
				using var emailsstrem = assembly.GetManifestResourceStream("GitSecretsHelper.encrypt_emails.json");
				using var reader = new StreamReader(emailsstrem);
				var emailsstr = await reader.ReadToEndAsync();
				var jobj = JObject.Parse(emailsstr);
				var emails = jobj["emails"].ToObject<List<string>>();
				var strrecipients = "";
				foreach (var email in emails)
					strrecipients += $" --recipient {email} ";

				var files = await File.ReadAllLinesAsync(path_to_mapping);
				if (files?.Any() == true)
				{
					foreach (var filePath in files)
					{
						var path = filePath.Substring(0, filePath.IndexOf(":"));
						Console.WriteLine(path);
#if DEBUG
						path = relative + path;
#endif
						if (File.Exists($"{path}.secret"))
							File.Delete($"{path}.secret");
						var command = $"/c \" gpg --output {path}.secret --encrypt {strrecipients} {path} \"";
						Process.Start("cmd", command);
					}
				}

				Console.WriteLine("Finish encoding");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"EXCEPTION: {ex.Message}\n{ex}");
			}
		}

		private static async Task AddFiledToSecretsAsync()
		{

		} 
	}
}
