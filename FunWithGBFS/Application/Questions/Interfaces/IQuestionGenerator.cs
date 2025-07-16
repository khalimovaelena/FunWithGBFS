using FunWithGBFS.Domain.Models;

namespace FunWithGBFS.Application.Questions.Interfaces
{
    /// <summary>
    /// General interface for creating questions based on input data.
    /// </summary>
    public interface IQuestionGenerator
    {
        Question Generate(object input, int optionsCount);
    }

    /// <summary>
    /// Typed interface for creating questions based on specific input type.
    /// </summary>
    /// <typeparam name="T">Input type that contains data the will be used for generating questions</typeparam>
    public interface IQuestionGenerator<T>: IQuestionGenerator
    {
        Question Generate(List<T> input, int optionsCount);
    }
}
