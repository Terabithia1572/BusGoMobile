using SQLite;
using BusGoMobile.Models;

namespace BusGoMobile.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _db;

    private const string DbFileName = "BusGoDb.db";

    // Bağlantıyı hazırla: gömülü dosyayı yazılabilir klasöre kopyala, sonra bağlan
    private async Task InitAsync()
    {
        if (_db is not null)
            return;

        string targetPath = @"C:\Users\Yunus\Downloads\BusGoDb.db";

        _db = new SQLiteAsyncConnection(targetPath);

        // Tablolar yoksa otomatik oluşturulmasını sağla:
        await _db.CreateTableAsync<User>();
        await _db.CreateTableAsync<Category>();
        await _db.CreateTableAsync<City>();

        System.Diagnostics.Debug.WriteLine($"VERITABANI YOLU: {targetPath}");
    }

    // Yeni kullanıcı ekle (ham INSERT query'si)
    public async Task<int> AddUserAsync(User user)
    {
        await InitAsync();

        return await _db.ExecuteAsync(
            @"INSERT INTO User (FirstName, LastName, Email, Phone, Password, MarketingOptIn, CreatedAt)
              VALUES (?, ?, ?, ?, ?, ?, ?);",
            user.FirstName, user.LastName, user.Email, user.Phone,
            user.Password, user.MarketingOptIn, user.CreatedAt);
    }

    // E-posta zaten kayıtlı mı?
    public async Task<bool> EmailExistsAsync(string email)
    {
        await InitAsync();

        int count = await _db.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM User WHERE Email = ?;", email);

        return count > 0;
    }

    // Login için: e-posta + şifre ile kullanıcı bul
    public async Task<User> GetUserAsync(string email, string password)
    {
        await InitAsync();

        var results = await _db.QueryAsync<User>(
            "SELECT * FROM User WHERE Email = ? AND Password = ? LIMIT 1;",
            email, password);

        return results.FirstOrDefault();
    }

    // Tüm kategorileri getir
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await InitAsync();

        return await _db.QueryAsync<Category>("SELECT * FROM Category ORDER BY Id;");
    }

    // Tüm şehirleri getir
    public async Task<List<City>> GetCitiesAsync()
    {
        await InitAsync();

        return await _db.QueryAsync<City>("SELECT * FROM City ORDER BY Name;");
    }


}
