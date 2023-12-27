using KacLibrary.Abstract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace KacLibrary.Concrete
{
    internal class Bomba : Cisim
    {
        public Bomba(int height, Size hareketAlaniBoyutlari) : base(hareketAlaniBoyutlari)
        {
            BringToFront();
            Image = Image.FromFile("Resimler\\shuriken.png");
            BackColor = Color.Transparent;
        }
    }
}
