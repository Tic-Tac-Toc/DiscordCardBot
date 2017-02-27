using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordCardBot.Enums
{
    public enum CardType
    {
        Monstre = 1,
        Continue = 0x20000,
        Contre = 0x100000,
        Effet = 0x20,
        Equipement = 0x40000,
        Terrain = 0x80000,
        Flip = 0x200000,
        Fusion = 0x40,
        Normal = 0x10,
        Jeu_Rapide = 0x10000,
        Rituelle = 0x80,
        Magie = 2,
        Esprit = 0x200,
        Synchro = 0x2000,
        Jeton = 0x4000,
        Toon = 0x400000,
        Piège = 4,
        Monstre_Piège = 0x100,
        Syntoniseur = 0x1000,
        Union = 0x400,
        Xyz = 0x800000,
        Gemini = 0x800,
        Pendule = 0x1000000,
        SpecialSummon = 0x2000000
    }
}
