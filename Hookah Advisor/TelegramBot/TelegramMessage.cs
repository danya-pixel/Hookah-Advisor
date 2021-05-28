using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Hookah_Advisor.Repository_Interfaces;


namespace Hookah_Advisor.TelegramBot
{
    public static class TelegramMessage
    {
        public static async void MessageReceived(Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository, ITelegramBotClient botClient)
        {
            var userFirstName = message.From.FirstName;
            var userId = message.From.Id;

            switch (message.Text)
            {
                case BotSettings.StartCommand:
                {
                    TelegramMessageSender.SendStartMessage(message, botClient);
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
                    
                    break;
                }
                case BotSettings.HelpCommand:
                    TelegramMessageSender.SendHelpMessage(message, botClient);
                    userRepository.UpdateUserCondition(userId, UserCondition.None);
                    userRepository.UpdateUserQuestionNumber(userId, 0);
                    userRepository.Save();
                    break;

                case BotSettings.RandomCommand:
                    var rnd = new Random();
                    var repoSize = tobaccoRepository.GetRepositorySize()-1;
                    var rndTobacco = tobaccoRepository.GetItemById(rnd.Next(0, repoSize));
                    TelegramMessageSender.PrintTobaccoToKeyboard(message, botClient, new List<Tobacco> {rndTobacco});
                    break;

                case BotSettings.ClearHistoryCommand:
                    var user = userRepository.GetUserById(message.From.Id);
                    user.SmokedHistory.Clear();
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        BotSettings.ClearHistoryMessage);
                    break;
                
                case BotSettings.ButtonSearch:
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        BotSettings.SearchQuestion);

                    userRepository.UpdateUserCondition(userId, UserCondition.Search);
                    break;

                case BotSettings.ButtonRecommendations:
                    //UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    //UserRepository.UpdateUserQuestionNumber(userId, 0);
                    ///TODO 
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        "К сожалению, эта функция пока не работает :c");
                    //    $"Тебя интересует табак с холодком?");
                    //PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                    userRepository.UpdateUserQuestionNumber(userId, 1);
                    break;

                case BotSettings.ButtonSmokeLater:
                    user = userRepository.GetUserById(message.From.Id);
                    var tobaccos = user.SmokeLater.Select(t => tobaccoRepository.GetItemById(t));
                    if (!tobaccos.Any())
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            BotSettings.SmokeLaterEmpty);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            BotSettings.SmokeLaterMessage,
                            replyMarkup: new InlineKeyboardMarkup(TelegramMessageSender.GetInlineKeyboard(
                                tobaccos.Select(t => t.ToString()),
                                user.SmokeLater, BotSettings.TypeSearchTobacco)));
                    }

                    break;

                case BotSettings.ButtonHistory: 
                    user = userRepository.GetUserById(message.From.Id);
                    var tobaccosHistory = user.SmokedHistory.Select(t => tobaccoRepository.GetItemById(t));
                    if (!tobaccosHistory.Any())
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            BotSettings.SmokedHistoryEmpty);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            BotSettings.SmokedHistoryMessage,
                            replyMarkup: new InlineKeyboardMarkup(TelegramMessageSender.GetInlineKeyboard(
                                tobaccosHistory.Select(t => t.ToString()),
                                user.SmokedHistory, BotSettings.TypeSearchTobacco)));
                    }

                    break;

                default:
                    switch (userRepository.GetUserCondition(userId).GetCondition())
                    {
                        case UserCondition.None:
                            break;
                        case UserCondition.Search:
                        {
                            var resultRequest = tobaccoRepository.SearchTobaccoInDict(message.Text.ToLower());
                            if (resultRequest.Count == 0)
                            {
                                await botClient.SendTextMessageAsync(
                                    message.Chat,
                                    BotSettings.SearchListEmpty);
                            }
                            else
                                TelegramMessageSender.PrintTobaccoToKeyboard(message, botClient, resultRequest);

                            break;
                        }

                        case UserCondition.Recommendation:
                            break;
                    }

                    break;
            }
        }
    }
}