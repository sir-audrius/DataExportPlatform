using Microsoft.EntityFrameworkCore;

namespace DataExportPlatform
{
    public class DataExportContext: DbContext
    {
        public DataExportContext(DbContextOptions<DataExportContext> options)
        : base(options)
        {
        }

        public DbSet<DataExportRecord> DataExports { get; set; }
    }

    public class DataExportRecord
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DataExportStatus Status { get; set; }
    }
}
