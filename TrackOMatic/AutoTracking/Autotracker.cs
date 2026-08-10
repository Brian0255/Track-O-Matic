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
using System.Collections.Concurrent;

namespace TrackOMatic
{
    public delegate bool ProcessNewItem(ItemName itemName, RegionName regionName, bool hint = false, bool newRegion = false);
    public delegate void UpdateCollectible(ItemType collectibleType, int newTotal);
    public delegate void SetRegionLighting(RegionName region, bool lightUp);
    public delegate void SetShopkeepers(bool on);
    public delegate void SetSong(string songGame, string songName);
    public delegate void UpdateUIAmountToNextHint(int amountToNextHint);
    public delegate void UpdateProgHintImage(ItemType itemType);
    public class Autotracker
    {
        public ProcessNewItem ProcessNewItem { get; set; }
        public UpdateCollectible UpdateCollectible { get; set; }
        public SetRegionLighting SetRegionLighting { get; set; }
        public SetShopkeepers SetShopkeepers { get; set; }
        public SetSong SetSong{ get; set; }
        public UpdateUIAmountToNextHint UpdateUIAmountToNextHint { get; set; }
        public UpdateProgHintImage UpdateProgHintImage { get; set; }
        public Process EmulatorProcess { get; private set; }
        public List<AutotrackedCheck> Checks;
        private ConcurrentDictionary<ItemName, bool> TrackedAlready;
        private Dictionary<ItemName, RegionName> StartingItems { get; set; }
        public GameVerificationInfo GameVerificationInfo { get; private set; }
        public RegionName CurrentRegion { get; private set; }
        private RegionName previousRegion;
        private string currentSongGame { get; set; }
        private string currentSongName { get; set; }
        public int RandomizerVersion { get; private set; }
        public int RandomizerSubVersion { get; private set; }
        public Action ResetCompleted { get; set; }

