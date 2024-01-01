/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/



using KacLibrary.Enum;
using System;

namespace KacLibrary.Interface
{
    internal interface IOyun
    {

        event EventHandler GecenSureDegisti;

        bool DevamEdiyorMu { get; }
        TimeSpan GecenSure { get; }

        void Baslat();
        void Durdur();
        void KarakteriHareketEttir(Yon yon);
        
    }
}
