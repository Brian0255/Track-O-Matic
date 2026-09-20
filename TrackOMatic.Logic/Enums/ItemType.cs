namespace TrackOMatic.Logic.Enums;

public enum ItemType
{
    MISC = 0,
    /// <summary>
    /// A move granted by the Banana Fairy Queen.
    /// </summary>
    FAIRY_MOVE,
    /// <summary>
    /// A move that is shared by all of the Kongs.
    /// </summary>
    SHARED_MOVE,
    /// <summary>
    /// A specialized shared move that is taught as training by Cranky Kong.
    /// </summary>
    TRAINING_MOVE,
    /// <summary>
    /// A move that fires a projectile.
    /// </summary>
    GUN,
    /// <summary>
    /// A move that plays a musical instrument.
    /// </summary>
    INSTRUMENT,
    /// <summary>
    /// A move that showcases a Kong's physical attributes.
    /// </summary>
    PHYSICAL_MOVE,
    /// <summary>
    /// A move that is powered by jumping into a barrel.
    /// </summary>
    BARREL_MOVE,
    /// <summary>
    /// A move that is powered by standing on a pad.
    /// </summary>
    PAD_MOVE,
    GOLDEN_BANANA,

    /// <summary>
    /// The yellow blueprints found by Donkey Kong.
    /// </summary>
    /// <remarks>It is advised to not reorder any of these blueprint values.</remarks>
    DONKEY_BLUEPRINT,
    /// <summary>
    /// The yellow blueprints turned in by Donkey Kong.
    /// </summary>
    DONKEY_BLUEPRINT_TURNED,
    /// <summary>
    /// The red blueprints found by Diddy Kong.
    /// </summary>
    DIDDY_BLUEPRINT,
    /// <summary>
    /// The red blueprints turned in by Diddy Kong.
    /// </summary>
    DIDDY_BLUEPRINT_TURNED,
    /// <summary>
    /// The blue blueprints found by Lanky Kong.
    /// </summary>
    LANKY_BLUEPRINT,
    /// <summary>
    /// The blue blueprints turned in by Lanky Kong.
    /// </summary>
    LANKY_BLUEPRINT_TURNED,
    /// <summary>
    /// The purple blueprints found by Tiny Kong.
    /// </summary>
    TINY_BLUEPRINT,
    /// <summary>
    /// The purple blueprints turned in by Tiny Kong.
    /// </summary>
    TINY_BLUEPRINT_TURNED,
    /// <summary>
    /// The green blueprints found by Chunky Kong.
    /// </summary>
    CHUNKY_BLUEPRINT,
    /// <summary>
    /// The green blueprints turned in by Chunky Kong.
    /// </summary>
    CHUNKY_BLUEPRINT_TURNED,

    RAINBOW_COIN,
    BATTLE_CROWN,
    FAIRY,
    BANANA_MEDAL,
    PEARL,

    COMPANY_COIN,

    TOTAL_BLUEPRINTS,

    KEY,

    KONG,
    SHOPKEEPER,

    COLORED_BANANA
}
