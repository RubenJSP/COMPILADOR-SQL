using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace SQL_Escaner
{
    class Escaner
    {
        private string[] data;
        private int id, cons;
        private List<Token> error;
        private List<Token> tokens;
        public Escaner(string[] data)
        {
            this.data = data;
            this.id = 401;
            this.cons = 600;
            error = new List<Token>();
            tokens = new List<Token>();
        }


        //Aqui se formatean las cadenas de tipo 'palabra plabra'
        private void format()
        {
            string[] str;
            for (int line = 0; line < data.Length; line++)
            {
                str = Regex.Split(data[line], @"(\'.*?\')"); // Se hace split con cualquier palabra que esté entre comillas
                for (int word = 0; word < str.Length; word++)
                {
                    if (Regex.IsMatch(str[word], @"\'.*?\'"))  //Hace match con las palabras que tengan comillas
                    {

                        string newWord = str[word];
                        newWord = Regex.Replace(str[word], @"\s+", "¤"); //Se reemplazan los espacios por ese caracter
                        newWord = Regex.Replace(newWord, @"\'", ""); //eliminamos las comillas
                        data[line] = Regex.Replace(data[line], @"(?:^|\W)" + Regex.Escape(str[word]) + @"(?:$|\W)", "'•" + newWord + "'"); //formateamos el texto de salida en su 
                        //linea correspondiente 'palabra¤palabra'

                    }
                }

            }

        }


        public List<Token> output()
        {
            this.format();
            for (int i = 0; i < data.Length; i++)
            {
                string[] str = Regex.Split(data[i], @"(\s+|\,|\(|\)|\'|\;)"); //Hace split con esas coincidencias
                for (int word = 0; word < str.Length; word++)
                {
                    str[word] = Regex.Replace(str[word], @"\s+", "");

                    if (str[word] != "")
                    {

                        if (Regex.IsMatch(str[word], @"^[A-Za-z_]+\w*.")) //Si es de tipo ID.ID
                        {
                            string[] temp = Regex.Split(str[word], @"(\.)"); //Hace split en . para separa los identificadores
                            for (int j = 0; j < temp.Length; j++)
                            {
                                this.add(temp[j], i + 1); //Añade los tokens a la lista
                            }
                            continue;
                        }

                        this.add(str[word], i + 1); //Se añade el resto de tokens a la lista
                        if (error.Count > 0) return this.tokens; //Si existe un error en la lista de errores, entonces se regresa la lista de tokens y se detiene el análisis

                    }
                }
            }

            return this.tokens;
        }
        public void add(string word, int line) //Este metodo añade los tokens a la lista de tokens
        {
            Token token = this.typeOf(word, line);

            if (exist(word)) //verifica si el token ya existe en la lista
            {
                token = getExistent(word); //retorna el token existente en la lista
                token.addRef(line); //Se añade la línea a las referencis del token existente
                tokens.Add(new Token(word, token.Codigo, line, token.Tipo)); //Se crea un nuevo token con el mismo valor y linea del encontrado
            }
            else if (token != null) //En caso de que no exista el token
            {
                if (token.Tipo == 6) cons++; //Si es constante
                else if (token.Tipo == 4) id++; //si es identificador

                this.tokens.Add(token); //Se añade a la lista de tokens
            }
            else
            {
                addError(word, line);  //Aquí se añaden los errores en caso de existir alguno
            }
        }
        private bool exist(string str) //verifica  si un token existe
        {
            foreach (Token data in this.tokens)
            {
                if (data.Dato.Equals(str))
                    return true;
            }
            return false;
        }
        private Token getExistent(string str) //retorna un token existente en la lista
        {
            foreach (Token data in this.tokens)
            {
                if (data.Dato.Equals(str))
                    return data;
            }
            return null;
        }

        private void addError(string str, int line) //Añade los errores encontrados a la lista de errores
        {

            if (Regex.IsMatch(str, @"\w+"))//ERROR DE ELEMENTO
            {
                this.error.Add(new Token(str, 0, line, -1));

            }
            else
            {
                string[] chars = Regex.Split(str, @""); //ERROR DE SIMBOLO
                for (int i = 0; i < chars.Length; i++)
                {
                    if (chars[i] != "" && chars[i] != " ")
                        this.error.Add(new Token(chars[i], 0, line, -2));
                }
            }



        }

        public Token typeOf(string str, int line) //REGRESA LOS TOKENS SEGUN LA CADENA DE ENTRADA 
        {
            //Console.WriteLine("Match " + str);

            if (Regex.IsMatch(str.ToUpper(), @"^SELECT$")) { return new Token(str, 10, line, 1); } //PALABRAS RESERVADAS
            else if (Regex.IsMatch(str.ToUpper(), @"^FROM$")) { return new Token(str, 11, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^WHERE$")) { return new Token(str, 12, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^IN$")) { return new Token(str, 13, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^AND$")) { return new Token(str, 14, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^OR$")) { return new Token(str, 15, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^CREATE$")) { return new Token(str, 16, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^TABLE$")) { return new Token(str, 17, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^CHAR$")) { return new Token(str, 18, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^NUMERIC$")) { return new Token(str, 19, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^NOT$")) { return new Token(str, 20, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^NULL$")) { return new Token(str, 21, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^CONSTRAINT$")) { return new Token(str, 22, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^KEY$")) { return new Token(str, 23, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^PRIMARY$")) { return new Token(str, 24, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^FOREIGN$")) { return new Token(str, 25, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^REFERENCES$")) { return new Token(str, 26, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^INSERT$")) { return new Token(str, 27, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^INTO$")) { return new Token(str, 28, line, 1); }
            else if (Regex.IsMatch(str.ToUpper(), @"^VALUES$")) { return new Token(str, 23, line, 1); }
            else if (Regex.IsMatch(str, @"^\,$")) { return new Token(str, 50, line, 5); } //DELIMITADORES
            else if (Regex.IsMatch(str, @"^\.$")) { return new Token(str, 51, line, 5); }
            else if (Regex.IsMatch(str, @"^\($")) { return new Token(str, 52, line, 5); }
            else if (Regex.IsMatch(str, @"^\)$")) { return new Token(str, 53, line, 5); }
            else if (Regex.IsMatch(str, @"^\'$")) { return new Token("'", 54, line, 5); }
            else if (Regex.IsMatch(str, @";$")) { return new Token(";", 55, line, 5); }
            else if (Regex.IsMatch(str, @"^\+$")) { return new Token(str, 70, line, 7); } //OPERADORES
            else if (Regex.IsMatch(str, @"^\-$")) { return new Token(str, 71, line, 7); }
            else if (Regex.IsMatch(str, @"^\*$")) { return new Token(str, 72, line, 7); }
            else if (Regex.IsMatch(str, @"^\/$")) { return new Token(str, 73, line, 7); }
            else if (Regex.IsMatch(str, @"^\>$")) { return new Token(str, 81, line, 8); } //OPERADORES RELACIONALES
            else if (Regex.IsMatch(str, @"^\<$")) { return new Token(str, 82, line, 8); }
            else if (Regex.IsMatch(str, @"^\=$")) { return new Token(str, 83, line, 8); }
            else if (Regex.IsMatch(str, @"\>=")) { return new Token(str, 84, line, 8); }
            else if (Regex.IsMatch(str, @"\<=")) { return new Token(str, 85, line, 8); }
            else if (Regex.IsMatch(str, @"^\d*([\.\,])?\d+$|^\d+[\.\,]?\d+$")) { return new Token(str, cons, line, 6, 61); } //CONSTANTES NUMERICAS
            else if (Regex.IsMatch(str, @"^\•")) { str = Regex.Replace(str, @"\•", ""); return new Token(Regex.Replace(str, @"([¤])", " "), cons, line, 6, 62); } //CONSTANTES ALFANUMERICAS
            else if (Regex.IsMatch(str, @"^[A-Za-z _]\w*[#]?$")) { return new Token(str, id, line, 4); }  //IDENTIFICADORES
            //Regex.Replace(str, @"\'", "")
            return null;
        }

        public List<Token> errores() //retorna la lista de errores
        {
            return this.error;
        }
    }
}
