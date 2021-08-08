using DataExportPlatform.Shared;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public interface IDataExportRegisteredHandler
    {
        Task Handle(DataExportRegisteredMessage message);
    }

    public class DataExportRegisteredHandler : IDataExportRegisteredHandler
    {
        private readonly DataExportContext _dataExportContext;
        private readonly IPushNotificationService _pushNotificationService;

        public DataExportRegisteredHandler(DataExportContext dataExportContext, IPushNotificationService pushNotificationService)
        {
            _dataExportContext = dataExportContext;
            _pushNotificationService = pushNotificationService;
        }

        public async Task Handle(DataExportRegisteredMessage message)
        {
            var record = await _dataExportContext.FindAsync<DataExportRecord>(message.Id);
            await _pushNotificationService.PushDataExportUpdatedAsync(new DataExport.List.DataExport
            {
                Id = record.Id,
                Name = record.Name,
                Status = record.Status
            });

        }
    }
}
