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
using System.Threading;

namespace WorkScheduleBOT
{
    public class MenuUser
    {
    
        public static async void IncomenMessage(TelegramBotClient client, User user, MessageEventArgs e)
        {
           // Thread.Sleep(100);
            DataManager dataManager = new("empty", "empty", Program.pathJSON);
            user.CountRequest++;
            var msg = e.Message;
            if (msg.Text is not null && msg.Text == Exit || msg.Text == "/start")
            {
                try
                {
                user.LastMessage = "empty";
                await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
                }
                catch (Exception Ex)
                {
                    BagReport.SetBagAndSave(Ex.Message, Program.pathBagJSON);
                }
                }else if(msg.Text == ExitProfMenu) 
                {

                try
                {
                    if (msg.Chat.Id == 1143288883)
                    {
                        await client.SendTextMessageAsync(
                                    msg.Chat.Id,
                                    $"розширене меню адміна",
                                    replyMarkup: ProfMenuAdmin()
                                    );
                    }
                    else { 
                        await client.SendTextMessageAsync(
                                    msg.Chat.Id,
                                    $"розширене меню",
                                    replyMarkup: ProfMenu()
                                    );
                    }
                }
                catch (Exception)
                { }
            }
            try
            {
            if (user.LastMessage is not null && user.LastMessage.Contains("пишем"))
            {

                try
                {
                    
                    await client.SendTextMessageAsync(
                                long.Parse(user.LastMessage.Split(' ')[1]),
                                $"{msg.Text}",
                                replyMarkup: ButtonStart()
                                );
                }
                catch (Exception)
                { }
                user.LastMessage = "empty";
            }

            }
            catch (Exception)
            {
            }
            try
            {

            if (msg.Text == "немає вістежень\nДОДАТИ?") msg.Text = Follow;
            }

            catch (Exception)
            {

            }
            string dateView = "";
            try
            {
                if (user.LastMessage is not null && user.LastMessage.Contains("#"))
                {
                    dateView = user.LastMessage.Split('#')[0] + " " + msg.Text;
                    user.LastMessage = "redirectToView";
                }
            }
            catch (Exception)
            {

            }
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
                         
                            try
                            {
                            await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви підписались на оновлення даних в графіку працівника {usInSch.Name}",
                            replyMarkup: ProfMenu()
                            );

                            }
                            catch (Exception)
                            { }
                        }
                        else
                        {
                            try
                            {
                            await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви вже підписані на працівника {usInSch.Name}",
                            replyMarkup: ProfMenu()
                            );

                            }
                            catch (Exception)
                            { }
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
                        try
                        {
                        await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"Ви відписались від оновлення даних в графіку працівника {usInSchUn.Name}",
                            replyMarkup: ProfMenu()
                            );

                        }
                        catch (Exception)
                        { }
                        user.LastMessage = "empty";
                    }
                    
                    break;
                case WriteAll:
                    Program.WriteMessage = msg.Text;

                    Program.WriteAllUsers();
                    

                    user.LastMessage = "empty";
                    break;
                    
                //case "hello all old":
                //    Program.WriteMessage2 = msg.Text;
                //    Program.WriteAllUsers2();
                    
                //    user.LastMessage = "empty";
                //    break;
                    
                case "написати":
                    try
                    {
                        await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   $"пиши {msg.Text}"
                   
                   );
                        user.LastMessage = $"пишем {Program.Users.Find(s => (s.Name + s.Surname) == msg.Text).Id}";
                    }
                    catch (Exception)
                    { }
                    break;

                case ViewUser:
                    try
                    {
                    await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonAllEmployersShift(msg.Text)
                   );

                    }
                    catch (Exception)
                    { }
                    user.LastMessage = ViewUserInShift;
                    break;

                case ViewUserInShift:
                    var us = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                    if (us is not null)
                    {
                        try
                        {

                        await client.SendTextMessageAsync(msg.Chat.Id, us.ViewShifts());
                        }
                        catch (Exception)
                        { }
                    }
                    try
                    {
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );

                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "empty";
                    break;

                case ViewScheduleShift:
                    foreach (var item in Program.ListUserSchedule)
                    {
                        if (item.Shift.ToString() == msg.Text)
                        {
                            try
                            {
                            await client.SendTextMessageAsync(msg.Chat.Id, item.ViewShifts());

                            }
                            catch (Exception)
                            { }
                        }
                    }
                    try
                    {
                        try
                        {
                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );

                        }
                        catch (Exception)
                        { }
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "empty";
                    break;

                    case ViewOwnSchedule:
                   
                        var usw = Program.ListUserSchedule.Find(us => us.Name == msg.Text);
                        if (usw is not null)
                        {
                        try
                        {
                        await client.SendTextMessageAsync(msg.Chat.Id, usw.ViewShifts());

                        }
                        catch (Exception)
                        { }
                    }
                    
                    try
                    {

                    await client.SendTextMessageAsync(
                            msg.Chat.Id,
                            $"основне меню",
                            replyMarkup: ButtonStart()
                            );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "empty";
                    break;
                case WhoFeaters:


                    try
                    {
                       
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                $"Виберіть тип",
                                replyMarkup: WhoFeaturesClass.GetMarkupNameDay()
                                );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = $"{msg.Text}#";
                    break;
                case "redirectToView":


                    try
                    {
                        await client.SendTextMessageAsync(msg.Chat.Id, WhoFeaturesClass.FindUserInDate(dateView));
                        await client.SendTextMessageAsync(
                                msg.Chat.Id,
                                $"основне меню",
                                replyMarkup: ButtonStart()
                                );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "empty";
                    break;
                default:
                    break;
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////// input massege
            switch (msg.Text)
            {
                case "bot?":
                    try
                    {

                   
                    await client.SendTextMessageAsync(msg.Chat.Id,
                            $"users - view list users\n" +
                            $"hello all - write message to all users\n" +
                            $"hello all old - write message to all users\n"
                            );
                    }
                    catch (Exception)
                    { }
                    break;
                case "users":
                    int num = 1;
                    long countRequest = 0;
                    foreach (var item in Program.Users)
                    {
                        countRequest += item.CountRequest;
                        try
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, $"\n {num++} {item.Name} {item.Surname} [Request = {item.CountRequest}]");
                        }
                        catch (Exception)
                        { }
                    }
                    await client.SendTextMessageAsync(msg.Chat.Id, $"[All Request = {countRequest}]");
                    break;
                case ViewOwnSchedule:
                    try
                    {

                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: user.ListFollowersMainMenu()
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = ViewOwnSchedule;
                    break;
                case ViewScheduleShift:
                    try
                    {

                        await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonChoiceShift()
                   );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = ViewScheduleShift;
                    break;
                case ProMenu:
                    try
                    {
                      

                        if (msg.Chat.Id == 1143288883)
                        {
                            await client.SendTextMessageAsync(
                                        msg.Chat.Id,
                                        $"розширене меню адміна",
                                        replyMarkup: ProfMenuAdmin()
                                        );
                        }
                        else
                        {
                            await client.SendTextMessageAsync(
                         msg.Chat.Id,
                        ProMenu,
                        replyMarkup: ProfMenu()
                        );
                        }
                    }
                    catch (Exception)
                    { }
                    break;
                case ViewUser:
                    try
                    {
                        await client.SendTextMessageAsync(
                   msg.Chat.Id,
                   "виберіть зміну",
                   replyMarkup: ButtonChoiceShift()
                   );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = ViewUser;
                    break;

                case Follow:
                    try
                    {
                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: AllEmployers()
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = Follow;
                    break;
                case Unfollow:
                    try
                    {
                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    ChoiceEmploye,
                    replyMarkup: user.ListFollowers()
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = Unfollow;
                    break;
                case WriteAll:
                    try
                    {
                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    $"напишіть повідомлення для всіх"
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = WriteAll;
                    break;

                case "hello all old":
                    try
                    {
                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    $"напишіть повідомлення для всіх old"
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "hello all old";
                    break;
                case "написати":
                    try
                    {
                        await client.SendTextMessageAsync(
                    msg.Chat.Id,
                    $"виберіть кому написати",
                     replyMarkup: WriteConcretteUser()
                    );
                    }
                    catch (Exception)
                    { }
                    user.LastMessage = "написати";
                    break;

                case WhoFeaters: /////////////////////////////// start WhoFeature
                    //try
                    //{
                    //    await client.SendTextMessageAsync(
                    //    msg.Chat.Id, "Виберіть пошук", replyMarkup: Who());
                    //}
                    //catch (Exception) { }
                    //user.LastMessage = "empty";
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, "Виберіть пошук", replyMarkup: WhoFeaturesClass.GetAvailableListData());
                    }
                    catch (Exception) { }
                    user.LastMessage = WhoFeaters;
                    break;
                case WhoYesterdayDay:
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, WhoFeaturesClass.WhoYesterday(msg.Text));
                    }
                    catch (Exception) { }
                    user.LastMessage = "empty";
                    break;
                    
                case WhoYesterdayNight:
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, WhoFeaturesClass.WhoYesterday(msg.Text));
                    }
                    catch (Exception) { }
                    user.LastMessage = "empty";
                    break;

                case WhoTodayDay:
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, WhoFeaturesClass.WhoToday(msg.Text));                      
                    }
                    catch (Exception){ }
                    user.LastMessage = "empty";
                    break;

                case WhoTodayNight:
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, WhoFeaturesClass.WhoToday(msg.Text));
                    }
                    catch (Exception) { }
                    user.LastMessage = "empty";
                    break;


                case WhoTomorrowDay:
                    try
                    {
                    await client.SendTextMessageAsync(
                    msg.Chat.Id, WhoFeaturesClass.WhoTomorrow(msg.Text));
                    }
                    catch (Exception){ }
                    user.LastMessage = "empty";
                    break;

                case WhoTomorrowNight:
                    try
                    {
                        await client.SendTextMessageAsync(
                        msg.Chat.Id, WhoFeaturesClass.WhoTomorrow(msg.Text));
                    }
                    catch (Exception) { }
                    user.LastMessage = "empty";
                    break;
                case BagView:
                    try
                    {
                        foreach (var item in BagReport.GetBagAll())
                        {

                        await client.SendTextMessageAsync(
                        msg.Chat.Id, item);
                        }
                    }
                    catch (Exception) { }
                    user.LastMessage = "empty";
                    break;

                default:
                    //try
                    //{
                    //    await client.SendTextMessageAsync(
                    //        msg.Chat.Id,
                    //        $"основне меню",
                    //        replyMarkup: ButtonStart()
                    //        );
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                      
                    //}
                    break;
            }
        }
        
            public const string WhoFeaters = "пошук по даті";
            public const string BagView = "bag view";
            public const string WhoYesterdayDay = "хто був вчора день?";
            public const string WhoYesterdayNight = "хто був вчора ніч?";
            public const string WhoTomorrowDay = "хто буде завтра день?";
            public const string WhoTomorrowNight = "хто буде завтра ніч?";
            public const string WhoTodayDay = "хто є сьогодні день ?";
            public const string WhoTodayNight = "хто є сьогодні ніч?";
            public const string Follow = "підписатись на графік працівника";
            public const string Unfollow = "відписатись від графіка працівника";          
            public const string WriteAll  = "hello all";
            public const string ViewUserInShift  = "viewUserShift";
            public const string ChoiceEmploye  = "виберіть працівника";
            public const string ExitProfMenu  = ".вийти";
            public const string Exit  = "вийти";

        public static IReplyMarkup Who()
        {

            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = Exit} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoYesterdayDay } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoYesterdayNight } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoTodayDay } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoTodayNight } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoTomorrowDay } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoTomorrowNight } }

                }

            };

        }
        public static IReplyMarkup WriteConcretteUser()
        {
            List<List<KeyboardButton>> l = new();
            List<KeyboardButton> button = new();
            button.Add(new KeyboardButton { Text = Exit });
            l.Add(button);

            if (Program.ListUserSchedule is not null)
                foreach (var item in Program.Users)
                {
                    
                        List<KeyboardButton> buttons = new();
                        buttons.Add(new KeyboardButton { Text = item.Name + item.Surname});
                        l.Add(buttons);
                    
                }

            return new ReplyKeyboardMarkup
            {
                Keyboard = l
            };

        }
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
        public static IReplyMarkup ProfMenuAdmin()
        {

            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = Exit} },
                    new List<KeyboardButton>{ new KeyboardButton {Text = Follow } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = Unfollow } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "users" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = "написати" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = WriteAll } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = BagView } }

                }

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

                    new List<KeyboardButton>{ new KeyboardButton {Text = WhoFeaters } },
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
