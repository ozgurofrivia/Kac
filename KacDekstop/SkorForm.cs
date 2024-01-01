/****************************************
*    Bilişim Sistemleri Mühendisliği    *
*    Nesneye Dayalı Programlama         *
*                                       *
*    Ad: Özgür                          *
*    Soyad: Özgenç                      *
*    Numara: B221200015                 *
****************************************/



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KacDekstop
{
    public partial class SkorForm : Form
    {
        public SkorForm()
        {
            InitializeComponent();
            
            LoadTop5ScoresFromFile();
            
        }

        private void LoadTop5ScoresFromFile()
        {
            string filePath = "yüksekskorlar.txt"; // Text dosyasının yolu

            // Dosya var mı kontrol et
            if (File.Exists(filePath))
            {
                try
                {
                    // Dosyayı okuma işlemi
                    List<Tuple<string, int>> scoreList = new List<Tuple<string, int>>();

                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        // Satır satır oku ve isim-puan çiftlerini listeye ekle
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            string[] parts = line.Split(' ');

                            if (parts.Length >= 2 && int.TryParse(parts[parts.Length - 1], out int score))
                            {
                                string name = string.Join(" ", parts.Take(parts.Length - 1));
                                scoreList.Add(new Tuple<string, int>(name, score));
                            }
                        }
                    }

                    // Puanlara göre sırala (büyükten küçüğe)
                    scoreList.Sort((a, b) => b.Item2.CompareTo(a.Item2));

                    // İlk 5 isim-puan çiftini ListBox'a ekle
                    int count = 0;
                    foreach (var item in scoreList.Take(5))
                    {
                        skorListBox.Items.Add($"{item.Item1} - {item.Item2}");
                        count++;
                    }

                    // Eğer dosyadaki isim-puan çiftleri 5'ten az ise geri kalan yerlere "N/A" ekleyin
                    while (count < 5)
                    {
                        skorListBox.Items.Add("N/A");
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Dosya okuma hatası: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Dosya bulunamadı.");
            }
        }

        private void SkorForm_Load(object sender, EventArgs e)
        {

        }
    }
}
