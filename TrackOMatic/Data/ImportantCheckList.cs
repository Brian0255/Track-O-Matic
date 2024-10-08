﻿using System.Collections.Generic;
using System.Windows.Controls.Primitives;

namespace TrackOMatic
{
    public static class ImportantCheckList
    {
        public static readonly Dictionary<ItemName, ImportantCheck> ITEMS = new() {
        {ItemName.SNIPER_SCOPE, new ImportantCheck(ItemName.SNIPER_SCOPE, ItemType.SHARED_MOVE, VialColor.CLEAR)},
        {ItemName.HOMING_AMMO, new ImportantCheck(ItemName.HOMING_AMMO,ItemType.SHARED_MOVE, VialColor.CLEAR)},

        {ItemName.PROGRESSIVE_SLAM_1, new ImportantCheck(ItemName.PROGRESSIVE_SLAM_1,ItemType.SHARED_MOVE, VialColor.CLEAR)},
        {ItemName.PROGRESSIVE_SLAM_2, new ImportantCheck(ItemName.PROGRESSIVE_SLAM_2,ItemType.SHARED_MOVE, VialColor.CLEAR)},
        {ItemName.PROGRESSIVE_SLAM_3, new ImportantCheck(ItemName.PROGRESSIVE_SLAM_3,ItemType.SHARED_MOVE, VialColor.CLEAR)},

        {ItemName.FAIRY_CAMERA, new ImportantCheck(ItemName.FAIRY_CAMERA, ItemType.FAIRY_MOVE, VialColor.CLEAR) },
        {ItemName.SHOCKWAVE, new ImportantCheck(ItemName.SHOCKWAVE, ItemType.FAIRY_MOVE, VialColor.CLEAR) },

        {ItemName.VINE_SWINGING, new ImportantCheck(ItemName.VINE_SWINGING,ItemType.TRAINING_MOVE, VialColor.CLEAR)},
        {ItemName.BARREL_THROWING, new ImportantCheck(ItemName.BARREL_THROWING,ItemType.TRAINING_MOVE, VialColor.CLEAR)},
        {ItemName.DIVING, new ImportantCheck(ItemName.DIVING,ItemType.TRAINING_MOVE, VialColor.CLEAR)},
        {ItemName.ORANGE_THROWING, new ImportantCheck(ItemName.ORANGE_THROWING,ItemType.TRAINING_MOVE, VialColor.CLEAR)},
        {ItemName.CLIMBING, new ImportantCheck(ItemName.CLIMBING,ItemType.TRAINING_MOVE, VialColor.CLEAR)},

        {ItemName.COCONUT_GUN, new ImportantCheck(ItemName.COCONUT_GUN,ItemType.GUN, VialColor.YELLOW)},
        {ItemName.PEANUT_POPGUNS,new  ImportantCheck(ItemName.PEANUT_POPGUNS,ItemType.GUN, VialColor.RED)},
        {ItemName.GRAPE_SHOOTER,new  ImportantCheck(ItemName.GRAPE_SHOOTER,ItemType.GUN, VialColor.BLUE)},
        {ItemName.FEATHER_BOW, new ImportantCheck(ItemName.FEATHER_BOW,ItemType.GUN, VialColor.PURPLE)},
         {ItemName.PINEAPPLE_LAUNCHER,new ImportantCheck(ItemName.PINEAPPLE_LAUNCHER,ItemType.GUN, VialColor.GREEN)},

        {ItemName.BONGO_BLAST, new ImportantCheck(ItemName.BONGO_BLAST,ItemType.INSTRUMENT, VialColor.YELLOW)},
        {ItemName.GUITAR_GAZUMP, new ImportantCheck(ItemName.GUITAR_GAZUMP,ItemType.INSTRUMENT, VialColor.RED)},
        {ItemName.TROMBONE_TREMOR, new ImportantCheck(ItemName.TROMBONE_TREMOR,ItemType.INSTRUMENT, VialColor.BLUE)},
        {ItemName.SAXOPHONE_SLAM, new ImportantCheck(ItemName.SAXOPHONE_SLAM,ItemType.INSTRUMENT, VialColor.PURPLE)},
        {ItemName.TRIANGLE_TRAMPLE, new ImportantCheck(ItemName.TRIANGLE_TRAMPLE,ItemType.INSTRUMENT, VialColor.GREEN)},

        {ItemName.GORILLA_GRAB, new ImportantCheck(ItemName.GORILLA_GRAB,ItemType.PHYSICAL_MOVE, VialColor.YELLOW)},
        {ItemName.CHIMPY_CHARGE, new ImportantCheck(ItemName.CHIMPY_CHARGE,ItemType.PHYSICAL_MOVE, VialColor.RED)},
        {ItemName.ORANGSTAND, new ImportantCheck(ItemName.ORANGSTAND,ItemType.PHYSICAL_MOVE, VialColor.BLUE)},
        {ItemName.PONYTAIL_TWIRL, new ImportantCheck(ItemName.PONYTAIL_TWIRL,ItemType.PHYSICAL_MOVE, VialColor.PURPLE)},
        {ItemName.PRIMATE_PUNCH, new ImportantCheck(ItemName.PRIMATE_PUNCH,ItemType.PHYSICAL_MOVE, VialColor.GREEN) },

        {ItemName.STRONG_KONG, new ImportantCheck(ItemName.STRONG_KONG,ItemType.BARREL_MOVE, VialColor.YELLOW)},
        {ItemName.ROCKETBARREL_BOOST, new ImportantCheck(ItemName.ROCKETBARREL_BOOST,ItemType.BARREL_MOVE, VialColor.RED)},
        {ItemName.ORANGSTAND_SPRINT, new ImportantCheck(ItemName.ORANGSTAND_SPRINT,ItemType.BARREL_MOVE, VialColor.BLUE)},
        {ItemName.MINI_MONKEY, new ImportantCheck(ItemName.MINI_MONKEY,ItemType.BARREL_MOVE, VialColor.PURPLE)},
        {ItemName.HUNKY_CHUNKY, new ImportantCheck(ItemName.HUNKY_CHUNKY,ItemType.BARREL_MOVE, VialColor.GREEN)},

        {ItemName.BABOON_BLAST, new ImportantCheck(ItemName.BABOON_BLAST,ItemType.PAD_MOVE, VialColor.YELLOW)},
        {ItemName.SIMIAN_SPRING, new ImportantCheck(ItemName.SIMIAN_SPRING,ItemType.PAD_MOVE, VialColor.RED)},
        {ItemName.BABOON_BALLOON, new ImportantCheck(ItemName.BABOON_BALLOON,ItemType.PAD_MOVE, VialColor.BLUE)},
        {ItemName.MONKEYPORT, new ImportantCheck(ItemName.MONKEYPORT,ItemType.PAD_MOVE, VialColor.PURPLE)},
        {ItemName.GORILLA_GONE, new ImportantCheck(ItemName.GORILLA_GONE,ItemType.PAD_MOVE, VialColor.GREEN)},

        {ItemName.KEY_1, new ImportantCheck(ItemName.KEY_1,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_2,  new ImportantCheck(ItemName.KEY_2,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_3,  new ImportantCheck(ItemName.KEY_3,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_4,  new ImportantCheck(ItemName.KEY_4,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_5,  new ImportantCheck(ItemName.KEY_5,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_6,  new ImportantCheck(ItemName.KEY_6,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_7,  new ImportantCheck(ItemName.KEY_7,ItemType.KEY, VialColor.KEY)},
        {ItemName.KEY_8,  new ImportantCheck(ItemName.KEY_8,ItemType.KEY, VialColor.KEY)},

        {ItemName.DONKEY, new ImportantCheck(ItemName.DONKEY,ItemType.KONG, VialColor.KONG)},
        {ItemName.DIDDY, new ImportantCheck(ItemName.DIDDY,ItemType.KONG, VialColor.KONG)},
        {ItemName.LANKY, new ImportantCheck(ItemName.LANKY,ItemType.KONG, VialColor.KONG)},
        {ItemName.TINY, new ImportantCheck(ItemName.TINY,ItemType.KONG, VialColor.KONG)},
        {ItemName.CHUNKY, new ImportantCheck(ItemName.CHUNKY,ItemType.KONG, VialColor.KONG)},
        {ItemName.KRUSHA, new ImportantCheck(ItemName.KRUSHA,ItemType.KONG, VialColor.KONG)},

        {ItemName.CRANKY, new ImportantCheck(ItemName.CRANKY,ItemType.SHOPKEEPER, VialColor.KONG)},
        {ItemName.CANDY, new ImportantCheck(ItemName.CANDY,ItemType.SHOPKEEPER, VialColor.KONG)},
        {ItemName.FUNKY, new ImportantCheck(ItemName.FUNKY,ItemType.SHOPKEEPER, VialColor.KONG)},
        {ItemName.SNIDE, new ImportantCheck(ItemName.SNIDE,ItemType.SHOPKEEPER, VialColor.KONG)},

        {ItemName.BEAN, new ImportantCheck(ItemName.BEAN,ItemType.MISC, VialColor.CLEAR)},

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

        {ItemName.DIDDY_JAPES_BP, new ImportantCheck(ItemName.DIDDY_JAPES_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_AZTEC_BP, new ImportantCheck(ItemName.DIDDY_AZTEC_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_FACTORY_BP, new ImportantCheck(ItemName.DIDDY_FACTORY_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_GALLEON_BP, new ImportantCheck(ItemName.DIDDY_GALLEON_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_FOREST_BP, new ImportantCheck(ItemName.DIDDY_FOREST_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_CAVES_BP, new ImportantCheck(ItemName.DIDDY_CAVES_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_CASTLE_BP, new ImportantCheck(ItemName.DIDDY_CASTLE_BP, ItemType.DIDDY_BLUEPRINT)},
        {ItemName.DIDDY_ISLES_BP, new ImportantCheck(ItemName.DIDDY_ISLES_BP, ItemType.DIDDY_BLUEPRINT)},

        {ItemName.LANKY_JAPES_BP, new ImportantCheck(ItemName.LANKY_JAPES_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_AZTEC_BP, new ImportantCheck(ItemName.LANKY_AZTEC_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_FACTORY_BP, new ImportantCheck(ItemName.LANKY_FACTORY_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_GALLEON_BP, new ImportantCheck(ItemName.LANKY_GALLEON_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_FOREST_BP, new ImportantCheck(ItemName.LANKY_FOREST_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_CAVES_BP, new ImportantCheck(ItemName.LANKY_CAVES_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_CASTLE_BP, new ImportantCheck(ItemName.LANKY_CASTLE_BP, ItemType.LANKY_BLUEPRINT)},
        {ItemName.LANKY_ISLES_BP, new ImportantCheck(ItemName.LANKY_ISLES_BP, ItemType.LANKY_BLUEPRINT)},

        {ItemName.TINY_JAPES_BP, new ImportantCheck(ItemName.TINY_JAPES_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_AZTEC_BP, new ImportantCheck(ItemName.TINY_AZTEC_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_FACTORY_BP, new ImportantCheck(ItemName.TINY_FACTORY_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_GALLEON_BP, new ImportantCheck(ItemName.TINY_GALLEON_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_FOREST_BP, new ImportantCheck(ItemName.TINY_FOREST_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_CAVES_BP, new ImportantCheck(ItemName.TINY_CAVES_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_CASTLE_BP, new ImportantCheck(ItemName.TINY_CASTLE_BP, ItemType.TINY_BLUEPRINT)},
        {ItemName.TINY_ISLES_BP, new ImportantCheck(ItemName.TINY_ISLES_BP, ItemType.TINY_BLUEPRINT)},

        {ItemName.CHUNKY_JAPES_BP, new ImportantCheck(ItemName.CHUNKY_JAPES_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_AZTEC_BP, new ImportantCheck(ItemName.CHUNKY_AZTEC_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_FACTORY_BP, new ImportantCheck(ItemName.CHUNKY_FACTORY_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_GALLEON_BP, new ImportantCheck(ItemName.CHUNKY_GALLEON_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_FOREST_BP, new ImportantCheck(ItemName.CHUNKY_FOREST_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_CAVES_BP, new ImportantCheck(ItemName.CHUNKY_CAVES_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_CASTLE_BP, new ImportantCheck(ItemName.CHUNKY_CASTLE_BP, ItemType.CHUNKY_BLUEPRINT)},
        {ItemName.CHUNKY_ISLES_BP, new ImportantCheck(ItemName.CHUNKY_ISLES_BP, ItemType.CHUNKY_BLUEPRINT)},

        {ItemName.DONKEY_JAPES_BP_TURNED, new ImportantCheck(ItemName.DONKEY_JAPES_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_AZTEC_BP_TURNED, new ImportantCheck(ItemName.DONKEY_AZTEC_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_FACTORY_BP_TURNED, new ImportantCheck(ItemName.DONKEY_FACTORY_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_GALLEON_BP_TURNED, new ImportantCheck(ItemName.DONKEY_GALLEON_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_FOREST_BP_TURNED, new ImportantCheck(ItemName.DONKEY_FOREST_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_CAVES_BP_TURNED, new ImportantCheck(ItemName.DONKEY_CAVES_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_CASTLE_BP_TURNED, new ImportantCheck(ItemName.DONKEY_CASTLE_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},
        {ItemName.DONKEY_ISLES_BP_TURNED, new ImportantCheck(ItemName.DONKEY_ISLES_BP_TURNED, ItemType.DONKEY_BLUEPRINT_TURNED)},

        {ItemName.DIDDY_JAPES_BP_TURNED, new ImportantCheck(ItemName.DIDDY_JAPES_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_AZTEC_BP_TURNED, new ImportantCheck(ItemName.DIDDY_AZTEC_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_FACTORY_BP_TURNED, new ImportantCheck(ItemName.DIDDY_FACTORY_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_GALLEON_BP_TURNED, new ImportantCheck(ItemName.DIDDY_GALLEON_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_FOREST_BP_TURNED, new ImportantCheck(ItemName.DIDDY_FOREST_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_CAVES_BP_TURNED, new ImportantCheck(ItemName.DIDDY_CAVES_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_CASTLE_BP_TURNED, new ImportantCheck(ItemName.DIDDY_CASTLE_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},
        {ItemName.DIDDY_ISLES_BP_TURNED, new ImportantCheck(ItemName.DIDDY_ISLES_BP_TURNED, ItemType.DIDDY_BLUEPRINT_TURNED)},

        {ItemName.LANKY_JAPES_BP_TURNED, new ImportantCheck(ItemName.LANKY_JAPES_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_AZTEC_BP_TURNED, new ImportantCheck(ItemName.LANKY_AZTEC_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_FACTORY_BP_TURNED, new ImportantCheck(ItemName.LANKY_FACTORY_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_GALLEON_BP_TURNED, new ImportantCheck(ItemName.LANKY_GALLEON_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_FOREST_BP_TURNED, new ImportantCheck(ItemName.LANKY_FOREST_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_CAVES_BP_TURNED, new ImportantCheck(ItemName.LANKY_CAVES_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_CASTLE_BP_TURNED, new ImportantCheck(ItemName.LANKY_CASTLE_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},
        {ItemName.LANKY_ISLES_BP_TURNED, new ImportantCheck(ItemName.LANKY_ISLES_BP_TURNED, ItemType.LANKY_BLUEPRINT_TURNED)},

        {ItemName.TINY_JAPES_BP_TURNED, new ImportantCheck(ItemName.TINY_JAPES_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_AZTEC_BP_TURNED, new ImportantCheck(ItemName.TINY_AZTEC_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_FACTORY_BP_TURNED, new ImportantCheck(ItemName.TINY_FACTORY_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_GALLEON_BP_TURNED, new ImportantCheck(ItemName.TINY_GALLEON_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_FOREST_BP_TURNED, new ImportantCheck(ItemName.TINY_FOREST_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_CAVES_BP_TURNED, new ImportantCheck(ItemName.TINY_CAVES_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_CASTLE_BP_TURNED, new ImportantCheck(ItemName.TINY_CASTLE_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},
        {ItemName.TINY_ISLES_BP_TURNED, new ImportantCheck(ItemName.TINY_ISLES_BP_TURNED, ItemType.TINY_BLUEPRINT_TURNED)},

        {ItemName.CHUNKY_JAPES_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_JAPES_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_AZTEC_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_AZTEC_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_FACTORY_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_FACTORY_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_GALLEON_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_GALLEON_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_FOREST_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_FOREST_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_CAVES_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_CAVES_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_CASTLE_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_CASTLE_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},
        {ItemName.CHUNKY_ISLES_BP_TURNED, new ImportantCheck(ItemName.CHUNKY_ISLES_BP_TURNED, ItemType.CHUNKY_BLUEPRINT_TURNED)},

        {ItemName.DONKEY_JAPES_MEDAL, new ImportantCheck(ItemName.DONKEY_JAPES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_AZTEC_MEDAL, new ImportantCheck(ItemName.DONKEY_AZTEC_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_FACTORY_MEDAL, new ImportantCheck(ItemName.DONKEY_FACTORY_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_GALLEON_MEDAL, new ImportantCheck(ItemName.DONKEY_GALLEON_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_FOREST_MEDAL, new ImportantCheck(ItemName.DONKEY_FOREST_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_CAVES_MEDAL, new ImportantCheck(ItemName.DONKEY_CAVES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_CASTLE_MEDAL, new ImportantCheck(ItemName.DONKEY_CASTLE_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DONKEY_HELM_MEDAL, new ImportantCheck(ItemName.DONKEY_HELM_MEDAL, ItemType.BANANA_MEDAL)},

        {ItemName.DIDDY_JAPES_MEDAL, new ImportantCheck(ItemName.DIDDY_JAPES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_AZTEC_MEDAL, new ImportantCheck(ItemName.DIDDY_AZTEC_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_FACTORY_MEDAL, new ImportantCheck(ItemName.DIDDY_FACTORY_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_GALLEON_MEDAL, new ImportantCheck(ItemName.DIDDY_GALLEON_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_FOREST_MEDAL, new ImportantCheck(ItemName.DIDDY_FOREST_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_CAVES_MEDAL, new ImportantCheck(ItemName.DIDDY_CAVES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_CASTLE_MEDAL, new ImportantCheck(ItemName.DIDDY_CASTLE_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_HELM_MEDAL, new ImportantCheck(ItemName.DIDDY_HELM_MEDAL, ItemType.BANANA_MEDAL)},

        {ItemName.LANKY_JAPES_MEDAL, new ImportantCheck(ItemName.LANKY_JAPES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_AZTEC_MEDAL, new ImportantCheck(ItemName.LANKY_AZTEC_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_FACTORY_MEDAL, new ImportantCheck(ItemName.LANKY_FACTORY_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_GALLEON_MEDAL, new ImportantCheck(ItemName.LANKY_GALLEON_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_FOREST_MEDAL, new ImportantCheck(ItemName.LANKY_FOREST_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_CAVES_MEDAL, new ImportantCheck(ItemName.LANKY_CAVES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_CASTLE_MEDAL, new ImportantCheck(ItemName.LANKY_CASTLE_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_HELM_MEDAL, new ImportantCheck(ItemName.LANKY_HELM_MEDAL, ItemType.BANANA_MEDAL)},

        {ItemName.TINY_JAPES_MEDAL, new ImportantCheck(ItemName.TINY_JAPES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_AZTEC_MEDAL, new ImportantCheck(ItemName.TINY_AZTEC_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_FACTORY_MEDAL, new ImportantCheck(ItemName.TINY_FACTORY_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_GALLEON_MEDAL, new ImportantCheck(ItemName.TINY_GALLEON_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_FOREST_MEDAL, new ImportantCheck(ItemName.TINY_FOREST_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_CAVES_MEDAL, new ImportantCheck(ItemName.TINY_CAVES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_CASTLE_MEDAL, new ImportantCheck(ItemName.TINY_CASTLE_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_HELM_MEDAL, new ImportantCheck(ItemName.TINY_HELM_MEDAL, ItemType.BANANA_MEDAL)},

        {ItemName.CHUNKY_JAPES_MEDAL, new ImportantCheck(ItemName.CHUNKY_JAPES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_AZTEC_MEDAL, new ImportantCheck(ItemName.CHUNKY_AZTEC_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_FACTORY_MEDAL, new ImportantCheck(ItemName.CHUNKY_FACTORY_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_GALLEON_MEDAL, new ImportantCheck(ItemName.CHUNKY_GALLEON_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_FOREST_MEDAL, new ImportantCheck(ItemName.CHUNKY_FOREST_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_CAVES_MEDAL, new ImportantCheck(ItemName.CHUNKY_CAVES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_CASTLE_MEDAL, new ImportantCheck(ItemName.CHUNKY_CASTLE_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_HELM_MEDAL, new ImportantCheck(ItemName.CHUNKY_HELM_MEDAL, ItemType.BANANA_MEDAL)},

        {ItemName.JAPES_BATTLE_CROWN, new ImportantCheck(ItemName.JAPES_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.AZTEC_BATTLE_CROWN, new ImportantCheck(ItemName.AZTEC_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.FACTORY_BATTLE_CROWN, new ImportantCheck(ItemName.FACTORY_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.GALLEON_BATTLE_CROWN, new ImportantCheck(ItemName.GALLEON_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.FOREST_BATTLE_CROWN, new ImportantCheck(ItemName.FOREST_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.CAVES_BATTLE_CROWN, new ImportantCheck(ItemName.CAVES_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.CASTLE_BATTLE_CROWN, new ImportantCheck(ItemName.CASTLE_BATTLE_CROWN, ItemType.BATTLE_CROWN) },
        {ItemName.ISLES_BATTLE_CROWN_1, new ImportantCheck(ItemName.ISLES_BATTLE_CROWN_1, ItemType.BATTLE_CROWN) },
        {ItemName.ISLES_BATTLE_CROWN_2, new ImportantCheck(ItemName.ISLES_BATTLE_CROWN_2, ItemType.BATTLE_CROWN) },
        {ItemName.HELM_BATTLE_CROWN, new ImportantCheck(ItemName.HELM_BATTLE_CROWN, ItemType.BATTLE_CROWN) },



        {ItemName.JAPES_FAIRY_1, new ImportantCheck(ItemName.JAPES_FAIRY_1, ItemType.FAIRY) },
        {ItemName.JAPES_FAIRY_2, new ImportantCheck(ItemName.JAPES_FAIRY_2, ItemType.FAIRY) },

        {ItemName.AZTEC_FAIRY_1, new ImportantCheck(ItemName.AZTEC_FAIRY_1, ItemType.FAIRY) },
        {ItemName.AZTEC_FAIRY_2, new ImportantCheck(ItemName.AZTEC_FAIRY_2, ItemType.FAIRY) },

        {ItemName.FACTORY_FAIRY_1, new ImportantCheck(ItemName.FACTORY_FAIRY_1, ItemType.FAIRY) },
        {ItemName.FACTORY_FAIRY_2, new ImportantCheck(ItemName.FACTORY_FAIRY_2, ItemType.FAIRY) },

        {ItemName.GALLEON_FAIRY_1, new ImportantCheck(ItemName.GALLEON_FAIRY_1, ItemType.FAIRY) },
        {ItemName.GALLEON_FAIRY_2, new ImportantCheck(ItemName.GALLEON_FAIRY_2, ItemType.FAIRY) },

        {ItemName.FOREST_FAIRY_1, new ImportantCheck(ItemName.FOREST_FAIRY_1, ItemType.FAIRY) },
        {ItemName.FOREST_FAIRY_2, new ImportantCheck(ItemName.FOREST_FAIRY_2, ItemType.FAIRY) },

        {ItemName.CAVES_FAIRY_1, new ImportantCheck(ItemName.CAVES_FAIRY_1, ItemType.FAIRY) },
        {ItemName.CAVES_FAIRY_2, new ImportantCheck(ItemName.CAVES_FAIRY_2, ItemType.FAIRY) },

        {ItemName.CASTLE_FAIRY_1, new ImportantCheck(ItemName.CASTLE_FAIRY_1, ItemType.FAIRY) },
        {ItemName.CASTLE_FAIRY_2, new ImportantCheck(ItemName.CASTLE_FAIRY_2, ItemType.FAIRY) },

        {ItemName.HELM_FAIRY_1, new ImportantCheck(ItemName.HELM_FAIRY_1, ItemType.FAIRY) },
        {ItemName.HELM_FAIRY_2, new ImportantCheck(ItemName.HELM_FAIRY_2, ItemType.FAIRY) },

        {ItemName.ISLES_FAIRY_1, new ImportantCheck(ItemName.ISLES_FAIRY_1, ItemType.FAIRY) },
        {ItemName.ISLES_FAIRY_2, new ImportantCheck(ItemName.ISLES_FAIRY_2, ItemType.FAIRY) },
        {ItemName.ISLES_FAIRY_3, new ImportantCheck(ItemName.ISLES_FAIRY_3, ItemType.FAIRY) },
        {ItemName.ISLES_FAIRY_4, new ImportantCheck(ItemName.ISLES_FAIRY_4, ItemType.FAIRY) },



        {ItemName.RAINBOW_COIN_1, new ImportantCheck(ItemName.RAINBOW_COIN_1, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_2, new ImportantCheck(ItemName.RAINBOW_COIN_2, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_3, new ImportantCheck(ItemName.RAINBOW_COIN_3, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_4, new ImportantCheck(ItemName.RAINBOW_COIN_4, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_5, new ImportantCheck(ItemName.RAINBOW_COIN_5, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_6, new ImportantCheck(ItemName.RAINBOW_COIN_6, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_7, new ImportantCheck(ItemName.RAINBOW_COIN_7, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_8, new ImportantCheck(ItemName.RAINBOW_COIN_8, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_9, new ImportantCheck(ItemName.RAINBOW_COIN_9, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_10, new ImportantCheck(ItemName.RAINBOW_COIN_10, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_11, new ImportantCheck(ItemName.RAINBOW_COIN_11, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_12, new ImportantCheck(ItemName.RAINBOW_COIN_12, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_13, new ImportantCheck(ItemName.RAINBOW_COIN_13, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_14, new ImportantCheck(ItemName.RAINBOW_COIN_14, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_15, new ImportantCheck(ItemName.RAINBOW_COIN_15, ItemType.RAINBOW_COIN) },
        {ItemName.RAINBOW_COIN_16, new ImportantCheck(ItemName.RAINBOW_COIN_16, ItemType.RAINBOW_COIN) },

        {ItemName.PEARL_1, new ImportantCheck(ItemName.PEARL_1, ItemType.PEARL)},
        {ItemName.PEARL_2, new ImportantCheck(ItemName.PEARL_2, ItemType.PEARL)},
        {ItemName.PEARL_3, new ImportantCheck(ItemName.PEARL_3, ItemType.PEARL)},
        {ItemName.PEARL_4, new ImportantCheck(ItemName.PEARL_4, ItemType.PEARL)},
        {ItemName.PEARL_5, new ImportantCheck(ItemName.PEARL_5, ItemType.PEARL)},

        {ItemName.NINTENDO_COIN, new ImportantCheck(ItemName.NINTENDO_COIN, ItemType.COMPANY_COIN)},
        {ItemName.RAREWARE_COIN, new ImportantCheck(ItemName.RAREWARE_COIN, ItemType.COMPANY_COIN)},

        {ItemName.DONKEY_ISLES_MEDAL, new ImportantCheck(ItemName.DONKEY_ISLES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.DIDDY_ISLES_MEDAL, new ImportantCheck(ItemName.DIDDY_ISLES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.LANKY_ISLES_MEDAL, new ImportantCheck(ItemName.LANKY_ISLES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.TINY_ISLES_MEDAL, new ImportantCheck(ItemName.TINY_ISLES_MEDAL, ItemType.BANANA_MEDAL)},
        {ItemName.CHUNKY_ISLES_MEDAL, new ImportantCheck(ItemName.CHUNKY_ISLES_MEDAL, ItemType.BANANA_MEDAL)},
       };
    }

}
