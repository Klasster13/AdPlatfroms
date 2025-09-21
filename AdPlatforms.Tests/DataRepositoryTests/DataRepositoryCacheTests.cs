using AdPlatforms.Models;
using AdPlatforms.Repository.Impl;
using Microsoft.Extensions.Logging;
using Moq;
using System.Diagnostics;

namespace AdPlatforms.Tests.DataRepositoryTests;

public class DataRepositoryCacheTests
{
    private readonly Mock<ILogger<DataRepository>> _loggerMock;
    private readonly DataRepository _dataRepository;


    public DataRepositoryCacheTests()
    {
        _loggerMock = new Mock<ILogger<DataRepository>>();
        _dataRepository = new DataRepository(_loggerMock.Object);
    }


    [Fact]
    public void FindPlatromsForLocation_UsingCache_ImprovedPerformance()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru"] },
            new() { Name = "Ревдинский рабочий", Locations = ["/ru/svrd/revda","/ru/svrd/pervik"] },
            new() { Name = "Газета уральских москвичей", Locations = ["/ru/msk", "/ru/permobl", "/ru/chelobl"] },
            new() { Name = "Крутая реклама", Locations = ["/ru/svrd"] }
        };

        _dataRepository.UploadData(platforms);
        var stopwatch = new Stopwatch();


        stopwatch.Start();
        var result1 = _dataRepository.FindPlatromsForLocation("/ru/svrd/revda");
        stopwatch.Stop();
        var firstTime = stopwatch.ElapsedTicks;

        stopwatch.Restart();
        var result2 = _dataRepository.FindPlatromsForLocation("/ru/svrd/revda");
        stopwatch.Stop();
        var secondTime = stopwatch.ElapsedTicks;


        Assert.True(firstTime > secondTime, $"Cache error: first time {firstTime}, second time {secondTime}");
        Assert.Equal(result1, result2);
    }
}
