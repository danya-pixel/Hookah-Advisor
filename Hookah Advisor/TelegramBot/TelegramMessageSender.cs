﻿using Telegram.Bot.Args;
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
    public class TelegramMessageSender
    {
        private const string ButtonSearch = "Поиск";
        private const string ButtonRecommendations = "Рекомендации";
        private const string ButtonSmokeLater = "Покурить позже";
        private const string ButtonHistory = "История";

        public static async void SendStartMessage(Message message, ITelegramBotClient _botClient)
        {
            var chat = message.Chat;
            var userFirstName = message.From.FirstName;

            await _botClient.SendTextMessageAsync(
                chat,
                $"Привет {userFirstName},\n" +
                "Добро пожаловать в бота HookahAdvisor \n" + "\n" +
                "Этот бот помогает найти табак для кальяна под твои предпочтения💨 \n " + "\n" +
                "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст🔞\n" +
                "\n" +
                " «Поиск🔎» помогает найти табак по твоему запросу. \n" + "\n" +
                " «Рекомендации⭐️» подсказывают табак на основании опроса. \n" + "\n" +
                " «Покурить позже🌫» хранит все сохранённые тобой табаки. \n" + "\n" +
                " Жми на нужную тебе кнопку снизу!👇", replyMarkup: GetButtons());
        }

        public static async void SendHelpMessage(Message message, ITelegramBotClient _botClient)
        {
            var chat = message.Chat;

            await _botClient.SendTextMessageAsync(
                chat,
                $"Этот бот помогает найти табак для кальяна под твои предпочтения💨\n" + "\n" +
                "Курение вредит Вашему здоровью! Используя этот бот, вы подтверждаете свой совершеннолетний возраст🔞\n" +
                "\n" +
                "«Поиск🔎» помогает найти табак по твоему запросу.\n" + "\n" +
                "«Рекомендации⭐️» подсказывают табак на основании опроса.\n" + "\n" +
                "«Покурить позже🌫» хранит все сохранённые тобой табаки.\n" + "\n" +
                "Жми на нужную тебе кнопку снизу!👇\n");
        }

        public static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new() {new KeyboardButton {Text = ButtonSearch}, new KeyboardButton {Text = ButtonRecommendations}},
                    new() {new KeyboardButton {Text = ButtonSmokeLater}, new KeyboardButton {Text = ButtonHistory}}
                },
                ResizeKeyboard = true
            };
        }

        public static async void PrintTobaccoToKeyboard(Message message, ITelegramBotClient _botClient,
            List<Tobacco> tobaccos)
        {
            var chat = message.Chat;
            var array = tobaccos.Select(t => t.ToString());
            var idTobaccos = tobaccos.Select(t => t.id);

            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(array, idTobaccos, "tobaccoFromRequest"));
            await _botClient.SendTextMessageAsync(
                message.From.Id,
                "Смотри, что я нашёл:",
                replyMarkup: keyboardMarkup
            );
        }

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(
            IEnumerable<string> stringArray,
            IEnumerable<T> idTobaccos, string type)
        {
            var keyboardInline = stringArray
                .Zip(idTobaccos, (str, idTobacco) => new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                });

            return keyboardInline;
        }

        public static IEnumerable<IEnumerable<InlineKeyboardButton>> GetInlineKeyboard<T>(string str, T idTobacco,
            string type)
        {
            return new[]
            {
                new[]
                {
                    new InlineKeyboardButton
                    {
                        Text = str,
                        CallbackData = $"{type}_{idTobacco}"
                    }
                }
            };
        }
    }
}