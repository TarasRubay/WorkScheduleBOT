using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace WorkScheduleBOT
{
    public static class Menu
    {
       
        public static async void ViewShift(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule,List<string> WorkScheduleShift)
        {
                var msg = e.Message;
            try
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "Last up date: " + LastUpDateWorkShedule);
                if (WorkScheduleShift is not null)
                    foreach (var item in WorkScheduleShift)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, item);
                    }
                await client.SendTextMessageAsync(
                msg.Chat.Id,
                $"виберіть зміну",
                replyMarkup: WeeksButtons()
                );
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }
        }
        private static IReplyMarkup WeeksButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton>{ new KeyboardButton {Text = Program.LastUpDateWorkShedulePenunlimate } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = Program.LastUpDateWorkSheduleLast } }
                }
            };
        }
    }
}
