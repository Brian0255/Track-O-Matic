using System.ComponentModel;

using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services;
public interface IUserSettingsService : INotifyPropertyChanged
{
    #region Display Toggles
    bool TopMost { get; set; }
    bool SongDisplay { get; set; }
    bool CompactMode { get; set; }
    bool HelmInLevelOrder { get; set; }
    bool HelmDoors { get; set; }
    bool AutoSortPathHints { get; set; }
    bool EnemiesInAutofill { get; set; }
    bool ColoredBarrelPadMoves { get; set; }
    bool Autotracking { get; set; }

    #endregion

    #region Collectibles Display
    bool ShowTotalBPs { get; set; }
    bool ShowCompanyCoins { get; set; }
    #endregion

    #region Spoiler Hint Display
    bool ShowKRoolOrder { get; set; }
    bool ShowHelmOrder { get; set; }
    #endregion

    #region Broadcast View
    bool BroadcastViewActive { get; set; }
    bool BroadcastHelmKRool { get; set; }
    bool BroadcastShopkeepers { get; set; }
    bool BroadcastSongDisplay { get; set; }
    BroadcastNumberLabel BroadcastNumberLabel { get; set; }
    #endregion

    #region Misc Items
    HintDisplayMode HintDisplay { get; set; }

    bool ShowAmountForHints { get; set; }
    #endregion
}
