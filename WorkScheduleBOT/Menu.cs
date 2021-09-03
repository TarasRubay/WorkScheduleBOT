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
        public static async void ViewSchedule(TelegramBotClient client, MessageEventArgs e,string LastUpDateWorkShedule)
        {
            try
            {
                await client.SendTextMessageAsync(
                e.Message.Chat.Id,
                "Last up date: " + LastUpDateWorkShedule,
                replyMarkup: Shifts()
                );

            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(e.Message.Chat.Id, ex.ToString());
            }
        }
        public static async void ShiftOne(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule,List<string> WorkScheduleShift1)
        {
                var msg = e.Message;
            try
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "Last up date: " + LastUpDateWorkShedule);
                if (WorkScheduleShift1 is not null)
                    foreach (var item in WorkScheduleShift1)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, item);
                    }
                await client.SendTextMessageAsync(
                msg.Chat.Id,
                $"зміна 1",
                replyMarkup: Shifts()
                );
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }
        }
        public static async void ShiftTwo(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule, List<string> WorkScheduleShift2)
        {
            var msg = e.Message;
            try
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "Last up date: " + LastUpDateWorkShedule);
                if (WorkScheduleShift2 is not null)
                    foreach (var item in WorkScheduleShift2)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, item);
                    }
                await client.SendTextMessageAsync(
                msg.Chat.Id,
                $"зміна 2",
                replyMarkup: Shifts()
                );
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }

        }
        public static async void ShiftThree(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule, List<string> WorkScheduleShift3)
        {
            var msg = e.Message;
            try
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "Last up date: " + LastUpDateWorkShedule);
                if (WorkScheduleShift3 is not null)
                    foreach (var item in WorkScheduleShift3)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, item);
                    }
                await client.SendTextMessageAsync(
                msg.Chat.Id,
                $"зміна 3",
                replyMarkup: Shifts()
                );
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }
        }
        public static async void ShiftFour(TelegramBotClient client, MessageEventArgs e, string LastUpDateWorkShedule, List<string> WorkScheduleShift4)
        {
            var msg = e.Message;
            try
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "Last up date: " + LastUpDateWorkShedule);
                if (WorkScheduleShift4 is not null)
                    foreach (var item in WorkScheduleShift4)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, item);
                    }
                await client.SendTextMessageAsync(
                msg.Chat.Id,
                $"зміна 4",
                replyMarkup: Shifts()
                );
            }
            catch (Exception ex)
            {
                await client.SendTextMessageAsync(msg.Chat.Id, ex.ToString());
            }
        }
    
    private static IReplyMarkup Shifts()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 1" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 2" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 3" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "зміна 4" } }

                }

            };
        }
        private static IReplyMarkup GetButtonsMenu()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "переглянути розклад" } }

                }

            };
        }
    }
}
