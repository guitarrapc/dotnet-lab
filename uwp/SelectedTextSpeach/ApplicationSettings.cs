using System.Collections.Generic;

namespace SelectedTextSpeach
{
    public static class ApplicationSettings
    {
        public static List<(int order, string Key, string Value)> HarryPotterStoryTextResources => new List<(int, string, string)>{
            ( 0, "HarrySorcererStoneCharter2SentenceTitle1/Text", "HarrySorcererStoneCharter2Sentence1/Text"),
            ( 1, "HarrySorcererStoneCharter2SentenceTitle2/Text", "HarrySorcererStoneCharter2Sentence2/Text"),
            ( 2, "HarrySorcererStoneCharter2SentenceTitle3/Text", "HarrySorcererStoneCharter2Sentence3/Text"),
            ( 3, "HarrySorcererStoneCharter2SentenceTitle4/Text", "HarrySorcererStoneCharter2Sentence4/Text"),
            ( 4, "HarrySorcererStoneCharter2SentenceTitle5/Text", "HarrySorcererStoneCharter2Sentence5/Text"),
            ( 5, "HarrySorcererStoneCharter2SentenceTitle6/Text", "HarrySorcererStoneCharter2Sentence6/Text"),
            ( 6, "HarrySorcererStoneCharter2SentenceTitle7/Text", "HarrySorcererStoneCharter2Sentence7/Text"),
        };
    }
}
