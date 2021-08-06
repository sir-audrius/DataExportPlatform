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
        private readonly IMessageBus _messageBus;
        private readonly Random _random = new Random();

        public DataExportRegisteredHandler(DataExportContext dataExportContext, IMessageBus messageBus)
        {
            _dataExportContext = dataExportContext;
            _messageBus = messageBus;
        }

        public async Task Handle(DataExportRegisteredMessage message)
        {
            var export = _dataExportContext.DataExports.Find(message.Id);
            export.Status = DataExportStatus.Started;
            _dataExportContext.SaveChanges();
            _messageBus.SendUpdated(message.Id);

            var delay = _random.Next(5, 10);
            await Task.Delay(TimeSpan.FromSeconds(delay));

            export.Status = DataExportStatus.Completed;
            export.Result = $"completed on {DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")}";
            _dataExportContext.SaveChanges();
            _messageBus.SendUpdated(message.Id);
        }
    }
}
