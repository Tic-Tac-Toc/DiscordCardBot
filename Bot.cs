using Discord;
using Discord.Commands;
using DiscordCardBot.Cards_Stuff;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordCardBot
{
    public class Bot
    {      
        public DiscordClient Client;
        public Configuration Config;

        public Poll CurrentPoll;
        public bool IsPolling = false;

        public Bot()
        {
            Config = new Configuration();
            Init();
            LoadConfig();
            SaveConfig();
            Commands.Init(this);
            CardManager.LoadCDB("cards.cdb", false, true);
            Console.WriteLine("[Database] : Cards loaded.");
        }

        private void Init()
        {
            DiscordConfigBuilder ClientConfig = new DiscordConfigBuilder();
            ClientConfig.AppName = "CrowBot";
            ClientConfig.AppUrl = "https://github.com/Tic-Tac-Toc/DiscordCardBot";
            ClientConfig.MessageCacheSize = 0;
            ClientConfig.AppVersion = "1.0";
            ClientConfig.LogLevel = LogSeverity.Info;
            ClientConfig.LogHandler = OnLogMessage;            

            Client = new DiscordClient(ClientConfig).UsingCommands(x => {
                x.PrefixChar = '+';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;
                x.ErrorHandler = OnCommandError;
            });

            SetCommands();
        }
        private void LoadConfig()
        {
            Config = new Configuration();
            Config.BotToken = "Mjg1NzY1MDY5OTE1MDI5NTA0.C5W7Ag.pWddyYYcnhgxMT5D8ZSUysw-QWc";
            Config.ClientID = 285765069915029504;
            Config.OwnerID = 210137207317594123;
            Config.AllowedChannelsId = new List<ulong>();
            Config.AllowedChannelsId.Add(285770178644410368);
            Config.AllowedChannelsId.Add(206090421036646401);
            Config.AllowedChannelsId.Add(206089416589049856);
        }
        private void SaveConfig()
        {
            File.WriteAllText("config.json", JsonConvert.SerializeObject(Config));
        }

        public void Connect()
        {
            Client.ExecuteAndWait(async () =>
            {
                await Client.Connect(Config.BotToken, TokenType.Bot);
            });
        }

        static void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");
        }
        static void OnCommandError(object sender, CommandErrorEventArgs e)
        {
            var msg = e.Exception?.Message;
            if (msg == null)
            {
                switch (e.ErrorType)
                {
                    case CommandErrorType.Exception:
                        msg = "Erreur inconnue.";
                        break;

                    case CommandErrorType.BadPermissions:
                        msg = "Vous n'avez pas la permission pour utiliser cette commande.";
                        break;

                    case CommandErrorType.BadArgCount:
                        msg = "Erreur dans les arguments de votre requête.";
                        break;

                    case CommandErrorType.InvalidInput:
                        msg = "Impossible de découper votre commande. Revoyez sa forme.";
                        break;

                    case CommandErrorType.UnknownCommand:
                        msg = "Commande inconnue.";
                        break;
                }
            }
            if (msg == null)
            {
                return;
            }
            e.Channel.SendMessage(msg);
        }
        void SetCommands()
        {
            Client.GetService<CommandService>().
                   CreateCommand("card").
                   Alias("c", "cd").
                   Description("Obtient des informations sur une carte Yu-Gi-Oh!").
                   Parameter("Nom de la carte", ParameterType.Unparsed).
                   Do(Commands.LookupCard());

            Client.GetService<CommandService>().
                   CreateCommand("inviteurl").
                   Alias("invite", "i").
                   Description($"Donne le lien permettant à {Client.Config.AppName} de rejoindre votre serveur.").
                   Do(Commands.SendInviteUrl());

            Client.GetService<CommandService>().
                   CreateCommand("shutdown").
                   AddCheck((command, user, arg3) => user.Id == Config.OwnerID).
                   Hide().
                   Alias("sd").
                   Description("Ferme la console gérant le bot.").
                   Do(Commands.Shutdown());

            Client.GetService<CommandService>().CreateGroup
                   ("poll", cgb => {
                       cgb.CreateCommand("start").
                           AddCheck((command, user, arg3) => user.Id == Config.OwnerID).
                           Description("Démarre un sondage.").
                           Parameter("Question du sondage", ParameterType.Unparsed).
                           Do(Commands.StartPoll());

                       cgb.CreateCommand("vote").
                           Description("Permet de voter pour le sondage actuel.\n" + "Votes valides: [yes/y, no/n, abstain/idk/a].").
                           Parameter("Vote").
                           Do(Commands.Vote());

                       cgb.CreateCommand("stop").
                           AddCheck((command, user, arg3) => user.Id == Config.OwnerID).
                           Description("Arrête le sondage actuel.").
                           Do(Commands.StopPoll());
                   });
        }


    }
}
