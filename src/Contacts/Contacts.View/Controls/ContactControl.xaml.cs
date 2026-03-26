using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Contacts.View.Controls
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
        /// DependencyProperty для режима просмотра (только чтение)
        /// </summary>
        public static readonly DependencyProperty IsViewModeProperty =
            DependencyProperty.Register(
                nameof(IsViewMode),
                typeof(bool),
                typeof(ContactControl),
                new PropertyMetadata(false));

        /// <summary>
        /// Режим просмотра (поля только для чтения)
        /// </summary>
        public bool IsViewMode
        {
            get => (bool)GetValue(IsViewModeProperty);
            set => SetValue(IsViewModeProperty, value);
        }

        private void PhoneTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

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

        private bool IsTextAllowed(string text)
        {
            return _phoneRegex.IsMatch(text);
        }
    }
}
