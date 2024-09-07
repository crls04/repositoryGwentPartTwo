using cardMaker;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    string datos;
    public List<GameObject> Cards = new();
    public GameObject LeaderSelect;
    public GameObject Deck1, Deck2;
    public Transform Pos1, Pos2;
    public Deck deck1, deck2;
    public Camera P1, P2;
    public bool invoke = false;
    public int Turn = 1;
    public TextMeshProUGUI P1Round,P2Round,P1Power,P2Power;
    public float Round1,Round2, RoundPower1,RoundPower2;
    public GameObject[] Climas,Climas_Pos = new GameObject[3];
    public bool Jug1_End, Jug2_End = false;
    public bool initial1, initial2 = true;
    public Context context;
    public GameObject[] Board = new GameObject[24];
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        PowerInCamp();
        EndRound();
        EndGame();
        DeterminateContext();
        
    }

    //Funcion de Inicializar juego 
    private void StartGame()
    {
        string[] Players = File.ReadAllText(Application.dataPath + "/Resources/Setting/PlayerSetting.txt").Split('|');

        LoadDeck(Players[0], Deck1, Pos1,deck1,1);
        deck1.Mazo = new GameObject[Cards.Count];
        deck1.Mazo = Cards.ToArray();
        deck1.Mazo = deck1.Barajear(deck1.Mazo);
        deck1.Leader = LeaderSelect;
        deck1.Leader.transform.position = deck1.Field_leader.transform.position;
        LeaderSelect = null;
        Cards.Clear();

        LoadDeck(Players[1], Deck2, Pos2,deck2,2);
        deck2.Mazo = new GameObject[Cards.Count];
        deck2.Mazo = Cards.ToArray();
        Cards.Clear();
        deck2.Mazo = deck2.Barajear(deck2.Mazo);
        deck2.Leader = LeaderSelect;
        deck2.Leader = LeaderSelect;
        deck2.Leader.transform.position = deck2.Field_leader.transform.position;
        deck2.Leader.transform.rotation = deck2.Field_leader.transform.rotation;
        LeaderSelect = null;
    }

    //Funcion para cargar decks
    private void LoadDeck(string decks,GameObject deckPos,Transform positions, Deck player,int TriggerPlayer)
    {
        datos = File.ReadAllText(Application.dataPath + "/Resources/Decks/" + decks + ".txt");
        language process = new();
        string[] cartas = datos.Split('\n');
        foreach (string cart in cartas)
        {
            string[] deck = cart.Split('|');
            GameObject gameObject = new();
            List<AttackMode> attack = new();
            string[] ataques = deck[4].Split('-');
            foreach (string a in ataques)
            {
                if (process.verifyAttack(a) != AttackMode.None)
                {
                    attack.Add(process.verifyAttack(a));
                }
            }
            if (deck[1] != "Oro" && deck[1] != "Plata")
            {
                deck[3] = "0";
            }
            SkeletonCard card = new(deck[0], process.verifyCard(deck[1]), deck[2], int.Parse(deck[3]), attack,TriggerPlayer);
            if(deck.Length == 6 && File.Exists(Application.dataPath + "/Resources/Effects/" + deck[5].Split('*')[0] + ".txt"))
            {
                string effec = File.ReadAllText(Application.dataPath + "/Resources/Effects/" + deck[5].Split('*')[0] + ".txt");
                card.effect = new effect(effec, deck[5].Split('*')[2],card, deck[5].Split('*')[1]);
            }
            gameObject.AddComponent<cardSupremus>();
            gameObject.GetComponent<cardSupremus>().Create(card,player,TriggerPlayer);
            gameObject.transform.parent = deckPos.transform;
            gameObject.transform.position = positions.position;
            if(card.Type == CardType.Lider)
            {
                LeaderSelect = gameObject;
            }
            else
            {
                Cards.Add(gameObject);
            }
        }
    }

    //Funcion para cambiar turno
    public void ChangeTurn()
    {
        bool cam_Change = false;
        if (!invoke && Turn == 1 & !initial1)
        {
            Jug1_End = true;
        }
        if (!invoke && Turn == 2 && !initial2)
        {
            Jug2_End = true;
        }
        TextMeshProUGUI text;
        if(P1.isActiveAndEnabled && !Jug2_End && !initial1)
        {
            P1.gameObject.SetActive(false);
            P2.gameObject.SetActive(true);
            cam_Change = true;
            Turn = 2;
            for(int i=0;i<Climas_Pos.Length;i++)
            {
                Climas_Pos[i].GetComponent<Casilla_Invocacion>().player = 2;
                Climas_Pos[i].GetComponent<Casilla_Invocacion>().Deck = deck2;
            }          
        }
        else if(P2.isActiveAndEnabled && !Jug1_End && !initial2)
        {

            P2.gameObject.SetActive(false);
            P1.gameObject.SetActive(true);
            cam_Change = true;
            Turn = 1;
            for (int i = 0; i < Climas_Pos.Length; i++)
            {
                Climas_Pos[i].GetComponent<Casilla_Invocacion>().player = 1;
                Climas_Pos[i].GetComponent<Casilla_Invocacion>().Deck = deck1;
            }
        }

        if(cam_Change)
        {
            text = P1Power;
            P1Power = P2Power;
            P2Power = text;
            text = P1Round;
            P1Round = P2Round;
            P2Round = text;
        }
        if ((!Jug1_End && !initial1) || (!Jug2_End && !initial2))
        {
            invoke = false;
        }
        
    }

    //Funcion para verificar fin de rondas
    public void EndRound()
    {
        if (Jug1_End && Jug2_End)
        {
            if (RoundPower1 > RoundPower2)
            {
                Round1++;
            }
            else if (RoundPower1 < RoundPower2)
            {
                Round2++;
            }
            else
            {
                Round2++;
                Round1++;
            }
            P1Round.text =  Round1.ToString();
            P2Round.text =  Round2.ToString();
            Jug1_End = false;
            Jug2_End = false;

            for (int i = 0; i < Climas_Pos.Length; i++)
            {
                if (Climas[i] != null)
                {
                    if (Climas[i].GetComponent<cardSupremus>().players == 1) deck1.Graveyard.Add(Climas[i]);
                    if (Climas[i].GetComponent<cardSupremus>().players == 2) deck2.Graveyard.Add(Climas[i]);
                }
                Climas[i] = null;
            }

            for (int i = 0; i < deck1.Field.Length; i++)
            {
                if (deck1.Field[i] != null) deck1.Graveyard.Add(deck1.Field[i]);
                deck1.Field[i] = null;
                if (deck2.Field[i] != null) deck2.Graveyard.Add(deck2.Field[i]);
                deck2.Field[i] = null;
            }

            for (int i = 0; i < deck1.Aum.Length; i++)
            {
                if (deck1.Aum[i] != null) deck1.Graveyard.Add(deck1.Aum[i]);
                deck1.Aum[i] = null;
                if (deck2.Aum[i] != null) deck2.Graveyard.Add(deck2.Aum[i]);
                deck2.Aum[i] = null;
            }
            deck1.Robar(2);
            deck2.Robar(2);
        }
    }

    //Funcion para verificar poder en el campo
    public void PowerInCamp()
    {
        float i = 0;
        float s = 0;
        for (int a = 0; a < 12; a++)
        {
            if (deck1.Field[a] != null)
            {
                i += deck1.Field[a].GetComponent<cardSupremus>().Power;
            }
            if (deck2.Field[a] != null)
            {
                s += deck2.Field[a].GetComponent<cardSupremus>().Power;
            }
        }
        RoundPower1 = i;
        RoundPower2 = s;

        P1Power.text = "Poder: " + RoundPower1;
        P2Power.text = "Poder: " + RoundPower2;
    }

    //Funcion para ver fin del juego
    public void EndGame()
    {
        if(Round1 == 2 && Round2 < 2)
        {
            //Gana jugador 1
        }

        if (Round1 < 2 && Round2 == 2)
        {
            //Gana jugador 2
        }

        if (Round1 == 2 && Round2 == 2)
        {
            //Empate
        }
    }

    
    public void DeterminateContext()
    {
        context.Board.Clear();
        context.HandOfPlayer_1.Clear();
        context.FieldOfPlayer_1.Clear();
        context.GraveyardOfPlayer_1.Clear();
        context.DeckOfPlayer_1.Clear();
        context.HandOfPlayer_2.Clear();
        context.FieldOfPlayer_2.Clear();
        context.GraveyardOfPlayer_2.Clear();
        context.DeckOfPlayer_2.Clear();

        //Cartas en la mano de cada jugador;
        for (int i = 0; i < 10; i++)
        {
            if (deck1.Hand[i] != null)
            {
                context.HandOfPlayer_1.Add(deck1.Hand[i].GetComponent<cardSupremus>());
            }
            if (deck2.Hand[i] != null)
            {
                context.HandOfPlayer_2.Add(deck2.Hand[i].GetComponent<cardSupremus>());
            }
        }

        //Cartas del campo
        for (int i = 0; i < 12; i++)
        {
            if (deck1.Field[i] != null)
            {
                context.Board.Add(deck1.Field[i].GetComponent<cardSupremus>());
            }
            if (deck2.Field[i] != null)
            {
                context.Board.Add(deck2.Field[i].GetComponent<cardSupremus>());
            }
        }

        //Cartas del deck1
        for(int i = 0; i < deck1.Mazo.Length;i++)
        {
            if (deck1.Mazo[i] != null)
            {
                context.DeckOfPlayer_1.Add(deck1.Mazo[i].GetComponent<cardSupremus>());
            }
        }

        //Cartas del deck2
        for (int i = 0; i < deck2.Mazo.Length; i++)
        {
            if (deck2.Mazo[i] != null)
            {
                context.DeckOfPlayer_2.Add(deck2.Mazo[i].GetComponent<cardSupremus>());
            }
        }

        //Cartas del field1
        for (int i = 0; i < deck1.Field.Length; i++)
        {
            if (deck1.Field[i] != null)
            {
                context.FieldOfPlayer_1.Add(deck1.Field[i].GetComponent<cardSupremus>());
            }
        }
        //Cartas del field2
        for (int i = 0; i < deck2.Field.Length; i++)
        {
            if (deck2.Field[i] != null)
            {
                context.FieldOfPlayer_2.Add(deck2.Field[i].GetComponent<cardSupremus>());
            }
        }

        //Cartas del Graveyard1
        for (int i = 0; i < deck1.Graveyard.Count; i++)
        {
            if (deck1.Graveyard[i] != null)
            {
                context.GraveyardOfPlayer_1.Add(deck1.Graveyard[i].GetComponent<cardSupremus>());
            }
        }
        //Cartas del Graveyard2
        for (int i = 0; i < deck2.Graveyard.Count; i++)
        {
            if (deck2.Graveyard[i] != null)
            {
                context.GraveyardOfPlayer_2.Add(deck2.Graveyard[i].GetComponent<cardSupremus>());
            }
        }
    }
}


