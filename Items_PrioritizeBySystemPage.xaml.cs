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
    //*******************************************************************
    // DESCRIPTION: 	This page displays the prioritization of open items by system.
    //                  The user is given a combobox of systems they belong to, and chooses one.
    //                  Upon choosing, a query is run to pull the necessary data and display it in the page on a datagrid.
    //                  In each Open Item row, there is an Edit button, which on click takes the user to the EditRecord form,
    //                      with the existing data on that specific issue.
    //*******************************************************************
    public partial class Items_PrioritizeBySystemPage : Page
    {
        public String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True";
        private string[] arr;                       //local variable to store login-based user data
        private DataRowView priorBySystemRow;       //local variable to store the row of data in the 'Prioritization by System' DataGrid
        


        public Items_PrioritizeBySystemPage(string[] user_data)
        {
            InitializeComponent();
            
            arr = user_data;

            FillSystemComboBox(arr[7]);

            DataScroll.Visibility = Visibility.Collapsed;
            Report.Visibility = Visibility.Collapsed;
            
        }



        //*******************************************************************
        // DESCRIPTION: Parses the string containing the user's systems, delimited by '/',
        //                  and fills the System combobox with these various systems.
        //              This will become important as the system chosen here drives the results of the query on this page.
        //*******************************************************************
        private void FillSystemComboBox(string systemString)
        {
            char delimiter = '/';
            string[] sys = systemString.Split(delimiter);

            int len = sys.Length;
            for (int x = 0; x < len; x++)
            {
                SystemComboBox.Items.Add(sys[x]);
            }
            SystemComboBox.Items.Add("CIM");
        }




        //*******************************************************************
        // DESCRIPTION: Function that runs the Prioritization by System query and fills the data grid with the result table.
        //              First, the SELECT query is run to pull the data on the open items. 
        //                  The system is specified by the system chosen in the combobox.
        //              Then, a SQLDataAdapter is used to fill the datatable with these results. 
        //              See Items_PrioritizationBySystemPage.xaml for more on data binding. Note that the names of the result columns
        //                  match the names of the binding columns. That is how the query result table is connected to the datagrid.
        //
        // INPUT:       string sys: this string specifies the system whose issues the user is trying to view. Is passed into the query
        //*******************************************************************
        public void BindDataGrid(string sys)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    string query = "SELECT Priority_Number, Sys_Impact as [System], Category, TFS_BC_HDFS_Num as BID_ID, " +
                                    "Assigned_To as [Owner], FORMAT(Opened_Date,'MM/dd/yyyy') as Req, [Status], Title, " +
                                    "Impact, IIf(Completed_Date Is Not Null, DATEDIFF(DAY, Opened_Date, Completed_Date), DATEDIFF(DAY, Opened_Date, Getdate())) as [Days], ID " +
                                    "FROM New_Issues Where (Sys_Impact like '%" + sys + "%' AND [Status]!= 'Closed' AND [Status]!= 'Deferred' AND [Status]!= 'Implemented' AND [Status]!= 'Dropped') " +
                                    "ORDER BY Priority_Number ASC, TFS_BC_HDFS_Num;";
                    
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


        
        //*******************************************************************
        // DESCRIPTION: Runs when the user selects a system from the combobox.
        //              The datagrid becomes visible on the page, and BindDataGrid(string sys) is called,
        //                  running the Prioritization by System query and filling the datagrid with the results.
        //                  passing the value of the ComboBox that was just chosen as the string parameter.
        //              On the page, the user will see the datagrid with all the open items for the particular system they chose.
        //*******************************************************************
        private void SystemComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataScroll.Visibility = Visibility.Visible;
            Report.Visibility = Visibility.Visible;
            
            BindDataGrid(SystemComboBox.SelectedItem.ToString());
        }




        //*******************************************************************
        // DESCRIPTION: Runs when the user clicks the "Edit" button in one of the datagrid rows.
        //              On that button click, the data from that row of the datatable is pulled as a DataRowView object, named priorbySystemRow.
        //              An instance of the EditRecord form is then created, passing:
        //                      1) this page itself, which is so that the updates can be completed
        //                      2) login-based user data arr (string[] object)
        //                      3) prioritization-by-system data priorBySystemRow (DataRowView object)
        //              The user is then taken to the EditRecord form, where the data of that particular issue auto-populates the form.
        //*******************************************************************
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //On Edit Button click, pulls the data from that row of the datagrid, and stores it as a DataRowView object
                priorBySystemRow = (DataRowView)((Button)e.Source).DataContext;
                
                // this PrioritizeBySystemPage, is being passed so it can be updated
                //priorBySystemRow is a DataRowView object containing the data from that row of PBS datagrid
                EditRecord editRecord = new EditRecord(this, arr, priorBySystemRow);
                editRecord.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}