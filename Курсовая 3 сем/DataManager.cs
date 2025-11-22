using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using System.Net.Http;

namespace QR_Generator
{
    public class HistoryItem
    {
        public string ID { get; set; } = "";
        public string Date { get; set; } = "";
        public string Content { get; set; } = "";
    }

    public static class CurrentUser
    {
        public static string Username { get; set; } = "Guest";
        public static bool IsLoggedIn { get; set; } = false;
    }

    public static class DataManager
    {
        private static readonly string BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");
        private static readonly string UsersFile = Path.Combine(BaseFolder, "users.xml");
        private static readonly string HistoryFile = Path.Combine(BaseFolder, "history.xml");

        static DataManager()
        {
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);
        }

        public static async void SendLogToTelegram(string message)
        {
            try
            {
                string botToken = "8499800086:AAFg31zx8W2MWSOcrZCQnBkITrzUD4A_ig4";
                string chatId = "6250975346";

                using HttpClient client = new();
                string url = $"https://api.telegram.org/bot{botToken}/sendMessage?chat_id={chatId}&text={message}";
                await client.GetAsync(url);
            }
            catch { }
        }

        // --- АВТОРИЗАЦИЯ ---

        public static bool Register(string login, string password)
        {
            XDocument doc = LoadOrCreate(UsersFile, "Users");

            if (doc.Root!.Elements("User").Any(u => u.Attribute("Login")?.Value == login))
                return false;

            doc.Root.AddFirst(new XElement("User",
                new XAttribute("Login", login),
                new XAttribute("Password", HashPassword(password))
            ));

            doc.Save(UsersFile);
            SendLogToTelegram($"Новый пользователь: {login}");
            return true;
        }

        public static bool Login(string login, string password)
        {
            if (!File.Exists(UsersFile)) return false;

            var doc = XDocument.Load(UsersFile);
            if (doc.Root == null) return false;

            var user = doc.Root.Elements("User")
                .FirstOrDefault(u => u.Attribute("Login")?.Value == login &&
                                     u.Attribute("Password")?.Value == HashPassword(password));

            if (user != null)
            {
                CurrentUser.Username = login;
                CurrentUser.IsLoggedIn = true;
                return true;
            }
            return false;
        }

        // --- ИСТОРИЯ ---

        public static void SaveHistory(string contentSummary)
        {
            XDocument doc = LoadOrCreate(HistoryFile, "History");

            doc.Root!.AddFirst(new XElement("Scan",
                new XAttribute("ID", Guid.NewGuid().ToString()),
                new XAttribute("Date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")),
                new XAttribute("User", CurrentUser.Username),
                new XAttribute("IsDeleted", "false"),
                new XElement("Content", contentSummary)
            ));

            doc.Save(HistoryFile);
        }

        // Чтение истории с учетом ID
        public static List<HistoryItem> GetUserHistory()
        {
            var list = new List<HistoryItem>();
            if (!File.Exists(HistoryFile)) return list;

            var doc = XDocument.Load(HistoryFile);
            if (doc.Root == null) return list;

            var items = doc.Root.Elements("Scan")
                .Where(x => x.Attribute("User")?.Value == CurrentUser.Username)
                .Where(x => x.Attribute("IsDeleted")?.Value != "true")
                .OrderByDescending(x => x.Attribute("Date")?.Value)
                .Select(x => new HistoryItem
                {
                    ID = x.Attribute("ID")?.Value ?? "", // <--- Читаем ID из XML
                    Date = x.Attribute("Date")?.Value ?? "Unknown",
                    Content = x.Element("Content")?.Value ?? ""
                });

            list.AddRange(items);
            return list;
        }

        // --- НОВЫЕ МЕТОДЫ ДЛЯ УДАЛЕНИЯ ---

        // Удаление одной записи по ID
        public static void DeleteHistoryItem(string id)
        {
            if (!File.Exists(HistoryFile)) return;
            var doc = XDocument.Load(HistoryFile);

            // Ищем запись
            var element = doc.Root?.Elements("Scan")
                .FirstOrDefault(x => x.Attribute("ID")?.Value == id);

            if (element != null)
            {
                element.SetAttributeValue("IsDeleted", "true"); // Удаляем
                doc.Save(HistoryFile); // Сохраняем
            }
        }

        // Очистка всей истории текущего пользователя
        public static void ClearUserHistory()
        {
            if (!File.Exists(HistoryFile)) return;
            var doc = XDocument.Load(HistoryFile);

            if (doc.Root != null)
            {
                var userItems = doc.Root.Elements("Scan")
                    .Where(x => x.Attribute("User")?.Value == CurrentUser.Username);

                foreach (var item in userItems)
                {
                    item.SetAttributeValue("IsDeleted", "true");
                }

                doc.Save(HistoryFile);
            }
        }

        // --- ВСПОМОГАТЕЛЬНЫЕ ---

        private static XDocument LoadOrCreate(string path, string rootName)
        {
            if (File.Exists(path))
            {
                try
                {
                    var doc = XDocument.Load(path);
                    if (doc.Root == null) doc.Add(new XElement(rootName));
                    return doc;
                }
                catch { }
            }
            return new XDocument(new XElement(rootName));
        }

        private static string HashPassword(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}