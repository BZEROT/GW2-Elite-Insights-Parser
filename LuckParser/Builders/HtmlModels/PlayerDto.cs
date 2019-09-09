﻿using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;

namespace LuckParser.Builders.HtmlModels
{

    public class PlayerDto
    {
        public int Group { get; set; }
        public int CombatReplayID { get; set; }
        public string Name { get; set; }
        public string Acc { get; set; }
        public string Profession { get; set; }
        public uint Condi { get; set; }
        public uint Conc { get; set; }
        public uint Heal { get; set; }
        public uint Tough { get; set; }
        public List<MinionDto> Minions { get; } = new List<MinionDto>();
        public List<string> L1Set { get; } = new List<string>();
        public List<string> L2Set { get; } = new List<string>();
        public List<string> A1Set { get; } = new List<string>();
        public List<string> A2Set { get; } = new List<string>();
        public string ColTarget { get; set; }
        public string ColCleave { get; set; }
        public string ColTotal { get; set; }
        public bool IsConjure { get; set; }
        public ActorDetailsDto Details { get; set; }

        public PlayerDto(Player player, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Group = player.Group;
            Name = player.Character;
            Acc = player.Account;
            Profession = player.Prof;
            Condi = player.Condition;
            Conc = player.Concentration;
            Heal = player.Healing;
            Tough = player.Toughness;
            ColTarget = GeneralHelper.GetLink("Color-" + player.Prof);
            ColCleave = GeneralHelper.GetLink("Color-" + player.Prof + "-NonBoss");
            ColTotal = GeneralHelper.GetLink("Color-" + player.Prof + "-Total");
            IsConjure = (player.IsFakeActor);
            BuildWeaponSets(player, log);
            Details = details;
            if (cr && !IsConjure)
            {
                CombatReplayID = player.GetCombatReplayID(log);
            }
            foreach (KeyValuePair<string, MinionsList> pair in player.GetMinions(log))
            {
                Minions.Add(new MinionDto()
                {
                    Id = pair.Value.MinionID,
                    Name = pair.Key.TrimEnd(" \0".ToArray())
                });
            }
        }

        private static void BuildWeaponSets(string[] weps, int offset, List<string> set1, List<string> set2)
        {

            for (int j = 0; j < 4; j++)
            {
                string wep = weps[j + offset];
                if (wep != null)
                {
                    if (wep != "2Hand")
                    {
                        if (j > 1)
                        {
                            set2.Add(wep);
                        }
                        else
                        {
                            set1.Add(wep);
                        }
                    }
                }
                else
                {
                    if (j > 1)
                    {
                        set2.Add("Unknown");
                    }
                    else
                    {
                        set1.Add("Unknown");
                    }
                }
            }
            if (set1[0] == "Unknown" && set1[1] == "Unknown")
            {
                set1.Clear();
            }
            if (set2[0] == "Unknown" && set2[1] == "Unknown")
            {
                set2.Clear();
            }
        }

        private void BuildWeaponSets(Player player, ParsedLog log)
        {
            string[] weps = player.GetWeaponsArray(log);
            BuildWeaponSets(weps, 0, L1Set, L2Set);
            BuildWeaponSets(weps, 4, A1Set, A2Set);
        }
    }
}
