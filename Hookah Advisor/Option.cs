using System.Collections.Generic;

namespace Hookah_Advisor
{
    public class Option
    {
        public int QuestionNumber { get; }

        public string Question { get; }

        public List<string> AnswersTastes { get; }

        public List<string> FirstAnswers { get; }


        public Option(int questionNumber, string question, List<string> answersTastes, List<string> firstAnswers)
        {
            QuestionNumber = questionNumber;
            Question = question;
            FirstAnswers = firstAnswers;
            AnswersTastes = answersTastes;
        }
    }
}