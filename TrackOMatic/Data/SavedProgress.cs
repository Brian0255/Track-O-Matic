using System.Collections.Generic;

namespace TrackOMatic
{
    public class SavedProgress
    {
        public Dictionary<ItemName, SavedItem> SavedItems { get; }
        public Dictionary<RegionName, string> SavedGBCounts { get; set; }
        public Dictionary<RegionName, int> BLockerImageIndexes { get; set; }
        public List<int> HelmDoorImageIndexes { get; set; }
        public List<string> HelmDoorCounts { get; set; }
        public List<SavedHint> SavedHints { get; }
        public string spoilerPath { get; set; }

        public SavedProgress()
        {
            SavedItems = new();
            SavedHints = new();
            SavedGBCounts = new();
            BLockerImageIndexes = new();
            HelmDoorImageIndexes = new();
            HelmDoorCounts = new();
            spoilerPath = "";
        }
    }
}
