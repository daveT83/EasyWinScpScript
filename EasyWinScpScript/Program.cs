using EasyWinScpScript.Infrastructure;
using EasyWinScpScript.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace EasyWinScpScript
{
    class Program
    {
        static int Main(string[] args)
        {
            EasyWinScpScript.Infrastructure.Logger logger = new EasyWinScpScript.Infrastructure.Logger(ParameterLoader.GetParameter(args, "Log"), ParameterLoader.IsEnableLogging(args));
            try
            {
                logger.LogSeperator('-');
                logger.Log("Collecting connection information...");
                ParameterLoader.AddLogger(logger);
                ConnectionInfo connectionInfo = new ConnectionInfo();
                if (ParameterLoader.IsDBConnection(args))
                {
                    logger.Log("Database call found");
                    ParameterLoader.LoadFromDataBase(args, connectionInfo);
                }
                if (ParameterLoader.isParameterLoad(args))
                {
                    logger.Log("Loading connection info from parameters");
                    ParameterLoader.LoadFromParameters(args, connectionInfo);
                }
                logger.LogSeperator('-');
                Connection conn = new Connection(connectionInfo, logger);

                conn.Initialize();
                string searchPattern = ParameterLoader.GetParameter(args, "Search_Pattern", "*");
                string sftpAction = ParameterLoader.GetParameter(args, "SFTP_Action").ToLower();
                string localDirectory = ParameterLoader.GetParameter(args, "Local_Directory");
                string remoteDirectory = ParameterLoader.GetParameter(args, "Remote_Directory");

                if (sftpAction.Equals(ConfigurationManager.AppSettings["PUT"]))
                {
                    conn.PutFilesInRemoteDirectory(conn.GetFiles(localDirectory, searchPattern), remoteDirectory);
                }
                else if (sftpAction.Equals(ConfigurationManager.AppSettings["PUT_DELETE"]))
                {
                    conn.PutFilesInRemoteDirectory(conn.GetFiles(localDirectory, searchPattern), remoteDirectory, true);
                }
                else if (sftpAction.Equals(ConfigurationManager.AppSettings["GET"]))
                {
                    conn.GetFilesInRemoteDirectory(conn.GetRemoteFiles(remoteDirectory, searchPattern), localDirectory);
                }
                else if (sftpAction.Equals(ConfigurationManager.AppSettings["GET_DELETE"]))
                {
                    conn.GetFilesInRemoteDirectory(conn.GetRemoteFiles(remoteDirectory, searchPattern), localDirectory, true);
                }
                else if (sftpAction.Equals(ConfigurationManager.AppSettings["GET_REMOTE_FILES"]))
                {
                    writeFile(conn.GetRemoteFiles(remoteDirectory, searchPattern), ParameterLoader.GetParameter(args, "Output_File"));
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                return 0;
            }
            finally
            {
                logger.Close();
            }
            return 1000;
        }

        /// <summary>
        /// writes a list of string to a file
        /// </summary>
        /// <param name="files"></param>
        /// <param name="outFile"></param>
        private static void writeFile(List<string> files, string outFile)
        {
            using (StreamWriter sw = new StreamWriter(outFile))
            {
                foreach (string file in files)
                {
                    sw.WriteLine(file);
                }
            }
        }
    }
}
