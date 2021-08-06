using DataExportPlatform.PushNotifications;
using DataExportPlatform.Shared;
using System;
using System.Threading.Tasks;

namespace DataExportPlatform
{
    public interface IDataExportRegistrationHandler
    {
        Task HandleAsync();
    }

    public class DataExportRegistrationHandler : IDataExportRegistrationHandler
    {
        private readonly DataExportContext _dataExportContext;
        private readonly IMessageBus _messageBus;
        private readonly IPushNotificationService _pushNotificationService;

        public DataExportRegistrationHandler(DataExportContext dataExportContext, IMessageBus messageBus, IPushNotificationService pushNotificationService)
        {
            _dataExportContext = dataExportContext;
            _messageBus = messageBus;
            _pushNotificationService = pushNotificationService;
        }

        public async Task HandleAsync()
        {
            var record = new DataExportRecord
            {
                Name = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK")
            };

            _dataExportContext.DataExports.Add(record);
            _dataExportContext.SaveChanges();
            _messageBus.SendRegistered(record.Id);
            await _pushNotificationService.PushDataExportUpdatedAsync(new DataExport
            {
                Name = record.Name,
                Status = record.Status
            });
        }
    }
}
