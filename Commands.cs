using Discord.Commands;
using DiscordCardBot.Cards_Stuff;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCardBot
{
    public static class Commands
    {
        public static Bot Bot;

        public static void Init(Bot bot)
        {
            Bot = bot;
        }

        #region Public Methods

        public static Func<CommandEventArgs, Task> AboutUs() => async e =>
        {
            await
                e.Channel.SendMessage("https://github.com/Tic-Tac-Toc/");
        };

        public static Func<CommandEventArgs, Task> LookupCard() => async e =>
        {
            CardInfos returnCard = null;
            var info = e.Args[0];

            if (Tools.IsInt(info))
            {
                if (CardManager.ContainsCard(Convert.ToInt32(info)))
                    returnCard = CardManager.GetCard(Convert.ToInt32(info));
            }
            else
            {
                List<CardInfos> results = CardManager.GetCard(info.ToString());
                if (results.Count > 0)
                {
                    if (results.Count > 1 && results.Count <= 50)
                    {
                        string msg = "**Il y a plusieurs cartes qui correspondent à votre recherche.**";
                        msg += Environment.NewLine + Environment.NewLine + "`";
                        string[] lists = Tools.ArrayCardsToString(",", results);
                        if (lists[0] != "")
                            msg += "Monstres: " + lists[0] + Environment.NewLine + Environment.NewLine;
                        if (lists[1] != "")
                            msg += "Magies: " + lists[1] + Environment.NewLine + Environment.NewLine;
                        if (lists[2] != "")
                            msg += "Pièges: " + lists[2] + Environment.NewLine + Environment.NewLine;
                        msg += "`";
                        await e.Channel.SendMessage(msg);
                        return;
                    }
                    else if (results.Count == 1)
                        returnCard = results[0];
                    else if (results.Count > 50)
                    {
                        await e.Channel.SendMessage($"Il y a plus de 50 cartes qui correspondent à votre recherche, essayez d'affiner votre recherche.");
                        return;
                    }
                }
            }


            if (returnCard?.ToString() == null)
            {
                await
                    e.Channel.SendMessage
                        ($"Je ne trouve aucune information pour la carte: " + info + ".");
            }
            else
            {
                if (File.Exists(Path.Combine(Program.path, "pics", returnCard.Id + ".jpg")))
                    await e.Channel.SendFile(Path.Combine(Program.path, "pics", returnCard.Id + ".jpg"));
                else
                    await e.Channel.SendFile(Path.Combine(Program.path, "pics", "unknown.jpg"));
                await e.Channel.SendMessage(returnCard.ToString());
            }
        };
        public static Func<CommandEventArgs, Task> LookupSetname() => async e =>
        {
            List<CardInfos> returnCards = new List<CardInfos>();
            var info = e.Args[0];

            List<CardInfos> results = CardManager.GetCardBySetname(info.ToString());
            if (results.Count > 0)
            {
                if (results.Count > 1)
                {
                    string msg = "**Il y a plusieurs cartes qui correspondent à votre recherche.**";
                    msg += Environment.NewLine + Environment.NewLine;
                    await e.Channel.SendMessage(msg);
                    msg = "";
                    string[] lists = Tools.ArrayCardsToString(",", results);
                    msg += "Monstres: " + lists[0];
                    await e.Channel.SendMessage("`" + msg + "`");
                    msg = "";
                    msg += "Magies: " + lists[1];
                    await e.Channel.SendMessage("`" + msg + "`");
                    msg = "";
                    msg += "`Pièges: " + lists[2] + Environment.NewLine + Environment.NewLine + "`";
                    msg += Environment.NewLine + Environment.NewLine + "**`Un exemple de carte de l'archétype:`**" + Environment.NewLine;
                    await e.Channel.SendMessage(msg);

                    Random rd = new Random();
                    CardInfos returnCard = results[rd.Next(results.Count)];
                    if (File.Exists(Path.Combine(Program.path, "pics", returnCard.Id + ".jpg")))
                        await e.Channel.SendFile(Path.Combine(Program.path, "pics", returnCard.Id + ".jpg"));
                    else
                        await e.Channel.SendFile(Path.Combine(Program.path, "pics", "unknown.jpg"));
                    await e.Channel.SendMessage(returnCard.ToString());
                    return;
                }
            }
            else
            {
                await
                    e.Channel.SendMessage
                        ($"Je ne trouve aucune information pour l'archétype: " + info + ".");

                await
                    e.User.SendMessage
                        ($"Liste des archétypes que vous pourriez rechercher:");
                await
                    e.User.SendMessage
                        ($"-" + Tools.ArrayToString(Environment.NewLine + "-", CardManager.SetCodesString[info[0]]));


            }
        };

        public static Func<CommandEventArgs, Task> Shutdown() => async e =>
        {
            await e.Channel.SendMessage("Au revoir à tous.");
            await Task.Delay(2500);
            await Bot.Client.Disconnect();
        };

        /*
        #region Poll
        public static Func<CommandEventArgs, Task> StartPoll() => async e => {
            if (Bot.IsPolling)
            {
                await
                    e.Channel.SendMessage
                        ("Je ne peux gérer qu'un sondage à la fois, veuillez clore l'actuel.");
                return;
            }
            Bot.CurrentPoll = new Poll(e.User, e.Args[0], e.Channel.Users.Select(x => x.Id));
            Bot.IsPolling = true;

            await e.Channel.SendMessage($"{e.Server.EveryoneRole.Mention} Sondage démarré par : {Bot.CurrentPoll.Creator}");
            await e.Channel.SendMessage($"Question:\n{Bot.CurrentPoll.Question}");
        };
        public static Func<CommandEventArgs, Task> StopPoll() => async e => {
            if (e.User != Bot.CurrentPoll.Creator)
            {
                await
                    e.Channel.SendMessage
                        ($"Seul le créateur ({Bot.CurrentPoll.Creator.Name}) peut arrêter le sondage.");
            }
            else
            {
                Bot.IsPolling = false;
                Bot.CurrentPoll = null;
                await e.Channel.SendMessage("Sondage terminé! Résultats:\n" +
                          $"Yes: {Bot.CurrentPoll.GetYesCount()}\n" +
                          $"No: {Bot.CurrentPoll.GetNoCount()}\n" +
                          $"Abstain: {Bot.CurrentPoll.GetAbstainCount()}");
            }
        };
        public static Func<CommandEventArgs, Task> Vote() => async e => {
            if (!Bot.IsPolling)
            {
                await
                    e.Channel.SendMessage
                        ("Il n'y a actuellement pas de sondage en cours.");
            }
            if (!Bot.CurrentPoll.UpdateVote(e.User.Id, e.Args[0]))
            {
                await
                    e.Channel.SendMessage
                        ("Vote invalide. Rappel répondez par : yes, no, or abstain!\n" +
                        "Aliases:[yes: y, no: n, abstain: idk, a]");
            }

            if (!Bot.CurrentPoll.IsPollComplete)
            {
                await
                    e.Channel.SendMessage
                        ("Votes:\n" + "Y\tN\tA\t\n" +
                         $"{Bot.CurrentPoll.GetYesCount()}\t" +
                         $"{Bot.CurrentPoll.GetNoCount()}\t" +
                         $"{Bot.CurrentPoll.GetAbstainCount()}");
            }
            else
            {
                await e.Channel.SendMessage("Sondage terminé! Résultats:\n" +
                         $"Yes: {Bot.CurrentPoll.GetYesCount()}\n" +
                         $"No: {Bot.CurrentPoll.GetNoCount()}\n" +
                         $"Abstain: {Bot.CurrentPoll.GetAbstainCount()}");
            }
        };
        #endregion */

        public static Func<CommandEventArgs, Task> SendInviteUrl() => async e =>
        {
            await e.Channel.SendMessage("Cliquez sur " + "https://discordapp.com/oauth2/authorize?client_id=" + Bot.Config.ClientID.ToString() + "&scope=bot&permissions=0 pour que Crow rejoigne votre serveur !");
        };

        public static Func<CommandEventArgs, Task> AddUserAllowed() => async e =>
        {
            ulong id = Convert.ToUInt64(e.Args[0]);
            Bot.Config.AllowedUserId.Add(id);
            Bot.SaveConfig();
            await e.Channel.SendMessage($"Opération sur les permissions effectuées.");
        };
        public static Func<CommandEventArgs, Task> RemoveUserAllowed() => async e =>
        {
            ulong id = Convert.ToUInt64(e.Args[0]);
            Bot.Config.AllowedUserId.Remove(id);
            Bot.SaveConfig();
            await e.Channel.SendMessage($"Opération sur les permissions effectuées.");
        };


        #endregion Public Methods
    }
}