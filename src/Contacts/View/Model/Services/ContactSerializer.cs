using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using View.Model;

namespace View.Model.Services
{
    /// <summary>
    /// Предоставляет методы для сериализации и десериализации контактов.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Путь к файлу для сохранения контакта.
        /// </summary>
        private readonly string _filePath;

        /// <summary>
        /// Инициализирует новый экземпляр класса ContactSerializer.
        /// </summary>
        public ContactSerializer()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string contactsDirectory = Path.Combine(documentsPath, "Contacts");

            // Создаем директорию, если она не существует
            if (!Directory.Exists(contactsDirectory))
            {
                Directory.CreateDirectory(contactsDirectory);
            }

            _filePath = Path.Combine(contactsDirectory, "contacts.json");
        }

        /// <summary>
        /// Сохраняет контакт в файл.
        /// </summary>
        /// <param name="contact">Контакт для сохранения.</param>
        public void SaveContact(Contact contact)
        {
            try
            {
                string json = JsonConvert.SerializeObject(contact, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception)
            {
                // Игнорируем ошибки
            }
        }

        /// <summary>
        /// Загружает контакт из файла.
        /// </summary>
        /// <returns>Загруженный контакт или новый контакт, если файл не найден.</returns>
        public Contact LoadContact()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonConvert.DeserializeObject<Contact>(json) ?? new Contact();
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки
            }

            return new Contact();
        }
    }
}