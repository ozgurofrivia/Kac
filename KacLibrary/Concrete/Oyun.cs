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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace KacLibrary.Concrete
{
    public class Oyun : IOyun
    {

        #region Alanlar
        private readonly Timer sandikOlusturmaTimer = new Timer { Interval = 5000 };
        private readonly Timer gecenSureTimer = new Timer { Interval = 1000 };
        private readonly Timer bombaOlusturmaTimer = new Timer { Interval = 3000 };

        


        private TimeSpan gecenSure;
        private readonly Panel OyunPanel;
        private readonly Label PuanLabel, CanLabel, SeviyeLabel;

        private Karakter karakter;
        private Zemin zemin;
        private Finish finish;
        private Sandık sandık;
        private Bomba bomba;
        private Dusman dusman;

        int can = 1000;
        int seviye = 1;
        int puan = 0;


        Random random = new Random();

        #endregion

        #region Olaylar

        public event EventHandler GecenSureDegisti;

        #endregion

        #region Özellikler

        public bool DevamEdiyorMu { get; private set; }
        public TimeSpan GecenSure
        {   get => gecenSure; 
            
            private set 
            {
                gecenSure = value;
                GecenSureDegisti?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Metotlar

        public Oyun(Panel oyunPanel, Label canLabel, Label seviyeLabel, Label puanLabel)
        {
            CanLabel = canLabel;
            SeviyeLabel = seviyeLabel;
            PuanLabel = puanLabel; 
            OyunPanel = oyunPanel;

            bombaOlusturmaTimer.Tick += BombaOlusturmaTimer_Tick;
            gecenSureTimer.Tick += GecenSureTimer_Tick;
            sandikOlusturmaTimer.Tick += SandikOlusturmaTimer_Tick;
            sandikOlusturmaTimer.Start();
        }

        private void SandikOlusturmaTimer_Tick(object sender, EventArgs e)
        {
            SandıkOlustur();
        }

        private void GecenSureTimer_Tick(object sender, EventArgs e) 
        {
            GecenSure += TimeSpan.FromSeconds(1);
        }
        private void BombaOlusturmaTimer_Tick(object sender, EventArgs e)
        {
            BombaOlustur();
        }



        public void Baslat()//Oyunu Baslat
        {
            if (DevamEdiyorMu) 
            {
                return;
            }
            else 
            {
                KarakterOlustur();
                ZeminOlustur();
                TuzakOlustur();


                gecenSureTimer.Start();
                sandikOlusturmaTimer.Start();
                DevamEdiyorMu = true;
                CanLabel.Text = can.ToString();
                SeviyeLabel.Text = seviye.ToString();
                PuanLabel.Text = puan.ToString();
                PuanLabel.Visible = false;
            }
        }

       
        private void Bitir()  //Oyunu bitirme
        {
            if (!DevamEdiyorMu)
            {
                return;
            }
            else
            {
                sandikOlusturmaTimer.Stop();
                gecenSureTimer.Stop();
                DevamEdiyorMu = false;
            }
        }

      
        public void Durdur()   //P tuşuna basıldığında oyununu durduracak gerekli fonksiyon
        {
            if (DevamEdiyorMu)
            {
                sandikOlusturmaTimer.Stop();
                gecenSureTimer.Stop();
                DevamEdiyorMu = false;
                
            }
            else
            {
                sandikOlusturmaTimer.Stop();
                gecenSureTimer.Start();
                DevamEdiyorMu = true;
                return;
            }
        }

        private void KarakterOlustur()  //Karakterimi oluşturuyorum 
        {
            karakter = new Karakter(OyunPanel.Height, OyunPanel.Size);

            OyunPanel.Controls.Add(karakter);
        }

        
       

        public void KarakteriHareketEttir(Yon yon)
        {
            //P tuşuna basıldığında karakterin hareketini engellemek için koyulan koşul
            if (DevamEdiyorMu) 
            {
                karakter.HareketEt(yon);

                KarakterTuzakKontrol();
                KarakterSandikKontrol();
                KarakterBombaKontrol();
                FinisheGeldiMi();
            }
            else 
            {
                return;
            }
            
        }

        private void ZeminOlustur()
        {
            int top = 90;
            int left = 125;

            FinishOlustur();


            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    Zemin zemin = new Zemin(OyunPanel.Height, OyunPanel.Size);

                    zemin.Left = left;
                    zemin.Top = top;
                    
                    OyunPanel.Controls.Add(zemin);
                    zemin.SendToBack();
                    left += 125;
                }
                top += 125;
                left = 125;
            }
        }


        private void TuzakOlustur()
        {
            int top = 90;
            int left = 125;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (random.Next(10) < 2) // Yüzde 20 olasılıkla tuzak ekler
                    {
                        Tuzak tuzak = new Tuzak(OyunPanel.Height, OyunPanel.Size);
                        tuzak.Left = left;
                        tuzak.Top = top;
                        tuzak.Visible = false;
                        OyunPanel.Controls.Add(tuzak);
                        tuzak.BringToFront();
                    }
                    left += 125;
                }
                top += 125;
                left = 125;
            }
        }


        private void KarakterTuzakKontrol() //Karakterin tuzakla etkileşimini kontrol eden bir fonksiyon
        {
            // Karakterin konumu ile tuzakların konumunu kontrol et
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Tuzak && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    //Karakter tuzağa denk geldiyse canını azalt
                    Tuzak tuzak = control as Tuzak;
                    tuzak.Visible = true;
                    CanAzalt();
                }
            }
        }


        private void BombaOlustur()
        {
            // Mevcut bombaları temizle
            foreach (Control control in OyunPanel.Controls.OfType<Bomba>().ToList())
            {
                OyunPanel.Controls.Remove(control);
            }

            // Rastgele 10 zemin seç ve üzerlerine bomba ekle
            for (int i = 0; i < 10; i++)
            {
                int randomIndex = random.Next(OyunPanel.Controls.Count);
                Control selectedZemin = OyunPanel.Controls[randomIndex];

                Bomba bomba = new Bomba(OyunPanel.Height, OyunPanel.Size);
                bomba.Left = selectedZemin.Left;
                bomba.Top = selectedZemin.Top;

                OyunPanel.Controls.Add(bomba);
                bomba.BringToFront();
            }
        }

        private void KarakterBombaKontrol()
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Bomba && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    Bomba bomba = control as Bomba;
                    CanAzalt();
                    // Eğer bir bomba ile etkileşim gerçekleşirse, döngüyü sonlandırabilirsiniz.
                    break;
                }
            }
        }

        private void SandıkOlustur()
        {
            Sandık sandik = new Sandık(OyunPanel.Height, OyunPanel.Size);

            // Sandığı rastgele bir konuma yerleştir
            Random random = new Random();

            // Zemin nesnelerini bul
            var zeminNesneleri = OyunPanel.Controls.OfType<Zemin>().ToArray();

            if (zeminNesneleri.Length > 0)
            {
                // Rastgele bir zemin seç
                Zemin rastgeleZemin = zeminNesneleri[random.Next(zeminNesneleri.Length)];

                // Sandığı seçilen zeminin içine yerleştir
                sandik.Left = random.Next(rastgeleZemin.Left, rastgeleZemin.Right - sandik.Width);
                sandik.Top = rastgeleZemin.Top - sandik.Height;

                OyunPanel.Controls.Add(sandik);
                sandik.BringToFront();
            }
        }

        private void KarakterSandikKontrol()
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Sandık && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    Sandık sandik = (Sandık)control;

                    Random random = new Random();
                    double rastgeleOlasilik = random.NextDouble(); // 0 ile 1 arasında rastgele bir sayı

                    if (rastgeleOlasilik < 0.8) // %80 ihtimalle iyi bir olay
                    {
                        CanYükselt();
                    }
                    else // %20 ihtimalle kötü bir olay
                    {
                        CanAzalt();
                    }

                    OyunPanel.Controls.Remove(control);
                    
                }
            }
        }
        
        private void FinishOlustur()
        {
            finish = new Finish(OyunPanel.Height, OyunPanel.Size);
            finish.Top = 90;
            finish.Left = 14 * 125;
            OyunPanel.Controls.Add(finish);
        }

        private void FinisheGeldiMi() 
        {
            if(karakter.Location.X == finish.Location.X) 
            {
                PuanHesapla();
                DevamEdiyorMu = false;
                sandikOlusturmaTimer.Stop();
                gecenSureTimer.Stop();
                
                SeviyeAtla();
            }
        }

        private void SeviyeAtla() 
        {
            seviye++;
            Sıfırla();
            if(seviye == 2) 
            {
                SeviyeAtlaD();
                bombaOlusturmaTimer.Start();
            }
            else if (seviye == 3) 
            {
                SeviyeAtlaD();
                bombaOlusturmaTimer.Stop();

            }
        }

        private void SeviyeAtlaD() 
        {
            KarakterOlustur();
            ZeminOlustur();
            SandıkOlustur();

            DevamEdiyorMu = true;
            can++;
            gecenSureTimer.Start();
            sandikOlusturmaTimer.Start();
            CanLabel.Text = CanLabel.ToString();
            SeviyeLabel.Text = seviye.ToString();
        }

        private void Sıfırla() 
        {
            OyunPanel.Controls.Remove(karakter); // Karakteri kaldır

            // Zemin nesnelerini bul ve kaldır
            foreach (Control control in OyunPanel.Controls.OfType<Zemin>().ToList())
            {
                OyunPanel.Controls.Remove(control);
            }

            // Tuzak nesnelerini bul ve kaldır
            if (seviye == 2) 
            {
                foreach (Control control in OyunPanel.Controls.OfType<Tuzak>().ToList())
                {
                    OyunPanel.Controls.Remove(control);
                }
            }

            if(seviye == 3) 
            {
                foreach (Control control in OyunPanel.Controls.OfType<Bomba>().ToList())
                {
                    OyunPanel.Controls.Remove(control);
                }
            }

            foreach (Control control in OyunPanel.Controls.OfType<Sandık>().ToList())
            {
                OyunPanel.Controls.Remove(control);
            }

            OyunPanel.Controls.Remove(finish);
        }

        private void CanAzalt() 
        {
            can--;

            if(can == 0) 
            {
                PuanHesapla();
                DevamEdiyorMu = false;
                gecenSureTimer.Stop();
            }

            CanLabel.Text = can.ToString();
        }

        private void CanYükselt() 
        {
            can++;
            CanLabel.Text = can.ToString();

        }

        private void PuanHesapla() 
        {
            puan = can * 500 + (1000 - gecenSure.Seconds);
            PuanLabel.Text = puan.ToString();
            PuanLabel.Visible = true;
        }

        #endregion
    }
}
