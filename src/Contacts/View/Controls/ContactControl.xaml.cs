using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace View.Controls
{
    /// <summary>
    /// Логика взаимодействия для ContactControl.xaml
    /// </summary>
    public partial class ContactControl : UserControl
    {
        private static readonly Regex _phoneRegex = new(@"^[0-9+\-\(\)\s]*$");

        public ContactControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обрабатывает ввод текста в поле телефона, разрешая только допустимые символы
        /// </summary>
        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Обрабатывает вставку текста из буфера обмена в поле телефона
        /// </summary>
        private void PhoneTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        /// <summary>
        /// Проверяет, содержит ли текст только допустимые символы для телефона
        /// </summary>
        private bool IsTextAllowed(string text)
        {
            return _phoneRegex.IsMatch(text);
        }
    }
}
