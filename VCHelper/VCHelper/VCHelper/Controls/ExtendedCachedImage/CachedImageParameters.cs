using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using ImageSource = Xamarin.Forms.ImageSource;
using ImageSourceConverter = FFImageLoading.Forms.ImageSourceConverter;
using TypeConverter = Xamarin.Forms.TypeConverter;
using TypeConverterAttribute = Xamarin.Forms.TypeConverterAttribute;

namespace VCHelper.Controls.ExtendedCachedImage
{
    public class CachedImageParameters : BindableObject
    {
        public static BindableProperty SourceProperty = BindableProperty.Create(
            nameof(Source),
            typeof(ImageSource),
            typeof(CachedImageParameters),
            default,
            BindingMode.OneWay,
            coerceValue: CoerceImageSource);

        public static BindableProperty TransformationsProperty = BindableProperty.Create(
            nameof(Transformations),
            typeof(List<ITransformation>),
            typeof(CachedImageParameters),
            default,
            BindingMode.OneWay);

        public static BindableProperty IsLoadedProperty = BindableProperty.Create(
            nameof(IsLoaded),
            typeof(bool),
            typeof(CachedImageParameters),
            false,
            BindingMode.TwoWay);

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public List<ITransformation> Transformations
        {
            get => (List<ITransformation>)GetValue(TransformationsProperty);
            set => SetValue(TransformationsProperty, value);
        }

        public bool IsLoaded
        {
            get => (bool)GetValue(IsLoadedProperty);
            set => SetValue(IsLoadedProperty, value);
        }

        public CachedImageParameters()
        {
            Transformations = new List<ITransformation>();
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
        }

        protected virtual ImageSource CoerceImageSource(object newValue)
        {
            var uriImageSource = newValue as UriImageSource;

            if (uriImageSource?.Uri?.OriginalString != null)
            {
                if (uriImageSource.Uri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
                    return Xamarin.Forms.ImageSource.FromFile(uriImageSource.Uri.LocalPath);

                if (uriImageSource.Uri.Scheme.Equals("resource", StringComparison.OrdinalIgnoreCase))
                    return new EmbeddedResourceImageSource(uriImageSource.Uri);

                if (uriImageSource.Uri.OriginalString.IsDataUrl())
                    return new DataUrlImageSource(uriImageSource.Uri.OriginalString);
            }

            return newValue as ImageSource;
        }


        private static object CoerceImageSource(BindableObject bindable, object newValue)
        {
            return ((CachedImageParameters)bindable).CoerceImageSource(newValue);
        }
    }
}
