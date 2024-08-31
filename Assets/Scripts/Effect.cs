using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace cardMaker
{
    public class effect
    {
        public string Name;
        public string[] Source;
        public SkeletonCard card;
        public List<string> Instruntions;
        public List<string> Local_param;
        public List<string> Targets;
        public Dictionary<string, string> Params;
        readonly syntax process = new();
        int InstructionActual = 0;
        //Parametros locales
        Dictionary<string, cardSupremus> Local_Param_List = new();
        Dictionary<string, cardSupremus> Local_Param_Cards = new();
        Dictionary<string, cardSupremus> Local_Param_Property = new();

        public effect(string Definicion, string source,SkeletonCard card)
        {
            string[] parts = Definicion.Split('&');
            Name = process.destroySpace(parts[0]);
            Local_param = parts[1].Split('^').ToList();
            Instruntions = parts[2].Split('-').ToList();
            Source = source.Split('.');
            this.card = card;
        }

        public void Action(Context context)
        {
            foreach (string instrucion in Instruntions)
            {
                if(instrucion != "")
                {
                    string[] parts = instrucion.Split('|');
                    if(parts.Length == 3) DeterminateList(parts[1], context, parts[0], parts[2]);
                    else DeterminateList(parts[1], context, parts[0]);
                    InstructionActual++;
                }
            }
        }
        public List<cardSupremus> EjecutarEfecto(List<cardSupremus> list,string instruction, string var = " ")
        {
            cardSupremus card;
            switch (instruction)
            {
                case "Pop":
                    card = list[0];
                    Local_Param_Cards[var] = card;
                    list.RemoveAt(0);
                    return list;
                case "Add":
                    if(Local_Param_Cards.ContainsKey(var))
                    {
                        list.Add(Local_Param_Cards[var]);
                        Local_Param_Cards.Remove(var);
                    }
                    return list;
                case "Shufle":
                    for (int i = 0; i < list.Count; i++)
                    {
                        int d = UnityEngine.Random.Range(0, list.Count);
                        card = list[i];
                        list[i] = list[d];
                        list[d] = card;
                    }
                    return list;
                case "Find":
                    string[] parts;
                    if (Source[2].Contains("=="))
                    {
                        parts = Source[2].Split("==");
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (process.destroySpace(parts[0]) == "Type" && list[i].Type.ToString() == process.destroySpace(parts[1]))
                            {
                                Local_Param_Cards[var] = list[i];
                                break;
                            }
                            if (process.destroySpace(parts[0]) == "Faction" && list[i].Faction == process.destroySpace(parts[1]))
                            {
                                Local_Param_Cards[var] = list[i];
                                break;
                            }
                            if (process.destroySpace(parts[0]) == "Power" && list[i].Power.ToString() == process.destroySpace(parts[1]))
                            {
                                Local_Param_Cards[var] = list[i];
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < list.Count; i++)
                    {
                        if(Source[2].Contains("Power"))
                        {
                            string[] veri = Source[2].Split("Power");
                            veri[0] = list[i].Power.ToString();
                            if (process.Compare(veri[0] + veri[1], 0))
                            {
                                Local_Param_Cards[var] = list[i];
                                break;
                            }
                        }
                    }
                    
                    
                    return list;
                case "Push":
                    if (Local_Param_Cards.ContainsKey(var))
                    {
                        list[0] = Local_Param_Cards[var];
                        Local_Param_Cards.Remove(var);
                    }
                    return list;
                case "SendBottom":
                    if (Local_Param_Cards.ContainsKey(var))
                    {
                        list[list.Count-1] = Local_Param_Cards[var];
                        Local_Param_Cards.Remove(var);
                    }
                    return list;
                case "Remove":
                    if (Local_Param_Cards.ContainsKey(var))
                    {
                        list.Remove(Local_Param_Cards[var]);
                        Local_Param_Cards.Remove(var);
                    }
                    return list;
                case "TargetPowerSum":
                    for(int i = 0; i< list.Count;i++)
                    {
                        if(list[i] != null)
                        {
                            list[i].Power++;
                        }
                    }
                    return list;
                case "TargetPowerRest":
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            list[i].Power--;
                            UnityEngine.Debug.Log(list[i].Power);
                        }
                    }
                    return list;
            }
            return null;
        }
        public void DeterminateList(string lista,Context context,string instruction,string var = " ")
        {
            string list = process.validateContext(lista);
            if (list != " ")
            {
                if (list == "Hand" || (list == "Source" && Source[0].Contains("hand")))
                {
                    if(card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "hand"))
                    {
                        context.HandOfPlayer_1 = EjecutarEfecto(context.HandOfPlayer_1,instruction,var);
                        context.GameManager.deck1.Hand = CopyList(context.HandOfPlayer_1, context.GameManager.deck1.Hand);
                        for (int i = 0; i < context.GameManager.deck1.Hand.Length; i++)
                        {
                            if(context.GameManager.deck1.Hand[i] != null)
                            {
                                context.GameManager.deck1.HandPosition(i);
                            }
                        }
                    }
                    else if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "otherhand"))
                    {
                        context.HandOfPlayer_2 = EjecutarEfecto(context.HandOfPlayer_2, instruction, var);
                        context.GameManager.deck1.Hand = CopyList(context.HandOfPlayer_2, context.GameManager.deck2.Hand);
                        for (int i = 0; i < context.GameManager.deck2.Hand.Length; i++)
                        {
                            if (context.GameManager.deck2.Hand[i] != null)
                            {
                                context.GameManager.deck2.HandPosition(i);
                            }
                        }
                    }
                    else if(card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "hand"))
                    {
                        context.HandOfPlayer_2 = EjecutarEfecto(context.HandOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Hand = CopyList(context.HandOfPlayer_2, context.GameManager.deck2.Hand);
                        for (int i = 0; i < context.GameManager.deck2.Hand.Length; i++)
                        {
                            if (context.GameManager.deck2.Hand[i] != null)
                            {
                                context.GameManager.deck2.HandPosition(i);
                            }

                        }
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "otherhand"))
                    {
                        context.HandOfPlayer_1 = EjecutarEfecto(context.HandOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Hand = CopyList(context.HandOfPlayer_1, context.GameManager.deck1.Hand);
                        for (int i = 0; i < context.GameManager.deck1.Hand.Length; i++)
                        {
                            if (context.GameManager.deck1.Hand[i] != null)
                            {
                                context.GameManager.deck1.HandPosition(i);
                            }

                        }
                    }
                }                    
                if (list == "Deck" || (list == "Source" && Source[0].Contains("deck")))
                {
                    if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "deck"))
                    {
                        context.DeckOfPlayer_1 = EjecutarEfecto(context.DeckOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Mazo = CopyList(context.DeckOfPlayer_1, context.GameManager.deck1.Mazo);
                    }
                    else if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "otherdeck"))
                    {
                        context.DeckOfPlayer_2 = EjecutarEfecto(context.DeckOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Mazo = CopyList(context.DeckOfPlayer_2, context.GameManager.deck2.Mazo);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "deck"))
                    {
                        context.DeckOfPlayer_2 = EjecutarEfecto(context.DeckOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Mazo = CopyList(context.DeckOfPlayer_2, context.GameManager.deck1.Mazo);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "deck"))
                    {
                        context.DeckOfPlayer_1 = EjecutarEfecto(context.DeckOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Mazo = CopyList(context.DeckOfPlayer_1, context.GameManager.deck1.Mazo);
                    }
                }
                if (list == "Graveyard" || (list == "Source" && Source[0].Contains("graveyard")))
                {
                    if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "graveyard"))
                    {
                        context.GraveyardOfPlayer_1 = EjecutarEfecto(context.GraveyardOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Graveyard = CopyList(context.GraveyardOfPlayer_1, context.GameManager.deck1.Graveyard);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "othergraveyard"))
                    {
                        context.GraveyardOfPlayer_1 = EjecutarEfecto(context.GraveyardOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Graveyard = CopyList(context.GraveyardOfPlayer_1, context.GameManager.deck1.Graveyard);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "graveyard"))
                    {
                        context.GraveyardOfPlayer_2 = EjecutarEfecto(context.GraveyardOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Graveyard = CopyList(context.GraveyardOfPlayer_2, context.GameManager.deck2.Graveyard);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "othergraveyard"))
                    {
                        context.GraveyardOfPlayer_1 = EjecutarEfecto(context.GraveyardOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Graveyard = CopyList(context.GraveyardOfPlayer_1, context.GameManager.deck2.Graveyard);
                    }
                }
                if (list == "Field" || (list == "Source" && Source[0].Contains("field")))
                {
                    if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "field"))
                    {            
                        context.FieldOfPlayer_1 = EjecutarEfecto(context.FieldOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Field = CopyList(context.FieldOfPlayer_1,context.GameManager.deck1.Field);
                    }
                    else if (card.TriggerPlayer == 1 || (list == "Source" && Source[0] == "otherfield"))
                    {
                        context.FieldOfPlayer_2 = EjecutarEfecto(context.FieldOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Field = CopyList(context.FieldOfPlayer_2, context.GameManager.deck2.Field);
                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "field"))
                    {
                        context.FieldOfPlayer_2 = EjecutarEfecto(context.FieldOfPlayer_2, instruction, var);
                        context.GameManager.deck2.Field = CopyList(context.FieldOfPlayer_2, context.GameManager.deck2.Field);

                    }
                    else if (card.TriggerPlayer == 2 || (list == "Source" && Source[0] == "otherfield"))
                    {
                        context.FieldOfPlayer_1 = EjecutarEfecto(context.FieldOfPlayer_1, instruction, var);
                        context.GameManager.deck1.Field = CopyList(context.FieldOfPlayer_1, context.GameManager.deck1.Field);

                    }
                }
                if (list == "Board" || (list == "Source" && Source[0] == "board"))
                {
                    context.Board = EjecutarEfecto(context.Board, instruction, var);
                    context.GameManager.Board = CopyList(context.Board, context.GameManager.Board);
                }

            }
        }
        public GameObject[] CopyList(List<cardSupremus> cards, GameObject[] Array)
        {
            for(int i = 0;i < Array.Length; i++)
            {
                if(Array[i] != null)
                {
                    if (!cards.Contains(Array[i].GetComponent<cardSupremus>()))
                    {
                        UnityEngine.Object.Destroy(Array[i]);
                        Array[i] = null;
                    }
                }
            }
            for(int i = 0; i < Array.Length; i++)
            {
                Array[i] = null;
            }
            int s = 0;
            foreach(cardSupremus card in cards)
            {
                Array[s] = card.gameObject;
                s++;
            }
            return Array;
        }
    }

}
