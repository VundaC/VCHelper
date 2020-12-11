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
			string command = "";
			while(command != "q" || command != "quit" )
				try
				{
					Console.WriteLine("encrypt/hide - encrypt files");
					Console.WriteLine("decode/reveal - decode files");
					Console.WriteLine("addfile {filename} - add file to map and giignore");
					Console.WriteLine("q/quit - close app");

					Console.Write("Write command: ");
					command = Console.ReadLine();
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

				var strrecipients = await GetEmailsRecipients();
				if (string.IsNullOrWhiteSpace(strrecipients))
					return;

				var files = await File.ReadAllLinesAsync(path_to_mapping);
				if (files?.Any() == true)
				{
					foreach (var filePath in files)
					{
						var path = filePath.Substring(0, filePath.IndexOf(":"));
						Console.WriteLine(path);
						path = path.GetRelative();

						EncryptFile(path, strrecipients);
					}
				}

				Console.WriteLine("Finish encoding");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"EXCEPTION: {ex.Message}\n{ex}");
			}
		}

		private static async Task<string> GetEmailsRecipients()
		{
			var email_path = ".gitsecret/encrypt_emails.json".GetRelative();
			if (!File.Exists(email_path))
			{
				Console.WriteLine($"Cannot find file: {email_path}");
				Console.ReadLine();
				return "";
			}
			var emailsstr = await File.ReadAllTextAsync(email_path);
			var jobj = JObject.Parse(emailsstr);
			var emails = jobj["emails"].ToObject<List<string>>();
			var strrecipients = "";
			foreach (var email in emails)
				strrecipients += $" --recipient {email} ";
			return strrecipients;
		}

		private static void EncryptFile(string path, string recipientsAtr)
		{
			if (File.Exists($"{path}.secret"))
				File.Delete($"{path}.secret");
			var command = $"/c \" gpg --output {path}.secret --encrypt {recipientsAtr} {path} \"";
			Process.Start("cmd", command);
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

			var atr = await GetEmailsRecipients();
			EncryptFile(file, atr);
			Console.WriteLine("Encrypted file");

			Console.WriteLine("Remove file from git index");
			var command = $"/c \" git rm --cahced {file} \"";
			Console.WriteLine("done");
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
