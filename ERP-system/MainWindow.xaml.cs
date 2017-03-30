using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.IO;

namespace ERP_system {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        static DataTable JarDt = new DataTable();
        static DataTable OrderDt = new DataTable();
        static DataTable TypeDt = new DataTable();
        private DataGrid lastFocusGrid;
        public static Database myDatabase;
        bool creatingOrder;

        public MainWindow() {
            //Connects to the database
            DataBaseConnector();

            InitializeComponent();

            //Sets the status to not creating order
            creatingOrder = false;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            //Calls the helper to update the datagrids
            Window_LoadedHelper();
            
            //Creates an eventhandler to check if amount is an integer between 0 - 100
            JarDt.TableNewRow += new DataTableNewRowEventHandler(CheckAmount);

            //Creates eventhandlers to store wich datagrid had focus last
            JarDataGrid.LostFocus += new RoutedEventHandler(LastFocus);
            OrderDataGrid.LostFocus += new RoutedEventHandler(LastFocus);
            EditDataGrid.LostFocus += new RoutedEventHandler(LastFocus);
        }
        #region Helpers
        /// <summary>
        /// Saves the last focused datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastFocus(object sender, EventArgs e) {
            lastFocusGrid = (DataGrid)sender;
        }
        /// <summary>
        /// Shows the window to connect to the database.
        /// If the database connection returns successfully, it stores the settings.
        /// Else it exits the program.
        /// </summary>
        private void DataBaseConnector() {
            try {
                using (DatabaseSettings settings = new DatabaseSettings(GetWindow(this))) {
                    settings.ShowDialog();
                    if (settings.Ok) {
                        myDatabase = new Database(settings.Ip, "OrdersAndInvoice", settings.Name, settings.Password);
                        Properties.Settings.Default.IpAddress = settings.Ip;
                        Properties.Settings.Default.userName = settings.Name;
                        Properties.Settings.Default.Save();
                    }
                    else Environment.Exit(-1);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                Environment.Exit(-1);
            }
        }
        /// <summary>
        /// Checks format in Amount and TypeId column
        /// </summary>
        /// <returns></returns>
        private bool CheckAmount() {
            try {
                int i = 1;
                foreach (DataRow r in JarDt.Rows) {
                    if (Convert.ToInt32(r["Amount"]) < 0 || Convert.ToInt32(r["Amount"]) > 100) {
                        MessageBox.Show("Mengde må være et heltall mellom 0 og 100 ved rad " + i.ToString());
                        return true;
                    }
                    if (r["TypeId"].ToString() == "") {
                        MessageBox.Show("Glass må være valgt ved rad " + i.ToString());
                        return true;
                    }
                    i++;
                }
                return false;
            }
            catch (Exception) {
                MessageBox.Show("Mengde må være et heltall mellom 0 og 100");
                return true;
            }
        }
        /// <summary>
        /// Event handler for checking format in Amount column
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckAmount(object sender, DataTableNewRowEventArgs e) {
            try {
                if (Convert.ToInt32(e.Row["Amount"]) < 0 || Convert.ToInt32(e.Row["Amount"]) > 100) {
                    MessageBox.Show("Mengde må være et heltall");
                    
                    //Sets focus on the cell with error
                    EditDataGrid.CurrentCell = new DataGridCellInfo(EditDataGrid.Items[EditDataGrid.Items.Count - 1], EditDataGrid.Columns[0]);
                    EditDataGrid.BeginEdit();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Checks given datatable for changes row by row
        /// </summary>
        /// <param name="dt">Datatable to check</param>
        /// <returns>'true' if any rows has status; updated, deleted or added</returns>
        private bool CheckTableForChanges(DataTable dt) {
            try {
                if (dt.GetChanges() != null) return true;
                else return false;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Creates and updates database with new order
        /// </summary>
        /// <param name="id">CustomerId</param>
        private void CreateOrder(int id) {
            try {
                DataRow newRow = OrderDt.NewRow();

                newRow["Status"] = 0;
                newRow["OrderedTime"] = DateTime.Now;
                newRow["CustomerId"] = id;
                OrderDt.Rows.Add(newRow);

                myDatabase.UpdateDb(OrderDt);
                OrderDt = new DataTable();
                OrderDt = myDatabase.OrdersGetLast();

                //Shows the edit datagrid
                EditJars();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Toggles visibility of datagrids JarDataGrid and EditDataGrid
        /// </summary>
        private void EditJars() {
            if (creatingOrder) {
                JarDataGrid.Visibility = Visibility.Visible;
                EditDataGrid.Visibility = Visibility.Hidden;
                creatingOrder = false;
            }
            else {
                creatingOrder = true;

                JarDt = myDatabase.JarsGetByOrderId(OrderDt.Rows[0]["OrderId"].ToString());

                JarDataGrid.Visibility = Visibility.Hidden;
                EditDataGrid.Visibility = Visibility.Visible;
            }
        }
        /// <summary>
        /// Loads values from database
        /// </summary>
        private void init() {
            try {
                OrderDt = myDatabase.OrdersGet();
                JarDt = myDatabase.JarsGet();
                TypeDt = myDatabase.JarTypesGet();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Loads values async at start
        /// </summary>
        private async void Window_LoadedHelper() {
            //Updates the datagrids
            try {
                Task task = Task.Run((Action) init);
                await task;
                UpdateDatagrid();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Updates ItemSource in datagrid
        /// </summary>
        /// <param name="dg">Datagrid to update</param>
        /// <param name="dt">Datatable connected to datagrid</param>
        private void UpdateDatagrid() {
            try {
                OrderDataGrid.ItemsSource = OrderDt.DefaultView;
                JarDataGrid.ItemsSource = JarDt.DefaultView;
                EditDataGrid.ItemsSource = JarDt.DefaultView;

                foreach (DataGridColumn c in JarDataGrid.Columns) {
                    c.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion
        #region EventHandlers
        /// <summary>
        /// Shows a dialog to select customer, then creates a new entry in orders
        /// </summary>
        private void newOrderBtn_Click(object sender, RoutedEventArgs e) {
            try {
                if (!creatingOrder) {
                    using (NewOrder newOrder = new NewOrder(myDatabase.CustomersGet(), GetWindow(this))) {
                        newOrder.ShowDialog();
                        if (newOrder.Result != null) {
                            if ((int)newOrder.Result != -1) {
                                CreateOrder((int)newOrder.Result);

                                UpdateDatagrid();
                                JarDataGrid.Columns[2].Visibility = Visibility.Hidden;
                                JarDataGrid.Columns[3].Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Shows a dialog to create new customer, then creates a new entry in customers
        /// </summary>
        private async void newCustomerBtn_Click(object sender, RoutedEventArgs e) {
            try {
                using (NewCustomer newCustomer = new NewCustomer(GetWindow(this))) {
                    newCustomer.ShowDialog();
                    if (newCustomer.ok != false) {
                        if (newCustomer.name != null) {
                            DataTable dt = myDatabase.CustomersGet();
                            DataRow dr = dt.NewRow();

                            dr["Name"] = newCustomer.name;
                            if (newCustomer.address != null) dr["Address"] = newCustomer.address;
                            dt.Rows.Add(dr);

                            myDatabase.UpdateDb(dt);
                        }
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// If a table has been changed, Database is updated and a message is shown
        /// </summary>
        private async void saveBtn_Click(object sender, RoutedEventArgs e) {
            try {
                bool flag = false;
                if (creatingOrder && !CheckAmount()) {
                    string search;
                    double Maxweight;
                    int percent, weight;
                    for (int i = 0; i < JarDt.Rows.Count; i++) {

                        //Gets max value for the jar from the database then converts pecentage to grams
                        search = string.Format("TypeId = {0}", JarDt.Rows[i]["TypeId"].ToString());
                        Maxweight = Convert.ToDouble(TypeDt.Select(search)[0]["MaxWeight"]);
                        percent = Convert.ToInt32(JarDt.Rows[i]["Amount"]);
                        weight = Convert.ToInt32(percent * (Maxweight / 100));

                        JarDt.Rows[i]["Amount"] = weight.ToString();

                        JarDt.Rows[i]["OrderId"] = OrderDt.Rows[0]["OrderId"];
                        JarDt.Rows[i]["Status"] = "0";
                    }
                    //Updates the database and sets the flag to true
                    myDatabase.UpdateDb(JarDt);
                    flag = true;
                    init();
                    EditJars();
                    UpdateDatagrid();

                }
                if (flag) MessageBox.Show("Endringer er lagret");
            }
            catch (Exception ex) {
                MessageBox.Show("Endringene kan være ulagret \r\n" + ex.Message);
            }
        }
        /// <summary>
        /// Eventhandler for changing the data in jars datagrid. Opens the jars in the selected order
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                if (OrderDataGrid.SelectedItem != null) {
                    DataRowView SelectedRow = OrderDataGrid.SelectedItem as DataRowView;
                    if (SelectedRow != null) {
                        int myint = Convert.ToInt32(SelectedRow[0]);
                        string CustomerId = SelectedRow[2].ToString();
                        
                        //Updates the values in the textboxes
                        orderIdtxt.Text = SelectedRow[0].ToString();
                        statustxt.Text = SelectedRow[1].ToString();
                        customerNametxt.Text = (myDatabase.CustomersGetByCustomerId(CustomerId).Rows[0] as DataRow)["Name"].ToString();
                        customerAddresstxt.Text = (myDatabase.CustomersGetByCustomerId(CustomerId).Rows[0] as DataRow)["Address"].ToString();
                        
                        //Updates the jars datagrid
                        JarDt = myDatabase.JarsGetByOrderId(myint.ToString());
                        UpdateDatagrid();
                    }
                    else {
                        //Resets the values in the text
                        orderIdtxt.Text = "";
                        statustxt.Text = "";
                        customerNametxt.Text = "";
                        customerAddresstxt.Text = "";

                        //Updates the jars datagrid to default values
                        JarDt = myDatabase.JarsGet();
                        UpdateDatagrid();
                    }
                }
            }
            catch (IndexOutOfRangeException) {

            }

            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Deletes and/or discards changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelBtn_Click(object sender, RoutedEventArgs e) {
            try {
                if (creatingOrder) {
                    if (MessageBox.Show("Avbryte endringer? \r\nDette kan ikke angres", "ERP", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                        EditJars();
                        myDatabase.OrderDeleteById(OrderDt.Rows[0]["OrderId"].ToString());
                    }
                }
                else {
                    if (CheckTableForChanges(OrderDt) && CheckTableForChanges(JarDt)) {
                        if (MessageBox.Show("Avbryte alle endringer siden siste lagring? \r\nDette kan ikke angres", "ERP", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                            OrderDt.RejectChanges();
                            JarDt.RejectChanges();
                        }
                    }
                    else if (CheckTableForChanges(OrderDt)) {
                        if (MessageBox.Show("Avbryte endringer gjort i ordre? \r\nDette kan ikke angres", "ERP", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                            OrderDt.RejectChanges();
                        }
                    }
                    else if (CheckTableForChanges(JarDt)) {
                        if (MessageBox.Show("Avbryte endringer gjort i bestilling? \r\nDette kan ikke angres", "ERP", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                            JarDt.RejectChanges();
                        }
                    }
                }
                JarDt = myDatabase.JarsGet();
                OrderDt = myDatabase.OrdersGet();
                UpdateDatagrid();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Creates an invoice then, if the user chooses to, prints it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void invoiceBtn_Click(object sender, RoutedEventArgs e) {
            try {
                bool print;
                if (OrderDataGrid.SelectedItem == null) {
                    MessageBox.Show("Velg en ordre");
                    return;
                }
                switch (MessageBox.Show("Send til skriver?", "Regning", MessageBoxButton.YesNoCancel)) {
                case (MessageBoxResult.Yes):
                    print = true;
                    break;
                case (MessageBoxResult.No):
                    print = false;
                    break;
                default:
                    return;
                }
                //Creates a new invoice with the template
                Invoice invoice = new Invoice(myDatabase, @"Invoice SMART Line.htm");
                foreach (DataRowView r in OrderDataGrid.SelectedItems) {
                    if (r[0] != DBNull.Value) {
                        //Does the work as a task. Saves the file in MyDocuments\SMARTline
                        Task t = Task.Run(() => invoice.CreatePDF(Convert.ToInt32(r[0]),Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + @"\SMARTline\", print));
                        await t;
                    }
                }
                
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Creates production data pdf then, if the user chooses to, prints it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void productionDataBtn_Click(object sender, RoutedEventArgs e) {
            try {
                bool print;
                if (OrderDataGrid.SelectedItem == null) {
                    MessageBox.Show("Velg en ordre");
                    return;
                }
                switch (MessageBox.Show("Send til skriver?", "Produksjonsdata", MessageBoxButton.YesNoCancel)) {
                case (MessageBoxResult.Yes):
                    print = true;
                    break;
                case (MessageBoxResult.No):
                    print = false;
                    break;
                default:
                    return;
                }
                //Creates a new productiondata pdf with the template
                ProductionData pData = new ProductionData(myDatabase, @"ProductionData SMART Line.htm");
                foreach (DataRowView r in OrderDataGrid.SelectedItems) {
                    if (r[0] != DBNull.Value) {
                        //Does the work as a task. Saves the file in MyDocuments\SMARTline
                        Task t = Task.Run(() => pData.CreatePDF(Convert.ToInt32(r[0]),Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToString() + @"\SMARTline\", print));
                        await t;
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Updates the datagrids
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateBtn_Click(object sender, RoutedEventArgs e) {
            if (!creatingOrder) {
                init();
                UpdateDatagrid();
            }
        }
        /// <summary>
        /// Deletes last selected rows from last focused datagrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteBtn_Click(object sender, RoutedEventArgs e) {
            if (OrderDataGrid == lastFocusGrid && OrderDataGrid.SelectedItem != null) {
                if (MessageBox.Show("Slette valgte rader fra ordre?", "Slette?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) {
                    foreach (DataRowView r in OrderDataGrid.SelectedItems) {
                        myDatabase.OrderDeleteById(r["OrderId"].ToString());
                    }
                    init();
                    UpdateDatagrid();
                    MessageBox.Show("Slettet valgte rader");
                }
            }
            else if (JarDataGrid == lastFocusGrid && JarDataGrid.SelectedItem != null) {
                if (MessageBox.Show("Slette valgte rader fra bestilling?", "Slette?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) {
                    foreach (DataRowView r in JarDataGrid.SelectedItems) {
                        myDatabase.JarDeleteById(r["JarId"].ToString());
                    }
                    init();
                    UpdateDatagrid();
                    MessageBox.Show("Slettet valgte rader");
                }
            }
            else if (EditDataGrid == lastFocusGrid && EditDataGrid.SelectedItem != null) {
                if (MessageBox.Show("Slette valgte rader fra bestilling?", "Slette?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) {
                    foreach (DataRowView r in EditDataGrid.SelectedItems) {
                        r.Delete();
                    }
                    init();
                    UpdateDatagrid();
                    MessageBox.Show("Slettet valgte rader");
                }
            }
        }
        #endregion
    }
}
