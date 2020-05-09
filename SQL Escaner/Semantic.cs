using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Escaner
{
    class Semantic
    {
        private Tabla tablas;
        private Atributo atributos;
        private Restricciones restricciones;
        private List<Token> tokens;
        public List<Token> dml;
        public List<bool> isSub;
        public List<Token> atrib, tables, alias,compare,exist;
        public bool isComp, subQuery;
        public Semantic(List<Token> tokens)
        {
            this.tokens = tokens;
            tablas = new Tabla();
            atributos = new Atributo();
            restricciones = new Restricciones();
            atrib = new List<Token>();
            tables = new List<Token>();
            alias = new List<Token>();
            compare = new List<Token>();
            exist = new List<Token>();
            isSub = new List<bool>();
        }
        public void def()
        {
            subQuery = false;
            isComp = false;

        }
        private int findKey(int table, string atrib)
        {
            for (int i = 0; i < atributos.Nombre.Count; i++)
                if (atributos.Tabla[i] == table)
                    if (atributos.Nombre[i] == atrib)
                        return i;

            return -1;
        }

        public void type(int x)
        {
            if (x == 10)
            {
                //tables.Clear();
                atrib.Clear();
                compare.Clear();
                exist.Clear();
            }
            if(x==14 || x == 15)
            {
                subQuery = false;
            }

            if (x == 12)
            {
                subQuery = false;
                isComp = true;
            }
            if (x == 13)
            {
                subQuery = true;
                isComp = false;
            }
           
                

        }
        public void fill()
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                //Tablas
                if(tokens[i].Codigo == 17)
                {
                    string nombre = tokens[i + 1].Dato;
                    if (!tablas.Nombre.Contains(nombre))
                    {
                        tablas.Nombre.Add(nombre);
                        tablas.Atributos.Add(0);
                        tablas.Restricciones.Add(0);

                    }
                }
                int tablaPos = tablas.Nombre.Count() - 1;
                //Atributos
                if (tokens[i].Codigo == 18 || tokens[i].Codigo == 19)
                {
                    tablas.Atributos[tablaPos]++;
                    atributos.Nombre.Add(tokens[i - 1].Dato);
                    atributos.Tipo.Add(tokens[i].Dato);
                    atributos.Longitud.Add(int.Parse(tokens[i+2].Dato));
                    atributos.Tabla.Add(tablaPos);
                    if (tokens[i + 4].Codigo == 20)
                        atributos.Not_null.Add(true);
                    else
                        atributos.Not_null.Add(false);
                }
                //Restricciones
                if (tokens[i].Codigo == 22)
                {
                    tablas.Restricciones[tablaPos]++;
                    restricciones.NumeroTabla.Add(tablaPos);
                    restricciones.Nombre.Add(tokens[i + 1].Dato);
                    if (tokens[i + 2].Codigo == 24)
                    {
                        Console.WriteLine("T: " + tablaPos + " AT: " + tokens[i + 5].Dato + " F: " + findKey(tablaPos, tokens[i + 5].Dato));
                        restricciones.Atributo_asoc.Add(findKey(tablaPos, tokens[i + 5].Dato));
                        restricciones.Tabla.Add(-1);
                        restricciones.Tipo.Add(1);
                        restricciones.Atributo.Add(-1);

                    }
                    else
                    {
                        restricciones.Tipo.Add(2);
                        restricciones.Atributo_asoc.Add(findKey(tablaPos, tokens[i + 5].Dato));

                    }
                }
                if(tokens[i].Codigo == 26)
                {
                    tablaPos = tablas.Nombre.IndexOf(tokens[i + 1].Dato);
                    restricciones.Tabla.Add(tablaPos);
                    restricciones.Atributo.Add(findKey(tablaPos,tokens[i+3].Dato));
                }
            }
        }
  
        public Token validarAtributo(Token item)
        {
            //Console.WriteLine(tables.Count());

            if (!isComp)
            {
                if (atributos.Nombre.Contains(item.Dato.ToUpper()))
                {
                    atrib.Add(item);
                    return null;
                }
                else
                {
                    return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
                }
            }
            else
            {
                if(tables.Count == 1)
                {
                    int indexTable = tablas.Nombre.IndexOf(tables[0].Dato);
                    if (findKey(indexTable, item.Dato) != -1)
                    {
                        compare.Add(item);
                        return null;
                    }
                    return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
                }
                else if(tables.Count>1)
                {
                   /* for (int i = 0; i < tables.Count; i++)
                    {
                        Console.WriteLine( "#" + (i+1) +"  T: " + tables[i].Dato + " SUB: " + isSub[i]);
                    }*/
                    /*Token ambiguo = null;
                    if (!subQuery)
                        ambiguo = ambiguedad();

                    return (ambiguo != null) ? ambiguo : perteneceA();
                    */
                    return (ambiguedad() != null) ? ambiguedad() : perteneceA();

                }
            }
            return null;
        }

        public Token validarTabla(Token item)
        {
            if (tablas.Nombre.Contains(item.Dato.ToUpper()))
            {
                //Console.WriteLine(item.Dato);
                if (subQuery)
                {
                    isSub.Add(true);
                }
                else
                    isSub.Add(false);

                tables.Add(item);
                return null;
            }
            else
            {
                return new Token("El nombre de la tabla '" + item.Dato + "' no es válido", 314, item.Linea, 3);
            }

        }

        public Token validaTablaAtributo(Token item)
        {
                
                Token table = dml[dml.IndexOf(item) - 2];
                if (tablas.Nombre.Contains(table.Dato))
                {
                    int tableIndex = tablas.Nombre.IndexOf(table.Dato);
                    //Console.WriteLine("AV " + tableIndex + " " + tablas.Nombre.IndexOf(table.Dato));

                    if (findKey(tableIndex, item.Dato.ToUpper()) != -1)
                    {
                        compare.Add(item);
                        return null;
                    }

                    return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
                }
                else
                {
                    return new Token("El identificador '" + table.Dato + "." + item.Dato + "' no es válido", 315, item.Linea, 3);
                }
        }
  
        public Token perteneceA()
        {
            foreach (Token tabla in tables)
            {
               // Console.WriteLine("T: " + tabla.Dato);
                foreach (Token item in atrib)
                {
                    if (!item.Dato.Equals(" "))
                    {
                        if (findKey(tablas.Nombre.IndexOf(tabla.Dato), item.Dato) != -1)
                        {
                            item.Dato = " ";
                            continue;
                         
                        }
                        else
                           return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
                    }

                }
                
                   
            }
          
    
            return null;
        }
        public Token ambiguedad()
        {
            List<string> data = new List<string>();

            for (int i = 0; i < tables.Count; i++)
            {
                if (isSub[i])
                    continue;

                    int tableIndex = tablas.Nombre.IndexOf(tables[i].Dato);
                    for (int j = 0; j < atrib.Count; j++)
                    {
                
                       
                        if (findKey(tableIndex, atrib[j].Dato) != -1)
                        {

                            if (!data.Contains(atrib[j].Dato))
                            {
                                data.Add(atrib[j].Dato);
                                exist.Add(atrib[j]);
                            }
                            else
                                return new Token("El nombre del atributo '" + atrib[j].Dato + "' es ambigüo", 312, atrib[j].Linea, 3);
                        }

                    }
                
            }
            return null;
        }

        public Token analyze(int rule, Token token)
        {
            switch (rule)
            {
                case 700:
                    return validarAtributo(token);
                case 701:
                    return validarTabla(token);
                case 703:
                    return validaTablaAtributo(token);


                default:
                    return null;
            }
           
        }
        public void print()
        {
            try
            {
                this.fill();
                Console.WriteLine("<----- TABLAS ----->");
                tablas.print();
                Console.WriteLine("<----- ATRIBUTOS ----->");
                atributos.print();
                Console.WriteLine("<----- RESTRICCIONES ----->");
                restricciones.print();
            }
            catch(Exception e)
            {
                Console.Write("Error");
            }
           

        }
    }
}
