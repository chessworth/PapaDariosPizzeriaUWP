using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Data.SqlClient;
using System.Data;
using Windows.UI.Popups;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FinalExamGursimranSinghMudhar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //set up connection string as property
        private static string connectionString = @"Server=DESKTOP-DAK1RS1;Database=final;User Id=Gursimran;Password=123456;";
        public string ConnectionString { get => connectionString; set =>connectionString = value; }
        public bool LoggedIn { get; set; }
        public bool admin { get; set; }
        public User user { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
        }
        //methods for password decode/encode (Stack Overflow + .NET DOCS - System.Text)
        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
        public static string Base64Decode(string base64EncodedData)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64EncodedData));
        }
        private async void Login_submit(object sender, RoutedEventArgs e)
        {
            string login = username.Text;
            string pw = Base64Encode(password.Password);
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                string query = "select * from users where username = @0 and password = @1;";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@0", login);
                cmd.Parameters.AddWithValue("@1", pw);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    User curLogged = new User();
                    IDataReader data = (IDataReader)reader;
                    curLogged.UserID = (int)data[0];
                    curLogged.Fname = data[1].ToString();
                    curLogged.Lname = data[2].ToString();
                    curLogged.Username = data[3].ToString();
                    admin = (bool)data[5];
                    LoggedIn = true;
                    user = curLogged;
                    username.Text = "";
                    password.Password = "";
                    //TODO : Redirect to home
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Invalid Username or Password");
                    await dialog.ShowAsync();
                }
                
            }
            catch(Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message.ToString());
                await dialog.ShowAsync();
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }

        private void RegisterView_Click(object sender, RoutedEventArgs e)
        {
            loginGrid.Visibility = Visibility.Collapsed;
            registerGrid.Visibility = Visibility.Visible;
            username.Text = "";
            password.Password = "";
        }
        private void LoginView_Click(object sender, RoutedEventArgs e)
        {
            registerGrid.Visibility = Visibility.Collapsed;
            loginGrid.Visibility = Visibility.Visible;
            newpassword.Password = "";
            newpasswordconfirm.Password = "";
            newusername.Text = "";
            firstname.Text = "";
            lastname.Text = "";
        }
        private async void Register_submit(object sender, RoutedEventArgs e)
        {
            string fname = firstname.Text;
            string lname = lastname.Text;
            string login = newusername.Text;
            string pw = Base64Encode(newpassword.Password);
            string confirmpw = Base64Encode(newpasswordconfirm.Password);
            if(fname == "" || lname == "" || login == "" || pw == "" || confirmpw == "")
            {
                //TODO add red border to empty inputs
                MessageDialog dialog = new MessageDialog("Please enter all values");
                await dialog.ShowAsync();
                return;
            }
            if (!pw.Equals(confirmpw))
            {
                //TODO add red border to empty inputs
                MessageDialog dialog = new MessageDialog("Passwords don't match!");
                await dialog.ShowAsync();
                return;
            }
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                string query = "INSERT INTO USERS(FNAME,LNAME,USERNAME,PASSWORD) VALUES(@0, @1, @2, @3); ";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@0", fname);
                cmd.Parameters.AddWithValue("@1", lname);
                cmd.Parameters.AddWithValue("@2", login);
                cmd.Parameters.AddWithValue("@3", pw);
                conn.Open();
                if(cmd.ExecuteNonQuery() > 0)
                {
                    MessageDialog dialog = new MessageDialog("You've been registered Succesfully! Login to continue.");
                    await dialog.ShowAsync();
                    LoginView_Click(sender, e);
                }
                else
                {
                    MessageDialog dialog = new MessageDialog("Registration error. Try again or contact an admin.");
                    await dialog.ShowAsync();
                }

            }
            catch (Exception ex)
            {
                MessageDialog dialog = new MessageDialog(ex.Message.ToString());
                await dialog.ShowAsync();
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        private void Hide_all()
        {
            login.Visibility = Visibility.Collapsed;
            home.Visibility = Visibility.Collapsed;
            about.Visibility = Visibility.Collapsed;
            feedback.Visibility = Visibility.Collapsed;
            checkout.Visibility = Visibility.Collapsed;
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            Hide_all();
            var page = (StackPanel)((Button)sender).Tag;
            page.Visibility = Visibility.Visible;
        }
    }
}
