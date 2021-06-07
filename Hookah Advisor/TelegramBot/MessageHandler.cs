using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Hookah_Advisor.Repository_Interfaces;


namespace Hookah_Advisor.TelegramBot
{
    public static class MessageHandler
    {
        public static void MessageReceived(Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository, ITelegramBotClient botClient,
            IRecommendation<Option> recommendation
        )
        {
            var userFirstName = message.From.FirstName;
            var userId = message.From.Id;

            if (IsInvalidMessage(userId, userRepository, message))
            {
                MessageSender.SendText(BotSettings.InvalidUserMessage, botClient, message);
                return;
            }
            

            switch (message.Text)
            {
                case BotSettings.StartCommand:
                {
                    Commands.Start(botClient, message, userRepository, userFirstName);
                    break;
                }

                case BotSettings.HelpCommand:
                    Commands.Help(botClient, message, userRepository);
                    break;

                case BotSettings.RandomCommand:
                    Commands.Random(botClient, message, tobaccoRepository);
                    break;

                case BotSettings.ClearHistoryCommand:
                    Commands.ClearHistory(botClient, message, userRepository);
                    break;

                case BotSettings.ButtonSearch:
                    Commands.Search(botClient, message, userRepository);
                    break;

                case BotSettings.ButtonRecommendation:


                    Commands.Recommendation(botClient, message, userRepository, recommendation, tobaccoRepository);

                    break;

                case BotSettings.ButtonSmokeLater:
                    Commands.SmokeLater(botClient, message, userRepository, tobaccoRepository);
                    break;

                case BotSettings.ButtonHistory:
                    Commands.History(botClient, message, userRepository, tobaccoRepository);
                    break;

                default:
                    Commands.TextReceived(botClient, message, userRepository, tobaccoRepository, recommendation,
                        "default");
                    break;
            }
        }

        private static bool IsInvalidMessage(int userId, IUserRepository userRepository, Message message)
        {
            return !userRepository.IsUserRegistered(userId) & message.Text != BotSettings.StartCommand;
        }
    }
}