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
using System.Timers;

namespace TrackOMatic
{
    public delegate bool ProcessNewItem(ItemName itemName, RegionName regionName, bool hint = false, bool newRegion = false);
    public delegate void UpdateCollectible(ItemType collectibleType, int newTotal);
    public delegate void SetRegionLighting(RegionName region, bool lightUp);
    public delegate void SetShopkeepers(bool on);
    public delegate void SetSong(string songGame, string songName);
    public delegate void UpdateUIAmountToNextHint(int amountToNextHint);
    public class Autotracker
    {
        public ProcessNewItem ProcessNewItem { get; set; }
        public UpdateCollectible UpdateCollectible { get; set; }
        public SetRegionLighting SetRegionLighting { get; set; }
        public SetShopkeepers SetShopkeepers { get; set; }
        public SetSong SetSong{ get; set; }
        public UpdateUIAmountToNextHint UpdateUIAmountToNextHint { get; set; }
        public Process EmulatorProcess { get; private set; }
        public List<AutotrackedCheck> Checks;
        public Dictionary<ItemName, bool> TrackedAlready;
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; }
        public GameVerificationInfo GameVerificationInfo { get; private set; }
        public RegionName CurrentRegion { get; private set; }
        private RegionName previousRegion;
        public string currentSongGame { get; private set; }
        public string currentSongName { get; private set; }
        private SavedProgress savedProgress;
        public int RandomizerVersion { get; private set; }

