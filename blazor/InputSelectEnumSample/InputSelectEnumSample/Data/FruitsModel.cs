using System;
using System.ComponentModel.DataAnnotations;

namespace InputSelectEnumSample.Data
{
    [Flags]
    public enum FruitType
    {
        [Display(Name = "🍎")]
        Apple,
        [Display(Name = "🥑")]
        Avocado,
        [Display(Name = "🍌")]
        Banana,
        [Display(Name = "🍒")]
        Cherries,
        [Display(Name = "🍇")]
        Grapes,
        [Display(Name = "🥝")]
        Kiwi,
        [Display(Name = "🍈")]
        Melon,
        [Display(Name = "🍍")]
        Pineapple,
        [Display(Name = "🍑")]
        Peach,
        [Display(Name = "🍓")]
        Strawberry,
        [Display(Name = "🍊")]
        Tangerine,
    }

    public class FruitsModel
    {
        public FruitType Fruit { get; set; }
    }
}
