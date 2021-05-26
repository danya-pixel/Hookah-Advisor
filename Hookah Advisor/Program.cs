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
using Hookah_Advisor.TelegramBot;
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor
{
    class Program
    {
        static ITelegramBotClient _botClient;
        private static readonly UserRepository UserRepository = new(new UserParser());
        private static readonly TobaccoRepository TobaccoRepository = new(new TobaccoParser());

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

            if (message.Type != MessageType.Text)
            {
                await _botClient.SendTextMessageAsync(
                    message.Chat,
                    $"Не понимаю тебя, отправь мне текстовое сообщение");
                return;
            }

            TelegramMessage.MessegeRecieved(message, UserRepository,
                TobaccoRepository, _botClient);
        }

        private static async void BotOnCallbackQueryReceived(object sender,
            CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackHandler.BotOnCallbackQueryReceived(UserRepository, TobaccoRepository, callbackQueryEventArgs,
                _botClient);
        }
    }
}