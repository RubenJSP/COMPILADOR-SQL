using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Escaner
{
    class Tabla
    {
        private List<string> nombre;
        private List<int> atributos;
        private List<int> restricciones;
       public Tabla()
        {
            nombre = new List<string>();
            atributos = new List<int>();
            restricciones = new List<int>();
        }

        public List<int> Atributos { get => atributos; set => atributos = value; }
        public List<int> Restricciones { get => restricciones; set => restricciones = value; }
        public List<string> Nombre { get => nombre; set => nombre = value; }

        public void print()
        {
            for (int i = 0; i < nombre.Count; i++)
            {
                Console.WriteLine("#[" + i + "] Nombre [" + nombre[i] + "] Atributos [" + atributos[i] + "] Restricciones [" + restricciones[i]+"]");
            }
        }
    }
}
