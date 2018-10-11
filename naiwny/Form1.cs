using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace naiwny
{
    public partial class Form1 : Form
    {
        private int _x;
        private int _y;
        private bool _work = true;
        private static int _logikaSasiedztwa = 0;                        //von Neumann
        private static bool _logikaPeridyczne = false;                   //nieperiodyczne
        private static int _iloscStartowychZiaren = 0;                   
        private static int _ukladZiaren = 0;                             //losowe                           
        private Algorytmy _logic;
        private bool pause = false;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "300";
            textBox2.Text = "300";
            textBox3.Text = "10";
            comboBox1.Items.AddRange(new object[] {"von Neumann'a",
                        "Moore'a",
                        "Heksagonal left",
                        "Hexagonal rigth",
                        "Hexagonal random",
                        "Pentagonal random"});
            comboBox1.SelectedIndex = 0;
            comboBox2.Items.AddRange(new object[] {"Losowe",
                        "Równomierne",
                        "Losowe z promieniem R",
                        "Przez kliknięcie"});
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _x = Convert.ToInt32(textBox2.Text);
            _y = Convert.ToInt32(textBox1.Text);
            if (_x > 1000 || _y > 1000 || _x < 0 || _y < 0)
            {
                MessageBox.Show("Wymiary za duze", "Error!", MessageBoxButtons.OK);
            }
            else {
                pcb.BorderStyle = BorderStyle.FixedSingle;
                pcb.Height = _y;                                         //generowanie tablicy
                pcb.Width = _x;
            }
            _iloscStartowychZiaren = Convert.ToInt32(textBox3.Text);
            button2.Visible = true;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            _logic.Start();                                      //ustawia pierwsze ziarna
            _logic.UstawUkladZiaren(_ukladZiaren);                   //uklad ziaren
            _logic.UstawSasiedztwo(_logikaSasiedztwa);
            _logic.UstawPeriodyczne(_logikaPeridyczne);
            while (_work)
            {
                while (!pause)
                {
                    Draw();
                    if (_ukladZiaren == 3)
                    {
                        _logic.UstawSasiedztwo(_logikaSasiedztwa);
                        _logic.NextStep();
                    }
                    else
                    {
                        _logic.NextStep();
                    }
                    
                }

            }

        }

        private void pcb_MouseClick(object sender, MouseEventArgs e)
        {
            var x = e.Location.X;                                       //przez klikniecie
            var y = e.Location.Y;
            _logic.NoweZiarno(y, x);
        }

        private void button2_Click(object sender, EventArgs e) //start
        {
            SelectNeighbourhoodLogic();
            _logikaPeridyczne = checkBox1.Checked;
            SelectGrainType();
            _iloscStartowychZiaren = Convert.ToInt32(textBox3.Text);
            _logic = new Algorytmy(_x, _y, _iloscStartowychZiaren, _ukladZiaren, Convert.ToInt32(textBox4.Text));
            if (backgroundWorker1.IsBusy != true)
            {
                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                //backgroundWorker1.CancelAsync();
            }
        }

        private void SelectGrainType()
        {
            switch (comboBox2.SelectedItem.ToString())
            {
                case "Losowe":
                    _ukladZiaren = 0;
                    break;
                case "Równomierne":
                    _ukladZiaren = 1;
                    break;
                case "Losowe z promieniem R":
                    _ukladZiaren = 2;
                    break;
                case "Przez kliknięcie":
                    _ukladZiaren = 3;
                    break;
                default:
                    _ukladZiaren = 0;
                    break;
            }
        }

        private void SelectNeighbourhoodLogic()
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "von Neumann'a":
                    _logikaSasiedztwa = 1;
                    break;
                case "Moore'a":
                    _logikaSasiedztwa = 0;
                    break;
                case "Heksagonal left":
                    _logikaSasiedztwa = 2;
                    break;
                case "Hexagonal rigth":
                    _logikaSasiedztwa = 3;
                    break;
                case "Hexagonal random":
                    _logikaSasiedztwa = 4;
                    break;
                case "Pentagonal random":
                    _logikaSasiedztwa = 5;
                    break;
                default:
                    _logikaSasiedztwa = 1;
                    break;
            }
        }

        private void Draw()
        {
            var n = new Bitmap(_x, _y);
            for (int i = 0; i < _x; i++)
            {
                for (int j = 0; j < _y; j++)
                {
                    n.SetPixel(i, j, _logic.NowaMapa[i, j].DajKolor());
                }
            }
            pcb.Image = n;
        }


        private void button3_Click_1(object sender, EventArgs e)//pause
        {
            //_work = !_work;
            pause = !pause;
            if (pause)
                button3.Text = "Resume";
            else
                button3.Text = "Pause";
        }

        private void button4_Click(object sender, EventArgs e) //reset
        {
            _work = false;
            for (int i = 0; i < _x + 4; i++)
            {
                for (int j = 0; j < _y+4; j++)
                {
                    _logic.Mapa[i, j] = new Punkt();
                    _logic.NowaMapa[i, j] = new Punkt();
                }
            }
            _work = true;
            SelectNeighbourhoodLogic();

            _logikaPeridyczne = checkBox1.Checked;

            SelectGrainType();
            
            _iloscStartowychZiaren = Convert.ToInt32(textBox3.Text);

            _logic = new Algorytmy(_x, _y, _iloscStartowychZiaren, _ukladZiaren,Convert.ToInt32(textBox4.Text));
            _logic.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
