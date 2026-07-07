namespace TrackOMatic.Logic.Helpers
{
    public static class HintHelper
    {
        private static readonly double EXPONENT = 1.7;
        private static readonly double OFFSET_DIVISOR = 15;

        private static int GetHintRequirement(int hintSlot, int hintCap)
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

        /// <summary>
        /// Generates thresholds based on the provided hint cap value.
        /// </summary>
        /// <param name="hintCap">The maximum hint cap value used to generate thresholds.</param>
        /// <returns>A list of threshold values for progressive hints.</returns>
        public static List<int> GenerateThresholds(int hintCap)
        {
            var thresholds = new List<int>();
            for (int i = 0; i < 33; i += 4)
            {
                thresholds.Add(GetHintRequirement(i, hintCap));
            }
            thresholds.Add(hintCap);
            return thresholds;
        }

        /// <summary>
        /// Calculates the amount of items needed to reach the next hint threshold.
        /// </summary>
        /// <param name="totalItems">The current total item count.</param>
        /// <param name="thresholds">The list of hint thresholds.</param>
        /// <returns>The number of items needed to reach the next threshold, or 0 if at/past all thresholds.</returns>
        public static int GetAmountToNextHint(int totalItems, List<int> thresholds)
        {
            foreach (var threshold in thresholds)
            {
                if (totalItems < threshold)
                {
                    return (threshold - totalItems);
                }
            }
            return 0;
        }
    }
}

