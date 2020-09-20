using System.Security;

namespace EasyWinScpScript.Models
{
    public class PrivateKey
    {
        public bool IsUsed { get; set; }
        public string KeyPath { get; set; }
        public SecureString KeyPassword { get; set; }
    }
}
