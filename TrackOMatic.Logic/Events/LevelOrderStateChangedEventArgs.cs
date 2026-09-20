using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Logic.Events;

/// <summary>
/// Represents the event arguments for changing which level each region is associated with.
/// </summary>
/// <param name="oldValue"></param>
/// <param name="newValue"></param>
/// <param name="changeReason"></param>
public class LevelOrderStateChangedEventArgs(List<int> oldValue, List<int> newValue, ChangeReason changeReason = ChangeReason.Unknown) : ValueChangedEventArgs<List<int>>(oldValue, newValue, changeReason)
{
}
