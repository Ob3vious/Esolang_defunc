using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace defunc
{
    class Program
    {
        static string vars = String.Empty;
        static string funcs = "0+?.,";
        static List<string> definitions = new List<string> { };
        static List<ConvData> conversions = new List<ConvData> { };

        public struct ConvData {
            public ConvData (string fn, List<int> returns)
            {
                Fn = fn;
                Rt = returns;
            }
            public string Fn;
            public List<int> Rt;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("defunc interpreter v1.2");
            while (true)
            {
                string input = Console.ReadLine();
                if (input != "")
                    if (!VerifyDef(input))
                    {
                        if (VerifyFunc(input))
                            Defunc(input); 
                        else
                            Console.WriteLine("Failed to parse code.");
                    }
            }
        }

        static List<string> Split(string code)
        {
            List<string> split = new List<string> { };
            int d = 1;
            string snippet = "";
            for (int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '0':
                    case ',':
                        d--;
                        break;
                    case '+':
                    case '.':
                        break;
                    case '?':
                        d += 3;
                        break;
                    default:
                        string def = definitions.Where(x => x[0] == code[i]).First();
                        for (int j = 1; j < def.Length && !funcs.Contains(def[j]) && !def.Substring(1, j - 1).Contains(def[j]); j++)
                            d++;
                        d--;
                        break;
                }
                snippet += code[i].ToString();
                if (d == 0)
                {
                    split.Add(snippet);
                    snippet = "";
                    d++;
                }
            }
            return split;
        }

        static int Defunc(string function)
        {
            //Console.WriteLine("\"" + function + "\"");
            if (conversions.Any(x => x.Fn == function))
            {
                List<int> data = conversions.Where(x => x.Fn == function).First().Rt;
                for (int i = 1; i < data.Count(); i++)
                    Console.WriteLine(data[i]);
                return data.First();
            }
            else
                switch (function[0])
                {
                    case '0':
                        conversions.Add(new ConvData(function, new List<int> { 0 }));
                        return 0;
                    case '+':
                        int np = Defunc(Snip(function, 0)) + 1;
                        if (conversions.Any(x => x.Fn == Snip(function, 0)))
                            conversions.Add(new ConvData(function, new List<int> { np }.Concat(conversions.Where(x => x.Fn == Snip(function, 0)).First().Rt.Skip(1)).ToList()));
                        return np;
                    case '?':
                        int a = Defunc(Snip(function, 0));
                        int b = Defunc(Snip(function, 1));
                        int c = 0;
                        if (a > b)
                            c = Defunc(Snip(function, 2));
                        else
                            c = Defunc(Snip(function, 3));
                        if (conversions.Any(x => x.Fn == Snip(function, 0)) && conversions.Any(x => x.Fn == Snip(function, 1)) && conversions.Any(x => x.Fn == Snip(function, a > b ? 2 : 3)))
                            conversions.Add(new ConvData(function, new List<int> { c }.Concat(conversions.Where(x => x.Fn == Snip(function, 0)).First().Rt.Skip(1)).Concat(conversions.Where(x => x.Fn == Snip(function, 1)).First().Rt.Skip(1)).Concat(conversions.Where(x => x.Fn == Snip(function, a > b ? 2 : 3)).First().Rt.Skip(1)).ToList()));
                        return c;
                    case '.':
                        int nf = Defunc(Snip(function, 0));
                        if (conversions.Any(x => x.Fn == Snip(function, 0)))
                            conversions.Add(new ConvData(function, new List<int> { nf }.Concat(conversions.Where(x => x.Fn == Snip(function, 0)).First().Rt.Skip(1)).Concat(new List<int> { nf }).ToList()));
                        Console.WriteLine(nf);
                        return nf;
                    default:
                        if (function.Contains(','))
                        {
                            string sets = String.Empty;
                            for (int i = 0; i < function.Length; i++)
                            {
                                if (function[i] == ',')
                                {
                                    Console.Write("Input? ");
                                    int nc;
                                    while (!int.TryParse(Console.ReadLine(), out nc) || nc < 0) { Console.WriteLine("Invalid input."); Console.Write("Input? "); }
                                    sets += string.Join("", Enumerable.Repeat("+", nc)) + "0";
                                }
                                else
                                    sets += function[i];
                            }
                            return Defunc(sets);
                        }
                        else
                        {
                            int d = 1;
                            string def = definitions.Where(x => x[0] == function[0]).First();
                            for (int j = 1; j < def.Length && !funcs.Contains(def[j]) && !def.Substring(1, j - 1).Contains(def[j]); j++)
                            {
                                if (!vars.Contains(def[j]))
                                    vars += def[j];
                                d++;
                            }
                            string def2 = def.Substring(d, def.Length - d);
                            for (int j = 1; j < d; j++)
                            {
                                def2 = def2.Replace(def[j].ToString(), Snip(function, j - 1));
                            }
                            int nd = Defunc(def2);
                            if (conversions.Any(x => x.Fn == def2))
                                conversions.Add(new ConvData(function, new List<int> { nd }.Concat(conversions.Where(x => x.Fn == def2).First().Rt.Skip(1)).ToList()));
                            return nd;
                        }
                }
        }

        static string Snip(string function, int index)
        {
            return Split(function.Substring(1, function.Length - 1))[index];
        }

        static bool VerifyFunc(string code)
        {
            int d = 1;
            for (int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '0':
                    case ',':
                        d--;
                        break;
                    case '+':
                    case '.':
                        break;
                    case '?':
                        d += 3;
                        break;
                    default:
                        if (!funcs.Contains(code[i]))
                            return false;
                        string def = definitions.Where(x => x[0] == code[i]).First();
                        for (int j = 1; j < def.Length && !funcs.Contains(def[j]) && !def.Substring(1, j - 1).Contains(def[j]); j++)
                            d++;
                        d--;
                        break;
                }
                if (d == 0 && i + 1 != code.Length)
                    return false;
            }
            return d == 0;
        }

        static bool VerifyDef(string definition)
        {
            if ((funcs + vars).Contains(definition[0]))
                return false;
            string tempvars = String.Empty;
            string code = String.Empty;
            for (int i = 1; i < definition.Length; i++)
            {
                if ((funcs + definition[0] + tempvars).Contains(definition[i]))
                {
                    code = definition.Substring(i, definition.Length - i);
                    i = definition.Length;
                }
                else
                    tempvars += definition[i];
            }
            int d = 1;
            for (int i = 0; i < code.Length; i++)
            {
                switch (code[i])
                {
                    case '0':
                    case ',':
                        d--;
                        break;
                    case '+':
                    case '.':
                        break;
                    case '?':
                        d += 3;
                        break;
                    default:
                        string def = "";
                        if (code[i] == definition[0])
                            def = definitions.Concat(new string[] { definition }).Where(x => x[0] == code[i]).First();
                        else
                        {
                            if (!funcs.Contains(code[i]) && !tempvars.Contains(code[i]))
                                return false;
                            if(funcs.Contains(code[i]))
                                def = definitions.Where(x => x[0] == code[i]).FirstOrDefault();
                        }
                        for (int j = 1; def != null && j < def.Length && !(funcs + definition[0]).Contains(def[j]) && !def.Substring(1, j - 1).Contains(def[j]); j++)
                            d++;
                        d--;
                        break;
                }
                if (d == 0 && i + 1 != code.Length)
                    return false;
            }
            if (d != 0)
                return false;
            for (int i = 0; i < tempvars.Length; i++)
                if (!vars.Contains(tempvars[i]))
                    vars += tempvars[i];
            funcs += definition[0];
            definitions.Add(definition);
            return true;
        }
    }
}
