using System;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ERP_system {
    /// <summary>
    /// Handles direct connection with an SQL database
    /// </summary>
    public class SqlConn {
        
        protected SqlConnection myConnection;
        protected SqlCommand myCommand;
        protected SqlDataAdapter myDataAdapter;
        protected SqlCommandBuilder myCommandBuilder;
        string connectionString;
        string selectCommand;
        int timeout  = 5;
        /// <summary>
        /// Creates a new empty object
        /// </summary>
        protected SqlConn() {

        }
        /// <summary>
        /// Initializes an instance of the SqlConn class
        /// </summary>
        /// <param name="server">Server to connect to</param>
        /// <param name="database">Database in the server</param>
        /// <param name="user">Username for connecting to database</param>
        /// <param name="password">Password for user</param>
        public SqlConn(string server, string database, string user, string password) {
            connectionString =
                "Data Source=" + server + ";" +
                "Initial Catalog=" + database + ";" +
                "User ID=" + user + ";" +
                "Password=" + password + ";" + 
                "Connection Timeout=" + timeout.ToString() + ";";
            selectCommand = "SELECT 1";
            try {
                myConnection = new SqlConnection(connectionString);
                myCommand = new SqlCommand(selectCommand, myConnection);
                myDataAdapter = new SqlDataAdapter(myCommand);
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        #region Runs
        /// <summary>
        /// Runs any given query
        /// </summary>
        /// <param name="query">Query to run</param>
        public void RunQuery(string query) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    query, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
            }
        }
        /// <summary>
        /// Async version
        /// Runs any given query
        /// </summary>
        /// <param name="query">Query to run</param>
        /// <returns></returns>
        public async Task RunQueryAsync(string query) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    query, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    await myConnection.OpenAsync();
                    myCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
            }
        }
        #endregion
        #region Opens
        /// <summary>
        /// Opens a connection to the database
        /// and reads the values in table to a dataset
        /// </summary>
        /// <param name="table">Table to read</param>
        /// <returns>DataTable containing table</returns>
        protected DataTable OpenDb(string table) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(string.Format("SELECT * FROM {0}", table),myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    DataTable dt = new DataTable();
                    myConnection.Open();
                    myDataAdapter.Fill(dt);
                    dt.TableName = table;
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Async version
        /// Opens a connection to the database
        /// and reads the values in table to a dataset
        /// </summary>
        /// <param name="table">Table to read</param>
        /// <returns>DataTable containing table</returns>
        protected async Task<DataTable> OpenDbAsync(string table) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(string.Format("SELECT * FROM {0}", table), myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    DataTable dt = new DataTable();
                    await myConnection.OpenAsync();
                    myDataAdapter.Fill(dt);
                    dt.TableName = table;
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }

        }
        /// <summary>
        /// Opens a connection to the database
        /// and reads the chosen values in table "table" to a dataset
        /// </summary>
        /// <param name="table">Table to read</param>
        /// <param name="value">Values to select as SQL</param>
        /// <returns>DataTable of selected columns from table</returns>
        protected DataTable OpenDb(string table, string columns) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    string.Format("SELECT {0} FROM {1}", columns, table), myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    DataTable dt = new DataTable();
                    myConnection.Open();
                    myDataAdapter.Fill(dt);
                    dt.TableName = table;
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Async version
        /// Opens a connection to the database
        /// and reads the chosen values in table "table" to a dataset
        /// </summary>
        /// <param name="table">Table to read</param>
        /// <param name="value">Values to select as SQL</param>
        /// <returns>DataTable of selected columns from table</returns>
        protected async Task<DataTable> OpenDbAsync(string table, string columns) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    string.Format("SELECT {0} FROM {1}", columns, table), myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    DataTable dt = new DataTable();
                    await myConnection.OpenAsync();
                    myDataAdapter.Fill(dt);
                    dt.TableName = table;
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Opens a connection to the database and runs the given query
        /// !!! DOES NOT RETURN TABLE NAME !!!
        /// </summary>
        /// <param name="selectQuery">Query to run. Should be a select-statement</param>
        /// <returns>DataTable of the data. !!! DOES NOT RETURN TABLE NAME !!! </returns>
        protected DataTable OpenDbMan(string selectQuery) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    selectQuery, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    myCommand.CommandTimeout = timeout;
                    DataTable dt = new DataTable();
                    myConnection.Open();
                    myDataAdapter.SelectCommand = myCommand;
                    myDataAdapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Opens a connection to the database and runs the given query
        /// !!! DOES NOT RETURN TABLE NAME !!!
        /// </summary>
        /// <param name="selectQuery">Query to run. Should be a select-statement</param>
        /// <returns>DataTable of the data.!!! DOES NOT RETURN TABLE NAME !!!</returns>
        protected async Task<DataTable> OpenDbManAsync(string selectQuery) {
            try {
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(
                    selectQuery, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand)) {
                    DataTable dt = new DataTable();
                    await myConnection.OpenAsync();
                    myDataAdapter.SelectCommand = myCommand;
                    myDataAdapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex) {

                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        #endregion
        #region Updates
        /// <summary>
        /// Updates values in table and deletes removed rows.
        /// </summary>
        /// <param name="dt">DataTable to update from</param>
        protected void updateTable(DataTable dt) {
            try {
                selectCommand = string.Format("SELECT * FROM {0}", dt.TableName);
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(selectCommand, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand))
                using (myCommandBuilder = new SqlCommandBuilder(myDataAdapter)) {
                    myCommand.CommandTimeout = timeout;
                    myConnection.Open();
                    myDataAdapter.Update(dt);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Async version.
        /// Updates values in table and deletes removed rows.
        /// </summary>
        /// <param name="dt">DataTable to update from</param>
        protected async Task updateTableAsync(DataTable dt) {
            try {
                selectCommand = string.Format("SELECT * FROM {0}", dt.TableName);
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(selectCommand, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand))
                using (myCommandBuilder = new SqlCommandBuilder(myDataAdapter)) {
                    myCommand.CommandTimeout = timeout;
                    await myConnection.OpenAsync();
                    myDataAdapter.Update(dt);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Updates values in table and deletes removed rows.
        /// </summary>
        /// <param name="dt">DataTable to update from</param>
        /// <param name="table">Table the DataTable comes from</param>
        protected void updateTable(DataTable dt, string table) {
            try {
                selectCommand = string.Format("SELECT * FROM {0}", table);
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(selectCommand, myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand))
                using (myCommandBuilder = new SqlCommandBuilder(myDataAdapter)) {
                    myCommand.CommandTimeout = timeout;
                    myConnection.Open();
                    myDataAdapter.Update(dt);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        /// <summary>
        /// Async version.
        /// Updates values in table and deletes removed rows.
        /// </summary>
        /// <param name="dt">DataTable to update from</param>
        /// <param name="table">Table the DataTable comes from</param>
        protected async Task updateTableAsync(DataTable dt, string table)
        {
            try {
                selectCommand = string.Format("SELECT * FROM {0}", table);
                using (myConnection = new SqlConnection(connectionString))
                using (myCommand = new SqlCommand(selectCommand,myConnection))
                using (myDataAdapter = new SqlDataAdapter(myCommand))
                using (myCommandBuilder = new SqlCommandBuilder(myDataAdapter)) {
                    myCommand.CommandTimeout = timeout;
                    await myConnection.OpenAsync();
                    myDataAdapter.Update(dt);
                }
            }
            catch (Exception ex) {
                throw ex;
            }
            finally {
                myConnection.Close();
                GC.Collect();
            }
        }
        #endregion
    }
}
