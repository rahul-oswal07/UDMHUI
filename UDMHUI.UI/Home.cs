using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UDMHUI.BusinessLogic;
using UDMHUI.Entities.Result;

namespace UDMHUI.UI
{
    public partial class Home : Form
    {
        #region Variable Declaration
        private const string DBPATH = "DBPath";
        private const string LOOKUPPATH = "LookupPath";
        private const string DTEXECPATH = "DTExecPath";
        private const string SQLUTILITYPATH = "SQLUtilityPath";
        private const string XO2PATH = "XO2Path";
        private const string DATABASENAME = "DatabaseName";
        private const string SQLSERVERINSTANCENAME = "SQLServerInstanceName";
        private const string PACKAGEPATH = "PackagePath";
        private const string CREATEDBPROCEDURE = "CreateDB";
        private const string CREATETABLEPROCEDURE = "CreateTables";
        private const string CREATELUTTABLEPROCEDURE = "CreateLUTTables";
        private const string CREATEUDMHTABLES = "CreateUDMHTables";
        private const string CONFIGUREDB = "ConfigureDb";
        private const string CREATEDBSPNAME = "CREATEDB";

        #endregion

        #region Events


        private void OnXO2PathBrowseButtonClick(object sender, EventArgs e)
        {
            txtXO2Path.Text = GetFilePathOrDirectoryPath(false);
        }
        private void OnSQLUtilityPathBrowseButtonClick(object sender, EventArgs e)
        {
            txtSQLUtilityPath.Text = GetFilePathOrDirectoryPath(true);
        }
        private void OnDTExeBrowseButtonClick(object sender, EventArgs e)
        {
            txtDTExecPath.Text = GetFilePathOrDirectoryPath(true);
        }

        private void OnPackagePathBrowseButtonClick(object sender, EventArgs e)
        {
            txtPackagePath.Text = GetFilePathOrDirectoryPath(false);
        }

        private void OnLookupBrowseButtonClick(object sender, EventArgs e)
        {

            txtLookupPath.Text = GetFilePathOrDirectoryPath(false);
        }

        private void OnDBPathBrowseButtonClick(object sender, EventArgs e)
        {
            txtDBPath.Text = GetFilePathOrDirectoryPath(false);
        }

        private void OnResetButtonClick(object sender, EventArgs e)
        {
            this.lblMessage.Visible = false;
            LoadDefaultValues();
        }
        private void OnCreateDBButtonClick(object sender, EventArgs e)
        {

            //create a stored proceduere on masterdb to create db
            string connectionString = ConfigurationController.Default.MasterDb;
            string sqlServerInstance = ConfigurationController.Default.SQLServerInstanceName;
            connectionString = ReplaceValue(connectionString, SQLSERVERINSTANCENAME, sqlServerInstance);

            string dbName = ConfigurationController.Default.DatabaseName;
            string dbPath = ConfigurationController.Default.DBPath;
            bool result = CreateExecuteCreateDBProcedure(dbName, dbPath, connectionString);
            if (result)
                AddMessage("{0} database created...", dbName);

            string configureDBScript = File.ReadAllText(@"..\..\..\Solution Items\Configure.sql");
            result = CreateAndExecuteStoredProcedure(configureDBScript, connectionString, CONFIGUREDB);

            //create source table on the database created in above script
            string createTableScript = File.ReadAllText(@"..\..\..\Solution Items\CreateSRCTables.sql");
            connectionString = ConfigurationController.Default.UDHMRUNDB;
            connectionString = ReplaceValue(connectionString, DATABASENAME, dbName);
            connectionString = ReplaceValue(connectionString, SQLSERVERINSTANCENAME, sqlServerInstance);


            result = CreateAndExecuteStoredProcedure(createTableScript, connectionString, CREATETABLEPROCEDURE);
            if (result)
                AddMessage("SRCTables created...");

            //create LUT tables
            string createLUTTables = File.ReadAllText(@"..\..\..\Solution Items\CreateLUTTables.sql");
            result = CreateAndExecuteStoredProcedure(createLUTTables, connectionString, CREATELUTTABLEPROCEDURE);
            if (result)
                AddMessage("LUT tables created...");

            string createUDMHTables = File.ReadAllText(@"..\..\..\Solution Items\CreateUDMHTables.sql");
            result = CreateAndExecuteStoredProcedure(createUDMHTables, connectionString, CREATEUDMHTABLES);
            if (result)
                AddMessage("UDMH table created...");

            string createSSISFrmkTables = File.ReadAllText(@"..\..\..\Solution Items\CreateSSISFrmkTables.sql");

            result = ExecuteSqlScriptFromFile(connectionString, createSSISFrmkTables);
            if (result)
                AddMessage("SSIS Frmk tables created...");

            string createTGTTables = File.ReadAllText(@"..\..\..\Solution Items\CreateTGTTables.sql");
            result = ExecuteSqlScriptFromFile(connectionString, createTGTTables);
            if (result)
                AddMessage("TGT tables created...");

            string createSQLUtility = File.ReadAllText(@"..\..\..\Solution Items\CreateSQLUtility.sql");
            createSQLUtility = string.Format(createSQLUtility, ConfigurationController.Default.DatabaseName, ConfigurationController.Default.SQLUtilityPath);
            result = ExecuteSqlScriptFromFile(connectionString, createSQLUtility);
            if (result)
                AddMessage("SQL Utility Assembly registered");

            string createViewsForFilters = File.ReadAllText(@"..\..\..\Solution Items\CreateViewsForFilters.sql");
            result = ExecuteSqlScriptFromFile(connectionString, createViewsForFilters);
            if (result)
                AddMessage("View for filters created...");

            string createSplitDBObjectsSql = File.ReadAllText(@"..\..\..\Solution Items\CreateSplitDBObjects.sql");
            result = ExecuteSqlScriptFromFile(connectionString, createSplitDBObjectsSql);
            if (result)
                AddMessage("Execution CreateSplitDBObjects script successfully");

            string createBQFormattedViewsSql = File.ReadAllText(@"..\..\..\Solution Items\CreateBQFormattedViews.sql");
            result = ExecuteSqlScriptFromFile(connectionString, createBQFormattedViewsSql);
            if (result)
                AddMessage("Execution CreateBQFormattedViews script successfully");

            string createTGTSPsSql = File.ReadAllText(@"..\..\..\Solution Items\CreateTGTSPs.sql");
            result = ExecuteSqlScriptFromFile(connectionString, createTGTSPsSql);
            if (result)
                AddMessage("Execution CreateTGTSPs script successfully");

        }
        #endregion

