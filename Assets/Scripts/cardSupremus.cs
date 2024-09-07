using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cardMaker;
using UnityEngine.UI;
using System.IO;
using TMPro;


public class cardSupremus : MonoBehaviour
{
    public string Name, Faction;
    public float Power;
    public CardType Type;
    public List<AttackMode> Attack;
    public RawImage Imag_Des;
    public GameObject Text_Des_Img;
    public TextMeshProUGUI Text_Des;
    public Deck Deck;
    public bool invoke,effects = false;
    public effect effect;
    public int players;
    public SkeletonCard Card;
    public Sprite sprite;
    private void Start()
    {
        Imag_Des = GameObject.FindGameObjectWithTag("imageCard").GetComponent<RawImage>();
        Text_Des_Img = GameObject.FindGameObjectWithTag("imageDescription");
        Text_Des = GameObject.FindGameObjectWithTag("textDescription").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (players == GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>().Turn || invoke || CardType.Lider == Type) gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        else gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Random");

        if (invoke && !effects && effect != null)
        {
            GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>().DeterminateContext();
            effect.Action();
            effects = true;
        }
    }
    public void Create(SkeletonCard carta,Deck player,int Player)
    {
        Deck = player;
        players = Player;
        Name = carta.Name;
        Faction = carta.Faction;
        Power = carta.Power;
        Type = carta.Type;
        Attack = carta.AttackType;
        effect = carta.effect;
        Card = carta;

        gameObject.name = Name;
        gameObject.transform.localScale = new Vector3(0.11587f, 0.11686f, 1);

        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<BoxCollider2D>();

        if(File.Exists(Application.dataPath + "/Resources/Images/" + Name + ".jpg"))
        {
            sprite = Resources.Load<Sprite>("Images/" + Name);
        }
        else
        {
            sprite = Resources.Load<Sprite>("Images/Random");
        }
        gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.GetComponent<BoxCollider2D>().size = new Vector2(8.03f, 10.49f);
    }

    private void OnMouseEnter()
    {
        Imag_Des.transform.localScale = Vector3.one;
        Text_Des_Img.transform.localScale = Vector3.one;
        Imag_Des.texture = GetComponent<SpriteRenderer>().sprite.texture;
        string ataques = "";
        foreach(AttackMode s in Attack)
        {
            ataques += s.ToString() + " ";
        }
        Text_Des.text = "Name: " + Name + "\n" + "Type: " + Type + "\n" + "Faction: " + Faction + "\n" + "Power: " + Power + "\n" + "Attack: " + ataques;
    }

    private void OnMouseExit()
    {
        Imag_Des.transform.localScale = Vector3.zero;
        Text_Des_Img.transform.localScale = Vector3.zero;
    }

    private void OnMouseDown()
    {
        if (!GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>().initial1 && players == 1)Deck.Select_Invoke(gameObject);
        if (!GameObject.FindGameObjectWithTag("gameManager").GetComponent<GameManager>().initial2 && players == 2)Deck.Select_Invoke(gameObject);
    }
}
