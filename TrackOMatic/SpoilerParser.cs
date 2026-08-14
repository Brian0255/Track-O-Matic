using System.Windows;

using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Models;
using TrackOMatic.Logic.Models.Spoilers;
using TrackOMatic.Services;

namespace TrackOMatic
{
    public class SpoilerParser
    {
        public MainWindow MainWindow { get; }
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; } = [];

        private IUserSettingsService UserSettings { get; init; }
        private ISpoilerService SpoilerService { get; set; }
        private IParsedSpoilerDataService ParsedSpoilerDataService { get; init; }

        public SpoilerParser(MainWindow mainWindow, IUserSettingsService userSettings, ISpoilerService spoilerService, IParsedSpoilerDataService parsedSpoilerDataService = null!)
        {
            MainWindow = mainWindow;
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
                    MainWindow.InitRegionsFromEmptySpoiler();
                    return spoilerSettings;
                }

                var parsedData = result.Data;

                // Update the parsed spoiler data service so other components can access it
                ParsedSpoilerDataService.UpdateParsedData(parsedData);

                // Extract settings from parsed data
                spoilerSettings = parsedData.SpoilerSettings;

                UpdateWindowsWithSpoilerData(parsedData);

                // Populate starting items
                if (parsedData.StartingItems != null && parsedData.StartingItems.Count > 0)
                {
                    StartingItems = parsedData.StartingItems;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing spoiler file: {ex.Message}");
                MainWindow.InitRegionsFromEmptySpoiler();
            }

            return spoilerSettings;
        }

        /// <summary>
        /// Updates the MainWindow and its regions with the parsed spoiler data.
        /// </summary>
        /// <remarks>
        /// The <see cref="ParsedSpoilerData"/> will eventually be made available in its own standalone service
        /// for XAML classes to consume more locally.
        /// </remarks>
        /// <param name="spoilerData">The parsed spoiler data.</param>
        private void UpdateWindowsWithSpoilerData(ParsedSpoilerData spoilerData)
        {
            // Extract settings from parsed data
            var spoilerSettings = spoilerData.SpoilerSettings;
            /*
            // Populate regions with parsed spoiler data
            if (spoilerData.RegionData != null && spoilerData.RegionData.Count > 0)
            {
                foreach (var regionEntry in spoilerData.RegionData)
                {
                    if (MainWindow.Regions.TryGetValue(regionEntry.Key, out var region))
                    {
                        region.AddPoints(regionEntry.Value.Points);
                        region.AddRequiredCheckTotal(regionEntry.Value.WothCount);
                        region.SpoilerSettings = spoilerSettings;

                        // Process vials for this region
                        if (regionEntry.Value.VialColors != null && regionEntry.Value.VialColors.Count > 0)
                        {
                            foreach (var vial in regionEntry.Value.VialColors)
                            {
                                region.RegionGrid.AddInitialVial(vial);
                            }
                        }
                    }
                }
            }

            // Populate Helm Kong order
            if (spoilerData.HelmOrder != null && spoilerData.HelmOrder.Count > 0)
            {
                if (UserSettings.ShowHelmOrder)
                {
                    for (int i = 0; i < MainWindow.HelmKongs.Count; i++)
                    {
                        if (i < spoilerData.HelmOrder.Count)
                        {
                            MainWindow.HelmKongs[i].SetIndex(spoilerData.HelmOrder[i]);
                            MainWindow.HelmKongs[i].Enabled = false;
                            MainWindow.HelmKongs[i].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MainWindow.HelmKongs[i].Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            */

            // Populate Final Boss Kong order (KRool order)
            if (spoilerData.FinalBossOrder != null && spoilerData.FinalBossOrder.Count > 0)
            {
                if (UserSettings.ShowKRoolOrder)
                {
                    for (int i = 0; i < MainWindow.BossKongs.Count; i++)
                    {
                        if (i < spoilerData.FinalBossOrder.Count)
                        {
                            MainWindow.BossKongs[i].SetIndex(spoilerData.FinalBossOrder[i]);
                            MainWindow.BossKongs[i].Enabled = false;
                            MainWindow.BossKongs[i].Visibility = Visibility.Visible;
                        }
                        else
                        {
                            MainWindow.BossKongs[i].Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
        }
    }
}
