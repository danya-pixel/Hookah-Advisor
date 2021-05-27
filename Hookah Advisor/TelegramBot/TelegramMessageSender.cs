using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Hookah_Advisor.TelegramBot
{
    public static class TelegramMessageSender
    {
        public static async void SendWhenNotTextMessage(Message message, ITelegramBotClient botClient)
        {
            await botClient.SendTextMessageAsync(
                message.Chat,
                BotSettings.InvalidMessage);
        }

        public static async void SendStartMessage(Message message, ITelegramBotClient botClient)
        {
            var chat = message.Chat;
            var userFirstName = message.From.FirstName;

            await botClient.SendTextMessageAsync(
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

        public static async void SendHelpMessage(Message message, ITelegramBotClient botClient)
        {
            var chat = message.Chat;

            await botClient.SendTextMessageAsync(
                chat,
                BotSettings.HelpMessage);
        }

        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new()
                    {
                        new KeyboardButton {Text = BotSettings.ButtonSearch},
                        new KeyboardButton {Text = BotSettings.ButtonRecommendations}
                    },
                    new()
                    {
                        new KeyboardButton {Text = BotSettings.ButtonSmokeLater},
                        new KeyboardButton {Text = BotSettings.ButtonHistory}
                    }
                },
                ResizeKeyboard = true
            };
        }

        public static async void PrintTobaccoToKeyboard(Message message, ITelegramBotClient botClient,
            List<Tobacco> tobaccos)
        {
            var array = tobaccos.Select(t => t.ToString());
            var idTobaccos = tobaccos.Select(t => t.Id);

            var keyboardMarkup = new InlineKeyboardMarkup(GetInlineKeyboard(array, idTobaccos, BotSettings.TypeSearchTobacco));
            await botClient.SendTextMessageAsync(
                message.From.Id,
                BotSettings.SearchListMessage,
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