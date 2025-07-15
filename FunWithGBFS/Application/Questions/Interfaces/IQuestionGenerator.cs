using FunWithGBFS.Core.Models;

namespace FunWithGBFS.Application.Questions.Interfaces
{
    public interface IQuestionGenerator
    {
        //TODO: methods to generate questions based on bikes
        Question Generate(List<Station> stations);
    }
}
