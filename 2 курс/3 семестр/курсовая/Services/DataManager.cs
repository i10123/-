using QR_generator.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace QR_generator.Services
{
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


        // АВТОРИЗАЦИЯ
        public static bool Register(string login, string password)
        {
            XDocument doc = LoadOrCreate(UsersFile, "Users");

            // ищем внутри <Users>, есть ли уже <User> с таким Login
            if (doc.Root!.Elements("User").Any(u => u.Attribute("Login")?.Value == login))
                return false;

            doc.Root.AddFirst(
                new XElement("User", 
                new XAttribute("Login", login), 
                new XAttribute("Password", HashPassword(password))));

            doc.Save(UsersFile);
            return true;
        }

        public static bool Login(string login, string password)
        {
            if (!File.Exists(UsersFile)) 
                return false;

            var doc = XDocument.Load(UsersFile);
            if (doc.Root == null) 
                return false;

            // Ищем пользователя, у которого совпадает и логин, и хэш пароля
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


        // ИСТОРИЯ
        public static void SaveHistory(string contentSummary)
        {
            XDocument doc = LoadOrCreate(HistoryFile, "History");

            doc.Root!.AddFirst(
                new XElement("QR",
                    new XAttribute("ID", Guid.NewGuid().ToString()),
                    new XAttribute("Date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")),
                    new XAttribute("User", CurrentUser.Username),
                    new XAttribute("IsDeleted", "false"),

                new XElement("Content", Encode(contentSummary))
            ));

            doc.Save(HistoryFile);
        }

        public static List<HistoryItem> GetUserHistory()
        {
            var list = new List<HistoryItem>();
            if (!File.Exists(HistoryFile)) 
                return list;

            var doc = XDocument.Load(HistoryFile);
            if (doc.Root == null) 
                return list;

            var items = doc.Root.Elements("QR")
                .Where(x => x.Attribute("User")?.Value == CurrentUser.Username)
                .Where(x => x.Attribute("IsDeleted")?.Value != "true")
                .OrderByDescending(x => x.Attribute("Date")?.Value)
                .Select(x => new HistoryItem
                {
                    ID = x.Attribute("ID")?.Value ?? "",
                    Date = x.Attribute("Date")?.Value ?? "Unknown",
                    Content = Decode(x.Element("Content")?.Value ?? "")
                });

            list.AddRange(items);
            return list;
        }


        // Удаление одной записи по ID
        public static void DeleteHistoryItem(string id)
        {
            if (!File.Exists(HistoryFile)) 
                return;
            var doc = XDocument.Load(HistoryFile);

            var element = doc.Root?.Elements("QR").FirstOrDefault(x => x.Attribute("ID")?.Value == id);

            if (element != null)
            {
                element.SetAttributeValue("IsDeleted", "true");
                doc.Save(HistoryFile);
            }
        }

        // Очистка всей истории текущего пользователя
        public static void ClearUserHistory()
        {
            if (!File.Exists(HistoryFile))
                return;

            var doc = XDocument.Load(HistoryFile);

            if (doc.Root != null)
            {
                var userItems = doc.Root.Elements("QR").Where(x => x.Attribute("User")?.Value == CurrentUser.Username);

                foreach (var item in userItems)
                    item.SetAttributeValue("IsDeleted", "true");

                doc.Save(HistoryFile);
            }
        }


        private static XDocument LoadOrCreate(string path, string rootName)
        {
            if (File.Exists(path))
            {
                try
                {
                    var doc = XDocument.Load(path);

                    if (doc.Root == null) 
                        doc.Add(new XElement(rootName));

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


        private static string Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private static string Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                return Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch { 
                return "Ошибка чтения";
            }
        }
    }
}