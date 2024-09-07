using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class initialGame : MonoBehaviour
{
    public Deck deck1, deck2;
    private bool inicial = true;
    public RawImage[] Image = new RawImage[10];
    public GameObject Boton;
    private int cant = 0;
    public GameManager manager;

    private void OnMouseDown()
    {
        if(inicial)
        {
            deck1.Robar(10);
            deck2.Robar(10);
            inicial = false;
            for(int f = 0; f < Image.Length;f++)
            {
                Image[f].texture = deck1.Hand[f].GetComponent<SpriteRenderer>().sprite.texture;
                Image[f].transform.localScale = Vector2.one;
            }
            Boton.transform.localScale = Vector2.one;
        }
    }

    //Metodo que muestra cartas a descartar del rival
    public void Siguiente_Jugador()
    {
        if(manager.Round1 == 0 && manager.Round2 == 0)
        {
            if (manager.Turn == 2 && manager.initial2)
            {
                for (int f = 0; f < Image.Length; f++)
                {
                    Image[f].texture = deck2.Hand[f].GetComponent<SpriteRenderer>().sprite.texture;
                    Image[f].transform.localScale = Vector2.one;
                }
                Boton.transform.localScale = Vector2.one;
                manager.invoke = true;
            }
            
        }
        
    }

    //Metodo que descarta y roba carta nueva
    public void Botar(int carta, Deck deck)
    {
        Destroy(deck.Hand[carta]);
        deck.Hand[carta] = null;
        deck.Robar(1);
        Image[carta].transform.localScale = Vector2.zero;
        cant++;
        if(cant == 2)
        {
            End_Fase();
        }
    }

    //Metodo que permite comenzar a invocar carta
    public void End_Fase()
    {
        for(int f = 0; f < Image.Length; f++)
        {
            if (Image[f].texture != null)
            {
                Image[f].transform.localScale = Vector3.zero;
                Boton.transform.localScale = Vector3.zero;
            }
        }
        if(manager.Turn == 2)
        {
            manager.initial2 = false;
        }
        if (manager.Turn == 1)
        {
            manager.initial1 = false;
        }
        cant = 0;
        manager.invoke = false;
    }
}
