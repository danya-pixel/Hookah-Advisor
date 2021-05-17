using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading.Tasks;
using Hookah_Advisor.Repositories;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient botClient;
        private static UserRepository userRepository = new UserRepository();
        private static TobaccoRepository tobaccoRepository = new TobaccoRepository();
        private const string buttonSearch = "Поиск";
        private const string buttonRecomenations = "Рекомендации";
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
            //var tobaccoRepository = new TobaccoRepository();
            var userRepository = new UserRepository();


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
            }

            if (userRepository.GetUserCondition(userId).GetCondition() == userCondition.search)
            {
                
                var resultRequest = tobaccoRepository.SearchTobaccoInDict(message.Text);
                PrintArray(message.Chat, TobaccoToString(resultRequest));
                //взять новый меседж и printArray(уже реализованная функция в Program, она пишет массив в инлайнкиборд) из таблички всё с этим вкусом
            }

            if (message.Text == "Поиск")
            {
                botClient.SendTextMessageAsync(
                    chatId: message.Chat,
                    text: $"Напиши, какой вкус ты ищешь:");

                userRepository.UpdateUserCondition(userId, userCondition.search);
            }

            if (message.Text == "Рекомендации")
            {
                userRepository.UpdateUserCondition(userId, userCondition.recommendation);
                userRepository.UpdateUserQuestionNumber(userId, 0);
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

        public static string[] TobaccoToString(List<Tobacco> tobaccos)
        {
            Console.WriteLine("преобразую листы в массив");
            var array = new string[tobaccos.Count];
            Console.WriteLine(tobaccos.Count);
            for (int i = 0; i <= tobaccos.Count; i++)
            {
                array[i] = tobaccos[i].name;
                //Console.WriteLine(array[i]);
            }

            return array;
        }

        public static async void PrintArray(Chat message, string[] array)
        {
            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(array));
            Console.WriteLine("преобразую листы в массив");
            await botClient.SendTextMessageAsync(
                chatId: message.Id,
                text: "Выбирай: ",
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
                text: "говно",
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