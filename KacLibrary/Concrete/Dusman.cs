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

namespace KacLibrary.Concrete
{
    internal class Dusman : Cisim
    {
        public Dusman(int height, Size hareketAlaniBoyutlari) : base(hareketAlaniBoyutlari)
        {
            BringToFront();
            Image = Image.FromFile("Resimler\\sasuke.png");
            BackColor = Color.Transparent;

            Left = 14 * 125;
            Top = 90;
        }

      
    }
}
