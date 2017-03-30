using System;
using System.Data;

namespace ERP_system {
    /// <summary>
    /// Helps with the combobox in EditDataGrid
    /// </summary>
    public class ComboHelper {
        public DataView GetTypes() {
            try {
                return MainWindow.myDatabase.JarTypesGet().DefaultView;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}