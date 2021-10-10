using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

namespace WorkScheduleBOT
{
    [Serializable]
    public class User
    {
        
        public delegate void UserHandler(object sender, string message);
        public event UserHandler Notify;
        public long Id { get; set; }
        public long CountRequest { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string LastMessage { get; set; }
        public string ListFollowersForJson { get; set; } = "";
        
        private List<string> listFollowing = new();

        public List<string> GetListFollower()
        {
            return listFollowing;
        }
        public bool AddFollower(string follower)
        {
            if (listFollowing.Contains(follower))
            {
                return false;
            }
            else
            {
                listFollowing.Add(follower);
                ListFollowersForJson += follower + '#';
                return true;
            }
        }
        public void DeleteFollower(string follower)
        {
            ListFollowersForJson = "";
            listFollowing.Remove(follower);
            foreach (var item in listFollowing)
            {
                ListFollowersForJson += item + '#';
            }
        }
        public  IReplyMarkup ListFollowers()
        {
            if(listFollowing.Count == 0)
            {
                
                return new ReplyKeyboardMarkup
                {
                    Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "немає вістежень\nДОДАТИ?" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = MenuUser.ExitProfMenu } }
                }
                };
            }
            else
            {

            List<List<KeyboardButton>> l = new();
                List<KeyboardButton> button = new();
                button.Add(new KeyboardButton { Text = MenuUser.ExitProfMenu });
                l.Add(button);
            if (Program.ListUserSchedule is not null)
                foreach (var item in listFollowing)
                {
                    List<KeyboardButton> buttons = new();
                    buttons.Add(new KeyboardButton { Text = item });
                    l.Add(buttons);
                }
                return new ReplyKeyboardMarkup
                {
                Keyboard = l
                };
            }

        }
        public IReplyMarkup ListFollowersMainMenu()
        {
            if (listFollowing.Count == 0)
            {

                return new ReplyKeyboardMarkup
                {
                    Keyboard = new List<List<KeyboardButton>>
                {

                    new List<KeyboardButton>{ new KeyboardButton {Text = "немає вістежень\nДОДАТИ?" } },
                    new List<KeyboardButton>{ new KeyboardButton {Text = MenuUser.Exit } }
                }
                };
            }
            else
            {

                List<List<KeyboardButton>> l = new();
                List<KeyboardButton> button = new();
                button.Add(new KeyboardButton { Text = MenuUser.Exit });
                l.Add(button);
                if (Program.ListUserSchedule is not null)
                    foreach (var item in listFollowing)
                    {
                        List<KeyboardButton> buttons = new();
                        buttons.Add(new KeyboardButton { Text = item });
                        l.Add(buttons);
                    }
                return new ReplyKeyboardMarkup
                {
                    Keyboard = l
                };
            }

        }
        public void MessageFromUser(string message)
        {
            
            Notify?.Invoke(this, message);
        }
    }
}
