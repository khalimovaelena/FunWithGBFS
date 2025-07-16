using FunWithGBFS.Application.Questions;
using FunWithGBFS.Domain.Models;
using Moq;

namespace FunWithGBFSUnitTests.Questions
{
    public class VehicleStatsQuestionGeneratorTests
    {
        [Fact]
        public void Generate_ReturnsNoData_WhenVehiclesIsNull()
        {
            var generator = new VehicleStatsQuestionGenerator();
            var result = generator.Generate(null, 3);

            Assert.Equal("No vehicle data available.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsNoData_WhenVehiclesIsEmpty()
        {
            var generator = new VehicleStatsQuestionGenerator();
            var result = generator.Generate(new List<Vehicle>(), 3);

            Assert.Equal("No vehicle data available.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsDisabledVehicleQuestion_WhenRandomIsMocked()
        {
            var vehicles = new List<Vehicle>
            {
                new Vehicle { ProviderName = "ProviderA", IsDisabled = true },
                new Vehicle { ProviderName = "ProviderA", IsDisabled = true },
                new Vehicle { ProviderName = "ProviderB", IsDisabled = false }
            };

            // Mock the Random class
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(2)).Returns(0);  // Force a "disabled" question (Next(2) will return 0)

            var generator = new VehicleStatsQuestionGenerator(randomMock.Object);

            var result = generator.Generate(vehicles, 3);

            Assert.Contains("disabled", result.Text);
            Assert.Contains(result.Options, option => option == "ProviderA");
        }

        [Fact]
        public void Generate_ReturnsReservedVehicleQuestion_WhenRandomIsMockedForReserved()
        {
            var vehicles = new List<Vehicle>
            {
                new Vehicle { ProviderName = "ProviderA", IsReserved = true },
                new Vehicle { ProviderName = "ProviderA", IsReserved = true },
                new Vehicle { ProviderName = "ProviderB", IsReserved = false }
            };

            // Mock the Random class
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(2)).Returns(1);  // Force a "reserved" question (Next(2) will return 1)

            var generator = new VehicleStatsQuestionGenerator(randomMock.Object);

            var result = generator.Generate(vehicles, 3);

            Assert.Contains("reserved", result.Text);
            Assert.Contains(result.Options, option => option == "ProviderA");
        }
    }
}
