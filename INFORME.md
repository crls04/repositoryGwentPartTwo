# Informe Gwent
### Tabla de Contenidos
- Como pudiesen comenzar los nuevos usuarios con el proyecto(****Tutorial Inicial****):
   - Introduccion
   - Reglas
    
- Funcionalidad del proyecto:
   - Juego Inicial
   - Compilador
## Tutorial Inicial
### Introduccion
  Hola ***{Name}***, si eres nuevo por aca, este es el documento que te ayudara a modo de tutorial,
  a entender la jugabildad del famoso juego de cartas ***Gwent***, recreado para un proyecto universitario.
  ### Reglas
   Al comenzar una partida, cada jugador roba ****10****cartas de su deck. antes de que arranque la batalla, los jugadores pueden escoger hasta ****2**** cartas
                       para regresarlas a la baraja y robar la misma cantidad. La maxima cantidad de cartas que se puede tener en la mano es ****10****, cualquier otra carta robada que supere el limite sera descartada. El jugador que gane la ronda anterior, sera quien comience la siguiente
.              . Al inicio de la segunda ronda cada jugador roba ****2**** cartas y si el juego alcanza una tercera ronda cada jugador roba tambien ****2**** cartas antes de comenzarla. Por otra parte cada jugador tienen un campo formado por tres filas, una para las cartas cuerpo a cuerpo(****M****), otra para las de rango(****R****) yotra para las de asedio(****S****). cada fila tiene espacio para una carta de aumento las cuales dan bonificacion de poder en una fila especifica. lomismo pasa con las cartas clima lo que estas disminuyen poder y tienen un espacio en cada filacde cada jugador individualmente. a esto se le suma un espacio al mazo, cementerio y carta lider. cada carta va colocada en la fila de su modo de ataque ya sea (****M****), (****R****) o (****S****). todas las cartas de unidad tienen un indicador de poder, las cartas (****ORO****) no son afectadas por ningun tipode habilidad y cada una puede tener un efecto al ser colocadas. porotra parte quedan las cartas de tipo despeje y senuelo. la primera mencionada quita todos los tipos de carta clima y el senuelo permite colocar una carta con poder 0 en lugar de una carta del campo para regresar a la mano. tambien cada lider tiene una habilidad especifica. la logica del juego consiste en jugar una carta, utilizar una habilidad de lider o pasar. el hecho de pasar implica que el jugador termino de jugar en la ronda actual y portanto no utilizara ninguna otra carta, en el momento en que los dos jugadores hayan pasado, la ronda termina, todas las cartas son enviadas al cementerio y la persona con mayor puntaje ganara. dos rondas ganadas marcan la victoria. si la ronda termina en empate, cada jugador obtiene un punto de ronda. espero que estas instrucciones te sean de ayuda para ganar la batalla, animo !    

      
## Funcionalidad del Proyecto
### Juego Inicial
Despues de ver un tutorial del juego para los nuevos usuarios poder iniciar y probarlo sin problemas, vendria bien mostrar un poco de codigo por si algunos desarrolladores de alguna forma estan interesados en como se creo este proyecto o en coolaborar con el desarrollo de posibles expansiones.  

##### Script InitialGame
```
//metodo para dar click en el gameObject del deck y robar 10 cartas.

private void OnMouseDown()
    {
        if(inicial)
        {
            deck1.Robar(10);
            deck2.Robar(10);
            inicial = false;
            for(int f = 0; f < Image.Length;f++) //ciclo para recorrer los objetos de las cartas y pasarle las imagenes.
            {
                Image[f].texture = deck1.Hand[f].GetComponent<cardSupremus>().sprite.texture;
                Image[f].transform.localScale = Vector2.one;
            }
            Boton.transform.localScale = Vector2.one;
        }
    }

```
##### Funcion para el comienzo del juego
```
//Metodo que permite comenzar a invocar carta
//Este funcion esta en el boton de comenzar para a la hora de descartar dos cartas cuando el contadorllega a 2,
//se vuelve a reiniciar llamando a este metodo para que comience el juego
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
```
##### Script PushCard

```
// metodo para descartar dos cartas.
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
//hay un metodo Siguiente_Jugador para realizarlo con el otro jugador.
```

