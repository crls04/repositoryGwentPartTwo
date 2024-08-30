using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace cardMaker
{
    internal class syntax
    {
        public Dictionary<string, string> VarString = new();
        public Dictionary<string, string> VarNumber = new();
        public Dictionary<string, string> VarBool = new();
        public List<compilerErrors> errors = new();
        readonly language desarrollo = new();

        //este metodo esta creado con el fin de eliminar los espacios en blanco al principio de cada linea 
        public string destroySpace(string code)
        {
            bool play = false;
            int i = 0;
            int d = 0;
            for (int a = code.Length - 1; a >= 0; a--)
            {
                if (code[a] != ' ')
                {
                    i = a;
                    break;
                }
            }
            foreach (char c in code)
            {
                if (!play && c != ' ')
                {
                    play = true;
                }
                if (play && d <= i)
                {
                    code += c;
                    d++;
                }
            }
            return code;
        }

        //metodo para verificar el final de una linea
        private bool finalLine(string Code, char end)
        {
            if (Code.Length >= 1 && Code[^1] == end)
            {
                return true;
            }
            return false;
        }
        //funcion empleada para encontrar una coincidencia en una linea
        public bool findCoincidence(string Code, string sentence)
        {
            if (Code.Contains(sentence))
            {
                return true;
            }
            return false;
        }

        //metodo que devuelve valor de una variable tipo numero y verificar si existe
        private string VarValue(Dictionary<string, string> Variables, string Var)
        {
            foreach (var word in Variables.Keys)
            {
                if (Var == word)
                {
                    return Variables[Var];
                }
            }
            return " ";
        }

        //funcion para crear y concatenar string usando el operador concatenar
        public string createString(string Code, int Nline)
        {
            Code = destroySpace(Code);
            if (findCoincidence(Code, "@"))
            {
                string[] var = Code.Split('@');
                for (int i = 0; i < var.Length; i++)
                {
                    var[i] = destroySpace(var[i]);

                    //se verifica si es una variable
                    if (desarrollo.verifyValidate(var[i]) == TypeToken.Var)
                    {
                        var[i] = VarValue(VarString, var[i]);
                    }
                }
                if (findCoincidence(Code, "@@"))
                {
                    int spacio = 0;
                    for (int i = 0; i < var.Length; i++)
                    {
                        if (desarrollo.verifyValidate(var[i]) == TypeToken.String)
                        {
                            spacio = 0;
                        }
                        if (i < var.Length - 1 && spacio < 1 && var[i] == "")
                        {
                            var[i] = "' '";
                            spacio++;
                        }

                    }
                }

                foreach (string s in var)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.String)
                    {

                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                        return " ";
                    }
                }

                string line = "";
                foreach (string s in var)
                {
                    foreach (char c in s)
                    {
                        if (c != '\'')
                        {
                            line += c;
                        }
                    }

                }
                return line;

            }
            else if (desarrollo.verifyValidate(Code) == TypeToken.String)
            {
                string line = "";
                foreach (char c in Code)
                {
                    if (c != '\'')
                    {
                        line += c;
                    }
                }
                return line;
            }
            else
            {
                return " ";
            }
        }

        //funcion empleada para poder crear una variable de tipo string
        public bool creatingStringVariable(string code, int Nline)
        {
            if (finalLine(code, ';')) //revisa que la declaracion de la constate termine en ;
            {
                if (findCoincidence(code, "=")) //encuentra el operador de asignacion de la constante
                {
                    string[] Sentencias = code.Split('=');
                    string[] var = Sentencias[0].Split(' ');
                    if (desarrollo.verifyValidate(var[1]) != TypeToken.Var || var[0] != "string") //se verifica que la variable sea valida del tipo string
                    {
                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
                        return false;
                    }
                    string[] declarate = Sentencias[1].Split(';');
                    declarate[0] = destroySpace(declarate[0]);

                    //Verifse vrifica si es una variable
                    if (desarrollo.verifyValidate(declarate[0]) == TypeToken.Var)
                    {
                        declarate[0] = VarValue(VarString, declarate[0]);
                    }

                    //evalua si existe la operacion de concatenacion de strings
                    string notNull = createString(declarate[0], Nline);
                    if (notNull != " ")
                    {
                        VarString[var[1]] = notNull;
                        return true;
                    }
                    else
                    {
                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
                        return false;
                    }
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSameOperator));
                    return false;
                }
            }
            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSemicolon));
                return false;
            }
        }

        //Metodo para encontrar un operador doble en una linea
        public bool IncOrDec(string code)

        {
            string[] num;
            if (findCoincidence(code, "++"))
            {
                num = code.Split("++");
                if (VarNumber.ContainsKey(destroySpace(num[0])))
                {
                    VarNumber[destroySpace(num[0])] = (int.Parse(VarNumber[destroySpace(num[0])]) + 1).ToString();
                    return true;
                }
            }
            else if (findCoincidence(code, "--"))
            {
                num = code.Split("++");
                if (VarNumber.ContainsKey(destroySpace(num[0])))
                {
                    VarNumber[destroySpace(num[0])] = (int.Parse(VarNumber[destroySpace(num[0])]) - 1).ToString();
                    return true;
                }
            }
            return false;
        }

        //funcion creada para la implementacion de las operaciones aritmeticas(suma, resta, multiplicacion, divsion, potencia)
        public string OperationAritmetic(string Code, int Nline)
        {
            string[] sum, rest, mul, div, pot;
            Code = destroySpace(Code);
            bool number;

            //se verifica si es un numero
            if (desarrollo.verifyValidate(Code) == TypeToken.Number)
            {
                return Code;
            }

            //Verificar operador doble
            else if (IncOrDec(Code))
            {
                return VarValue(VarNumber, Code);
            }

            //se verifica si es una variable
            else if (desarrollo.verifyValidate(Code) == TypeToken.Var)
            {
                return VarValue(VarNumber, Code);
            }
            //se verifica si existe la operacion de suma
            else if (findCoincidence(Code, "+"))
            {
                number = true;
                sum = Code.Split('+');
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i] = destroySpace(sum[i]);
                    if (desarrollo.verifyValidate(sum[i]) == TypeToken.Var)
                    {
                        sum[i] = VarValue(VarNumber, sum[i]);
                    }
                    if (desarrollo.verifyValidate(sum[i]) != TypeToken.Number)
                    {
                        sum[i] = OperationAritmetic(sum[i], Nline);
                    }
                }
                foreach (string s in sum)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(sum[0]);
                    for (int i = 1; i < sum.Length; i++)
                    {
                        var += float.Parse(sum[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                    return " ";
                }
            }

            //se verifica si existe la operacion de resta
            else if (findCoincidence(Code, "-"))
            {
                number = true;
                rest = Code.Split('-');
                for (int i = 0; i < rest.Length; i++)
                {
                    rest[i] = destroySpace(rest[i]);
                    if (desarrollo.verifyValidate(rest[i]) == TypeToken.Var)
                    {
                        rest[i] = VarValue(VarNumber, rest[i]);
                    }
                    if (desarrollo.verifyValidate(rest[i]) != TypeToken.Number)
                    {
                        rest[i] = OperationAritmetic(rest[i], Nline);
                    }
                }
                foreach (string s in rest)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(rest[0]);
                    for (int i = 1; i < rest.Length; i++)
                    {
                        var -= float.Parse(rest[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                    return " ";
                }
            }

            //se verfica si si existe la operacion de multiplicar
            else if (findCoincidence(Code, "*"))
            {
                number = true;
                mul = Code.Split('*');
                for (int i = 0; i < mul.Length; i++)
                {
                    mul[i] = destroySpace(mul[i]);
                    if (desarrollo.verifyValidate(mul[i]) == TypeToken.Var)
                    {
                        mul[i] = VarValue(VarNumber, mul[i]);
                    }
                    if (desarrollo.verifyValidate(mul[i]) != TypeToken.Number)
                    {
                        mul[i] = OperationAritmetic(mul[i], Nline);
                    }
                }
                foreach (string s in mul)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(mul[0]);
                    for (int i = 1; i < mul.Length; i++)
                    {
                        var *= float.Parse(mul[i]);
                    }
                    return var.ToString();
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                    return " ";
                }
            }

            //se verifica si existe la operacion de division (incluida la divsion entre 0)
            else if (findCoincidence(Code, "/"))
            {
                number = true;
                div = Code.Split('/');
                for (int i = 0; i < div.Length; i++)
                {
                    div[i] = destroySpace(div[i]);
                    if (desarrollo.verifyValidate(div[i]) == TypeToken.Var)
                    {
                        div[i] = VarValue(VarNumber, div[i]);
                    }
                    if (desarrollo.verifyValidate(div[i]) != TypeToken.Number)
                    {
                        div[i] = OperationAritmetic(div[i], Nline);
                    }
                }
                foreach (string s in div)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    float var = float.Parse(div[0]);
                    for (int i = 1; i < div.Length; i++)
                    {
                        if (float.Parse(div[i]) != 0)
                        {
                            var /= float.Parse(div[i]);
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerDivitionZero));
                            return " ";
                        }

                    }
                    return var.ToString();
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                    return " ";
                }
            }

            //se verifica si existe la operacion de potencia
            else if (findCoincidence(Code, "^"))
            {
                number = true;
                pot = Code.Split('^');
                for (int i = 0; i < pot.Length; i++)
                {
                    pot[i] = destroySpace(pot[i]);
                    if (desarrollo.verifyValidate(pot[i]) == TypeToken.Var)
                    {
                        pot[i] = VarValue(VarNumber, pot[i]);
                    }
                }
                foreach (string s in pot)
                {
                    if (desarrollo.verifyValidate(s) != TypeToken.Number)
                    {
                        number = false;
                    }
                }
                if (number)
                {
                    for (int i = pot.Length - 2; i >= 0; i--)
                    {
                        pot[i] = Math.Pow(double.Parse(pot[i]), double.Parse(pot[i + 1])).ToString();
                    }

                    return pot[0];
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                    return " ";
                }
            }

            //Verificar si es un error de entrada
            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));
                return " ";
            }

        }

        //funcion empleada para crear una variable de tipo number
        private void creatingNumberVariable(string Line, int Nline)
        {
            if (finalLine(Line, ';'))
            {
                if (findCoincidence(Line, "="))
                {
                    string[] separanum = Line.Split('=');
                    string[] Verify = separanum[0].Split(' ');
                    string[] ent = separanum[1].Split(';');
                    if (Verify.Length >= 2 && desarrollo.verifyValidate(Verify[1]) == TypeToken.Var && Verify[0] == "number")
                    {
                        string num = OperationAritmetic(ent[0], Nline);
                        if (num != " ")
                        {
                            VarNumber[Verify[1]] = num;
                        }

                    }

                    else
                    {
                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
                    }
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSameOperator));
                }
            }
            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSemicolon));
            }
        }

        //funcion empleada para crear una variable de tipo bool
        private void CreateBoolVar(string Line, int Nline)
        {
            if (finalLine(Line, ';'))
            {
                if (findCoincidence(Line, "="))
                {
                    string[] separavar = Line.Split('=');
                    string[] Verify = separavar[0].Split(' ');
                    string[] ent = separavar[1].Split(';');
                    if (Verify.Length >= 2 && desarrollo.verifyValidate(Verify[1]) == TypeToken.Var && Verify[0] == "bool")
                    {
                        try
                        {
                            bool var;
                            var = bool.Parse(ent[0]);
                            VarBool[Verify[1]] = var.ToString();
                        }
                        catch (FormatException)
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerInadmissibleValue));

                        }
                    }
                    else
                    {
                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
                    }
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSameOperator));
                }
            }
            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerSemicolon));
            }

        }

        //funcion implementada para poder definir una crata con todas sus caracterisiticas( tipo, poder, etc)
        private void CardCreate(string[] Code, int Nline)
        {
            string Type = " ";
            string Name = " ";
            string Faction = " ";
            string Power = " ";
            string Range = " ";
            string onActivation = "*";
            if (finalLine(Code[Nline - 1], '{'))
            {
                while (Code[Nline - 1] != "}")
                {
                    if (Nline + 1 < Code.Length)
                    {
                        Nline++;
                    }
                    else
                    {
                        break;
                    }
                    Code[Nline - 1] = destroySpace(Code[Nline - 1]);
                    if (Code[Nline - 1] != "")
                    {
                        string Definition = VerificateCard(Code[Nline - 1], Nline);

                        if (Definition != " ")
                        {
                            string[] var = Definition.Split('-');

                            //se verifica si la variable es Name
                            if (var[0] == "Name" && Name == " ")
                            {
                                Name = var[1];
                            }

                            //se verifica si la variable es Faction
                            else if (var[0] == "Faction" && Faction == " ")
                            {
                                Faction = var[1];
                            }

                            //se verifica si la variable es Type
                            else if (var[0] == "Type" && Type == " ")
                            {
                                Type = var[1];
                            }

                            //se verifica si la variable es Power
                            else if (var[0] == "Power" && Power == " ")
                            {
                                Power = var[1];
                            }

                            //se verifica si la variable es Range
                            else if (var[0] == "Range" && Range == " ")
                            {
                                for (int i = 1; i < var.Length; i++)
                                {
                                    Range += var[i] + "-";
                                }
                            }

                            //se verifica la definicion OnActivation
                            else if (Definition == "OnActivation")
                            {
                                if (destroySpace(Code[Nline]) == "{")
                                {
                                    Nline++;
                                    onActivation = VerificateOnActivation(Code, Nline);
                                    if (onActivation != " ")
                                    {
                                        string[] on = onActivation.Split('*');
                                        int linesAdd = on[1].Split('.').Length + 9;
                                        Nline += linesAdd;
                                    }
                                }
                                else
                                {
                                    errors.Add(new compilerErrors(Nline, "Definicion onactivacion missing {"));
                                }
                            }

                            //se comprueba si se implemento una propiedad doble o una propiedad que no existe
                            else
                            {
                                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerNoProperty));
                            }
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerNoProperty));
                            return;
                        }
                    }
                }
                if (Code[Nline] != "}")
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerBrace2));
                }

                //crea una carta y la guarda como un archivo dentro de un deck
                if (Type != " " && Name != " " && Faction != " " && Power != " " && Range != " " && errors.Count == 0)
                {
                    Range = destroySpace(Range);
                    string card = (Name + "|" + Type + "|" + Faction + "|" + Power + "|" + Range + "|" + onActivation);
                    string decksito;
                    string decks = File.ReadAllText(Application.dataPath + "/Resources/Setting/DeckInGame.txt");
                    if (File.Exists(Application.dataPath + "/Resources/Decks/" + Faction + ".txt"))
                    {
                        decksito = File.ReadAllText(Application.dataPath + "/Resources/Decks/" + Faction + ".txt");
                        decksito += "\n" + card;
                        SaveCard(Faction, decksito, decks);
                    }
                    else
                    {
                        SaveCard(Faction, card, decks);
                    }
                }
                else
                {
                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
                }
            }

            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerBrace1));

            }
        }

        //Metodo para guardar carta
        private void SaveCard(string Faction, string randomVar, string decks)
        {
            StreamWriter sw = new(Application.dataPath + "/Resources/Decks/" + Faction + ".txt");
            if (!findCoincidence(decks, Faction))
            {
                StreamWriter sw2 = new(Application.dataPath + "/Resources/Setting/DeckInGame.txt");
                decks += "-" + Faction;
                sw2.Write(decks);
                sw2.Close();
            }
            sw.Write(randomVar);
            sw.Close();
        }

        //Metodo para verificar parametros de carta
        private string VerificateCard(string Code, int Nline)
        {
            string[] sintax;
            if (finalLine(Code, ','))
            {
                if (findCoincidence(Code, ":"))
                {
                    ////se verifica que sea valido y se pueda declarar un array de Range
                    if (Regex.IsMatch(Code, @"^Range|<#definition\s+['{']*;$"))
                    {
                        if (findCoincidence(Code, "[") && findCoincidence(Code, "]"))
                        {
                            string arrayAttack = "Range";
                            sintax = Code.Split('[', ']');
                            if (findCoincidence(sintax[1], ","))
                            {
                                sintax = sintax[1].Split(',');
                                if (sintax.Length > 3 || sintax.Length < 1)
                                {
                                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                                    return " ";
                                }
                                else
                                {
                                    foreach (string s in sintax)
                                    {
                                        string d;
                                        if (desarrollo.verifyValidate(s) == TypeToken.Var)
                                        {
                                            d = VarValue(VarString, s);
                                        }
                                        else if (desarrollo.verifyValidate(s) == TypeToken.String)
                                        {
                                            d = createString(s, Nline);
                                        }
                                        else
                                        {
                                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                                            return " ";
                                        }
                                        if (desarrollo.verifyAttack(d) != AttackMode.None && !findCoincidence(arrayAttack, d) && d != " ")
                                        {

                                            arrayAttack += "-" + d;
                                        }
                                        else
                                        {
                                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerNoProperty));
                                            return " ";
                                        }
                                    }
                                    return arrayAttack;
                                }
                            }

                            else
                            {
                                if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                                {
                                    sintax[1] = VarValue(VarString, sintax[1]);
                                }
                                else if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                                {
                                    sintax[1] = createString(sintax[1], Nline);
                                }
                                else
                                {
                                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                                    return " ";
                                }
                                if (desarrollo.verifyAttack(sintax[1]) != AttackMode.None && !findCoincidence(arrayAttack, sintax[1]))
                                {

                                    arrayAttack += "-" + sintax[1];
                                }
                                else
                                {
                                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerNoProperty));
                                    return " ";
                                }
                            }
                            return arrayAttack;

                        }
                    }

                    //Continuar verificaciones con variables simples
                    sintax = Code.Split(':', ',');
                    sintax[1] = destroySpace(sintax[1]);

                    ////se verifica que sea valido y se pueda declarar la variable Name
                    if (Regex.IsMatch(Code, @"^Name|<#definition\s+['{']*;$") || Regex.IsMatch(Code, @"^Faction|<#definition\s+['{']*;$"))
                    {
                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                        {
                            sintax[1] = VarValue(VarString, sintax[1]);
                        }

                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                        {
                            sintax[1] = createString(sintax[1], Nline);
                        }

                        if (sintax[1] != " " && findCoincidence(Code, "Name"))
                        {
                            return "Name-" + sintax[1];
                        }
                        if (sintax[1] != " " && findCoincidence(Code, "Faction"))
                        {
                            return "Faction-" + sintax[1];
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                        }
                    }


                    ////se verifica que sea valido y se pueda declarar la variable Type
                    if (Regex.IsMatch(Code, @"^Type|<#definition\s+['{']*;$"))
                    {

                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                        {
                            sintax[1] = VarValue(VarString, sintax[1]);
                        }
                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                        {
                            sintax[1] = createString(sintax[1], Nline);
                        }
                        if (desarrollo.verifyCard(sintax[1]) != CardType.None)
                        {
                            return "Type-" + sintax[1];
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                            return " ";
                        }
                    }

                    ////se verifica que sea valido y se pueda declarar la variable Power
                    if (Regex.IsMatch(Code, @"^Power|<#definition\s+['{']*;$"))
                    {
                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                        {
                            sintax[1] = VarValue(VarNumber, sintax[1]);
                        }
                        if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                        {
                            sintax[1] = OperationAritmetic(sintax[1], Nline);
                        }
                        if (findCoincidence(Code, "Power") && sintax[1] != " ")
                        {
                            return "Power-" + sintax[1];
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                            return " ";
                        }
                    }
                }
            }
            //Verificacion onActivation
            else if (finalLine(Code, '['))
            {

                if (Regex.IsMatch(Code, @"^OnActivation|<#definition\s+['{']*;$"))
                {
                    if (findCoincidence(Code, ":"))
                    {
                        if (Code.Split(':').Length == 2)
                        {
                            return "OnActivation";
                        }
                        else
                        {
                            errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                            return " ";
                        }
                    }
                    else
                    {
                        errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerUndefinedVar));
                        return " ";
                    }
                }
            }

            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerColon));
            }
            return " ";
        }

        //esta funcion se encarga de la revision de todo el desarrollo de la sintaxis
        public void verifySyntax(string code)
        {
            string[] lines = code.Split('\n'); //Con la funcion split se separa el bloque de codigo linea por linea
            int NLine = 1;

            //aqui se hace la llamada al metodo destroySpace para que elimine los espacios en blanco al principio y al final de cada linea
            for (int f = 0; f < lines.Length; f++)
            {
                lines[f] = destroySpace(lines[f]);

            }

            foreach (string line in lines)
            {
                //se verifica que se cumplan los siguientes parametros para la declaracion de un tipo string
                if (Regex.IsMatch(line, @"^string|<#texto\s+[a-z](1,15)(\s+:\s+[a-z](1,15)')*;$"))
                {
                    creatingStringVariable(line, NLine);
                }

                //se verifica que se cumplan los siguientes parametros para la declaracion de un tipo number
                else if (Regex.IsMatch(line, @"^number|<#real\s+[a-z](1,15)(\s+:\s+\d(0,32000))*;$"))
                {
                    creatingNumberVariable(line, NLine);
                }

                //se verifica que se cumplan los siguientes parametros para la declaracion de booleanos simples
                if (Regex.IsMatch(line, @"^bool|<#boolean\s+[a-z](1,15)(\s+:\s+(true|false))*;$"))
                {
                    CreateBoolVar(line, NLine);
                }

                //se verifica que se cumplan los siguientes parametros para la creacion de cartas
                if (Regex.IsMatch(line, @"^card|<#definition\s+['{']*;$"))
                {
                    CardCreate(lines, NLine);
                }

                //Verificacion de operador doble 
                else if (IncOrDec(line)) continue;

                //se verifica si hubo errores en la iteracion
                if (errors.Count != 0)
                {
                    break;
                }
                NLine++;
            }
        }

        //Metodo para verificar OnActivation
        private string VerificateOnActivation(string[] Code, int Nline)
        {
            bool cierre = false;
            string[] OnActive = new string[3];

            //Nombre del efecto
            if (Regex.IsMatch(Code[Nline], @"^Effect|<#definition\s+['{']*;$") && findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], '{'))
            {
                while (destroySpace(Code[Nline]) != "}" && !Regex.IsMatch(Code[Nline], @"^Selector|<#definition\s+['{']*;$"))
                {
                    Nline++;
                    //Encontrando nombres
                    if (Regex.IsMatch(Code[Nline], @"^Name|<#definition\s+['{']*;$"))
                    {
                        if (findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], ','))
                        {
                            string[] sintax = Code[Nline].Split(':', ',');
                            sintax[1] = destroySpace(sintax[1]);
                            if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                            {
                                sintax[1] = VarValue(VarString, sintax[1]);
                            }

                            if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                            {
                                sintax[1] = createString(sintax[1], Nline);
                            }

                            if (sintax[1] != " ")
                            {
                                if (File.Exists(Application.dataPath + "/Resources/Effects/" + sintax[1] + ".txt"))
                                {
                                    OnActive[0] = sintax[1];

                                }
                            }
                        }
                    }

                    //Encontrando Parametros
                    if (findCoincidence(Code[Nline], ":") && findCoincidence(Code[Nline], ",") && !Regex.IsMatch(Code[Nline], @"^Name|<#definition\s+['{']*;$"))
                    {
                        string[] param = Code[Nline].Split(':', ',');
                        param[1] = destroySpace(param[1]);
                        param[0] = destroySpace(param[0]);
                        if (desarrollo.verifyValidate(param[0]) == TypeToken.Var)
                        {
                            OnActive[1] += param[0] + ":" + param[1] + ".";
                        }
                    }
                }
                if (destroySpace(Code[Nline]) == "}") cierre = true;
            }
            Nline++;

            //Selector
            if (cierre)
            {
                cierre = false;
                if (Regex.IsMatch(Code[Nline], @"^Selector|<#definition\s+['{']*;$") && findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], '{'))
                {
                    string source = "";
                    string single = "false";
                    string predicate = "";
                    int i = 0;
                    Nline++;
                    while (i < 3)
                    {
                        if (Regex.IsMatch(Code[Nline], @"^Source|<#definition\s+['{']*;$"))
                        {
                            if (findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = destroySpace(sintax[1]);
                                if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                                {
                                    sintax[1] = VarValue(VarString, sintax[1]);
                                }

                                if (desarrollo.verifyValidate(sintax[1]) == TypeToken.String)
                                {
                                    sintax[1] = createString(sintax[1], Nline);
                                }

                                sintax[1] = desarrollo.verifySource(sintax[1]);
                                if (sintax[1] != " ")
                                {
                                    source = sintax[1];
                                }
                                else
                                {
                                    errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerNoProperty));
                                }
                            }
                        }
                        if (Regex.IsMatch(Code[Nline], @"^Single|<#definition\s+['{']*;$"))
                        {
                            if (findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = destroySpace(sintax[1]);
                                if (desarrollo.verifyValidate(sintax[1]) == TypeToken.Var)
                                {
                                    sintax[1] = VarValue(VarString, sintax[1]);
                                }
                                sintax[1] = destroySpace(sintax[1]);
                                if (sintax[1] == "false" || sintax[1] == "true")
                                {
                                    single = sintax[1];
                                }
                            }
                        }
                        if (Regex.IsMatch(Code[Nline], @"^Predicate|<#definition\s+['{']*;$"))
                        {
                            if (findCoincidence(Code[Nline], ":") && finalLine(Code[Nline], ','))
                            {
                                string[] sintax = Code[Nline].Split(':', ',');
                                sintax[1] = destroySpace(sintax[1]);
                                if (findCoincidence(sintax[1], "(") && findCoincidence(sintax[1], ")"))
                                {
                                    string[] unit = sintax[1].Split('(', ')');
                                    if (destroySpace(unit[1]) == "unit" && findCoincidence(sintax[1], "=>") && findCoincidence(sintax[1], "unit."))
                                    {
                                        predicate = sintax[1].Split('.')[1];
                                    }
                                }
                            }
                        }

                        i++;
                        Nline++;
                    }
                    if (source != "" && predicate != "")
                    {
                        OnActive[2] = source + "." + single + "." + predicate;
                    }
                }

            }
            else
            {
                errors.Add(new compilerErrors(Nline, definingErrors.FailedCompilerStatament));
            }


            //Creando respuesta de OnActivation
            if (destroySpace(Code[Nline]) == "}" && destroySpace(Code[Nline + 1]) == "}")
            {
                if (destroySpace(Code[Nline + 2]) == "]")
                {
                    if (OnActive[0] != null && OnActive[2] != null)
                    {
                        string code = OnActive[0] + "*" + OnActive[1] + "*" + OnActive[2];
                        return code;
                    }
                }
                else
                {
                    errors.Add(new compilerErrors(Nline + 2, definingErrors.FailedCompilerSquareBracket2));
                }
            }
            else
            {
                errors.Add(new compilerErrors(Nline + 1, definingErrors.FailedCompilerBrace2));
            }
            return " ";
        }

        

        //funcion implementada para eliminar todo lo que se introduce en el compilador
        public void delete()
        {
            errors.Clear();
            VarBool.Clear();
            VarNumber.Clear();
            VarString.Clear();
        }
    }
}
