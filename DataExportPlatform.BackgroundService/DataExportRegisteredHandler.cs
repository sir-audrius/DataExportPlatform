using DataExportPlatform.Shared;
using System;
using System.Threading.Tasks;

namespace DataExportPlatform.BackgroundService
{
    public interface IDataExportRegisteredHandler
    {
        Task Handle(DataExportRegisteredMessage message);
    }

    public class DataExportRegisteredHandler : IDataExportRegisteredHandler
    {
        private readonly DataExportContext _dataExportContext;
        private readonly Random _random = new Random();

        public DataExportRegisteredHandler(DataExportContext dataExportContext)
        {
            _dataExportContext = dataExportContext;
        }

        public async Task Handle(DataExportRegisteredMessage message)
        {
            var export = _dataExportContext.DataExports.Find(message.Id);
            export.Status = DataExportStatus.Started;
            _dataExportContext.SaveChanges();

            var delay = _random.Next(5, 10);
            await Task.Delay(TimeSpan.FromSeconds(delay));

            export.Status = DataExportStatus.Completed;
            _dataExportContext.SaveChanges();
        }
    }
}
