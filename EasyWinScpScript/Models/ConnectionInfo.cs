using System.Security;
using WinSCP;

namespace EasyWinScpScript.Models
{
    public class ConnectionInfo
    {
        public string HostName { get; set; }
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string SshHostKeyFingerprint { get; set; }
        public int PortNumber { get; set; }
        public int TimeoutMilliseconds { get; set; }
        public Protocol Protocol { get; set; }
        public PrivateKey PrivateKey { get; set; }

        public ConnectionInfo()
        {
            PortNumber = -1;
            TimeoutMilliseconds = -1;
        }
    }
}
