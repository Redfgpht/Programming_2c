using ObjectOrientedPractics.Services;

namespace ObjectOrientedPractics.View
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            LoadData();
            InitializeComponent();
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

        private void itemsTabs1_Load(object sender, EventArgs e)
        {

        }
    }
}