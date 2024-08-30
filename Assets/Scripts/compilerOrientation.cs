using cardMaker;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class compilerOrientation : MonoBehaviour
{
    public TMP_InputField Console, Output;

    public void compiler()
    {
        syntax process = new();
        process.delete();
        process.verifySyntax(Console.text);
        Output.text = "";
        if (process.errors.Count <= 0)
        {
           
            Output.text += "Compilacion exitosa";

        }

        else
        {
            foreach ( compilerErrors lines in process.errors)
            {
                Output.text += "Line: " + lines.line + ": *" + lines.errors + "*" + "\n";
            }
        }

    }
}
