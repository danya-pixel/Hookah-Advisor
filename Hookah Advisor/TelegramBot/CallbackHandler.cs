using System;
using Hookah_Advisor.Repository_Interfaces;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;


namespace Hookah_Advisor.TelegramBot
{
    public static class CallbackHandler
    {
        public static string GetAnswer(CallbackQuery callbackQuery)
        {
            var answer = callbackQuery.Data.Split('_');
            if (answer.Length == 2)
                return null;

            return answer[2];
        }

        public static void BotOnCallbackQueryReceived(IUserRepository userRepository,
            IItemRepository<Tobacco> tobaccoRepository, IOptionRepository<Option> optionRepository,
            CallbackQueryEventArgs callbackQueryEventArgs, ITelegramBotClient botClient)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;
            var callbackType = callbackQuery.Data.Split('_')[0];
            var idObject = Convert.ToInt32(callbackQuery.Data.Split('_')[1]);
            var answer = GetAnswer(callbackQuery);
            var tobaccoSelected = tobaccoRepository.GetItemById(idObject);

            var user = userRepository.GetUserById(callbackQuery.From.Id);

            switch (callbackType)
            {
                case BotSettings.TypeSearchTobacco:
                    var tags = tobaccoSelected.GetTagsFromTobacco();
                    var messageWithTobacco =
                        $"{tobaccoSelected}\n{string.Join(" ", tags)}\n\n{tobaccoSelected.Description}";

                    MessageSender.SendTobacco(messageWithTobacco, tobaccoSelected,
                        user.SmokeLater.Contains(idObject) ? BotSettings.TypeUnSmoke : BotSettings.TypeSmokeLater,
                        callbackQuery, botClient);

                    break;

                case BotSettings.TypeOption:
                {
                    Console.WriteLine(answer);
                    var resultRequest = tobaccoRepository.SearchItemInDict(answer);
                    var a1 = callbackQuery.Message;
                    a1.From.Id = callbackQuery.From.Id;
                    if (resultRequest.Count == 0)
                    {
                        MessageSender.SendText(BotSettings.SearchListEmpty, botClient, callbackQuery.Message);
                    }
                    else

                        MessageSender.PrintTobaccosToKeyboard(callbackQuery.Message, botClient, resultRequest);

                    break;
                }


                case BotSettings.TypeOptionYesNo:
                {
                    answer = answer.ToLower();
                    if (answer == "нет")
                    {
                        var userQuest = user.Condition.QuestionNumber;

                        if (userQuest == 9)
                        {
                            MessageSender.SendText("Не знаю, что тебе еще можно предложить ", botClient,
                                callbackQuery.Message);
                            user.Condition.UserConditionProp = UserCondition.None;
                            user.Condition.QuestionNumber = 0;
                            return;
                        }

                        user.Condition.QuestionNumber = userQuest + 1;
                        callbackQuery.Message.From.Id = callbackQuery.From.Id;
                        Commands.TextReceived(botClient, callbackQuery.Message, userRepository, tobaccoRepository,
                            optionRepository,
                            BotSettings.CommandTypeYesNo);
                    }
                    else
                    {
                        var userQuest = user.Condition.QuestionNumber;

                        if (userQuest != idObject)
                            return;

                        if (userQuest == 0)
                        {
                            var resultRequest = tobaccoRepository.SearchItemInDict(answer);
                            var a1 = callbackQuery.Message;
                            a1.From.Id = callbackQuery.From.Id;
                            if (resultRequest.Count == 0)
                            {
                                MessageSender.SendText(BotSettings.SearchListEmpty, botClient, callbackQuery.Message);
                            }
                            else

                                MessageSender.PrintTobaccosToKeyboard(callbackQuery.Message, botClient, resultRequest);

                            return;
                        }


                        callbackQuery.Message.From.Id = callbackQuery.From.Id;
                        Commands.TextReceived(botClient, callbackQuery.Message, userRepository, tobaccoRepository,
                            optionRepository,
                            BotSettings.CommandTypeTastes);
                    }

                    break;
                }


                case BotSettings.TypeSmokeLater:
                    user.SmokeLater.Add(idObject);

                    MessageSender.SendAnswerCallback(tobaccoSelected, BotSettings.AnswerSmokeLater, botClient,
                        callbackQuery);

                    MessageSender.EditButtonText(BotSettings.KeyboardUnSmokeLater, BotSettings.TypeUnSmoke, botClient,
                        callbackQuery,
                        idObject);
                    break;

                case BotSettings.TypeUnSmoke:
                    user.SmokedHistory.Add(idObject);
                    user.SmokeLater.Remove(idObject);

                    MessageSender.SendAnswerCallback(tobaccoSelected, BotSettings.AnswerUnSmokeLater, botClient,
                        callbackQuery);

                    MessageSender.EditButtonText(BotSettings.KeyboardSmokeLater,
                        BotSettings.TypeSmokeLater, botClient, callbackQuery, idObject);
                    break;
            }
        }
    }
}