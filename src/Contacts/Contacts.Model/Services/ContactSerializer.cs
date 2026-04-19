using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Contacts.Model;

namespace Contacts.Model.Services;

/// <summary>
/// Предоставляет методы для сериализации и десериализации коллекции контактов.
/// </summary>
public class ContactSerializer
{
    private readonly string _filePath;

    /// <summary>
    /// Инициализирует новый экземпляр класса ContactSerializer.
    /// </summary>
    public ContactSerializer()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string contactsDirectory = Path.Combine(documentsPath, "Contacts");

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
    /// <exception cref="IOException">Возникает при ошибке записи в файл.</exception>
    /// <exception cref="JsonException">Возникает при ошибке сериализации.</exception>
    public void SaveContacts(IEnumerable<Contact> contacts)
    {
        string json = JsonConvert.SerializeObject(contacts, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }

    /// <summary>
    /// Загружает коллекцию контактов из файла.
    /// </summary>
    /// <returns>Загруженная коллекция контактов или пустая коллекция, если файл не найден.</returns>
    /// <exception cref="IOException">Возникает при ошибке чтения файла.</exception>
    /// <exception cref="JsonException">Возникает при ошибке десериализации.</exception>
    public List<Contact> LoadContacts()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Contact>>(json) ?? new List<Contact>();
        }

        return new List<Contact>();
    }
}