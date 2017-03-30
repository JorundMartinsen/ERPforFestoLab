using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ERP_system {
    /// <summary>
    /// Interaction logic for DatabaseSettings.xaml
    /// Tests connection to the database. Disposes if connected and user clicks Ok
    /// </summary>
    public partial class DatabaseSettings : Window, IDisposable {
        private static string Ip_;
        private static string Name_;
        private static string Password_;
        private static bool ok;
        private static bool ok_;
        private static SqlConn conn;
        public string Ip {
            get {
                return Ip_;
            }
        }
        public string Name {
            get {
                return Name_;
            }
        }
        public string Password {
            get {
                return Password_;
            }
        }
        public bool Ok {
            get {
                return ok;
            }
        }
        internal static DatabaseSettings window;
        internal string testokContent {
            get { return testOk.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { testOk.Content = value;
                testOk.IsEnabled = true;
            })); }
        }
        public DatabaseSettings(Window Owner) {
            InitializeComponent();
            this.Owner = Owner;
            ok = false;
            ok_ = false;
            window = this;
            iptxt.Text = Properties.Settings.Default.IpAddress;
            uNametxt.Text = Properties.Settings.Default.userName;
        }
        public void Dispose() {
            if (ok_) ok = true;
            Close();
        }
        /// <summary>
        /// Disposes the window without returning successfull connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e) {
            ok_ = false;
            Dispose();
        }
        /// <summary>
        /// Indicates testing is in progress
        /// </summary>
        private void testOkdisable() {
            testOk.Content = "Tester...";
            testOk.IsEnabled = false;
        }
        /// <summary>
        /// Shows test if no connection was established
        /// Shows Ok if connection was successfull
        /// </summary>
        private void testOkenable() {
            if (ok_) testOk.Content = "Ok";
            else testOk.Content = "Test";
            testOk.IsEnabled = true;
        }
        /// <summary>
        /// Shows test if no connection was established
        /// Shows Ok if connection was successfull
        /// </summary>
        private void testOkenableThread() {
            if (ok_) window.testokContent = "Ok";
            else window.testokContent = "Test";
        }
        /// <summary>
        /// Tests connection to database
        /// </summary>
        /// <returns>null if connection successfull. Error message if not</returns>
        private string test() {
            try {
                conn = new SqlConn(Ip_, "OrdersAndInvoice", Name_, Password_);
                conn.RunQuery("Select 1");
                
                return null;
            }
            catch(Exception ex) {
                return ex.Message;
            }
        }
        /// <summary>
        /// Continues work if connection successfull
        /// </summary>
        /// <param name="tsk"></param>
        private void worker(Task<string> tsk) {
            if (tsk.Result == null) {
                ok_ = true;
                testOkenableThread();
            }
            else {
                testOkenableThread();
                MessageBox.Show(tsk.Result);
            }
        }
        /// <summary>
        /// Gathers data for testing connection, then starts a task with the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void testOk_Click(object sender, RoutedEventArgs e) {
            if (ok_) Dispose();
            else {
                try {
                    if (iptxt.Text == null || uNametxt.Text == null || Passwordtxt.Password == null) {
                        MessageBox.Show("Alle feltene må fylles");
                        return;
                    }
                    else {
                        Ip_ = iptxt.Text;
                        Name_ = uNametxt.Text;
                        Password_ = Passwordtxt.Password;
                        testOkdisable();
                        //Creates a new task, that continues with worker.
                        var task = Task<string>.Factory.StartNew(() => test()).ContinueWith(x => worker(x));
                    }
                }
                catch (System.Data.SqlClient.SqlException ex) {
                    MessageBox.Show("Connection failed. \r\n" + ex.Message);
                    testOkenable();
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    testOkenable();
                }
            }
        }
        /// <summary>
        /// Checks if text has changed then makes it possible to try a new test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextChanged(object sender, TextChangedEventArgs e) {
            ok_ = false;
            testOkenable();
        }
        /// <summary>
        /// Sets the focus on the first empty textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (iptxt.Text == "") iptxt.Focus();
            else if (uNametxt.Text == "") uNametxt.Focus();
            else Passwordtxt.Focus();
        }
    }
}
