# e-Appointment Uygulaması
e-Appointment, Angular, .NET ve PostgreSQL kullanılarak geliştirilmiş bir elektronik doktor randevu uygulamasıdır. Uygulama, kullanıcıların randevu oluşturmasını, doktorların ve personelin sisteme eklenmesini sağlar. Admin, kullanıcı yönetimini ve diğer kritik işlemleri gerçekleştirir.

## Özellikler
### Admin Paneli:
Kullanıcı, doktor ve personel ekleme.
### Sisteme ilk giriş için gerekli admin bilgileri:
#### <li>Kullanıcı Adı: admin
#### <li>Şifre: 12
## Kullanıcı İşlemleri:
Kullanıcılar sisteme kayıt olabilir ve giriş yapabilir.
Doktor randevusu oluşturabilir.
# Doktor İşlemleri:
Doktorlar randevu takvimlerini görüntüleyebilir.
# Kullanılan Teknolojiler
<li>Frontend: Angular</li>
<li>Backend: .NET Core</li>
<li>Veritabanı: PostgreSQL</li>

# Projenin Çalıştırılması
Proje, client (Angular uygulaması) ve server (.NET API) olmak üzere iki ayrı klasörden oluşmaktadır.
### 1.Projenin server klasörüne gidin ve aşağıdaki komutu çalıştırın:
<li>dotnet restore</li>
  
### 2.Veritabanını Ayarlayın
<p>PostgreSQL'de bir veritabanı oluşturun ve bağlantı dizesini appsettings.json dosyasında güncelleyin:</p>

#### "ConnectionStrings": {"PostgreServer": "Server=localhost; Port=PORT;Database=DbName;UserId=postgres; Password=PASSWORD"}
### 3. Migration ve Veritabanı Güncelleme
dotnet run
### 4. API’yi Başlatın
dotnet run

## Frontend (Client) Çalıştırma
#### 1.Bağımlılıkları Yükleyin
Projenin client klasörüne gidin ve aşağıdaki komutu çalıştırın:
<p>npm install</p>

#### 2. Angular Uygulamasını Başlatın
ng serve

## Kullanıcı Girişi
Sisteme giriş yapmak için başlangıçta admin bilgilerini kullanabilirsiniz:

#### <li>Kullanıcı Adı: admin
#### <li>Şifre: 12
