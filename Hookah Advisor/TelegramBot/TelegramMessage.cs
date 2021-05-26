using Telegram.Bot.Args;
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
using Telegram.Bot.Types.Enums;


namespace Hookah_Advisor.TelegramBot
{
    public static class TelegramMessage
    {
        private const string ButtonSearch = "Поиск";
        private const string ButtonRecommendations = "Рекомендации";
        private const string ButtonSmokeLater = "Покурить позже";
        const string ButtonHistory = "История";
        private static readonly List<string> YesOrNoKeyboard = new() {"Да", "Нет"};

        public static async void MessegeRecieved(Message message, UserRepository userRepository,
            TobaccoRepository tobaccoRepository, ITelegramBotClient _botClient)
        {
            var userFirstName = message.From.FirstName;
            var userId = message.From.Id;

            switch (message.Text)
            {
                case "/start":
                {
                    TelegramMessageSender.SendStartMessage(message, _botClient);
                    if (!userRepository.IsUserRegistered(userId))
                    {
                        userRepository.AddUserById(userId, userFirstName);
                        userRepository.Save();
                    }
                    else
                    {
                        userRepository.UpdateUserCondition(userId, userCondition.none);
                        userRepository.UpdateUserQuestionNumber(userId, 0);
                    }


                    break;
                }
                case "/help":
                    TelegramMessageSender.SendHelpMessage(message, _botClient);
                    userRepository.UpdateUserCondition(userId, userCondition.none);
                    userRepository.UpdateUserQuestionNumber(userId, 0);
                    userRepository.Save();
                    break;

                case "/random":
                    var rnd = new Random();
                    var rndTobacco = tobaccoRepository.GetItemById(rnd.Next(0, 946));
                    TelegramMessageSender.PrintTobaccoToKeyboard(message, _botClient, new List<Tobacco> {rndTobacco});
                    break;

                case ButtonSearch:
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"Напиши, какой вкус ты ищешь:");

                    userRepository.UpdateUserCondition(userId, userCondition.search);
                    break;

                case ButtonRecommendations:
                    //UserRepository.UpdateUserCondition(userId, userCondition.recommendation);
                    //UserRepository.UpdateUserQuestionNumber(userId, 0);
                    ///TODO 
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"К сожалению, эта функция пока не работает :c");
                    //    $"Тебя интересует табак с холодком?");
                    //PrintAnswerOptionsToKeyboard(message.Chat, YesOrNoKeyboard);
                    userRepository.UpdateUserQuestionNumber(userId, 1);
                    break;

                case ButtonSmokeLater:
                    var user = userRepository.GetUserById(message.From.Id);
                    var tobaccos = user.SmokeLater.Select(t => tobaccoRepository.GetItemById(t));
                    if (tobaccos.Count() == 0)
                    {
                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"У тебя нет планов на покур😤. \n\nДобавь что-нибудь😈😈😈");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            message.Chat,
                            $"Ты хотел покурить: ",
                            replyMarkup: new InlineKeyboardMarkup(TelegramMessageSender.GetInlineKeyboard(
                                tobaccos.Select(t => t.ToString()),
                                user.SmokeLater, "tobaccoFromRequest")));
                    }

                    break;

                case ButtonHistory:
                    ///TODO
                    await _botClient.SendTextMessageAsync(
                        message.Chat,
                        $"К сожалению, эта функция пока не работает :c");
                    break;

                default:
                    switch (userRepository.GetUserCondition(userId).GetCondition())
                    {
                        case userCondition.none:
                            break;
                        case userCondition.search:
                        {
                            var resultRequest = tobaccoRepository.SearchTobaccoInDict(message.Text.ToLower());
                            if (resultRequest.Count == 0)
                            {
                                await _botClient.SendTextMessageAsync(
                                    message.Chat,
                                    $"К сожалению, у меня нет табака с таким вкусом :c");
                            }
                            else
                                TelegramMessageSender.PrintTobaccoToKeyboard(message, _botClient, resultRequest);

                            break;
                        }

                        case userCondition.recommendation:
                            break;
                    }

                    break;
            }
        }
    }
}