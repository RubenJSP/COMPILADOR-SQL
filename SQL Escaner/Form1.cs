using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace SQL_Escaner
{
    
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        Semantic tablasSemanticas;
        List<Token> tokens;
        string conection = "Data Source=GECKO;Initial Catalog=INSCRITOS;Integrated Security=True";
        string tablas = @"CREATE TABLE DEPARTAMENTOS(
                            D# CHAR(2) NOT NULL,
                            DNOMBRE CHAR(6) NOT NULL
                            CONSTRAINT PK_DEPARTAMENTOS PRIMARY KEY(D#));
                            CREATE TABLE CARRERAS(
                            C# CHAR(2) NOT NULL,
                            CNOMBRE CHAR(3) NOT NULL,
                            VIGENCIA CHAR(4) NOT NULL,
                            SEMESTRES NUMERIC(2) NOT NULL,
                            D# CHAR(2) NOT NULL,
                            CONSTRAINT PK_CARRERAS PRIMARY KEY(C#),
                            CONSTRAINT FK_CARRERAS FOREIGN KEY (D#) REFERENCES DEPARTAMENTOS(D#));
                            CREATE TABLE ALUMNOS(
                            A# CHAR(2) NOT NULL,
                            ANOMBRE CHAR(20) NOT NULL,
                            GENERACION CHAR(4) NOT NULL,
                            SEXO CHAR(1) NOT NULL,
                            C# CHAR(2) NOT NULL,
                            CONSTRAINT PK_ALUMNOS PRIMARY KEY(A#),
                            CONSTRAINT FK_ALUMNOS FOREIGN KEY(C#)REFERENCES CARRERAS(C#));
                            CREATE TABLE MATERIAS(
                            M# CHAR(2) NOT NULL,
                            MNOMBRE CHAR(6) NOT NULL,
                            CREDITOS NUMERIC(2) NOT NULL,
                            C# CHAR(2) NOT NULL,
                            CONSTRAINT PK_MATERIAS PRIMARY KEY(M#),
                            CONSTRAINT FK_MATERIAS FOREIGN KEY (C#) REFERENCES CARRERAS(C#));
                            CREATE TABLE PROFESORES(
                            P# CHAR(2) NOT NULL,
                            PNOMBRE CHAR(20) NOT NULL,
                            EDAD NUMERIC(2) NOT NULL,
                            SEXO CHAR(1)NOT NULL,
                            ESP CHAR(4) NOT NULL,
                            GRADO CHAR(3) NOT NULL,
                            D# CHAR(2) NOT NULL,
                            CONSTRAINT PK_PROFESORES PRIMARY KEY(P#),
                            CONSTRAINT FK_PROFESORES FOREIGN KEY (D#) REFERENCES DEPARTAMENTOS(D#));
                            CREATE TABLE INSCRITOS(
                            R# CHAR(3) NOT NULL,
                            A# CHAR(2) NOT NULL,
                            M# CHAR(2) NOT NULL,
                            P# CHAR(2) NOT NULL,
                            TURNO CHAR(1) NOT NULL,
                            SEMESTRE CHAR(6) NOT NULL,
                            CALIFICACION NUMERIC(3) NOT NULL,
                            CONSTRAINT PK_INSCRITOS PRIMARY KEY(R#),
                            CONSTRAINT FK_INSCRITOS_01 FOREIGN KEY (A#) REFERENCES ALUMNOS(A#),
                            CONSTRAINT FK_INSCRITOS_02 FOREIGN KEY (M#) REFERENCES MATERIAS(M#),
                            CONSTRAINT FK_INSCRITOS_03 FOREIGN KEY (P#) REFERENCES PROFESORES(P#));";
        public Form1()
        {

            InitializeComponent();
            this.StyleManager = styleMang;
            styleMang.Theme = MetroFramework.MetroThemeStyle.Dark;
          
        }

        public void load()
        {
            string[] datos = tablas.Split();
            Escaner data = new Escaner(datos);
            List<Token> items = new List<Token>();
            items = data.output();
            tablasSemanticas = new Semantic(items);
            tablasSemanticas.fill();
            //tablasSemanticas.print();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Load(object sender, EventArgs e)
        {
           // this.richTextBox1.Text += "";

          
            foreach (DataGridViewColumn col in gridErr.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            }

            gridErr.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

        }

        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

          

        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            load();
            gridErr.Rows.Clear();
            string[] datos = richTextBox1.Lines;
            Escaner scan = new Escaner(datos);
            tokens = scan.output();
            tablasSemanticas.dml = tokens;
            /*foreach (Token i in tokens)
            {
                Console.WriteLine(i.Dato + " " + i.Codigo);

            }*/
            Parser parser = new Parser(datos);
            parser.scan = scan;
            parser.tokens = tokens;
            parser.semantica = tablasSemanticas;
            //Console.WriteLine((parser.analyze())? "Todo salió bien :) TQM":"El Query está mal, como todo en tu vida");


            if (parser.analyze())
            {
                gridErr.Rows.Add("1", "100", "Sin error");
                gridErr.Rows.Add("2", "200", "Sin error");
                gridErr.Rows.Add("3", "300", "Sin error");
                /*SqlConnection conectar = new SqlConnection(conection);
                conectar.Open();
                if(conectar.State == System.Data.ConnectionState.Open)
                {
                    try
                    {
                        string query = richTextBox1.Text;
                        SqlCommand cmd = new SqlCommand(query, conectar);
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Listo");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);

                    }
                }*/

            }
            else
            {

                if (parser.scan.errores().Count > 0)
                    foreach (Token error in parser.scan.errores())
                    {
                        if (error.Tipo == -1)
                            gridErr.Rows.Add(1, 102, "Error en línea " + error.Linea + " Elemento inválido: " + error.Dato);
                        else
                            gridErr.Rows.Add(1, 101, "Error en línea " + error.Linea + " Símbolo desconocido: " + error.Dato);

                    }
                else
                {
                    gridErr.Rows.Add("1", "100", "Sin error");
                    foreach (Token error in parser.errores)
                    {
                        if (error.Codigo > 300)
                        {
                            gridErr.Rows.Add("2", "200", "Sin error");
                            gridErr.Rows.Add("3", error.Codigo, "Error en línea: " + error.Linea + " " + error.Dato);
                        }
                        else
                        {
                            gridErr.Rows.Add("2", error.Codigo, "Error en línea: " + error.Linea + " " + error.Dato);

                        }

                    }
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            gridErr.Rows.Clear();

            richTextBox1.Text = "";
        }

        private void checkBox1_MouseClick(object sender, MouseEventArgs e)
        {
           
            
        }

    }
    static class Utility
    {
        public static void HighlightText(this RichTextBox myRtb, string word, Color color)
        {

            if (word == string.Empty)
                return;

            int s_start = myRtb.SelectionStart, startIndex = 0, index;

            while ((index = myRtb.Text.IndexOf(word, startIndex)) != -1)
            {
                myRtb.Select(index, word.Length);
                myRtb.SelectionColor = color;

                startIndex = index + word.Length;
            }

            myRtb.SelectionStart = s_start;
            myRtb.SelectionLength = 0;
            //myRtb.SelectionColor = Color.Black;
        }
    }
}
