using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace DK64PointsTracker
{
    public static class ImportantCheckList
    {
        public static readonly Dictionary<ItemName, ImportantCheck> ITEMS = new() {
        {ItemName.SNIPER_SCOPE, new ImportantCheck(ItemName.SNIPER_SCOPE, ItemType.SHARED_MOVE)},
        {ItemName.HOMING_AMMO, new ImportantCheck(ItemName.HOMING_AMMO,ItemType.SHARED_MOVE)},
        {ItemName.PROGRESSIVE_SLAM_1, new ImportantCheck(ItemName.PROGRESSIVE_SLAM_1,ItemType.SHARED_MOVE)},
        {ItemName.PROGRESSIVE_SLAM_2, new ImportantCheck(ItemName.PROGRESSIVE_SLAM_2,ItemType.SHARED_MOVE)},

        {ItemName.VINE_SWINGING, new ImportantCheck(ItemName.VINE_SWINGING,ItemType.TRAINING_MOVE)},
        {ItemName.BARREL_THROWING, new ImportantCheck(ItemName.BARREL_THROWING,ItemType.TRAINING_MOVE)},
        {ItemName.DIVING, new ImportantCheck(ItemName.DIVING,ItemType.TRAINING_MOVE)},
        {ItemName.ORANGE_THROWING, new ImportantCheck(ItemName.ORANGE_THROWING,ItemType.TRAINING_MOVE)},

        {ItemName.COCONUT_GUN, new ImportantCheck(ItemName.COCONUT_GUN,ItemType.GUN)},
        {ItemName.PEANUT_POPGUNS,new  ImportantCheck(ItemName.PEANUT_POPGUNS,ItemType.GUN)},
        {ItemName.GRAPE_SHOOTER,new  ImportantCheck(ItemName.GRAPE_SHOOTER,ItemType.GUN)},
        {ItemName.FEATHER_BOW, new ImportantCheck(ItemName.FEATHER_BOW,ItemType.GUN)},
         {ItemName.PINEAPPLE_LAUNCHER,new ImportantCheck(ItemName.PINEAPPLE_LAUNCHER,ItemType.GUN)},

        {ItemName.BONGO_BLAST, new ImportantCheck(ItemName.BONGO_BLAST,ItemType.INSTRUMENT)},
        {ItemName.GUITAR_GAZUMP, new ImportantCheck(ItemName.GUITAR_GAZUMP,ItemType.INSTRUMENT)},
        {ItemName.TROMBONE_TREMOR, new ImportantCheck(ItemName.TROMBONE_TREMOR,ItemType.INSTRUMENT)},
        {ItemName.SAXOPHONE_SLAM, new ImportantCheck(ItemName.SAXOPHONE_SLAM,ItemType.INSTRUMENT)},
        {ItemName.TRIANGLE_TRAMPLE, new ImportantCheck(ItemName.TRIANGLE_TRAMPLE,ItemType.INSTRUMENT)},

        {ItemName.GORILLA_GRAB, new ImportantCheck(ItemName.GORILLA_GRAB,ItemType.PHYSICAL_MOVE)},
        {ItemName.CHIMPY_CHARGE, new ImportantCheck(ItemName.CHIMPY_CHARGE,ItemType.PHYSICAL_MOVE)},
        {ItemName.ORANGSTAND, new ImportantCheck(ItemName.ORANGSTAND,ItemType.PHYSICAL_MOVE)  },
        {ItemName.PONYTAIL_TWIRL, new ImportantCheck(ItemName.PONYTAIL_TWIRL,ItemType.PHYSICAL_MOVE)},
        {ItemName.PRIMATE_PUNCH, new ImportantCheck(ItemName.PRIMATE_PUNCH,ItemType.PHYSICAL_MOVE) },


        {ItemName.STRONG_KONG, new ImportantCheck(ItemName.STRONG_KONG,ItemType.BARREL_MOVE)},
        {ItemName.ROCKETBARREL_BOOST, new ImportantCheck(ItemName.ROCKETBARREL_BOOST,ItemType.BARREL_MOVE)},
        {ItemName.ORANGSTAND_SPRINT, new ImportantCheck(ItemName.ORANGSTAND_SPRINT,ItemType.BARREL_MOVE)},
        {ItemName.MINI_MONKEY, new ImportantCheck(ItemName.MINI_MONKEY,ItemType.BARREL_MOVE)},
        {ItemName.HUNKY_CHUNKY, new ImportantCheck(ItemName.HUNKY_CHUNKY,ItemType.BARREL_MOVE)},

        {ItemName.BABOON_BLAST, new ImportantCheck(ItemName.BABOON_BLAST,ItemType.PAD_MOVE)},
        {ItemName.SIMIAN_SPRING, new ImportantCheck(ItemName.SIMIAN_SPRING,ItemType.PAD_MOVE)},
        {ItemName.BABOON_BALLOON, new ImportantCheck(ItemName.BABOON_BALLOON,ItemType.PAD_MOVE)},
        {ItemName.MONKEYPORT, new ImportantCheck(ItemName.MONKEYPORT,ItemType.PAD_MOVE)},
        {ItemName.GORILLA_GONE, new ImportantCheck(ItemName.GORILLA_GONE,ItemType.PAD_MOVE)},

        {ItemName.KEY_1, new ImportantCheck(ItemName.KEY_1,ItemType.KEY)},
        {ItemName.KEY_2,  new ImportantCheck(ItemName.KEY_2,ItemType.KEY)},
        {ItemName.KEY_3,  new ImportantCheck(ItemName.KEY_3,ItemType.KEY)},
        {ItemName.KEY_4,  new ImportantCheck(ItemName.KEY_4,ItemType.KEY)},
        {ItemName.KEY_5,  new ImportantCheck(ItemName.KEY_5,ItemType.KEY)},
        {ItemName.KEY_6,  new ImportantCheck(ItemName.KEY_6,ItemType.KEY)},
        {ItemName.KEY_7,  new ImportantCheck(ItemName.KEY_7,ItemType.KEY)},
        {ItemName.KEY_8,  new ImportantCheck(ItemName.KEY_8,ItemType.KEY)},

        {ItemName.DONKEY, new ImportantCheck(ItemName.DONKEY,ItemType.KONG)},
        {ItemName.DIDDY, new ImportantCheck(ItemName.DIDDY,ItemType.KONG)},
        {ItemName.LANKY, new ImportantCheck(ItemName.LANKY,ItemType.KONG)},
        {ItemName.TINY, new ImportantCheck(ItemName.TINY,ItemType.KONG)},
        {ItemName.CHUNKY, new ImportantCheck(ItemName.CHUNKY,ItemType.KONG)},
        {ItemName.KRUSHA, new ImportantCheck(ItemName.KRUSHA,ItemType.KONG)},

        {ItemName.BEAN, new ImportantCheck(ItemName.BEAN,ItemType.MISC)},

        {ItemName.DONKEY_JAPES_GBS, new ImportantCheck(ItemName.DONKEY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_AZTEC_GBS, new ImportantCheck(ItemName.DONKEY_AZTEC_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_FACTORY_GBS, new ImportantCheck(ItemName.DONKEY_FACTORY_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_GALLEON_GBS, new ImportantCheck(ItemName.DONKEY_GALLEON_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_FOREST_GBS, new ImportantCheck(ItemName.DONKEY_FOREST_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_CAVES_GBS, new ImportantCheck(ItemName.DONKEY_CAVES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_CASTLE_GBS, new ImportantCheck(ItemName.DONKEY_CASTLE_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_ISLES_GBS, new ImportantCheck(ItemName.DONKEY_ISLES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DONKEY_HELM_GBS, new ImportantCheck(ItemName.DONKEY_HELM_GBS, ItemType.GOLDEN_BANANA)},

        {ItemName.DIDDY_JAPES_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_AZTEC_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_FACTORY_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_GALLEON_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_FOREST_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_CAVES_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_CASTLE_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_ISLES_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.DIDDY_HELM_GBS, new ImportantCheck(ItemName.DIDDY_JAPES_GBS, ItemType.GOLDEN_BANANA)},

        {ItemName.LANKY_JAPES_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_AZTEC_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_FACTORY_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_GALLEON_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_FOREST_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_CAVES_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_CASTLE_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_ISLES_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.LANKY_HELM_GBS, new ImportantCheck(ItemName.LANKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},

        {ItemName.TINY_JAPES_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_AZTEC_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_FACTORY_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_GALLEON_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_FOREST_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_CAVES_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_CASTLE_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_ISLES_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.TINY_HELM_GBS, new ImportantCheck(ItemName.TINY_JAPES_GBS, ItemType.GOLDEN_BANANA)},

        {ItemName.CHUNKY_JAPES_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_AZTEC_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_FACTORY_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_GALLEON_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_FOREST_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_CAVES_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_CASTLE_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_ISLES_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},
        {ItemName.CHUNKY_HELM_GBS, new ImportantCheck(ItemName.CHUNKY_JAPES_GBS, ItemType.GOLDEN_BANANA)},

         {ItemName.DONKEY_JAPES_BP, new ImportantCheck(ItemName.DONKEY_JAPES_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_AZTEC_BP, new ImportantCheck(ItemName.DONKEY_AZTEC_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_FACTORY_BP, new ImportantCheck(ItemName.DONKEY_FACTORY_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_GALLEON_BP, new ImportantCheck(ItemName.DONKEY_GALLEON_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_FOREST_BP, new ImportantCheck(ItemName.DONKEY_FOREST_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_CAVES_BP, new ImportantCheck(ItemName.DONKEY_CAVES_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_CASTLE_BP, new ImportantCheck(ItemName.DONKEY_CASTLE_BP, ItemType.DONKEY_BLUEPRINT)},
        {ItemName.DONKEY_ISLES_BP, new ImportantCheck(ItemName.DONKEY_ISLES_BP, ItemType.DONKEY_BLUEPRINT)},
      /*
        {ItemName.PEARL_1, new ImportantCheck(ItemName.PEARL_1, ItemType.PEARL)},
        {ItemName.PEARL_2, new ImportantCheck(ItemName.PEARL_2, ItemType.PEARL)},
        {ItemName.PEARL_3, new ImportantCheck(ItemName.PEARL_3, ItemType.PEARL)},
        {ItemName.PEARL_4, new ImportantCheck(ItemName.PEARL_4, ItemType.PEARL)},
        {ItemName.PEARL_5, new ImportantCheck(ItemName.PEARL_5, ItemType.PEARL)},*/
       };
    }

}
