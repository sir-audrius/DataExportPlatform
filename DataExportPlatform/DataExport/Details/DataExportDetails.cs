using DataExportPlatform.Shared;

namespace DataExportPlatform.DataExport.Details
{
    public class DataExportDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataExportStatus Status { get; set; }
        public string Result { get; set; }
    }
}
