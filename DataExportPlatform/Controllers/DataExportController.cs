using DataExportPlatform.DataExport.Details;
using DataExportPlatform.DataExport.List;
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
        private readonly IDataExportDetailsReader _dataExportDetailsReader;

        public DataExportController(ILogger<DataExportController> logger, IDataExportRegistrationHandler registrationHandler, IDataExportListReader dataExportListReader, IDataExportDetailsReader dataExportDetailsReader)
        {
            _logger = logger;
            _registrationHandler = registrationHandler;
            _dataExportListReader = dataExportListReader;
            _dataExportDetailsReader = dataExportDetailsReader;
        }

        [HttpGet("")]
        public async Task<IEnumerable<DataExport.List.DataExport>> GetAsync()
        {
            return await _dataExportListReader.ReadAsync();
        }

        [HttpGet("{id}")]
        public async Task<DataExportDetails> GetDetailsAsync(int id)
        {
            return await _dataExportDetailsReader.ReadAsync(id);
        }

        [HttpPost]
        public async Task RegisterAsync()
        {
            await _registrationHandler.HandleAsync();
        }
    }
}
