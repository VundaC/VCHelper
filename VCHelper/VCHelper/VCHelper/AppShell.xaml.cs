using System;
using System.Collections.Generic;
using VCHelper.ViewModels;
using VCHelper.Views;
using Xamarin.Forms;

namespace VCHelper
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        }
    }
}
