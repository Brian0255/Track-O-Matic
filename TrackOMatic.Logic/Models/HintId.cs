namespace TrackOMatic.Logic.Models;

/// <summary>
/// A strongly typed hint identifier that is 8 characters long. This is used to uniquely identify hints within the game.
/// </summary>
public record HintId
{
    public string Value { get; private init; }

    public HintId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("HintId cannot be empty.", nameof(value));
        }
        if (value.Length != 8)
        {
            throw new ArgumentException("HintId must be exactly 8 characters long.", nameof(value));
        }
        Value = value;
    }

    public HintId(Guid guid)
    {
        if (guid.Equals(Guid.Empty))
        {
            throw new ArgumentException("HintId cannot be all zeroes.", nameof(guid));
        }

        Value = guid.ToString()[..8];
    }

    public override string ToString() => Value;
    public static implicit operator string(HintId hintId) => hintId.Value;
}
