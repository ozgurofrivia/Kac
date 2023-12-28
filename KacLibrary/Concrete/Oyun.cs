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
using System.IO;

namespace KacLibrary.Concrete
{
    public class Oyun : IOyun
    {

        #region Alanlar
        private readonly Timer sandikOlusturmaTimer = new Timer { Interval = 5000 };
        private readonly Timer gecenSureTimer = new Timer { Interval = 1000 };
        private readonly Timer bombaOlusturmaTimer = new Timer { Interval = 3000 };
        private readonly Timer dusmanOlusturmaTimer = new Timer { Interval = 2000 };
        private readonly Timer dusmanHareketEttirmeTimer = new Timer { Interval = 1000 };

        private TimeSpan gecenSure;
        private readonly Panel OyunPanel;
        private readonly Label PuanLabel, CanLabel, SeviyeLabel, OyuncuAd;
        
        private Karakter karakter;
        private Zemin zemin;
        private Finish finish;
        private Sandık sandık;
        private Bomba bomba;
        private Dusman dusman;

        int can = 999;
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

        public Oyun(Panel oyunPanel, Label canLabel, Label oyuncuLabel,  Label seviyeLabel, Label puanLabel)
        {
            CanLabel = canLabel;
            SeviyeLabel = seviyeLabel;
            PuanLabel = puanLabel; 
            OyunPanel = oyunPanel;
            OyuncuAd = oyuncuLabel;
            bombaOlusturmaTimer.Tick += BombaOlusturmaTimer_Tick;
            gecenSureTimer.Tick += GecenSureTimer_Tick;
            sandikOlusturmaTimer.Tick += SandikOlusturmaTimer_Tick;
            dusmanOlusturmaTimer.Tick += DusmanOlusturmaTimer_Tick;
            dusmanHareketEttirmeTimer.Tick += DusmanHareketEttirmeTimer_Tick;
            Console.WriteLine("OyuncuAd: " + CanLabel.Text);
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
        private void DusmanOlusturmaTimer_Tick(object sender, EventArgs e) 
        {
            DusmanOlustur();
        }
        private void DusmanHareketEttirmeTimer_Tick( object sender, EventArgs e) 
        {
            DusmanHareketEt();
           
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
      
        public void Durdur()   //P tuşuna basıldığında oyununu durduracak gerekli fonksiyon
        {
            if (DevamEdiyorMu)
            {

                sandikOlusturmaTimer.Stop();
                gecenSureTimer.Stop();
                DevamEdiyorMu = false;
                if (seviye == 2)
                {
                    bombaOlusturmaTimer.Stop();
                }
                if (seviye == 3) 
                {
                    dusmanOlusturmaTimer.Stop();
                    dusmanHareketEttirmeTimer.Stop();
                }
                
            }
            else
            {
                sandikOlusturmaTimer.Start();
                gecenSureTimer.Start();
                DevamEdiyorMu = true;

                if (seviye == 2)
                {
                    bombaOlusturmaTimer.Start();
                }
                if (seviye == 3)
                {
                    dusmanOlusturmaTimer.Start();
                    dusmanHareketEttirmeTimer.Start();
                }

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
                KarakterDusmanKontrol();
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
                    CanAzalt();
                    karakter.BringToFront() ;
                    Tuzak tuzak = control as Tuzak;
                    tuzak.Visible = true;
                    
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

            // Rastgele 10 zemin seçip bomba yerleştir
            for (int i = 0; i < 15; i++)
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

        private void KarakterBombaKontrol()  //Karakterin bombayla etkileşimini kontrol eden bir fonksiyon
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Bomba && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    CanAzalt();
                    karakter.BringToFront();
                   
                 
                }
            }
        }

        private void DusmanOlustur()
        {
          

            int satirSayisi = random.Next(0, 6);
            
           
                dusman = new Dusman(OyunPanel.Height, OyunPanel.Size);

                
                dusman.Top = dusman.Top+ dusman.Width * satirSayisi;

                OyunPanel.Controls.Add(dusman);
                dusman.BringToFront();
            
        }

        private void KarakterDusmanKontrol()
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Dusman && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    Dusman dusman = control as Dusman;
                    karakter.BringToFront();
                    CanAzalt();
                  
                }
            }
        }



        private void DusmanHareketEt()
        {
            foreach (Control control in OyunPanel.Controls.OfType<Dusman>().ToList())
            {
                
                control.Left -= control.Width;

                
                if (control.Left < 125) // Zeminlerin dışına çıkmışsa düşmanı kaldır.
                {
                    OyunPanel.Controls.Remove(control);
                }
            }
        }


        private void SandıkOlustur()
        {
            Sandık sandik = new Sandık(OyunPanel.Height, OyunPanel.Size);

            // Sandığı rastgele bir konuma yerleştir
            

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
                   
                    double rastgeleOlasilik = random.NextDouble(); // 0 ile 1 arasında rastgele bir sayı

                    if (rastgeleOlasilik < 0.8) // %80 ihtimalle iyi bir olay
                    {
                        CanYükselt();
                    }
                    else // %20 ihtimalle kötü bir olay
                    {
                        CanAzalt();
                    }
                    karakter.BringToFront();
                    OyunPanel.Controls.Remove(control);
                    
                }
            }
        }
        
        private void FinishOlustur()
        {
            finish = new Finish(OyunPanel.Height, OyunPanel.Size);
           
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
            switch (seviye)
            {
                case 2:
                    SeviyeAtlaD();
                    bombaOlusturmaTimer.Start();
                    break;
                case 3:
                    SeviyeAtlaD();
                    bombaOlusturmaTimer.Stop();
                    dusmanOlusturmaTimer.Start();
                    dusmanHareketEttirmeTimer.Start();
                    break;
                case 4:

                    OyunBitti();
                   
                    break;

            }

        }

        private void SeviyeAtlaD() 
        {
            KarakterOlustur();
            ZeminOlustur();

            DevamEdiyorMu = true;
            can++;
            gecenSureTimer.Start();
            sandikOlusturmaTimer.Start();
            CanLabel.Text = can.ToString();
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

            if (seviye == 4)
            {
                foreach (Control control in OyunPanel.Controls.OfType<Dusman>().ToList())
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
            CanLabel.Text = can.ToString();

            if (can == 0) 
            {
                OyunBitti();

                DevamEdiyorMu = false;
                
            }

            
        }

        private void CanYükselt() 
        {
            can++;
            CanLabel.Text = can.ToString();

        }

        private const string YuksekSkorDosyaAdı = "yüksekskorlar.txt";

        private List<string> yuksekSkorlar;

        public void YuksekSkorlarıKaydet(string oyuncuAdı, int skor)
        {
            // Bu kontrolü ekleyin
            if (yuksekSkorlar == null)
            {
                yuksekSkorlar = new List<string>();
            }

            yuksekSkorlar.Add($"{oyuncuAdı}: {skor}");

            using (StreamWriter writer = new StreamWriter(YuksekSkorDosyaAdı, true)) // true: dosyaya ekleme modu
            {
                writer.WriteLine($"{oyuncuAdı}: {skor}");
            }
        }


        private void PuanHesapla() 
        {
            puan = can * 500 + (1000 - gecenSure.Seconds);
            PuanLabel.Text = puan.ToString();
            PuanLabel.Visible = true;

            
        }

        private void TimerStop() 
        {
            bombaOlusturmaTimer.Stop();
            gecenSureTimer.Stop();
            sandikOlusturmaTimer.Stop();
            dusmanOlusturmaTimer.Stop();
            dusmanHareketEttirmeTimer.Stop();  
        }

        private void OyunBitti() 
        {
            TimerStop();
            PuanHesapla();
            YuksekSkorlarıKaydet(OyuncuAd.Text, puan);
            if (can == 0) 
            {
                MessageBox.Show("Başaramadın");
            }
            else 
            {
                MessageBox.Show("Neyi başaramadım");
            }
            Application.Exit();
        }
        #endregion

    }
}
