using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using VCHelper.Models;
using VCHelper.Services.Interfaces;
using Xamarin.Forms;

namespace VCHelper.ViewModels
{
    public class CachedImagesPageViewModel : BaseViewModel
    {
        #region variables
        private const int PageSize = 30;

        private ObservableRangeCollection<PixabayHitsResult> _images;
        public ObservableRangeCollection<PixabayHitsResult> Images
        {
            get => _images;
            private set => SetProperty(ref _images, value);
        }

        private bool _isFailed;
        public bool IsFailed
        {
            get => _isFailed;
            private set => SetProperty(ref _isFailed, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        #endregion

        #region services
        private IImagesService ImagesService => DependencyService.Get<IImagesService>();
        #endregion

        #region commands
        private ICommand _retryCommand;
        public ICommand RetryCommand =>
          _retryCommand ??= new Command(Initialize);
        #endregion

        public CachedImagesPageViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            try
            {
                Images = new ObservableRangeCollection<PixabayHitsResult>();
                var images = await ImagesService.GetImagesAsync(PageSize);
                Images = new ObservableRangeCollection<PixabayHitsResult>(images);
            }
            catch(Exception ex)
            {
                IsFailed = true;
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
