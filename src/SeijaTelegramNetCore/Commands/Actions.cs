using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeijaTelegram.Main;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SeijaTelegram.Commands
{
    class Actions
    {
        public static void Hug(Message msg, string[] args)
        {
            if (msg.From.Id == Brain.settings.masterId)
            {
                Brain.SendMessage("/(✿◕⁀◕)\\", msg.Chat.Id).Wait();
            }
            else {
                Brain.SendMessage(string.Format("( ° ͜ʖ͡°)╭∩╮ Hey {0}, here's a \"hug\" for you. ( ° ͜ʖ͡°)╭∩╮", msg.From.FirstName), msg.Chat.Id).Wait();
            }
        }

        async public static void AngelHalo(Message msg, string[] args)
        {
            const string GBF_TWITTER_ID = "1549889018";
            string result = "";
            try {
                var tweets = await Brain.twitterConnector.LoadTweets(GBF_TWITTER_ID);
                var parser = new GranblueParser(tweets);
                var info = parser.GetHaloInfo();
                var argList = new List<string>(args);
                var groupArg = argList.Find(str => str.StartsWith("-g="));
                if (groupArg != null)
                {
                    var group = groupArg[3];
                    GroupInfo groupInfo = info.Groups.First(g => new List<string>(g.GroupNames).Contains(group.ToString()));
                    if (groupInfo == null)
                        result = string.Format("Cant find group with key {0}", group);
                    else
                    {
                        var newGroups = new List<GroupInfo>();
                        newGroups.Add(groupInfo);
                        info.Groups = newGroups.ToArray();
                        result = info.ToString();
                    }
                }
                else {
                    result = info.ToString();
                }
            } catch (Exception e)
            {
                result = e.Message;
            } finally
            {
                Brain.SendMessage(result, msg.Chat.Id).Wait();
            }
        }
        async public static void DefenceOrder(Message msg, string[] args)
        {
            const string GBF_TWITTER_ID = "1549889018";
            string result = "";
            try
            {
                var tweets = await Brain.twitterConnector.LoadTweets(GBF_TWITTER_ID);
                var parser = new GranblueParser(tweets);
                var info = parser.GetDOInfo();
                result = info.ToString();
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            finally
            {
                Brain.SendMessage(result, msg.Chat.Id).Wait();
            }
        }

       
    }
}
