﻿using System.Collections.Generic;
using LuckParser.EIData;
using LuckParser.Models;

namespace LuckParser.Builders.JsonModels
{
    /// <summary>
    /// Class representing buff on targets
    /// </summary>
    public class JsonTargetBuffs
    {
        /// <summary>
        /// Target buff item
        /// </summary>
        public class JsonTargetBuffsData
        {
            /// <summary>
            /// Uptime of the buff
            /// </summary>
            public double Uptime;
            /// <summary>
            /// Presence of the buff (intensity only)
            /// </summary>
            public double Presence;
            /// <summary>
            /// Buff generated by
            /// </summary>
            public Dictionary<string, double> Generated;
            /// <summary>
            /// Buff overstacked by
            /// </summary>
            public Dictionary<string, double> Overstacked;
            /// <summary>
            /// Buff wasted by
            /// </summary>
            public Dictionary<string, double> Wasted;
            /// <summary>
            /// Buff extended by unknown for
            /// </summary>
            public Dictionary<string, double> UnknownExtended;
            /// <summary>
            /// Buff by extension
            /// </summary>
            public Dictionary<string, double> ByExtension;
            /// <summary>
            /// Buff extended for
            /// </summary>
            public Dictionary<string, double> Extended;


            private static Dictionary<string, double> ConvertKeys(Dictionary<Player, double> toConvert)
            {
                var res = new Dictionary<string, double>();
                foreach (KeyValuePair<Player, double> pair in toConvert)
                {
                    res[pair.Key.Character] = pair.Value;
                }
                return res;
            }

            public JsonTargetBuffsData(Statistics.FinalTargetBuffs stats)
            {
                Uptime = stats.Uptime;
                Presence = stats.Presence;
                Generated = ConvertKeys(stats.Generated);
                Overstacked = ConvertKeys(stats.Overstacked);
                Wasted = ConvertKeys(stats.Wasted);
            }
        }

        /// <summary>
        /// ID of the buff
        /// </summary>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id;
        /// <summary>
        /// Array of buff data \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonTargetBuffsData"/>
        public List<JsonTargetBuffsData> BuffData;
        /// <summary>
        /// Array of int[2] that represents the number of the given buff status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of the buff present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> States;
    }

}
