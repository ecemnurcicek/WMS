# 📦 WMS - Depo Yönetim Sistemi

Depo operasyonlarını yönetmek için tasarlanmış, modern mimariyle inşa edilmiş bir **Depo Yönetim Sistemi (Warehouse Management System)**.

## 🎯 Proje Amacı

WMS, depoların günlük operasyonlarını yönetmeyi, ürün stoklarını takip etmeyi, mağazalar arası transferleri koordine etmeyi ve rol bazlı erişim kontrolü ile depo süreçlerini otomatikleştirmeyi amaçlamaktadır. Sistem, iyi tanımlanmış API katmanı ile bölüm bazında ölçeklenebilir ve bakımı kolay bir yapıya sahiptir.

## 🏗️ Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────────┐
│                    WebUI (MVC) - Port 7004                  │
│          (ASP.NET Core, Bootstrap 5, Font Awesome 6)        │
└────────────────────────┬────────────────────────────────────┘
                         │
               HttpClient + ApiProxyController
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
   │Services │  │(DTOs,Entities)│  │(EF Core,     │
   │Repos    │  │              │  │ Identity)    │
   └─────────┘  └──────────────┘  └──────────────┘
        │                                 │
        └─────────────────┬───────────────┘
                          ▼
              ┌──────────────────────────┐
              │   SQLite Database        │
              │   (Data/app.db)          │
              └──────────────────────────┘
