namespace TrackOMatic.Logic.Models.Autotracking;

/// <summary>
/// A record that confirms the player is playing Donkey Kong 64 Randomizer.
/// </summary>
/// <param name="TargetAddress"></param>
/// <param name="TotalBits"></param>
/// <param name="TargetValue"></param>
public record GameVerificationInfo(uint TargetAddress, int TotalBits, uint TargetValue);
