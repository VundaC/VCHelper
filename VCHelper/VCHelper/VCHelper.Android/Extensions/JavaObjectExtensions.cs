using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VCHelper.Droid.Extensions
{
    public static class JavaObjectExtensions
    {
        public static bool IsDisposed(this Java.Lang.Object obj)
        {
            return obj.Handle == IntPtr.Zero;
        }

        public static bool IsAlive(this Java.Lang.Object obj)
        {
            if (obj == null)
                return false;

            return !obj.IsDisposed();
        }

        public static bool IsDisposed(this global::Android.Runtime.IJavaObject obj)
        {
            return obj.Handle == IntPtr.Zero;
        }

        public static bool IsAlive(this global::Android.Runtime.IJavaObject obj)
        {
            if (obj == null)
                return false;

            return !obj.IsDisposed();
        }
    }
}