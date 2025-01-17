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

        //����JSON��ơA�i�ϥ�postman����
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
                // ���� Locations
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("�d�L���");
                }

                var finalResult = new List<WeatherDataResult>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");
           
                        // �Ω��x�s��e��m���Ѯ�ƾ�
                        var weatherDataResult = new WeatherDataResult
                        {
                            LocationName = locationName,
                            Day = new List<string>(),
                            AvgTemperature = new List<int>(),
                            MaxTemperature = new List<int>(),
                            MinTemperature = new List<int>()
                        };

                        // ����ƬO��12�p�ɤѮ�w���A�T�u�ѦҨC�Ѧ��W6�I�ܤU��6�I���
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // �u�B�zstartTime�����W6�I�����
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd}";

                                    switch (elementName)
                                    {
                                        case "�����ū�":
                                            weatherDataResult.Day.Add(formattedDate);
                                            var avgTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString());
                                            weatherDataResult.AvgTemperature.Add(avgTemperature);
                                            break;
                                        case "�̰��ū�":
                                            var maxTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString());
                                            weatherDataResult.MaxTemperature.Add(maxTemperature);
                                            break;
                                        case "�̧C�ū�":
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
                // �N���G�ഫ�� JSON �榡
                var jsonResult = JsonSerializer.Serialize(finalResult);
                return Content(jsonResult, "application/json");
            }
        }

        [HttpGet("{cityName}")]
        public async Task<IActionResult> GetWeather(string cityName)
        {
            //�ѩ�api�O��12�p�ɤѮ�w���A�ҥH�ѭ��6�I�ܤU��6�I
            DateTime timeFrom = DateTime.Today;
            DateTime timeTo = timeFrom.AddDays(7).AddHours(23).AddMinutes(59).AddSeconds(59);
            string formattedTimeFrom = timeFrom.ToString("yyyy-MM-ddT06:00:00");         
            string formattedTimeTo = timeTo.ToString("yyyy-MM-ddTHH:mm:ss");

            var forecastData = await _cityWeatherService.GetCityWeatherForecastAsync(cityName, formattedTimeFrom, formattedTimeTo);
            if (string.IsNullOrEmpty(forecastData))
            {
                return NotFound("�d�L�ū׸��");
            }

            using (JsonDocument doc = JsonDocument.Parse(forecastData))
            {
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("�d�L���");
                }

                var finalResult = new List<WeatherDataResult>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // �Ω��x�s��e��m���Ѯ�ƾ�
                        var weatherDataResult = new WeatherDataResult
                        {
                            LocationName = locationName,
                            Day = new List<string>(),
                            AvgTemperature = new List<int>(),
                            MaxTemperature = new List<int>(),
                            MinTemperature = new List<int>()
                        };

                        // ����ƬO��12�p�ɤѮ�w���A�T�u�ѦҨC�Ѧ��W6�I�ܤU��6�I���
                        foreach (var weatherElement in weatherElements.EnumerateArray())
                        {
                            var elementName = weatherElement.GetProperty("ElementName").GetString();
                            var times = weatherElement.GetProperty("Time");

                            foreach (var time in times.EnumerateArray())
                            {
                                var startTime = time.GetProperty("StartTime").GetString();
                                DateTime parsedDateTime = DateTime.Parse(startTime);

                                // �u�B�zstartTime�����W6�I�����
                                if (parsedDateTime.TimeOfDay == TimeSpan.FromHours(6))
                                {
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd}";

                                    switch (elementName)
                                    {
                                        case "�����ū�":
                                            weatherDataResult.Day.Add(formattedDate);
                                            var avgTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString());
                                            weatherDataResult.AvgTemperature.Add(avgTemperature);
                                            break;
                                        case "�̰��ū�":
                                            var maxTemperature = int.Parse(time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString());
                                            weatherDataResult.MaxTemperature.Add(maxTemperature);
                                            break;
                                        case "�̧C�ū�":
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
                // �N���G�ഫ�� JSON �榡
                var jsonResult = JsonSerializer.Serialize(finalResult);
                return Content(jsonResult, "application/json");
            }    
        }
    }
}
