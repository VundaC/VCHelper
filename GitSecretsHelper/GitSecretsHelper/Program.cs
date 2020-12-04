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
			DecodeFilesAsync().Wait();
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
						Process.Start("cmd", $"/c \" gpg --output {path} --decrypt {path}.secret \"");
					}
				}

				Console.WriteLine("finish");
				Console.ReadLine();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"EXCEPTION DecodeFilesAsync() : {ex.Message}\n{ex}");
				Console.WriteLine($"EXCEPTION: {ex.Message}\n{ex}");
				Console.ReadLine();
			}
		}
	}
}
