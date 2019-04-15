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
    // DESCRIPTION: 	Contains the Main Menu of the application.
    //                  Role (user/manager) determines which options are displayed to the user.
    //                  User can click to go to New Issue Form, Browse Items, Generate reports, or Managers Only buttons.
    //*******************************************************************
    public partial class UserMenu_MainPage : Page
    {
        private string[] arr;   


        
        public UserMenu_MainPage(string[] user_data)
        {
            InitializeComponent();
            
            arr = user_data;
        }



        //*******************************************************************
        // DESCRIPTION: Opens a new New Issue form on button click, by creating and showing an instance of Window NewRecord.
        //              Passes login-based data to NewRecord form for pre-population of fields.
        //*******************************************************************
        private void NewRecordbutton_Click(object sender, RoutedEventArgs e)
        {
            NewRecord newRecord = new NewRecord(arr);
            newRecord.Show();
        }




        //*******************************************************************
        // DESCRIPTION: Runs when the page is loaded.
        //              This function checks the user's role. If the user is not a Manager, 
        //                  certain features are collapsed and unavailable.
        //*******************************************************************
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(arr[6] != "Manager")
            {
                ForManagersbutton.Visibility = Visibility.Collapsed;
            }
        }



        
        // Runs when 'Browse Items' button is clicked. Navigates to UserMenu_ItemsPage Page, passing login-based data in arr
        private void BrowseItemsbutton_Click(object sender, RoutedEventArgs e)
        {
            UserMenu_ItemsPage itemsPage = new UserMenu_ItemsPage(arr);
            this.NavigationService.Navigate(itemsPage);
        }

        
    }
}
