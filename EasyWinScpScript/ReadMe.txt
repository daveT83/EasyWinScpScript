###########################################################################################################################################################################################################

Overview: 
EasyWinScpConnection is intended to be a simple way to transfer files from your local maching to a remote one. this uses the WinSCP package in order to move the file(s). Because of how this program has 
been set up, you can import the exact settings that you want in a couple different ways. You can either using command line arguments, a SQL query to import the necessary information into he program or 
a combination of the two. Just remember if you choose to mix and match it will retreive as much data from the SQL query first and then attempt to use the command line arguments. If a command line 
arguement provides data that is also provide from the query it will be overwritten.

###########################################################################################################################################################################################################

###########################################################################################################################################################################################################

How To Use With Command Line Arguements:
To use command line arguements with this program you must use the field title, the field seperator, and the value you would like to pass in. Below is an example of a valid command line arguement:

UserName:::Admin123

Here is a breakdown to clear up any confusion:
Field title: UserName
Field Seperator: :::
Value: Admin123
Please refer to the Arguements section for a full list.

###########################################################################################################################################################################################################

###########################################################################################################################################################################################################

How To Use With SQL Query:
To use a sql query to pass data into this program you must supply the connection string and the path to your sql script via the command line. Below is an example of a valid sql query:

SELECT CD.HostName as	HostName
FROM WINSCPConnectionData CD

This works because EasyWinScpConnection looks for the column name in order to load the data correctly. All of the column names should correspond to the appropriate command line arugement.
Please refer to the Arguements section for a full list.

###########################################################################################################################################################################################################

###########################################################################################################################################################################################################

Arguements:
	Required:
	HostName - This is the host name of the server you are trying to connect to. This will be where you are either retreiving or sending files from/to.
	Username - This is the username that will be used to log into the remote server.
	SshHostFingerprint - This is required by WinSCP and can be retreived from the winSCP app.This can be retreove from the winSCP app by first connecting to the site, then clicking on 'Session' at the 
						top of the window and finally select the 'Server/Protocol Information' option. The SshHostFingerprint should be listed at the bottom where it says 'SHA-256: '. 
						For more information please refer to WinSCP FAQ: https://winscp.net/eng/docs/faq_hostkey
	Protocall - This is the protocall that wil be used to connect to the remote server. The following are acceptable protocalls: FTP, SFTP , S3, SCP, and WebDev
	Action - this is the action you wish to take. The available options are as follows: p, pd, g, gd, and grf. For a more in depth explination of these place see the Actions section.
	LocalDirectory - This is the path to the directory on your local machine.
	RemoteDirectory - This is the directory on the remote server.

	Optional:
	PortNumber - This is the prt that will be used while trying to connect to the remote server. If no value is provided it will default to 22.
	Timeout - This is the timeout in milliseconds before the connection fails. By default this is set to 100;
	SearchPattern - This is a Rebex pattern used to filter out files from your desired action. If this isn't supplied it will default to '*' or will take every file.
	OutputFile - This is the path to the file that you want the list of files to be saved in. This is only necessary if the grf action is taken.
	SQLScript ($$)- This is the path to a SQL Scrpit that will be used to load information into the program. Thisis only needed if you plan on using a query to load data into the program.
	ConnectionString ($$)- This is the connection string used to run the SQL script. this is only necessary if you plan on using a SQL script.

	Logging:
	DisableLogging - This arguement will disable logging for this program. If you choose to using this you will not need to supply a Log directory. This arguement is the only one that does not 
					require a value to be passed.
	Log - This is the file where you would like the log to be saved into.

	Passwords:
	Password: - This is the password used to log into the remote server. This is only needed if a key pair is not being used.
	PrivateKeyPassword: - This is the password for the private key. This will only be used when a key pair is used.
	PrivateKeyPath: - This is the path to the Private key. This is only used if a key pair is used.

	$$ - Exclusive command line arguements

###########################################################################################################################################################################################################

###########################################################################################################################################################################################################

Actions:
p - Put. This will take a file from a local directory and place it on a remote server.
pd - Put Delete. This will take a file from a local directory and place it on a remote server. Once the file has been moved it will be deleted from the local directory.
g - Get. This will take a file from a remote server and place it in a local directory.
pd - Get Delete. This will take a file from a remote server and place it in a local directory. Once the file has been moved it will be deleted from the remote server.
grf - Get Remote Files. This will output a list of files in on a remote server.

###########################################################################################################################################################################################################