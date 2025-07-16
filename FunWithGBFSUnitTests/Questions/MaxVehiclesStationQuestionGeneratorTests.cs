using FunWithGBFS.Application.Questions;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFSUnitTests.Questions
{
    public class MaxVehiclesStationQuestionGeneratorTests
    {
        [Fact]
        public void Generate_ReturnsNoData_WhenStationsIsNull()
        {
            var generator = new MaxVehiclesStationQuestionGenerator();
            var result = generator.Generate(null, 3);

            Assert.Equal("No stations available to determine max bike station.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsNoData_WhenStationsIsEmpty()
        {
            var generator = new MaxVehiclesStationQuestionGenerator();
            var result = generator.Generate(new List<Station>(), 3);

            Assert.Equal("No stations available to determine max bike station.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsQuestion_WithValidData()
        {
            var stations = new List<Station>
        {
            new Station { City = "City1", Name = "StationA", BikesAvailable = 5 },
            new Station { City = "City1", Name = "StationB", BikesAvailable = 10 },
            new Station { City = "City2", Name = "StationC", BikesAvailable = 7 }
        };
            var generator = new MaxVehiclesStationQuestionGenerator();
            var result = generator.Generate(stations, 3);

            Assert.StartsWith("Which station in", result.Text);
            Assert.Equal(3, result.Options.Count); // We want 3 options
            Assert.Contains(result.Options, option => option == "StationB"); // The station with the most bikes
        }
    }
}
