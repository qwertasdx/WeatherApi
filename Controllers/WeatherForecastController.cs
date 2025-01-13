using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WeatherApi.Services;


namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherService _cityWeatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService cityWeatherService)
        {
            _logger = logger;
            _cityWeatherService = cityWeatherService;
        }

        //代刚JSON戈
        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> GetWeather()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.json");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found");
            }

            var jsonData = await System.IO.File.ReadAllTextAsync(filePath);

            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                // 矗 Locations
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("琩礚戈");
                }

                var finalResult = new List<string>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // ノ纗讽玡竚ぱ计沮
                        var temperatureData = new Dictionary<string, (string AvgTemp, string MaxTemp, string MinTemp)>();

                        // 戈琌即12ぱ箇厨㏕把σ–ぱΝ6翴と6翴戈
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // 矪瞶startTimeΝ6翴戈
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd} 06:00:00"; 

                                    if (!temperatureData.ContainsKey(formattedDate))
                                    {
                                        temperatureData[formattedDate] = ("", "", "");
                                    }

                                    switch (elementName)
                                    {
                                        case "キА放":
                                            var temperature = time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString();
                                            temperatureData[formattedDate] = (temperature, temperatureData[formattedDate].MaxTemp, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "程蔼放":
                                            var maxTemperature = time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, maxTemperature, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "程放":
                                            var minTemperature = time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, temperatureData[formattedDate].MaxTemp, minTemperature);
                                            break;
                                    }
                                }                              
                            }                               
                        }
       
                        foreach (var entry in temperatureData)
                        {
                            var formattedResult = $"{entry.Key},{locationName},キА放:{entry.Value.AvgTemp},程蔼放:{entry.Value.MaxTemp},程放:{entry.Value.MinTemp}";
                            finalResult.Add(formattedResult);
                        }
                    }                 
                }
              
                return Ok(finalResult);
            }
        }

        [HttpGet("{cityName}")]
        public async Task<IActionResult> GetWeather(string cityName)
        {
            //パapi琌ъ12ぱ箇厨┮パ贬6翴と6翴
            DateTime timeFrom = DateTime.Today;
            DateTime timeTo = timeFrom.AddDays(7).AddHours(23).AddMinutes(59).AddSeconds(59);
            string formattedTimeFrom = timeFrom.ToString("yyyy-MM-ddT06:00:00");         
            string formattedTimeTo = timeTo.ToString("yyyy-MM-ddTHH:mm:ss");

            var forecastData = await _cityWeatherService.GetCityWeatherForecastAsync(cityName, formattedTimeFrom, formattedTimeTo);
            if (string.IsNullOrEmpty(forecastData))
            {
                return NotFound("琩礚放戈");
            }

            using (JsonDocument doc = JsonDocument.Parse(forecastData))
            {
                // 矗 Locations
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("琩礚戈");
                }

                var finalResult = new List<string>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // ノ纗讽玡竚ぱ计沮
                        var temperatureData = new Dictionary<string, (string AvgTemp, string MaxTemp, string MinTemp)>();

                        // 戈琌即12ぱ箇厨㏕把σ–ぱΝ6翴と6翴戈
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // 矪瞶startTimeΝ6翴戈
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd} 06:00:00";

                                    if (!temperatureData.ContainsKey(formattedDate))
                                    {
                                        temperatureData[formattedDate] = ("", "", "");
                                    }

                                    switch (elementName)
                                    {
                                        case "キА放":
                                            var temperature = time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString();
                                            temperatureData[formattedDate] = (temperature, temperatureData[formattedDate].MaxTemp, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "程蔼放":
                                            var maxTemperature = time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, maxTemperature, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "程放":
                                            var minTemperature = time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, temperatureData[formattedDate].MaxTemp, minTemperature);
                                            break;
                                    }
                                }
                            }
                        }

                        foreach (var entry in temperatureData)
                        {
                            var formattedResult = $"{entry.Key},{locationName},キА放:{entry.Value.AvgTemp},程蔼放:{entry.Value.MaxTemp},程放:{entry.Value.MinTemp}";
                            finalResult.Add(formattedResult);
                        }
                    }
                }

                return Ok(finalResult);
            }    
        }
    }
}
