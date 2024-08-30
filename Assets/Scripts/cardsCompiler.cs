using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cardMaker
{
    public class generalCard
    {
        public string Name;
        public CardType Type;
        public string Faction;
        public int Power;
        public List<AttackMode> AttackType;
        public int TriggerPlayer;
        public selectProperty effect;
        public generalCard(string name, CardType type, string faction, int power, List<AttackMode> attackType, int trigerPlayer)
        {
            Name = name;
            Type = type;
            Faction = faction;
            Power = power;
            AttackType = attackType;
            TriggerPlayer = trigerPlayer;
        }
    }
}