using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public int Player;
    public GameObject[] Mazo;
    public GameObject[] Hand_Pos = new GameObject[10];
    public GameObject[] Field_Pos = new GameObject[12];
    public GameObject[] Aum_Pos = new GameObject[3];
    public GameObject[] Aum = new GameObject[3];
    public GameObject[] Hand = new GameObject[10];
    public GameObject[] Field = new GameObject[12];
    public GameObject[] Graveyard = new GameObject[50];
    public GameObject Field_leader;
    public GameObject Card_Invoke;
    public GameObject Leader;

    private int carta_actual_deck = 0;


    //Funcion barajear deck
    public GameObject[] Barajear(GameObject[] Mazo)
    {
        GameObject card;

        for (int i = 0; i < Mazo.Length; i++)
        {
            int d = Random.Range(0, Mazo.Length);
            card = Mazo[i];
            Mazo[i] = Mazo[d];
            Mazo[d] = card;
        }
        return Mazo;
    }

    //Robar carta del mazo
    public void Robar(int cantidad)
    {
        int j = 0;
        for (int i = 0; i < Hand.Length; i++)
        {
            //Robar carta y poner en la mano
            if (Hand[i] == null)
            {
                Hand[i] = Mazo[carta_actual_deck];
                HandPosition(i);
                Mazo[carta_actual_deck] = null;
                carta_actual_deck++;
                j += 1;
            }


            if (j == cantidad) //Destruir carta por no caber en la mano o haber robado todas las cartas
            {
                break;
            }
        }
    }

    public void HandPosition(int i)
    {
        Hand[i].transform.SetPositionAndRotation(Hand_Pos[i].transform.position, Hand_Pos[i].transform.rotation);
    }

    //Selecionar para invocar
    public void Select_Invoke(GameObject card)
    {
        Card_Invoke = card;
    }
}
