using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RecognizeLibrary;

namespace RecognationTest
{
    public partial class Form1 : Form
    {
        RecognizeLibrary.Recognator R;
        public Form1()
        {
            R = new Recognator("C:\\Program Files (x86)\\Tesseract-OCR\\tessdata", "rus");
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Bitmap B = new Bitmap(openFileDialog1.FileName);

                R.SetImage(openFileDialog1.FileName);
                List<TextBlock> Blocs = R.GetBlocks();
                Graphics G = Graphics.FromImage(B);
                for (int i = 0; i < Blocs.Count; i++)
                {
                    G.DrawRectangle(Pens.Black, Blocs[i].left, Blocs[i].top, Blocs[i].right - Blocs[i].left, Blocs[i].bottom - Blocs[i].top);
                    textBox1.Text = textBox1.Text + R.GetBlockText(Blocs[i]);
                }

                pictureBox1.Image = B;

                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;


                
            }
        }
    }
}
