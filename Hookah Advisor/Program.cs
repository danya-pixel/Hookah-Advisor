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
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient _botClient;
        private static readonly UserRepository UserRepository = new(new UserParser());
        private static readonly TobaccoRepository TobaccoRepository = new(new TobaccoParser());
        private const string ButtonSearch = "Поиск";
        private const string ButtonRecommendations = "Рекомендации";
        private const string ButtonSmokeLater = "Покурить позже";
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
            UserRepository.Save();
        }

        private static async void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            var userId = message.From.Id;
            var userFirstName = message.From.FirstName;

            if (message.Type != MessageType.Text)
            {
                await _botClient.SendTextMessageAsync(
                    message.Chat,
                    $"Не понимаю тебя, отправь мне текстовое сообщение");
                return;
            }

            switch (message.Text)
            {
                case "/start":
                {
                    SendStartMessage(message.Chat, userFirstName);
                    if (!UserRepository.IsUserRegistered(userId))
                    {
                        UserRepository.AddUserById(userId, userFirstName);
                        UserRepository.Save();
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

                case "/random":
                    var rnd = new Random();
                    var rndTobacco = TobaccoRepository.GetItemById(rnd.Next(0, 946));
                    PrintTobaccoToKeyboard(message.Chat, new List<Tobacco> {rndTobacco});
                    break;

                case ButtonSearch:
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"Напиши, какой вкус ты ищешь:");

                    UserRepository.UpdateUserCondition(userId, userCondition.search);
                    break;

                case ButtonRecommendations:
                    //UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    //UserRepository.UpdateUserQuestionNumber(userId, 0);
                    ///TODO 
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"К сожалению, эта функция пока не работает :c");
                    //    $"Тебя интересует табак с холодком?");
                    //PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                    UserRepository.UpdateUserQuestionNumber(userId, 1);
                    break;

                case ButtonSmokeLater:
                    var user = UserRepository.GetUserById(message.From.Id);
                    var tobaccos = user.SmokeLater.Select(t => TobaccoRepository.GetItemById(t));
                    if (tobaccos.Count() == 0)
                    {
                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"У тебя нет планов на покур😤. \n\nДобавь что-нибудь😈😈😈");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"Ты хотел покурить: ",
                            replyMarkup: new InlineKeyboardMarkup(GetInlineKeyboard(tobaccos.Select(t => t.ToString()),
                                user.SmokeLater, "tobaccoFromRequest")));
                    }

                    break;

                case ButtonHistory:
                    ///TODO
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"К сожалению, эта функция пока не работает :c");
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
                            else
                                PrintTobaccoToKeyboard(message.Chat, resultRequest);

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
                "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст.🔞\n" +
                "\n" +
                " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                " «Рекомендации⭐️» подсказывают табак на основании опроса. \n" + "\n" +
                " «Покурить позже🌫» хранит все сохранённые тобой табаки. \n" + "\n" +
                " Жми на нужную тебе кнопку снизу!👇", replyMarkup: GetButtons());
        }

        static async void SendHelpMessage(Chat chat)
        {
            await _botClient.SendTextMessageAsync(
                chat,
                $"Этот бот помогает найти табак для кальяна под твои предпочтения.\n" + "\n" +
                "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст.🔞\n" +
                "\n" +
                "«Поиск🔎» помогает найти табак по твоему запросу.\n" + "\n" +
                "«Рекомендации⭐️» подсказывают табак на основании опроса.\n" + "\n" +
                "«Покурить позже🌫» хранит все сохранённые тобой табаки.\n" + "\n" +
                "Жми на нужную тебе кнопку снизу!👇\n");
        }

        public static async void PrintTobaccoToKeyboard(Chat message, List<Tobacco> tobaccos)
        {
            var array = tobaccos.Select(t => t.ToString());
            var idTobaccos = tobaccos.Select(t => t.id);

            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(array, idTobaccos, "tobaccoFromRequest"));
            await _botClient.SendTextMessageAsync(
                message.Id,
                "Смотри, что я нашёл:",
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

        private static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(string str, T idTobacco,
            string type)
        {
            return new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                }
            };
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new() {new KeyboardButton {Text = ButtonSearch}, new KeyboardButton {Text = ButtonRecommendations}},
                    new() {new KeyboardButton {Text = ButtonSmokeLater}, new KeyboardButton {Text = ButtonHistory}}
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
            var user = UserRepository.GetUserById(callbackQuery.From.Id);

            switch (type)
            {
                case "tobaccoFromRequest":
                    var tags = new HashSet<string>();
                    foreach (var t in tobaccoFromTap.categories.Select(t => $"#{t}"))
                    {
                        tags.Add(t);
                    }

                    foreach (var t in tags.Concat(tobaccoFromTap.tastes.Select(t => $"#{t}")))
                    {
                        tags.Add(t);
                    }

                    var result =
                        $"{tobaccoFromTap}\n{string.Join(" ", tags)}\n\n{tobaccoFromTap.description}";
                    if (user.SmokeLater.Contains(idTobacco))
                    {
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                        await _botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, result,
                            replyMarkup: new InlineKeyboardMarkup(
                                GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                        await _botClient.AnswerCallbackQueryAsync(
                            callbackQuery.Id,
                            $"{tobaccoFromTap}"
                        );
                    }

                    break;
                case "shmokeLater":
                    user.SmokeLater.Add(idTobacco);
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покумарим {tobaccoFromTap}"
                    );
                    await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            GetInlineKeyboard("Я покурил", idTobacco, "unShmokeLater")));
                    break;
                case "unShmokeLater":
                    user.SmokeLater.Remove(idTobacco);
                    await _botClient.AnswerCallbackQueryAsync(
                        callbackQuery.Id,
                        $"Покалюмбасили {tobaccoFromTap}"
                    );
                    await _botClient.EditMessageTextAsync(callbackQuery.Message.Chat.Id,
                        callbackQuery.Message.MessageId, callbackQuery.Message.Text,
                        replyMarkup: new InlineKeyboardMarkup(
                            GetInlineKeyboard("Покурить позже", idTobacco, "shmokeLater")));
                    break;
            }
        }
    }
}