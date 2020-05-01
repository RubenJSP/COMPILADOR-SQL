using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQL_Escaner
{
    class Restricciones
    {
        private List<int> numeroTabla;
        private List<int> numero;
        private List<int> tipo;
        private List<string> nombre;
        private List<int> atributo_asoc;
        private List<int> tabla;
        private List<int> atributo;
        public Restricciones() {
            numeroTabla = new List<int>();
            numero = new List<int>();
            tipo = new List<int>();
            nombre = new List<string>();
            atributo_asoc = new List<int>();
            tabla = new List<int>();
            atributo = new List<int>();
        }

        public void print()
        {
           
            for (int i = 0; i < nombre.Count; i++)
            {
                Console.WriteLine("NT [" + numeroTabla[i] + "] # [" + i + "] Tipo [" + tipo[i] + "] Nombre ["  + nombre[i] +"] A.ASC [" + atributo_asoc[i]
                  + "] Tabla: [" + tabla[i] + "] A ["  + atributo[i] + "]");
            }
        }
        public List<int> NumeroTabla { get => numeroTabla; set => numeroTabla = value; }
        public List<int> Numero { get => numero; set => numero = value; }
        public List<int> Tipo { get => tipo; set => tipo = value; }
        public List<string> Nombre { get => nombre; set => nombre = value; }
        public List<int> Atributo_asoc { get => atributo_asoc; set => atributo_asoc = value; }
        public List<int> Tabla { get => tabla; set => tabla = value; }
        public List<int> Atributo { get => atributo; set => atributo = value; }
    }
}
