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
    // DESCRIPTION: 	This window is for the user to add or edit a status for an open item. This form is navigated to directly from the EditRecord form,
    //                      by either clicking the "Add a status" button or by double-clicking the row of an existing status (to edit it).
    //                  Each of these options has a different AddEditStatus constructor and runs different code.
    //                  Regardless of if the status is new or is being edited, on a successful submit, this window will close, and the updates will be displayed in 
    //                      EditRecord, as we passed the parent Editrecord window itself to this window.
    //*******************************************************************
    public partial class EditRecord_AddEditStatus : Window
    {
        public String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True";
        private DataRowView pbsRow;     //DataRowView variable that stores the row from Prioritization by system
        private DataRowView histRow;    //DataRowView variable that stores the row from the History data that is displayed on the edit form. It is sent here on row double-click.
        private bool isNewStatus;       //Local boolean value that checks if the user is trying to add a new status (true) or edit an existing status (false)
        private EditRecord form;        //Holds the parent EditRecord window currently Open in the applicaiton, which will be updated once the status is added/edited



        //*******************************************************************
        // DESCRIPTION: Constructor for AddEditStatus that can only be called when "Add a Status" is clicked in the EditRecord form.
        //              Does not pre-populate any fields aside from today's date.
        //              Sets isNewStatus to true.
        //
        // INPUT:       EditRecord editRecord: this is the parent EditRecord Window on which the "Add a status" button was clicked.
        //              DataRowView priorBySystemRow: this is the prioritizationBysystem row that contains the data used to query for & populate the parent EdiRecord form
        //*******************************************************************
        public EditRecord_AddEditStatus(EditRecord editRecord, DataRowView priorBySystemRow)
        {
            InitializeComponent();
            
            form = editRecord;
            pbsRow = priorBySystemRow;
            StatusDatePicker.SelectedDate = DateTime.Today;

            isNewStatus = true;
            
            Fill_HistoryStatusComboBox();
        }




        //*******************************************************************
        // DESCRIPTION: Constructor for AddEditStatus that can only be called when an existing row in the datagrid of the EditRecord form is double-clicked.
        //              Does not pre-populate any fields aside from today's date.
        //              Sets isNewStatus to false.
        //
        // INPUT:       EditRecord editRecord: this is the parent EditRecord Window on which the "Add a status" button was clicked.
        //              DataRowView priorBySystemRow: this is the prioritizationBysystem row that contains the data used to query for & populate the parent EdiRecord form
        //              DataRowView statusDataRow: this is the row in EditRecord form that was double-clicked on to call this constructor and pre-populate this form.
        //*******************************************************************
        public EditRecord_AddEditStatus(EditRecord editRecord, DataRowView priorBySystemRow, DataRowView statusDataRow)
        {
            InitializeComponent();
            
            form = editRecord;
            pbsRow = priorBySystemRow;
            histRow = statusDataRow;
            
            isNewStatus = false;
            
            Fill_HistoryStatusComboBox();
            
            HistoryStatusComboBox.SelectedValue = histRow["Status"].ToString();
            StatusNoteText.Text = histRow["Status_Note"].ToString();

            if (!DateTime.TryParse(histRow["EntryDate"].ToString(), out DateTime myDate))
            {
            }
            else
            {
                StatusDatePicker.SelectedDate = myDate;
            }
        }


        private void Fill_HistoryStatusComboBox()
        {
            HistoryStatusComboBox.Items.Add("Item Not Assigned");
            HistoryStatusComboBox.Items.Add("Analysis in Progress");
            HistoryStatusComboBox.Items.Add("Coding in Progress");
            HistoryStatusComboBox.Items.Add("Testing in Progress");
            HistoryStatusComboBox.Items.Add("Pending Verification");
            HistoryStatusComboBox.Items.Add("Scheduled Implementation");
            HistoryStatusComboBox.Items.Add("Work Delayed");
            HistoryStatusComboBox.Items.Add("Waiting on CIM");
            HistoryStatusComboBox.Items.Add("Waiting for Other Group");
            HistoryStatusComboBox.Items.Add("Resolved");
            HistoryStatusComboBox.Items.Add("CIM Knowledge");
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



        //*******************************************************************
        // DESCRIPTION: Runs when the user clicks the 'Submit' button. Checks if isNewStatus is true or false.
        //              If isNewStatus is true, it will run an insert query into the History table since the user is adding a new status.
        //              if isNewStatus is false, it will run an update query to the History table since the user is editing an existing status.
        //*******************************************************************
        private void SubmitIssueButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNewStatus)
            {
                Insert_HistoryTable();
            }
            
            else
            {
                Update_HistoryTable();
            }
        }



        // Runs the Insert query to the history table for the new status being added
        private void Insert_HistoryTable()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    string TaskNum = pbsRow[10].ToString();
                    string date = StatusDatePicker.SelectedDate.ToString();
                    string sysstr = form.SystemComboBox.SelectedItem.ToString();
                    string stusnt = StatusNoteText.Text;
                    int comboind = (HistoryStatusComboBox.SelectedIndex + 1);
                    string query = "Insert into History Values(" + TaskNum + ", '" + date + "', '" + sysstr + "', '" + stusnt + "', " + comboind.ToString() + ");";

                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    int result = cmd.ExecuteNonQuery();

                    MessageBox.Show("Insert successful!");
                    this.Close();
                    form.BindDataGrid(TaskNum);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
        }



        // Runs an uodate query to the history table to make changes to the already-existing status
        private void Update_HistoryTable()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    string TaskNum = pbsRow[10].ToString();
                    string date = StatusDatePicker.SelectedDate.ToString();
                    string sysstr = form.SystemComboBox.SelectedItem.ToString();
                    string stusnt = StatusNoteText.Text;
                    int comboind = (HistoryStatusComboBox.SelectedIndex + 1);
                    string histID = histRow["ID"].ToString();
                    string query = "UPDATE History SET EntryDate='" + date + "', [Status]='" + stusnt + "', StatusNote=" + comboind + ", [Group]='" + sysstr + "' " +
                                   "WHERE ID=" + histID + " AND TaskNum=" + TaskNum;
                    
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Update successful!");
                    this.Close();
                    form.BindDataGrid(TaskNum);
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

    }
}
