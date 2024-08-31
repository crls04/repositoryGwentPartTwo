using cardMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casilla_Invocacion : MonoBehaviour
{
    public int player = 1;
    public int casilla;
    public AttackMode AttackMode;
    public Deck Deck;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void Invocar()
    {
        bool invocada = false;
        if (Deck.Card_Invoke != null && !Deck.Card_Invoke.GetComponent<cardSupremus>().invoke && !gameManager.invoke && gameManager.Turn == Deck.Player)
        {

            foreach(AttackMode att in Deck.Card_Invoke.GetComponent<cardSupremus>().Attack)
            {
                if (att == AttackMode)
                {                   
                    if (Deck.Card_Invoke.GetComponent<cardSupremus>().Type == CardType.Oro || Deck.Card_Invoke.GetComponent<cardSupremus>().Type == CardType.Plata && Deck.Field[casilla] == null)
                    {
                        Deck.Card_Invoke.transform.position = transform.position;
                        Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                        Deck.Field[casilla] = Deck.Card_Invoke;
                        invocada = true;
                    }
                    
                    if (Deck.Card_Invoke.GetComponent<cardSupremus>().Type == CardType.Clima)
                    {
                        if(att == AttackMode.Melee && gameManager.Climas[0] == null && casilla == 12)
                        {
                            Deck.Card_Invoke.transform.position = gameManager.Climas_Pos[0].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            gameManager.Climas[0] = Deck.Card_Invoke;
                            invocada = true;
                        }

                        if (att == AttackMode.Ranged && gameManager.Climas[1] == null && casilla == 13)
                        {
                            Deck.Card_Invoke.transform.position = gameManager.Climas_Pos[1].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            gameManager.Climas[1] = Deck.Card_Invoke;
                            invocada = true;
                        }

                        if (att == AttackMode.Siege && gameManager.Climas[2] == null  && casilla == 14)
                        {
                            Deck.Card_Invoke.transform.position = gameManager.Climas_Pos[2].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            gameManager.Climas[2] = Deck.Card_Invoke;
                            invocada = true;
                        }
                    }

                    if (Deck.Card_Invoke.GetComponent<cardSupremus>().Type == CardType.Aumento)
                    {
                        if (att == AttackMode.Melee && Deck.Aum[0] == null && casilla == 15)
                        {
                            Deck.Card_Invoke.transform.position = Deck.Aum_Pos[0].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            Deck.Aum[0] = Deck.Card_Invoke;
                            invocada = true;
                        }

                        if (att == AttackMode.Ranged && Deck.Aum[1] == null && casilla == 16)
                        {
                            Deck.Card_Invoke.transform.position = Deck.Aum_Pos[1].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            Deck.Aum[1] = Deck.Card_Invoke;
                            invocada = true;
                        }

                        if (att == AttackMode.Siege && Deck.Aum[2] == null && casilla == 17)
                        {
                            Deck.Card_Invoke.transform.position = Deck.Aum_Pos[2].transform.position;
                            Deck.Card_Invoke.GetComponent<cardSupremus>().invoke = true;
                            Deck.Aum[2] = Deck.Card_Invoke;
                            invocada = true;
                        }
                    }

                    if (invocada)
                    {
                        for (int i = 0; i < Deck.Hand.Length; i++)
                        {
                            if (Deck.Card_Invoke == Deck.Hand[i])
                            {
                                Deck.Hand[i] = null;
                            }
                        }
                        for(int i = 0; i < gameManager.Board.Length; i++)
                        {
                            if(gameManager.Board[i] == null)
                            {
                                gameManager.Board[i] = Deck.Card_Invoke;
                                break;
                            }
                        }
                        Deck.Card_Invoke = null;
                        gameManager.invoke = true;
                    }
                }

            }
        }
    }

    private void OnMouseDown()
    {
        Invocar();
    }
}
