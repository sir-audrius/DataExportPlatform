namespace DataExportPlatform
{
    public class DataExport
    {
        public string Name { get; set; }
        public DataExportStatus Status { get; set; }
    }

    public enum DataExportStatus
    {
        Registered,
        Started,
        Completed
    }
}
