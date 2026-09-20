namespace TrackOMatic.Services;

/// <summary>
/// The collection of settings that the user does not directly control, but should still be stored and
/// remembered for future usages of Track-O-Matic.
/// </summary>
public class ApplicationStateData
{
    // Window state
    public double WindowX { get; set; } = 100;
    public double WindowY { get; set; } = 100;
    public double DesiredWidth { get; set; } = 1800;
    public double DesiredHeight { get; set; } = 820;

    // Recent file
    public string LastFolderPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    // Progressive hints (runtime state determined by autotracking/spoiler log parsing)
    public int ProgressiveHintItem { get; set; } = 9;
    public int ProgressiveHintCap { get; set; } = 50;
}
