using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordCardBot
{
    public class Configuration
    {
        #region Public Fields + Properties

        [JsonProperty("botToken")]
        public string BotToken { get; set; }

        [JsonProperty("clientID")]
        public ulong ClientID { get; set; }

        [JsonProperty("ownerID")]
        public ulong OwnerID { get; set; }

        [JsonProperty("allowedUserId")]
        public List<ulong> AllowedUserId { get; set; }

        #endregion Public Fields + Properties
    }
}
