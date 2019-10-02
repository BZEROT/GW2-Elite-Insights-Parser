﻿using System.Collections.Generic;
using LuckParser.Parser.ParsedData.CombatEvents;

namespace LuckParser.EIData
{
    public class BuffMapDictionary : Dictionary<long, List<AbstractBuffEvent>>
    {
        // Constructors
        public BuffMapDictionary()
        {
        }
        public BuffMapDictionary(Buff buff)
        {
            this[buff.ID] = new List<AbstractBuffEvent>();
        }

        public BuffMapDictionary(IEnumerable<Buff> buffs)
        {
            foreach (Buff boon in buffs)
            {
                this[boon.ID] = new List<AbstractBuffEvent>();
            }
        }


        public void Add(IEnumerable<Buff> buffs)
        {
            foreach (Buff buff in buffs)
            {
                if (ContainsKey(buff.ID))
                {
                    continue;
                }
                this[buff.ID] = new List<AbstractBuffEvent>();
            }
        }

        public void Add(Buff buff)
        {
            if (ContainsKey(buff.ID))
            {
                return;
            }
            this[buff.ID] = new List<AbstractBuffEvent>();
        }

        private int CompareApplicationType(AbstractBuffEvent x, AbstractBuffEvent y)
        {
            if (x.Time < y.Time)
            {
                return -1;
            }
            else if (x.Time > y.Time)
            {
                return 1;
            }
            else
            {
                return x.CompareTo(y);
            }
        }


        public void Sort()
        {
            foreach (KeyValuePair<long, List<AbstractBuffEvent>> pair in this)
            {
                pair.Value.Sort(CompareApplicationType);
            }
        }

    }

}