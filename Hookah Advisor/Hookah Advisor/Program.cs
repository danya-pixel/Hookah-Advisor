using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient botClient;
        private static UsersTable usersTable;

        class UsersTable //здесь будет таблица пользователей
        {
            private Dictionary<int, string> NameByIdDict;

            public void AddUser(int userId, string userName)
            {
                if (!NameByIdDict.ContainsKey(userId))
                    NameByIdDict[userId] = userName;
                Console.WriteLine($"User {userName} has been added");
            }

            public bool IsUserRegistered(int userId)
            {
                return NameByIdDict.ContainsKey(userId);
            }

            public UsersTable()
            {
                NameByIdDict = new Dictionary<int, string>();
            }
        }

        static void Main()
        {
            botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_TOKEN"));

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
            
            await SendInlineKeyboard(message);
           
        }


        static async void SendStartMessage(Chat chat, string userFirstName)
        {
            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Привет {userFirstName},\n" + "Добро пожаловть в Hookah Advisor \n");
            
            await botClient.SendTextMessageAsync(
                chatId: chat,
                text: $"Что тебе интересно?");
        }

        static async Task SendInlineKeyboard(Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
            await Task.Delay(500);

            // Simulate longer running task

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                // first row
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Поиск", "11"),
                    InlineKeyboardButton.WithCallbackData("Рекомендации", "22"),
                    InlineKeyboardButton.WithCallbackData("История", "33"),
                },
            });
            await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: "Выбирай: ",
                replyMarkup: inlineKeyboard
            );
        }
        
        
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
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
