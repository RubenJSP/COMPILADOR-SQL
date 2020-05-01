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

        public Semantic(List<Token> tokens)
        {
            this.tokens = tokens;
            tablas = new Tabla();
            atributos = new Atributo();
            restricciones = new Restricciones();
        }
        private int findKey(int table, string atrib)
        {
            for (int i = 0; i < atributos.Nombre.Count; i++)
                if (atributos.Tabla[i] == table)
                    if (atributos.Nombre[i] == atrib)
                        return i;

            return -1;
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
                        restricciones.Atributo_asoc.Add(-1);
                        restricciones.Tabla.Add(-1);
                        restricciones.Tipo.Add(1);
                        restricciones.Atributo.Add(-1);

                    }
                    else
                    {
                        restricciones.Tipo.Add(2);
                        Console.WriteLine("AT: " + tokens[i + 5].Dato + " T: " + tablaPos);
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

        public void print()
        {
            this.fill();
            Console.WriteLine("<----- TABLAS ----->");
            tablas.print();
            Console.WriteLine("<----- ATRIBUTOS ----->");
            atributos.print();
            Console.WriteLine("<----- RESTRICCIONES ----->");
            restricciones.print();

        }
    }
}
