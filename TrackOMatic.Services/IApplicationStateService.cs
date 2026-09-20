using System.ComponentModel;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services;

public interface IApplicationStateService : INotifyPropertyChanged
{
    // Window state
    double WindowX { get; set; }
    double WindowY { get; set; }
    double DesiredWidth { get; set; }
    double DesiredHeight { get; set; }

    // Recent file
    string LastFolderPath { get; set; }

    // Progressive hints (runtime state determined by autotracking/spoiler log parsing)
    ItemType ProgressiveHintItem { get; set; }
    int ProgressiveHintCap { get; set; }
}
