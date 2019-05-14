namespace SelectedTextSpeach.Models.Entities
{
    public class StoryEntity
    {
        public string Title { get; private set; }
        public string Content { get; private set; }

        private int TitleHash;

        public StoryEntity(string title, string content)
        {
            Title = title;
            Content = content;
            TitleHash = Title.GetHashCode();
        }
    }
}
