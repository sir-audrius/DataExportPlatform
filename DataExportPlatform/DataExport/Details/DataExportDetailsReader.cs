using DataExportPlatform.Shared;
using System.Threading.Tasks;

namespace DataExportPlatform.DataExport.Details
{
    public interface IDataExportDetailsReader
    {
        Task<DataExportDetails> ReadAsync(int id);
    }

    public class DataExportDetailsReader : IDataExportDetailsReader
    {
        private readonly DataExportContext _context;

        public DataExportDetailsReader(DataExportContext context)
        {
            _context = context;
        }

        public async Task<DataExportDetails> ReadAsync(int id)
        {
            var record = await _context.DataExports.FindAsync(id);
            return new DataExportDetails
            {
                Id = record.Id,
                Name = record.Name,
                Status = record.Status,
                Result = record.Result
            };
        }
    }
}
