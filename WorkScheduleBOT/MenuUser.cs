using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace WorkScheduleBOT
{
    public class MenuUser
    {
    
        public static async void IncomenMessage(TelegramBotClient client, User user, MessageEventArgs e)
        {
            DataManager dataManager = new("empty", "empty", Program.pathJSON);
            user.CountRequest++;
            var msg = e.Message;
            if (msg.Text == Exit || msg.Text == "/start")
            {
                user.LastMessage = "empty";
                await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
            }else if(msg.Text == ExitProfMenu)
            {
                user.LastMessage = "empty";
                await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"розширене меню",
                            replyMarkup: ProfMenu()
                            );
            }
            if (msg.Text == "немає вістежень\nДОДАТИ?") msg.Text = Follow;
            switch (user.LastMessage)
            {
                case Follow:
                    var t = Stopwatch.StartNew();
                    var usInSch = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                    t.Stop();
                    Console.WriteLine($"benchmark {t.ElapsedTicks}");
                    if (usInSch is not null)
                    {
                        if (user.AddFollower(usInSch.Name))
                        {
                            usInSch.NotifyShift += user.MessageFromUser;
                            usInSch.NotifyNewWeek += user.MessageFromUser;
                            dataManager.SaveDataJSON(Program.Users);
                            await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви підписались на оновлення даних в графіку працівника {usInSch.Name}",
                            replyMarkup: ProfMenu()
                            );
                        }
                        else
                        {
                            await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви вже підписані на працівника {usInSch.Name}",
                            replyMarkup: ProfMenu()
                            );
                        }
                        user.LastMessage = "empty";
                    }
                    break;
                case Unfollow:
                    var usInSchUn = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                    if (usInSchUn is not null)
                    {
                        usInSchUn.NotifyShift -= user.MessageFromUser;
                        usInSchUn.NotifyNewWeek -= user.MessageFromUser;
                        user.DeleteFollower(usInSchUn.Name); 
                        dataManager.SaveDataJSON(Program.Users);
                        await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви відписались від оновлення даних в графіку працівника {usInSchUn.Name}",
                            replyMarkup: ProfMenu()
                            );
                        user.LastMessage = "empty";
                    }
                    
                    break;
                case WriteAll:
                    foreach (var item in Program.Users)
                    {
                        await client.SendTextMessageAsync(item.Id, msg.Text);

                    }
                        user.LastMessage = "empty";
                    break;

                case ViewUser:
                    await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonAllEmployersShift(msg.Text)
                   );
                    user.LastMessage = ViewUserInShift;
                    break;

                case ViewUserInShift:
                    var us = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                    if (us is not null)
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, us.ViewShifts());
                    }
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
                    user.LastMessage = "empty";
                    break;

                case ViewScheduleShift:
                    foreach (var item in Program.ListUserSchedule)
                    {
                    if (item.Shift.ToString() == msg.Text) await client.SendTextMessageAsync(msg.Chat.Id, item.ViewShifts());
                    }
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
                    user.LastMessage = "empty";
                    break;

                    case ViewOwnSchedule:
                   
                        var usw = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                        if (usw is not null)
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, usw.ViewShifts());
                        }
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
                    user.LastMessage = "empty";
                    break;
                default:
                    break;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////// input massege
            switch (msg.Text)
            {
                case "bot?":
                        await client.SendTextMessageAsync(msg.Chat.Id,
                            $"users - view list users\n" +
                            $"hello all - write message to all users"
                            );
                    break;
                case "users":
                    int num = 1;
                    long countRequest = 0;
                    foreach (var item in Program.Users)
                    {
                        countRequest += item.CountRequest;
                        await client.SendTextMessageAsync(msg.Chat.Id, $"\n {num++} {item.Name} {item.Surname} [Request = {item.CountRequest}]");
                    }
                    await client.SendTextMessageAsync(msg.Chat.Id, $"[All Request = {countRequest}]");
                    break;
                case ViewOwnSchedule:
                    await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: user.ListFollowersMainMenu()
                    );
                    user.LastMessage = ViewOwnSchedule;
                    break;
                case ViewScheduleShift:
                    await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonChoiceShift()
                   );
                    user.LastMessage = ViewScheduleShift;
                    break;
                case ProMenu:
                    await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ProMenu,
                    replyMarkup: ProfMenu()
                    );
                    break;
                case ViewUser:
                    await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonChoiceShift()
                   );
                    user.LastMessage = ViewUser;
                    break;
                case Follow:
                    await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: AllEmployers()
                    );
                    user.LastMessage = Follow;
                    break;
                case Unfollow:
                    await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: user.ListFollowers()
                    );
                    user.LastMessage = Unfollow;
                    break;
                case WriteAll:
                    await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    $"напишіть повідомлення для всіх"
                    );
                    user.LastMessage = WriteAll;
                    break;
               

                default:
                    break;
            }
        }
        
            public const string Follow = "підписатись на графік працівника";
            public const string Unfollow = "відписатись від графіка працівника";          
            public const string WriteAll  = "hello all";
            public const string ViewUserInShift  = "viewUserShift";
            public const string ChoiceEmploye  = "виберіть працівника";
            public const string ExitProfMenu  = ".вийти";
            public const string Exit  = "вийти";
           
        
        public static IReplyMarkup AllEmployers()
        {
            List<List<KeyboardButton>> l = new();
            List<KeyboardButton> button = new();
            button.Add(new KeyboardButton { Text = ExitProfMenu });
            l.Add(button);

            if (Program.ListUserSchedule is not null)
                foreach (var item in Program.ListUserSchedule)
                {
                    List<KeyboardButton> buttons = new();
                    buttons.Add(new KeyboardButton { Text = item.Name });
                    l.Add(buttons);
                }

            return new ReplyKeyboardMarkup
            {
                Keyboard = l
            };

        }
        public static IReplyMarkup ButtonAllEmployersShift(string shift)
        {
            List<List<KeyboardButton>> l = new();
            List<KeyboardButton> button = new();
            button.Add(new KeyboardButton { Text = Exit });
            l.Add(button);

            if (Program.ListUserSchedule is not null)
                foreach (var item in Program.ListUserSchedule)
                {
                    if (item.Shift.ToString() == shift) {
                        List<KeyboardButton> buttons = new();
                        buttons.Add(new KeyboardButton { Text = item.Name });
                        l.Add(buttons);
                    }
                }

            return new ReplyKeyboardMarkup
            {
                Keyboard = l
            };

        }
        public static IReplyMarkup ProfMenu()
        {

            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = Follow} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = Unfollow } },
                   
                   
                    new List<KeyboardButton>{ new KeyboardButton {Text = Exit } }

                }

            };

        }
        public const string ViewOwnSchedule = "переглянути підписані графіки";
        public const string ViewScheduleShift = "переглянути графік зміни №";
        public const string ViewUser = "переглянути графік працівника";
        public const string ProMenu = "розширене меню";
        public static IReplyMarkup ButtonStart()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = ViewOwnSchedule } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = ViewScheduleShift } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = ViewUser } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = ProMenu } }
                    

                }

            };
        }
        public static IReplyMarkup ButtonChoiceShift()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "1" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "2" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "3" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "4" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = Exit } }

                }

            };
        }

    }
}
