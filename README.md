# 📦 WMS - Depo Yönetim Sistemi

Depo operasyonlarını yönetmek için tasarlanmış, modern mimariyle inşa edilmiş bir **Depo Yönetim Sistemi (Warehouse Management System)**.

## 🎯 Proje Amacı

WMS, depoların günlük operasyonlarını yönetmeyi, ürün stoklarını takip etmeyi ve depo süreçlerini otomatikleştirmeyi amaçlamaktadır. Sistem, iyi tanımlanmış API katmanı ile bölüm bazında ölçeklenebilir ve bakımı kolay bir yapıya sahiptir.

## 🏗️ Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                    WebUI (MVC) - Port 7081                  │
│                 (ASP.NET Core, Bootstrap 5)                 │
└────────────────────────┬────────────────────────────────────┘
                         │
                    HttpClient
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   WebAPI (REST) - Port 7234                 │
│              (ASP.NET Core, Swagger/OpenAPI)                │
└────────────────────────┬────────────────────────────────────┘
                         │
                    Dependency Injection
                         │
        ┌────────────────┼────────────────┐
        ▼                ▼                ▼
   ┌─────────┐  ┌──────────────┐  ┌──────────────┐
   │Business │  │    Core      │  │    Data      │
   │Services │  │ (DTOs, Models)  │(EF Core)     │
   └─────────┘  └──────────────┘  └──────────────┘
        │                                 │
        └─────────────────┬───────────────┘
                          ▼
              ┌──────────────────────────┐
              │   SQLite Database        │
              │   (Data/app.db)          │
              └──────────────────────────┘
```

## 🛠️ Teknoloji Yığını

| Katman | Teknoloji | Sürüm |
|--------|-----------|-------|
| **Sunum** | ASP.NET Core MVC | 8.0 |
| **API** | ASP.NET Core Web API | 8.0 |
| **İş Mantığı** | C# Services | - |
| **Veri Erişim** | Entity Framework Core | - |
| **Veritabanı** | SQLite | - |
| **Frontend** | Bootstrap 5, jQuery | 5.x |
| **API Dokümantasyonu** | Swagger/OpenAPI | - |

## 📋 Proje Yapısı

```
WMS/
├── WebUI/                      # MVC Sunum Katmanı
│   ├── Controllers/           # MVC Controllers
│   ├── Views/                 # Razor Views
│   ├── wwwroot/              # Statik Dosyalar (CSS, JS, Görseller)
│   ├── Middleware/           # Özel Middleware (Exception Handling)
│   └── appsettings.json      # Konfigürasyon
│
├── WebAPI/                     # REST API Katmanı
│   ├── Controllers/           # API Controllers
│   ├── Program.cs            # DI ve Middleware Konfigürasyonu
│   └── appsettings.json      # API Konfigürasyonu
│
├── Business/                   # İş Mantığı Katmanı
│   ├── Services/             # Business Services (Region, User, vb.)
│   ├── Interfaces/           # Service Interfaces
│   ├── Managers/             # Business Managers
│   └── Utilities/            # Yardımcı Fonksiyonlar
│
├── Core/                       # Veri Modelleri Katmanı
│   ├── Dtos/                 # Data Transfer Objects
│   ├── Entities/             # Database Entities
│   └── Enums/                # Enumeration Türleri
│
├── Data/                       # Veri Erişim Katmanı
│   ├── Context/              # DbContext
│   ├── Migrations/           # EF Core Migrations
│   └── Repositories/         # Generic Repository Pattern
│
└── WMS.sln                     # Visual Studio Solution
```

## 🚀 Hızlı Başlangıç

### Ön Koşullar

- **.NET 8.0 SDK** veya üzeri ([İndir](https://dotnet.microsoft.com/download))
- **Visual Studio 2022** veya **VS Code**
- **Git**

### Kurulum Adımları

#### 1. Projeyi Klonlayın

```bash
git clone <repository-url>
cd WMS
```

#### 2. Çözümü Derleyin

```bash
dotnet build
```

#### 3. Veritabanı Migrasyonlarını Uygulayın

WebAPI başlatıldığında otomatik olarak migrate edilir, ancak manuel olarak yapmak için:

```bash
cd Data
dotnet ef database update --project ../Data/Data.csproj
```

#### 4. Uygulamaları Başlatın

**Seçenek 1: Terminal ile (iki ayrı terminal açın)**

Terminal 1 - WebAPI başlat:
```bash
cd WebAPI
dotnet run
# API başlatılacak: https://localhost:7234
# Swagger UI: https://localhost:7234/swagger
```

Terminal 2 - WebUI başlat:
```bash
cd WebUI
dotnet run
# MVC başlatılacak: https://localhost:7081
```

**Seçenek 2: Visual Studio ile**

1. WebUI'ı Startup Project olarak ayarlayın
2. `F5` tuşuna basın veya "Debug > Start Debugging" seçin
3. WebAPI'yi ayrı bir debug session'da başlatın (WebAPI projesine sağ tık > Debug > Start New Instance)

## 📖 Kullanım

### WebUI (MVC Uygulaması)
- **Adres:** https://localhost:7081
- **İşlevler:**
  - Kullanıcı Girişi (Login)
  - Bölge Yönetimi
  - Şehir Yönetimi
  - Depo Yönetimi
  - Dashboard ve Raporlar

### WebAPI (REST API)
- **Adres:** https://localhost:7234
- **Swagger UI:** https://localhost:7234/swagger
- **Uç Noktalar:**
  - `GET /api/region` - Tüm bölgeleri listele
  - `POST /api/region` - Yeni bölge ekle
  - `PUT /api/region/{id}` - Bölgeyi güncelle
  - `DELETE /api/region/{id}` - Bölgeyi sil
  - [Swagger UI'da diğer endpoint'leri görün]

## 🔧 Konfigürasyon

### WebUI appsettings.json

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7234"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../Data/app.db"
  }
}
```

