using DahuaTracker.Enums;
using System.ComponentModel.DataAnnotations;

namespace DahuaTracker
{
    public class CameraCredentials
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        [Key]
        public Mode Mode { get; set; }
    }
}
