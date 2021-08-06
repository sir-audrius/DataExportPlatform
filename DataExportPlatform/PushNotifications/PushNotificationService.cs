using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public interface IPushNotificationService
    {
        Task PushDataExportUpdatedAsync(DataExport.List.DataExport dataExport);
    }

    public class PushNotificationService : IPushNotificationService
    {
        private readonly IHubContext<DataExportHub, IDataExportHub> _dataExportHub;

        public PushNotificationService(IHubContext<DataExportHub, IDataExportHub> dataExportHub)
        {
            _dataExportHub = dataExportHub;
        }

        public async Task PushDataExportUpdatedAsync(DataExport.List.DataExport dataExport)
        {
            await _dataExportHub.Clients.All.SendExportUpdated(dataExport);
        }
    }
}