### WebAPI appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=../Data/app.db"
  }
}
```

## 🔐 Güvenlik

- **Oturum Yönetimi:** HttpContext.Session kullanılmaktadır
- **Hata İşleme:** Global ExceptionHandlingMiddleware ile merkezi hata yönetimi
- **CORS:** WebAPI, WebUI adresinden gelen isteklere izin vermektedir

## 📊 Veritabanı

Sistem **SQLite** veritabanı kullanmaktadır ve şu tablolara sahiptir:

- `Users` - Kullanıcı bilgileri
- `Regions` - Depo bölgeleri
- `Cities` - Şehirler
- `Towns` - İlçeler
- `Products` - Ürün katalog
- `Shelves` - Raf ve depolama birimleri
- `Transfers` - Depo hareketleri
- [Daha fazla tablo Migrations klasöründe]

## 🐛 Hata Ayıklama

### Yaygın Sorunlar

**Problem:** "no such table" SQLite hatası
- **Çözüm:** Veritabanı migrasyonları çalıştırıldığından emin olun

**Problem:** "Connection refused" WebAPI'ye bağlanamama
- **Çözüm:** WebAPI'nin çalıştığından ve port 7234'ün açık olduğundan emin olun

**Problem:** CORS hatası
- **Çözüm:** WebAPI appsettings'te CORS policy'sinin doğru konfigüre edildiğini kontrol edin

## 📝 Geliştirme Rehberi

### Yeni Bir Service Eklemek

1. **Interface Oluşturun** (`Business/Interfaces/INewService.cs`)
2. **Service Sınıfı Yazın** (`Business/Services/NewService.cs`)
3. **Program.cs'e Kaydedin:**
   ```csharp
   builder.Services.AddScoped<INewService, NewService>();
   ```
4. **DTO'ları Oluşturun** (`Core/Dtos/NewDto.cs`)
5. **Entity Oluşturun** (`Core/Entities/New.cs`)
6. **Migration Oluşturun:**
   ```bash
   dotnet ef migrations add AddNewEntity
   ```

### Yeni Bir API Controller'ı Eklemek

1. `WebAPI/Controllers/` dizininde yeni controller oluşturun
2. Base CRUD işlemlerini implement edin
3. DTO'ları kullanarak response dönün
4. Swagger otomatik olarak belgelendirecektir

## 🤝 Katkı

Katkılarınız hoş geldiniz! Lütfen şu adımları izleyin:

1. Depoyu Fork edin
2. Feature branch oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişiklikleri commit edin (`git commit -m 'Yeni özellik eklendi'`)
4. Branch'i push edin (`git push origin feature/YeniOzellik`)
5. Pull Request açın

## 📄 Lisans

Bu proje [MIT Lisansı](LICENSE) altında yayınlanmıştır.

## 📞 İletişim

Sorular veya öneriler için lütfen bir **Issue** açın veya proje yöneticilerine başvurun.

---

**Son Güncelleme:** 25 Ocak 2026  
**Sürüm:** 1.0.0  
**Durum:** 🟢 Aktif Geliştirme
