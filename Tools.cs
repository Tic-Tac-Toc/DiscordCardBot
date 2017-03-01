using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCardBot
{
    public static class Tools
    {
        public static bool IsInt(string Nombre)
        {
            try
            {
                int.Parse(Nombre);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string[] ArrayCardsToString(string separator, IList array)
        {
            string monster = "";
            string spell = "";
            string trap = "";
                
            foreach (var v in array)
            {
                CardInfos card = ((CardInfos)v);
                if (card.GetCardTypes().Contains(Enums.CardType.Monstre))
                    monster += card.Name + separator;
                else if (card.GetCardTypes().Contains(Enums.CardType.Magie))
                    spell += card.Name + separator;
                else if (card.GetCardTypes().Contains(Enums.CardType.Piège))
                    trap += card.Name + separator;

            }

            return new string[] { monster, spell, trap };   
        }

        public static string ArrayToString(string separator, IList array)
        {
            string toReturn = "";

            foreach (var v in array)
                toReturn += v + separator;

            return toReturn;
        }
    }
}
