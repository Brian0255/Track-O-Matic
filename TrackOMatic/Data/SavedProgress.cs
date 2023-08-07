using System.Collections.Generic;

namespace TrackOMatic
{
    public class SavedProgress
    {
        public string SpoilerLogName { get; }
        public Dictionary<ItemName, SavedItem> SavedItems { get; }

        public SavedProgress(string spoilerLogName)
        {
            SpoilerLogName = spoilerLogName;
            SavedItems = new();
        }
    }
}
