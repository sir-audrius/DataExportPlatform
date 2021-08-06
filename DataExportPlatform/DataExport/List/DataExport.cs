using DataExportPlatform.Shared;

namespace DataExportPlatform.DataExport.List
{
    public class DataExport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataExportStatus Status { get; set; }
    }
}
