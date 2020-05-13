using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SQL_Escaner
{
    class Semantic
    {
        public struct Alias
        {
            public string alias;
            public string table;
            public Alias(string a, string t)
            {
                this.alias = a;
                this.table = t;
            }
        }
        public struct Atrib
        {
            public string atributo;
            public string tipo;
            public Atrib(string atributo, string tipo)
            {
                this.atributo = atributo;
                this.tipo = tipo;
            }
        }
        private Tabla tablas;
        private Atributo atributos;
        private Restricciones restricciones;
        private List<Token> tokens;
        public List<Token> dml;
        public List<Atrib> comparar;
        public List<Alias> aliases;
        public List<Token> atrib, suAtrib, tables, suTable, exist;
        List<string> data;
        public bool isComp, subQuery;
        public List<Token> errores;
        public Semantic(List<Token> tokens)
        {
            this.tokens = tokens;
            tablas = new Tabla();
            atributos = new Atributo();
            restricciones = new Restricciones();
            atrib = new List<Token>();
            tables = new List<Token>();
            comparar = new List<Atrib>();
            aliases = new List<Alias>();
            exist = new List<Token>();
            suAtrib = new List<Token>();
            suTable = new List<Token>();
            data = new List<string>();
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
                exist.Clear();
                isComp = false;
                data.Clear();
            }
            if (x == 12)
            {
                isComp = true;
            }
            if (x == 13)
            {
                suTable.Clear();
                subQuery = true;
                isComp = false;
            }
            if (x == 53)
                subQuery = false;

        }
        public void fill()
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                //Tablas
                if (tokens[i].Codigo == 17)
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
                    atributos.Longitud.Add(int.Parse(tokens[i + 2].Dato));
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
                        //Console.WriteLine("T: " + tablaPos + " AT: " + tokens[i + 5].Dato + " F: " + findKey(tablaPos, tokens[i + 5].Dato));
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
                if (tokens[i].Codigo == 26)
                {
                    tablaPos = tablas.Nombre.IndexOf(tokens[i + 1].Dato);
                    restricciones.Tabla.Add(tablaPos);
                    restricciones.Atributo.Add(findKey(tablaPos, tokens[i + 3].Dato));
                }
            }
        }

        public Token existAtrib(Token item)
        {
            Token dot = dml[dml.IndexOf(item) + 1];
            if (!isComp)
            {
                if (atributos.Nombre.Contains(item.Dato))
                {

                    if (atributos.Nombre.Contains(item.Dato))
                    {
                        atrib.Add(item);
                    }

                    return null;
                }
                else
                {
                    return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
                }
            }
            else if(dot.Dato != ".")
            {
                if (!subQuery)
                {

                    return atribInTable(item,tables);
                }
                else
                {
                    return atribInTable(item,suTable);
                }

            }
            return null;
        }

        public Token atribInTable(Token atributo, List<Token> tables)
        {
            int tableIndex = tablas.Nombre.IndexOf(tables[0].Dato);
            if (findKey(tableIndex, atributo.Dato) != -1)
                return null;

            return new Token("El nombre del atributo '" + atributo.Dato + "' no es válido", 311, atributo.Linea, 3);
        }
        public Token validarAtributo(Token item)
        {

            return existAtrib(item);
        }

        public Token validarTabla(Token item)
        {
            if (tablas.Nombre.Contains(item.Dato.ToUpper()))
            {
                if (subQuery)
                    suTable.Add(item);
                else
                    tables.Add(item);

                return ambiguedad(item);
            }
            else
            {
                return new Token("El nombre de la tabla '" + item.Dato + "' no es válido", 314, item.Linea, 3);
            }

        }
        private bool tableIsDeclared(Token table)
        {
            foreach (Token t in tables)
            {
                if (t.Dato.Equals(table.Dato))
                    return true;
            }

            return false;
        }
        public Token validaTablaAtributo(Token item)
        {
            Token table = dml[dml.IndexOf(item) - 2];
            string tableName = tableAlias(table);
            if (tableName != null)
                table.Dato = tableName;
            if ((tablas.Nombre.Contains(table.Dato)) && tableIsDeclared(table))
            {
                int tableIndex = tablas.Nombre.IndexOf(table.Dato);
                if (findKey(tableIndex, item.Dato.ToUpper()) != -1)
                {
                    return null;
                }

                return new Token("El nombre del atributo '" + item.Dato + "' no es válido", 311, item.Linea, 3);
            }
            else
            {
                return new Token("El identificador '" + table.Dato + "." + item.Dato + "' no es válido", 315, item.Linea, 3);
            }

        }
        public string tableAlias(Token alias)
        {
            foreach (Alias a in aliases)
                if (alias.Dato.Equals(a.alias))
                    return a.table;

            return null;

        }
        public string getType(Token table,Token item)
        {
            int tableIndex = tablas.Nombre.IndexOf(table.Dato);
            int itemIndex = findKey(tableIndex,item.Dato);
            if (itemIndex != -1)
                return (atributos.Tipo[itemIndex].Equals("NUMERIC")) ? "INT":"CHAR";

            return null;
        }

        public Token compararTipos(Token item)
        {
            bool isRightTable = (tablas.Nombre.Contains(dml[dml.IndexOf(item) + 1].Dato)) ? true : false;
            Token table = dml[dml.IndexOf(item) + 1];
            Token right = (tablas.Nombre.Contains(table.Dato)) ? dml[dml.IndexOf(item) + 3] : table;
            Token left = dml[dml.IndexOf(item) - 1];
            Token leftTable = null;
            if (!subQuery)
                leftTable = (tables.Count < 2) ? tables[0]: dml[dml.IndexOf(item) - 3];
            else
                leftTable = (suTable.Count < 2) ? suTable[0] : dml[dml.IndexOf(item) - 3];

            string leftType = getType(leftTable, left);
            string rightType = null;
            if (isRightTable)
            {
                rightType = getType(dml[dml.IndexOf(item) + 1], right);
                if (rightType == null)
                    return null;
            }
            else
            {
                if (right.Codigo == 54)
                    rightType = "CHAR";
                else if (right.Codigo == 61)
                    rightType = "INT";
                else
                    return null;
            }
            if (!leftType.Equals(rightType))
                return new Token("Error de conversión al convertir el valor del atributo " +  left.Dato + " del tipo " +  leftType  + " a tipo de dato " + rightType,313,left.Linea,3);
            
            return null;
        }
        public Token ambiguedad(Token tabla)
        {
            if (!subQuery)
            {
                int tableIndex = tablas.Nombre.IndexOf(tabla.Dato);
                foreach (Token a in atrib)
                {
                    if (findKey(tableIndex, a.Dato.ToUpper()) != -1)
                    {
                        if (!data.Contains(a.Dato.ToUpper()))
                            data.Add(a.Dato.ToUpper());
                        else
                            return new Token("El nombre del atributo '" + a.Dato + "' es ambigüo", 312, a.Linea, 3);
                    }
                }
            }

            return null;
        }
        public void alias(Token token)
        {
            Token table = dml[dml.IndexOf(token) - 1];
            aliases.Add(new Alias(token.Dato, table.Dato));
        }
        public Token analyze(int rule, Token token)
        {
            switch (rule)
            {
                case 700:
                    return validarAtributo(token);
                case 701:
                    return validarTabla(token);
                case 702:
                    alias(token);
                    return null;
                case 703:
                    return validaTablaAtributo(token);
                case 704:
                    return compararTipos(token);
                default:
                    return null;
            }

        }
        public void print()
        {
            try
            {
                Console.WriteLine("<----- TABLAS ----->");
                tablas.print();
                Console.WriteLine("<----- ATRIBUTOS ----->");
                atributos.print();
                Console.WriteLine("<----- RESTRICCIONES ----->");
                restricciones.print();
            }
            catch
            {
            }


        }
    }
}
