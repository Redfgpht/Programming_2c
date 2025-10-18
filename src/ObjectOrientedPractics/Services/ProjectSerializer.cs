using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics.Services
{
    /// <summary>
    /// Класс для работы с json.
    /// </summary>
    public static class ProjectSerializer
    {
        private static FormClosingEventHandler _autoSaveHandler;

        /// <summary>
        /// Путь хранения данных о предметах.
        /// </summary>
        private static readonly string _filePathItems = "dataItems.json";

        /// <summary>
        /// Путь хранения данных о пользователях.
        /// </summary>
        private static readonly string _filePathCustomers = "dataCustomers.json";

        /// <summary>
        /// Сохранение всех данных в соответствующие файлы.
        /// </summary>
        public static void SaveAllData()
        {
            try
            {
                // Сохранение предметов
                if (AppData.Items != null)
                {
                    string itemsJson = JsonConvert.SerializeObject(AppData.Items, Formatting.Indented);
                    File.WriteAllText(_filePathItems, itemsJson);
                }

                // Сохранение пользователей
                if (AppData.Customers != null)
                {
                    string customersJson = JsonConvert.SerializeObject(AppData.Customers, Formatting.Indented);
                    File.WriteAllText(_filePathCustomers, customersJson);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Сохранение данных о предметах.
        /// </summary>
        /// <param name="items">Список предметов для сохранения.</param>
        public static void SaveItems(List<Item> items)
        {
            try
            {
                string json = JsonConvert.SerializeObject(items, Formatting.Indented);
                File.WriteAllText(_filePathItems, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Сохранение данных о пользователях.
        /// </summary>
        /// <param name="customers">Список пользователей для сохранения.</param>
        public static void SaveCustomers(List<Customer> customers)
        {
            try
            {
                string json = JsonConvert.SerializeObject(customers, Formatting.Indented);
                File.WriteAllText(_filePathCustomers, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка всех данных из файлов.
        /// </summary>
        public static void LoadAllData()
        {
            LoadItems();
            LoadCustomers();
        }

        /// <summary>
        /// Загрузка данных о предметах.
        /// </summary>
        public static void LoadItems()
        {
            try
            {
                if (!File.Exists(_filePathItems))
                {
                    AppData.Items = new List<Item>();
                    return;
                }

                string json = File.ReadAllText(_filePathItems);
                AppData.Items = JsonConvert.DeserializeObject<List<Item>>(json) ?? new List<Item>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предметов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppData.Items = new List<Item>();
            }
        }

        /// <summary>
        /// Загрузка данных о пользователях.
        /// </summary>
        public static void LoadCustomers()
        {
            try
            {
                if (!File.Exists(_filePathCustomers))
                {
                    AppData.Customers = new List<Customer>();
                    return;
                }

                string json = File.ReadAllText(_filePathCustomers);
                AppData.Customers = JsonConvert.DeserializeObject<List<Customer>>(json) ?? new List<Customer>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppData.Customers = new List<Customer>();
            }
        }

        /// <summary>
        /// Автоматическое сохранение при закрытии приложения.
        /// </summary>
        /// <param name="mainForm">Форма при закрытии которой будут сохраняться данные.</param>
        public static void EnableAutoSave(Form mainForm)
        {




            _autoSaveHandler = (s, e) =>
            {
                if (AppData.IsExitSaving)
                {
                    MessageBox.Show("Данные успешно сохранены!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    SaveData(new { Items = AppData.Items, Customers = AppData.Customers, IsExitSaving = AppData.IsExitSaving });
                }
            };

            mainForm.FormClosing += _autoSaveHandler;
        }
    }
}