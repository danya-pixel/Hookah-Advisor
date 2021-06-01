using System;
using System.Collections.Generic;
using System.Linq;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Hookah_Advisor.TelegramBot
{
    public static class Commands
    {
        public static void TextReceived(ITelegramBotClient botClient, Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository)
        {
            var userId = message.From.Id;

            switch (userRepository.GetUserCondition(userId).GetCondition())
            {
                case UserCondition.None:
                    break;
                case UserCondition.Search:
                {
                    if (message.Text.Length < 3) return;
                    var resultRequest = tobaccoRepository.SearchItemInDict(message.Text.ToLower());
                    if (resultRequest.Count == 0)
                    {
                        MessageSender.SendText(BotSettings.SearchListEmpty, botClient, message);
                    }
                    else
                        MessageSender.PrintTobaccosToKeyboard(message, botClient, resultRequest);

                    break;
                }

                case UserCondition.Recommendation:
                    break;
            }
        }

        public static void Start(ITelegramBotClient botClient, Message message, IUserRepository userRepository,
            string userFirstName)
        {
            MessageSender.SendStartMessage(message, botClient);
            var userId = message.From.Id;

            if (!userRepository.IsUserRegistered(userId))
            {
                userRepository.AddUserById(userId, userFirstName);
                userRepository.Save();
            }
            else
            {
                userRepository.UpdateUserCondition(userId, UserCondition.None);
                userRepository.UpdateUserQuestionNumber(userId, 0);
            }
        }

        public static void Help(ITelegramBotClient botClient, Message message, IUserRepository userRepository)
        {
            var userId = message.From.Id;
            MessageSender.SendText(BotSettings.HelpMessage, botClient, message);
            userRepository.UpdateUserCondition(userId, UserCondition.None);
            userRepository.UpdateUserQuestionNumber(userId, 0);
            userRepository.Save();
        }

        public static void Random(ITelegramBotClient botClient, Message message,
            IItemRepository<Tobacco> tobaccoRepository)
        {
            var repositorySize = tobaccoRepository.GetRepositorySize() - 1;
            var randomTobacco = tobaccoRepository.GetItemById(new Random().Next(0, repositorySize));
            MessageSender.PrintTobaccosToKeyboard(message, botClient, new List<Tobacco> {randomTobacco});
        }

        public static void ClearHistory(ITelegramBotClient botClient, Message message, IUserRepository userRepository)
        {
            var user = userRepository.GetUserById(message.From.Id);
            user.SmokedHistory.Clear();

            MessageSender.SendText(BotSettings.ClearHistoryMessage, botClient, message);
        }

        public static void Search(ITelegramBotClient botClient, Message message, IUserRepository userRepository)
        {
            var userId = message.From.Id;
            MessageSender.SendText(BotSettings.SearchQuestion, botClient, message);
            userRepository.UpdateUserCondition(userId, UserCondition.Search);
        }

        public static void Recommendation(ITelegramBotClient botClient, Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository)
        {
            var userId = message.From.Id;
            userRepository.UpdateUserCondition(userId, UserCondition.Recommendation);
            userRepository.UpdateUserQuestionNumber(userId, 0);
        }

        public static void SmokeLater(ITelegramBotClient botClient, Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository)
        {
            var user = userRepository.GetUserById(message.From.Id);
            var tobaccos = user.SmokeLater.Select(t => tobaccoRepository.GetItemById(t));
            if (!tobaccos.Any())
            {
                MessageSender.SendText(BotSettings.SmokeLaterEmpty, botClient, message);
            }
            else
            {
                MessageSender.SendTextWithInlineKeyboard(BotSettings.SmokeLaterMessage, BotSettings.TypeSearchTobacco,
                    botClient, message, tobaccos, user);
            }
        }

        public static void History(ITelegramBotClient botClient, Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository)
        {
            var user = userRepository.GetUserById(message.From.Id);
            var tobaccosHistory = user.SmokedHistory.Select(t => tobaccoRepository.GetItemById(t));
            if (!tobaccosHistory.Any())
            {
                MessageSender.SendText(BotSettings.SmokedHistoryEmpty, botClient, message);
            }
            else
            {
                MessageSender.SendTextWithInlineKeyboard(BotSettings.SmokedHistoryMessage,
                    BotSettings.TypeSearchTobacco,
                    botClient, message, tobaccosHistory, user);
            }
        }
    }
}