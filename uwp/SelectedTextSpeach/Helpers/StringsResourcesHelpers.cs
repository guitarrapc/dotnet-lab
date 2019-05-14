using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SelectedTextSpeach
{
    // About String Resources : https://docs.microsoft.com/ja-jp/windows/uwp/app-resources/localize-strings-ui-manifest
    public static class StringsResourcesHelpers
    {
        public static async Task<ResourceLoader> SafeGetForCurrentViewAsync()
        {
            ResourceLoader loader = null;
            if (CoreWindow.GetForCurrentThread() != null)
            {
                loader = ResourceLoader.GetForCurrentView();
            }
            else
            {
                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    loader = ResourceLoader.GetForCurrentView();
                });
            }
            return loader;
        }

        public static async Task<ResourceLoader> SafeGetForCurrentViewAsync(string name)
        {
            ResourceLoader loader = null;
            if (CoreWindow.GetForCurrentThread() != null)
            {
                loader = ResourceLoader.GetForCurrentView(name);
            }
            else
            {
                await Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    loader = ResourceLoader.GetForCurrentView(name);
                });
            }
            return loader;
        }
    }
}
