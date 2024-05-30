using System.Net;

namespace Auth_BlackList.Model
{
    public class SecurityIpModel
    {
        public Guid Id { get; set; }
        public string IpAddress { get; set; }
        public DateTime DateBlockIp { get; set; }
        public int CountForbiddenAccess { get; set; }
        public BlockTypeAccess BlockAccessApi { get; set; }
    }

    public enum BlockTypeAccess
    {
        BlockOff = 0,
        BlockOn = 1
    }
}
