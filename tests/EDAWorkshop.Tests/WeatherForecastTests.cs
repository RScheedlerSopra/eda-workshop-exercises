namespace EDAWorkshop.Tests;

public class WeatherForecastTests
{
    [Fact]
    public void TemperatureF_ShouldConvertCorrectlyFromCelsius()
    {
        // Arrange
        var forecast = new WeatherForecast(DateOnly.FromDateTime(DateTime.Today), 100, "Scorching");

        // Act
        var tempF = forecast.TemperatureF;

        // Assert
        Assert.Equal(211, tempF);
    }

    [Fact]
    public void WeatherForecast_ShouldStorePropertiesCorrectly()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);
        var temperatureC = 25;
        var summary = "Warm";

        // Act
        var forecast = new WeatherForecast(date, temperatureC, summary);

        // Assert
        Assert.Equal(date, forecast.Date);
        Assert.Equal(temperatureC, forecast.TemperatureC);
        Assert.Equal(summary, forecast.Summary);
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
