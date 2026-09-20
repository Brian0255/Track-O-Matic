namespace TrackOMatic.Logic.Enums;

/// <summary>
/// Concrete hint panel types that can be targeted by the hint service.
/// Each value corresponds to a specific hint panel XAML control in the primary window.
/// </summary>
public enum HintPanelType
{
    // Direct item hint panels (9 total: 8 regions + Helm)
    /// <summary>Isles region direct item hints (IslesPanel)</summary>
    Isles = 0,

    /// <summary>Aztec region direct item hints (AztecPanel)</summary>
    Aztec = 1,

    /// <summary>Galleon region direct item hints (GalleonPanel)</summary>
    Galleon = 2,

    /// <summary>Caves region direct item hints (CavesPanel)</summary>
    Caves = 3,

    /// <summary>Japes region direct item hints (JapesPanel)</summary>
    Japes = 4,

    /// <summary>Factory region direct item hints (FactoryPanel)</summary>
    Factory = 5,

    /// <summary>Forest region direct item hints (ForestPanel)</summary>
    Forest = 6,

    /// <summary>Castle region direct item hints (CastlePanel)</summary>
    Castle = 7,

    /// <summary>Helm region direct item hints (HelmPanel)</summary>
    Helm = 8,

    // Non-region-specific hint panels (5 total)
    /// <summary>Multipath hints panel (PathsPanel)</summary>
    Paths = 9,

    /// <summary>Foolish regions hint panel (FoolishPanel)</summary>
    Foolish = 10,

    /// <summary>Kongs hint panel (KongsPanel)</summary>
    Kongs = 11,

    /// <summary>Way of the Hoard hint panel (WotHPanel)</summary>
    WayOfTheHoard = 12,

    /// <summary>Region potion counts hint panel (PotionCountsPanel)</summary>
    PotionCounts = 13,

    /// <summary>Unhinted moves hint panel (UnhintedPanel)</summary>
    Unhinted = 14,
}
