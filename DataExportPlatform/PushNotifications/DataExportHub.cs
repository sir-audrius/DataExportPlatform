using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public interface IDataExportHub
    {
        Task SendExportUpdated(DataExport dataExport);
    }

    public class DataExportHub : Hub<IDataExportHub>, IDataExportHub
    {
        public async Task SendExportUpdated(DataExport dataExport)
        {
            await Clients.All.SendExportUpdated(dataExport);
        }
    }
}
