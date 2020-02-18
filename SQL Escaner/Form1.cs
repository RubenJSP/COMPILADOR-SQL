﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            this.richTextBox1.Text += " ";

            foreach (DataGridViewColumn col in dgvTokens.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
                col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            foreach (DataGridViewColumn col in gridCons.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            foreach (DataGridViewColumn col in gridId.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            foreach (DataGridViewColumn col in gridErr.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;

                col.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            }

            gridId.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridErr.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gridCons.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvTokens.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

        }

        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string[] datos = richTextBox1.Lines;
            Escaner scan = new Escaner(datos);
            List<Token> words = scan.output();

            foreach (Token token in words)
            {

                if (token.Tipo == 1)
                {
                    Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(40, 51, 250));
                }
                else if (token.Tipo == 4)
                {
                    Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(88, 199, 158));
                }
                else if (token.Value >= 61 || token.Codigo == 54)
                {
                    Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(219, 206, 13));

                }
                else
                {
                    Utility.HighlightText(richTextBox1, token.Dato, System.Drawing.Color.FromArgb(222, 222, 222));

                }
            }
        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {

        }

        public bool existInGrid(DataGridView dat, string value)
        {
            foreach (DataGridView row  in dat.Rows)
            {

            }

            return false;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            dgvTokens.Rows.Clear();
            gridCons.Rows.Clear();
            gridErr.Rows.Clear();
            gridId.Rows.Clear();

            int number = 0;
            string[] datos = richTextBox1.Lines;
            Escaner scan = new Escaner(datos);
            tokens = scan.output();
            foreach (Token token in tokens)
            {

                number++;
                string dato = token.Dato;
                if (token.Tipo == 4)
                {
                    string refs = "";

                    foreach (int lines in token.references())
                    {
                        refs +=lines + ",";
                    }
                    refs = refs.Remove(refs.Length - 1);
                        gridId.Rows.Add(token.Dato, token.Codigo,refs);

                }else if (token.Value>=61)
                {
                    dato = "CONSTANTE";
                    gridCons.Rows.Add(number,token.Dato,token.Value,token.Codigo);

                }
                
                dgvTokens.Rows.Add(number, token.Linea, dato, token.Tipo, token.Codigo);

            }
            if (scan.errores().Count() < 1)
            {
                gridErr.Rows.Add("", "", "Sin error");
            }
            else
                foreach (Token error in scan.errores())
                {
                    if (error.Tipo == -1)
                        gridErr.Rows.Add(1, 101, "Error en línea " + error.Linea + " Símbolo desconocido: " + error.Dato);
                    else
                        gridErr.Rows.Add(1, 102, "Error en línea " + error.Linea + "Elemento inválido: " + error.Dato);

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
            myRtb.SelectionColor = Color.Black;
        }
    }
}
