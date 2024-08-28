using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Globalization;
using System.Text;

namespace TrackOMatic
{
    public delegate bool ProcessNewItem(ItemName itemName, RegionName regionName, bool hint = false);
    public delegate void UpdateCollectible(ItemType collectibleType, int newTotal);
    public delegate void SetRegionLighting(RegionName region, bool lightUp);
    public delegate void SetShopkeepers(bool on);
    public delegate void SetSong(string songGame, string songName);
    public class Autotracker
    {
        public ProcessNewItem ProcessNewItem { get; set; }
        public UpdateCollectible UpdateCollectible { get; set; }
        public SetRegionLighting SetRegionLighting { get; set; }
        public SetShopkeepers SetShopkeepers { get; set; }
        public SetSong SetSong{ get; set; }
        public Process EmulatorProcess { get; private set; }
        public List<AutotrackedCheck> Checks;
        public Dictionary<ItemName, bool> TrackedAlready;
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; }
        public GameVerificationInfo GameVerificationInfo { get; private set; }
        public RegionName CurrentRegion { get; private set; }
        public string currentSongGame { get; private set; }
        public string currentSongName { get; private set; }
        private SavedProgress savedProgress;
        public int RandomizerVersion { get; private set; }

        private Dictionary<ItemName, RegionName> trackedItemLocations;
        private System.Threading.Timer timer;
        private bool attached = false;
        private ulong startAddress;
        private int timeout;
        private bool is64Bit = false;
        private bool spoilerLoaded = false;
        public Autotracker(ProcessNewItem processItemCallback, UpdateCollectible updateCollectibleCallback, SetRegionLighting setRegionLightingCallback, SetShopkeepers setShopkeepersCallback, SetSong setSong)
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
            SetShopkeepers = setShopkeepersCallback;
            SetSong = setSong;
            currentSongName = "";
            currentSongGame = "";
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

        public void Reset()
        {
            spoilerLoaded = false;
            attached = false;
            InitializeChecks();
            Application.Current.Dispatcher.Invoke(() => {
                SetRegionLighting?.Invoke(CurrentRegion, false);
            });
            CurrentRegion = RegionName.UNKNOWN;
            currentSongName = "";
            RandomizerVersion = 0;
            currentSongGame = "";
        }

        public void ResetChecks()
        {
            InitializeChecks();
            ExcludeStartingItems();
            CurrentRegion = RegionName.UNKNOWN;
        }

        public void SetStartingItems(Dictionary<ItemName, RegionName> newItems)
        {
            StartingItems = newItems;
            ExcludeStartingItems();
        }

        public void SetSpoilerLoaded(string fileName)
        {
            spoilerLoaded = true;
        }

        private void AttachIfNecessary()
        {
            if (attached) return;
            var verificationInfo = new GameVerificationInfo(0x759290, 32, 0x52414D42);
            var attachedProcessInfo = AttachToEmulator.Attach(verificationInfo);
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

        private void CheckVersion()
        {
            uint versionOffset = 0x7FFFF4;
            int version = ReadMemory(versionOffset, 8);
            if (version != RandomizerVersion)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetShopkeepers?.Invoke(version >= 4);
                });
            }
            RandomizerVersion = version;
        }

        private string ReadAscii(ref uint startAddress)
        {
            List<byte> ascii = new();
            while(ascii.Count < 50)
            {
                byte next = (byte)ReadMemory(startAddress, 8);
                if (next > 127) return "";
                if (next == 0x00) break;
                ascii.Add(next);
                ++startAddress;
            }
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            string name = Encoding.ASCII.GetString(ascii.ToArray());
            return SongFormatting.FormatSongString(textInfo.ToLower(name));
        }

        private void WriteToSongFiles(string songGame, string songName)
        {
            var songDisplayFolder = "TrackOMatic_SongDisplayOutput";
            Dictionary<string, string> fileWrites = new()
                {
                    {"song_game_and_name.txt", songGame+"\n"+songName },
                    {"song_game.txt",songGame },
                    {"song_name.txt",songName }
                };
            Directory.CreateDirectory(songDisplayFolder);
            foreach (var entry in fileWrites)
            {
                try
                {
                    File.WriteAllText(songDisplayFolder + "/" + entry.Key, entry.Value);
                }
                catch (IOException) { }
                catch (Exception) { }
            }
        }

        private void UpdateCurrentSong()
        {
            var songGame = "";
            var songName = "";
            if (RandomizerVersion >= 4)
            {
                uint songPointer = 0x7FFFF0;
                int songAddr = ReadMemory(songPointer, 32);
                if (songAddr == 0x00000000) return;
                uint actualOffset = (uint)songAddr - 0x80000000;
                songGame = ReadAscii(ref actualOffset);
                actualOffset++;
                songName = ReadAscii(ref actualOffset);
            }
            if(songName == "")
            {
                songName = songGame;
                songGame = "Donkey Kong 64";
            }
            if (songGame != currentSongGame || songName != currentSongName)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SetSong?.Invoke(songGame, songName);
                });
                WriteToSongFiles(songGame, songName);
            }
            currentSongGame = songGame;
            currentSongName = songName;
        }

        private void Autotrack(object state)
        {
            if (!Properties.Settings.Default.Autotracking) return;
            //if (!spoilerLoaded) return;
            AttachIfNecessary();
            if (!attached) return;
            if (!ProcessConnected()) return;
            CheckVersion();
            UpdateCurrentRegion();
            UpdateCurrentSong();
            if (CurrentRegion == RegionName.UNKNOWN) return;
            ResetCollectibleAmounts();
            ReadMemoryForChecks();
            UpdateCollectibles();
        }

        private void ReadMemoryForChecks()
        {
            foreach (var check in Checks)
            {
                var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
                var bitMask = check.Bitmask;
                var isSlam = check.ItemName.ToString().Contains("PROGRESSIVE_SLAM");
                if (isSlam) bitMask = 0xF;
                //slams are weird, we instead will use the slam's bitmask as a direct value to check
                var output = ReadMemory(check.Offset, check.TotalBits, bitMask);
                var valid = (output == check.Bitmask) || (checkInfo.ItemType == ItemType.GOLDEN_BANANA);
                if (isSlam) valid = (output >= check.Bitmask);
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
            var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
            if (checkInfo.ItemType == ItemType.SHOPKEEPER && RandomizerVersion < 4)
            {
                TrackedAlready[check.ItemName] = true;
                return;
            }
            if (TrackedAlready[check.ItemName]) return;
            bool success = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                success = (bool)ProcessNewItem?.Invoke(check.ItemName, CurrentRegion);
            });
            TrackedAlready[check.ItemName] = success;
        }

        public void ProcessSavedItem(ItemName item)
        {
            TrackedAlready[item] = true;
        }

        public bool ItemWasTracked(ItemName item)
        {
            return TrackedAlready[item];
        }

        private void UpdateCollectibles()
        {
            if (Application.Current == null) return;
            foreach (var entry in CollectibleItemAmounts.ToList())
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
