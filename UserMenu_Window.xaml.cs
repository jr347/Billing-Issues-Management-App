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
    // DESCRIPTION: 	Window that holds the Main Menu and other sub-menu's of the application. 
    //                  On initialization, the frame displays UserMenuPage, the Page containing main app menu.
    //                  Frame also can display sub-menus via Navigation between Pages. See UserMenuPage.xaml.cs for more.
    //*******************************************************************
    public partial class UserMenu_Window : Window 
    {
        
        private string[] arr;


        //*******************************************************************
        // DESCRIPTION: Constructor for Subwindow1 Class, the "Main Menu" window of this application.
        //                  Also pre-populates login-based fields at top of form.
        //                  UserMenuPage, the page containing the buttons of the user menu, is displayed in this window.
        //*******************************************************************
        public UserMenu_Window(string[] user_data)
        {
            InitializeComponent();
            
            
            arr = user_data;
            
            ADIDtext.Text = arr[0];
            Nametext.Text = arr[1] + " "+ arr[2];
            Roletext.Text = arr[6];

            Loaded += UserWindow_Loaded;
        }




        //*******************************************************************
        // DESCRIPTION: Is executed when Subwindow1 is loaded.
        //              Uses NavigationService to display UserMenuPage, the WPF Page containing Main Menu Options & buttons.
        //*******************************************************************
        private void UserWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Usermenu1.NavigationService.Navigate(new UserMenu_MainPage(arr));
        }

    

        //*******************************************************************
        // DESCRIPTION: Is executed when 'Log Out' button is clicked.
        //              Creates an instance of and displays MainWindow, while closing the current window, Subwindow1.
        //*******************************************************************
        private void Logoutbutton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow Loginpage = new MainWindow();
            this.Close();
            Loginpage.Show();
        }



        //*******************************************************************
        // DESCRIPTION: Is executed when 'Exit' button is clicked.
        //              Displays a MessageBox asking the user to confirm if they wish to exit application.
        //*******************************************************************
        private void Exitbutton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Exit Application?", "Exit Confirmation", MessageBoxButton.OKCancel, MessageBoxImage.Stop);

            if (messageBoxResult == MessageBoxResult.OK)
            {
                this.Close();
            }
        }

    }
}
