using Newtonsoft.Json;
using System;
using System.IO;

namespace View.Model.Services
{
    /// <summary>
    /// Класс для сериализации и десериализации контактов в JSON.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Путь к папке с файлами контактов.
        /// </summary>
        private readonly string _folderPath;

        /// <summary>
        /// Имя файла для сохранения контакта.
        /// </summary>
        private const string FileName = "contacts.json";

        /// <summary>
        /// Конструктор по умолчанию. Устанавливает путь в папку "Мои документы\Contacts".
        /// </summary>
        public ContactSerializer()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _folderPath = Path.Combine(documentsPath, "Contacts");

            // Создаем папку, если её нет
            if (!Directory.Exists(_folderPath))
            {
                Directory.CreateDirectory(_folderPath);
            }
        }

        /// <summary>
        /// Полный путь к файлу.
        /// </summary>
        private string FilePath => Path.Combine(_folderPath, FileName);

        /// <summary>
        /// Сохраняет контакт в файл.
        /// </summary>
        /// <param name="contact">Контакт для сохранения.</param>
        public void SaveContact(Contact contact)
        {
            string json = JsonConvert.SerializeObject(contact, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        /// <summary>
        /// Загружает контакт из файла.
        /// </summary>
        /// <returns>Загруженный контакт или новый контакт, если файл не найден.</returns>
        public Contact LoadContact()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<Contact>(json) ?? new Contact();
            }

            return new Contact();
        }
    }
}
