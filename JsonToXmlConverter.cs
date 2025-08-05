using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Company.Function
{
    public class JsonToXmlConverter
    {
        private readonly ILogger<JsonToXmlConverter> _logger;

        public JsonToXmlConverter(ILogger<JsonToXmlConverter> logger)
        {
            _logger = logger;
        }

        [Function("JsonToXmlConverter")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogInformation("Processing JSON to XML conversion.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            try
            {
                JObject json = JObject.Parse(requestBody);
                XDocument xml = JsonConvert.DeserializeXNode(json.ToString(), "Root");

                return new OkObjectResult(xml.ToString());
            }
            catch (JsonReaderException ex)
            {
                _logger.LogError($"Invalid JSON: {ex.Message}");
                return new BadRequestObjectResult("Invalid JSON format.");
            }
        }
    }
}
