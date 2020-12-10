using FFImageLoading;
using FFImageLoading.Forms;
using FFImageLoading.Work;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using ImageSource = Xamarin.Forms.ImageSource;
using ImageSourceConverter = FFImageLoading.Forms.ImageSourceConverter;

namespace VCHelper.Controls.ExtendedCachedImage
{
    public class ExtendedCachedImage : CachedImage
    {
        public static readonly BindableProperty FullSourceProperty = BindableProperty.Create(
            nameof(FullSource),
            typeof(ImageSource),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay,
            coerceValue: CoerceImageSource);

        public static readonly BindableProperty FullTransformationsProperty = BindableProperty.Create(
            nameof(FullTransformations),
            typeof(List<ITransformation>),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay);

        public static readonly BindableProperty IsFullImageLoadedProperty = BindableProperty.Create(
            nameof(IsFullImageLoaded),
            typeof(bool),
            typeof(ExtendedCachedImage),
            false,
            BindingMode.OneWay);

        public static readonly BindableProperty PreviewSourceProperty = BindableProperty.Create(
            nameof(PreviewSource),
            typeof(ImageSource),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay,
            coerceValue:CoerceImageSource);

        public static readonly BindableProperty PreviewTransformationsProperty = BindableProperty.Create(
            nameof(PreviewTransformations),
            typeof(List<ITransformation>),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay);

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource FullSource
        {
            get => (ImageSource)GetValue(FullSourceProperty);
            set => SetValue(FullSourceProperty, value);
        }

        public List<ITransformation> FullTransformations
        {
            get => (List<ITransformation>)GetValue(FullTransformationsProperty);
            set => SetValue(FullTransformationsProperty, value);
        }

        public bool IsFullImageLoaded
        {
            get => (bool)GetValue(IsFullImageLoadedProperty);
            set => SetValue(IsFullImageLoadedProperty, value);
        }


        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource PreviewSource
        {
            get => (ImageSource)GetValue(PreviewSourceProperty);
            set => SetValue(PreviewSourceProperty, value);
        }

        public List<ITransformation> PreviewTransformations
        {
            get => (List<ITransformation>)GetValue(PreviewTransformationsProperty);
            set => SetValue(PreviewTransformationsProperty, value);
        }

        public ExtendedCachedImage() : base()
        {
            PreviewTransformations = new List<ITransformation>();
            FullTransformations = new List<ITransformation>();
        }

        private static object CoerceImageSource(BindableObject bindable, object newValue)
        {
            return ((ExtendedCachedImage)bindable).CoerceImageSource(newValue);
        }

        private void LoadImageSource()
        {
            var source = FullSource as UriImageSource;
            IsFullImageLoaded = false;
            SetPreview();
            if (source == null)
                return;
            Task.Run(async() =>
            {
                try
                {
                    var density = DeviceDisplay.MainDisplayInfo.Density;
                    await ImageService
                        .Instance
                        .LoadUrl(source.Uri.AbsoluteUri)
                        .DownSample((int)(DownsampleWidth * density), (int)(DownsampleHeight * density), true)
                        .Success(() =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                IsFullImageLoaded = true;
                            });
                        })
                        .Error((ex) =>
                        {

                        })
                        .PreloadAsync();
                }
                catch(Exception ex)
                {

                }
            });
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            switch (propertyName)
            {
                case nameof(IsFullImageLoaded):
                case nameof(FullTransformations):
                    OnFullImageLoadedChanged();
                    break;
                case nameof(FullSource):
                    LoadImageSource();
                    break;
                case nameof(PreviewSource):
                case nameof(PreviewTransformations):
                    SetPreview();
                    break;
            }
        }

        private void SetPreview()
        {
            if (IsFullImageLoaded)
                return;
            Source = PreviewSource;
            Transformations = PreviewTransformations;
        }

        private void OnFullImageLoadedChanged()
        {
            if (IsFullImageLoaded)
            {
                Source = FullSource;
                Transformations = FullTransformations;
            }
            else
            {
                Source = null;
            }
        }
    }
}
