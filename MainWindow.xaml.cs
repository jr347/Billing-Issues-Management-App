﻿using System;
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
    // DESCRIPTION: 	The main login screen that appears when the application starts.
    //                  Identifies if login information is valid. Throws a message box if not, and asks user to try again.
    //                  Passes login-based information about user to Main Menu on successful login.
    //*******************************************************************
    public partial class MainWindow : Window
    {
        private String connectionString = "Data Source=svrp0006ca66;Initial Catalog=Johnny_DB;Integrated Security=True";
        private int x;
        private string[] user_data;



        public MainWindow()
        {
            InitializeComponent();
        }



        private void Cancelbutton_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        

        private void Submitbutton_Click(object sender, RoutedEventArgs e)
        {
            AttemptLogin();
        }


        
        // Checks if the user presses Return (Enter) key in ADID or Password box, which then triggers AttemptLogin to start login verification.
        private void Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AttemptLogin();
            }
        }



        //*******************************************************************
        //  Runs on any attempt the user makes to log into the application.
        //  Captures the user data and passes it to Subwindow1 if login is valid. Closes current window and opens main menu window.
        //  On an invalid login, prompts the user to try again.
        //*******************************************************************
        private void AttemptLogin()
        {
            if (AdidPass_IsValid())
            {
                user_data = FillUserData();
                UserMenu_Window userMenu = new UserMenu_Window(user_data);
                this.Close();
                userMenu.Show();
            }
            else
            {
                MessageBox.Show("Login failed. Try again.");
                ADIDtext.Clear();
                Passwordtext.Clear();
            }
        }



        // Opens a SQL connection. Returns true if there is exactly 1 valid ADID/pw combination
        private bool AdidPass_IsValid()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    con.Open();
                    x = ExecuteLogin_GetADIDPasswordCombos(con);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message);
                }
                finally
                {
                    con.Close();
                }
            return (x == 1);
        }


        //*******************************************************************
        // Takes an open SQL connection as input, and queries the New_Contacts table using SQL parameters for security.
        // Returns the number of valid ADID/PW combos as an int based on user input of ADID and Password.
        //*******************************************************************
        private int ExecuteLogin_GetADIDPasswordCombos(SqlConnection con)
        {
            string query1 = "select count(*) from New_Contacts where ADID = @ADID  and Password = @Pass";
            SqlCommand cmd1 = new SqlCommand(query1, con);

            var Adid = new SqlParameter("@ADID", SqlDbType.VarChar, 50);
            var Pass = new SqlParameter("@Pass", SqlDbType.VarChar, 100);
            Adid.Value = ADIDtext.Text.ToString();
            Pass.Value = Passwordtext.Password.ToString();
            cmd1.Parameters.Add(Adid);
            cmd1.Parameters.Add(Pass);

            int y = 0;
            SqlDataReader reader1 = cmd1.ExecuteReader();
            while (reader1.Read())
            {
                y = reader1.GetInt32(0);
            }
            reader1.Close();
            return y;
        }



        //*******************************************************************
        // Queries the New_Contacts table and pulls several data fields on particular user.
        // Returns a string[] containing the data on the user.
        //*******************************************************************
        private string[] FillUserData()
        {
            string[] query_results;
            using (SqlConnection con = new SqlConnection(connectionString))
                try
                {
                    con.Open();
                    string query2 = "select top 1 ADID, First_Name, Last_Name, Manager, Director, [Group], Role, Systems from New_Contacts where ADID = @ADID and Password = @Pass";
                    SqlCommand cmd2 = new SqlCommand(query2, con);
                    
                    var Adid2 = new SqlParameter("@ADID", SqlDbType.VarChar, 50);
                    var Pass2 = new SqlParameter("@Pass", SqlDbType.VarChar, 100);
                    Adid2.Value = ADIDtext.Text.ToString();
                    Pass2.Value = Passwordtext.Password.ToString();
                    cmd2.Parameters.Add(Adid2);
                    cmd2.Parameters.Add(Pass2);
                    
                    SqlDataReader reader2;
                    reader2 = cmd2.ExecuteReader();

                    query_results = PullDataFromReader(reader2);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    query_results = new string[1];
                }
                finally
                {
                    con.Close();
                }

            return query_results;
        }


        
        // Reads from a SqlDataReader and stores each field as an element of a string[], and returns that string[] when it is finished reading.
        private string[] PullDataFromReader(SqlDataReader reader2)
        {
            int cols = reader2.FieldCount;
            string[] reader_data = new string[cols];
            while (reader2.Read())
            {
                for (int i = 0; i < cols; i++)
                {
                    reader_data[i] = reader2.GetValue(i).ToString();
                }
            }
            reader2.Close();
            return reader_data;
        }
    }
}
