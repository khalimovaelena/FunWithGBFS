using FunWithGBFS.Application.Questions;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFSUnitTests.Questions
{
    public class AverageVehicleAvailabilityQuestionGeneratorTests
    {
        [Fact]
        public void Generate_ReturnsNoData_WhenStationsIsNull()
        {
            var generator = new AverageVehicleAvailabilityQuestionGenerator();
            var result = generator.Generate(null, 3);

            Assert.Equal("No station data available to calculate average availability.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsNoData_WhenStationsIsEmpty()
        {
            var generator = new AverageVehicleAvailabilityQuestionGenerator();
            var result = generator.Generate(new List<Station>(), 3);

            Assert.Equal("No station data available to calculate average availability.", result.Text);
            Assert.Single(result.Options);
            Assert.Equal("None", result.Options[0]);
        }

        [Fact]
        public void Generate_ReturnsQuestion_WithValidData()
        {
            var stations = new List<Station>
        {
            new Station { City = "City1", BikesAvailable = 5 },
            new Station { City = "City1", BikesAvailable = 10 },
            new Station { City = "City2", BikesAvailable = 7 }
        };
            var generator = new AverageVehicleAvailabilityQuestionGenerator();
            var result = generator.Generate(stations, 3);

            Assert.StartsWith("What is the average number of vehicles available per station in", result.Text);
            Assert.Equal(3, result.Options.Count); // We want 3 options
            Assert.Contains(result.Options, option => option == "8"); // Average is 7.5, rounded to 8
        }
    }
}
