using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly List<string> YesOrNoKeyboard = new() {"Да", "Нет"};

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
                        message.Chat,
                        $"Напиши, какой вкус ты ищешь:");

                    UserRepository.UpdateUserCondition(userId, userCondition.search);
                    break;
                case "Рекомендации":
                    UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    UserRepository.UpdateUserQuestionNumber(userId, 0);

                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"Тебя интересует табак с холодком?");
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
                                    message.Chat,
                                    $"К сожалению, у меня нет табака с таким вкусом :c");
                            }
                            else PrintTobaccoToKeyboard(message.Chat, resultRequest);

                            break;
                        }

                        case userCondition.recommendation:
                            break;
                    }

                    break;
            }
        }

        static async void SendStartMessage(Chat chat, string userFirstName)
        {
            await _botClient.SendTextMessageAsync(
                chat,
                $"Привет {userFirstName},\n" +
                "Добро пожаловать в бота HookahAdvisor \n" + "\n" +
                "Этот бот помогает найти табак для кальяна под твои предпочтения.💨 \n " + "\n" +
                "Внимание! Данный бот разрешен только лицам, достигшим возраста 18 лет.🔞 \n" + "\n" +
                " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                " «Рекомендации⭐️» подсказывают табак под твои предпочтения, основанные на истории. \n" + "\n" +
                " «История📜» хранит все оцененные тобой табаки. \n" + "\n" +
                " Жми на нужную тебе кнопку снизу!👇");

            await _botClient.SendTextMessageAsync(
                chat,
                $"Что тебе интересно?",
                replyMarkup: GetButtons());
        }

        static async void SendHelpMessage(Chat chat)
        {
            await _botClient.SendTextMessageAsync(
                chat,
                $"Этот бот помогает найти табак для кальяна под твои предпочтения.\n" + "\n" +
                "Внимание! Данный бот разрешен только лицам, достигшим возраста 18 лет.🔞\n" + "\n" +
                "«Поиск🔎» помогает найти табак по твоему запросу.\n" + "\n" +
                "«Рекомендации⭐️» подсказывают табак под твои предпочтения, основанные на истории.\n" + "\n" +
                "«История📜» хранит все оцененные тобой табаки.\n" + "\n" +
                "Жми на нужную тебе кнопку снизу!👇\n");
        }
        
        public static async void PrintTobaccoToKeyboard(Chat message, List<Tobacco> tobaccos)
        {
            var array = tobaccos.Select(t => t.ToString());
            var idTobaccos = tobaccos.Select(t => t.id);

            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(array, idTobaccos, "tobaccoFromRequest"));
            await _botClient.SendTextMessageAsync(
                message.Id,
                "Выбирай: ",
                replyMarkup: keyboardMarkup
            );
        }

        private static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(
            IEnumerable<string> stringArray,
            IEnumerable<T> idTobaccos, string type)
        {
            var keyboardInline = stringArray
                .Zip(idTobaccos, (str, idTobacco) => new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                });

            return keyboardInline;
        }

        public static async void PrintAnswerOptionsToKeyboard(Chat message, List<string> array)
        {
            var keyboardMarkup =
                new InlineKeyboardMarkup(GetInlineKeyboard(array, Enumerable.Range(0, array.Count), "yesno_"));
            await _botClient.SendTextMessageAsync(
                message.Id,
                "Выбирай: ",
                replyMarkup: keyboardMarkup
            );
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new()
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
            var type = callbackData.Split('_')[0];
            var idTobacco = Convert.ToInt32(callbackData.Split('_')[1]);
            var tobaccoFromTap = TobaccoRepository.GetItemById(idTobacco);
            var result = tobaccoFromTap.brand + ": " + tobaccoFromTap.name + "\n" + "\n" + tobaccoFromTap.description;//прикрутить перегруженный tostring

            if (type == "tobaccoFromRequest")
            {
                await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result);
            }

            if (type == "")
            {
            }
            
            /*await _botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );*/
        }
    }
}