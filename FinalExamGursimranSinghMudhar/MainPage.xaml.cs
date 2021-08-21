using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Data.SqlClient;
using System.Data;
using Windows.UI.Popups;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FinalExamGursimranSinghMudhar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        //set up connection string as property
        private static string connectionString = @"Server=DESKTOP-DAK1RS1;Database=final;User Id=Gursimran;Password=123456;";
        public string ConnectionString { get => connectionString; set => connectionString = value; }
        private bool loggedin;
        public bool LoggedIn
        {
            get => loggedin;
            set
            {
                loggedin = value;
                NotifyPropertyChanged();
            }
        }
        public bool admin { get; set; }
        public User user { get; set; }
        public List<IFoodItem> Cart { get; set; } = new List<IFoodItem>();
        public List<IFoodItem> Menu { get; set; } = new List<IFoodItem>();

        public event PropertyChangedEventHandler PropertyChanged;
        public MainPage()
        {
            this.InitializeComponent();
            LoadMenu();
            PopulateMenu();
        }
        //notify property change for binding
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                _ = cmd.Parameters.AddWithValue("@0", login);
                _ = cmd.Parameters.AddWithValue("@1", pw);
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
                    Hide_all();
                    home.Visibility = Visibility.Visible;
                    loginNav.Content = "Log Out";
                }
                else
                {
                    _ = await Alert("Invalid Input", "Invalid Username or Password");
                }

            }
            catch (Exception ex)
            {
                _ = await Alert("Error!", ex.Message.ToString());
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
            if (fname == "" || lname == "" || login == "" || pw == "" || confirmpw == "")
            {
                //TODO add red border to empty inputs
                _ = await Alert("Invalid Input", "Please enter all values");
                return;
            }
            if (!pw.Equals(confirmpw))
            {
                //TODO add red border to empty inputs
                _ = await Alert("Invalid Input", "Passwords don't match!");
                return;
            }
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                string query = "INSERT INTO USERS(FNAME,LNAME,USERNAME,PASSWORD) VALUES(@0, @1, @2, @3); ";
                cmd.CommandText = query;
                _ = cmd.Parameters.AddWithValue("@0", fname);
                _ = cmd.Parameters.AddWithValue("@1", lname);
                _ = cmd.Parameters.AddWithValue("@2", login);
                _ = cmd.Parameters.AddWithValue("@3", pw);
                conn.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    _ = await Alert("Succes", "You've been registered Succesfully! Login to continue.");
                    LoginView_Click(sender, e);
                }
                else
                {
                    _ = await Alert("Error!", "Registration error. Try again or contact an admin.");
                }

            }
            catch (Exception ex)
            {
                _ = await Alert("Error!", ex.Message.ToString());
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        //hide all pages
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
            StackPanel page = (StackPanel)((Button)sender).Tag;
            page.Visibility = Visibility.Visible;
            if ((Button)sender == loginNav && loginNav.Content.ToString().Equals("Log Out", StringComparison.OrdinalIgnoreCase))
            {
                user = null;
                LoggedIn = false;
                admin = false;
                loginNav.Content = "Login/Register";
            }
        }
        private async void Feedback_submit(object sender, RoutedEventArgs e)
        {
            int parsedOrderID = 0, parsedRating = 0;
            //must be a member to give feedback
            if (!loggedin)
            {
                _ = await Alert("Error!", "You must be logged in to sumit feedback");
                return;
            }
            //check validity of values
            if (orderRating == null || !int.TryParse(orderRating.Text, out parsedRating) || parsedRating < 1 || parsedRating > 5)
            {
                _ = await Alert("Invalid Input", "Rating Value must be an integer between 1 and 5!");
                return;
            }
            else if (review.Text == "")
            {
                _ = await Alert("Invalid Input", "Please enter some feedback!");
                return;
            }
            else
            {
                //check validity of order input
                if (orderNum.Text != "" && !int.TryParse(orderNum.Text, out parsedOrderID))
                {

                    _ = await Alert("Invalid Input", "Please enter valid Order Number");
                }
                else
                {
                    int feedbackAnyway = -1;

                    SqlConnection conn = new SqlConnection(ConnectionString);
                    SqlCommand cmd = conn.CreateCommand();
                    try
                    {
                        if (orderNum.Text == "")
                        {
                            feedbackAnyway = 1;
                            conn.Open();
                        }
                        //check if order number exists in database
                        else
                        {
                            string query = "select * from orders where order_id = @0;";
                            cmd.CommandText = query;
                            _ = cmd.Parameters.AddWithValue("@0", parsedOrderID);
                            conn.Open();
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (!reader.HasRows)
                            {
                                feedbackAnyway = (int)await Alert("Invalid Input!", "The order number was not found! Send Feedback anyway?", true);
                            }
                            reader.Close();
                        }
                        //1 means yes was selected
                        if (feedbackAnyway == 1 || feedbackAnyway == -1)
                        {
                            string query;
                            if (feedbackAnyway == -1)
                            {
                                query = "insert into reviews(user_id, order_id, rating, review) values(@1, @0, @2, @3)";
                            }
                            else
                            {
                                query = "insert into reviews(user_id, rating, review) values(@1, @2, @3)";
                            }
                            cmd.CommandText = query;
                            _ = cmd.Parameters.AddWithValue("@1", user.UserID);
                            _ = cmd.Parameters.AddWithValue("@2", parsedRating);
                            _ = cmd.Parameters.AddWithValue("@3", review.Text);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                _ = await Alert("Succes", "Review Added Successfully!");
                                PizzaLottery();
                                orderNum.Text = "";
                                orderRating.Text = "";
                                review.Text = "";
                            }
                            else
                            {
                                _ = await Alert("Error!", "Error submitting review. Try again or contact an admin.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = await Alert("Error!", ex.Message.ToString());
                    }
                    finally
                    {
                        cmd.Dispose();
                        conn.Close();
                    }
                }
            }
        }
        private async void PizzaLottery()
        {
            Random rand = new Random();
            int roll = rand.Next(20);
            _ = roll == 20
               ? await Alert("Lottery Result!", "You've won a free pizza! We don't know how to deliver it yet.")
               : await Alert("Lottery Result!", $"Sorry! You didn't win anything. You rolled a {roll} on a 20-sided dice");
        }
        //return visibility set
        private Visibility GetVisibility(bool visible)
        {
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }
        private Visibility GetReverseVisibility(bool visible)
        {

            return !visible ? Visibility.Visible : Visibility.Collapsed;
        }
        //utility error function
        private async System.Threading.Tasks.Task<ContentDialogResult> Alert(string title, string message, bool isYEsNo = false)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = title,
                Content = message,
                SecondaryButtonText = isYEsNo ? "No" : "Ok",
                IsPrimaryButtonEnabled = isYEsNo,
                PrimaryButtonText = isYEsNo ? "Yes" : ""
            };
            return await dialog.ShowAsync();
        }
        private async void LoadMenu()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = conn.CreateCommand();
            try
            {
                string query = "select i.food_id, c.CATEGORY_NAME, i.name, i.price, i.description, i.image from food_item i inner join food_category c on i.CATEGORY_ID = c.CATEGORY_ID;";
                cmd.CommandText = query;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    IFoodItem food;
                    IDataReader data = (IDataReader)reader;
                    if(data[1].ToString().Equals("Pizza", StringComparison.OrdinalIgnoreCase))
                    {
                        food = new Pizza((int)data[0], data[1].ToString(), data[2].ToString(), (double) data[3], data[4].ToString(), data[5].ToString());
                    }
                    else
                    {
                        food = new Sides((int)data[0], data[1].ToString(), data[2].ToString(), (double)data[3], data[4].ToString(), data[5].ToString());
                    }
                    Menu.Add(food);
                }
            }
            catch (Exception ex)
            {
                _ = await Alert("Error!", ex.Message.ToString());
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        private void PopulateMenu()
        {
            foreach(IFoodItem foodItem in Menu)
            {
                if(foodItem is Pizza)
                {

                }
                else
                {

                }
            }
        }
    }
}
