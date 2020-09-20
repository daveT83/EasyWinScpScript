using EasyWinScpScript.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.RegularExpressions;
using WinSCP;

namespace EasyWinScpScript.Infrastructure
{
    public class Connection
    {
        private ConnectionInfo connectionInfo;
        private SessionOptions sessionOptions;
        private EasyWinScpScript.Infrastructure.Logger logger;

        public Connection(ConnectionInfo connection, EasyWinScpScript.Infrastructure.Logger logger)
        {
            this.connectionInfo = connection;
            this.sessionOptions = new SessionOptions();
            this.logger = logger;
        }

        /// <summary>
        /// Configures the connection settings
        /// </summary>
        public void Initialize()
        {
            sessionOptions.HostName = connectionInfo.HostName;
            sessionOptions.PortNumber = connectionInfo.PortNumber;
            sessionOptions.Protocol = connectionInfo.Protocol;
            sessionOptions.SshHostKeyFingerprint = connectionInfo.SshHostKeyFingerprint;
            sessionOptions.UserName = connectionInfo.Username;

            if(!(connectionInfo.PortNumber == -1))
            {
                sessionOptions.PortNumber = connectionInfo.PortNumber;
            }
            else
            {
                sessionOptions.PortNumber = 22;
            }

            if (!(connectionInfo.TimeoutMilliseconds == -1))
            {
                sessionOptions.TimeoutInMilliseconds = connectionInfo.TimeoutMilliseconds;
            }
            else
            {
                sessionOptions.PortNumber = 100;
            }

            if (connectionInfo.PrivateKey.IsUsed)
            {
                sessionOptions.SshPrivateKeyPath = connectionInfo.PrivateKey.KeyPath;
                sessionOptions.PrivateKeyPassphrase = ConvertToString(connectionInfo.PrivateKey.KeyPassword);
            }
            else
            {
                sessionOptions.Password = ConvertToString(connectionInfo.Password);
            }
        }

        /// <summary>
        /// Retrieves a list of files from the local path
        /// </summary>
        /// <returns></returns>
        public List<string> GetFiles(string path, string searchPattern = "*")
        {
            logger.Log("Reteiving local file list from '" + path + "' directory, using the following search pattern: " + searchPattern);

            List<string> files = Directory.GetFiles(path, searchPattern).ToList();
            logger.Log(files.Count + " files found:");
            logger.LogEnumarable(files);

            return files;
        }

        /// <summary>
        /// Retreives a list of files from the remote path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public List<string> GetRemoteFiles(string path, string searchPattern = "")
        {
            Regex regex = new Regex(searchPattern);
            List<string> files = new List<string>();

            logger.Log("Reteiving remote file list from '" + path + "' directory, using the following search pattern: " + searchPattern);

            using (Session session = new Session())
            {
                session.Open(sessionOptions);
                RemoteDirectoryInfo directory = session.ListDirectory(path);
                foreach (RemoteFileInfo fileInfo in directory.Files)
                {
                    if (regex.Match(fileInfo.Name).Success && !fileInfo.IsDirectory)
                    {
                        files.Add(fileInfo.FullName);
                    }
                }
            }
            logger.Log(files.Count + " files found:");
            logger.LogEnumarable(files);
            return files;
        }

        /// <summary>
        /// Uploads the files from the local directory to the remote directory
        /// </summary>
        /// <param name="filePaths"></param>
        public void PutFilesInRemoteDirectory(List<string> filePaths, string remotePath, bool isPutDelete = false)
        {
            logger.Log("Opening connection to remote server");
            using (Session session = new Session())
            {
                session.Open(sessionOptions);
                logger.Log("Connection established");
                TransferOptions transferOptions = new TransferOptions();
                TransferOperationResult transferOperationResult;
                transferOptions.TransferMode = TransferMode.Automatic;
                logger.Log("Attempting to put files in remote directory");
                foreach (string path in filePaths)
                {
                    transferOperationResult = session.PutFiles(path, remotePath, isPutDelete, transferOptions);
                    transferOperationResult.Check();
                    if (isPutDelete)
                    {
                        logger.Log("Local file " + path + " successfully moved to " + remotePath + " and deleted from local directory");
                    }
                    else
                    {
                        logger.Log("Local file " + path + " successfully moved to " + remotePath);
                    }
                }
            }
        }

        /// <summary>
        /// Uploads the files from the remote directory to the local directory
        /// </summary>
        /// <param name="filePaths"></param>
        /// <param name="remotePath"></param>
        /// <param name="isPutDelete"></param>
        public void GetFilesInRemoteDirectory(List<string> filePaths, string localPath, bool isPutDelete = false)
        {
            logger.Log("Opening connection to remote server");
            using (Session session = new Session())
            {
                session.Open(sessionOptions);
                logger.Log("Connection established");
                TransferOptions transferOptions = new TransferOptions();
                TransferOperationResult transferOperationResult;
                transferOptions.TransferMode = TransferMode.Automatic;
                logger.Log("Attempting to get files from remote directory");
                foreach (string path in filePaths)
                {
                    transferOperationResult = session.GetFiles(path, localPath, isPutDelete, transferOptions);
                    transferOperationResult.Check();
                    if (isPutDelete)
                    {
                        logger.Log("Remote file " + path + " successfully moved to " + localPath + " and deleted from remote directory");
                    }
                    else
                    {
                        logger.Log("remote file " + path + " successfully moved to " + localPath);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a secure string to a string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
