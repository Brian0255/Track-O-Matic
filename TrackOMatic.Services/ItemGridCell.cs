namespace TrackOMatic.Services;

/// <summary>
/// A data class consistent of coordinates for a grid and a resource key that will be turned into an image.
/// </summary>
/// <param name="Row">The row index of the cell in the grid.</param>
/// <param name="Column">The column index of the cell in the grid.</param>
/// <param name="ResourceKey">The resource key mapping to an image resource.</param>
public record ItemGridCell(int Row, int Column, string ResourceKey);
