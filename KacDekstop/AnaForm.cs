/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/


using KacLibrary.Concrete;
using KacLibrary.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KacDekstop
{
    public partial class AnaForm : Form
    {
        private readonly Oyun oyun;

        public string oyuncuAd;

        public AnaForm()
        {
            InitializeComponent();
         
            oyun = new Oyun(oyunpanel,canlabel,levellabel,puanlabel);
            oyun.GecenSureDegisti += Oyun_GecenSureDegisti;
            oyunpanel.BackColor = Color.Transparent;
            

        }

        private void AnaForm_Load(object sender, EventArgs e)
        {
            oyun.Baslat();
            oyunculabel.Text = oyuncuAd;

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void AnaForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case Keys.Right:
                    oyun.KarakteriHareketEttir(Yon.Sag);
                    break;
                case Keys.Left:
                    oyun.KarakteriHareketEttir(Yon.Sol);
                    break;
                case Keys.Up:
                    oyun.KarakteriHareketEttir(Yon.Yukari);
                    break;
                case Keys.Down:
                    oyun.KarakteriHareketEttir(Yon.Asagi);
                    break;
                case Keys.P:
                    oyun.Durdur();
                    break;
            }
        }

        private void Oyun_GecenSureDegisti(object sender, EventArgs e) 
        {
            surelabel.Text = $"{oyun.GecenSure.Seconds}";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
    }
}
