using System;
using System.Collections.Generic;
using Hookah_Advisor.Parsers;
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
        static ITelegramBotClient _botClient;
        private static readonly UserRepository UserRepository = new(new UserParser());
        private static readonly TobaccoRepository TobaccoRepository = new(new TobaccoParser());
        private const string ButtonSearch = "Поиск";
        private const string ButtonRecommendations = "Рекомендации";
        private const string ButtonHistory = "История";
        private static readonly string[] YesOrNoKeyboard = {"Да", "Нет"};

        static void Main()
        {
            _botClient = new TelegramBotClient(BotSettings.Token);

            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            _botClient.OnMessage += BotOnMessage;
            _botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
            _botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            _botClient.StopReceiving();
        }

        private static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id;
            var userFirstName = message.From.FirstName;


            switch (message.Text)
            {
                case "/start":
                {
                    SendStartMessage(message.Chat, userFirstName);
                    if (!UserRepository.IsUserRegistered(userId))
                    {
                        UserRepository.AddUserById(userId, userFirstName);
                    }
                    else
                    {
                        UserRepository.UpdateUserCondition(userId, userCondition.none);
                        UserRepository.UpdateUserQuestionNumber(userId, 0);
                    }

                    break;
                }
                case "/help":
                    SendHelpMessage(message.Chat);
                    UserRepository.UpdateUserCondition(userId, userCondition.none);
                    UserRepository.UpdateUserQuestionNumber(userId, 0);
                    UserRepository.Save();
                    break;

                case "Поиск":
                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: $"Напиши, какой вкус ты ищешь:");

                    UserRepository.UpdateUserCondition(userId, userCondition.search);
                    break;
                case "Рекомендации":
                    UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    UserRepository.UpdateUserQuestionNumber(userId, 0);

                    await _botClient.SendTextMessageAsync(
                        chatId: message.Chat,
                        text: $"Тебя интересует табак с холодком?");
                    PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                    UserRepository.UpdateUserQuestionNumber(userId, 1);
                    break;
                default:
                    switch (UserRepository.GetUserCondition(userId).GetCondition())
                    {
                        case userCondition.none:
                            break;
                        case userCondition.search:
                        {
                            var resultRequest = TobaccoRepository.SearchTobaccoInDict(message.Text.ToLower());
                            if (resultRequest.Count == 0)
                            {
                                await _botClient.SendTextMessageAsync(
                                    chatId: message.Chat,
                                    text: $"К сожалению, у меня нет табака с таким вкусом :c");
                            }
                            else PrintTobaccoToKeyboard(message.Chat, resultRequest);

                            break;
                        }

                        case userCondition.recommendation:
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        static async void SendStartMessage(Chat chat, string userFirstName)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Привет {userFirstName},\n" +
                      "Добро пожаловать в бота HookahAdvisor \n" + "\n" +
                      "Этот бот помогает найти табак для кальяна под твои предпочтения.💨 \n " + "\n" +
                      "Внимание! Данный бот разрешен только лицам, достигшим возраста 18 лет.🔞 \n" + "\n" +
                      " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                      " «Рекомендации⭐️» подсказывают табак под твои предпочтения, основанные на истории. \n" + "\n" +
                      " «История📜» хранит все оцененные тобой табаки. \n" + "\n" +
                      " Жми на нужную тебе кнопку снизу!👇");

            await _botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Что тебе интересно?",
                replyMarkup: GetButtons());
        }

        static async void SendHelpMessage(Chat chat)
        {
            await _botClient.SendTextMessageAsync(
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
            await _botClient.SendTextMessageAsync(
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
            await _botClient.SendTextMessageAsync(
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
                        new KeyboardButton {Text = ButtonSearch}, new KeyboardButton {Text = ButtonRecommendations},
                        new KeyboardButton {Text = ButtonHistory}
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

            await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await _botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }
    }
}