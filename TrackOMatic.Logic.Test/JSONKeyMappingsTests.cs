using Xunit;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Helpers;

namespace TrackOMatic.Logic.Test
{
    public class JSONKeyMappingsTests
    {
        [Fact]
        public void ITEM_MAP_IsNotNull()
        {
            Assert.NotNull(JSONKeyMappings.ITEM_MAP);
        }

        [Fact]
        public void REGION_MAP_IsNotNull()
        {
            Assert.NotNull(JSONKeyMappings.REGION_MAP);
        }

        [Fact]
        public void VIAL_MAP_IsNotNull()
        {
            Assert.NotNull(JSONKeyMappings.VIAL_MAP);
        }

        [Fact]
        public void KONGS_ContainsAllFiveKongs()
        {
            Assert.NotNull(JSONKeyMappings.KONGS);
            Assert.Equal(5, JSONKeyMappings.KONGS.Count);
            Assert.Contains(ItemName.DONKEY, JSONKeyMappings.KONGS);
            Assert.Contains(ItemName.DIDDY, JSONKeyMappings.KONGS);
            Assert.Contains(ItemName.LANKY, JSONKeyMappings.KONGS);
            Assert.Contains(ItemName.TINY, JSONKeyMappings.KONGS);
            Assert.Contains(ItemName.CHUNKY, JSONKeyMappings.KONGS);
        }

        [Theory]
        [InlineData("Baboon Blast", ItemName.BABOON_BLAST)]
        [InlineData("Grape", ItemName.GRAPE_SHOOTER)]
        [InlineData("Coconut", ItemName.COCONUT_GUN)]
        [InlineData("Donkey", ItemName.DONKEY)]
        [InlineData("Bean", ItemName.BEAN)]
        public void ITEM_MAP_ContainsCorrectMappings(string key, ItemName expectedValue)
        {
            Assert.True(JSONKeyMappings.ITEM_MAP.ContainsKey(key));
            Assert.Equal(expectedValue, JSONKeyMappings.ITEM_MAP[key]);
        }

        [Theory]
        [InlineData("DK Isles", RegionName.DK_ISLES)]
        [InlineData("Jungle Japes", RegionName.JUNGLE_JAPES)]
        [InlineData("Angry Aztec", RegionName.ANGRY_AZTEC)]
        [InlineData("Frantic Factory", RegionName.FRANTIC_FACTORY)]
        [InlineData("Gloomy Galleon", RegionName.GLOOMY_GALLEON)]
        [InlineData("Fungi Forest", RegionName.FUNGI_FOREST)]
        [InlineData("Crystal Caves", RegionName.CRYSTAL_CAVES)]
        [InlineData("Creepy Castle", RegionName.CREEPY_CASTLE)]
        [InlineData("Hideout Helm", RegionName.HIDEOUT_HELM)]
        public void REGION_MAP_ContainsCorrectMappings(string key, RegionName expectedValue)
        {
            Assert.True(JSONKeyMappings.REGION_MAP.ContainsKey(key));
            Assert.Equal(expectedValue, JSONKeyMappings.REGION_MAP[key]);
        }

        [Theory]
        [InlineData("DK", RegionName.DK_ISLES)]
        [InlineData("Japes", RegionName.JUNGLE_JAPES)]
        [InlineData("Aztec", RegionName.ANGRY_AZTEC)]
        [InlineData("Factory", RegionName.FRANTIC_FACTORY)]
        [InlineData("Galleon", RegionName.GLOOMY_GALLEON)]
        [InlineData("Forest", RegionName.FUNGI_FOREST)]
        [InlineData("Caves", RegionName.CRYSTAL_CAVES)]
        [InlineData("Castle", RegionName.CREEPY_CASTLE)]
        public void SHORTENED_SHOP_TO_REGION_ContainsCorrectMappings(string key, RegionName expectedValue)
        {
            Assert.True(JSONKeyMappings.SHORTENED_SHOP_TO_REGION.ContainsKey(key));
            Assert.Equal(expectedValue, JSONKeyMappings.SHORTENED_SHOP_TO_REGION[key]);
        }

        [Theory]
        [InlineData(RegionName.DK_ISLES, "DK ISLES")]
        [InlineData(RegionName.JUNGLE_JAPES, "JAPES")]
        [InlineData(RegionName.ANGRY_AZTEC, "AZTEC")]
        [InlineData(RegionName.HIDEOUT_HELM, "HELM")]
        public void REGION_NAME_TO_SHORTENED_ContainsCorrectMappings(RegionName key, string expectedValue)
        {
            Assert.True(JSONKeyMappings.REGION_NAME_TO_SHORTENED.ContainsKey(key));
            Assert.Equal(expectedValue, JSONKeyMappings.REGION_NAME_TO_SHORTENED[key]);
        }

        [Fact]
        public void KROOL_MAP_TO_IMAGE_INDEX_IsNotEmpty()
        {
            Assert.NotNull(JSONKeyMappings.KROOL_MAP_TO_IMAGE_INDEX);
            Assert.True(JSONKeyMappings.KROOL_MAP_TO_IMAGE_INDEX.Count > 0);
        }

        [Fact]
        public void SPOILER_BARRIER_TO_BARRIER_ITEM_IsNotEmpty()
        {
            Assert.NotNull(JSONKeyMappings.SPOILER_BARRIER_TO_BARRIER_ITEM);
            Assert.True(JSONKeyMappings.SPOILER_BARRIER_TO_BARRIER_ITEM.Count > 0);
        }

        [Fact]
        public void POINT_NAME_TO_GROUP_ContainsExpectedKeys()
        {
            Assert.True(JSONKeyMappings.POINT_NAME_TO_GROUP.ContainsKey("kongs"));
            Assert.Equal(ItemType.KONG, JSONKeyMappings.POINT_NAME_TO_GROUP["kongs"]);
        }

        [Fact]
        public void RANDO_NAME_TO_ITEM_NAME_IsNotEmpty()
        {
            Assert.NotNull(JSONKeyMappings.RANDO_NAME_TO_ITEM_NAME);
            Assert.True(JSONKeyMappings.RANDO_NAME_TO_ITEM_NAME.Count > 0);
        }
    }
}
