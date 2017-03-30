using System;
using System.Windows;
using System.Data;

namespace ERP_system
{
    /// <summary>
    /// Interaction logic for NewOrder.xaml
    /// Gathers data for a new order
    /// </summary>
    public partial class NewOrder : Window, IDisposable
    {
        DataTable dt;
        public NewOrder(DataTable Customers,Window Owner)
        {
            InitializeComponent();
            this.Owner = Owner;
            dt = Customers;
            comboBox.ItemsSource = dt.DefaultView;
        }
        public object Result;
        private void okBtn_Click(object sender, RoutedEventArgs e) {
            Result = comboBox.SelectedValue;
            Dispose();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            Dispose();
        }
        public void Dispose() {
            Close();
        }
    }
}
