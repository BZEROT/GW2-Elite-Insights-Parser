﻿using System;
using System.Collections.Generic;
using System.Linq;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.Builders.HtmlModels
{
    public class TargetDto
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public long Health { get; set; }
        public int CombatReplayID { get; set; }
        public long HbWidth { get; set; }
        public long HbHeight { get; set; }
        public uint Tough { get; set; }
        public List<MinionDto> Minions { get; } = new List<MinionDto>();
        public double Percent { get; set; }
        public double HpLeft { get; set; }
        public ActorDetailsDto Details { get; set; }

        public TargetDto(Target target, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Name = target.Character;
            Icon = GeneralHelper.GetNPCIcon(target.ID);
            Health = target.GetHealth(log.CombatData);
            HbHeight = target.HitboxHeight;
            HbWidth = target.HitboxWidth;
            Tough = target.Toughness;
            Details = details;
            if (cr)
            {
                CombatReplayID = target.GetCombatReplayID(log);
            }
            if (log.FightData.Success)
            {
                HpLeft = 0;
            }
            else
            {
                List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(target.AgentItem);
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            Percent = Math.Round(100.0 - HpLeft, 2);
            foreach (KeyValuePair<string, MinionsList> pair in target.GetMinions(log))
            {
                Minions.Add(new MinionDto() { Id = pair.Value.MinionID, Name = pair.Key.TrimEnd(" \0".ToArray()) });
            }
        }
    }
}
