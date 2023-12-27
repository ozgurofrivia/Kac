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
    internal class Karakter : Cisim
    {
        public Karakter(int panelBoy, Size hareketAlaniBoyutlari) : base(hareketAlaniBoyutlari)
        {
            Image = Image.FromFile("Resimler\\naruto3.png");
            HareketMesafesi = Width;
            Top = 340;
            BringToFront();

        }
    }
}
