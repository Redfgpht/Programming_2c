using ObjectOrientedPractics.Model;
using ObjectOrientedPractics.Services;
using ObjectOrientedPractics.View.Dialogs;

namespace ObjectOrientedPractics.View.Tabs
{
    public partial class CustomersTab : UserControl
    {
        /// <summary>
        /// Конкретный покупатель
        /// </summary>
        private Customer _currentCustomer;

        public CustomersTab()
        {
            InitializeComponent();
            UpdateUI();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        private void UpdateUI()
        {
            CustomerListBox.Items.Clear();
            AppData.Customers.ForEach(x => CustomerListBox.Items.Add(x));

        }

        /// <summary>
        /// Редактирование данных
        /// </summary>
        private void UpdateTextProperty(Customer item)
        {
            IdTextBox.Text = item.Id.ToString();
            FullNameTextBox.Text = item.FullName;
            if (_currentCustomer.IsPriority)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

        }

        /// <summary>
        /// Очистка введеных полей
        /// </summary>
        private void ClearTextProperty()
        {
            IdTextBox.Text = string.Empty;
            FullNameTextBox.Text = string.Empty;
            AddressTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Выбор покупателя
        /// </summary>
        private void CustomerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CustomerListBox.Items != null && CustomerListBox.SelectedItem != null)
            {
                _currentCustomer = CustomerListBox.SelectedItem as Customer;
                UpdateTextProperty(_currentCustomer);
            }
        }

        /// <summary>
        /// Переименование пользователя
        /// </summary>
        private void FullNameTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _currentCustomer.FullName = FullNameTextBox.Text;
                FullNameTextBox.BackColor = AppColors.BaseInput;
            }
            catch
            {
                FullNameTextBox.BackColor = AppColors.ErrorInput;
            }
        }

        /// <summary>
        /// Изменение адреса
        /// </summary>
        private void AddressTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _currentCustomer.Address = AddressTextBox.Text;
                AddressTextBox.BackColor = AppColors.BaseInput;
            }
            catch
            {
                AddressTextBox.BackColor = AppColors.ErrorInput;
            }
        }

        /// <summary>
        /// Добавление покупателя
        /// </summary>
        private void AddBtn_Click(object sender, EventArgs e)
        {
            if (new CustomerAddForm().ShowDialog() == DialogResult.OK)
            {
                UpdateUI();
                MessageBox.Show("Вы добавили нового покупателя!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Удаление покупателя
        /// </summary>
        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            if (CustomerListBox.Items.Count > 0)
            {
                if (MessageBox.Show($"Вы уверены что хотите удалить {_currentCustomer.FullName}?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ClearTextProperty();
                    AppData.Customers.Remove(_currentCustomer);
                    UpdateUI();
                    _currentCustomer = null;
                    FullNameTextBox.BackColor = AppColors.BaseInput;
                    AddressTextBox.BackColor = AppColors.BaseInput;
                }
            }
        }

        /// <summary>
        /// Запрет на ввод данных
        /// </summary>
        private void InputData(object sender, KeyPressEventArgs e)
        {
            ValueValidator.InterdictionInputData(sender, e);
        }

        /// <summary>
        /// Добавление рандомного покупателя
        /// </summary>
        private void AddRandomBtn_Click(object sender, EventArgs e)
        {
            AppData.Customers.Add(CustomerFactory.CreateRandomCustomer());
            UpdateUI();
            MessageBox.Show("Вы добавили случайного покупателя!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                _currentCustomer.IsPriority = true;
            }
            else
            {
                _currentCustomer.IsPriority = false;
            }
        }
    }
}