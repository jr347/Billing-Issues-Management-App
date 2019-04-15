using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfApp1
{
    // Will be used for Search Items functionality, did not have add anything to this page yet.
    public partial class Items_SearchItemsPage : Page
    {
        public String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True";
        private string[] arr;                       //local variable to store login-based user data
        private DataRowView priorBySystemRow;       //local variable to store the row of data in the 'Prioritization by System' DataGrid
        private string title_;
        private string cat_;
        private string bid_;


        public Items_SearchItemsPage(string[] user_data)
        {
            InitializeComponent();

            arr = user_data;

            DataScroll.Visibility = Visibility.Collapsed;
            Report.Visibility = Visibility.Collapsed;

        }

        public void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            title_ = TitleBox.Text.ToString();
            if (CategoryComboBox.SelectedItem == null)
            {
                cat_ = "";
            }
            else
            {
                cat_ = CategoryComboBox.SelectedItem.ToString();
            }
            bid_ = BIDBox.Text.ToString();

            DataScroll.Visibility = Visibility.Visible;
            Report.Visibility = Visibility.Visible;

            BindDataGrid(title_, cat_, bid_);

        }


        public void BindDataGrid(string t, string c, string b)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    string query;

                    if (b.Length > 0)
                    {
                        query = "SELECT Priority_Number, Sys_Impact as [System], Category, TFS_BC_HDFS_Num as BID_ID, " +
                                    "Assigned_To as [Owner], FORMAT(Opened_Date,'MM/dd/yyyy') as Req, [Status], Title, " +
                                    "Impact, IIf(Completed_Date Is Not Null, DATEDIFF(DAY, Opened_Date, Completed_Date), DATEDIFF(DAY, Opened_Date, Getdate())) as [Days], ID " +
                                    "FROM New_Issues Where Title like '%" + t + "%' AND Category like '%" + c + "%' AND BIDNumber like '%" + b + "%'";
                    }
                    else
                    {
                        query = "SELECT Priority_Number, Sys_Impact as [System], Category, TFS_BC_HDFS_Num as BID_ID, " +
                                    "Assigned_To as [Owner], FORMAT(Opened_Date,'MM/dd/yyyy') as Req, [Status], Title, " +
                                    "Impact, IIf(Completed_Date Is Not Null, DATEDIFF(DAY, Opened_Date, Completed_Date), DATEDIFF(DAY, Opened_Date, Getdate())) as [Days], ID " +
                                    "FROM New_Issues Where Title like '%" + t + "%' AND Category like '%" + c + "%'";
                    }

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);

                    DataTable dt = new DataTable();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    using (sda)
                    {
                        sda.Fill(dt);
                    }
                    Report.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    con.Close();
                }
        }
        public void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //On Edit Button click, pulls the data from that row of the datagrid, and stores it as a DataRowView object
                priorBySystemRow = (DataRowView)((Button)e.Source).DataContext;

                //priorBySystemRow is a DataRowView object containing the data from that row of PBS datagrid
                EditRecord editRecord = new EditRecord(this, arr, priorBySystemRow);
                editRecord.Show();
                //MessageBox.Show(priorBySystemRow[1].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CategoryComboBox.Items.Add("");
            CategoryComboBox.Items.Add("BC / TI");
            CategoryComboBox.Items.Add("Defect");
            CategoryComboBox.Items.Add("HDFS");
            CategoryComboBox.Items.Add("Inquiry");
            CategoryComboBox.Items.Add("Issue");
            CategoryComboBox.Items.Add("Strategic Task");
            CategoryComboBox.Items.Add("Task");

        }
    }


}
