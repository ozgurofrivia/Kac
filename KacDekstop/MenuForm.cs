using KacLibrary.Concrete;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace KacDekstop
{
    public partial class MenuForm : Form
    {

        
        


        public MenuForm()
        {
            InitializeComponent();
        }

        

        private void MenuForm_Load(object sender, EventArgs e)
        {
           
        }

        private void startbutton_Click(object sender, EventArgs e)
        {
            AnaForm anaForm = new AnaForm();
            anaForm.oyuncuAd = textBox1.Text;
            anaForm.Show();
           
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TusForm tusForm = new TusForm();
            tusForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SkorForm skorForm = new SkorForm();
            skorForm.Show();
        }
    }
}
