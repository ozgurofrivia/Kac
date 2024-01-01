/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/



using KacLibrary.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace KacLibrary.Concrete
{
    internal class Finish : Cisim
    {
        public Finish(int height, Size hareketAlaniBoyutlari) : base(hareketAlaniBoyutlari)
        {
         BringToFront();
            Image = Image.FromFile("Resimler\\finish.png");
            BackColor = Color.Transparent;
            Width = 125;
            Height = 6 * 125;
            Top = 90;
            Left = 14 * 125;
        }
    }
}
