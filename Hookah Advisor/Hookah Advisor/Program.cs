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

            if (message.Text == "/start")
                SendStartMessage(message.Chat, userFirstName);

            if (message.Text == "/help")
                SendHelpMessage(message.Chat);

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

        static async Task SendInlineKeyboard(Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await Task.Delay(500);

            // Simulate longer running task

            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Выбирай: ",
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