using ObjectOrientedPractics.Model;

namespace ObjectOrientedPractics.Services
{
    /// <summary>
    /// Набор данных приложения
    /// </summary>
    public static class AppData
    {

        /// <summary>
        /// Список предметов
        /// </summary>
        public static List<Item> Items { get; set; } = new List<Item>();

        /// <summary>
        /// Список покупателей
        /// </summary>
        public static List<Customer> Customers { get; set; } = new List<Customer>();
    }
}