using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DataExportPlatform.PushNotifications
{
    public interface IDataExportHub
    {
        Task SendExportUpdated(DataExport.List.DataExport dataExport);
    }

    public class DataExportHub : Hub<IDataExportHub>, IDataExportHub
    {
        public async Task SendExportUpdated(DataExport.List.DataExport dataExport)
        {
            await Clients.All.SendExportUpdated(dataExport);
        }
    }
}
