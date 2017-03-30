using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Globalization;

namespace ERP_system {
    /// <summary>
    /// Gathers data, opens a template then saves a pdf using the former
    /// </summary>
    class Invoice {
        private string templatePath;
        Database db;
        /// <summary>
        /// Creates a new empty instance of the Invoice class
        /// </summary>
        public Invoice() { }
        /// <summary>
        /// Create a new instance of the Invoice class
        /// </summary>
        /// <param name="db">Database where orders are</param>
        /// <param name="invoiceTemplate">Path to the template to use</param>
        public Invoice(Database db,string invoiceTemplate){
            try {
                this.db = db;
            templatePath = invoiceTemplate;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Creates a Pdf
        /// </summary>
        /// <param name="order">Ordernumber</param>
        /// <param name="print">Print invoice</param>
        public void CreatePDF(int order, string finalPath, bool print = false) {
            //try {
            NumberFormatInfo numformat = new NumberFormatInfo();
            numformat.NumberDecimalSeparator = ".";
                DataRow orderDr = db.OrdersGetByOrderId(order.ToString()).Rows[0];
                if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
            //Gives the file a name then checks if the file exists
            finalPath = finalPath + string.Format(@"Invoice{0}.pdf", orderDr["OrderId"]);
                finalPath = CheckPaths(templatePath, finalPath);

            //Checks if order has been produced
            if (Convert.ToInt32(orderDr["Status"]) < 2) {
                    if (MessageBox.Show("Bestilling er ikke produsert. Fortsett?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                }

            //Gathers data for the template
            DataTable typeDt = db.JarTypesGet();
                DataTable jarsDt = db.JarsGetByOrderId(order.ToString());

                typeDt.Columns.Add("num");
                typeDt.Columns.Add("amount");
                foreach (DataRow r in typeDt.Rows) {
                    r["num"] = 0;
                    r["amount"] = 0;
                }

                foreach (DataRow jarRow in jarsDt.Rows) {
                    foreach (DataRow typeRow in typeDt.Rows)
                        if (jarRow["TypeId"].ToString() == typeRow["TypeId"].ToString()) {
                            if (typeRow["num"] != DBNull.Value) typeRow["num"] = Convert.ToDouble(typeRow["num"]) + 1;

                        if (jarRow["AmountDelivered"] != DBNull.Value) {
                            double d = Convert.ToDouble(jarRow["AmountDelivered"],numformat);
                            typeRow["amount"] = Convert.ToDouble(typeRow["amount"]) + d;
                        }
                        }
                }
                string newline = "<br/>";
                string[] formData = {"","","","","","","","","","" };
                double subTotal = 0.0;

                if (orderDr["Name"] != DBNull.Value) formData[0] = orderDr["Name"].ToString();
                if (orderDr["Address"] != DBNull.Value) formData[1] = orderDr["Address"].ToString();

                foreach (DataRow r in typeDt.Rows) {
                    if (Convert.ToInt32(r["num"]) != 0) {
                        formData[2] = formData[2] + r["amount"] + newline;
                        formData[3] = formData[3] + r["num"] + " * " + r["Type"] + newline;
                        formData[4] = formData[4] + string.Format("{0:C}", Convert.ToDouble(r["CostPrMl"])) + newline;
                        subTotal += (Convert.ToDouble(r["CostPrMl"]) * Convert.ToDouble(r["amount"]));
                        formData[5] = formData[5] + string.Format("{0:C}", (Convert.ToDouble(r["CostPrMl"]) * Convert.ToDouble(r["amount"]))) + newline;
                    }
                }

                formData[6] = string.Format("{0:c}", subTotal);
                formData[7] = string.Format("{0:c}", subTotal * 0.25);
                formData[8] = string.Format("{0:c}", subTotal * 1.25);

            //Creates a pdf from the data gathered
            MakePdfFromXml(formData, templatePath, finalPath, print);
                MessageBox.Show(string.Format("Regning lagret som {0}", finalPath));
            //}
            //catch (Exception ex) {
            //    MessageBox.Show(ex.Message);
            //}
        }
        /// <summary>
        /// Creates a pdf at the specified location.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="template"></param>
        private void MakePdfFromXml(string[] formData, string template, string final,bool print) {
            try {

                //Sets values to use with the template
                StringBuilder b = new StringBuilder(File.ReadAllText(template));

                b.Replace("name", formData[0]);
                b.Replace("address", formData[1]);
                b.Replace("amount", formData[2]);
                b.Replace("description", formData[3]);
                b.Replace("unitPrice", formData[4]);
                b.Replace("sum", formData[5]);
                b.Replace("subtotal_", formData[6]);
                b.Replace("tax", formData[7]);
                b.Replace("total_1", formData[8]);

                //Creates a new pdf from the template
                FileStream fs = new FileStream(final, FileMode.Create, FileAccess.Write);
                MemoryStream ms = new MemoryStream();
                Document doc = new Document(PageSize.A4, 30, 30, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                TextReader tr = new StringReader(b.ToString());
                doc.Open();
                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, tr);
                
                tr.Close();
                if (doc.IsOpen()) doc.Close();
                ms.Close();
                fs.Close();

                if (print) Print(final);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Checks for template file, then for invoice path. If invoice already exists, asks to create new and finds a free path. Else throws exception.
        /// </summary>
        /// <param name="template">path of template</param>
        /// <param name="invoice">path of invoice</param>
        /// <returns>New invoice path</returns>
        private string CheckPaths(string template, string invoice) {
            try {
                if (!File.Exists(template)) throw new Exception("Mal mangler. Kan ikke lage fil");
                //Checks if the file exists, then finds a new filename
                if (File.Exists(invoice)) {
                    if (MessageBox.Show("Fil eksisterer. Lage ny?", "", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) {
                    int version = 0;
                    while (File.Exists(invoice)) {
                        version++;
                        string[] split = invoice.Split('.');
                        for (int i = 1; i < split.Length - 2; i++) {
                            split[0] = split[0] + '.' + split[i];
                        }
                        invoice = split[0] + "-" + version.ToString() + '.' + split[split.Length-1];
                    }
                }
                else throw new FileNotFoundException();
            }
            return invoice;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Prints doument at specified path
        /// </summary>
        /// <param name="path">Document to print</param>
        private void Print(string path) {
            try {
                ProcessStartInfo info = new ProcessStartInfo(path);
                info.Verb = "Print";
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process.Start(info);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }
}
