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

namespace SQL_Escaner
{
    
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        List<Token> tokens;
        public Form1()
        {

            InitializeComponent();
            this.StyleManager = styleMang;
            styleMang.Theme = MetroFramework.MetroThemeStyle.Dark;
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

            if (checkBox1.Checked)
            {
                string[] datos = richTextBox1.Lines;
                Escaner scan = new Escaner(datos);
                List<Token> words = scan.output();

                foreach (Token token in words)
                {



                    if (token.Value >= 61 || token.Dato == "'")
                    {
                        Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(219, 206, 13));

                    }
                    else if (token.Tipo == 4)
                    {
                        Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(88, 199, 158));
                    }
                    else if (token.Tipo == 1)
                    {
                        Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(40, 51, 250));
                    }
                    else
                    {
                        Console.WriteLine(token.Dato);
                        Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(222, 222, 222));

                    }
                }
                foreach (Token error in scan.errores())
                {
                    Utility.HighlightText(richTextBox1, error.Dato, Color.Red);

                }
            }
            else
            {
               
                    
            }

        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            gridErr.Rows.Clear();
            string[] datos = richTextBox1.Lines;
            Escaner scan = new Escaner(datos);
            tokens = scan.output();
            foreach (Token i in tokens)
            {
                Console.WriteLine(i.Dato);

            }
             Parser parser = new Parser(datos);
            //Console.WriteLine((parser.analyze())? "Todo salió bien :) TQM":"El Query está mal, como todo en tu vida");


            if (parser.analyze())
            {
                gridErr.Rows.Add("", "100", "Sin error");
                gridErr.Rows.Add("", "200", "Sin error");
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
                    gridErr.Rows.Add("", "100", "Sin error");
                    foreach (Token error in parser.errores)
                    {

                        gridErr.Rows.Add("2", error.Codigo, "Error en línea: " + error.Linea + " " + error.Dato);

                    }
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            gridErr.Rows.Clear();

            richTextBox1.Text = "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            

        }

        private void checkBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!checkBox1.Checked)
            {
                richTextBox1.Select(0, richTextBox1.Text.Length);
                richTextBox1.SelectionColor = System.Drawing.Color.FromArgb(222, 222, 222);
            }
            else
            {
                richTextBox1.Text += "";
            }
            
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
