using System.Collections.Generic;
using System.ComponentModel;

namespace Hookah_Advisor
{
    public class Option
    {
        public int question_number { get; }

        public string question { get; }

        public List<string> answersTastes { get; }

        public List<string> firstAnswers { get; }


        public Option(int question_number, string question, List<string> answersTastes, List<string> firstAnswers)
        {
            this.question_number = question_number;
            this.question = question;
            this.firstAnswers = firstAnswers;
            this.answersTastes = answersTastes;
        }
    }
}