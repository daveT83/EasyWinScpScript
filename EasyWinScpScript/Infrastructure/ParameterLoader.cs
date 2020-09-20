using EasyWinScpScript.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;

namespace EasyWinScpScript.Infrastructure
{
    public class ParameterLoader
    {
        private static string seperator = ConfigurationManager.AppSettings["Parameter_Seperator"];
        private static EasyWinScpScript.Infrastructure.Logger logger;
        /// <summary>
        /// Maps the given parameters to the appropriate object
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ConnectionInfo LoadFromParameters(string[] args, ConnectionInfo conn)
        {
            if (conn.PrivateKey == null)
            {
                conn.PrivateKey = new PrivateKey();
            }

            foreach (string arg in args)
            {
                string[] argSplit = arg.Split(seperator);
                LoadParameters(conn, argSplit[0], argSplit[1]);
            }

            if (conn.PrivateKey.KeyPassword.Length > 0 && conn.PrivateKey.KeyPath.Length > 0)
            {
                conn.PrivateKey.IsUsed = true;
            }
            else
            {
                conn.PrivateKey.IsUsed = false;
            }

            return conn;
        }

        /// <summary>
        /// Reads in connection information via a sql script.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ConnectionInfo LoadFromDataBase(string[] args, ConnectionInfo conn)
        {
            string query = "";
            if (conn.PrivateKey == null)
            {
                conn.PrivateKey = new PrivateKey();
            }

            logger.Log("Reading SQL script");
            using (StreamReader sr = new StreamReader(GetParameter(args, "SQLScript")))
            {
                while (!sr.EndOfStream)
                {
                    query += sr.ReadLine();
                }
            }
            logger.Log("SQL scrip successfully read in");
            using (SqlConnection con = new SqlConnection(GetParameter(args, "Connection_String")))
            {
                logger.Log("Attempting to connect to database");
                SqlCommand command = new SqlCommand(query, con);
                con.Open();
                logger.Log("Connection established");

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string colName = reader.GetName(i);
                            string colValue = reader[colName].ToString();
                            conn = LoadParameters(conn, colName, colValue);
                        }
                    }
                }
            }

            if (conn.PrivateKey.KeyPassword.Length > 0 && conn.PrivateKey.KeyPath.Length > 0)
            {
                conn.PrivateKey.IsUsed = true;
            }
            else
            {
                conn.PrivateKey.IsUsed = false;
            }

            return conn;
        }

        /// <summary>
        /// Determines if there is a database connection needed
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsDBConnection(string[] args)
        {
            foreach (string arg in args)
            {
                string[] argSplit = arg.Split(seperator);

                if (argSplit[0].Equals(ConfigurationManager.AppSettings["SQLScript"]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines if values are loaded in via the program parameters
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool isParameterLoad(string[] args)
        {
            List<string> keys = ConfigurationManager.AppSettings.AllKeys.ToList();
            foreach (string arg in args)
            {
                string[] argSplit = arg.Split(seperator);

                if (!argSplit[0].Equals(ConfigurationManager.AppSettings["SQLScript"]) && keys.Contains(argSplit[0]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the a parameter is provided
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetParameter(string[] args, string parameterName, string defaultValue = "")
        {
            Log("Attempting to find parameter: " + parameterName);
            foreach (string arg in args)
            {
                string[] argSplit = arg.Split(seperator);

                if (argSplit[0].Equals(ConfigurationManager.AppSettings[parameterName]))
                {
                    Log("Parameter found: " + argSplit[1]);
                    return argSplit[1];
                }
            }
            Log("Parameter not found");
            return defaultValue;
        }

        /// <summary>
        /// Sets the logger
        /// </summary>
        /// <param name="log"></param>
        public static void AddLogger(EasyWinScpScript.Infrastructure.Logger log)
        {
            logger = log;
        }

        /// <summary>
        /// determines if logging should be enabled
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool IsEnableLogging(string[] args)
        {
            foreach (string arg in args)
            {
                string[] argSplit = arg.Split(seperator);

                if (argSplit[0].Equals(ConfigurationManager.AppSettings["Disable_Logging"]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Logs a string
        /// </summary>
        /// <param name="str"></param>
        private static void Log(string str)
        {
            if (logger != null)
            {
                logger.Log(str);
            }
        }

        /// <summary>
        /// Converts a string to a secure string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static SecureString ConvertToSecureString(string str)
        {
            char[] chars = str.ToCharArray();
            SecureString secure = new SecureString();

            foreach (char c in chars)
            {
                secure.AppendChar(c);
            }

            return secure;
        }

        /// <summary>
        /// Sets the connection info fields
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="colName"></param>
        /// <param name="colValue"></param>
        /// <returns></returns>
        private static ConnectionInfo LoadParameters(ConnectionInfo conn, string colName, string colValue)
        {
            if (colName.Trim().Equals(ConfigurationManager.AppSettings["HostName"]))
            {
                conn.HostName = colValue.Trim();
                Log("Host Name loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["Username"]))
            {
                conn.Username = colValue.Trim();
                Log("Username loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["Password"]))
            {
                conn.Password = ConvertToSecureString(colValue.Trim());
                Log("Password loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["SshHostFingerprint"]))
            {
                conn.SshHostKeyFingerprint = colValue.Trim();
                Log("Ssh Host Fingerprint loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["PortNumber"]))
            {
                conn.PortNumber = Convert.ToInt32(colValue.Trim());
                Log("Port Number loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["Timeout"]))
            {
                conn.TimeoutMilliseconds = Convert.ToInt32(colValue.Trim());
                Log("Timeout loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["Protocall"]))
            {
                if (colValue.Trim().ToLower().Equals("ftp"))
                {
                    conn.Protocol = WinSCP.Protocol.Ftp;
                    Log("Protocol loaded: FTP");
                }
                else if (colValue.Trim().ToLower().Equals("sftp"))
                {
                    conn.Protocol = WinSCP.Protocol.Sftp;
                    Log("Protocol loaded: SFTP");
                }
                else if (colValue.Trim().ToLower().Equals("s3"))
                {
                    conn.Protocol = WinSCP.Protocol.S3;
                    Log("Protocol loaded: S3");
                }
                else if (colValue.Trim().ToLower().Equals("scp"))
                {
                    conn.Protocol = WinSCP.Protocol.Scp;
                    Log("Protocol loaded: SCP");
                }
                else if (colValue.Trim().ToLower().Equals("webdev"))
                {
                    conn.Protocol = WinSCP.Protocol.Webdav;
                    Log("Protocol loaded: WebDev");
                }
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["PrivateKey_Password"]))
            {
                conn.PrivateKey.KeyPassword = ConvertToSecureString(colValue.Trim());
                Log("Private Key Password loaded");
            }
            else if (colName.Trim().Equals(ConfigurationManager.AppSettings["PrivateKey_Path"]))
            {
                conn.PrivateKey.KeyPath = colValue.Trim();
                Log("Private Key Path loaded");
            }

            return conn;
        }
    }
}
