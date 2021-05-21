using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.Repository_Interfaces;


namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient botClient;
        private static UserRepository userRepository = new();
        private static TobaccoRepository tobaccoRepository = new();
        private const string buttonSearch = "Поиск";
        private const string buttonRecomendations = "Рекомендации";
        private const string buttonHistory = "История";
        private static readonly string[] YesOrNoKeyboard = {"Да", "Нет"};

        static void Main()
        {
            botClient = new TelegramBotClient(BotSettings.Token);

            var me = botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            botClient.OnMessage += Bot_OnMessage;
            botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id;
            var userFirstName = message.From.FirstName;

            if (message.Text == "/start")
            {
                SendStartMessage(message.Chat, userFirstName);
                if (!userRepository.IsUserRegistered(userId))
                {
                    userRepository.AddUserById(userId, userFirstName);
                }
                else
                {
                    userRepository.UpdateUserCondition(userId, userCondition.none);
                    userRepository.UpdateUserQuestionNumber(userId, 0);
                }
            }

            if (message.Text == "/help")
            {
                SendHelpMessage(message.Chat);
                userRepository.UpdateUserCondition(userId, userCondition.none);
                userRepository.UpdateUserQuestionNumber(userId, 0);
                userRepository.SaveToJson("users.json");
            }

            if (userRepository.GetUserCondition(userId).GetCondition() == userCondition.search)
            {
                var resultRequest = tobaccoRepository.SearchTobaccoInDict(message.Text.ToLower());
                if (resultRequest.Count == 0)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: $"К сожалению, у меня нет табака с таким вкусом :c");
                }
                else PrintTobaccoToKeyboard(message.Chat, resultRequest);
            }

            if (message.Text == "Поиск")
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: $"Напиши, какой вкус ты ищешь:");

                userRepository.UpdateUserCondition(userId, userCondition.search);
            }

            if (userRepository.GetUserCondition(userId).GetCondition() == userCondition.recommendation)
            {
                //userRepository.GetUserCondition(userId).GetQuestionNumber() == 
            }

            if (message.Text == "Рекомендации")
            {
                userRepository.UpdateUserCondition(userId, userCondition.recommendation);
                userRepository.UpdateUserQuestionNumber(userId, 0);

                await botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: $"Тебя интересует табак с холодком?");
                PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                userRepository.UpdateUserQuestionNumber(userId, 1);
            }
        }

        static async void SendStartMessage(Chat chat, string userFirstName)
        {
            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Привет {userFirstName},\n" +
                      "Добро пожаловать в бота HookahAdvisor \n" + "\n" +
                      "Этот бот помогает найти табак для кальяна под твои предпочтения.💨 \n " + "\n" +
                      "Внимание! Данный бот разрешен только лицам, достигшим возраста 18 лет.🔞 \n" + "\n" +
                      " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                      " «Рекомендации⭐️» подсказывают табак под твои предпочтения, основанные на истории. \n" + "\n" +
                      " «История📜» хранит все оцененные тобой табаки. \n" + "\n" +
                      " Жми на нужную тебе кнопку снизу!👇");

            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Что тебе интересно?",
                replyMarkup: GetButtons());
        }

        static async void SendHelpMessage(Chat chat)
        {
            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Этот бот помогает найти табак для кальяна под твои предпочтения.\n" + "\n" +
                      "Внимание! Данный бот разрешен только лицам, достигшим возраста 18 лет.🔞\n" + "\n" +
                      "«Поиск🔎» помогает найти табак по твоему запросу.\n" + "\n" +
                      "«Рекомендации⭐️» подсказывают табак под твои предпочтения, основанные на истории.\n" + "\n" +
                      "«История📜» хранит все оцененные тобой табаки.\n" + "\n" +
                      "Жми на нужную тебе кнопку снизу!👇\n");
        }

        public static string[] TobaccoToString(List<Tobacco> tobaccos)
        {
            Console.WriteLine("преобразую листы в массив");
            var array = new string[tobaccos.Count];
            Console.WriteLine(tobaccos.Count);
            for (var i = 0; i < tobaccos.Count; i++)
            {
                array[i] = tobaccos[i].brand + ": " + tobaccos[i].name;
            }

            return array;
        }

        public static async void PrintTobaccoToKeyboard(Chat message, List<Tobacco> tobaccos)
        {
            var array = TobaccoToString(tobaccos);
            var idTobaccos = new List<int>();

            foreach (var tobacco in tobaccos)
            {
                idTobaccos.Add(tobacco.id);
            }

            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboardForSearch(array, idTobaccos));
            Console.WriteLine("преобразую листы в массив");
            await botClient.SendTextMessageAsync(
                chatId: message.Id,
                text: "Выбирай: ",
                replyMarkup: keyboardMarkup
            );
        }

        private static InlineKeyboardButton[][] GetInlineKeyboardForSearch(string[] stringArray, List<int> idTobaccos)
        {
            var keyboardInline = new InlineKeyboardButton[stringArray.Length][];

            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardInline[i] = new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = stringArray[i],
                        CallbackData =
                            "tobaccoFromRequest_" + idTobaccos[i]
                    }
                };
            }

            return keyboardInline;
        }

        public static async void PrintAnswerOptionsToKeyboard(Chat message, string[] array)
        {
            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboardForRecomendation(array));
            Console.WriteLine("преобразую листы в массив");
            await botClient.SendTextMessageAsync(
                chatId: message.Id,
                text: "Выбирай: ",
                replyMarkup: keyboardMarkup
            );
        }

        private static InlineKeyboardButton[][] GetInlineKeyboardForRecomendation(string[] stringArray)
        {
            var keyboardInline = new InlineKeyboardButton[stringArray.Length][];

            for (var i = 0; i < stringArray.Length; i++)
            {
                keyboardInline[i] = new InlineKeyboardButton[]
                {
                    new InlineKeyboardButton
                    {
                        Text = stringArray[i],
                        CallbackData =
                            "yesno_" + i
                    }
                };
            }

            return keyboardInline;
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton {Text = buttonSearch}, new KeyboardButton {Text = buttonRecomendations},
                        new KeyboardButton {Text = buttonHistory}
                    }
                },
                ResizeKeyboard = true
            };
        }

        private static async void BotOnCallbackQueryReceived(object sender,
            CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            var callbackData = callbackQuery.Data;
            var keyboardCondition = callbackData.Split('_')[0];

            if (keyboardCondition == "tobaccoFromRequest")
            {
            }

            if (keyboardCondition == "")
            {
            }

            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }
    }
}