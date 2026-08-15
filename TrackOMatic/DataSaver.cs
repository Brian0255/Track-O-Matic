using System.IO;
using System.Text.Json;

using TrackOMatic.Logic.Models;
using TrackOMatic.Services;
using TrackOMatic.Services.TrackerState;

namespace TrackOMatic
{
    public class DataSaver
    {
        private SavedProgress CurrentSavedProgress { get; set; }
        public MainWindow MainWindow { get; }

        private readonly IDataPersistenceService _dataPersistenceService;
        private readonly IParsedSpoilerDataService _parsedSpoilerDataService;
        private readonly ISpoilerService _spoilerService;

        public bool WriteToFile { get; set; }

        public DataSaver(MainWindow mainWindow, IDataPersistenceService dataPersistenceService, IParsedSpoilerDataService parsedSpoilerDataService, ISpoilerService spoilerService)
        {
            MainWindow = mainWindow;
            _dataPersistenceService = dataPersistenceService;
            _parsedSpoilerDataService = parsedSpoilerDataService;
            _spoilerService = spoilerService;
            CurrentSavedProgress = new SavedProgress();
        }

        public void Reset()
        {
            CurrentSavedProgress = new SavedProgress();

            // Sync fresh SavedProgress with provider
            var provider = ServiceLocator.GetService<ISavedProgressProvider>();
            if (provider != null && CurrentSavedProgress != null)
            {
                provider.UpdateProgress(CurrentSavedProgress);
            }
        }

        private void FindSavedHints()
        {
            CurrentSavedProgress.SavedHints.Clear();
            foreach (var hintPanel in MainWindow.HintPanels)
            {
                foreach (var hint in hintPanel.GetSavedHints())
                {
                    CurrentSavedProgress.SavedHints.Add(hint);
                }
            }
        }

        public void Save(string filePath = "autosave.json", bool writeToFile = true)
        {
            if (CurrentSavedProgress == null)
            {
                return;
            }

            FindSavedHints();
            var JSONString = JsonSerializer.Serialize(CurrentSavedProgress);
            if (writeToFile)
            {
                File.WriteAllText(filePath, JSONString);
            }
        }

        private async void ReadSavedProgress()
        {
            if (CurrentSavedProgress == null)
            {
                return;
            }

            // Load spoiler log if it exists in the saved progress
            if (CurrentSavedProgress.spoilerPath != "" && File.Exists(CurrentSavedProgress.spoilerPath))
            {
                await MainWindow.ParseSpoiler(CurrentSavedProgress.spoilerPath);
                // Also parse into ParsedSpoilerDataService for access throughout the app
                try
                {
                    var result = await _spoilerService.DeserializeAndParseAsync(CurrentSavedProgress.spoilerPath);
                    if (result.IsSuccess && result.Data != null)
                    {
                        _parsedSpoilerDataService.UpdateParsedData(result.Data);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error parsing spoiler log for ParsedSpoilerDataService: {e}");
                }
            }

            foreach (var savedHint in CurrentSavedProgress.SavedHints.ToList())
            {
                var hintPanel = (HintPanel)MainWindow.FindName(savedHint.HintPanelKey);
                hintPanel.AddSavedHint(savedHint);
            }
        }

        public void AddSavedItem(SavedItem savedItem)
        {
            if (savedItem == null || CurrentSavedProgress == null)
            {
                return;
            }

            var itemName = savedItem.ItemName;
            if (CurrentSavedProgress.SavedItems.ContainsKey(itemName))
            {
                CurrentSavedProgress.SavedItems[itemName] = savedItem;
            }
            else
            {
                CurrentSavedProgress.SavedItems.Add(itemName, savedItem);
            }
        }

        public void ReadSavedDataFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            try
            {
                var jsonString = File.ReadAllText(filePath);
                SavedProgress? savedData = JsonSerializer.Deserialize<SavedProgress>(jsonString);
                if (savedData == null)
                {
                    return;
                }
                CurrentSavedProgress = savedData;
                ReadSavedProgress();

                // Sync SavedProgress with provider after loading from file
                var provider = ServiceLocator.GetService<ISavedProgressProvider>();
                if (provider != null && CurrentSavedProgress != null)
                {
                    provider.UpdateProgress(CurrentSavedProgress);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void setSpoilerPath(string newSpoilerPath)
        {
            if (CurrentSavedProgress == null)
            {
                return;
            }

            CurrentSavedProgress.spoilerPath = newSpoilerPath;
        }
    }
}
