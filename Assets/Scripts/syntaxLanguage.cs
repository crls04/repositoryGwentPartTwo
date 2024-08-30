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
