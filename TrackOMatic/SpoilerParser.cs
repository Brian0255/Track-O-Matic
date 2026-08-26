using System.Windows;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;

namespace TrackOMatic
{
    public class SpoilerParser
    {
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; } = [];

        private IUserSettingsService UserSettings { get; init; }
        private ISpoilerService SpoilerService { get; set; }
        private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

        public SpoilerParser(IUserSettingsService userSettings, ISpoilerService spoilerService, IParsedSpoilerDataService parsedSpoilerDataService = null!)
        {
            UserSettings = userSettings;
            SpoilerService = spoilerService;
            ParsedSpoilerDataService = parsedSpoilerDataService ?? ServiceLocator.GetService<IParsedSpoilerDataService>();
        }

        public async Task<SpoilerSettings> ParseSpoilerAsync(string fileName)
        {
            var spoilerSettings = new SpoilerSettings();
            StartingItems = [];

            try
            {
                // Use the new service to deserialize and parse
                var result = await SpoilerService.DeserializeAndParseAsync(fileName);
                if (!result.IsSuccess || result.Data == null)
                {
                    return spoilerSettings;
                }

                var parsedData = result.Data;

                // Update the parsed spoiler data service so other components can access it
                ParsedSpoilerDataService.UpdateParsedData(parsedData);

                // Extract settings from parsed data
                spoilerSettings = parsedData.SpoilerSettings;

                // Populate starting items
                if (parsedData.StartingItems != null && parsedData.StartingItems.Count > 0)
                {
                    StartingItems = parsedData.StartingItems;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing spoiler file: {ex.Message}");
            }

            return spoilerSettings;
        }
    }
}
