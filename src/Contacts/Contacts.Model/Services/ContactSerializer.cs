using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Contacts.Model;

namespace Contacts.Model.Services
{
    /// <summary>
    /// Предоставляет методы для сериализации и десериализации коллекции контактов.
    /// </summary>
    public class ContactSerializer
    {
        /// <summary>
        /// Путь к файлу для сохранения контактов.
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
        /// Сохраняет коллекцию контактов в файл.
        /// </summary>
        /// <param name="contacts">Коллекция контактов для сохранения.</param>
        public void SaveContacts(IEnumerable<Contact> contacts)
        {
            try
            {
                string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception)
            {
                // Игнорируем ошибки
            }
        }

        /// <summary>
        /// Загружает коллекцию контактов из файла.
        /// </summary>
        /// <returns>Загруженная коллекция контактов или пустая коллекция, если файл не найден.</returns>
        public List<Contact> LoadContacts()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    string json = File.ReadAllText(_filePath);
                    return JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки
            }

            return new List<Contact>();
        }
    }
}