        #region Private Methods

        private void AddMessage(string message, params object[] args)
        {
            string formattedMessage = string.Format(message, args);
            lstMessageBox.Items.Add(formattedMessage);
            lstMessageBox.Refresh();
        }
        private string GetFilePathOrDirectoryPath(bool isFile)
        {
            if (isFile)
            {
                using (FileDialog fileDialog = new OpenFileDialog())
                {
                    if (DialogResult.OK == fileDialog.ShowDialog())
                        return fileDialog.FileName;
                    else
                        return string.Empty;
                }
            }
            else
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    if (DialogResult.OK == folderBrowserDialog.ShowDialog())
                        return folderBrowserDialog.SelectedPath;
                    else
                        return string.Empty;
                }
            }
        }




        /// <summary>
        /// Loads the default values for the fields defined in screen from config file
        /// </summary>
        private void LoadDefaultValues()
        {

            this.txtDatabaseName.Text = ConfigurationController.Default.DatabaseName;
            this.txtSQLServerInstanceName.Text = ConfigurationController.Default.SQLServerInstanceName;
            this.txtDBPath.Text = ConfigurationController.Default.DBPath;
            this.txtLookupPath.Text = ConfigurationController.Default.LookupPath;
            this.txtDTExecPath.Text = ConfigurationController.Default.DTExecPath;
            this.txtSQLUtilityPath.Text = ConfigurationController.Default.SQLUtilityPath;
            this.txtXO2Path.Text = ConfigurationController.Default.XO2Path;
            this.txtPackagePath.Text = ConfigurationController.Default.PackagePath;
        }

        private void OnUpdateButtonClick(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            ValidateInputResult validateInputResult = ValidateInputs();

            if (validateInputResult.ResultCode == ValidateInputResultCode.Success)
            {
                lblMessage.ForeColor = Color.Green;
                bool result = SaveDetails();
                if (result)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Details Saved Successfully";
                }
                else
                {
                    lblMessage.ForeColor = Color.Red;
                    lblMessage.Text = "Error Saving details";
                }
                LoadDefaultValues();
            }
            else
            {
                lblMessage.Text = validateInputResult.Message;
                lblMessage.ForeColor = Color.Red;
                lblMessage.Visible = true;
            }


        }
        /// <summary>
        /// save the information from screen to config file
        /// </summary>
        /// <returns></returns>
        private bool SaveDetails()
        {
            try
            {
                ConfigurationController.ChangeKeyValue(DBPATH, txtDBPath.Text, false, true);
                ConfigurationController.ChangeKeyValue(LOOKUPPATH, txtLookupPath.Text, false, true);
                ConfigurationController.ChangeKeyValue(SQLSERVERINSTANCENAME, txtSQLServerInstanceName.Text, false, true);
                ConfigurationController.ChangeKeyValue(SQLUTILITYPATH, txtSQLUtilityPath.Text, false, true);
                ConfigurationController.ChangeKeyValue(PACKAGEPATH, txtPackagePath.Text, false, true);
                ConfigurationController.ChangeKeyValue(LOOKUPPATH, txtLookupPath.Text, false, true);
                ConfigurationController.ChangeKeyValue(DATABASENAME, txtDatabaseName.Text, false, true);
                ConfigurationController.ChangeKeyValue(XO2PATH, txtXO2Path.Text, false, true);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Saving Details");
                return false;
            }
        }
        /// <summary>
        /// Replaces the key with the values in the string
        /// </summary>
        /// <param name="content"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ReplaceValue(string content, string key, string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    content = content.Replace("%", "");
                    content = content.Replace(key, value);
                    return content;
                }
                else
                    return string.Empty;

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Replacing values");
                return string.Empty;
            }

        }
        /// <summary>
        /// Executes the sql scripts for a specified connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        private static bool ExecuteSqlScriptFromFile(string connectionString, string script)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    Server server = new Server(new ServerConnection(sqlConnection));
                    server.ConnectionContext.ExecuteNonQuery(script);
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Executing sql script");
                return false;
            }

        }
        /// <summary>
        /// Create and execute the stored procedure
        /// </summary>
        /// <param name="script"></param>
        /// <param name="connectionString"></param>
        /// <param name="procedureName"></param>
        /// <returns></returns>
        private bool CreateAndExecuteStoredProcedure(string script, string connectionString, string procedureName)
        {
            try
            {
                string query = string.Format("select * from sysobjects where type='P' and name='{0}'", procedureName);
                bool spExists = false;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                spExists = true;
                                break;
                            }
                        }
                    }
                    if (!spExists)
                    {
                        using (SqlCommand sqlCommand = new SqlCommand(script, sqlConnection))
                        {
                            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        }
                    }



                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string commandText = procedureName;
                    SqlCommand createDBCommand = new SqlCommand(commandText, sqlConnection);
                    createDBCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    SqlDataReader sqlDataReader = createDBCommand.ExecuteReader();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Creating and executing stored procedure");
                return false;
            }
        }


        /// <summary>
        /// Creates and executes the create db stored procedure
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="dbPath"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private bool CreateExecuteCreateDBProcedure(string dbName, string dbPath, string connectionString)
        {
            try
            {
                string script = File.ReadAllText(@"..\..\..\Solution Items\CreateDatabase.sql");
                string query = string.Format("select * from sysobjects where type='P' and name='{0}'", CREATEDBPROCEDURE);
                bool spExists = false;
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                spExists = true;
                                break;
                            }
                        }
                    }
                    if (!spExists)
                    {
                        SqlCommand sqlCommand = new SqlCommand(script, sqlConnection);
                        sqlCommand.ExecuteNonQuery();
                    }

                }
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string commandText = CREATEDBPROCEDURE;
                    SqlCommand createDBCommand = new SqlCommand(commandText, sqlConnection);
                    createDBCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    createDBCommand.Parameters.AddWithValue("@DBDATAPATH", dbPath);
                    createDBCommand.Parameters.AddWithValue("@DBNAME", dbName);
                    SqlDataReader sqlDataReader = createDBCommand.ExecuteReader();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error Executing Create stored procedure");
                return false;
            }

        }
        #endregion

        #region Validations
        /// <summary>
        /// Validates the input entered by the user
        /// </summary>
        /// <returns></returns>
        private ValidateInputResult ValidateInputs()
        {
            //validate the database name
            if (string.IsNullOrEmpty(txtDatabaseName.Text))
                return new ValidateInputResult { Message = "Database name cannot be empty", ResultCode = ValidateInputResultCode.Failure };

            //validate Sql server Instance 
            if (string.IsNullOrEmpty(txtSQLServerInstanceName.Text))
                return new ValidateInputResult { Message = "SQLServerInstanceName cannot be empty", ResultCode = ValidateInputResultCode.Failure };

            //validate the DB Path
            if (string.IsNullOrEmpty(txtDBPath.Text))
                return new ValidateInputResult { Message = "DB Path cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!Directory.Exists(txtDBPath.Text))
                return new ValidateInputResult { Message = "Specified DB Path Does not exists", ResultCode = ValidateInputResultCode.Failure };

            //validate the DT Exe Path
            if (string.IsNullOrEmpty(txtDTExecPath.Text))
                return new ValidateInputResult { Message = "DTExecPath cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!File.Exists(txtDTExecPath.Text))
                return new ValidateInputResult { Message = "Specified DTExec Path Does not exists", ResultCode = ValidateInputResultCode.Failure };

            //validate Lookup path
            if (string.IsNullOrEmpty(txtLookupPath.Text))
                return new ValidateInputResult { Message = "LookupPath cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!Directory.Exists(txtLookupPath.Text))
                return new ValidateInputResult { Message = "Specified Lookup path Does not exists", ResultCode = ValidateInputResultCode.Failure };

            //validate Package Path
            if (string.IsNullOrEmpty(txtPackagePath.Text))
                return new ValidateInputResult { Message = "PackagePath cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!Directory.Exists(txtPackagePath.Text))
                return new ValidateInputResult { Message = "Specified Package Path Does not exists", ResultCode = ValidateInputResultCode.Failure };

            //validate SQL UtilityPath
            if (string.IsNullOrEmpty(txtSQLUtilityPath.Text))
                return new ValidateInputResult { Message = "SQLUtilityPath cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!File.Exists(txtSQLUtilityPath.Text))
                return new ValidateInputResult { Message = "Specified SQLUtilityPathDoes not exists", ResultCode = ValidateInputResultCode.Failure };

            //validate X02 Path
            if (string.IsNullOrEmpty(txtXO2Path.Text))
                return new ValidateInputResult { Message = "XO2Path cannot be empty", ResultCode = ValidateInputResultCode.Failure };
            else if (!Directory.Exists(txtXO2Path.Text))
                return new ValidateInputResult { Message = "Specified XO2Path Does not exists", ResultCode = ValidateInputResultCode.Failure };

            return new ValidateInputResult { Message = "Save Successfully", ResultCode = ValidateInputResultCode.Success };

        }
        #endregion

        #region Constructor
        public Home()
        {
            InitializeComponent();
            LoadDefaultValues();
        }
        #endregion

        private void lstMessageBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void OnLogResetButtonClick(object sender, EventArgs e)
        {
            lstMessageBox.Items.Clear();
        }

        private void OnLoadLookupButtonClick(object sender, EventArgs e)
        {
            string lookupFilePath = ConfigurationController.Default.LookupPath;
            string packageFilePath = ConfigurationController.Default.PackagePath;
            string dtexecPath = ConfigurationController.Default.DTExecPath;
            string connectionString = ConfigurationController.Default.UDHMRUNDB;
            string dbName = ConfigurationController.Default.DatabaseName;
            string sqlServerInstance = ConfigurationController.Default.SQLServerInstanceName;
            connectionString = ReplaceValue(connectionString, DATABASENAME, dbName);
            connectionString = ReplaceValue(connectionString, SQLSERVERINSTANCENAME, sqlServerInstance);
            IEnumerable<string> files = File.ReadLines(@"..\..\..\Solution Items\LookupFiles.csv");
            foreach (string file in files)
            {
                string[] filenames = file.Split('\t');
                string packagePath = Path.Combine(packageFilePath, filenames[0]);
                string lookupPath = Path.Combine(lookupFilePath, filenames[1]);
                bool result = ExecutePackage(dtexecPath, packagePath, connectionString, lookupPath);

            }


        }

        private bool ExecutePackage(string dtexecPath, string packagePath, string connectionString, string lookupPath)
        {
            Process process = new Process();
            process.StartInfo.FileName = dtexecPath;
            process.StartInfo.Arguments = " /File \"" + packagePath + "\"" + " /SET \"\\Package.Variables[connTransformation].Value\";" + "\"\\\"" + connectionString.Trim() + "\\\"\"" + " /SET \"\\Package.Variables[srcFilePath].Value\";" + "\"" + lookupPath.Trim() + "\"";
            process.Start();
            //   string output = process.StandardOutput.ReadToEnd();
            return true;





            //Microsoft.SqlServer.Dts.Runtime.Application app = new Microsoft.SqlServer.Dts.Runtime.Application();
            //Package package = null;
            //DTSExecResult pkgResults;
            //Variables vars;
            //package = app.LoadPackage(packagePath, null);
            //vars = package.Variables;
            //vars["connTransformation"].Value = connectionString;
            //vars["srcFilePath"].Value = lookupPath;
            //pkgResults = package.Execute(null, vars, null, null, null);

            //return true;


        }
    }
}
