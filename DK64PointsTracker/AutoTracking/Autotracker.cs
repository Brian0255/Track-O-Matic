using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace DK64PointsTracker
{
    public delegate void ProcessNewItem(ItemName itemName, RegionName regionName);
    public delegate void UpdateCollectible(ItemType collectibleType, int newTotal);
    public class Autotracker
    {
        public ProcessNewItem ProcessNewItem { get; set; }
        public UpdateCollectible UpdateCollectible { get; set; }
        public Process EmulatorProcess { get; private set; }
        public List<AutotrackedCheck> Checks;
        public Dictionary<ItemName, bool> TrackedAlready;
        public Dictionary<ItemName, RegionName> StartingItems { get; private set; }
        public GameVerificationInfo GameVerificationInfo { get; private set; }
        public RegionName CurrentRegion { get; private set; }

        private System.Threading.Timer timer;
        private bool attached = false;
        private uint startAddress;
        private int timeout;
        private bool is64Bit = false;
        private bool spoilerLoaded = false;
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

        public Autotracker(ProcessNewItem processItemCallback, UpdateCollectible updateCollectibleCallback)
        {
            CurrentRegion = RegionName.UNKNOWN;
            Checks = new();
            StartingItems = new();
            TrackedAlready = new();
            InitializeChecks();
            timer = new System.Threading.Timer(Autotrack, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            ProcessNewItem = processItemCallback;
            UpdateCollectible = updateCollectibleCallback;
        }

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

        public void SetSpoilerLoaded()
        {
            spoilerLoaded = true;
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
                CurrentRegion = MapToRegion.MAP[area];
            }
        }

        private void ResetCollectibleAmounts()
        {
            foreach (var key in CollectibleItemAmounts.Keys.ToList())
            {
                CollectibleItemAmounts[key] = 0;
            }
        }

        private void Autotrack(object state)
        {
            if (!Properties.Settings.Default.Autotracking) return;
            if (!spoilerLoaded) return;
            AttachIfNecessary();
            if (!attached) return;
            if (!ProcessConnected()) return;
            UpdateCurrentRegion();
            ResetCollectibleAmounts();
            foreach (var check in Checks)
            {
                var output = ReadMemory(check.Offset, check.TotalBits, check.Bitmask);
                var checkInfo = ImportantCheckList.ITEMS[check.ItemName];
                var itemType = checkInfo.ItemType;
                var valid = (output == check.Bitmask) || (checkInfo.ItemType == ItemType.GOLDEN_BANANA);
                if (!valid) continue;
                if (CollectibleItemAmounts.ContainsKey(checkInfo.ItemType))
                {
                    var toAdd = output;
                    //collectibles not gbs are flags, add 1 to their total instead of the bitmask
                    if (checkInfo.ItemType != ItemType.GOLDEN_BANANA) toAdd = 1;
                    CollectibleItemAmounts[checkInfo.ItemType] = CollectibleItemAmounts[checkInfo.ItemType] + 1;
                }
                else
                {
                    if (TrackedAlready[check.ItemName] == true) continue;
                    TrackedAlready[check.ItemName] = true;
                    Application.Current.Dispatcher.Invoke(() => {
                        ProcessNewItem?.Invoke(check.ItemName, CurrentRegion);
                    });
                }
            }
            foreach (var entry in CollectibleItemAmounts)
            {
                Application.Current.Dispatcher.Invoke(() => {
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

        private void StartTimer()
        {
            timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }
}
