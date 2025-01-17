
# WeatherApi

此資源來自氣象資料開放平台，提供臺灣各鄉鎮市區未來 1 週的天氣預報資料。資料來源網址為 https://opendata.cwa.gov.tw/dataset/forecast/F-D0047-091 ，需申請 API 授權碼才能使用。



## API Reference

#### Get item

```http
  GET /api/WeatherForecast/${cityName}
```

| Parameter | Type     | Description                             |
| :-------- | :------- | :---------------------------------------|
| `cityName`| `string` |輸入台灣各縣市名稱，可取得平均溫度、最高溫度和最低溫度|




## 🚀 其他
- 此資料為 12 小時天氣預報，所以撈取的資料為一週的凌晨 6 點至下午 6 點。

- 可與專案 Weather (https://github.com/qwertasdx/Weather) 一同使用。

