using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;

namespace ERP_system {
    /// <summary>
    /// Connects to a database and simplifies communication
    /// </summary>
    public class Database : SqlConn {
        #region Initialization
        // Tables to use
        protected string ordersTable = "Orders";
        protected string customerTable = "Customers";
        protected string jarTable = "Jars";
        protected string jarTypeTable = "JarTypes";
        protected List<string> tables = new List<string>();
        protected string JarTable {
            get {
                return jarTable;
            }

            set {
                jarTable = value;
            }
        }
        protected string orderQueryBase;
        protected string jarQueryBase;

        /// <summary>
        /// Initializes an instance of the Database class
        /// </summary>
        /// <param name="DbPath">Path to Database</param>
        public Database() : base() {

        }
        public Database(string Server, string Database, string User, string Pass) : base(Server, Database, User, Pass) {
            tables.Add(ordersTable);
            tables.Add(customerTable);
            tables.Add(jarTable);
            tables.Add(jarTypeTable);

            orderQueryBase = string.Format(
               "SELECT {0}.OrderId, {0}.Status, {0}.CustomerId, {1}.Name, {1}.Address, {0}.OrderedTime,{0}.ProductionTime,{0}.DisgardedJars " +
               "FROM {0} " +
               "INNER JOIN {1} " +
               "ON {0}.CustomerId = {1}.CustomerId "
               , ordersTable, customerTable);
            jarQueryBase = string.Format(
                "SELECT j.JarId, j.Amount, j.TypeId, t.Type, j.OrderId, c.Name, j.Status, j.AmountDelivered " +
                "FROM {0} j " +
                "INNER JOIN {1} t " +
                "ON j.TypeId = t.TypeId " +
                "INNER JOIN {2} o " +
                "ON o.OrderId = j.OrderId " +
                "INNER JOIN {3} c " +
                "ON o.CustomerId = c.CustomerId "
                , jarTable, jarTypeTable, ordersTable, customerTable);
        }
        #endregion
        #region Gets
        /// <summary>
        /// Returns all data in table "Jars"
        /// </summary>
        /// <returns>DataTable Jars</returns>
        public DataTable JarsGet() {
            try {
                //return OpenDb(jarTable);
                DataTable dt = OpenDbMan(jarQueryBase);
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with OrderId = id from "Jars" 
        /// </summary>
        /// <param name="id">OrderId to search for</param>
        /// <returns>Returns rows with matching OrderId as DataTable</returns>
        public DataTable JarsGetByOrderId(string id) {
            try {
                DataTable dt = OpenDbMan(string.Format("{0} WHERE j.OrderId = {1}",jarQueryBase,id));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with Status = status from "Jars" 
        /// </summary>
        /// <param name="status">Status to search for</param>
        /// <returns>Returns rows with matching Status as DataTable</returns>
        public DataTable JarsGetByStatus(string status) {
            try {
                DataTable dt = OpenDbMan(string.Format("SELECT * FROM {1} WHERE Status = {0}",status,jarTable));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with Rfid = Rfid from "Jars" 
        /// </summary>
        /// <param name="Rfid">Rfid to search for</param>
        /// <returns>Returns rows with matching Rfid as DataTable</returns>
        public DataTable JarsGetByRfid(string Rfid) {
            try {
                DataTable dt = OpenDbMan(string.Format("SELECT * FROM {1} WHERE Rfid = {0}",Rfid,jarTable));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns all data in table "Jars"
        /// </summary>
        /// <returns>DataTable Jars</returns>
        public async Task<DataTable> JarsGetAsync() {
            try {
                return await OpenDbAsync(jarTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with OrderId = id from "Jars" 
        /// </summary>
        /// <param name="id">OrderId to search for</param>
        /// <returns>Returns rows with matching OrderId as DataTable</returns>
        public async Task<DataTable> JarsGetByOrderIdAsync(string id) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE OrderId = {0}",id,jarTable));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with Status = status from "Jars" 
        /// </summary>
        /// <param name="status">Status to search for</param>
        /// <returns>Returns rows with matching Status as DataTable</returns>
        public async Task<DataTable> JarsGetByStatusAsync(string status) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE Status = {0}",status,jarTable));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with Rfid = Rfid from "Jars" 
        /// </summary>
        /// <param name="Rfid">Rfid to search for</param>
        /// <returns>Returns rows with matching Rfid as DataTable</returns>
        public async Task<DataTable> JarsGetByRfidAsync(string Rfid) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE Rfid = {0}",Rfid,jarTable));
                dt.TableName = jarTable;
                return dt;
            }
            catch (Exception ex) {

                throw ex;
            }
        }
        /// <summary>
        /// Returns all data in table "Customers"
        /// </summary>
        /// <returns>DataTable Customers</returns>
        public DataTable CustomersGet() {
            try {
                return OpenDb(customerTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with CustomerId = id from "Customers" 
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Returns rows with matching CustomerId as DataTable</returns>
        public DataTable CustomersGetByCustomerId(string id) {
            try {
                DataTable dt = OpenDbMan(string.Format("SELECT * FROM {1} WHERE CustomerId = {0}",id,customerTable));
                dt.TableName = customerTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns all data in table "Customers"
        /// </summary>
        /// <returns>DataTable Customers</returns>
        public async Task<DataTable> CustomersGetAsync() {
            try {
                return await OpenDbAsync(customerTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns rows with CustomerId = id from "Customers" 
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Returns rows with matching CustomerId as DataTable</returns>
        public async Task<DataTable> CustomersGetByCustomerIdAsync(string id) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE CustomerId = {0}",id,customerTable));
                dt.TableName = customerTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns all data in table "Orders"
        /// </summary>
        /// <returns>DataTable Customers</returns>
        public DataTable OrdersGet() {
            try {
                DataTable dt = OpenDbMan(orderQueryBase);
                dt.TableName = "Orders";
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with OrderId = id from "Orders" 
        /// </summary>
        /// <param name="id">OrderId to search for</param>
        /// <returns>Returns rows with matching Status as DataTable</returns>
        public DataTable OrdersGetByOrderId(string id) {
            try {
                DataTable dt = OpenDbMan(string.Format(
                orderQueryBase +
                "WHERE {0}.OrderId = {1}"
                ,ordersTable, id));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with Status = status from "Orders" 
        /// </summary>
        /// <param name="status">Status to search for</param>
        /// <returns>Returns rows with matching Status as DataTable</returns>
        public DataTable OrdersGetByStatus(string status) {
            try {
                DataTable dt = OpenDbMan(string.Format(
                orderQueryBase +
                "WHERE {0}.Status = {1}"
                ,ordersTable, status));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns rows with CustomerId = id from "Orders" 
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Returns rows with matching CustomerId as DataTable</returns>
        public DataTable OrdersGetByCustomerId(string id) {
            try {
                DataTable dt = OpenDbMan(string.Format(
                orderQueryBase +
                "WHERE {0}.CustomerId = {1}"
                ,ordersTable, id));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns a datatable with one row, representing the last row in the database
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable OrdersGetLast() {
            //DataTable dt = OpenDbMan(string.Format("SELECT TOP 1 * FROM {0} ORDER BY OrderId", ordersTable));
            try {
                DataTable dt = OpenDbMan(string.Format(
                "SELECT TOP 1 {0}.OrderId, {0}.Status, {0}.CustomerId, {1}.Name, {1}.Address, {0}.OrderedTime " +
                "FROM {0} " +
                "INNER JOIN {1} " +
                "ON {0}.CustomerId = {1}.CustomerId " +
                "ORDER BY {0}.OrderId DESC;"
                , ordersTable, customerTable));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns all data in table "Orders"
        /// </summary>
        /// <returns>DataTable Customers</returns>
        public async Task<DataTable> OrdersGetAsync() {
            try {
                return await OpenDbAsync(ordersTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns rows with Status = status from "Orders" 
        /// </summary>
        /// <param name="status">Status to search for</param>
        /// <returns>Returns rows with matching Status as DataTable</returns>
        public async Task<DataTable> OrdersGetByStatusAsync(string status) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE Status = {0}", status , ordersTable));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns rows with CustomerId = id from "Orders" 
        /// </summary>
        /// <param name="id">Id to search for</param>
        /// <returns>Returns rows with matching CustomerId as DataTable</returns>
        public async Task<DataTable> OrdersGetByCustomerIdAsync(string id) {
            try {
                DataTable dt = await OpenDbManAsync(string.Format("SELECT * FROM {1} WHERE CustomerId = {0}", id , ordersTable));
                dt.TableName = ordersTable;
                return dt;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Returns all data in table "JarTypes"
        /// </summary>
        /// <returns>DataTable JarTypes</returns>
        public DataTable JarTypesGet() {
            try {
                return OpenDb(jarTypeTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Async version.
        /// Returns all data in table "JarTypes"
        /// </summary>
        /// <returns>DataTable JarTypes</returns>
        public async Task<DataTable> JarTypesGetAsync() {
            try {
                return await OpenDbAsync(jarTypeTable);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
        #region Updates
        /// <summary>
        /// Updates edited and deleted rows in table dt
        /// </summary>
        /// <param name="dt">Table to update</param>
        public void UpdateDb(DataTable dt) {
            try {
                if (dt.TableName != null) {
                    if (tables.Contains(dt.TableName)) {
                        updateTable(dt);
                    }
                    else throw new Exception("Wrong table name");
                }
                else throw new Exception("No table name");
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
        #region Deletes
        /// <summary>
        /// Deletes an order and the associated jars from the database
        /// </summary>
        /// <param name="id">Id of the order to delete</param>
        public void OrderDeleteById(string id) {
            try {
                RunQuery(String.Format("DELETE FROM {0} WHERE OrderId = {1}", jarTable, id));
                RunQuery(String.Format("DELETE FROM {0} WHERE OrderId = {1}", ordersTable, id));
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Deletes a jar in the database
        /// </summary>
        /// <param name="id">Id of the jar to delete</param>
        public void JarDeleteById(string id) {
            try {
                RunQuery(String.Format("DELETE FROM {0} WHERE JarId = {1}", jarTable, id));
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
    }
}
