using DahuaTracker.Enums;
using System.ComponentModel;

namespace DahuaTracker
{
    public class InfoView
    {
        [DisplayName("Vaqt")]
        public string Time { get; set; }
        [DisplayName("Nomer")]
        public string PlateNumber { get; set; }
        [DisplayName("Nomer Rangi")]
        public string PlateColor { get; set; }
        [DisplayName("Moshina turi")]
        public string VehicleType { get; set; }
        [DisplayName("Moshina rangi")]
        public string VehicleColor { get; set; }
        [DisplayName("Moshina hajmi")]
        public string VehicleSize { get; set; }
        [DisplayName("Rasm joylashuvi")]
        public string FileName { get; set; }
        public Mode Mode { get; set; }
        public DateTime Date { get; set; }
    }
}
