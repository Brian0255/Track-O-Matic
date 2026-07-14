using Humanizer;

namespace TrackOMatic
{
    /// <summary>
    /// This class allows for formatting song strings from DK 64 Rando to a consistent format.
    /// </summary>
    /// <remarks>This is a potential DI service candidate.</remarks>
    public class SongFormatting
    {
        private static Dictionary<string, string> SINGLE_WORD_REPLACEMENTS { get; } = new()
        {
            {"ii","II" },
            {"iii","III" },
            {"iv","IV" },
            {"v","V" },
            {"vi","VI" },
            {"vii","VII" },
            {"viii","VIII" },
            {"ix","IX" },
            {"x","X" },
            {"xi","XI" },
            {"xii","XII" },
            {"xiii","XIII" },
            {"xiii-2","XIII-2" },
            {"xiv","XIV" },
            {"xv","XV" },
            {"xvi" ,"XVI" },
            {"zx" ,"ZX" },

            {"i:","II:" },
            {"ii:","II:" },
            {"iii:","III:" },
            {"iv:","IV:" },
            {"v:","V:" },
            {"vi:","VI:" },


            {"dkc", "DKC" },
            {"dk","DK" },
            {"isss64","ISSS64" },
            {"actraiser","ActRaiser" },
            {"antonblast","ANTONBLAST" },
            {"td","TD" },
            {"snk","SNK" },
            {"clayfighter","ClayFighter" },
            {"crosscode","CrossCode" },
            {"earthbound" ,"EarthBound" },
            {"jets'n'guns","Jets'n'Guns" },
            {"mini-land","Mini-Land" },
            {"usa","USA" },
            {"pac-man","Pac-Man" },
            {"3d","3D" },
            {"ds","DS" },
            {"rpg","RPG" },
            {"smrpg:","SMRPG:" },
            {"tmnt","TMNT" },
            {"yu-gi-oh!","Yu-Gi-Oh!" },
            {"mmmenu","MMMenu" },
            {"cccool","CCCool" },
            {"dks","DKs" },
            {"dk's" ,"DK's" },
            {"dppt","DPPT" },
            {"gsc","GSC" },
        };

        private static Dictionary<string, string> ENTIRE_NAME_REPLACEMENTS { get; } = new()
        {
            {"live a live","Live A Live" },
            {"nights into dreams...","NiGHTS into Dreams..." },
            {"rise of the triad","Rise Of The Triad" },
        };

        public static string FormatSongString(string songString)
        {
            songString = songString.ToLower();
            if (ENTIRE_NAME_REPLACEMENTS.ContainsKey(songString))
            {
                return ENTIRE_NAME_REPLACEMENTS[songString];
            }
            var words = songString.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            var copy = words.ToList();
            for (int i = 0; i < copy.Count; ++i)
            {
                var word = copy[i];
                if (SINGLE_WORD_REPLACEMENTS.ContainsKey(word))
                {
                    words[i] = SINGLE_WORD_REPLACEMENTS[word];
                }
            }
            songString = string.Join(" ", words);
            return songString.Transform(To.TitleCase);
        }
    }
}
