namespace DataExportPlatform
{
    public interface IDataExportRegistrationHandler
    {
        void Handle();
    }

    public class DataExportRegistrationHandler : IDataExportRegistrationHandler
    {
        private readonly DataExportContext _dataExportContext;

        public DataExportRegistrationHandler(DataExportContext dataExportContext)
        {
            _dataExportContext = dataExportContext;
        }

        public void Handle()
        {
            _dataExportContext.DataExports.Add(new DataExportRecord());
            _dataExportContext.SaveChanges();
        }
    }
}