        private readonly ConcurrentQueue<Action> _commands = new();
        private System.Timers.Timer timer;
        private bool attached = false;
        private ulong startAddress;
        private int timeout;
        private volatile bool spoilerLoaded = false;
        private bool autosave = false;
        private int previousMap;
        private uint addressBase;
        private ItemType progHintItem;
        private IntPtr processHandle;
        public Autotracker(ProcessNewItem processItemCallback, UpdateCollectible updateCollectibleCallback, SetRegionLighting setRegionLightingCallback, SetShopkeepers setShopkeepersCallback, SetSong setSong, UpdateUIAmountToNextHint updateUIAmountToNextHint, UpdateProgHintImage updateProgHintImage)
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
            ProcessNewItem = processItemCallback;
            UpdateCollectible = updateCollectibleCallback;
            SetRegionLighting = setRegionLightingCallback;
            SetShopkeepers = setShopkeepersCallback;
            UpdateUIAmountToNextHint = updateUIAmountToNextHint;
            UpdateProgHintImage = updateProgHintImage;
            SetSong = setSong;
            currentSongName = "";
            currentSongGame = "";
            timer.Start();
            previousMap = -1;
            addressBase = 0x00000000;
        }
        private ConcurrentDictionary<ItemType, int> CollectibleItemAmounts { get; } =
            new ConcurrentDictionary<ItemType, int>(
                new Dictionary<ItemType, int>
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
                }
            );
        private Dictionary<ItemType, ItemType> TURNED_BLUEPRINT_TO_COLLECTIBLE { get; } = new()
        {
            {ItemType.DONKEY_BLUEPRINT_TURNED, ItemType.DONKEY_BLUEPRINT },
            {ItemType.DIDDY_BLUEPRINT_TURNED, ItemType.DIDDY_BLUEPRINT },
            {ItemType.LANKY_BLUEPRINT_TURNED, ItemType.LANKY_BLUEPRINT },
            {ItemType.TINY_BLUEPRINT_TURNED, ItemType.TINY_BLUEPRINT },
            {ItemType.CHUNKY_BLUEPRINT_TURNED, ItemType.CHUNKY_BLUEPRINT }
        };
        private void InitializeChecks(bool resetTrackedItems = true)
        {
            //create new list and then assign it while locking to avoid threading issues
            var newChecks = new List<AutotrackedCheck>();
            foreach (var offsetInfo in OffsetInfo.OFFSETS)
            {
                newChecks.Add(new AutotrackedCheck(offsetInfo.ItemName, offsetInfo.Offset, offsetInfo.TotalBits, offsetInfo.Bitmask, offsetInfo.UsesCountStruct));
                if (resetTrackedItems)
                {
                    TrackedAlready[offsetInfo.ItemName] = false;
                }
            }
            Checks = newChecks;
        }
        public void Reset()
        {
            _commands.Enqueue(() =>
            {
                ResetInternal();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ResetCompleted?.Invoke();
                });
            });
        }

        public void ResetInternal()
        {
            Detach();
            InitializeChecks();
            spoilerLoaded = false;
            StartingItems = new();
            CurrentRegion = RegionName.UNKNOWN;
            currentSongName = "";
            RandomizerVersion = 0;
            RandomizerSubVersion = 0;
            currentSongGame = "";
            autosave = false;
            previousMap = -1;
            progHintItem = ItemType.GOLDEN_BANANA;
        }

        public void SetStartingItems(Dictionary<ItemName, RegionName> newItems)
        {
            var items = new Dictionary<ItemName, RegionName>(newItems);
            _commands.Enqueue(() =>
            {
                StartingItems = items;
                spoilerLoaded = true;
                ExcludeStartingItems();
            });
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
            processHandle = attachedProcessInfo.Handle;
        }

        private void ExcludeStartingItems()
        {
            //probably unnecessary, but let's just be extra safe here
            var snapshot = StartingItems;
            if (snapshot == null) return;
            foreach (var entry in snapshot.ToList())
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
        private int GetAmountToNextHintPack(int totalItems)
        {
            uint startAddress = 0x7FF898;
            for(int i = 0; i < 10; ++i)
            {
                var threshold = ReadMemory((uint)(startAddress + (i * 2)), 16);
                if (totalItems < threshold)
                {
                    return (threshold - totalItems);
                }
            }
            return 0;
        }

        private void UpdateProgHintItem()
        {
            var ToItemType = new Dictionary<int, ItemType>()
            {
                {3, ItemType.GOLDEN_BANANA },
                {4, ItemType.TOTAL_BLUEPRINTS },
                {5, ItemType.FAIRY },
                {6, ItemType.KEY },
                {7, ItemType.BATTLE_CROWN },
                {9, ItemType.BANANA_MEDAL },
                {11, ItemType.PEARL },
                {12, ItemType.RAINBOW_COIN },
                {15, ItemType.COLORED_BANANA }
            };
            var hintItem = ReadMemory(0x7FF8C3, 8);
            if (!ToItemType.ContainsKey(hintItem)){
                hintItem = 3; //default to GBs
            }
            var itemType = ToItemType[hintItem];
            if(progHintItem == itemType) { return; }
            progHintItem = itemType;
            Application.Current.Dispatcher.Invoke(() => UpdateProgHintImage(itemType));
        }

        public void UpdateAmountToNextHint()
        {
            int totalItems = 0;
            if (progHintItem == ItemType.COLORED_BANANA)
            {
                totalItems = GetTotalCBs();
            }
            else if (CollectibleItemAmounts.ContainsKey(progHintItem))
            {
                totalItems = CollectibleItemAmounts[progHintItem];
            }
            var amount = GetAmountToNextHintPack(totalItems);
            Application.Current.Dispatcher.Invoke(() => UpdateUIAmountToNextHint(amount));
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
                if (newRegion != CurrentRegion)
                {
                    Application.Current.Dispatcher.Invoke(() => SetRegionLighting(newRegion, true));
                    Application.Current.Dispatcher.Invoke(() => SetRegionLighting(CurrentRegion, false));
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

        private void UpdateVersion()
        {
            RandomizerVersion = ReadMemory(0x7FFFF4, 8);
            RandomizerSubVersion = ReadMemory(0x7FFFF5, 8);
        }

        private void CheckVersion()
        {
            var useNewOffsets = (RandomizerVersion >= 5);
            if (useNewOffsets != OffsetInfo.useNewOffsets)
            {
                OffsetInfo.useNewOffsets = useNewOffsets;
                InitializeChecks(false);
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
                Application.Current.Dispatcher.Invoke(() => SetSong(songGame, songName));
                WriteToSongFiles(songGame, songName);
            }
            currentSongGame = songGame;
            currentSongName = songName;
        }

        private void TimerHandler(object sender, ElapsedEventArgs e)
        {
            while (_commands.TryDequeue(out var command))
            {
                command();
            }
            try
            {
                Autotrack();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Autotracker exception: {ex}");
            }
            finally
            {
                try { timer.Start(); } catch { }
            }
        }

        private void Autotrack()
        {
            if (!Properties.Settings.Default.Autotracking) return;
            AttachIfNecessary();
            if (!attached) return;
            if (!ProcessConnected()) return;
            UpdateVersion();
            CheckVersion();
            UpdateAddressBase();
            UpdateCurrentRegion();
            UpdateCurrentSong();
            if (CurrentRegion == RegionName.UNKNOWN) return;
            ResetCollectibleAmounts();
            ReadBlueprintsObtained();
            ReadMemoryForChecks();
            UpdateCollectibles();
            UpdateProgHintItem();
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

        private bool IsOldBlueprintSystem()
        {
            return ((RandomizerVersion < 5.0) || (RandomizerVersion == 5.0 && RandomizerSubVersion == 0));
        }

        private void ReadBlueprintsObtained()
        {
            //To note, BP "turned in" flags still exist and subtract from these set totals after
            CollectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] = 0;
            if (RandomizerVersion < 5.0) return;
            var blueprintKeys = new List<ItemType>() { 
                ItemType.DONKEY_BLUEPRINT, ItemType.DIDDY_BLUEPRINT, ItemType.LANKY_BLUEPRINT, ItemType.TINY_BLUEPRINT, ItemType.CHUNKY_BLUEPRINT 
            };
            for(int i = 0; i < blueprintKeys.Count; i++)
            {
                var itemType = blueprintKeys[i];
                var kongBlueprints = ReadMemory((uint)(addressBase + i), 8);
                kongBlueprints = IsOldBlueprintSystem() ? CountBits(kongBlueprints) : kongBlueprints;
                CollectibleItemAmounts[itemType] = kongBlueprints;
                CollectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] = CollectibleItemAmounts[ItemType.TOTAL_BLUEPRINTS] + kongBlueprints;
                if (!IsOldBlueprintSystem())
                {
                    var turnedInBPs = ReadMemory((uint)(addressBase + 0x19 + i),8);
                    CollectibleItemAmounts[itemType] = CollectibleItemAmounts[itemType] - turnedInBPs;
                }
            }
        }
        private void ReadMemoryForChecks()
        {
            foreach (var check in Checks.ToList())
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
                if (TURNED_BLUEPRINT_TO_COLLECTIBLE.ContainsKey(checkInfo.ItemType) && IsOldBlueprintSystem())
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
            if (TrackedAlready.TryGetValue(check.ItemName, out var alreadyTracked) && alreadyTracked)
                return;
            bool newRegion = (CurrentRegion != previousRegion && previousRegion != RegionName.UNKNOWN);
            bool success = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                success = ProcessNewItem(check.ItemName, regionToUse, false, autosave);
            });

            TrackedAlready[check.ItemName] = success;
        }
        public void ProcessSavedItems(List<ItemName> items)
        {
            _commands.Enqueue(() =>
            {
                foreach(var item in items)
                {
                    TrackedAlready[item] = true;
                }
            });
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
                Application.Current.Dispatcher.Invoke(() => UpdateCollectible(entry.Key, entry.Value));
            }
        }
        private int ReadMemory(uint addr, int numOfBits, int bitmask = 0)
        {
            int toReturn;
            switch (numOfBits)
            {
                case 8:
                    toReturn = Memory.ReadInt8(processHandle, startAddress + Memory.Int8AddrFix(addr));
                    break;
                case 16:
                    toReturn = Memory.ReadInt16(processHandle, startAddress + Memory.Int16AddrFix(addr));
                    break;
                case 32:
                    toReturn = Memory.ReadInt32(processHandle, startAddress + addr);
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
            if (EmulatorProcess == null || EmulatorProcess.HasExited)
            {
                Detach();
                return false;
            }
            if (ReadMemory(GameVerificationInfo.TargetAddress, GameVerificationInfo.TotalBits) == GameVerificationInfo.TargetValue)
            {
                timeout = 0;
                return true;
            }
            timeout++;
            if (timeout > 10)
            {
                Detach();
            }
            return false;
        }

        private void Detach()
        {
            if (processHandle != IntPtr.Zero)
            {
                Memory.CloseHandleSafe(processHandle);
                processHandle = IntPtr.Zero;
            }
            attached = false;
        }

        public void Shutdown()
        {
            timer.Stop();
            timer.Elapsed -= TimerHandler;
            Detach();
        }
    }
}
