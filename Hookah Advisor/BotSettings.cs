using System;
using System.Collections.Generic;

namespace Hookah_Advisor
{
    public static class BotSettings
    {
        public static string Token { get; } = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

        public const string ButtonSearch = "Поиск";
        public const string ButtonRecommendations = "Рекомендации";
        public const string ButtonSmokeLater = "Покурить позже";
        public const string ButtonHistory = "История";

        public const string StartCommand = "/start";
        public const string HelpCommand = "/help";
        public const string RandomCommand = "/random";
        public const string ClearHistoryCommand = "/clearhistory";

        public const string SearchQuestion = "Напиши, какой вкус ты ищешь:";
        public const string SearchListEmpty = "К сожалению, у меня нет табака с таким вкусом :c";
        public const string SearchListMessage = "Смотри, что я нашёл:";
        public const string ClearHistoryMessage = "История покуров очищена😤";

        public const string SmokeLaterEmpty = "У тебя нет планов на покур😤. \n\nДобавь что-нибудь😈😈😈";
        public const string SmokeLaterMessage = "Ты хотел покурить: ";

        public const string SmokedHistoryEmpty = "Ты еще ничего не курил😳😳😳";
        public const string SmokedHistoryMessage = "История твоих покуров🤤🤤🤤";

        public const string HelloMessage = "Привет, ";

        public const string StartMessage = ",\n" +
                                           "Добро пожаловать в бота HookahAdvisor \n" + "\n" +
                                           "Этот бот помогает найти табак для кальяна под твои предпочтения💨 \n " +
                                           "\n" +
                                           "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст🔞\n" +
                                           "\n" +
                                           " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                                           " «Рекомендации⭐️» подсказывают табак на основании опроса. \n" + "\n" +
                                           " «Покурить позже🌫» хранит все сохранённые тобой табаки. \n" + "\n" +
                                           " Жми на нужную тебе кнопку снизу!👇";

        public const string InvalidMessage = "Не понимаю тебя, отправь мне текстовое сообщение";

        public const string HelpMessage = "Этот бот помогает найти табак для кальяна под твои предпочтения💨\n" + "\n" +
                                          "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст🔞\n" +
                                          "\n" +
                                          "«Поиск🔎» помогает найти табак по твоему запросу.\n" + "\n" +
                                          "«Рекомендации⭐️» подсказывают табак на основании опроса.\n" + "\n" +
                                          "«Покурить позже🌫» хранит все сохранённые тобой табаки.\n" + "\n" +
                                          "Жми на нужную тебе кнопку снизу!👇\n";

        public const string TypeSearchTobacco = "tobaccoFromRequest";
        public const string TypeUnSmoke = "unShmokeLater";
        public const string TypeSmokeLater = "shmokeLater";

        public const string KeyboardSmokeLater = "Покурить позже";
        public const string KeyboardUnSmokeLater = "Я покурил";
        public const string AnswerSmokeLater = "Покумарим";
        public const string AnswerUnSmokeLater = "Покалюмбасили";

        private static readonly List<string> YesOrNoKeyboard = new() {"Да", "Нет"};
    }
}