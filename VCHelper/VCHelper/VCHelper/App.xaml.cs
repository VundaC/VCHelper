using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.MobileBlazorBindings;
using VCHelper.Blazor.Services;
﻿using VCHelper.Services.Implementations;
using VCHelper.Services.Interfaces;
using Xamarin.Forms;

namespace VCHelper
{
	public partial class App : Application
	{
		public static IHost Host { get; private set; }

		public App(IFileProvider fileProvider = null)
		{
			InitializeComponent();

			DependencyService.Register<ICommandService, CommandService>();
            Config.Config.Load();
            //Register services here
            DependencyService.RegisterSingleton<IImagesService>(new ImagesService());

			var hostBuilder = MobileBlazorBindingsHost.CreateDefaultBuilder()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddBlazorHybrid();
					services.AddSingleton(DependencyService.Resolve<ICommandService>());
				})
				.UseWebRoot("wwwroot");

			if (fileProvider != null)
			{
				hostBuilder.UseStaticFiles(fileProvider);
			}
			else
			{
				hostBuilder.UseStaticFiles();
			}

			Host = hostBuilder.Build();

			MainPage = new AppShell();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
