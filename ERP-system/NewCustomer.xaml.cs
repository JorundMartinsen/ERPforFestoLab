using System;
using System.Windows;

namespace ERP_system {
    /// <summary>
    /// Interaction logic for NewCustomer.xaml
    /// Gathers data for a new customer
    /// </summary>
    public partial class NewCustomer : Window, IDisposable {
        public string name;
        public string address;
        public bool ok;
        public NewCustomer(Window Owner) {
            InitializeComponent();
            ok = false;
            this.Owner = Owner;
        }
        public void Dispose() {
            Close();
        }
        private void okBtn_Click(object sender, RoutedEventArgs e) {
            if (Name.Text != null && Name.Text.Length >= 1) {
                name = Name.Text;
                if (Address.Text != null) address = Address.Text;
                ok = true;
                Dispose();
            }
        }
        private void CancelBtn_Click(object sender, RoutedEventArgs e) {
            ok = false;
            Dispose();
        }
    }
}
