using TrackOMatic.Logic.Enums;

namespace TrackOMatic.Services;

/// <summary>
/// A data class representing an item located on a grid.
/// </summary>
/// <param name="Row">The row index of the cell in the grid.</param>
/// <param name="Column">The column index of the cell in the grid.</param>
/// <param name="ItemName">The item name that will be turned into a resource key for rendering purposes.</param>
public record HintGridCell(int Row, int Column, ItemName ItemName);
