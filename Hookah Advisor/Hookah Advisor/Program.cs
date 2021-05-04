using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Hookah_Advisor.Repositories;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient botClient;
        private static UserRepository userRepository;
        private const string buttonSearch = "Поиск";
        private const string buttonRecomenations = "Рекомандации";
        private const string buttonHistory = "История";

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
            var tobaccoRepository = new TobaccoRepository();
            switch (message.Text)
            {
                case "/start":
                    SendStartMessage(message.Chat, userFirstName);
                    break;
                case "/help":
                    SendHelpMessage(message.Chat);
                    break;
                case "Поиск":
                    tobaccoRepository.SearchTobacco(message.Text);
                    break;
            }

            await SendInlineKeyboard(message);
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
                text: $"Что тебе интересно?");
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

        static async void SearchTobacco(Chat message)
        {
            string[] tastes = new string[]
            {
                "банан", "дыня", "десерт", "печенье", "хлопья", "пирог", "мороженое", "конфеты",
                "шоколад", "мёд", "жвачка", "карамель", "ваниль", "маффин", "фрукты", "дыня", "персик", "киви", "яблоко", "ананас", "груша", "личи", "питайя"
            };
            
            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(tastes));

            await botClient.SendTextMessageAsync(
                chatId: message.Id,
                text: "Выбирай вкус табака: ",
                replyMarkup: keyboardMarkup
            );
        }

        private static InlineKeyboardButton[][] GetInlineKeyboard(string[] stringArray)
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
                            "Some Callback Data" //хз почему, но без этой строчки бот падает из-за того что не может парсить idk
                    }
                };
            }

            return keyboardInline;
        }

        static async Task SendInlineKeyboard(Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await Task.Delay(500);

            // Simulate longer running task

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "тест",
                replyMarkup: GetButtons()
            );
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>
                    {
                        new KeyboardButton {Text = buttonSearch}, new KeyboardButton {Text = buttonRecomenations},
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
            switch (callbackQuery.Id)
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
