using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataExportPlatform.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataExportController : ControllerBase
    {
        private readonly ILogger<DataExportController> _logger;
        private readonly IDataExportRegistrationHandler _registrationHandler;
        private readonly IDataExportListReader _dataExportListReader;

        public DataExportController(ILogger<DataExportController> logger, IDataExportRegistrationHandler registrationHandler, IDataExportListReader dataExportListReader)
        {
            _logger = logger;
            _registrationHandler = registrationHandler;
            _dataExportListReader = dataExportListReader;
        }

        [HttpGet]
        public IEnumerable<DataExport> Get()
        {
            return _dataExportListReader.Read();
        }

        [HttpPost]
        public async Task RegisterAsync()
        {
            await _registrationHandler.HandleAsync();
        }
    }
}
