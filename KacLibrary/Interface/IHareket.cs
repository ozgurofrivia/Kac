/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/


using KacLibrary.Enum;
using System.Drawing;

namespace KacLibrary.Interface
{
    internal interface IHareket
    {

        Size HareketAlaniBoyutlari {  get; }

        int HareketMesafesi { get; }

        /// <summary>
        /// Cismi hareket ettirir
        /// </summary>
        /// <param name="yon">Hangi yöne hareket edileceği</param>
        /// <returns>Cisim duvara çarparsa true döndürür</returns>
        bool HareketEt(Yon yon);


    }
}
