using FFImageLoading.Forms;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VCHelper.Controls.ExtendedCachedImage
{
    public class ExtendedCachedImage : CachedImage
    {
        public static BindableProperty ImageParametersProperty = BindableProperty.Create(
            nameof(ImageParameters),
            typeof(CachedImageParameters),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay,
            propertyChanged: OnImageParametersChanged);

        public static BindableProperty PreviewParametersProperty = BindableProperty.Create(
            nameof(PreviewParameters),
            typeof(CachedImageParameters),
            typeof(ExtendedCachedImage),
            default,
            BindingMode.OneWay,
            propertyChanged: OnPreviewParametersChanged);

        public CachedImageParameters ImageParameters
        {
            get => (CachedImageParameters)GetValue(ImageParametersProperty);
            set => SetValue(ImageParametersProperty, value);
        }

        public CachedImageParameters PreviewParameters
        {
            get => (CachedImageParameters)GetValue(PreviewParametersProperty);
            set => SetValue(PreviewParametersProperty, value);
        }

        public static void OnImageParametersChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedCachedImage image)
                image.OnImageParametersChanged(oldValue as CachedImageParameters, newValue as CachedImageParameters);
        }

        private static void OnPreviewParametersChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ExtendedCachedImage image)
                image.OnPreviewParametersChanged(oldValue as CachedImageParameters, newValue as CachedImageParameters);
        }

        public virtual void OnImageParametersChanged(CachedImageParameters oldParams, CachedImageParameters newParams)
        {
            if (oldParams != null)
                oldParams.PropertyChanged -= ImageParameters_PropertyChanged;
            if(newParams != null)
                newParams.PropertyChanged += ImageParameters_PropertyChanged;

            if (newParams?.IsLoaded == true)
                Source = newParams.Source;
            else
            {
                SetPreview();

                LoadImageSource(newParams);
            }
        }

        private void ImageParameters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CachedImageParameters.Source))
                LoadImageSource(sender as CachedImageParameters);
        }

        public virtual void OnPreviewParametersChanged(CachedImageParameters oldParams, CachedImageParameters newParams)
        {
            if (oldParams != null)
                oldParams.PropertyChanged -= PreviewParameters_PropertyChanged;
            if (newParams != null)
                newParams.PropertyChanged += PreviewParameters_PropertyChanged;
            SetPreview();
        }

        private void SetPreview()
        {
            if (ImageParameters?.IsLoaded == true)
                return;

            Source = PreviewParameters?.Source;
            Transformations = PreviewParameters?.Transformations ?? new System.Collections.Generic.List<FFImageLoading.Work.ITransformation>();
        }

        private void PreviewParameters_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(CachedImageParameters.Source):
                    OnPreviewImageSourceChanged();
                    break;
            }
        }

        private void OnPreviewImageSourceChanged()
        {
            if (ImageParameters?.IsLoaded != true)
                Source = PreviewParameters?.Source;
        }

        private void LoadImageSource(CachedImageParameters newParams)
        {
            if (newParams?.Source is UriImageSource source)
            {
                Task.Run(() =>
                {
                    var density = DeviceDisplay.MainDisplayInfo.Density;
                    var work = FFImageLoading.ImageService.Instance.LoadUrl(source.Uri.AbsoluteUri);
                    FFImageLoading
                        .ImageService
                        .Instance
                        .LoadUrl(source.Uri.AbsoluteUri)
                        .DownSample((int)(DownsampleWidth * density), (int)(DownsampleHeight * density), true)
                        .Success(() =>
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                newParams.IsLoaded = true;
                                if (ImageParameters == newParams)
                                {
                                    Transformations = newParams.Transformations;
                                    Source = newParams.Source;
                                }
                            });
                        });
                });
            }
        }
    }
}
