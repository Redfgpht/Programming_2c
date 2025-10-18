using ObjectOrientedPractics.Services;
using System.Windows.Forms;

namespace ObjectOrientedPractics.View
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            LoadData();
            InitializeComponent();
            ProjectSerializer.EnableAutoSave(this);
        }

        private void LoadData()
        {
            ProjectSerializer.LoadData();

            if (AppData.Items.Count == 0)
            {
                CreateSampleData();
            }
        }

        private void CreateSampleData()
        {
            // Тестовые данные
            AppData.Items.Add(ItemFactory.CreateRandomItem());
            AppData.Customers.Add(CustomerFactory.CreateRandomCustomer());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (tabControl1.SelectedIndex)
            {
                case 2:
                    //cartsTab1.UpdateUI();
                    break;
                case 3:
                    ordersTab1.UpdateOrdersList();
                    break;

            }
        }
    }
}