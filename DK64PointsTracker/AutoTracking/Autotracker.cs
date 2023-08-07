using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace DK64PointsTracker
{
    public delegate bool ProcessNewItem(ItemName itemName, RegionName regionName);
    public delegate void UpdateCollectible(ItemType collectibleType, int newTotal);
    public delegate void SetRegionLighting(RegionName region, bool lightUp);
    public class Autotracker
    {
        public ProcessNewItem ProcessNewItem { get; set; }
        public UpdateCollectible UpdateCollectible { get; set; }
        public SetRegionLighting SetRegionLighting { get; set; }
        public Process EmulatorProcess { get; private set; }
        public List<AutotrackedCheck> Checks;
        public Dictionary<ItemName, bool> TrackedAlready;
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; }
        public GameVerificationInfo GameVerificationInfo { get; private set; }
        public RegionName CurrentRegion { get; private set; }
        private SavedProgress savedProgress;

        private Dictionary<ItemName, RegionName> trackedItemLocations;
        private System.Threading.Timer timer;
        private bool attached = false;
        private uint startAddress;
        private int timeout;
        private bool is64Bit = false;
        private bool spoilerLoaded = false;
        public Autotracker(ProcessNewItem processItemCallback, UpdateCollectible updateCollectibleCallback, SetRegionLighting setRegionLightingCallback)
        {
            CurrentRegion = RegionName.UNKNOWN;
            Checks = new();
            StartingItems = new();
            TrackedAlready = new();
            InitializeChecks();
            timer = new System.Threading.Timer(Autotrack, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            trackedItemLocations = new();
            ProcessNewItem = processItemCallback;
            UpdateCollectible = updateCollectibleCallback;
            SetRegionLighting = setRegionLightingCallback;
        }

        private class SavedProgress
        {
            public string SpoilerLogName { get; }
            public Dictionary<ItemName, RegionName> TrackedItemLocations { get; }

            public SavedProgress(string spoilerLogName)
            {
                SpoilerLogName = spoilerLogName;
                TrackedItemLocations = new();
            }
        }
        private Dictionary<ItemType, int> CollectibleItemAmounts { get; } = new()
        {
            {ItemType.GOLDEN_BANANA, 0 },
            {ItemType.DONKEY_BLUEPRINT, 0 },
            {ItemType.DIDDY_BLUEPRINT, 0 },
            {ItemType.LANKY_BLUEPRINT, 0 },
            {ItemType.TINY_BLUEPRINT, 0 },
            {ItemType.CHUNKY_BLUEPRINT, 0 },
            {ItemType.PEARL, 0 },
            {ItemType.BANANA_MEDAL, 0 },
            {ItemType.FAIRY, 0 },
            {ItemType.RAINBOW_COIN, 0 },
            {ItemType.BATTLE_CROWN, 0 }
        };
        private Dictionary<ItemType, ItemType> TURNED_BLUEPRINT_TO_COLLECTIBLE { get; } = new()
        {
            {ItemType.DONKEY_BLUEPRINT_TURNED, ItemType.DONKEY_BLUEPRINT },
            {ItemType.DIDDY_BLUEPRINT_TURNED, ItemType.DIDDY_BLUEPRINT },
            {ItemType.LANKY_BLUEPRINT_TURNED, ItemType.LANKY_BLUEPRINT },
            {ItemType.TINY_BLUEPRINT_TURNED, ItemType.TINY_BLUEPRINT },
            {ItemType.CHUNKY_BLUEPRINT_TURNED, ItemType.CHUNKY_BLUEPRINT }
        };
        private void InitializeChecks()
        {
            Checks = new();
            foreach (var offsetInfo in OffsetInfo.OFFSETS)
            {
                Checks.Add(new AutotrackedCheck(offsetInfo.ItemName, offsetInfo.Offset, offsetInfo.TotalBits, offsetInfo.TargetValue, offsetInfo.Bitmask));
                TrackedAlready[offsetInfo.ItemName] = false;
            }
        }

        public void SaveProgress()
        {
            if (savedProgress == null) return;
            var JSONString = JsonConvert.SerializeObject(savedProgress);
            var filePath = "autosave.json";
            File.WriteAllText(filePath, JSONString);
        }

        private void ReadSavedProgress()
        {
            if (savedProgress == null) return;
            foreach (var itemEntry in savedProgress.TrackedItemLocations)
            {
                var itemName = itemEntry.Key;
                var region = itemEntry.Value;
                TrackedAlready[itemName] = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    TrackedAlready[itemName] = (bool)(ProcessNewItem?.Invoke(itemName, region));
                });
            }
            SaveProgress();
        }

        private void CheckForAutosave()
        {
            var filePath = "autosave.json";
            if (!File.Exists(filePath)) return;
            try
            {
                var jsonString = File.ReadAllText(filePath);
                SavedProgress savedData = JsonConvert.DeserializeObject<SavedProgress>(jsonString);
                if(savedData.SpoilerLogName  == savedProgress.SpoilerLogName) savedProgress = savedData;
                ReadSavedProgress();
            }
            catch(Exception)
            {
                Console.WriteLine("Error reading autosave file");
            }

        }

        public void Reset()
        {
            spoilerLoaded = false;
            attached = false;
            InitializeChecks();
            Application.Current.Dispatcher.Invoke(() => {
                SetRegionLighting?.Invoke(CurrentRegion, false);
            });
        }

        public void ResetChecks()
        {
            InitializeChecks();
            ExcludeStartingItems();
        }

        public void SetStartingItems(Dictionary<ItemName, RegionName> newItems)
        {
            StartingItems = newItems;
            ExcludeStartingItems();
        }

        public void SetSpoilerLoaded(string fileName)
        {
            spoilerLoaded = true;
            savedProgress = new SavedProgress(fileName);
            CheckForAutosave();
        }

        private void AttachIfNecessary()
        {
            if (attached) return;
            var verificationInfo = new GameVerificationInfo(0x759260, 32, 0x444F4E4B);
            var attachedProcessInfo = AttachToEmulator.Attach(verificationInfo, EmulatorName.PROJECT_64);
            if (attachedProcessInfo == null) return;
            attached = true;
            startAddress = attachedProcessInfo.StartAddress;
            EmulatorProcess = attachedProcessInfo.Process;
            GameVerificationInfo = verificationInfo;
        }

        private void ExcludeStartingItems()
        {
            foreach (var entry in StartingItems)
            {
                TrackedAlready[entry.Key] = true;
            }
        }

        private void UpdateCurrentRegion()
        {
            uint offset = 0x76A0A8;
            int area = ReadMemory(offset, 32);
            if (MapToRegion.MAP.ContainsKey(area))
            {
                RegionName newRegion = MapToRegion.MAP[area];
                if(newRegion != CurrentRegion)
                {
                    Application.Current.Dispatcher.Invoke(() => {
                        SetRegionLighting?.Invoke(newRegion, true);
                    });
                    Application.Current.Dispatcher.Invoke(() => {
                        SetRegionLighting?.Invoke(CurrentRegion, false);
                    });
                    CurrentRegion = newRegion;
                }
            }
        }

        private void ResetCollectibleAmounts()
        {
            foreach (var key in CollectibleItemAmounts.Keys.ToList()) CollectibleItemAmounts[key] = 0;
        }

        private void Autotrack(object state)
        {
            if (!Properties.Settings.Default.Autotracking) return;
            if (!spoilerLoaded) return;
            AttachIfNecessary();
            if (!attached) return;
            if (!ProcessConnected()) return;
            UpdateCurrentRegion();
            if (CurrentRegion == RegionName.UNKNOWN) return;
            ResetCollectibleAmounts();
            ReadMemoryForChecks();
            UpdateCollectibles();
        }

        private void ReadMemoryForChecks()
        {
            foreach (var check in Checks)
            {
                var output = ReadMemory(check.Offset, check.TotalBits, check.Bitmask);
                var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
                var valid = (output == check.Bitmask) || (checkInfo.ItemType == ItemType.GOLDEN_BANANA);
                if (!valid) continue;
                var collectible = CollectibleItemAmounts.ContainsKey(checkInfo.ItemType) ||
                                  TURNED_BLUEPRINT_TO_COLLECTIBLE.ContainsKey(checkInfo.ItemType);
                if(collectible) ProcessCollectible(output, checkInfo);
                else ProcessRegularItem(check);
            }
        }

        private void ProcessCollectible(int output, ImportantCheck checkInfo)
        {
            var toAdd = output;
            var itemTypeToUse = checkInfo.ItemType;
            //collectibles not gbs are flags, add 1 to their total instead of the bitmask
            if (checkInfo.ItemType != ItemType.GOLDEN_BANANA)
            {
                toAdd = 1;
                if (TURNED_BLUEPRINT_TO_COLLECTIBLE.ContainsKey(checkInfo.ItemType))
                {
                    //adjust correctly if the user has turned in any blueprints
                    itemTypeToUse = TURNED_BLUEPRINT_TO_COLLECTIBLE[itemTypeToUse];
                    toAdd = -1;
                }
            }
            CollectibleItemAmounts[itemTypeToUse] = CollectibleItemAmounts[itemTypeToUse] + toAdd;
        }

        private void ProcessRegularItem(AutotrackedCheck check)
        {
            if (TrackedAlready[check.ItemName]) return;
            bool success = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                success = (bool)ProcessNewItem?.Invoke(check.ItemName, CurrentRegion);
            });
            TrackedAlready[check.ItemName] = success;
            if(success && !savedProgress.TrackedItemLocations.ContainsKey(check.ItemName))
            {
                savedProgress.TrackedItemLocations.Add(check.ItemName, CurrentRegion);
                SaveProgress();
            }
        }

        private void UpdateCollectibles()
        {
            foreach (var entry in CollectibleItemAmounts)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateCollectible?.Invoke(entry.Key, entry.Value);
                });
            }
        }

        private int ReadMemory(uint addr, int numOfBits, int bitmask = -1)
        {
            /* idk why this check even exists in the original code
            if (!is64Bit)
            {
                switch (numOfBits)
                {
                    case 8:
                        return Memory.ReadInt8(EmulatorProcess, startAddress + Memory.Int8AddrFix(addr)) & bitmask;
                    case 16:
                        return Memory.ReadInt16(EmulatorProcess, startAddress + Memory.Int16AddrFix(addr)) & bitmask;
                    case 32:
                        var stuff = Memory.ReadInt32(EmulatorProcess, startAddress + addr) & bitmask;
                        return Memory.ReadInt32(EmulatorProcess, startAddress + addr) & bitmask;
                    default:
                        return 0;
                }
            }
            */
            int toReturn;
            switch (numOfBits)
            {
                case 8:
                    toReturn = Memory.ReadInt8(EmulatorProcess, startAddress + Memory.Int8AddrFix(addr)) & bitmask;
                    break;
                case 16:
                    toReturn = Memory.ReadInt16(EmulatorProcess, startAddress + Memory.Int16AddrFix(addr));
                    break;
                case 32:
                    toReturn = Memory.ReadInt32(EmulatorProcess, startAddress + addr);
                    break;
                default:
                    return 0;
            }
            if (bitmask != -1)
            {
                toReturn &= bitmask;
            }
            return toReturn;
        }

        private bool ProcessConnected()
        {
            if (ReadMemory(GameVerificationInfo.TargetAddress, GameVerificationInfo.TotalBits) == GameVerificationInfo.TargetValue)
            {
                timeout = 0;
                return true;
            }
            timeout++;
            if (timeout > 10)
            {
                attached = false;
            }
            return false;
        }

        public void StopTimer()
        {
            timer.Change(-1, -1);
        }
    }
}
