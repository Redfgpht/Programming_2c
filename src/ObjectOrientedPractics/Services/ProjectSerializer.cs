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
        /// Путь хранения данных 
        /// </summary>
        private static readonly string _filePath = "data.json";

        /// <summary>
        /// Сохранение данных
        /// </summary>
        /// <param name="data">Данные для сохранения</param>
        public static void SaveData(object data)
        {
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        public static void LoadData()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }

                string json = File.ReadAllText(_filePath);
                var data = JObject.Parse(json);
                AppData.Items = data["Items"]?.ToObject<List<Item>>() ?? new List<Item>();
                AppData.Customers = data["Customers"]?.ToObject<List<Customer>>() ?? new List<Customer>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке: {ex.Message}", "Ошибка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    
    }
}