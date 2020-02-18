using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SQL_Escaner
{
    class Token
    {
        private string dato;
        private int codigo, linea, tipo, value;
        private List<int> referencias;
        public Token(string dato, int codigo, int linea, int tipo)
        {
            this.dato = dato;
            this.Codigo = codigo;
            this.linea = linea;
            this.tipo = tipo;
            this.referencias = new List<int>();
            referencias.Add(this.linea);
        }

        public Token(string dato, int codigo, int linea, int tipo, int value)
        {
            this.dato = dato;
            this.Codigo = codigo;
            this.linea = linea;
            this.tipo = tipo;
            this.Value = value;
            this.referencias = new List<int>();
            referencias.Add(this.linea);
        }


        public string Dato { get => dato; set => dato = value; }
        public int Codigo { get => codigo; set => codigo = value; }
        public int Linea { get => linea; set => linea = value; }
        public int Tipo { get => tipo; set => tipo = value; }
        public int Value { get => value; set => this.value = value; }

        public List<int> references()
        {
            List<int> lines =referencias.Distinct().ToList();
            lines.Sort();
            return lines;
        }
        public void addRef(int line)
        {
            this.referencias.Add(line);
        }
        
        public bool equal(Token token)
        {
 
            return this.dato.Equals(token.dato);
        }
    }
}
