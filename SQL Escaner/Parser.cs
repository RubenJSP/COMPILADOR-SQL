using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SQL_Escaner
{
    class Parser
    {
        private string[,] rules;
        private Stack<int> producciones;
        public Escaner scan;
        public List<Token> errores;
        public Parser(string[] data)
        {
            scan = new Escaner(data);
            producciones = new Stack<int>();
            errores = new List<Token>();
            rules = new string[,]  {
                {"0","0","10 301 11 306 310","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"302","0","0","0","0","0","0","0","0","0","0","0","0","0","72","0" },
                {"304 303","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"0","0","0","99","0","0","0","0","50 302","0","0","0","0","0","0","99" },
                {"4 305","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"0","99","0","99","0","99","99","99","99","51 4","99","0","0","0","0","99" },
                {"308 307","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"0","0","0","0","99","0","0","0","50 306","0","99","0","0","0","0","99" },
                {"4 309","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"4","0","0","0","99","0","0","0","99","0","99","0","0","0","0","99" },
                {"0","0","0","0","12 311","0","0","0","0","0","99","0","0","0","0","99" },
                {"313 312","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0", },
                {"0","0","0","0","0","0","317 311","317 311","0","0","99","0","0","0","0","99" },
                {"304 314","0","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"0","315 316","0","0","0","13 52 300 53","0","0","0","0","0","0","0","0","0","0" },
                {"0","8","0","0","0","0","0","0","0","0","0","0","0","0","0","0" },
                {"304","0","0","0","0","0","0","0","0","0","0","54 318 54","319","0","0","0" },
                {"0","0","0","0","0","0","14","15","0","0","0","0","0","0","0","0" },
                {"0","0","0","0","0","0","0","0","0","0","0","0","0","62","0","0" },
                {"0","0","0","0","0","0","0","0","0","0","0","0","61","0","0","0" }
            };
        
        }

        //SELECT  FROM TABLA
        private void error(int x,int k ,int line)
        {
            if (x >= 300)
            {
                if (x == 305 && (k == 4 || k == 62))
                    this.errores.Add(new Token("Se esperaba delimitador",205,line,2));
                else if (x == 316 && (k == 62||k==53))
                    this.errores.Add(new Token("Se esperaba delimitador",205,line,2));
                else if (x == 305 && (k == 54 || k == 61))
                    this.errores.Add(new Token("Se esperaba operador relacional", 208, line, 2));
                else if (x == 305 && k == 52)
                    this.errores.Add(new Token("Se esperaba palabra reservada", 201, line, 2));
                else if (x == 300 || x == 310 || x == 312 || x == 317)
                    this.errores.Add(new Token("Se esperaba palabra reservada", 201, line, 2));
                else if (x == 301 || x == 302 || x == 304 || x == 305 || x == 306 || x == 308 || x == 309 || x == 311 || x == 313 || (x == 316 && k == 4))
                    this.errores.Add(new Token("Se esperaba identificador", 204, line, 2));
                else if (x == 303 || x == 307)
                    this.errores.Add(new Token("Se esperaba delimitador", 205, line, 2));
                else if (x == 314)
                    this.errores.Add(new Token("Se esperaba operador relacional", 208, line, 2));
                else if (x == 318 || x == 319 || x == 316)
                    this.errores.Add(new Token("Se esperaba constante", 206, line, 2));
            }
            else
            {
                this.errores.Add(new Token("Se esperaba delimitador", 205, line, 2));
            }
        
        }

        private string getProduccion(int x, int terminal)
        {
            int k = getTerminal(terminal);
            int index = x % 300;
            if (k != -1)
                return rules[index, k];
            else
                return "0";
        }

        public bool analyze()
        {
            //$ = 199 P = 300
            producciones.Clear();
            List<Token> tokens = scan.output();
            tokens.Add(new Token("$", 199, 0, 0));
            bool done = false;
            if (scan.errores().Count() < 1)
            {
                int x = 0,k = 0;
                producciones.Push(199);
                producciones.Push(300);
                int pointer = 0;
                do
                {
                    x = producciones.Pop();
                    k = tokens[pointer].Codigo;

                    if (isTerminal(x) || x == 199)
                    {
                        if (x == k)
                            pointer++;
                        else
                        {
                            int line = tokens[pointer].Linea;
                            if (tokens[pointer].Codigo == 199)
                                line = tokens[pointer - 1].Linea;
                            error(x,k,line);
                            return false;
                        }       
                    }
                    else
                    {
                        if (checkRules(x, k))
                        {
                            string produccion = getProduccion(x, k);
                            if (produccion != "99")
                            {
                                insert(produccion);
                            }

                        }
                        else
                        {
                            //Console.WriteLine(": X: " + x + " K: " + k);

                            error(x,k, tokens[pointer].Linea);

                            return false;
                        }
                    }

                    if (x == 199)
                        done = true;

                } while (!done);
            }
            else
            {
                return false;
            }

            return done;
        }
        private int getTerminal(int terminal)
        {
            switch (terminal)
            {
                case 4:
                    return 0;
                case 8:
                    return 1;
                case 10:
                    return 2;
                case 11:
                    return 3;
                case 12:
                    return 4;
                case 13:
                    return 5;
                case 14:
                    return 6;
                case 15:
                    return 7;
                case 50:
                    return 8;
                case 51:
                    return 9;
                case 53:
                    return 10;
                case 54:
                    return 11;
                case 61:
                    return 12;
                case 62:
                    return 13;
                case 72:
                    return 14;
                case 199:
                    return 15;
                default:
                    return -1;
            }
        }


        private bool checkRules(int produccion, int k)
        {
            return getProduccion(produccion, k) != "0";
        }
        private bool isTerminal(int terminal)
        {
            return terminal > 0 && terminal < 300;
        }

        private void insert(string data)
        {
            string[] datos = Regex.Split(data, @"(\d+)");
            for (int i = datos.Length-1; i > 0; i--)
            {

                if (datos[i]!=" "&&datos[i]!="")
                {

                   producciones.Push(int.Parse(datos[i]));

                }
            }
        }
    }
}
