using System.Collections.Concurrent;
using System.Collections.Generic;
using SelectedTextSpeach.Models.Entities;
using SelectedTextSpeach.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SelectedTextSpeach.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly ConcurrentDictionary<string, MediaElement> playerDictionary = new ConcurrentDictionary<string, MediaElement>();
        private readonly static Dictionary<int, string> StoryTextReferences = new Dictionary<int, string>();

        private MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // このようにe.Parameterで前のページから渡された値を取得できます。
            // 値はキャストして取り出します。
            var param = e.Parameter as string;

            base.OnNavigatedTo(e);

            // Clear the Stack
            Frame.BackStack.Clear();
        }

        public async void NavigateChoiceArtifactPage(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChoiceArtifactsPage), "hogemoge");
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedItem = listView.SelectedItem as PersonEntity;
        }

        private void TextBoxInput_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.TextBoxSelection.Value = textBoxInput.SelectedText;
            label1.Text = $"Selection length is {textBoxInput.SelectionLength}";
            label2.Text = $"Selection starts at {textBoxInput.SelectionStart}";
        }
    }
}
