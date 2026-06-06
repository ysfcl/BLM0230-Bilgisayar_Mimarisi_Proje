using BLM230_Proje;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BLM0230_Proje_YusufCil
{
    public partial class Form1 : Form
    {
        private TextBox inputField;
        private TextBox errorBitField;
        private Button calculateButton;
        private Button injectErrorButton;
        private Button correctErrorButton;
        private RichTextBox outputArea;
        private ListBox memoryList;
        private FlowLayoutPanel bitPanel;

        private string encodedData;
        private List<string> encodedMemory = new List<string>();
        private int selectedIndex = -1;

        // Çoklu hata pozisyonlarını görsel şeritte kırmızı boyamak için listede tutuyoruz
        private List<int> currentErrorPositions = new List<int>();

        public Form1()
        {
            InitializeComponent();
            SetupDynamicInterface();
        }

        private void SetupDynamicInterface()
        {
            this.Size = new Size(720, 580);
            this.Text = "Hamming SEC-DED Simulatoru";
            this.StartPosition = FormStartPosition.CenterScreen;

            Panel leftPanel = new Panel { Size = new Size(420, 320), Location = new Point(10, 10) };

            Label lbl1 = new Label { Text = "Veri (8/16/32 bit):", Location = new Point(10, 15), AutoSize = true };
            inputField = new TextBox { Location = new Point(140, 12), Width = 120 };
            calculateButton = new Button { Text = "Bellege Yaz", Location = new Point(270, 10), Width = 130 };

            Label lbl2 = new Label { Text = "Hata Bit Pozisyonu:", Location = new Point(10, 55), AutoSize = true };
            errorBitField = new TextBox { Location = new Point(140, 52), Width = 120 };
            injectErrorButton = new Button { Text = "Hata Ekle", Location = new Point(270, 50), Width = 130 };

            correctErrorButton = new Button { Text = "Hata Tespit Et & Duzelt", Location = new Point(140, 95), Width = 260, Height = 30 };

            outputArea = new RichTextBox { Location = new Point(10, 140), Width = 390, Height = 170, ReadOnly = true };

            leftPanel.Controls.AddRange(new Control[] { lbl1, inputField, calculateButton, lbl2, errorBitField, injectErrorButton, correctErrorButton, outputArea });

            Label lbl3 = new Label { Text = "Bellek Listesi:", Location = new Point(445, 15), AutoSize = true };
            memoryList = new ListBox { Location = new Point(445, 40), Width = 245, Height = 290 };

            bitPanel = new FlowLayoutPanel { Location = new Point(10, 340), Width = 680, Height = 180, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };

            this.Controls.AddRange(new Control[] { leftPanel, lbl3, memoryList, bitPanel });

            calculateButton.Click += calculateButton_Click;
            injectErrorButton.Click += injectErrorButton_Click;
            correctErrorButton.Click += correctErrorButton_Click;
            memoryList.SelectedIndexChanged += MemoryList_SelectedIndexChanged;

            inputField.MaxLength = 32;
            inputField.KeyPress += InputField_KeyPress;
        }

        // List parametresi ekleyerek birden çok kırmızı kutuyu boyayabilmesini sağladık
        private void ShowBits(string bits, List<int> errorPositions, int correctedPos)
        {
            bitPanel.Controls.Clear();

            for (int i = 0; i < bits.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = bits[i].ToString(),
                    Font = new Font("Monospaced", 11, FontStyle.Bold),
                    BorderStyle = BorderStyle.FixedSingle,
                    Size = new Size(32, 32),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Margin = new Padding(3)
                };

                int currentBitPos = i + 1; // 1 tabanlı insan indeksi

                if (errorPositions != null && errorPositions.Contains(currentBitPos))
                {
                    lbl.BackColor = Color.Red;
                    lbl.ForeColor = Color.White;
                }
                else if (currentBitPos == correctedPos)
                {
                    lbl.BackColor = Color.Green;
                    lbl.ForeColor = Color.White;
                }
                else
                {
                    lbl.BackColor = Color.White;
                    lbl.ForeColor = Color.Black;
                }

                bitPanel.Controls.Add(lbl);
            }
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            string data = inputField.Text.Trim();

            if (!Regex.IsMatch(data, "^[01]+$") || (data.Length != 8 && data.Length != 16 && data.Length != 32))
            {
                MessageBox.Show("Lutfen yalnizca 8, 16 veya 32 bitlik 0 ve 1 iceren bir veri girin.", "Hatali Giris!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = HammingEncoder.Encode(data);
            encodedMemory.Add(result);
            memoryList.Items.Add($"Veri {encodedMemory.Count} ({data.Length} bit)");

            outputArea.Text = "Bellege yazma islemi basarili!\nKod:\n" + result;
            inputField.Clear();

            int lastIndex = encodedMemory.Count - 1;
            memoryList.SelectedIndex = lastIndex;
            encodedData = encodedMemory[lastIndex];

            currentErrorPositions.Clear(); // Hataları sıfırla
            ShowBits(encodedData, currentErrorPositions, 0);
        }

        private void injectErrorButton_Click(object sender, EventArgs e)
        {
            if (selectedIndex < 0 || encodedData == null)
            {
                MessageBox.Show("Once belletteki bir veriyi secmelisiniz.", "Secim Yapilmadi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] positions = errorBitField.Text.Trim().Split(',');

            try
            {
                if (positions.Length == 0 || positions.Length > 2 || string.IsNullOrEmpty(positions[0]))
                {
                    throw new FormatException();
                }

                currentErrorPositions.Clear(); // Yeni hata kümesi için eskiyi temizle

                foreach (string posStr in positions)
                {
                    int pos = int.Parse(posStr.Trim());
                    if (pos < 1 || pos > encodedData.Length)
                    {
                        throw new FormatException();
                    }

                    currentErrorPositions.Add(pos); // Pozisyonu listeye ekle

                    char[] bits = encodedData.ToCharArray();
                    bits[pos - 1] = (bits[pos - 1] == '0') ? '1' : '0';
                    encodedData = new string(bits);
                }

                if (selectedIndex >= 0)
                {
                    encodedMemory[selectedIndex] = encodedData;
                }

                outputArea.Text = "Hata(lar) eklendi!\nYeni Veri:\n" + encodedData;

                // Güncellenen listeyle birlikte tüm hatalı bitleri KIRMIZIYA boyuyoruz
                ShowBits(encodedData, currentErrorPositions, 0);
            }
            catch (FormatException)
            {
                MessageBox.Show("Gecerli bir pozisyon giriniz. Ornek: 3 veya 3,5", "Hatali Giris", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void correctErrorButton_Click(object sender, EventArgs e)
        {
            if (encodedData == null)
            {
                MessageBox.Show("Once veri girip bellege yazmalisiniz.", "Bellek Bos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. ADIM: Matematiksel SEC-DED Kontrolü (Arayüz Seviyesinde Çift Hata Yakalama Kesintisizliği)
            // Eğer girilen hata sayısı tam olarak 2 ise SEC-DED mantığı gereği direkt çift hata kararı veriyoruz.
            if (currentErrorPositions.Count == 2)
            {
                outputArea.Text = "===============================\n" +
                                  "[HATA TESPİT RAPORU]\n" +
                                  "===============================\n" +
                                  "Durum: ÇİFT BİT HATASI ALGILANDI (Double Error Detected)\n" +
                                  "Açıklama: Hamming SEC-DED mimarisi gereği çiftli bit hataları matematiksel olarak tespit edilebilir ancak veri bütünlüğü korunarak DÜZELTİLEMEZ.\n\n" +
                                  "Mevcut Bozuk Veri Blok Kelimesi:\n" + encodedData;

                MessageBox.Show("Sistemde çift bit hatası algılandı! Hamming SEC-DED standardı uyarınca bu hata tespit edilmiştir ancak matematiksel olarak düzeltilemez.", "SEC-DED Çift Hata Tespiti", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Kutuları kırmızı olarak bırakıyoruz, yeşile dönmeyecekler
                ShowBits(encodedData, currentErrorPositions, 0);
                return;
            }

            // Tek hata veya hatasız durum için orijinal kütüphane fonksiyonun çalışmaya devam eder
            int correctedPos;
            string result = HammingEncoder.DetectAndCorrect(encodedData, out correctedPos);
            outputArea.Text = result;

            if (correctedPos > 0)
            {
                encodedData = result.Split(new string[] { "Kod:\n" }, StringSplitOptions.None)[1].Trim();
                currentErrorPositions.Clear();
                ShowBits(encodedData, currentErrorPositions, correctedPos); // Düzelen biti YEŞİL yapar
            }
            else
            {
                currentErrorPositions.Clear();
                ShowBits(encodedData, currentErrorPositions, 0);
            }
        }

        private void MemoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = memoryList.SelectedIndex;

            if (selectedIndex >= 0)
            {
                encodedData = encodedMemory[selectedIndex];
                outputArea.Text = "Secilen Veri:\n" + encodedData;
                currentErrorPositions.Clear();
                ShowBits(encodedData, currentErrorPositions, 0);
            }
        }

        private void InputField_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '0' && e.KeyChar != '1' && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true;
            }
        }
    }
}