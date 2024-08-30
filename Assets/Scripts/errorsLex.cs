using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

namespace cardMaker
{
    internal class compilerErrors
    {
        public int line;
        public string errors;
        public compilerErrors(int line, string errors)
        {
            this.line = line;
            this.errors = errors;
        }   
    }
}
public class definingErrors
{
    public const string FailedCompilerSemicolon = "Fallo al compilar, se necesita: ;";
    public const string FailedCompilerSeparator = "Fallo al compilar, se necesita: ,";
    public const string FailedCompilerSameOperator = "Fallo al compilar, se necesita: =";
    public const string FailedCompilerColon = "Fallo al compilar, se necesita: :";
    public const string FailedCompilerBrace1 = "Fallo al compilar, se necesita: {";
    public const string FailedCompilerBrace2 = "Fallo al compilar, se necesita: }";
    public const string FailedCompilerSquareBracket1 = "Fallo al compilar, se necesita: [";
    public const string FailedCompilerSquareBracket2 = "Fallo al compilar, se necesita: ]";
    public const string FailedCompilerInadmissibleValue = "Fallo al compilar, valor inadmisible.";
    public const string FailedCompilerDivitionZero = "Fallo al compilar, division por cero indefinida.";
    public const string FailedCompilerStatament = "Declaracion Inadmisible.";
    public const string FailedCompilerUndefinedVar = "Fallo al compilar, variable indefinida.";
    public const string FailedCompilerNoProperty = "Fallo al compilar, la propiedad no existe o ya ha sido utilizada.";
}