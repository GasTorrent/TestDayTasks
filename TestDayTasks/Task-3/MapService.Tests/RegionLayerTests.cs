using System.Linq;
using MapService.Domain.Entities;
using MapService.Domain.ValueObjects;
using Xunit;

namespace MapService.Tests
{
    public class RegionLayerTests
    {
        [Fact]
        public void GenerateRegions_CreatesCorrectNumberOfRegions()
        {
            // Arrange
            var mapWidth = 10;
            var mapHeight = 10;
            var regionWidth = 5;
            var regionHeight = 5;
            var layer = new RegionLayer((uint)mapWidth, (uint)mapHeight);

            // Act
            layer.GenerateRegions((uint)regionWidth, (uint)regionHeight);

            // Assert
            Assert.Equal(4, layer.Objects.Count);
        }

        [Fact]
        public void GetRegionId_ReturnsCorrectRegionId()
        {
            // Arrange
            var layer = new RegionLayer(10, 10);
            layer.GenerateRegions(5, 5);

            // Act
            var regionId1 = layer.GetRegionId(1, 1);
            var regionId2 = layer.GetRegionId(6, 6);

            // Assert
            Assert.Equal(1u, regionId1);
            Assert.Equal(4u, regionId2);
        }

        [Fact]
        public void GetRegion_ReturnsCorrectRegionObject()
        {
            // Arrange
            var layer = new RegionLayer(10, 10);
            layer.GenerateRegions(5, 5);

            // Act
            var region = layer.GetRegion(2);

            // Assert
            Assert.NotNull(region);
            Assert.Equal(2u, region.Id);
            Assert.Equal("Region 2", region.Name);
        }

        [Fact]
        public void ContainsTile_ReturnsTrueIfTileInRegion()
        {
            // Arrange
            var layer = new RegionLayer(10, 10);
            layer.GenerateRegions(5, 5);

            // Act
            var result1 = layer.ContainsTile(1, new Coordinate(1, 1));
            var result2 = layer.ContainsTile(4, new Coordinate(6, 6));
            var result3 = layer.ContainsTile(1, new Coordinate(0, 6));

            // Assert
            Assert.True(result1);
            Assert.True(result2);
            Assert.False(result3);
        }

        [Fact]
        public void GetRegionsInArea_ReturnsAllRegionsInArea()
        {
            // Arrange
            var layer = new RegionLayer(10, 10);
            layer.GenerateRegions(5, 5);
            var area = new Area(new Coordinate(0, 0), new Coordinate(6, 6));

            // Act
            var regions = layer.GetRegionsInArea(area).Select(r => r.Id).OrderBy(id => id).ToList();

            // Assert
            Assert.Equal(new[] { 1u, 2u, 3u, 4u }, regions);
        }
    }
    
    public class RegionLayerEdgeCaseTests
    {
        [Fact]
        public void GenerateRegions_HandlesNonDivisibleDimensions()
        {
            // Arrange
            var layer = new RegionLayer(11, 11);

            // Act
            layer.GenerateRegions(5, 5);

            // Assert
            // (11 / 5)^2 = (2.2)^2 -> 3*3 = 9 regions are expected to cover the map
            Assert.Equal(9, layer.Objects.Count);
        }

        [Fact]
        public void GetRegionId_ReturnsZeroForCoordinatesOutsideMap()
        {
            // Arrange
            var layer = new RegionLayer(10, 10);
            layer.GenerateRegions(5, 5);

            // Act
            var regionId = layer.GetRegionId(11, 5); // Coordinate (11, 5) is outside map boundaries

            // Assert
            Assert.Equal(0u, regionId);
        }

        [Fact]
        public void AllTiles_BelongToARegion()
        {
            // Arrange
            var mapWidth = 10;
            var mapHeight = 10;
            var layer = new RegionLayer((uint)mapWidth, (uint)mapHeight);
            layer.GenerateRegions(5, 5);

            // Act & Assert
            for (var x = 0; x < mapWidth; x++)
            {
                for (var y = 0; y < mapHeight; y++)
                {
                    var regionId = layer.GetRegionId(x, y);
                    Assert.True(regionId > 0, $"Tile at ({x}, {y}) does not belong to any region.");
                }
            }
        }
    }
}
