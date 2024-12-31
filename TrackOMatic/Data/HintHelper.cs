using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace TrackOMatic
{
    public static class HintHelper
    {
        private static List<int> thresholds;
        private static int hintCap;
        private static readonly double EXPONENT = 1.7;
        private static readonly double OFFSET_DIVISOR = 15;

        private static int GetHintRequirement(int hintSlot)
        {
            //based on https://github.com/2dos/DK64-Randomizer/blob/dev/randomizer%2FPatching%2FLib.py#L1092-L1105
            var offset = hintCap / OFFSET_DIVISOR;
            var multiplier = hintCap - offset;
            var final_offset = (hintCap + offset) / 2;
            var exp_result = 1 + (Math.Pow(hintSlot, EXPONENT) / Math.Pow(34, EXPONENT));
            var z = Math.PI * exp_result;
            var required_item_count = (int)(multiplier * 0.5 * Math.Cos(z) + final_offset);
            return (required_item_count == 0) ? 1 : required_item_count;

        }
        public static void GenerateThresholds()
        {
            hintCap = Properties.Settings.Default.ProgressiveHintCap;
            thresholds = new();
            for(int i = 0; i < 33; i += 4)
            {
                thresholds.Add(GetHintRequirement(i));
            }
            thresholds.Add(hintCap);
        }

        public static int GetAmountToNextHint(int totalItems)
        {
            foreach(var threshold in thresholds)
            {
                if (totalItems < threshold) return (threshold - totalItems);
            }
            return 0;
        }

        static HintHelper() { }
    }
}
