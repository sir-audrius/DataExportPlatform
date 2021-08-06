using DataExportPlatform.Shared;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public interface IDataExportUpdatedHandler
    {
        Task Handle(DataExportUpdatedMessage message);
    }

    public class DataExportUpdatedHandler : IDataExportUpdatedHandler
    {
        private readonly DataExportContext _dataExportContext;
        private readonly IPushNotificationService _pushNotificationService;

        public DataExportUpdatedHandler(DataExportContext dataExportContext, IPushNotificationService pushNotificationService)
        {
            _dataExportContext = dataExportContext;
            _pushNotificationService = pushNotificationService;
        }

        public async Task Handle(DataExportUpdatedMessage message)
        {
            var record = await _dataExportContext.FindAsync<DataExportRecord>(message.Id);
            await _pushNotificationService.PushDataExportUpdatedAsync(new DataExport
            {
                Id = record.Id,
                Name = record.Name,
                Status = record.Status
            });

        }
    }
}
