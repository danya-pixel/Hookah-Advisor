using System;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Hookah_Advisor.TelegramBot
{
    public class TelegramBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUserRepository _userRepository;
        private readonly IItemRepository<Tobacco> _itemRepository;
        private readonly IRecommendation<Option> _recommendation;

        ~TelegramBot()
        {
            Console.WriteLine("I sdox");
            Stop();
        }

        public TelegramBot(ITelegramBotClient botClient, IItemRepository<Tobacco> itemRepository,
            IUserRepository userRepository, IRecommendation<Option> recommendation)
        {
            _botClient = botClient;
            _itemRepository = itemRepository;
            _userRepository = userRepository; 
            _recommendation = recommendation;
            _botClient.OnMessage += BotOnMessage;
            _botClient.OnCallbackQuery += BotOnCallbackQueryReceived;
        }

        public void Start()
        {
            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Bot is working with id: {me.Id} and name {me.FirstName}."
            );
            _botClient.StartReceiving();
        }

        public void Stop()
        {
            _botClient.StopReceiving();
            _userRepository.Save();
        }

        private void BotOnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type != MessageType.Text)
            {
                MessageSender.SendText(BotSettings.InvalidMessage, _botClient, message);
                return;
            }

            MessageHandler.MessageReceived(message, _userRepository,
                _itemRepository, _botClient, _recommendation);
        }

        private void BotOnCallbackQueryReceived(object sender,
            CallbackQueryEventArgs callbackQueryEventArgs)
        {
            CallbackHandler.BotOnCallbackQueryReceived(_userRepository, _itemRepository, _recommendation, callbackQueryEventArgs,
                _botClient);
        }
    }
}