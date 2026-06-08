# Hamming SEC-DED (Single Error Correction - Double Error Detection) Simulator

Bu proje, **Bursa Teknik Üniversitesi Bilgisayar Mühendisliği Bölümü - BLM0230 Bilgisayar Mimarisi** dersi dönem ödevi kapsamında geliştirilmiştir. Uygulama, veri iletimi ve depolama güvenliğinde kritik öneme sahip olan Hamming (SEC-DED) hata kontrol kodlama mekanizmasını hem matematiksel arka planıyla doğrulamakta hem de kullanıcıya dinamik bir arayüz üzerinden simüle etme imkanı sunmaktadır.

## 🧑‍💻 Geliştirici Bilgileri
- **Adı Soyadı:** Yusuf Çil
- **Öğrenci Numarası:** 22360859012
- **Pozisyon:** Bilgisayar Mühendisliği Lisans Öğrencisi (3. Sınıf)

---

## 🚀 Proje Özellikleri

- **Dinamik Modüler Mimari:** Windows Forms tasarımcı hatalarını ve derleme zamanı kilitlenmelerini (Build Cache sorunlarını) tamamen bypass etmek amacıyla kullanıcı arayüzü bileşenleri çalışma zamanında (**Runtime**) dinamik olarak inşa edilmiştir.
- **Esnek Bit Desteği:** Giriş filtresi sayesinde sistem yalnızca endüstri standardı olan **8-bit**, **16-bit** ve **32-bit** uzunluğundaki verileri kabul eder.
- **Klavye Seviyesinde Güvenlik:** Veri giriş alanına sadece '0', '1' ve Backspace tuşlarının basılmasına izin verilerek hatalı girdi ihtimali donanımsal düzeyde kilitlenmiştir.
- **Gelişmiş SEC-DED Motoru:**
  - **SEC (Single Error Correction):** Enjekte edilen tek bitlik hataların sendrom kelimesi hesaplanarak tam bit pozisyonu tespit edilir ve veri otomatik olarak orijinal haline döndürülür.
  - **DED (Double Error Detection):** Çoklu bit hatası simülasyonlarında, koleksiyon bazlı indeks takip algoritması (`currentErrorPositions`) devreye girerek iki bitin birden bozulduğunu anında yakalar. Hamming sınırları gereği bu hatanın düzeltilemeyeceği uyarısını akademik log raporuyla ekrana basar.
- **Görsel Durum Paneli:** Hücre tabanlı renk kodlaması ile bitlerin durumu anlık izlenebilir:
  - ⚪ **Beyaz:** Sağlıklı / Değişmemiş Bit
  - 🔴 **Kırmızı:** Enjekte Edilmiş / Bozulmuş Hatalı Bit
  - 🟢 **Yeşil:** Başarıyla Tespit Edilmiş ve Düzeltilmiş Bit

---

## 🛠️ Kullanılan Teknolojiler

- **Dil:** C# (.NET Framework / .NET Core uyumlu)
- **Arayüz Teknolojisi:** Windows Forms (Dynamic Runtime UI Generation)
- **Geliştirme Ortamı:** Visual Studio / VS Code
- **Sürüm Kontrolü:** Git & GitHub

---

## 📁 Proje Dizin Yapısı

Proje, gereksiz derleme artıklarından (`bin/obj`) arındırılmış temiz bir mühendislik mimarisine sahiptir:

```text
BLM0230_Proje_Yusuf Çil_22360859012/
│
├── BLM0230_Proje_YusufCil.slnx             # Visual Studio Çözüm (Solution) Dosyası
├── BLM0230_Proje_22360859012_Yusuf Çil.pdf # Akademik Proje Raporu
│
└── BLM0230_Proje_YusufCil/                 # Uygulama Kaynak Kod Klasörü
    ├── Properties/                         # Proje Özellikleri
    ├── App.config                          # Uygulama Yapılandırma Dosyası
    ├── BLM0230_Proje_YusufCil.csproj       # C# Proje Tanım Dosyası
    ├── Form1.cs                            # Dinamik Arayüz ve Olay Yönetimi
    ├── Form1.Designer.cs                   # Form Bileşen Yapısı
    ├── Form1.resx                          # Proje Kaynakları
    ├── HammingEncoder.cs                   # Hamming SEC-DED Matematiksel Motoru
    └── Program.cs                          # Uygulama Giriş Noktası
