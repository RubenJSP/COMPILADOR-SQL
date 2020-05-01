using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Escaner
{
    class Atributo
    {
        private List<int> tabla;
        private List<string> nombre;
        private List<string> tipo;
        private List<int> longitud;
        private List<bool> not_null;
        public Atributo()
        {
            tabla = new List<int>();
            nombre = new List<string>();
            tipo = new List<string>();
            longitud = new List<int>();
            not_null = new List<bool>();
        }
        public void print()
        {
            for (int i = 0; i < nombre.Count; i++)
            {
                Console.WriteLine("T#[" + tabla[i] + "] # [" + i + "] Nombre [" + nombre[i] + "] Tipo [" + tipo[i] + "] Long [" + longitud[i] + "] NO NULL [" + not_null[i] + "]");
            }
        }
        public List<int> Tabla { get => tabla; set => tabla = value; }
        public List<string> Nombre { get => nombre; set => nombre = value; }
        public List<int> Longitud { get => longitud; set => longitud = value; }
        public List<string> Tipo { get => tipo; set => tipo = value; }
        public List<bool> Not_null { get => not_null; set => not_null = value; }
    }
}
