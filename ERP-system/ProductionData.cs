using System;
using System.Data;
using System.IO;
using System.Text;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;

namespace ERP_system {
    /// <summary>
    /// Gathers data, opens a template then saves a pdf using the former
    /// </summary>
    class ProductionData : IDisposable{
        private string templatePath;
        Database db;
        /// <summary>
        /// Creates a new empty instance of the ProductionData class
        /// </summary>
        public ProductionData() { }
        /// <summary>
        /// Create a new instance of the ProductionData class
        /// </summary>
        /// <param name="db">Database where orders are</param>
        /// <param name="productionDataTemplate">Path to the template to use</param>
        public ProductionData(Database db,string productionDataTemplate) {
            try {
                this.db = db;
            templatePath = productionDataTemplate;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Creates a pdf with option to print
        /// </summary>
        /// <param name="order">Ordernumber</param>
        /// <param name="print">Print pdf</param>
        public void CreatePDF(int order,string finalPath, bool print = false) {
            try {
                DataRow orderDr = db.OrdersGetByOrderId(order.ToString()).Rows[0];
                if (!Directory.Exists(finalPath)) Directory.CreateDirectory(finalPath);
                using (DataTable typeDt = db.JarTypesGet())
                using (DataTable jarsDt = db.JarsGetByOrderId(order.ToString())) {
                    //Gives the file a name then checks if the file exists
                    finalPath = finalPath + string.Format(@"ProductionData{0}.pdf", orderDr["OrderId"]);
                    finalPath = CheckPaths(templatePath, finalPath);

                    //Checks if order has been produced
                    if (Convert.ToInt32(orderDr["Status"]) < 2) {
                        if (MessageBox.Show("Bestilling er ikke produsert. Fortsett?", "", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
                    }
                    
                    //Gathers data for the template
                    string newline = "<br/>";
                    string[] formData = {"","","","","","","",""};
                    formData[0] = orderDr["OrderId"].ToString();
                    foreach (DataRow r in jarsDt.Rows) {
                        formData[1] = formData[1] + r["JarId"].ToString() + newline;
                        formData[2] = formData[2] + r["Amount"].ToString() + " g" + newline;
                        formData[3] = formData[3] + r["Type"].ToString() + newline;
                        formData[4] = formData[4] + r["Status"].ToString() + newline;
                        formData[5] = formData[5] + r["AmountDelivered"].ToString() + newline;
                    }
                    if (orderDr["ProductionTime"] != null) formData[6] = orderDr["ProductionTime"].ToString();
                    if (orderDr["DisgardedJars"] != null) formData[7] = orderDr["DisgardedJars"].ToString();

                    //Creates a pdf from the data gathered
                    MakePdfFromXml(formData, templatePath, finalPath, print);
                    MessageBox.Show(string.Format("Produksjonsdata lagret som {0}", finalPath));
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Creates a pdf at the specified location.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="template"></param>
        private void MakePdfFromXml(string[] formData, string template, string final, bool print) {
            try {

                //Sets values to use with the template
                StringBuilder b = new StringBuilder(File.ReadAllText(template));

                b.Replace("OrderId", formData[0]);
                b.Replace("JarNum", formData[1]);
                b.Replace("JarAmounts", formData[2]);
                b.Replace("JarType", formData[3]);
                b.Replace("JarStatus", formData[4]);
                b.Replace("JarAmountDelivered", formData[5]);
                b.Replace("OrderProductionTime", formData[6]);
                b.Replace("OrderDisgardedJars", formData[7]);

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
        /// Checks for template file, then for productiondata path. If productiondata already exists, asks to create new and finds a free path. Else throws exception.
        /// </summary>
        /// <param name="template">path of template</param>
        /// <param name="productionData">path of productiondata</param>
        /// <returns>New productiondata path</returns>
        private string CheckPaths(string template, string productionData) {
            try {
                if (!File.Exists(template)) throw new Exception("Mal mangler. Kan ikke lage fil");
                //Checks if the file exists, then finds a new filename
                if (File.Exists(productionData)) {
                if (MessageBox.Show("Fil eksisterer. Lage ny?", "", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) {
                    int version = 0;
                    while (File.Exists(productionData)) {
                        version++;
                        string[] split = productionData.Split('.');
                        for (int i = 1; i < split.Length - 2; i++) {
                            split[0] = split[0] + '.' + split[i];
                        }
                        productionData = split[0] + "-" + version.ToString() + '.' + split[split.Length-1];
                    }
                }
                else throw new FileNotFoundException();
            }
            return productionData;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        /// <summary>
        /// Prints doument at specified path. Problems with Windows 10. Edge is not compatible
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
        public void Dispose() {
            GC.Collect();
        }
    }
}
