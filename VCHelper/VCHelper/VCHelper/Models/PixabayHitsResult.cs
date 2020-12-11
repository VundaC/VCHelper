using MvvmHelpers;

namespace VCHelper.Models
{
    public class PixabayHitsResult : ObservableObject
    {
        private string _id;
        public string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _previewURL;
        public string PreviewURL
        {
            get => _previewURL;
            set => SetProperty(ref _previewURL, value);
        }

        private string _webformatURL;
        public string WebformatURL
        {
            get => _webformatURL;
            set => SetProperty(ref _webformatURL, value);
        }

        private string _largeImageURL;
        public string LargeImageURL
        {
            get => _largeImageURL;
            set => SetProperty(ref _largeImageURL, value);
        }
    }
}
