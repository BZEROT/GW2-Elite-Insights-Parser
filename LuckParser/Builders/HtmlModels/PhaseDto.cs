﻿using System;
using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Models;
using LuckParser.Parser;

namespace LuckParser.Builders.HtmlModels
{

    public class PhaseDto
    {
        public string Name { get; set; }
        public long Duration { get; set; }
        public double Start { get; set; }
        public double End { get; set; }
        public List<int> Targets { get; set; } = new List<int>();

        public List<List<object>> DpsStats { get; set; }
        public List<List<List<object>>> DpsStatsTargets { get; set; }
        public List<List<List<object>>> DmgStatsTargets { get; set; }
        public List<List<object>> DmgStats { get; set; }
        public List<List<object>> DefStats { get; set; }
        public List<List<object>> SupportStats { get; set; }
        // all
        public List<BoonData> BoonStats { get; set; }
        public List<BoonData> BoonGenSelfStats { get; set; }
        public List<BoonData> BoonGenGroupStats { get; set; }
        public List<BoonData> BoonGenOGroupStats { get; set; }
        public List<BoonData> BoonGenSquadStats { get; set; }

        public List<BoonData> OffBuffStats { get; set; }
        public List<BoonData> OffBuffGenSelfStats { get; set; }
        public List<BoonData> OffBuffGenGroupStats { get; set; }
        public List<BoonData> OffBuffGenOGroupStats { get; set; }
        public List<BoonData> OffBuffGenSquadStats { get; set; }

        public List<BoonData> DefBuffStats { get; set; }
        public List<BoonData> DefBuffGenSelfStats { get; set; }
        public List<BoonData> DefBuffGenGroupStats { get; set; }
        public List<BoonData> DefBuffGenOGroupStats { get; set; }
        public List<BoonData> DefBuffGenSquadStats { get; set; }

        public List<BoonData> PersBuffStats { get; set; }

        // active
        public List<BoonData> BoonActiveStats { get; set; }
        public List<BoonData> BoonGenActiveSelfStats { get; set; }
        public List<BoonData> BoonGenActiveGroupStats { get; set; }
        public List<BoonData> BoonGenActiveOGroupStats { get; set; }
        public List<BoonData> BoonGenActiveSquadStats { get; set; }

        public List<BoonData> OffBuffActiveStats { get; set; }
        public List<BoonData> OffBuffGenActiveSelfStats { get; set; }
        public List<BoonData> OffBuffGenActiveGroupStats { get; set; }
        public List<BoonData> OffBuffGenActiveOGroupStats { get; set; }
        public List<BoonData> OffBuffGenActiveSquadStats { get; set; }

        public List<BoonData> DefBuffActiveStats { get; set; }
        public List<BoonData> DefBuffGenActiveSelfStats { get; set; }
        public List<BoonData> DefBuffGenActiveGroupStats { get; set; }
        public List<BoonData> DefBuffGenActiveOGroupStats { get; set; }
        public List<BoonData> DefBuffGenActiveSquadStats { get; set; }

        public List<BoonData> PersBuffActiveStats { get; set; }

        public List<DamageModData> DmgModifiersCommon { get; set; }
        public List<DamageModData> DmgModifiersItem { get; set; }
        public List<DamageModData> DmgModifiersPers { get; set; }

        public List<List<BoonData>> TargetsCondiStats { get; set; }
        public List<BoonData> TargetsCondiTotals { get; set; }
        public List<BoonData> TargetsBoonTotals { get; set; }

        public List<List<int[]>> MechanicStats { get; set; }
        public List<List<int[]>> EnemyMechanicStats { get; set; }
        public List<long> PlayerActiveTimes { get; set; }

        public List<double> MarkupLines { get; set; }
        public List<AreaLabelDto> MarkupAreas { get; set; }
        public List<int> SubPhases { get; set; }

