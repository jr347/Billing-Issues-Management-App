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
using System.Diagnostics;

namespace WpfApp1
{
    //*******************************************************************
    // DESCRIPTION: 	NewRecord is the form for logging a new issue into the Issues DB.
    //                  The form contains various textboxes, comboboxes, checkboxes and datepickers which collect
    //                      all the information needed for a new issue. There is also the option to add a new status note.
    //*******************************************************************
    public partial class NewRecord : Window
    {
        public String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True"; //for SQL server
        private string[] arr;               //local variable to store login-based user data
        private int IDnum;                      //local variable to store issue ID number



        // DESCRIPTION: Constructor, which Takes in user data as input and auto-populates certain fields as the form is loaded, including ADID and name.
        public NewRecord(string[] user_data)
        {
            InitializeComponent();

            arr = user_data;

            ADIDtext.Text = arr[0];
            Nametext.Text = arr[1] + " " + arr[2];
            Roletext.Text = arr[6];
            Managertext.Text = arr[3];
            Startdatepicker.SelectedDate = DateTime.Today;
        }



        // runs on Submit button click, which then inserts data to New_Contacts and History
        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                //  try
                //{
                InsertData_NewContacts(con);

                IDnum = GetIssueID(con);

                InsertData_History(con, IDnum);

                MessageBox.Show("Insert Successful!");
                this.Close();
                /* }
                 catch (Exception ex)
                 {
                     var st = new StackFrame(ex, true);
                     var frame = st.GetFileLineNumber();
                     MessageBox.Show("Error: " + ex.Message);
                 }
                 finally
                 {
                     con.Close();
             }    }*/
            }
        }



            // Pulls the data from fields on the form, and uses them to execute an INSERT query into New_Contacts. Takes existing SqlConnection as input
            private void InsertData_NewContacts(SqlConnection connection)
            {
                string title = TitleText.Text.ToString();
                string assigned_to = arr[2].ToString();
                string req_dept = RequestingDeptComboBox.SelectedItem.ToString();
                string req_name = RequestedbyText.Text.ToString();
                string opened_date = Startdatepicker.SelectedDate.ToString();
                string due_date = Planneddatepicker.SelectedDate.ToString();
                string status = StatusComboBox.SelectedValue.ToString();
                string category = CategoryComboBox.SelectedValue.ToString();
                string BCTINumber = BCTItext1.Text.ToString();
                string sys_impact = SystemComboBox.SelectedItem.ToString();
                string priority_num = Int32.Parse(PriorityText.Text).ToString();
                string supporting_details = SupportingDetailsText.Text.ToString();
                string internal_notes = InternalNotesText.Text.ToString();
                string manager = Managertext.Text.ToString();
                string control_enhancement = ControlEnhancementCheckBox.IsChecked.ToString();
                string process_improvement = ProcessImprovementCheckBox.IsChecked.ToString();
                string impact = ImpacttypeComboBox.SelectedItem.ToString();
                string cim_val = CIMValueAddedCheckBox.IsChecked.ToString();
                string bus_impact = BusinessImpactsText.Text.ToString();
                string onetime_benefit = string.IsNullOrWhiteSpace(OneTimeBenefitText.Text) ? "0": OneTimeBenefitText.Text.ToString();
                string annual_benefit = string.IsNullOrWhiteSpace(AnnualBenefitText.Text) ? "0": AnnualBenefitText.Text.ToString();
                string cim_know = CIMKnowledgeCheckBox.IsChecked.ToString();

                string query = "INSERT INTO New_Issues (Title, Assigned_To, Req_Dept, Req_Name, Opened_Date, Due_Date, [Status], " +
                               "Category, TFS_BC_HDFS_Num, Sys_Impact, Priority_Number, Supporting_Details, Internal_Notes, " +
                               "Manager, [Control], Proc_Imp, Impact, Cim_Val, Bus_Impact, OneTimeBenefit, AnnualBenefit, CIMKnow) " +
                               "VALUES ('" + title + "', '" + assigned_to + "', '" + req_dept + "', '" + req_name + "', '" + opened_date + "', '" + due_date + "', '" + status +
                               "', '" + category + "', " + BCTINumber + ", '" + sys_impact + "', " + priority_num + ", '" + supporting_details + "', '" + internal_notes +
                               "', '" + manager + "', '" + control_enhancement + "', '" + process_improvement + "', '" + impact + "', '" + cim_val +
                               "', '" + bus_impact + "', " + onetime_benefit + ", " + annual_benefit + ", '" + cim_know + "');";

                connection.Open();
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }



            // Pulls the ID number of the issue that was just entered, so that it can be used to add a new status to the History Table (as taskNum). SqlConnection as input.
            private int GetIssueID(SqlConnection connection)
            {
                int y = 0;
                string query2 = "select top 1 (ID) from New_Issues order by ID desc";
                SqlCommand command2 = new SqlCommand(query2, connection);

                SqlDataReader reader2;
                reader2 = command2.ExecuteReader();
                while (reader2.Read())
                {
                    y = reader2.GetInt32(0);
                }
                reader2.Close();

                return y;
            }



            // Takes in ID number of newly-inserted issue and Sql connection, to insert a new status into the History table.
            // Logic included for default values if the user leaves certain fields blank.
            private void InsertData_History(SqlConnection connection, int x)
            {
                string ID = x.ToString();
                string status_date = StatusDatePicker.SelectedDate.ToString();
                string sys_impact = SystemComboBox.SelectedItem.ToString();

                string status_note;
                if (String.IsNullOrEmpty(StatusNoteTextBox.Text.ToString()))
                {
                    status_note = "Added to database as a new Issue.";
                }
                else
                {
                    status_note = StatusNoteTextBox.Text.ToString();
                }

                string comboind;
                if (HistoryStatusComboBox.SelectedIndex < 0)
                {
                    comboind = "Analysis in progress";
                }
                else
                {
                    comboind = (HistoryStatusComboBox.SelectedValue).ToString();
                }

                string query3 = "Insert into History (TaskNum, EntryDate, Status, New_StatusNote) Values(" + ID + ", '" + status_date + "', '" + status_note + "', '" + comboind + "');";
                SqlCommand command3 = new SqlCommand(query3, connection);
                command3.ExecuteNonQuery();
            }



            // DESCRIPTION: Triggered on "Cancel" button click. Has the user confirm that they want to exit the window
            private void CancelButton_Click(object sender, RoutedEventArgs e)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Exit form? All information entered will be cleared.", "Cancel Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }



            // Triggered on "Add New Status" button click. Hides the button and displays fields to add a Status.
            private void MoreInfoButton_Click(object sender, RoutedEventArgs e)
            {
                MoreInfoButton.Visibility = Visibility.Hidden;
                StatusStackPanel.Visibility = Visibility.Visible;
            }



            // DESCRIPTION: Triggered when NewRecord Window loads. String parses the systems of a user, and autofills comboboxes.
            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                FillSystemComboBox(arr[7]);

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

                HistoryStatusComboBox.Items.Add("Item Not Assigned");
                HistoryStatusComboBox.Items.Add("Analysis in Progress");
                HistoryStatusComboBox.Items.Add("Coding in Progress");
                HistoryStatusComboBox.Items.Add("Testing in Progress");
                HistoryStatusComboBox.Items.Add("Pending Verification");
                HistoryStatusComboBox.Items.Add("Scheduled Implementation");
                HistoryStatusComboBox.Items.Add("Work Delayed");
                HistoryStatusComboBox.Items.Add("Waiting on CIM");
                HistoryStatusComboBox.Items.Add("CIM Knowledge");
                HistoryStatusComboBox.Items.Add("Waiting for Other Group");
                HistoryStatusComboBox.Items.Add("Resolved");

                StatusDatePicker.SelectedDate = DateTime.Today;

                StatusStackPanel.Visibility = Visibility.Hidden;
            }


            // Delimits a user's systems by '/' and adds the systems to the ComboBox in the form
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



            // Implements logic that displays Status options only available to specific Categories (BC's vs. non-BC's)
            private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
            {
                if (CategoryComboBox.SelectedIndex == 0)
                {
                    StatusComboBox.Items.Clear();
                    StatusComboBox.Items.Add("BC Approved");
                    StatusComboBox.Items.Add("Active");
                    StatusComboBox.Items.Add("Deferred");
                    StatusComboBox.Items.Add("Dropped");
                    StatusComboBox.Items.Add("Implemented");
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


        }


    }

