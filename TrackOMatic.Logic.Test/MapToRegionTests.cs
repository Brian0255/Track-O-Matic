using Xunit;
using TrackOMatic.Logic.Enums;
using TrackOMatic.Logic.Helpers;

namespace TrackOMatic.Logic.Test
{
    public class MapToRegionTests
    {
        [Fact]
        public void MAP_IsNotNull()
        {
            Assert.NotNull(MapToRegion.MAP);
        }

        [Fact]
        public void MAP_ContainsExpectedEntries()
        {
            Assert.True(MapToRegion.MAP.Count > 0);
        }

        [Theory]
        [InlineData(0x002, RegionName.FRANTIC_FACTORY)]
        [InlineData(0x004, RegionName.JUNGLE_JAPES)]
        [InlineData(0x00E, RegionName.ANGRY_AZTEC)]
        [InlineData(0x01E, RegionName.GLOOMY_GALLEON)]
        [InlineData(0x030, RegionName.FUNGI_FOREST)]
        [InlineData(0x048, RegionName.CRYSTAL_CAVES)]
        [InlineData(0x050, RegionName.START)]
        [InlineData(0x057, RegionName.CREEPY_CASTLE)]
        [InlineData(0x0C8, RegionName.CRYSTAL_CAVES)]
        public void MAP_ContainsCorrectRegionForMapId(int mapId, RegionName expectedRegion)
        {
            Assert.True(MapToRegion.MAP.ContainsKey(mapId));
            Assert.Equal(expectedRegion, MapToRegion.MAP[mapId]);
        }

        [Fact]
        public void MAP_AllValuesAreValidRegions()
        {
            foreach (var kvp in MapToRegion.MAP)
            {
                Assert.True(Enum.IsDefined(typeof(RegionName), kvp.Value));
            }
        }
    }
}
