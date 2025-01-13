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

        //����JSON���
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

                var finalResult = new List<string>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // �Ω��x�s��e��m���Ѯ�ƾ�
                        var temperatureData = new Dictionary<string, (string AvgTemp, string MaxTemp, string MinTemp)>();

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
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd} 06:00:00"; 

                                    if (!temperatureData.ContainsKey(formattedDate))
                                    {
                                        temperatureData[formattedDate] = ("", "", "");
                                    }

                                    switch (elementName)
                                    {
                                        case "�����ū�":
                                            var temperature = time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString();
                                            temperatureData[formattedDate] = (temperature, temperatureData[formattedDate].MaxTemp, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "�̰��ū�":
                                            var maxTemperature = time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, maxTemperature, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "�̧C�ū�":
                                            var minTemperature = time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, temperatureData[formattedDate].MaxTemp, minTemperature);
                                            break;
                                    }
                                }                              
                            }                               
                        }
       
                        foreach (var entry in temperatureData)
                        {
                            var formattedResult = $"{entry.Key},{locationName},�����ū�:{entry.Value.AvgTemp}��,�̰��ū�:{entry.Value.MaxTemp}��,�̧C�ū�:{entry.Value.MinTemp}��";
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
                // ���� Locations
                var locationsElement = doc.RootElement
                    .GetProperty("records")
                    .GetProperty("Locations");

                if (locationsElement.ValueKind == JsonValueKind.Null)
                {
                    return NotFound("�d�L���");
                }

                var finalResult = new List<string>();

                foreach (var locations in locationsElement.EnumerateArray())
                {
                    var locationArray = locations.GetProperty("Location");

                    foreach (var location in locationArray.EnumerateArray())
                    {
                        var locationName = location.GetProperty("LocationName").GetString();
                        var weatherElements = location.GetProperty("WeatherElement");

                        // �Ω��x�s��e��m���Ѯ�ƾ�
                        var temperatureData = new Dictionary<string, (string AvgTemp, string MaxTemp, string MinTemp)>();

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
                                    string formattedDate = $"{parsedDateTime:yyyy-MM-dd} 06:00:00";

                                    if (!temperatureData.ContainsKey(formattedDate))
                                    {
                                        temperatureData[formattedDate] = ("", "", "");
                                    }

                                    switch (elementName)
                                    {
                                        case "�����ū�":
                                            var temperature = time.GetProperty("ElementValue")[0].GetProperty("Temperature").GetString();
                                            temperatureData[formattedDate] = (temperature, temperatureData[formattedDate].MaxTemp, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "�̰��ū�":
                                            var maxTemperature = time.GetProperty("ElementValue")[0].GetProperty("MaxTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, maxTemperature, temperatureData[formattedDate].MinTemp);
                                            break;
                                        case "�̧C�ū�":
                                            var minTemperature = time.GetProperty("ElementValue")[0].GetProperty("MinTemperature").GetString();
                                            temperatureData[formattedDate] = (temperatureData[formattedDate].AvgTemp, temperatureData[formattedDate].MaxTemp, minTemperature);
                                            break;
                                    }
                                }
                            }
                        }

                        foreach (var entry in temperatureData)
                        {
                            var formattedResult = $"{entry.Key},{locationName},�����ū�:{entry.Value.AvgTemp}��,�̰��ū�:{entry.Value.MaxTemp}��,�̧C�ū�:{entry.Value.MinTemp}��";
                            finalResult.Add(formattedResult);
                        }
                    }
                }

                return Ok(finalResult);
            }    
        }
    }
}
