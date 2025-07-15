using FunWithGBFS.Core.Models;
using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Questions.Interfaces
{
    public interface IQuestionGenerator
    {
        Question Generate(object input, int optionsCount);
    }
    public interface IQuestionGenerator<T>: IQuestionGenerator
    {
        Question Generate(List<T> input, int optionsCount);
    }
}
