﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackOMatic
{
    public class PointValues
    {
        public static Dictionary<ItemType, int> GroupedValues = new();
        public static Dictionary<ItemName, int> SpecificValues = new();
    }
}