```

### API Proxy Mimarisi

WebUI, WebAPI'ye istemci tarafından doğrudan erişim yerine **ApiProxyController** aracılığıyla `/api/**` isteklerini sunucu tarafında WebAPI'ye yönlendirir. Bu sayede CORS sorunları minimize edilir ve merkezi hata yönetimi sağlanır.

## 🛠️ Teknoloji Yığını

| Katman | Teknoloji | Sürüm |
|--------|-----------|-------|
| **Sunum** | ASP.NET Core MVC | 8.0 |
| **API** | ASP.NET Core Web API | 8.0 |
| **İş Mantığı** | C# Services + Generic Repository | - |
| **Kimlik Doğrulama** | ASP.NET Core Identity | 8.0 |
| **Veri Erişim** | Entity Framework Core | 8.0.22 |
| **Veritabanı** | SQLite | - |
| **Frontend** | Bootstrap 5, jQuery, Font Awesome 6 | 5.3.3 / 6.6.0 |
| **Şifreleme** | ASP.NET Identity PasswordHasher | - |
| **API Dokümantasyonu** | Swagger/OpenAPI (Swashbuckle) | 6.6.2 |

## 📋 Proje Yapısı

```
WMS/
├── WebUI/                      # MVC Sunum Katmanı
│   ├── Controllers/           # 15 MVC Controller (+ ApiProxy)
│   ├── Views/                 # Razor Views (16 klasör)
│   ├── ViewComponents/        # SidebarViewComponent (Dinamik Menü)
│   ├── wwwroot/              # CSS (14), JS (13), Kütüphaneler
│   ├── Middleware/           # ExceptionHandlingMiddleware
│   ├── Models/               # View modelleri
│   └── appsettings.json      # Konfigürasyon
│
├── WebAPI/                     # REST API Katmanı
│   ├── Controllers/           # 10 API Controller
│   ├── Program.cs            # DI, CORS, Middleware Konfigürasyonu
│   └── appsettings.json      # API Konfigürasyonu
│
├── Business/                   # İş Mantığı Katmanı
│   ├── Services/             # 17 Business Service
│   ├── Interfaces/           # 17 Service Interface
│   ├── Repositories/         # Generic Repository (IRepository<T>)
│   ├── Managers/             # BusinessManager (Login + Log)
│   ├── Exceptions/           # 5 Özel Exception Tipi
│   └── Utilities/            # PasswordHasherUtil
│
├── Core/                       # Veri Modelleri Katmanı
│   ├── Dtos/                 # 28+ Data Transfer Object
│   ├── Entities/             # 21 Entity Sınıfı
│   └── Enums/                # Enum Türleri
│
├── Data/                       # Veri Erişim Katmanı
│   ├── Context/              # ApplicationContext (IdentityDbContext)
│   ├── Migrations/           # 7 EF Core Migration
│   ├── DatabaseSeeder.cs     # Menü ve Rol Seed Verileri
│   └── Configurations/       # Entity Konfigürasyonları
│
└── WMS.sln                     # Visual Studio Solution
```

## ✨ Özellikler

### Yönetim Modülleri
- **Dashboard** — Özet istatistikler ve genel bakış
- **Bölge Yönetimi** — Bölge CRUD işlemleri
- **Şehir Yönetimi** — Bölgeye bağlı şehir yönetimi
- **İlçe Yönetimi** — Şehre bağlı ilçe yönetimi
- **Marka Yönetimi** — Marka CRUD işlemleri
- **Mağaza Yönetimi** — İlçeye bağlı mağaza yönetimi
- **Depo Yönetimi** — Mağazaya bağlı depo yönetimi
- **Raf Yönetimi** — Depoya bağlı raf yönetimi (kaskat bölge→şehir→ilçe→mağaza→depo filtresi)
- **Kullanıcı Yönetimi** — Kullanıcı CRUD, marka ve mağaza atama

### Ürün Yönetimi
- **Ürün Kataloğu** — Model/Renk/Beden bazında ürün opsiyon sistemi
- **Ürün-Raf Ataması** — Ürünleri belirli raflara miktar ile atama (ProductShelf)
- **Stok Özeti** — Opsiyon bazında stok durumu ve mağaza stok bilgisi
- **Ürün Arama** — Anasayfadan ürün arama

### Transfer Yönetimi
- **Transfer Oluşturma** — Mağazalar arası ürün transferi
- **Transfer Detayları** — Transfer alt kalemleri yönetimi
- **Durum Takibi** — Transfer durumu güncelleme (status management)
- **Hızlı Transfer** — Tek adımda mağazalar arası hızlı transfer (Quick Transfer)
- **Gelen/Giden Transferler** — Mağaza bazında gelen ve giden transfer filtreleme

### Giriş Alanı (Entry Area)
- **Raf Atama** — Gelen transferlerdeki ürünleri raflara atama iş akışı

### Rol Bazlı Erişim
- **Dinamik Menü Sistemi** — Kullanıcı rolüne göre menü öğeleri otomatik filtreleme
- **Roller:** Admin, Marka Sorumlusu, Mağaza Sorumlusu, Satış Temsilcisi
- **Session Tabanlı Kimlik Doğrulama** — Rol ID, Mağaza ID, Marka ID oturum bilgileri

### Altyapı
- **Merkezi Hata Yönetimi** — ExceptionHandlingMiddleware ile otomatik hata loglama
- **Özel Exception Hiyerarşisi** — BusinessException, ValidationException, NotFoundException, DataAccessException, BusinessRuleException
- **Veritabanı Loglama** — Tüm hatalar Log tablosuna kaydedilir
- **Generic Repository Pattern** — Tüm servisler için tekrar kullanılabilir CRUD altyapısı

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
# MVC başlatılacak: https://localhost:7004
```

**Seçenek 2: Visual Studio ile**

1. Solution'a sağ tıklayın → "Configure Startup Projects" → Multiple startup projects seçin
2. WebAPI ve WebUI'ı "Start" olarak ayarlayın
3. `F5` tuşuna basın veya "Debug > Start Debugging" seçin

## 📖 Kullanım

### WebUI (MVC Uygulaması)
- **Adres:** https://localhost:7004
- **Modüller:**
  - Kullanıcı Girişi (Login) — Özel login layout
  - Dashboard — Genel istatistikler
  - Bölge / Şehir / İlçe Yönetimi
  - Marka Yönetimi
  - Mağaza / Depo / Raf Yönetimi
  - Ürün Yönetimi (Opsiyonlar, Bedenler, Raf Ataması)
  - Transfer Yönetimi (Detaylar, Durum, Hızlı Transfer)
  - Giriş Alanı (Raf Atama)
  - Kullanıcı Yönetimi
  - Anasayfa Ürün Arama

### WebAPI (REST API)
- **Adres:** https://localhost:7234
- **Swagger UI:** https://localhost:7234/swagger
- **API Controller'ları:**

| Controller | Endpoint | Temel İşlemler |
|------------|----------|----------------|
| **Region** | `/api/region` | CRUD |
| **City** | `/api/city` | CRUD + Bölgeye göre filtreleme |
| **Town** | `/api/town` | CRUD + Şehre göre filtreleme |
| **Brand** | `/api/brand` | CRUD |
| **Shop** | `/api/shop` | CRUD + İlçeye göre filtreleme, Aktif mağazalar |
| **WareHouse** | `/api/warehouse` | CRUD + Mağazaya göre filtreleme |
| **Shelf** | `/api/shelf` | CRUD + Depoya göre filtreleme |
| **Product** | `/api/product` | CRUD + Opsiyonlar, Bedenler, Stok özeti, Raf ataması, Mağaza stok |
| **Transfer** | `/api/transfer` | CRUD + Detaylar, Durum güncelleme, Hızlı transfer, Gelen/Giden |
| **User** | `/api/user` | CRUD + Roller |

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

- **Oturum Yönetimi:** HttpContext.Session (30 dakika timeout) ile kullanıcı, rol, mağaza ve marka bilgilerini saklama
- **Şifre Güvenliği:** ASP.NET Identity PasswordHasher ile şifre hashleme ve doğrulama
- **Hata İşleme:** Global ExceptionHandlingMiddleware ile merkezi hata yönetimi ve veritabanı loglama
- **Rol Bazlı Erişim:** Dinamik menü sistemi ile kullanıcının rolüne göre erişim kontrolü
- **CORS:** WebAPI, belirli origin'lerden gelen isteklere izin vermektedir
- **API Proxy:** WebUI üzerinden tüm API istekleri sunucu tarafında proxy edilerek istemci tarafı CORS sorunları önlenir

## 📊 Veritabanı

Sistem **SQLite** veritabanı kullanmaktadır. `ApplicationContext`, `IdentityDbContext` üzerinden türetilmiştir.

### Tablolar

| Tablo | Açıklama |
|-------|----------|
| `Users` | Kullanıcı bilgileri (marka ve mağaza ilişkisi) |
| `Roles` | Kullanıcı rolleri (Admin, Marka Sorumlusu, vb.) |
| `UserRoles` | Kullanıcı-rol ilişkileri |
| `Menus` | Dinamik menü öğeleri |
| `MenuRoles` | Menü-rol erişim ilişkileri |
| `Regions` | Bölgeler |
| `Cities` | Şehirler |
| `Towns` | İlçeler |
| `Brands` | Markalar |
| `Shops` | Mağazalar |
| `Warehouses` | Depolar |
| `Shelves` | Raflar |
| `Products` | Ürün kataloğu |
| `ProductShelves` | Ürün-raf atamaları (miktar bilgisi) |
| `ProductTransactions` | Ürün hareket kayıtları |
| `Transfers` | Mağazalar arası transferler |
| `TransferDetails` | Transfer alt kalemleri |
| `Logs` | Hata ve işlem logları |
| *Identity Tabloları* | AspNetUsers, AspNetRoles, AspNetUserClaims, vb. |

### Migrations Geçmişi

| Migration | Tarih | Açıklama |
|-----------|-------|----------|
| InitialCreate | 19.01.2026 | İlk veritabanı şeması |
| ProductDescriptionAndCoverUrlText | 30.01.2026 | Ürün açıklama ve kapak URL alanları |
| ProductPriceReal | 30.01.2026 | Ürün fiyat alanı düzenleme |
| AddUpdateFieldsToTransferDetail | 09.02.2026 | Transfer detay güncelleme alanları |
| TransferStatusIntMigration | 09.02.2026 | Transfer durumu int tipine dönüştürme |
| AddMenuAndMenuRoles | 09.02.2026 | Dinamik menü ve rol-menü sistemi |
| AddBrandIdToUser | 13.02.2026 | Kullanıcı-marka ilişkisi |

## 🐛 Hata Ayıklama

### Özel Exception Hiyerarşisi

Sistem 5 farklı exception tipi kullanır, tümü `BusinessException` sınıfından türetilir:

- **BusinessException** — Genel iş mantığı hataları (LineNumber, ControllerName, OccurredAt içerir)
- **ValidationException** — Doğrulama hataları
- **NotFoundException** — Kayıt bulunamadı hataları
- **DataAccessException** — Veri erişim hataları
- **BusinessRuleException** — İş kuralı ihlalleri

### Yaygın Sorunlar

**Problem:** "no such table" SQLite hatası
- **Çözüm:** Veritabanı migrasyonları çalıştırıldığından emin olun

**Problem:** "Connection refused" WebAPI'ye bağlanamama
- **Çözüm:** WebAPI'nin çalıştığından ve port 7234'ün açık olduğundan emin olun

**Problem:** CORS hatası
- **Çözüm:** WebAPI Program.cs'te CORS policy'sinin doğru konfigüre edildiğini kontrol edin

**Problem:** Menü görünmüyor
- **Çözüm:** DatabaseSeeder'ın çalıştığından ve kullanıcının bir role atandığından emin olun

## 📝 Geliştirme Rehberi

### Servis Katmanları (17 Servis)

| Servis | Açıklama |
|--------|----------|
| LoginService | Kimlik doğrulama |
| LogService | Hata ve işlem loglama |
| UserService | Kullanıcı yönetimi |
| UserRoleService | Kullanıcı-rol atamaları |
| DashboardService | Dashboard istatistikleri |
| RegionService | Bölge yönetimi |
| CityService | Şehir yönetimi |
| TownService | İlçe yönetimi |
| BrandsService | Marka yönetimi |
| ShopsService | Mağaza yönetimi |
| WareHouseService | Depo yönetimi |
| ShelfService | Raf yönetimi |
| ProductService | Ürün ve opsiyon yönetimi |
| TransferService | Transfer yönetimi |
| EntryAreaService | Giriş alanı raf atama |
| MenuService | Dinamik menü yönetimi |
| MenuRoleService | Menü-rol erişim yönetimi |

### Yeni Bir Service Eklemek

1. **Interface Oluşturun** (`Business/Interfaces/INewService.cs`)
2. **Service Sınıfı Yazın** (`Business/Services/NewService.cs`)
3. **Program.cs'e Kaydedin** (WebAPI ve/veya WebUI):
   ```csharp
   builder.Services.AddScoped<INewService, NewService>();
   ```
4. **DTO'ları Oluşturun** (`Core/Dtos/NewDto.cs`)
5. **Entity Oluşturun** (`Core/Entities/New.cs`)
6. **DbContext'e DbSet Ekleyin** (`Data/Context/ApplicationContext.cs`)
7. **Migration Oluşturun:**
   ```bash
   dotnet ef migrations add AddNewEntity --project Data
   ```

### Yeni Bir API Controller'ı Eklemek

1. `WebAPI/Controllers/` dizininde yeni controller oluşturun
2. Base CRUD işlemlerini implement edin
3. DTO'ları kullanarak response dönün
4. Swagger otomatik olarak belgelendirecektir
5. WebUI'dan erişim için ApiProxyController otomatik olarak yönlendirme yapar

### Yeni Menü Eklemek

1. `DatabaseSeeder.cs` dosyasında yeni menü tanımlayın
2. İlgili roller için `MenuRole` kaydı ekleyin
3. WebUI'daki `SidebarViewComponent` menüyü otomatik olarak gösterecektir

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

**Son Güncelleme:** 13 Şubat 2026  
**Sürüm:** 1.1.0  
**Durum:** 🟢 Aktif Geliştirme
