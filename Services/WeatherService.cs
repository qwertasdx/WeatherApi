using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace WeatherApi.Services
{

    public class WeatherService 
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetCityWeatherForecastAsync(string cityName, string timeFrom, string timeTo)
        {
            //URL 編碼
            string encodedLocation = WebUtility.UrlEncode(cityName);
            string encodedTimeFrom = WebUtility.UrlEncode(timeFrom);
            string encodedTimeTo = WebUtility.UrlEncode(timeTo);
            string encodedElementName1 = WebUtility.UrlEncode("最高溫度");
            string encodedElementName2 = WebUtility.UrlEncode("平均溫度");
            string encodedElementName3 = WebUtility.UrlEncode("最低溫度");

            var apiUrl = $"https://opendata.cwa.gov.tw/api/v1/rest/datastore/F-D0047-091?Authorization=需自行申請&format=JSON&" +
                $"LocationName={cityName}&ElementName={encodedElementName1},{encodedElementName2},{encodedElementName3}&timeFrom={timeFrom}&timeTo={timeTo}"; // API 金鑰

            var response = await _httpClient.GetStringAsync(apiUrl);
            return response;
        }
    }
}
