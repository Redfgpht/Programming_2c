using ObjectOrientedPractics.View;

namespace ObjectOrientedPractics
{
    internal static class Program
    {
        /// <summary>
        ///  Главное окно приложения
        /// </summary>
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}