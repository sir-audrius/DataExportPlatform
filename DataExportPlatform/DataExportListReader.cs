using System.Collections.Generic;
using System.Linq;

namespace DataExportPlatform
{
    public interface IDataExportListReader
    {
        IList<DataExport> Read();
    }

    public class DataExportListReader : IDataExportListReader
    {
        private readonly DataExportContext _dataExportContext;

        public DataExportListReader(DataExportContext dataExportContext)
        {
            _dataExportContext = dataExportContext;
        }

        public IList<DataExport> Read()
        {
            return _dataExportContext.DataExports
                .OrderByDescending(x => x.Id)
                .Select(x=> new DataExport
                {
                    Name = x.Name,
                    Status = x.Status
                })
                .Take(20)
                .ToList();
        }
    }
}
