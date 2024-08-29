using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;


namespace cardMaker
{
    public enum TypeToken
    {
        Error,
        Number,
        String,
        Var
    }
    public enum AttackMode
    {
        Melee,
        Ranged,
        Siege,
        None
    }
    public enum CardType
    {
        Oro,
        Plata,
        Clima,
        Aumento,
        Lider,
        Despeje,
        Senuelo,
        None
    }
    public enum SourceType
    {
        hand,
        otherhand,
        filed,
        otherfield,
        deck,
        otherdeck,
        board

    }

    public class language
    {
        List<CardType> cardType = new();
        List<AttackMode> attackMode = new();
        List<SourceType> source = new();


        //Constructor con parametros predefinidos
        public language()
        {
            cardType.Add(CardType.Oro);
            cardType.Add(CardType.Plata);
            cardType.Add(CardType.Clima);
            cardType.Add(CardType.Aumento);
            cardType.Add(CardType.Senuelo);
            cardType.Add(CardType.Despeje);
            cardType.Add(CardType.Lider);

            attackMode.Add(AttackMode.Melee);
            attackMode.Add(AttackMode.Ranged);
            attackMode.Add(AttackMode.Siege);

            source.Add(SourceType.hand);
            source.Add(SourceType.otherhand);
            source.Add(SourceType.filed);
            source.Add(SourceType.otherfield);
            source.Add(SourceType.deck);
            source.Add(SourceType.otherdeck);
            source.Add(SourceType.board);

        }
        //Se revisa la validez del Token
        public TypeToken verifyValidate (string text)
        {
            if(Regex.IsMatch(text,@"^[\'][a-zA-Z' '0-9]*'"))
            {
                return TypeToken.String;
            }
            else if(Regex.IsMatch(text, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                return TypeToken.Var;
            
            }
            else if (Regex.IsMatch(text, @"^-?\d+$"))
            {
                return TypeToken.Number;
            }
            else
            {
                return TypeToken.Error;
            }
        }

        //Verificar que existe del tipo de carta definidio al inicio del script
        public CardType verifyCard(string type)
        {
            foreach(CardType s in cardType)
            {
                if (s.ToString() == type)
                {
                    return s;
                }
            }
            return CardType.None;
        }

        //Verificar que existe el tipo de ataque definido al inicio del script
        public AttackMode verifyAttack (string type)
        {
            foreach (AttackMode s in attackMode)
            {
                if (s.ToString() == type)
                {
                    return s;
                }
            }
            return AttackMode.None;
        }
        //Verificar que existe el selectorde posiciones (Source) que esta definidio al inicio de este script
        public string verifySource(string type)
        {
            foreach (SourceType s in source)
            {
                if (s.ToString() == type)
                {
                    return s.ToString();
                }
            }
            return " ";
        }
    }


}