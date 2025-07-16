using FunWithGBFS.Domain.Models;
using FunWithGBFS.Infrastructure.Gbfs;

namespace FunWithGBFSUnitTests
{
    public class GbfsStationDataMapperTests
    {
        private readonly GbfsStationDataMapper _mapper = new();
        private readonly Provider _provider = new Provider { Name = "TestProvider", City = "TestCity" };

        [Fact]
        public void MapStations_ValidJson_ReturnsStations()
        {
            // Arrange
            var infoJson = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "s1",
                        "name": "Central Station",
                        "lat": 52.1,
                        "lon": 4.3
                    }
                ]
            }
        }
        """;

            var statusJson = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "s1",
                        "num_bikes_available": 7
                    }
                ]
            }
        }
        """;

            // Act
            var result = _mapper.MapStations(infoJson, statusJson, _provider);

            // Assert
            Assert.Single(result);
            var station = result[0];
            Assert.Equal("s1", station.Id);
            Assert.Equal("Central Station", station.Name);
            Assert.Equal(52.1, station.Lat);
            Assert.Equal(4.3, station.Lon);
            Assert.Equal(7, station.BikesAvailable);
            Assert.Equal("TestProvider", station.ProviderName);
            Assert.Equal("TestCity", station.City);
        }

        [Fact]
        public void MapStations_ValidJson_MissingStatus_SetsBikesAvailableToZero()
        {
            // Arrange
            var infoJson = """
        {
            "data": {
                "stations": [
                    {
                        "station_id": "s2",
                        "name": "Backup Station",
                        "lat": 52.2,
                        "lon": 4.4
                    }
                ]
            }
        }
        """;

            var statusJson = """
        {
            "data": {
                "stations": []
            }
        }
        """;

            // Act
            var result = _mapper.MapStations(infoJson, statusJson, _provider);

            // Assert
            Assert.Single(result);
            Assert.Equal(0, result[0].BikesAvailable);
        }

        [Fact]
        public void MapStations_InvalidJson_ReturnsEmptyList()
        {
            // Arrange
            var invalidJson = "{ this is not valid JSON";

            // Act
            var result = _mapper.MapStations(invalidJson, invalidJson, _provider);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void MapStations_ValidJson_InvalidStructure_ReturnsEmptyList()
        {
            // Arrange
            var infoJson = """
        {
            "something_else": {}
        }
        """;

            var statusJson = """
        {
            "something_else": {}
        }
        """;

            // Act
            var result = _mapper.MapStations(infoJson, statusJson, _provider);

            // Assert
            Assert.Empty(result);
        }
    }
}
