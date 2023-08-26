using System.Windows;

namespace TrackOMatic
{
    public class SavedItem
    {
        public ItemName ItemName { get; }
        public RegionName Region { get; }
        public Visibility Starred { get; }
        public bool Autotracked { get; set; }
        public double Opacity { get; }
        public bool Hinted { get; }
        public SavedItem(ItemName itemName, RegionName region, Visibility starred, bool autotracked, double opacity, bool hinted = false)
        {
            ItemName = itemName;
            Region = region;
            Starred = starred;
            Autotracked = autotracked;
            Opacity = opacity;
            Hinted = hinted;
        }
    }
}
