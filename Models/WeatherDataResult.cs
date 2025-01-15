namespace WeatherApi.Models
{
    public class WeatherDataResult
    {
        public string LocationName { get; set; }
        public List<string> Day { get; set; }
        public List<int> AvgTemperature { get; set; }
        public List<int> MaxTemperature { get; set; }
        public List<int> MinTemperature { get; set; }
    }
}
