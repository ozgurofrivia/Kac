﻿/****************************************
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

        int can = 3;
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
            KarakterBombaKontrol();
        }
        private void DusmanOlusturmaTimer_Tick(object sender, EventArgs e) 
        {
            DusmanOlustur();
        }
        private void DusmanHareketEttirmeTimer_Tick( object sender, EventArgs e) 
        {
            DusmanHareketEt();
            KarakterDusmanKontrol();
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
            Random random = new Random(); // Rastgele seçim için random nesnesi

            // 3 tuzak resmini manuel olarak tanımlama
            Image tuzakResim1 = Image.FromFile("Resimler\\atestuzak.png");
            Image tuzakResim2 = Image.FromFile("Resimler\\chidori.png");
            Image tuzakResim3 = Image.FromFile("Resimler\\sasukesharingan.png");

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (random.Next(10) < 2) // Yüzde 20 olasılıkla tuzak ekler
                    {
                        Tuzak tuzak = new Tuzak(OyunPanel.Height, OyunPanel.Size);

                        // Rastgele bir tuzak resmi seçme
                        int randomResimIndex = random.Next(3); // 0, 1 veya 2

                        // Tuzak resmini ayarla
                        switch (randomResimIndex)
                        {
                            case 0:
                                tuzak.Image = tuzakResim1;
                                break;
                            case 1:
                                tuzak.Image = tuzakResim2;
                                break;
                            case 2:
                                tuzak.Image = tuzakResim3;
                                break;
                        }

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

            // Rastgele 15 zemin seçip bomba yerleştir
            for (int i = 0; i < 15; i++)
            {
                Zemin selectedZemin = GetRandomZemin();

                if (selectedZemin != null)
                {
                    Bomba bomba = new Bomba(OyunPanel.Height, OyunPanel.Size);
                    bomba.Left = selectedZemin.Left;
                    bomba.Top = selectedZemin.Top;
                    OyunPanel.Controls.Add(bomba);
                    bomba.BringToFront();
                }
            }
        }

        private Zemin GetRandomZemin()
        {
            // Zemin sayısı yetersizse null döndür
            if (OyunPanel.Controls.OfType<Zemin>().Count() < 15)
            {
                return null;
            }

            // Rastgele bir zemin seç
            int randomIndex = random.Next(OyunPanel.Controls.Count);
            Control selectedControl = OyunPanel.Controls[randomIndex];

            // Seçilen kontrol bir Zemin mi diye kontrol et
            if (selectedControl is Zemin)
            {
                return (Zemin)selectedControl;
            }
            else
            {
                // Eğer seçilen kontrol bir Zemin değilse, tekrar seç
                return GetRandomZemin();
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
                   OyunPanel.Controls.Remove(control);
                 
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

        private void KarakterDusmanKontrol() //Karakterin düşmanla etkileşimini kontrol eden bir fonksiyon
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Dusman && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                    CanAzalt();
                    karakter.BringToFront();
                   
                    OyunPanel.Controls.Remove(control);
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
            Zemin selectedZemin = GetRandomZemin();

            if (selectedZemin != null)
            {
                Sandık sandik = new Sandık(OyunPanel.Height, OyunPanel.Size);
                sandik.Left = selectedZemin.Left;
                sandik.Top = selectedZemin.Top;
                OyunPanel.Controls.Add(sandik);
                sandik.BringToFront();
            }
        
        }

        private void KarakterSandikKontrol()//Karakterin sandıkla etkileşimini kontrol eden bir fonksiyon
        {
            foreach (Control control in OyunPanel.Controls)
            {
                if (control is Sandık && karakter.Bounds.IntersectsWith(control.Bounds))
                {
                   
                    double rastgeleOlasilik = random.NextDouble(); // 0 ile 1 arasında rastgele bir sayı

                    if (rastgeleOlasilik < 0.8) // %80 ihtimalle canı 1 arttır
                    {
                        CanYükselt();
                    }
                    else // %20 ihtimalle canı 1 azalt
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

        private void FinisheGeldiMi() //Finish çizgisine ulaşılıp ulaşılmadığını kontrol eder
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

        private void SeviyeAtla() // Finishteyse seviyeyi 1 arttır ve Sfırla fonksiyonunu çağır
        {
            seviye++;
            Sıfırla();
            switch (seviye)
            {
                case 2: // Seviye 2 olunca bombaOlusturmaTimer çalışsın
                    SeviyeAtlaD(); 
                    bombaOlusturmaTimer.Start();
                    break;
                case 3: // Seviye 3 olunca bombaOlusturmaTimer dursun, dusmanOlusturmaTimer ve dusmanHareketEttirmeTimer çalışsın
                    dusmanHareketEttirmeTimer.Start();
                    SeviyeAtlaD();
                    bombaOlusturmaTimer.Stop();
                    dusmanOlusturmaTimer.Start();
                    dusmanHareketEttirmeTimer.Start();
                    break;
                case 4: // Eğer 4. kez finishe gelinmişse oyun bitmiş demektir oyunu bitir.

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

        private void Sıfırla()  // Her seviye atladıktan sonra paneli temizler
        {
            OyunPanel.Controls.Remove(karakter); // Karakteri kaldır

            // Zeminleri bul ve kaldır
            foreach (Control control in OyunPanel.Controls.OfType<Zemin>().ToList())
            {
                OyunPanel.Controls.Remove(control);
            }

            // Tuzakları bul ve kaldır
            if (seviye == 2) 
            {
                foreach (Control control in OyunPanel.Controls.OfType<Tuzak>().ToList())
                {
                    OyunPanel.Controls.Remove(control);
                }
            }

            // Bombaları bul ve kaldır
            if(seviye == 3) 
            {
                foreach (Control control in OyunPanel.Controls.OfType<Bomba>().ToList())
                {
                    OyunPanel.Controls.Remove(control);
                }
            }

            // Düşmanları bul ve kaldır
            if (seviye == 4)
            {
                foreach (Control control in OyunPanel.Controls.OfType<Dusman>().ToList())
                {
                    OyunPanel.Controls.Remove(control);
                }
            }

            // Sandıkları Kaldır
            foreach (Control control in OyunPanel.Controls.OfType<Sandık>().ToList())
            {
                OyunPanel.Controls.Remove(control);
            }

            // Finisihi kaldır
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

        private void TimerStop() // Timerları topluca durdurur
        {
            bombaOlusturmaTimer.Stop();
            gecenSureTimer.Stop();
            sandikOlusturmaTimer.Stop();
            dusmanOlusturmaTimer.Stop();
            dusmanHareketEttirmeTimer.Stop();  
        }

        private void OyunBitti() // Oyun bittiyse timerları durdur, puanı hesapla ve text dosyasına kaydet
        {
            TimerStop();
            PuanHesapla();
            YuksekSkorlarıKaydet(OyuncuAd.Text, puan);
            if (can == 0) 
            {
                MessageBox.Show("Olmadı, olsun");
            }
            else 
            {
                MessageBox.Show("Helal be!");
            }
            Application.Exit();
        }
        #endregion

    }
}
