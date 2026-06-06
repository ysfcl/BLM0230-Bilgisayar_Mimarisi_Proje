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
        // Tasarımcı hatasını bypass etmek için nesneleri doğrudan kod tarafında tanımlıyoruz
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

        public Form1()
        {
            InitializeComponent();
            SetupDynamicInterface();
        }

        // Arayüz bileşenlerini dinamik olarak formun üzerine yerleştiren metot
        private void SetupDynamicInterface()
        {
            this.Size = new Size(720, 580);
            this.Text = "Hamming SEC-DED Simulatoru";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Sol Panel (Girişler ve Butonlar için)
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

            // Sağ Liste (Bellek Kayıtları için)
            Label lbl3 = new Label { Text = "Bellek Listesi:", Location = new Point(445, 15), AutoSize = true };
            memoryList = new ListBox { Location = new Point(445, 40), Width = 245, Height = 290 };

            // Alt Panel (Renkli Bit Kutuları için)
            bitPanel = new FlowLayoutPanel { Location = new Point(10, 340), Width = 680, Height = 180, AutoScroll = true, BorderStyle = BorderStyle.FixedSingle };

            // Hepsini Forma Ekleme
            this.Controls.AddRange(new Control[] { leftPanel, lbl3, memoryList, bitPanel });

            // Olayları (Eventleri) Bağlama
            calculateButton.Click += calculateButton_Click;
            injectErrorButton.Click += injectErrorButton_Click;
            correctErrorButton.Click += correctErrorButton_Click;
            memoryList.SelectedIndexChanged += MemoryList_SelectedIndexChanged;

            // inputField için maksimum karakter sınırını 32 yapıyoruz
            inputField.MaxLength = 32;

            // Kullanıcının sadece 0, 1 ve Backspace (silme) tuşuna basmasına izin veriyoruz
            inputField.KeyPress += InputField_KeyPress;
        }

        private void ShowBits(string bits, int errorPos, int correctedPos)
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

                if (i + 1 == errorPos)
                {
                    lbl.BackColor = Color.Red;
                    lbl.ForeColor = Color.White;
                }
                else if (i + 1 == correctedPos)
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
            ShowBits(encodedData, 0, 0);
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

                foreach (string posStr in positions)
                {
                    int pos = int.Parse(posStr.Trim());
                    if (pos < 1 || pos > encodedData.Length)
                    {
                        throw new FormatException();
                    }

                    char[] bits = encodedData.ToCharArray();
                    bits[pos - 1] = (bits[pos - 1] == '0') ? '1' : '0';
                    encodedData = new string(bits);
                }

                if (selectedIndex >= 0)
                {
                    encodedMemory[selectedIndex] = encodedData;
                }

                outputArea.Text = "Hata(lar) eklendi!\nYeni Veri:\n" + encodedData;

                int firstErrorPos = int.Parse(positions[0].Trim());
                ShowBits(encodedData, firstErrorPos, 0);
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

            int correctedPos;
            string result = HammingEncoder.DetectAndCorrect(encodedData, out correctedPos);
            outputArea.Text = result;

            if (correctedPos > 0)
            {
                encodedData = result.Split(new string[] { "Kod:\n" }, StringSplitOptions.None)[1].Trim();
                ShowBits(encodedData, 0, correctedPos);
            }
            else
            {
                ShowBits(encodedData, 0, 0);
            }
        }

        private void MemoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedIndex = memoryList.SelectedIndex;

            if (selectedIndex >= 0)
            {
                encodedData = encodedMemory[selectedIndex];
                outputArea.Text = "Secilen Veri:\n" + encodedData;
                ShowBits(encodedData, 0, 0);
            }
        }

        private void InputField_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer basılan tuş '0' değilse VE '1' değilse VE Backspace (silme tuşu) değilse
            if (e.KeyChar != '0' && e.KeyChar != '1' && e.KeyChar != (char)Keys.Back)
            {
                // Tuş vuruşunu iptal et (metin kutusuna yazılmasın)
                e.Handled = true;
            }
        }
    }
}