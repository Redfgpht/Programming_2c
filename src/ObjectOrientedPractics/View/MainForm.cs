using ObjectOrientedPractics.Services;

namespace ObjectOrientedPractics.View
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            LoadData();
            InitializeComponent();
            AppData.IsExitSaving = true;
            ProjectSerializer.EnableAutoSave(this);
        }

        private void LoadData()
        {
            ProjectSerializer.LoadAllData();

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