using FluentAssertions;
using FunWithGBFS.Domain.Models;
using FunWithGBFS.Infrastructure.Gbfs;

namespace FunWithGBFSUnitTests
{
    public class GbfsVehicleDataMapperTests
    {
        [Fact]
        public void MapVehicles_ValidJson_ReturnsVehicles()
        {
            // Arrange
            var json = """
        {
            "data": {
                "vehicles": [
                    {
                        "vehicle_id": "v1",
                        "vehicle_type_id": "bike",
                        "is_disabled": true,
                        "is_reserved": false
                    },
                    {
                        "vehicle_id": "v2",
                        "vehicle_type_id": "scooter",
                        "is_disabled": false,
                        "is_reserved": true
                    }
                ]
            }
        }
        """;

            var provider = new Provider { Name = "TestProvider", City = "TestCity" };
            var mapper = new GbfsVehicleDataMapper();

            // Act
            var result = mapper.MapVehicles(json, provider);

            // Assert
            result.Should().HaveCount(2);
            result[0].Id.Should().Be("v1");
            result[0].IsDisabled.Should().BeTrue();
            result[0].IsReserved.Should().BeFalse();
            result[0].ProviderName.Should().Be("TestProvider");
        }

        [Fact]
        public void MapVehicles_InvalidJson_ReturnsEmptyList()
        {
            // Arrange
            var json = "{}";
            var provider = new Provider { Name = "Test", City = "Nowhere" };
            var mapper = new GbfsVehicleDataMapper();

            // Act
            var result = mapper.MapVehicles(json, provider);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
