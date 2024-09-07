using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pushCard : MonoBehaviour
{
    public initialGame game;
    public int carta = 0;
    public GameManager manager;

    public void Usar()
    {
        if (manager.Turn == 2)
        {
            game.Botar(carta, manager.deck2);
        }
        if (manager.Turn == 1)
        {
            game.Botar(carta, manager.deck1);
        }
    }

}
