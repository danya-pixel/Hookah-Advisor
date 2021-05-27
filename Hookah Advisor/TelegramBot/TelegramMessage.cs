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
        private const string ButtonSearch = "Поиск";
        private const string ButtonRecommendations = "Рекомендации";
        private const string ButtonSmokeLater = "Покурить позже";
        private const string ButtonHistory = "История";
        private static readonly List<string> YesOrNoKeyboard = new() {"Да", "Нет"};

        public static async void MessageReceived(Message message, IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository, ITelegramBotClient botClient)
        {
            var userFirstName = message.From.FirstName;
            var userId = message.From.Id;
            var user = userRepository.GetUserById(message.From.Id);

            switch (message.Text)
            {
                case "/start":
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
                case "/help":
                    TelegramMessageSender.SendHelpMessage(message, botClient);
                    userRepository.UpdateUserCondition(userId, UserCondition.None);
                    userRepository.UpdateUserQuestionNumber(userId, 0);
                    userRepository.Save();
                    break;

                case "/random":
                    var rnd = new Random();
                    var rndTobacco = tobaccoRepository.GetItemById(rnd.Next(0, 946));
                    TelegramMessageSender.PrintTobaccoToKeyboard(message, botClient, new List<Tobacco> {rndTobacco});
                    break;

                case ButtonSearch:
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        $"Напиши, какой вкус ты ищешь:");

                    userRepository.UpdateUserCondition(userId, UserCondition.Search);
                    break;

                case ButtonRecommendations:
                    //UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    //UserRepository.UpdateUserQuestionNumber(userId, 0);
                    ///TODO 
                    await botClient.SendTextMessageAsync(
                        message.Chat,
                        $"К сожалению, эта функция пока не работает :c");
                    //    $"Тебя интересует табак с холодком?");
                    //PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                    userRepository.UpdateUserQuestionNumber(userId, 1);
                    break;

                case ButtonSmokeLater:
                    var tobaccos = user.SmokeLater.Select(t => tobaccoRepository.GetItemById(t));
                    if (!tobaccos.Any())
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"У тебя нет планов на покур😤. \n\nДобавь что-нибудь😈😈😈");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"Ты хотел покурить: ",
                            replyMarkup: new InlineKeyboardMarkup(TelegramMessageSender.GetInlineKeyboard(
                                tobaccos.Select(t => t.ToString()),
                                user.SmokeLater, "tobaccoFromRequest")));
                    }

                    break;

                case ButtonHistory:
                    var tobaccosHistory = user.SmokedHistory.Select(t => tobaccoRepository.GetItemById(t));
                    if (!tobaccosHistory.Any())
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"Да ты еще не курил ниче");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(
                            message.Chat,
                            $"История твоих покуров🤤🤤🤤",
                            replyMarkup: new InlineKeyboardMarkup(TelegramMessageSender.GetInlineKeyboard(
                                tobaccosHistory.Select(t => t.ToString()),
                                user.SmokedHistory, "tobaccoFromRequest")));
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
                                    $"К сожалению, у меня нет табака с таким вкусом :c");
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