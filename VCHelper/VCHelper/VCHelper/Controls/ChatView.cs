using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace VCHelper.Controls
{
    public class ChatView : ItemsView
    {
        public static BindableProperty RemainingStartItemsThresholdProperty = BindableProperty.Create(
            nameof(RemainingStartItemsThreshold),
            typeof(int),
            typeof(ChatView),
            -1,
            BindingMode.OneWay);

        public static BindableProperty RemainingStartItemsThresholdCommandProperty = BindableProperty.Create(
            nameof(RemainingStartItemsThresholdCommand),
            typeof(ICommand),
            typeof(ChatView),
            default,
            BindingMode.OneWay);

        public static BindableProperty RemainingStartItemsThresholdCommandParameterProperty = BindableProperty.Create(
            nameof(RemainingStartItemsThresholdCommandParameter),
            typeof(object),
            typeof(ChatView),
            null,
            BindingMode.OneWay);

        public static BindableProperty ReadMessageCommandProperty = BindableProperty.Create(
            nameof(ReadMessageCommand),
            typeof(ICommand),
            typeof(ChatView),
            default,
            BindingMode.OneTime);

        public int RemainingStartItemsThreshold
        {
            get => (int)GetValue(RemainingStartItemsThresholdProperty);
            set => SetValue(RemainingStartItemsThresholdProperty, value);
        }

        public ICommand RemainingStartItemsThresholdCommand
        {
            get => (ICommand)GetValue(RemainingStartItemsThresholdCommandProperty);
            set => SetValue(RemainingStartItemsThresholdCommandProperty, value);
        }

        public object RemainingStartItemsThresholdCommandParameter
        {
            get => GetValue(RemainingStartItemsThresholdCommandParameterProperty);
            set => SetValue(RemainingStartItemsThresholdCommandParameterProperty, value);
        }

        public ICommand ReadMessageCommand
        {
            get => (ICommand)GetValue(ReadMessageCommandProperty);
            set => SetValue(ReadMessageCommandProperty, value);
        }

        public void SendRemainingStartItemsThresholdReached()
        {
            RemainingStartItemsThresholdCommand?.Execute(RemainingStartItemsThresholdCommandParameter);
        }

        protected override void OnScrolled(ItemsViewScrolledEventArgs e)
        {
            base.OnScrolled(e);

            if (!(ItemsSource is IEnumerable<object> source))
                return;

            for (int i = e.FirstVisibleItemIndex; i <= e.LastVisibleItemIndex; i++)
            {
                var message = source.ElementAtOrDefault(i);
                ReadMessageCommand?.Execute(message);
            }
        }
    }
}
