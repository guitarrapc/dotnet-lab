using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SelectedTextSpeach.Models.UseCases
{
    public enum SpeechLanugage
    {
        en,
        ja,
    }
    public interface IContentReader
    {
        Action<object, RoutedEventArgs> SeekCompletedAction { get; set; }
        bool IsPlaying { get; }
        bool IsPaused { get; }
        bool IsStopped { get; }

        void SetLanguage(SpeechLanugage language);
        void SetVoice(VoiceGender gender);
        Task SetContent(string content);
        void StartReadContent();
        void StopReadContent();
        void PauseReadContent();
    }


    public class ContentReaderUseCase : IContentReader
    {
        public Action<object, RoutedEventArgs> SeekCompletedAction { get; set; }
        public bool IsPlaying => MediaElementItem.CurrentState == MediaElementState.Playing;
        public bool IsPaused => MediaElementItem.CurrentState == MediaElementState.Paused;
        public bool IsStopped => MediaElementItem.CurrentState == MediaElementState.Stopped;

        private readonly MediaElement MediaElementItem = new MediaElement();
        private VoiceInformation voice = null;
        private string language = "en-US";

        public void SetLanguage(SpeechLanugage language)
        {
            switch (language)
            {
                case SpeechLanugage.en:
                    this.language = "en-US";
                    break;
                case SpeechLanugage.ja:
                    this.language = "ja-JP";
                    break;
                default:
                    this.language = "en-US";
                    break;
            }
        }
        public void SetVoice(VoiceGender gender)
        {
            var genders = SpeechSynthesizer.AllVoices.Where(x => x.Gender == gender);
            if (genders.Any(x => x.Language == language))
            {
                voice = genders.Where(x => x.Language == language)
                    .First();
            }
            else
            {
                voice = genders.First();
            }
        }

        public async Task SetContent(string content)
        {
            var resourceLoader = await StringsResourcesHelpers.SafeGetForCurrentViewAsync();
            using (var synth = new SpeechSynthesizer())
            {
                if (voice != null)
                {
                    synth.Voice = voice;
                }
                var stream = await synth.SynthesizeTextToStreamAsync(content);
                MediaElementItem.SetSource(stream, stream.ContentType);
            }
        }

        public void StartReadContent()
        {
            if (MediaElementItem.CurrentState == MediaElementState.Paused)
            {
                MediaElementItem.Play();
            }
            else
            {
                //TODO: Auto Play's end should change button text to PlayIcon
                MediaElementItem.SeekCompleted += (obj, player) =>
                {
                    SeekCompletedAction?.Invoke(obj, player);
                };
                MediaElementItem.Play();
            }
        }

        public void StopReadContent()
        {
            if (MediaElementItem.CurrentState != MediaElementState.Stopped)
            {
                MediaElementItem.Stop();
            }
        }

        public void PauseReadContent()
        {
            if (MediaElementItem.CurrentState == MediaElementState.Playing && MediaElementItem.CanPause)
            {
                MediaElementItem.Pause();
            }
        }
    }
}
