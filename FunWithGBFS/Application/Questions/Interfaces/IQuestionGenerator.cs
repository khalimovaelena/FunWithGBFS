using FunWithGBFS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithGBFS.Application.Questions.Interfaces
{
    public interface IQuestionGenerator
    {
        //TODO: methods to generate questions based on bikes
        Question Generate(List<Station> stations);
    }
}
