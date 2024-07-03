using System.Collections.Generic;
using System.Linq;

namespace MavLink.Serialize.Generator;

public static class StringExtensions
{
    public static readonly HashSet<string> _keywoards = new HashSet<string>()
    {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "void",
        "volatile",
        "while",
    };
    
    
    
    public static string ToPascalCase(this string str)
    {
        return string.Join("", str.Trim(' ', '.', ',')
            .Split(' ', '!', '@', '#', '$', '"', '%', '^', '&', '*', '(', ')', '_', '+', '-', '<', '>', '?', ',', '.',
                '/', '\\', ':', ';', '=', '{', '}', '[', ']','\'')
            .Where(t => !string.IsNullOrEmpty(t))
            .Select(t => t.Substring(0, 1).ToUpper() + t.ToLower().Substring(1)));
    }

    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return str;
            
        var pascalCase = ToPascalCase(str);
        return pascalCase.Substring(0,1).ToLower() + pascalCase.Substring(1);
    }
    
    public static string ToPascalCaseIdentifier(this string str)
    {
        var pascalCase = ToPascalCase(str);
        return char.IsDigit(pascalCase.FirstOrDefault()) ? "The" + pascalCase : pascalCase;
    }
    
    public static string ToCamelCaseIdentifier(this string str)
    {
        var pascalCase = ToPascalCase(str);
        return char.IsDigit(pascalCase.FirstOrDefault()) ? "the" + pascalCase : pascalCase;
    }

    public static string EscapeKeywords(this string str)
    {
        if (_keywoards.Contains(str))
        {
            return "@" + str;
        }

        return str;
    }

}