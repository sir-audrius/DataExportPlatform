using DataExportPlatform.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataExportPlatform.DataExport.List
{
    public interface IDataExportListReader
    {
        Task<IList<DataExport>> ReadAsync();
    }

    public class DataExportListReader : IDataExportListReader
    {
        private readonly DataExportContext _dataExportContext;

        public DataExportListReader(DataExportContext dataExportContext)
        {
            _dataExportContext = dataExportContext;
        }

        public async Task<IList<DataExport>> ReadAsync()
        {
            return await _dataExportContext.DataExports
                .OrderByDescending(x => x.Id)
                .Select(x => new DataExport
                {
                    Id = x.Id,
                    Name = x.Name,
                    Status = x.Status
                })
                .Take(20)
                .ToListAsync();
        }
    }
}
