using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DK64PointsTracker
{
    public class PointValues
    {
        public static readonly Dictionary<ItemType, int> GroupedValues = new()
        {
            {ItemType.SHARED_MOVE, 10},
            {ItemType.TRAINING_MOVE,8},

            {ItemType.GUN,3},
            {ItemType.INSTRUMENT,5},
            {ItemType.PHYSICAL_MOVE,2},
            {ItemType.BARREL_MOVE,7},
            {ItemType.PAD_MOVE,11},

            {ItemType.KONG, 13},
            {ItemType.KEY, 0 },
        };

        public static readonly Dictionary<ItemName, int> SpecificValues = new()
        {
            {ItemName.BEAN, 1 },
            {ItemName.KEY_1, 2 },
            {ItemName.KEY_2, 4 },
            {ItemName.KEY_3, 6},
            {ItemName.KEY_4, 8 },
            {ItemName.KEY_5, 10 },
            {ItemName.KEY_6, 12 },
            {ItemName.KEY_7, 14},
            {ItemName.KEY_8, 16 }
        };
    }
}
