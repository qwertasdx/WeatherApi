using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WeatherApi.Models;
using WeatherApi.Services;

namespace WeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherService _cityWeatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService cityWeatherService)
        {
            _logger = logger;
            _cityWeatherService = cityWeatherService;
        }

        //測試JSON資料，可使用postman測試
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
                // 提取 Locations
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("查無資料");
                }

                var finalResult = new List<WeatherDataResult>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");
           
                        // 用於儲存當前位置的天氣數據
                        var weatherDataResult = new WeatherDataResult
                        {
                            LocationName = locationName,
                            Day = new List<string>(),
                            AvgTemperature = new List<int>(),
                            MaxTemperature = new List<int>(),
                            MinTemperature = new List<int>()
                        };

                        // 此資料是撈12小時天氣預報，固只參考每天早上6點至下午6點資料
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // 只處理startTime為早上6點的資料
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd}";

                                    switch (elementName)
                                    {
                                        case "平均溫度":
                                            weatherDataResult.Day.Add(formattedDate);
                                            var avgTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString());
                                            weatherDataResult.AvgTemperature.Add(avgTemperature);
                                            break;
                                        case "最高溫度":
                                            var maxTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString());
                                            weatherDataResult.MaxTemperature.Add(maxTemperature);
                                            break;
                                        case "最低溫度":
                                            var minTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString());
                                            weatherDataResult.MinTemperature.Add(minTemperature);
                                            break;
                                    }
                                }                              
                            }                               
                        }

                        finalResult.Add(weatherDataResult);
                    }                 
                }
                // 將結果轉換為 JSON 格式
                var jsonResult = JsonSerializer.Serialize(finalResult);
                return Content(jsonResult, "application/json");
            }
        }

        [HttpGet("{cityName}")]
        public async Task<IActionResult> GetWeather(string cityName)
        {
            //由於此api是抓12小時天氣預報，所以由凌晨6點至下午6點
            DateTime timeFrom = DateTime.Today;
            DateTime timeTo = timeFrom.AddDays(7).AddHours(23).AddMinutes(59).AddSeconds(59);
            string formattedTimeFrom = timeFrom.ToString("yyyy-MM-ddT06:00:00");         
            string formattedTimeTo = timeTo.ToString("yyyy-MM-ddTHH:mm:ss");

            var forecastData = await _cityWeatherService.GetCityWeatherForecastAsync(cityName, formattedTimeFrom, formattedTimeTo);
            if (string.IsNullOrEmpty(forecastData))
            {
                return NotFound("查無溫度資料");
            }

            using (JsonDocument doc = JsonDocument.Parse(forecastData))
            {
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("查無資料");
                }

                var finalResult = new List<WeatherDataResult>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // 用於儲存當前位置的天氣數據
                        var weatherDataResult = new WeatherDataResult
                        {
                            LocationName = locationName,
                            Day = new List<string>(),
                            AvgTemperature = new List<int>(),
                            MaxTemperature = new List<int>(),
                            MinTemperature = new List<int>()
                        };

                        // 此資料是撈12小時天氣預報，固只參考每天早上6點至下午6點資料
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // 只處理startTime為早上6點的資料
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd}";

                                    switch (elementName)
                                    {
                                        case "平均溫度":
                                            weatherDataResult.Day.Add(formattedDate);
                                            var avgTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString());
                                            weatherDataResult.AvgTemperature.Add(avgTemperature);
                                            break;
                                        case "最高溫度":
                                            var maxTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString());
                                            weatherDataResult.MaxTemperature.Add(maxTemperature);
                                            break;
                                        case "最低溫度":
                                            var minTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString());
                                            weatherDataResult.MinTemperature.Add(minTemperature);
                                            break;
                                    }
                                }
                            }
                        }
                        finalResult.Add(weatherDataResult);
                    }
                }
                // 將結果轉換為 JSON 格式
                var jsonResult = JsonSerializer.Serialize(finalResult);
                return Content(jsonResult, "application/json");
            }    
        }
    }
}
