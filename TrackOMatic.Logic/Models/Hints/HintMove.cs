using System.Collections.ObjectModel;
using System.Reflection;

namespace TrackOMatic.Logic.Models.Hints;

public static class HintMove
{
    public static readonly HintNameEntry VINES = new("Vines");
    public static readonly HintNameEntry DIVING = new("Diving");
    public static readonly HintNameEntry ORANGES = new("Oranges");
    public static readonly HintNameEntry BARRELS = new("Barrels");
    public static readonly HintNameEntry CLIMBING = new("Climbing");
    public static readonly HintNameEntry BABOON_BLAST = new("Progressive Slam");
    public static readonly HintNameEntry STRONG_KONG = new("Strong Kong");
    public static readonly HintNameEntry GORILLA_GRAB = new("Gorilla Grab");
    public static readonly HintNameEntry CHIMPY_CHARGE = new("Chimpy Charge");
    public static readonly HintNameEntry ROCKETBARREL_BOOST = new("Rocketbarrel Boost");
    public static readonly HintNameEntry SIMIAN_SPRING = new("Simian Spring");
    public static readonly HintNameEntry ORANGSTAND = new("Orangstand");
    public static readonly HintNameEntry BABOON_BALLOON = new("Baboon Balloon");
    public static readonly HintNameEntry ORANGSTAND_SPRING = new("Orangstand Sprint");
    public static readonly HintNameEntry MINI_MONKEY = new("Mini Monkey");
    public static readonly HintNameEntry PONY_TAIL_TWIRL = new("Pony Tail Twirl");
    public static readonly HintNameEntry MONKEYPORT = new("Monkeyport");
    public static readonly HintNameEntry HUNKY_CHUNKY = new("Hunky Chunky");
    public static readonly HintNameEntry PRIMATE_PUNCH = new("Primate Punch");
    public static readonly HintNameEntry GORILLA_GONE = new("Gorilla Gone");
    public static readonly HintNameEntry COCONUT = new("Coconut");
    public static readonly HintNameEntry PEANUT = new("Peanut");
    public static readonly HintNameEntry GRAPE = new("Grape");
    public static readonly HintNameEntry FEATHER = new("Feather");
    public static readonly HintNameEntry PINEAPPLE = new("Pineapple");
    public static readonly HintNameEntry HOMING_AMMO = new("Homing Ammo");
    public static readonly HintNameEntry SNIPER_SIGHT = new("Sniper Sight");
    public static readonly HintNameEntry BONGOS = new("Bongos");
    public static readonly HintNameEntry GUITAR = new("Guitar");
    public static readonly HintNameEntry TROMBONE = new("Trombone");
    public static readonly HintNameEntry SAXOPHONE = new("Saxophone");
    public static readonly HintNameEntry TRIANGLE = new("Triangle");
    public static readonly HintNameEntry CAMERA = new("Camera");
    public static readonly HintNameEntry SHOCKWAVE = new("Shockwave");
    public static readonly HintNameEntry CRANKY = new("Cranky");
    public static readonly HintNameEntry FUNKY = new("Funky");
    public static readonly HintNameEntry CANDY = new("Candy");
    public static readonly HintNameEntry SNIDE = new("Snide");

    private static ReadOnlyCollection<HintNameEntry> InitializeAll() =>
        typeof(HintMove)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(HintNameEntry))
            .Select(f => (HintNameEntry)f.GetValue(null)!)
            .ToList()
            .AsReadOnly();

    private static ReadOnlyDictionary<string, HintNameEntry> InitializeByFieldName() =>
        typeof(HintMove)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(HintNameEntry))
            .ToDictionary(f => f.Name, f => (HintNameEntry)f.GetValue(null)!)
            .AsReadOnly();

    public static readonly IReadOnlyList<HintNameEntry> All = InitializeAll();

    public static readonly IReadOnlyDictionary<string, HintNameEntry> ByFieldName = InitializeByFieldName();
}
