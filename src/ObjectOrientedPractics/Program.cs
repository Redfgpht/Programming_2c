using ObjectOrientedPractics.View;

namespace ObjectOrientedPractics
{
    internal static class Program
    {
        /// <summary>
        ///  ������� ���� ����������
        /// </summary>
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}