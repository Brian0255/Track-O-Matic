using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Models;

public class SavedProgress
{
    public Dictionary<ItemName, SavedItem> SavedItems { get; }
    public Dictionary<RegionName, string> SavedGBCounts { get; set; }
    public Dictionary<RegionName, int> BLockerImageIndexes { get; set; }
    public List<int> HelmDoorImageIndexes { get; set; }
    public List<string> HelmDoorCounts { get; set; }
    public List<SavedHint> SavedHints { get; }
    public string spoilerPath { get; set; }
    public List<int>? HelmKongs { get; set; }
    public List<int>? BossKongs { get; set; }
    public List<int>? LevelOrder { get; set; }

    public SavedProgress()
    {
        SavedItems = [];
        SavedHints = [];
        SavedGBCounts = [];
        BLockerImageIndexes = [];
        HelmDoorImageIndexes = [];
        HelmDoorCounts = [];
        spoilerPath = "";
    }
}
