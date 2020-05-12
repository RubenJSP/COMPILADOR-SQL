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
        public List<Token> tokens;
        public Semantic semantica;
        public Parser(string[] data)
        {
            producciones = new Stack<int>();
            errores = new List<Token>();
            rules = new string[,]  {
                {"0", "0", "0", "0", "0", "0", "0", "0", "16 17 4 52 202 53 55 201", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "200", "0", "0", "0", "0", "0", "0", "0", "211", "0", "0", "0", "0", "0", "0", "0", "99", "0"},
                {"4 203 52 61 53 204 205", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "18", "19", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "20 21", "0", "0", "0", "0", "0", "99", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "50 206", "99", "0", "0", "0", "0", "0", "0", "0"},
                {"202", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "207", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "22 4 208 52 4 53 209", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "24 23", "25 23", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "26 4 52 4 53 210", "0", "50 207", "0", "99", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "50 207", "0", "99", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "27 28 4 29 52 212 53 55 215", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "213 214", "213 214", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "54 62 54 ", "61", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "50 212", "0", "99", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "200", "0", "0", "0", "0", "0", "0", "0", "211 ", "0", "0", "0", "99", "0", "0", "0", "0", "0"},
                {"0", "0", "10 320 301 11 306 310", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"302", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "72", "0", "0"},
                {"304 303", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "99", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "50 302", "0", "0", "0", "0", "0", "0", "99", "0"},
                {"4 700 305", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "99", "0", "99", "0", "99", "99", "99", "0", "0", "0", "0", "0", "0", "0", "0", "0", "99", "51 4 703", "99", "0", "0", "0", "0", "99", "0"},
                {"308 307", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "99", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "50 306", "0", "99", "0", "0", "0", "0", "99", "0"},
                {"4 701 309", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"4 702", "0", "0", "0", "99", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "99", "0", "99", "0", "0", "0", "0", "99", "0"},
                {"0", "0", "0", "0", "12 311", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "99", "0", "0", "0", "0", "99", "0"},
                {"313 312", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "317 311", "317 311", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "99", "0", "0", "0", "0", "99", "0"},
                {"304 314", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "315 316", "0", "0", "0", "13 52 300 53", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "8 704", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"304", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "54 318 54", "319", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "14", "15", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "62", "0", "0", "0"},
                {"0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "61", "0", "0", "0", "0"},
                {"99", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "99", "0", "40"}
            };
        }

        private void error(int x,int k ,int line)
        {
            if (x < 300)
            {

                Console.WriteLine("x " + x + " k " + k);
                if ((x == 200 || x == 201 || x == 203 || x == 204 || x == 207 || x == 208 || x == 211 || x == 215))
                    this.errores.Add(new Token("Se esperaba palabra reservada", 201, line, 2));
                else if ((x == 202 || x == 206))
                    this.errores.Add(new Token("Se esperaba identificador", 204, line, 2));
                else if ((x == 205 || x == 209 || x == 210 || x == 214 || x == 212))
                    this.errores.Add(new Token("Se esperaba delimitador", 205, line, 2));
                else if (x == 213)
                    this.errores.Add(new Token("Se esperaba constante", 206, line, 2));
                else if(x>=10 && x <= 29)
                    this.errores.Add(new Token("Se esperaba palabra reservada", 201, line, 2));
                else if(x>=50 && x<=55)
                    this.errores.Add(new Token("Se esperaba delimitador", 205, line, 2));
                else if(x==61||x==62)
                    this.errores.Add(new Token("Se esperaba constante", 206, line, 2));
                else
                    this.errores.Add(new Token("Se esperaba identificador", 204, line, 2));
            }
            else if (x >= 300)
            {
                if (x == 305 && (k == 4 || k == 62))
                    this.errores.Add(new Token("Se esperaba delimitador",205,line,2));
                else if (x == 316 && (k == 62||k==53))
                    this.errores.Add(new Token("Se esperaba delimitador",205,line,2));
                else if (x == 305 && (k == 54 || k == 61))
                    this.errores.Add(new Token("Se esperaba operador relacional", 208, line, 2));
                else if (x == 305 && k == 52)
                    this.errores.Add(new Token("Se esperaba palabra reservada", 201, line, 2));
                else if (x == 300 || x == 310 || x == 312 || x == 317||x==320)
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
            int index = 0;
            
                int k = getTerminal(terminal);
            //Console.WriteLine("i[" + x + "," + index + "," + k + "]");

            if (x >= 300)
            {
                index = (x % 300) + 16;
            }
            else
                index = x % 200;

                if (k != -1)
                    return rules[index, k];
                else
                    return "0";
            
        }

        public bool analyze()
        {
            //$ = 199 P = 300
            semantica.def();
            semantica.errores = errores;
            producciones.Clear();
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
                    semantica.type(x);
                    if (x >= 700)
                    {
                        //Console.WriteLine("X: " + x + " T " + tokens[pointer - 1].Dato + " L " + tokens[pointer - 1].Linea);

                        Token err = semantica.analyze(x, tokens[pointer - 1]);
                         if (err != null)
                         {
                             this.errores.Add(err);
                             return false;
                         }

                    }
                    else
                    {
                        if (isTerminal(x) || x == 199)
                        {
                            // Console.WriteLine("T[" + x + "]");
                            if (x == k)
                            {

                                pointer++;
                            }
                            else
                            {
                                //Console.WriteLine("ERR T");
                                int line = tokens[pointer].Linea;
                                if (tokens[pointer].Codigo == 199)
                                    line = tokens[pointer - 1].Linea;
                                error(x, k, line);
                                return false;
                            }
                        }
                        else
                        {
                            if (checkRules(x, k))
                            {
                                //Console.WriteLine("[" + x + "," + k + "]");
                                string produccion = getProduccion(x, k);
                                if (produccion != "99")
                                {
                                    insert(produccion);
                                }

                            }
                            else
                            {
                                // Console.WriteLine("ERR P [" + x + "," + k + "]" );

                                if (tokens[pointer].Codigo == 199)
                                    error(x, k, tokens[pointer - 1].Linea);
                                else
                                    error(x, k, tokens[pointer].Linea);

                                return false;
                            }
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
                case 16:
                    return 8;
                case 18:
                    return 9;
                case 19:
                    return 10;
                case 20:
                    return 11;
                case 22:
                    return 12;
                case 24:
                    return 13;
                case 25:
                    return 14;
                case 26:
                    return 15;
                case 27:
                    return 16;
                case 50:
                    return 17;
                case 51:
                    return 18;
                case 53:
                    return 19;
                case 54:
                    return 20;
                case 61:
                    return 21;
                case 62:
                    return 22;
                case 72:
                    return 23;
                case 199:
	                return 24;
                case 40:
                    return 25;
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
            return terminal > 0 && terminal < 200;
        }

        private void insert(string data)
        {
            string[] datos = Regex.Split(data, @"(\d+)");
            for (int i = datos.Length-1; i > 0; i--)
            {       
                    if(Regex.IsMatch(datos[i],@"\d+"))
                         producciones.Push(int.Parse(datos[i]));
               
            }
        }
    }
}
