﻿using System.Collections.Generic;
using System.Windows.Documents;

namespace TrackOMatic
{
    public static class OffsetInfo
    {
        public static readonly List<OffsetInfoEntry> OFFSETS = new()
        {
            new OffsetInfoEntry(ItemName.PROGRESSIVE_SLAM_1, 0x7FC9AF,8,2),
            new OffsetInfoEntry(ItemName.PROGRESSIVE_SLAM_2, 0x7FC9AF,8,3),

            new OffsetInfoEntry(ItemName.DONKEY, 0x7ECED8,8,2),
            new OffsetInfoEntry(ItemName.GORILLA_GRAB, 0x7FC950,8,4),
            new OffsetInfoEntry(ItemName.BABOON_BLAST, 0x7FC950,8,1),
            new OffsetInfoEntry(ItemName.STRONG_KONG, 0x7FC950,8,2),
            new OffsetInfoEntry(ItemName.COCONUT_GUN, 0x7FC952,8,1),
            new OffsetInfoEntry(ItemName.BONGO_BLAST, 0x7FC954,8,1),

            new OffsetInfoEntry(ItemName.DIDDY, 0x7ECEA8,8,64),
            new OffsetInfoEntry(ItemName.CHIMPY_CHARGE, 0x7FC9AE,8,1),
            new OffsetInfoEntry(ItemName.SIMIAN_SPRING, 0x7FC9AE,8,4),
            new OffsetInfoEntry(ItemName.ROCKETBARREL_BOOST, 0x7FC9AE,8,2),
            new OffsetInfoEntry(ItemName.PEANUT_POPGUNS, 0x7FC9B0,8,1),
            new OffsetInfoEntry(ItemName.GUITAR_GAZUMP, 0x7FC9B2,8,1),

            new OffsetInfoEntry(ItemName.LANKY, 0x7ECEB0,8,64),
            new OffsetInfoEntry(ItemName.ORANGSTAND, 0x7FCA0C,8,1),
            new OffsetInfoEntry(ItemName.BABOON_BALLOON, 0x7FCA0C,8,2),
            new OffsetInfoEntry(ItemName.ORANGSTAND_SPRINT, 0x7FCA0C,8,4),
            new OffsetInfoEntry(ItemName.GRAPE_SHOOTER, 0x7FCA0E,8,1),
            new OffsetInfoEntry(ItemName.TROMBONE_TREMOR, 0x7FCA10,8,1),

            new OffsetInfoEntry(ItemName.TINY, 0x7ECEB0,8,4),
            new OffsetInfoEntry(ItemName.PONYTAIL_TWIRL, 0x7FCA6A,8,2),
            new OffsetInfoEntry(ItemName.MONKEYPORT, 0x7FCA6A,8,4),
            new OffsetInfoEntry(ItemName.MINI_MONKEY, 0x7FCA6A,8,1),
            new OffsetInfoEntry(ItemName.FEATHER_BOW, 0x7FCA6C,8,1),
            new OffsetInfoEntry(ItemName.SAXOPHONE_SLAM, 0x7FCA6E,8,1),

            new OffsetInfoEntry(ItemName.CHUNKY, 0x7ECEB6,8,32),
            new OffsetInfoEntry(ItemName.PRIMATE_PUNCH, 0x7FCAC8,8,2),
            new OffsetInfoEntry(ItemName.GORILLA_GONE, 0x7FCAC8,8,4),
            new OffsetInfoEntry(ItemName.HUNKY_CHUNKY, 0x7FCAC8,8,1),
            new OffsetInfoEntry(ItemName.PINEAPPLE_LAUNCHER, 0x7FCACA,8,1),
            new OffsetInfoEntry(ItemName.TRIANGLE_TRAMPLE, 0x7FCACC,8,1),

            new OffsetInfoEntry(ItemName.KEY_1, 0x7ECEAB,8,4),
            new OffsetInfoEntry(ItemName.KEY_2, 0x7ECEB1,8,4),
            new OffsetInfoEntry(ItemName.KEY_3, 0x7ECEB9,8,4),
            new OffsetInfoEntry(ItemName.KEY_4, 0x7ECEBD,8,1),
            new OffsetInfoEntry(ItemName.KEY_5, 0x7ECEC5,8,16),
            new OffsetInfoEntry(ItemName.KEY_6, 0x7ECECC,8,16),
            new OffsetInfoEntry(ItemName.KEY_7, 0x7ECECF,8,32),
            new OffsetInfoEntry(ItemName.KEY_8, 0x7ECED7,8,16),

            new OffsetInfoEntry(ItemName.BEAN, 0x7ECF08,8,1),

            new OffsetInfoEntry(ItemName.DIVING, 0x7ECED8,8,4),
            new OffsetInfoEntry(ItemName.ORANGE_THROWING, 0x7ECED8,8,16),
            new OffsetInfoEntry(ItemName.BARREL_THROWING, 0x7ECED8,8,32),
            new OffsetInfoEntry(ItemName.VINE_SWINGING, 0x7ECED8,8,8),

            new OffsetInfoEntry(ItemName.HOMING_AMMO, 0x7FC9B0,8,2),
            new OffsetInfoEntry(ItemName.SNIPER_SCOPE, 0x7FC9B0,8,4),

            new OffsetInfoEntry(ItemName.DONKEY_JAPES_GBS, 0x7FC992,16),
            new OffsetInfoEntry(ItemName.DONKEY_AZTEC_GBS, 0x7FC994,16),
            new OffsetInfoEntry(ItemName.DONKEY_FACTORY_GBS, 0x7FC996,16),
            new OffsetInfoEntry(ItemName.DONKEY_GALLEON_GBS, 0x7FC998,16),
            new OffsetInfoEntry(ItemName.DONKEY_FOREST_GBS, 0x7FC99A,16),
            new OffsetInfoEntry(ItemName.DONKEY_CAVES_GBS, 0x7FC99C,16),
            new OffsetInfoEntry(ItemName.DONKEY_CASTLE_GBS, 0x7FC99E,16),
            new OffsetInfoEntry(ItemName.DONKEY_ISLES_GBS, 0x7FC9A0,16),
            new OffsetInfoEntry(ItemName.DONKEY_HELM_GBS, 0x7FC9A2,16),

            new OffsetInfoEntry(ItemName.DIDDY_JAPES_GBS, 0x7FC9F0,16),
            new OffsetInfoEntry(ItemName.DIDDY_AZTEC_GBS, 0x7FC9F2,16),
            new OffsetInfoEntry(ItemName.DIDDY_FACTORY_GBS, 0x7FC9F4,16),
            new OffsetInfoEntry(ItemName.DIDDY_GALLEON_GBS, 0x7FC9F6,16),
            new OffsetInfoEntry(ItemName.DIDDY_FOREST_GBS, 0x7FC9F8,16),
            new OffsetInfoEntry(ItemName.DIDDY_CAVES_GBS, 0x7FC9FA,16),
            new OffsetInfoEntry(ItemName.DIDDY_CASTLE_GBS, 0x7FC9FC,16),
            new OffsetInfoEntry(ItemName.DIDDY_ISLES_GBS, 0x7FC9FE,16),
            new OffsetInfoEntry(ItemName.DIDDY_HELM_GBS, 0x7FCA00,16),

            new OffsetInfoEntry(ItemName.LANKY_JAPES_GBS, 0x7FCA4E,16),
            new OffsetInfoEntry(ItemName.LANKY_AZTEC_GBS, 0x7FCA50,16),
            new OffsetInfoEntry(ItemName.LANKY_FACTORY_GBS, 0x7FCA52,16),
            new OffsetInfoEntry(ItemName.LANKY_GALLEON_GBS, 0x7FCA54,16),
            new OffsetInfoEntry(ItemName.LANKY_FOREST_GBS, 0x7FCA56,16),
            new OffsetInfoEntry(ItemName.LANKY_CAVES_GBS, 0x7FCA58,16),
            new OffsetInfoEntry(ItemName.LANKY_CASTLE_GBS, 0x7FCA5A,16),
            new OffsetInfoEntry(ItemName.LANKY_ISLES_GBS, 0x7FCA5C,16),
            new OffsetInfoEntry(ItemName.LANKY_HELM_GBS, 0x7FCA5E,16),

            new OffsetInfoEntry(ItemName.TINY_JAPES_GBS, 0x7FCAAC,16),
            new OffsetInfoEntry(ItemName.TINY_AZTEC_GBS, 0x7FCAAE,16),
            new OffsetInfoEntry(ItemName.TINY_FACTORY_GBS, 0x7FCAB0,16),
            new OffsetInfoEntry(ItemName.TINY_GALLEON_GBS, 0x7FCAB2,16),
            new OffsetInfoEntry(ItemName.TINY_FOREST_GBS, 0x7FCAB4,16),
            new OffsetInfoEntry(ItemName.TINY_CAVES_GBS, 0x7FCAB6,16),
            new OffsetInfoEntry(ItemName.TINY_CASTLE_GBS, 0x7FCAB8,16),
            new OffsetInfoEntry(ItemName.TINY_ISLES_GBS, 0x7FCABA,16),
            new OffsetInfoEntry(ItemName.TINY_HELM_GBS, 0x7FCABC,16),

            new OffsetInfoEntry(ItemName.CHUNKY_JAPES_GBS, 0x7FCB0A,16),
            new OffsetInfoEntry(ItemName.CHUNKY_AZTEC_GBS, 0x7FCB0C,16),
            new OffsetInfoEntry(ItemName.CHUNKY_FACTORY_GBS, 0x7FCB0E,16),
            new OffsetInfoEntry(ItemName.CHUNKY_GALLEON_GBS, 0x7FCB10,16),
            new OffsetInfoEntry(ItemName.CHUNKY_FOREST_GBS, 0x7FCB12,16),
            new OffsetInfoEntry(ItemName.CHUNKY_CAVES_GBS, 0x7FCB14,16),
            new OffsetInfoEntry(ItemName.CHUNKY_CASTLE_GBS, 0x7FCB16,16),
            new OffsetInfoEntry(ItemName.CHUNKY_ISLES_GBS, 0x7FCB18,16),
            new OffsetInfoEntry(ItemName.CHUNKY_HELM_GBS, 0x7FCB1A,16),

            new OffsetInfoEntry(ItemName.DONKEY_JAPES_BP, 0x7ECEE2,8,32),
            new OffsetInfoEntry(ItemName.DONKEY_AZTEC_BP, 0x7ECEE3,8,4),
            new OffsetInfoEntry(ItemName.DONKEY_FACTORY_BP, 0x7ECEE3,8,128),
            new OffsetInfoEntry(ItemName.DONKEY_GALLEON_BP, 0x7ECEE4,8,16),
            new OffsetInfoEntry(ItemName.DONKEY_FOREST_BP, 0x7ECEE5,8,2),
            new OffsetInfoEntry(ItemName.DONKEY_CAVES_BP, 0x7ECEE5,8,64),
            new OffsetInfoEntry(ItemName.DONKEY_CASTLE_BP, 0x7ECEE6,8,8),
            new OffsetInfoEntry(ItemName.DONKEY_ISLES_BP, 0x7ECEE7,8,1),

            new OffsetInfoEntry(ItemName.DIDDY_JAPES_BP, 0x7ECEE2,8,64),
            new OffsetInfoEntry(ItemName.DIDDY_AZTEC_BP, 0x7ECEE3,8,8),
            new OffsetInfoEntry(ItemName.DIDDY_FACTORY_BP, 0x7ECEE4,8,1),
            new OffsetInfoEntry(ItemName.DIDDY_GALLEON_BP, 0x7ECEE4,8,32),
            new OffsetInfoEntry(ItemName.DIDDY_FOREST_BP, 0x7ECEE5,8,4),
            new OffsetInfoEntry(ItemName.DIDDY_CAVES_BP, 0x7ECEE5,8,128),
            new OffsetInfoEntry(ItemName.DIDDY_CASTLE_BP, 0x7ECEE6,8,16),
            new OffsetInfoEntry(ItemName.DIDDY_ISLES_BP, 0x7ECEE7,8,2),

            new OffsetInfoEntry(ItemName.LANKY_JAPES_BP, 0x7ECEE2,8,128),
            new OffsetInfoEntry(ItemName.LANKY_AZTEC_BP, 0x7ECEE3,8,16),
            new OffsetInfoEntry(ItemName.LANKY_FACTORY_BP, 0x7ECEE4,8,2),
            new OffsetInfoEntry(ItemName.LANKY_GALLEON_BP, 0x7ECEE4,8,64),
            new OffsetInfoEntry(ItemName.LANKY_FOREST_BP, 0x7ECEE5,8,8),
            new OffsetInfoEntry(ItemName.LANKY_CAVES_BP, 0x7ECEE6,8,1),
            new OffsetInfoEntry(ItemName.LANKY_CASTLE_BP, 0x7ECEE6,8,32),
            new OffsetInfoEntry(ItemName.LANKY_ISLES_BP, 0x7ECEE7,8,4),

            new OffsetInfoEntry(ItemName.TINY_JAPES_BP, 0x7ECEE3,8,1),
            new OffsetInfoEntry(ItemName.TINY_AZTEC_BP, 0x7ECEE3,8,32),
            new OffsetInfoEntry(ItemName.TINY_FACTORY_BP, 0x7ECEE4,8,4),
            new OffsetInfoEntry(ItemName.TINY_GALLEON_BP, 0x7ECEE4,8,128),
            new OffsetInfoEntry(ItemName.TINY_FOREST_BP, 0x7ECEE5,8,16),
            new OffsetInfoEntry(ItemName.TINY_CAVES_BP, 0x7ECEE6,8,2),
            new OffsetInfoEntry(ItemName.TINY_CASTLE_BP, 0x7ECEE6,8,64),
            new OffsetInfoEntry(ItemName.TINY_ISLES_BP, 0x7ECEE7,8,8),

            new OffsetInfoEntry(ItemName.CHUNKY_JAPES_BP, 0x7ECEE3,8,2),
            new OffsetInfoEntry(ItemName.CHUNKY_AZTEC_BP, 0x7ECEE3,8,64),
            new OffsetInfoEntry(ItemName.CHUNKY_FACTORY_BP, 0x7ECEE4,8,8),
            new OffsetInfoEntry(ItemName.CHUNKY_GALLEON_BP, 0x7ECEE5,8,1),
            new OffsetInfoEntry(ItemName.CHUNKY_FOREST_BP, 0x7ECEE5,8,32),
            new OffsetInfoEntry(ItemName.CHUNKY_CAVES_BP, 0x7ECEE6,8,4),
            new OffsetInfoEntry(ItemName.CHUNKY_CASTLE_BP, 0x7ECEE6,8,128),
            new OffsetInfoEntry(ItemName.CHUNKY_ISLES_BP, 0x7ECEE7,8,16),

            new OffsetInfoEntry(ItemName.DONKEY_JAPES_BP_TURNED, 0x7ECEE7, 8, 32),
            new OffsetInfoEntry(ItemName.DIDDY_JAPES_BP_TURNED, 0x7ECEE7, 8, 64),
            new OffsetInfoEntry(ItemName.LANKY_JAPES_BP_TURNED, 0x7ECEE7, 8, 128),
            new OffsetInfoEntry(ItemName.TINY_JAPES_BP_TURNED, 0x7ECEE8, 8, 1),
            new OffsetInfoEntry(ItemName.CHUNKY_JAPES_BP_TURNED, 0x7ECEE8, 8, 2),

            new OffsetInfoEntry(ItemName.DONKEY_AZTEC_BP_TURNED, 0x7ECEE8, 8, 4),
            new OffsetInfoEntry(ItemName.DIDDY_AZTEC_BP_TURNED, 0x7ECEE8, 8, 8),
            new OffsetInfoEntry(ItemName.LANKY_AZTEC_BP_TURNED, 0x7ECEE8, 8, 16),
            new OffsetInfoEntry(ItemName.TINY_AZTEC_BP_TURNED, 0x7ECEE8, 8, 32),
            new OffsetInfoEntry(ItemName.CHUNKY_AZTEC_BP_TURNED, 0x7ECEE8, 8, 64),

            new OffsetInfoEntry(ItemName.DONKEY_FACTORY_BP_TURNED, 0x7ECEE8, 8, 128),
            new OffsetInfoEntry(ItemName.DIDDY_FACTORY_BP_TURNED, 0x7ECEE9, 8, 1),
            new OffsetInfoEntry(ItemName.LANKY_FACTORY_BP_TURNED, 0x7ECEE9, 8, 2),
            new OffsetInfoEntry(ItemName.TINY_FACTORY_BP_TURNED, 0x7ECEE9, 8, 4),
            new OffsetInfoEntry(ItemName.CHUNKY_FACTORY_BP_TURNED, 0x7ECEE9, 8, 8),

            new OffsetInfoEntry(ItemName.DONKEY_GALLEON_BP_TURNED, 0x7ECEE9, 8, 16),
            new OffsetInfoEntry(ItemName.DIDDY_GALLEON_BP_TURNED, 0x7ECEE9, 8, 32),
            new OffsetInfoEntry(ItemName.LANKY_GALLEON_BP_TURNED, 0x7ECEE9, 8, 64),
            new OffsetInfoEntry(ItemName.TINY_GALLEON_BP_TURNED, 0x7ECEE9, 8, 128),
            new OffsetInfoEntry(ItemName.CHUNKY_GALLEON_BP_TURNED, 0x7ECEEA, 8, 1),

            new OffsetInfoEntry(ItemName.DONKEY_FOREST_BP_TURNED, 0x7ECEEA, 8, 2),
            new OffsetInfoEntry(ItemName.DIDDY_FOREST_BP_TURNED, 0x7ECEEA, 8, 4),
            new OffsetInfoEntry(ItemName.LANKY_FOREST_BP_TURNED, 0x7ECEEA, 8, 8),
            new OffsetInfoEntry(ItemName.TINY_FOREST_BP_TURNED, 0x7ECEEA, 8, 16),
            new OffsetInfoEntry(ItemName.CHUNKY_FOREST_BP_TURNED, 0x7ECEEA, 8, 32),

            new OffsetInfoEntry(ItemName.DONKEY_CAVES_BP_TURNED, 0x7ECEEA, 8, 64),
            new OffsetInfoEntry(ItemName.DIDDY_CAVES_BP_TURNED, 0x7ECEEA, 8, 128),
            new OffsetInfoEntry(ItemName.LANKY_CAVES_BP_TURNED, 0x7ECEEB, 8, 1),
            new OffsetInfoEntry(ItemName.TINY_CAVES_BP_TURNED, 0x7ECEEB, 8, 2),
            new OffsetInfoEntry(ItemName.CHUNKY_CAVES_BP_TURNED, 0x7ECEEB, 8, 4),

            new OffsetInfoEntry(ItemName.DONKEY_CASTLE_BP_TURNED, 0x7ECEEB, 8, 8),
            new OffsetInfoEntry(ItemName.DIDDY_CASTLE_BP_TURNED, 0x7ECEEB, 8, 16),
            new OffsetInfoEntry(ItemName.LANKY_CASTLE_BP_TURNED, 0x7ECEEB, 8, 32),
            new OffsetInfoEntry(ItemName.TINY_CASTLE_BP_TURNED, 0x7ECEEB, 8, 64),
            new OffsetInfoEntry(ItemName.CHUNKY_CASTLE_BP_TURNED, 0x7ECEEB, 8, 128),

            new OffsetInfoEntry(ItemName.DONKEY_ISLES_BP_TURNED, 0x7ECEEC, 8, 1),
            new OffsetInfoEntry(ItemName.DIDDY_ISLES_BP_TURNED, 0x7ECEEC, 8, 2),
            new OffsetInfoEntry(ItemName.LANKY_ISLES_BP_TURNED, 0x7ECEEC, 8, 4),
            new OffsetInfoEntry(ItemName.TINY_ISLES_BP_TURNED, 0x7ECEEC, 8, 8),
            new OffsetInfoEntry(ItemName.CHUNKY_ISLES_BP_TURNED, 0x7ECEEC, 8, 16),

            new OffsetInfoEntry(ItemName.DONKEY_JAPES_MEDAL, 0x7ECEEC,8,32),
            new OffsetInfoEntry(ItemName.DONKEY_AZTEC_MEDAL, 0x7ECEED,8,4),
            new OffsetInfoEntry(ItemName.DONKEY_FACTORY_MEDAL, 0x7ECEED,8,128),
            new OffsetInfoEntry(ItemName.DONKEY_GALLEON_MEDAL, 0x7ECEEE,8,16),
            new OffsetInfoEntry(ItemName.DONKEY_FOREST_MEDAL, 0x7ECEEF,8,2),
            new OffsetInfoEntry(ItemName.DONKEY_CAVES_MEDAL, 0x7ECEEF,8,64),
            new OffsetInfoEntry(ItemName.DONKEY_CASTLE_MEDAL, 0x7ECEF0,8,8),
            new OffsetInfoEntry(ItemName.DONKEY_HELM_MEDAL, 0x7ECEF1,8,1),

            new OffsetInfoEntry(ItemName.DIDDY_JAPES_MEDAL, 0x7ECEEC,8,64),
            new OffsetInfoEntry(ItemName.DIDDY_AZTEC_MEDAL, 0x7ECEED,8,8),
            new OffsetInfoEntry(ItemName.DIDDY_FACTORY_MEDAL, 0x7ECEEE,8,1),
            new OffsetInfoEntry(ItemName.DIDDY_GALLEON_MEDAL, 0x7ECEEE,8,32),
            new OffsetInfoEntry(ItemName.DIDDY_FOREST_MEDAL, 0x7ECEEF,8,4),
            new OffsetInfoEntry(ItemName.DIDDY_CAVES_MEDAL, 0x7ECEEF,8,128),
            new OffsetInfoEntry(ItemName.DIDDY_CASTLE_MEDAL, 0x7ECEF0,8,16),
            new OffsetInfoEntry(ItemName.DIDDY_HELM_MEDAL, 0x7ECEF1,8,16),

            new OffsetInfoEntry(ItemName.LANKY_JAPES_MEDAL, 0x7ECEEC,8,128),
            new OffsetInfoEntry(ItemName.LANKY_AZTEC_MEDAL, 0x7ECEED,8,16),
            new OffsetInfoEntry(ItemName.LANKY_FACTORY_MEDAL, 0x7ECEEE,8,2),
            new OffsetInfoEntry(ItemName.LANKY_GALLEON_MEDAL, 0x7ECEEE,8,64),
            new OffsetInfoEntry(ItemName.LANKY_FOREST_MEDAL, 0x7ECEEF,8,8),
            new OffsetInfoEntry(ItemName.LANKY_CAVES_MEDAL, 0x7ECEF0,8,1),
            new OffsetInfoEntry(ItemName.LANKY_CASTLE_MEDAL, 0x7ECEF0,8,32),
            new OffsetInfoEntry(ItemName.LANKY_HELM_MEDAL, 0x7ECEF1,8,4),

            new OffsetInfoEntry(ItemName.TINY_JAPES_MEDAL, 0x7ECEED,8,1),
            new OffsetInfoEntry(ItemName.TINY_AZTEC_MEDAL, 0x7ECEED,8,32),
            new OffsetInfoEntry(ItemName.TINY_FACTORY_MEDAL, 0x7ECEEE,8,4),
            new OffsetInfoEntry(ItemName.TINY_GALLEON_MEDAL, 0x7ECEEE,8,128),
            new OffsetInfoEntry(ItemName.TINY_FOREST_MEDAL, 0x7ECEEF,8,16),
            new OffsetInfoEntry(ItemName.TINY_CAVES_MEDAL, 0x7ECEF0,8,2),
            new OffsetInfoEntry(ItemName.TINY_CASTLE_MEDAL, 0x7ECEF0,8,64),
            new OffsetInfoEntry(ItemName.TINY_HELM_MEDAL, 0x7ECEF1,8,8),

            new OffsetInfoEntry(ItemName.CHUNKY_JAPES_MEDAL, 0x7ECEED,8,2),
            new OffsetInfoEntry(ItemName.CHUNKY_AZTEC_MEDAL, 0x7ECEED,8,64),
            new OffsetInfoEntry(ItemName.CHUNKY_FACTORY_MEDAL, 0x7ECEEE,8,8),
            new OffsetInfoEntry(ItemName.CHUNKY_GALLEON_MEDAL, 0x7ECEEF,8,1),
            new OffsetInfoEntry(ItemName.CHUNKY_FOREST_MEDAL, 0x7ECEEF,8,32),
            new OffsetInfoEntry(ItemName.CHUNKY_CAVES_MEDAL, 0x7ECEF0,8,4),
            new OffsetInfoEntry(ItemName.CHUNKY_CASTLE_MEDAL, 0x7ECEF0,8,128),
            new OffsetInfoEntry(ItemName.CHUNKY_HELM_MEDAL, 0x7ECEF1,8,2),

            new OffsetInfoEntry(ItemName.JAPES_BATTLE_CROWN, 0x7ECEF4,8,2),
            new OffsetInfoEntry(ItemName.AZTEC_BATTLE_CROWN, 0x7ECEF4,8,4),
            new OffsetInfoEntry(ItemName.FACTORY_BATTLE_CROWN, 0x7ECEF4,8,8),
            new OffsetInfoEntry(ItemName.GALLEON_BATTLE_CROWN, 0x7ECEF4,8,16),
            new OffsetInfoEntry(ItemName.FOREST_BATTLE_CROWN, 0x7ECEF4,8,32),
            new OffsetInfoEntry(ItemName.CAVES_BATTLE_CROWN, 0x7ECEF5,8,1),
            new OffsetInfoEntry(ItemName.CASTLE_BATTLE_CROWN, 0x7ECEF5,8,2),
            new OffsetInfoEntry(ItemName.ISLES_BATTLE_CROWN_1, 0x7ECEF4,8,128),
            new OffsetInfoEntry(ItemName.ISLES_BATTLE_CROWN_2, 0x7ECEF4,8,64),
            new OffsetInfoEntry(ItemName.HELM_BATTLE_CROWN, 0x7ECEF5,8,4),


            new OffsetInfoEntry(ItemName.JAPES_FAIRY_1, 0x7ECEF1,8,32),
            new OffsetInfoEntry(ItemName.JAPES_FAIRY_2, 0x7ECEF1,8,64),

            new OffsetInfoEntry(ItemName.AZTEC_FAIRY_1, 0x7ECEF3,8,1),
            new OffsetInfoEntry(ItemName.AZTEC_FAIRY_2, 0x7ECEF3,8,2),

            new OffsetInfoEntry(ItemName.FACTORY_FAIRY_1, 0x7ECEF1,8,128),
            new OffsetInfoEntry(ItemName.FACTORY_FAIRY_2, 0x7ECEF3,8,4),

            new OffsetInfoEntry(ItemName.GALLEON_FAIRY_1, 0x7ECEF2,8,1),
            new OffsetInfoEntry(ItemName.GALLEON_FAIRY_2, 0x7ECEF3,8,8),

            new OffsetInfoEntry(ItemName.FOREST_FAIRY_1, 0x7ECEF2,8,8),
            new OffsetInfoEntry(ItemName.FOREST_FAIRY_2, 0x7ECEF2,8,16),

            new OffsetInfoEntry(ItemName.CAVES_FAIRY_1, 0x7ECEF2,8,32),
            new OffsetInfoEntry(ItemName.CAVES_FAIRY_2, 0x7ECEF4,8,1),

            new OffsetInfoEntry(ItemName.CASTLE_FAIRY_1, 0x7ECEF3,8,16),
            new OffsetInfoEntry(ItemName.CASTLE_FAIRY_2, 0x7ECEF3,8,32),

            new OffsetInfoEntry(ItemName.HELM_FAIRY_1, 0x7ECEF2,8,64),
            new OffsetInfoEntry(ItemName.HELM_FAIRY_2, 0x7ECEF2,8,128),

            new OffsetInfoEntry(ItemName.ISLES_FAIRY_1, 0x7ECEF2,8,2),
            new OffsetInfoEntry(ItemName.ISLES_FAIRY_2, 0x7ECEF2,8,4),
            new OffsetInfoEntry(ItemName.ISLES_FAIRY_3, 0x7ECEF3,8,64),
            new OffsetInfoEntry(ItemName.ISLES_FAIRY_4, 0x7ECEF3,8,128),

            new OffsetInfoEntry(ItemName.PEARL_1, 0x7ECEBF,8,4),
            new OffsetInfoEntry(ItemName.PEARL_2, 0x7ECEBF,8,8),
            new OffsetInfoEntry(ItemName.PEARL_3, 0x7ECEBF,8,16),
            new OffsetInfoEntry(ItemName.PEARL_4, 0x7ECEBF,8,32),
            new OffsetInfoEntry(ItemName.PEARL_5, 0x7ECEBF,8,64),

            new OffsetInfoEntry(ItemName.RAINBOW_COIN_1, 0x7ECEFB,8,64),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_2, 0x7ECEFB,8,128),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_3, 0x7ECEFC,8,1),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_4, 0x7ECEFC,8,2),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_5, 0x7ECEFC,8,4),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_6, 0x7ECEFC,8,8),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_7, 0x7ECEFC,8,16),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_8, 0x7ECEFC,8,32),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_9, 0x7ECEFC,8,64),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_10, 0x7ECEFC,8,128),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_11, 0x7ECEFD,8,1),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_12, 0x7ECEFD,8,2),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_13, 0x7ECEFD,8,4),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_14, 0x7ECEFD,8,8),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_15, 0x7ECEFD,8,16),
            new OffsetInfoEntry(ItemName.RAINBOW_COIN_16, 0x7ECEFD,8,32),
        };
    }

}