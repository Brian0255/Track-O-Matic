namespace TrackOMatic
{
    public class SavedItem
    {
        public ItemName ItemName { get; }
        public RegionName Region { get; }
        public ItemVisibilityState Starred { get; }
        public bool Autotracked { get; set; }
        public double Opacity { get; }
        public bool Hinted { get; }
        public SavedItem(ItemName itemName, RegionName region, ItemVisibilityState starred, bool autotracked, double opacity, bool hinted = false)
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