        private Dictionary<ItemName, RegionName> trackedItemLocations;
        private System.Timers.Timer timer;
        private bool attached = false;
        private ulong startAddress;
        private int timeout;
        private bool is64Bit = false;
        private bool spoilerLoaded = false;
        private bool autosave = false;
        private int previousMap;
        private static bool attaching = false;
        private uint addressBase;
        public Autotracker(ProcessNewItem processItemCallback, UpdateCollectible updateCollectibleCallback, SetRegionLighting setRegionLightingCallback, SetShopkeepers setShopkeepersCallback, SetSong setSong, UpdateUIAmountToNextHint updateUIAmountToNextHint)
        {
            CurrentRegion = RegionName.UNKNOWN;
            previousRegion = RegionName.UNKNOWN;
            Checks = new();
            StartingItems = new();
            TrackedAlready = new();
            InitializeChecks();
            timer = new System.Timers.Timer(1000);
            timer.AutoReset = false;
            timer.Elapsed += TimerHandler;
            trackedItemLocations = new();
            ProcessNewItem = processItemCallback;
            UpdateCollectible = updateCollectibleCallback;
            SetRegionLighting = setRegionLightingCallback;
            SetShopkeepers = setShopkeepersCallback;
            UpdateUIAmountToNextHint = updateUIAmountToNextHint;
            SetSong = setSong;
            currentSongName = "";
            currentSongGame = "";
            timer.Start();
            previousMap = -1;
            addressBase = 0x00000000;
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
            {ItemType.BATTLE_CROWN, 0 },
            {ItemType.COMPANY_COIN, 0 },
            { ItemType.TOTAL_BLUEPRINTS, 0 }
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
                Checks.Add(new AutotrackedCheck(offsetInfo.ItemName, offsetInfo.Offset, offsetInfo.TotalBits, offsetInfo.Bitmask, offsetInfo.UsesCountStruct));
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
            autosave = false;
            previousMap = -1;
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

        private int GetTotalCBs()
        {
            uint world_cb_offset_donkey = 0x7FC95A;
            int diff_between_kongs = 0x5E;
            int total = 0;
            for (int kong = 0; kong < 5; ++kong)
            {
                uint world_start = (uint)(world_cb_offset_donkey + (diff_between_kongs * kong));
                uint tns_start = world_start + 0x1C;
                for(int world = 0; world < 16; world += 2)
                {
                    total += ReadMemory((uint)(world_start + world), 16);
                }
                for (int tns_count = 0; tns_count < 16; tns_count +=2)
                {
                    total += ReadMemory((uint)(tns_start + tns_count), 16);
                }
            };
            return total;
        }

        public void UpdateAmountToNextHint()
        {
            var hintItem = (ItemType)Properties.Settings.Default.ProgressiveHintItem;
            int totalItems = 0;
            if (hintItem == ItemType.COLORED_BANANA)
            {
                totalItems = GetTotalCBs();
            }
            else if (CollectibleItemAmounts.ContainsKey(hintItem))
            {
                totalItems = CollectibleItemAmounts[hintItem];
            }
            var amount = HintHelper.GetAmountToNextHint(totalItems);
            Application.Current.Dispatcher.Invoke(() => {
                UpdateUIAmountToNextHint?.Invoke(amount);
            });
        }

        private void UpdateAddressBase()
        {
            if (RandomizerVersion < 5.0) return;
            uint countStructAddress = 0x7FFFB8;
            addressBase = ReadPointer(countStructAddress);
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
                }
                if(previousMap != -1 && area != previousMap)
                {
                    autosave = true;
                }
                previousRegion = CurrentRegion;
                CurrentRegion = newRegion;
                previousMap = area;
            }
        }

        private void ResetCollectibleAmounts()
        {
            foreach (var key in CollectibleItemAmounts.Keys.ToList()) CollectibleItemAmounts[key] = 0;
        }

        private void CheckVersion()
        {
            uint versionOffset = 0x7FFFF4;
            RandomizerVersion = ReadMemory(versionOffset, 8);
            var useNewOffsets = (RandomizerVersion >= 5);
            if (useNewOffsets != OffsetInfo.useNewOffsets)
            {
                OffsetInfo.useNewOffsets = useNewOffsets;
                InitializeChecks();
            }
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
                uint songAddr = ReadPointer(songPointer);
                if (songAddr == 0x00000000) return;
                songGame = ReadAscii(ref songAddr);
                songAddr++;
                songName = ReadAscii(ref songAddr);
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

        private void TimerHandler(object sender, ElapsedEventArgs e)
        {
            Autotrack();
            timer.Start();
        }

        private void Autotrack()
        {
            if (!Properties.Settings.Default.Autotracking) return;
            //if (!spoilerLoaded) return;
            AttachIfNecessary();
            if (!attached) return;
            if (!ProcessConnected()) return;
            CheckVersion();
            UpdateAddressBase();
            UpdateCurrentRegion();
            UpdateCurrentSong();
            if (CurrentRegion == RegionName.UNKNOWN) return;
            ResetCollectibleAmounts();
            ReadBlueprintsObtained();
            ReadMemoryForChecks();
            UpdateCollectibles();
            UpdateAmountToNextHint();
        }

        //this is some silly math magic but I trust that it works
        private int CountBits(int value)
        {
            int count = 0;
            while (value != 0)
            {
                value &= (value - 1);
                count++;
            }
            return count;
        }

        private void ReadBlueprintsObtained()
        {
            //To note, BP "turned in" flags still exist and subtract from these set totals after
            if (RandomizerVersion < 5.0) return;
            var blueprintKeys = new List<ItemType>() { 
                ItemType.DONKEY_BLUEPRINT, ItemType.DIDDY_BLUEPRINT, ItemType.LANKY_BLUEPRINT, ItemType.TINY_BLUEPRINT, ItemType.CHUNKY_BLUEPRINT 
            };
            for(int i = 0; i < blueprintKeys.Count; i++)
            {
                var itemType = blueprintKeys[i];
                var blueprintBitfield = ReadMemory((uint)(addressBase + i), 8);
                CollectibleItemAmounts[itemType] = CountBits(blueprintBitfield);
            }
        }
        private void ReadMemoryForChecks()
        {
            foreach (var check in Checks)
            {
                var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
                uint offset = 0x0000000;
                if (check.UsesCountStruct) offset = addressBase;
                var bitMask = check.Bitmask;
                var isFlag = (bitMask != 0);
                var isSlam = check.ItemName.ToString().Contains("PROGRESSIVE_SLAM");
                if (isSlam) bitMask = 0xF;
                //slams are weird, we instead will use the slam's bitmask as a direct value to check
                var output = ReadMemory(offset + check.Offset, check.TotalBits, bitMask);
                var valid = (output == check.Bitmask) || (bitMask == 0);
                if (isSlam) valid = (output >= check.Bitmask);
                if (!valid) continue;
                var collectible = CollectibleItemAmounts.ContainsKey(checkInfo.ItemType) ||
                                  TURNED_BLUEPRINT_TO_COLLECTIBLE.ContainsKey(checkInfo.ItemType);
                if(collectible) ProcessCollectible(output, checkInfo, isFlag);
                else ProcessRegularItem(check);
            }
        }

        private void ProcessCollectible(int output, ImportantCheck checkInfo, bool isFlag)
        {
            var toAdd = output;
            var itemTypeToUse = checkInfo.ItemType;
            if (checkInfo.ItemType.ToString().EndsWith("BLUEPRINT"))
            {
                CollectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] = CollectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] + 1;
            }
            //for flags, add 1 to their total instead of the bitmask
            if (isFlag)
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
            if (CurrentRegion == RegionName.START) return;
            var regionToUse = CurrentRegion;
            //when player initially loads in from menu, put the items in the star area
            if(CurrentRegion == RegionName.DK_ISLES && previousRegion == RegionName.START && !spoilerLoaded)
            {
                regionToUse = RegionName.START;
            }
            var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
            if (checkInfo.ItemType == ItemType.SHOPKEEPER && RandomizerVersion < 4)
            {
                TrackedAlready[check.ItemName] = true;
                return;
            }
            if (TrackedAlready[check.ItemName]) return;
            bool success = false;
            bool newRegion = (CurrentRegion != previousRegion && previousRegion != RegionName.UNKNOWN);
            Application.Current.Dispatcher.Invoke(() =>
            {
                success = (bool)ProcessNewItem?.Invoke(check.ItemName, regionToUse, false, autosave);
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
        private int ReadMemory(uint addr, int numOfBits, int bitmask = 0)
        {
            int toReturn;
            switch (numOfBits)
            {
                case 8:
                    toReturn = Memory.ReadInt8(EmulatorProcess, startAddress + Memory.Int8AddrFix(addr));
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
            if (bitmask != 0)
            {
                toReturn &= bitmask;
            }
            return toReturn;
        }
        private uint ReadPointer(uint pointerAddr)
        {
            uint addr = (uint)ReadMemory(pointerAddr, 32);
            var wrongFirstByte = ((uint)addr >> 24) != 0x80;
            var blankAddr = (addr == 0);
            return (blankAddr || wrongFirstByte) ? 0 : (addr & 0x00FFFFFF);
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
    }
}