        public PhaseDto(PhaseData phaseData, List<PhaseData> phases, ParsedLog log)
        {
            Name = phaseData.Name;
            Duration = phaseData.DurationInMS;
            Start = phaseData.Start / 1000.0;
            End = phaseData.End / 1000.0;
            foreach (Target target in phaseData.Targets)
            {
                Targets.Add(log.FightData.Logic.Targets.IndexOf(target));
            }
            PlayerActiveTimes = new List<long>();
            foreach (Player p in log.PlayerList)
            {
                PlayerActiveTimes.Add(phaseData.GetPlayerActiveDuration(p, log));
            }
            // add phase markup
            MarkupLines = new List<double>();
            MarkupAreas = new List<AreaLabelDto>();
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < phaseData.Start || curPhase.End > phaseData.End ||
                    (curPhase.Start == phaseData.Start && curPhase.End == phaseData.End))
                {
                    continue;
                }
                if (SubPhases == null)
                {
                    SubPhases = new List<int>();
                }
                SubPhases.Add(j);
                long start = curPhase.Start - phaseData.Start;
                long end = curPhase.End - phaseData.Start;
                if (curPhase.DrawStart)
                {
                    MarkupLines.Add(start / 1000.0);
                }

                if (curPhase.DrawEnd)
                {
                    MarkupLines.Add(end / 1000.0);
                }

                var phaseArea = new AreaLabelDto
                {
                    Start = start / 1000.0,
                    End = end / 1000.0,
                    Label = curPhase.Name,
                    Highlight = curPhase.DrawArea
                };
                MarkupAreas.Add(phaseArea);
            }
            if (MarkupAreas.Count == 0)
            {
                MarkupAreas = null;
            }

            if (MarkupLines.Count == 0)
            {
                MarkupLines = null;
            }
        }


        // helper methods

        public static List<object> GetDMGStatData(Statistics.FinalStatsAll stats)
        {
            List<object> data = GetDMGTargetStatData(stats);
            data.AddRange(new List<object>
                {
                    // commons
                    stats.TimeWasted, // 9
                    stats.Wasted, // 10

                    stats.TimeSaved, // 11
                    stats.Saved, // 12

                    stats.SwapCount, // 13
                    Math.Round(stats.StackDist, 2) // 14
                });
            return data;
        }

        public static List<object> GetDMGTargetStatData(Statistics.FinalStats stats)
        {
            var data = new List<object>
                {
                    stats.DirectDamageCount, // 0
                    stats.CritableDirectDamageCount, // 1
                    stats.CriticalCount, // 2
                    stats.CriticalDmg, // 3

                    stats.FlankingCount, // 4

                    stats.GlanceCount, // 5

                    stats.Missed,// 6
                    stats.Interrupts, // 7
                    stats.Invulned // 8
                };
            return data;
        }

        public static List<object> GetDPSStatData(Statistics.FinalDPS dpsAll)
        {
            var data = new List<object>
                {
                    dpsAll.Damage,
                    dpsAll.PowerDamage,
                    dpsAll.CondiDamage,
                };
            return data;
        }

        public static List<object> GetSupportStatData(Statistics.FinalSupport support)
        {
            var data = new List<object>()
                {
                    support.CondiCleanse,
                    support.CondiCleanseTime,
                    support.CondiCleanseSelf,
                    support.CondiCleanseTimeSelf,
                    support.BoonStrips,
                    support.BoonStripsTime,
                    support.Resurrects,
                    support.ResurrectTime
                };
            return data;
        }

        public static List<object> GetDefenseStatData(Statistics.FinalDefenses defenses, PhaseData phase)
        {
            var data = new List<object>
                {
                    defenses.DamageTaken,
                    defenses.DamageBarrier,
                    defenses.BlockedCount,
                    defenses.InvulnedCount,
                    defenses.InterruptedCount,
                    defenses.EvadedCount,
                    defenses.DodgeCount
                };

            if (defenses.DownDuration > 0)
            {
                var downDuration = TimeSpan.FromMilliseconds(defenses.DownDuration);
                data.Add(defenses.DownCount);
                data.Add(downDuration.TotalSeconds + " seconds downed, " + Math.Round((downDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1) + "% Downed");
            }
            else
            {
                data.Add(0);
                data.Add("0% downed");
            }

            if (defenses.DeadDuration > 0)
            {
                var deathDuration = TimeSpan.FromMilliseconds(defenses.DeadDuration);
                data.Add(defenses.DeadCount);
                data.Add(deathDuration.TotalSeconds + " seconds dead, " + (100.0 - Math.Round((deathDuration.TotalMilliseconds / phase.DurationInMS) * 100, 1)) + "% Alive");
            }
            else
            {
                data.Add(0);
                data.Add("100% Alive");
            }
            return data;
        }
    }
}
