/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/


using KacLibrary.Enum;
using KacLibrary.Interface;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace KacLibrary.Abstract
{
    internal abstract class Cisim : PictureBox, IHareket
    {
        public Size HareketAlaniBoyutlari { get; }

        public int HareketMesafesi { get; protected set; }

        protected Cisim(Size hareketAlaniBoyutlari)
        {
            HareketAlaniBoyutlari = hareketAlaniBoyutlari;
            Size = new Size(125, 125);
            SizeMode = PictureBoxSizeMode.StretchImage;
        }



        public bool HareketEt(Yon yon)
        {
            switch (yon)
            {
                case Yon.Sag:
                    return SagaHareketEttir();
                    break;
                case Yon.Sol:
                    return SolaHareketEttir();
                    break;
                case Yon.Asagi:
                    return AsagiHareketEttir();
                    break;
                case Yon.Yukari:
                    return YukariHareketEttir();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(yon), yon, null);
            }
        }

        private bool YukariHareketEttir()
        {
            if(Top == 90) { return true; }
            else 
            {
                var yeniTop = Top - HareketMesafesi;
                var tasacakMi = yeniTop < 90;
                Top = tasacakMi ? 90 : yeniTop;

                return Top == 90;
            }
        }

        private bool AsagiHareketEttir()
        {
            if(Bottom == HareketAlaniBoyutlari.Width - (HareketAlaniBoyutlari.Width - (6 * 125 + 90))) { return true; }

            else 
            {
                var yeniBottom = Bottom + HareketMesafesi;
                var tasacakMi = Bottom > HareketAlaniBoyutlari.Width - (HareketAlaniBoyutlari.Width - (6 * 125 + 90));
                var bottom = tasacakMi ? HareketAlaniBoyutlari.Width - (HareketAlaniBoyutlari.Width - (6 * 125 + 90)) : yeniBottom;
                Top = bottom - Height;
                return Bottom == HareketAlaniBoyutlari.Width - (HareketAlaniBoyutlari.Width - (6 * 125 + 90));
            }

        }

        private bool SolaHareketEttir()
        {
            if (Left == 0) { return true; }

            else
            {
                var yeniLeft = Left - HareketMesafesi;
                var tasacakMi = yeniLeft < 0;
                Left = tasacakMi ? 0 : yeniLeft;
                return Left == 0;
            }
        }

        private bool SagaHareketEttir() 
        {
            if (Right == HareketAlaniBoyutlari.Width-45) { return true; }
            else
            {
                var yeniRight = Right + HareketMesafesi;
                var tasacakMi = yeniRight > HareketAlaniBoyutlari.Width-45;
                var right = tasacakMi ? HareketAlaniBoyutlari.Width-45: yeniRight;
                Left = right - Width;

                return Right == HareketAlaniBoyutlari.Width - 45;
            }
        }
    }
}
