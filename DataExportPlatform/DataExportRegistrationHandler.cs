using DataExportPlatform.Shared;
using System;

namespace DataExportPlatform
{
    public interface IDataExportRegistrationHandler
    {
        void Handle();
    }

    public class DataExportRegistrationHandler : IDataExportRegistrationHandler
    {
        private readonly DataExportContext _dataExportContext;
        private readonly IMessageBus _messageBus;

        public DataExportRegistrationHandler(DataExportContext dataExportContext, IMessageBus messageBus)
        {
            _dataExportContext = dataExportContext;
            _messageBus = messageBus;
        }

        public void Handle()
        {
            var record = new DataExportRecord
            {
                Name = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
            };

            _dataExportContext.DataExports.Add(record);
            _dataExportContext.SaveChanges();
            _messageBus.SendRegistered(record.Id);
        }
    }
}
