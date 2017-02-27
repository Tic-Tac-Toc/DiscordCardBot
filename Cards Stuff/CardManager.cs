using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DiscordCardBot.Cards_Stuff.SQLCommands;

namespace DiscordCardBot.Cards_Stuff
{
    public static class CardManager
    {
        private static Dictionary<int, CardInfos> CardData = new Dictionary<int, CardInfos>();
        public static Dictionary<int, string> SetCodes = new Dictionary<int, string>();

        private static void RenameKey<TKey, TValue>(this IDictionary<TKey, TValue> dic,
                              TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }

        public static bool LoadCDB(string dir, bool overwrite, bool clearData = false)
        {
            if (!File.Exists(dir))
                return false;

            if (clearData)
            {
                CardData.Clear();
            }

            SQLiteConnection connection = new SQLiteConnection("Data Source=" + dir);
            List<string[]> datas = new List<string[]>();
            List<string[]> texts = new List<string[]>();

            try
            {
                connection.Open();
                datas = SQLiteCommands.LoadData(connection);
                texts = SQLiteCommands.LoadText(connection);
                connection.Close();
            }
            catch (Exception)
            {
                connection.Close();
                return false;
            }

            foreach (string[] row in datas)
            {
                if (overwrite)
                    CardManager.UpdateOrAddCard(new CardInfos(row));
                else
                {
                    if (!CardManager.ContainsCard(Int32.Parse(row[0])))
                        CardManager.UpdateOrAddCard(new CardInfos(row));
                }
            }
            foreach (string[] row in texts)
            {
                if (CardManager.ContainsCard(Int32.Parse(row[0])))
                    CardManager.GetCard(Int32.Parse(row[0])).SetCardText(row);
            }

            if (File.Exists("setname.txt"))
                LoadSetCodesFromFile(CreateFileStreamFromString(File.ReadAllText("setname.txt")));

            return true;
        }

        public static CardInfos GetCard(int id)
        {
            if (CardData.ContainsKey(id))
                return CardData[id];
            return null;
        }

        public static bool RemoveCard(int id)
        {
            if (CardData.ContainsKey(id))
                return CardData.Remove(id);
            return false;
        }

        public static void UpdateOrAddCard(CardInfos card)
        {
            if (ContainsCard(card.Id))
                CardData[card.Id] = card;
            else
                CardData.Add(card.Id, card);
        }

        public static bool ContainsCard(int id)
        {
            return CardData.ContainsKey(id);
        }

        public static void RenameKey(int fromkey, int tokey)
        {
            CardData.RenameKey(fromkey, tokey);
        }

        public static Dictionary<int, CardInfos>.KeyCollection GetKeys()
        {
            return CardData.Keys;
        }

        public static int Count
        {
            get { return CardData.Count; }
        }

        public static List<CardInfos> GetCard(string name)
        {
            List<CardInfos> cards = new List<CardInfos>();

            foreach (var v in CardData)
                if (v.Value.Name.ToUpper().Contains(name.ToUpper()))
                {
                    if (v.Value.Name.ToUpper() == name.ToUpper())
                    {
                        cards.Clear();
                        cards.Add(v.Value);
                        return cards;
                    }
                    else
                        cards.Add(v.Value);
                }
            return cards;
        }

        private static Stream CreateFileStreamFromString(string file)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(file);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
        private static bool LoadSetCodesFromFile(Stream file)
        {
            try
            {
                var reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    //!setcode 0x8d Ghostrick
                    string line = reader.ReadLine();
                    if (line == null || !line.StartsWith("!setname")) continue;
                    string[] parts = line.Split(' ');
                    if (parts.Length == 1) continue;

                    int setcode = Convert.ToInt32(parts[1], 16);
                    string setname = line.Split(new string[] { parts[1] }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();

                    if (!SetCodes.ContainsKey(setcode))
                        SetCodes.Add(setcode, setname);
                    else
                        SetCodes[setcode] = setname;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}