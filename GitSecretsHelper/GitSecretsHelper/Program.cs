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
		static void Main(string[] args)
		{
			try
			{
				var command = Console.ReadLine();
				switch (command)
				{
					case "encrypt":
					case "hide": EncodeFilesAsync().Wait(); break;
					case "decode":
					case "reveal": DecodeFilesAsync().Wait(); break;
					default: {
							if (command.StartsWith("addfile "))
							{
								var file = command.Replace("addfile ", "").Replace("add ", "");
								AddFiledToSecretsAsync(file).Wait();
							}
							else
							{
								Console.WriteLine("Command not correct");
							}
						} break;
				}
				Console.ReadLine();
			} catch (Exception ex)
			{
				Console.WriteLine($"Excpetion: {ex.Message}'\n{ex}");
				Console.ReadLine();
			}
		}

		private static async Task DecodeFilesAsync()
		{
			try
			{
				var path_to_mapping = ".gitsecret/paths/mapping.cfg".GetRelative();
				Console.WriteLine(path_to_mapping);
				Console.ReadLine();
				var files = await File.ReadAllLinesAsync(path_to_mapping);
				if (files?.Any() == true)
				{
					foreach (var filePath in files)
					{
						var path = filePath.Substring(0, filePath.IndexOf(":"));
						Console.WriteLine(path);
						path = path.GetRelative();

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
				var path_to_mapping = ".gitsecret/paths/mapping.cfg".GetRelative();

				var assembly = typeof(Program).Assembly;
				// json file contains emails property with array of emails for recipients
				var email_path = ".gitsecret/encrypt_emails.json";
				if(!File.Exists(email_path))
				{
					Console.WriteLine($"Cannot find file: {email_path}");
					Console.ReadLine();
					return;
				}
				var emailsstr = await File.ReadAllTextAsync(email_path);
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
						path = path.GetRelative();

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

		private static async Task AddFiledToSecretsAsync(string file)
		{

			if (!File.Exists(file)) throw new ArgumentException("filename");

			var path_to_mapping = ".gitsecret/paths/mapping.cfg".GetRelative();

			var files = await File.ReadAllLinesAsync(path_to_mapping);
			Console.WriteLine($"Already in secret {files.Length} files");
			if (files.FirstOrDefault(x => x.StartsWith(file)) != null)
				throw new ArgumentException("file already added");
			await File.AppendAllLinesAsync(path_to_mapping, new[] { $"{file}:{Guid.NewGuid()}" });

			var gitignorepath = ".gitignore".GetRelative();
			if (!File.Exists(gitignorepath))
			{
				Console.WriteLine("Create .gitignore");
				File.Create(gitignorepath);
			};
			await File.AppendAllLinesAsync(gitignorepath, new[] { file });
			Console.WriteLine("Append file to .gitignore");

			var command = $"/c \" git rm --cahced {file} \"";
		}
	}


	public static class PathExtenstions
	{
		public static string GetRelative(this string path)
		{
#if DEBUG
			return "../../../../../" + path;
#endif
			return path;
		}
	}
}