### Compilador
En esta parte pondremos un breve resumen del codigo de como funciona el compilador en la creacion de cartas y efectos para la cnfeccion de mazos.
Tambien puede resultar interesante porque en este ***Compilador*** no se uso el tipo de sintaxis que se queria para el ***Lexer***.
A continuacion se muestra como fue que se desarrollo el analisis lexico.
##### Script Lexico
```
//Definicion de tipos enum
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
    public enum selectProperty
    {
        Name,
        Faction,
        Power,
        Type,
        Owner
    }
    public class language
    {
//Listas donde se guardaran los enum anteriores

        List<CardType> cardType = new();
        List<AttackMode> attackMode = new();
        List<SourceType> source = new();
        List<selectProperty> cardCharacteristic = new();

        //Constructor con parametros predefinidos, el cual sera quien se encargue de llenar las listas 
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

        public TypeToken verifyValidate(string text)
        {
            if (Regex.IsMatch(text, @"^[\'][a-zA-Z' '0-9]*'"))
            {
                return TypeToken.String;
            }
            else if (Regex.IsMatch(text, @"^[a-zA-Z][a-zA-Z0-9_]*$"))
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
            foreach (CardType s in cardType)
            {
                if (s.ToString() == type)
                {
                    return s;
                }
            }
            return CardType.None;
        }

        //Verificar que existe el tipo de ataque definido al inicio del script

        public AttackMode verifyAttack(string type)
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
        public string verifySelectProperty(string type)
        {
            foreach (selectProperty s in source)
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
```
#####  public TypeToken verifyValidate(string text)
Este metodo en el script anterior contiene la funcion de ***C#***, ***Regex.IsMatch***, que sera la encargada de hacer el trabajo del analisis lexico.
De forma resumida, esta funcion se le pasara un parametro text para verificar que se cumpla que las estructuras que se definen sean iguales a las pasadas por parametro,
en caso de ser asi devuelve el tipo de variable que se le asigne.
##### Archivo txt Egyptians
```
//Ejemplo de deck compilado y guardado en un archivo txt para que despues se ejecute en el juego.

Ra|Lider|Egyptiams|0|Melee-|*
Sobek|Oro|Egyptiams|3|Melee-|*
Anubis|Oro|Egyptiams|3|Melee-|*
Osiris|Plata|Egyptiams|2|Ranged-|*
Osiris|Plata|Egyptiams|2|Ranged-|*
Esfinge|Plata|Egyptiams|2|Siege-|*
Esfinge|Plata|Egyptiams|2|Siege-|*
Ptah|Plata|Egyptiams|2|Melee-|Damage**otherfield.false.Faction == 'Egyptiams'.
Ptah|Plata|Egyptiams|2|Melee-|Damage**otherfield.false.Faction == 'Egyptiams'.
Amon|Oro|Egyptiams|2|Ranged-|Draw**board.false.Power < 4.
Horus|Plata|Egyptiams|1|Ranged-|*
Horus|Plata|Egyptiams|1|Ranged-|*
Tueris|Plata|Egyptiams|1|Ranged-|*
Tueris|Plata|Egyptiams|1|Ranged-|*
Bastet|Plata|Egyptiams|1|Ranged-|*
Bastet|Plata|Egyptiams|1|Ranged-|*
Esfinge|Plata|Egyptiams|2|Siege-|*
Ptah|Plata|Egyptiams|2|Melee-|Damage**otherfield.false.Faction == 'Egyptiams'.
```
#### Ejemplos de Compilacion
```
// Estructura definida para compilar una carta
card {
Name: 'Amon',
Type: 'Oro',
Faction: 'Egyptiams',
Range: ['Ranged'],
Power: 2,
}

//Estructura definida para compilar un efecto
effect {
Name: 'Draw',
Params: {
Amount: Number,
}
Action: (targets, context) => {

carta = context.Deck.Pop();
context.Hand.Add(carta);
}
}

//Estructura definida para compilar y agregarle el efecto a la carta
card {
Name: 'Amon',
Type: 'Oro',
Faction: 'Egyptiams',
Range: ['Ranged'],
Power: 2,
OnActivation: [
{
Effect: {
Name: 'Draw',
}
Selector: {
Source: 'board',
Single: true,
Predicate: (unit) => unit.Faction == 'Egyptiams',
}
}
]
}
```
***Nota***: *Espero que el contenido de este informe le haya sido de utilidad a la hora de comenzar a jugar por primera vez y tambien para que se entendiese de manera resumida algo sobre el codigo del proyecto a la hora de compilar mas bien y sobre como funcionaba el inicio(reciclado del primer proyecto).*
