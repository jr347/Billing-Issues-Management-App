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
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace WpfApp1
{
    //*******************************************************************
    // DESCRIPTION: 	The EditRecord Window allows for the user to edit information about an Issue in the database.
    //                  Takes login-based data and prioritization by system row DataRowView from PrioritizationBySystem (PBS) Page.
    //                  Upon loading, the fields of the form auto-populate based on data that is currently stored for the Issue.
    //                  When the user is finished making changes, they have the ability to click "Submit," which will update edited fields in the database
    //                  This form is also role-driven, so certain functionalities are only available to Managers and not Users.
    //                  Users may also add or edit a status note. To Add, click the "Add Status" button. To edit, double click the row user wishes to edit.
    //                          Will take the user to an AddEditStatus window to make these changes
    //*******************************************************************
    public partial class EditRecord : Window
    {
        public String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True";
        private DataRowView priorBySystemRow;           //holds data sent here by Prioritization by System
        private string[] arr;                           //holds login-based user data
        private string[] issue_data;                    //Holds the data about the issue, that will be used to populate the form when it loads
        private Page page;      //Holds the parent prioritization by system page currenty open in the application, which will be updated when the issue is edited.
        


        //*******************************************************************
        // DESCRIPTION: Initializes the EditRecord window, using login-based and PBS data. It calls other functions that fill in the form with existing data.
        //              Also takes as parameter the parent PBS page, where updates will be visible after the edits are made.
        //
        // INPUT:       PrioritizeBySystemPage prioritizeBySystemPage : PBS page currently open in the app. Edits will be made visible on this page after they are submitted.
        //              string[] user_data : login-based user data
        //              DataRowView prioritizationBySystemResultRow : row of PBS table that is sent after clicking 'Edit' button
        //*******************************************************************
        public EditRecord(Page prioritizeBySystemPage, string[] user_data, DataRowView prioritizationBySystemResultRow)
        {
            InitializeComponent();
            page = new Page();
            page = prioritizeBySystemPage;
            arr = user_data;
            priorBySystemRow = prioritizationBySystemResultRow;
            
            ADIDtext.Text = arr[0];
            Nametext.Text = arr[1] + " " + arr[2];
            Roletext.Text = arr[6];
            Managertext.Text = arr[3];
            
            SelectIssueData();
            FillInForm();
            BindDataGrid(priorBySystemRow[10].ToString());
        }



        //Runs when the window is loaded. Prepares comboboxes and checks user's role to set content visibilities.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemComboBox.Items.Add("CDC");
            SystemComboBox.Items.Add("IB");
            SystemComboBox.Items.Add("FCB");
            SystemComboBox.Items.Add("EBCM");
            SystemComboBox.Items.Add("eBilling");
            SystemComboBox.Items.Add("FBR");
            SystemComboBox.Items.Add("DOC");
            SystemComboBox.Items.Add("BRRS");
            SystemComboBox.Items.Add("EBA");
            SystemComboBox.Items.Add("CRIS");
            SystemComboBox.Items.Add("MDC");
            SystemComboBox.Items.Add("ABR");
            SystemComboBox.Items.Add("BIS");
            SystemComboBox.Items.Add("BAT");
            SystemComboBox.Items.Add("PS");
            SystemComboBox.Items.Add("ODBI");
            SystemComboBox.Items.Add("IFA");
            SystemComboBox.Items.Add("BFR");
            SystemComboBox.Items.Add("CIM");
            SystemComboBox.Items.Add("SOX");
            SystemComboBox.Items.Add("PMC");
            SystemComboBox.Items.Add("Vendor");
            SystemComboBox.Items.Add("BWS");



            CategoryComboBox.Items.Add("BC / TI");
            CategoryComboBox.Items.Add("Defect");
            CategoryComboBox.Items.Add("HDFS");
            CategoryComboBox.Items.Add("Inquiry");
            CategoryComboBox.Items.Add("Issue");
            CategoryComboBox.Items.Add("Strategic Task");
            CategoryComboBox.Items.Add("Task");

            RequestingDeptComboBox.Items.Add("Americas F&A");
            RequestingDeptComboBox.Items.Add("Applications");
            RequestingDeptComboBox.Items.Add("Asia F&A");
            RequestingDeptComboBox.Items.Add("Brokerage");
            RequestingDeptComboBox.Items.Add("Bus Dev");
            RequestingDeptComboBox.Items.Add("Call Center");
            RequestingDeptComboBox.Items.Add("Canada");
            RequestingDeptComboBox.Items.Add("CIM");
            RequestingDeptComboBox.Items.Add("Cust Tech");
            RequestingDeptComboBox.Items.Add("Eur F&A");
            RequestingDeptComboBox.Items.Add("FR&P");
            RequestingDeptComboBox.Items.Add("GBS");
            RequestingDeptComboBox.Items.Add("Internal Audit");
            RequestingDeptComboBox.Items.Add("Marketing");
            RequestingDeptComboBox.Items.Add("Pricing/IAS");
            RequestingDeptComboBox.Items.Add("PUNE");
            RequestingDeptComboBox.Items.Add("Rev Rec");
            RequestingDeptComboBox.Items.Add("US F&A");
            RequestingDeptComboBox.Items.Add("Other - TBD");


            if (arr[6] != "Manager")
            {
                ManagerReviewCheckBox.IsEnabled = false;
                BCApprovedCheckBox.IsEnabled = false;
                HotTopicCheckBox.IsEnabled = false;
                ManagerNotesText.IsReadOnly = true;
            }
        }



        // Changes content of Impact type combobox based on user's selection of Category.
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedIndex == 0)
            {
                StatusComboBox.Items.Clear();
                StatusComboBox.Items.Add("BC Approved");
                StatusComboBox.Items.Add("Active");
                StatusComboBox.Items.Add("Implemented");
                StatusComboBox.Items.Add("Deferred");                 
                StatusComboBox.Items.Add("Dropped");
                StatusComboBox.Items.Add("Pending");
                StatusComboBox.Items.Add("Submit");

                StatusComboBox.SelectedValue = "Pending";

                ImpacttypeComboBox.Items.Clear();
                ImpacttypeComboBox.Items.Add("Cost Savings");
                ImpacttypeComboBox.Items.Add("Compliance");
                ImpacttypeComboBox.Items.Add("New Revenue");
                ImpacttypeComboBox.Items.Add("Quality");
            }
            else
            {
                StatusComboBox.Items.Clear();
                StatusComboBox.Items.Add("Active");
                StatusComboBox.Items.Add("App Review");
                StatusComboBox.Items.Add("Closed");
                StatusComboBox.Items.Add("Pending");

                StatusComboBox.SelectedValue = "Pending";

                ImpacttypeComboBox.Items.Clear();
                ImpacttypeComboBox.Items.Add("Bad Bill");
                ImpacttypeComboBox.Items.Add("Not Billed Items");
                ImpacttypeComboBox.Items.Add("ISMT");
                ImpacttypeComboBox.Items.Add("Incentive Setup");
                ImpacttypeComboBox.Items.Add("Invoice Display");
                ImpacttypeComboBox.Items.Add("Reporting Issue");
                ImpacttypeComboBox.Items.Add("Compliance");
                ImpacttypeComboBox.Items.Add("Tech Request");
                ImpacttypeComboBox.Items.Add("Abend/Failure");
                ImpacttypeComboBox.Items.Add("Out of Balance");
                ImpacttypeComboBox.Items.Add("Quoting");
                ImpacttypeComboBox.Items.Add("Rate Fail");
                ImpacttypeComboBox.Items.Add("Other");
            }
        }



        // Executes an UPDATE query in SQL server Issues table, with all the new/edited values on the form.
        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    connection.Open();
                    
                    string ID = priorBySystemRow[10].ToString();
                    
                    string plannedDate;
                    string compDate;
                    if (Planneddatepicker.Text.Length == 0)
                    {
                        plannedDate = "NULL";
                    }
                    else
                    {
                        plannedDate = "'" + Planneddatepicker.SelectedDate.ToString() + "'";
                    }

                    if (CompDatePicker.Text.Length == 0)
                    {
                        compDate = "NULL";
                    }
                    else
                    {
                        compDate = "'" + CompDatePicker.SelectedDate.ToString() + "'";
                    }
                    
                    string query =  "UPDATE New_Issues SET Title = '" + TitleText.Text + "', " + "Req_Dept='" + RequestingDeptComboBox.SelectedItem.ToString() + "', " + "Req_Name='" + RequestedbyText.Text.ToString() + "', " + 
                                    "Opened_Date='" + Startdatepicker.SelectedDate.ToString() + "', " + "Due_Date=" + plannedDate + ", " + "Completed_Date=" + compDate +", " +
                                    "[Status]='" + StatusComboBox.SelectedItem.ToString() + "', " + "Category='" + CategoryComboBox.SelectedItem.ToString() + "', " + "TFS_BC_HDFS_Num=" + BCTItext1.Text.ToString() + ", " +
                                    "Sys_Impact='" + SystemComboBox.SelectedItem.ToString() + "', " + "Priority_Number=" + PriorityText.Text.ToString() + ", " + "Supporting_Details='" + SupportingDetailsText.Text.ToString() + "', " +
                                    "Internal_Notes='" + InternalNotesText.Text.ToString() + "', " + "BC_Approved='" + BCApprovedCheckBox.IsChecked.ToString() + "', " + "Hot_Topic='" + HotTopicCheckBox.IsChecked.ToString() + "', " +
                                    "Mgr_Notes='" + ManagerNotesText.Text.ToString() + "', " + "[Control]='" + ControlEnhancementCheckBox.IsChecked.ToString() + "', " + "Proc_Imp='" + ProcessImprovementCheckBox.IsChecked.ToString() + "', " +
                                    "Impact='" + ImpacttypeComboBox.SelectedItem.ToString() + "', " + "Cim_Val='" + CIMValueAddedCheckBox.IsChecked.ToString() + "', " + "Bus_Impact='" + BusinessImpactsText.Text.ToString() + "', " +
                                    "OneTimeBenefit=" + OneTimeBenefitText.Text.ToString() + ", " + "AnnualBenefit=" + AnnualBenefitText.Text.ToString() + " " + 
                                    "WHERE ID =" + ID + ";";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();


                    MessageBox.Show("Update Successful!");
                    this.Close();

                    //Re-binds the DataGrid on the Prioritization by System grid. Any changes made to this issue will be reflected in the DataGrid in that window
                    //page.BindDataGrid(SystemComboBox.SelectedItem.ToString());
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
        }



        // Asks user to confirm that they wish to leave the form if they click 'Cancel'
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Exit form? All information entered will be cleared.", "Cancel Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.OK)
            {
                this.Close();
            }
        }



        // Logic that sets an issue's status to Closed or Implemented based on if the user selects a completion date
        private void CompDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompDatePicker.Text.Length == 0)
            {
            }
            //I preset that "Implemented" for BC's and "Closed" for all other categories are both at index 2
            else
            {
                StatusComboBox.SelectedIndex = 2;
            }
        }


        
        // DESCRIPTION: Logic that sets an issue's completion date to the current date if the user changes the status to 'Closed' or 'Implemented'
        private void StatusComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //I preset that "Implemented" for BC's and "Closed" for all other categories are both at index 2
            if (StatusComboBox.SelectedIndex == 2)
            {
                CompDatePicker.SelectedDate = DateTime.Today;
            }
        }



        //*******************************************************************
        // DESCRIPTION: Function that runs the SELECT query in SQL server to pull the necessary issue data to populate Edit form. 
        //              Stores the results of the query in our string[] class variable issue_data
        //*******************************************************************
        private void SelectIssueData()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
                    try
                    {
                        string query = "SELECT Title, Req_Dept, Req_Name, Opened_Date, Due_Date, [Status], Category, TFS_BC_HDFS_Num, Sys_Impact, Priority_Number, " +
                                           "Supporting_Details, Internal_Notes, BC_Approved, Hot_Topic, ISNULL(Mgr_Notes, '') as Mgr_Notes, [Control], Proc_Imp, Impact, Cim_Val, Bus_Impact, " +
                                           "OneTimeBenefit, AnnualBenefit FROM New_Issues WHERE ID="+ priorBySystemRow[10].ToString() + ";";
                     
                        connection.Open();
                        SqlCommand command = new SqlCommand(query, connection);
                    
                        SqlDataReader reader = command.ExecuteReader();
                        int cols = reader.FieldCount;
                        string[] data = new string[cols];
                        while (reader.Read())
                        {
                            for (int x = 0; x < cols; x++)
                            {
                                data[x] = reader.GetValue(x).ToString();
                            }

                        }
                        reader.Close();

                        connection.Close();
                    
                        issue_data = data;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error:" + ex.Message);
                        string[] data = new string[21];
                        issue_data = data;
                    }
                    finally
                    {
                        connection.Close();
                    }
        }



        // Populates the fields of the Edit form using the string[] issue_data. Includes appropriate pasring/error handling for certain fields.
        private void FillInForm()
        {
            TitleText.Text = issue_data[0].ToString();
            RequestingDeptComboBox.SelectedItem = issue_data[1].ToString();
            RequestedbyText.Text = issue_data[2].ToString();

            //Parses strings containing dates
            if (!DateTime.TryParse(issue_data[3], out DateTime myDate))
            {
            }
            else { Startdatepicker.SelectedDate = myDate; }

            if (!DateTime.TryParse(issue_data[4], out DateTime myDate2))
            { 
            }
            else { Planneddatepicker.SelectedDate = myDate2; }


            StatusComboBox.SelectedItem = issue_data[5].ToString();
            CategoryComboBox.SelectedItem = issue_data[6].ToString();
            BCTItext1.Text = issue_data[7].ToString();
            SystemComboBox.SelectedItem = issue_data[8].ToString();
            PriorityText.Text = issue_data[9].ToString();
            SupportingDetailsText.Text = issue_data[10].ToString();
            InternalNotesText.Text = issue_data[11].ToString();

            if (issue_data[12].ToString() == "True")
            {
                BCApprovedCheckBox.IsChecked = true;
            }
            if (issue_data[13].ToString() == "True")
            {
                HotTopicCheckBox.IsChecked = true;
            }

            ManagerNotesText.Text = issue_data[14].ToString();

            if (issue_data[15].ToString() == "True")
            {
                ControlEnhancementCheckBox.IsChecked = true;
            }
            if (issue_data[16].ToString() == "True")
            {
                ProcessImprovementCheckBox.IsChecked = true;
            }

            ImpacttypeComboBox.SelectedItem = issue_data[17].ToString();

            if (issue_data[18].ToString() == "True")
            {
                CIMValueAddedCheckBox.IsChecked = true;
            }

            BusinessImpactsText.Text = issue_data[19].ToString();
            OneTimeBenefitText.Text = issue_data[20].ToString();
            AnnualBenefitText.Text = issue_data[21].ToString();
        }



        // Passes this Window and prioritization by system row to a new AddEditStatus window, then displays it.
        private void AddStatusButton_Click(object sender, RoutedEventArgs e)
        {
            EditRecord_AddEditStatus addStatus = new EditRecord_AddEditStatus(this, priorBySystemRow);
            addStatus.Show();
        }



        //*******************************************************************
        // Runs the SELECT query in SQL server to pull the necessary history data for particular issue, specified by TaskNum input.
        //  Fills in the DataGrid on this window with the results of this query.
        //  Displays EntryDate, Status, and StatusNote.
        //
        // INPUT:       string TaskNum : accepts the TaskNumber for the SELECT query. TaskNum in the History table is equivalent to ID in New_Issues Table.
        //*******************************************************************
        public void BindDataGrid(string TaskNum)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
                try
                {
                    string query = "select format(EntryDate, 'MM/dd/yyyy') as EntryDate, New_StatusNote as [Status], [Status] as Status_Note " +
                                   "from History where TaskNum = " + TaskNum + "order by History.ID desc;";
                    
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        sda.Fill(dt);
                    }
                    Report.ItemsSource = dt.DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
        }


        //*******************************************************************
        // DESCRIPTION: Runs when a row of Report datagrid is double-clicked. This pulls the data from that row and opens an AddEditRecord window,
        //                  passing that data along in the constructor so it can auto-populate upon loading.
        //              Also passes this window itself to that form, so that this EditRecord Window can update once the status is edited,
        //                  as well as PBS DataRowView.
        //*******************************************************************
        private void Report_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {   
            try
            {
                DataGrid dg = (DataGrid)sender;
                DataRowView dataRow = dg.SelectedItem as DataRowView;

                if (dataRow != null)
                {
                    //Pass this window, priorBySystem DataRowView, and the DataRowView that was generated from the double-click to a new AddEditStatus window
                    EditRecord_AddEditStatus editStatus = new EditRecord_AddEditStatus(this, priorBySystemRow, dataRow);
                    
                    editStatus.Show();
                    editStatus.Topmost = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }

}

