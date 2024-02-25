using DahuaTracker.Enums;

namespace DahuaTracker
{
    public class EventInfo
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public string Index { get; set; }
        public string Count { get; set; }
        public string PlateNumber { get; set; }
        public string PlateType { get; set; }
        public string PlateColor { get; set; }
        public string VehicleType { get; set; }
        public string VehicleColor { get; set; }
        public string VehicleSize { get; set; }
        public string LaneNumber { get; set; }
        public string FileName { get; set; }

        public Mode Mode { get; set; }
    }
}