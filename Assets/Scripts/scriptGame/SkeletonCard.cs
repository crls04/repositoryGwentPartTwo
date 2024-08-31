using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cardMaker
{
    public class SkeletonCard
    {
        public string Name;
        public CardType Type;
        public string Faction;
        public int Power;
        public List<AttackMode> AttackType;
        public int TriggerPlayer;
        public effect effect;
        public SkeletonCard(string name, CardType type, string faction, int power,List<AttackMode> attackType,int trigerPlayer)
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
