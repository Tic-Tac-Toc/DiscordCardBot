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
            if (File.Exists("config.json"))
                Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("config.json"));
            else
            {
                Config = new Configuration();
                Config.BotToken = "Mjg1NzY1MDY5OTE1MDI5NTA0.C5W7Ag.pWddyYYcnhgxMT5D8ZSUysw-QWc";
                Config.ClientID = 285765069915029504;
                Config.OwnerID = 210137207317594123;
                Config.AllowedUserId = new List<ulong>();
                Config.AllowedUserId.Add(Config.OwnerID);
            }
            if (!Config.AllowedUserId.Contains(Config.OwnerID))
                Config.AllowedUserId.Add(Config.OwnerID);
        }
        public void SaveConfig()
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
                   CreateCommand("arch").
                   Alias("ac", "sc", "sn").
                   Description("Obtient des informations sur un archétype Yu-Gi-Oh!").
                   Parameter("Nom de l'archétype", ParameterType.Unparsed).
                   Do(Commands.LookupSetname());

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
                   ("permission", cgb => {
                       cgb.CreateCommand("add").
                           AddCheck((command, user, arg3) => Config.AllowedUserId.Contains(user.Id)).
                           Description("Permet de donner la permission d'utiliser des commandes du bot.").
                           Parameter("Id de l'utilisateur", ParameterType.Unparsed).
                           Do(Commands.AddUserAllowed());

                       cgb.CreateCommand("remove").
                           AddCheck((command, user, arg3) => Config.AllowedUserId.Contains(user.Id)).
                           Description("Permet de retirer la permission d'utiliser des commandes du bot.").
                           Parameter("Id de l'utilisateur", ParameterType.Unparsed).
                           Do(Commands.RemoveUserAllowed());
                   });
        }


    }
}
