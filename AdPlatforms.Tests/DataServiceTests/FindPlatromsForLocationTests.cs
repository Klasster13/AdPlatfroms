using AdPlatforms.Repository;
using AdPlatforms.Repository.Impl;
using AdPlatforms.Services.Impl;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdPlatforms.Tests.DataServiceTests;

public class FindPlatromsForLocationTests
{
    private readonly Mock<IDataRepository> _dataRepositoryMock;
    private readonly DataService _dataService;
    private readonly Mock<ILogger<DataService>> _loggerMock;


    public FindPlatromsForLocationTests()
    {
        _dataRepositoryMock = new Mock<IDataRepository>();
        _loggerMock = new Mock<ILogger<DataService>>();
        _dataService = new DataService(_dataRepositoryMock.Object, _loggerMock.Object);
    }


    [Fact]
    public void FindPlatromsForLocation_ValidLocation_ReturnsRepositoryResult()
    {
        var targetLocation = "/ru/svrd/";
        var expectedResult = new List<string> { "Яндекс.Директ", "Ревдинский рабочий" };
        _dataRepositoryMock.Setup(r => r.FindPlatromsForLocation(targetLocation)).Returns(expectedResult);


        var result = _dataService.FindPlatromsForLocation(targetLocation);


        Assert.Equal(expectedResult, result);
        _dataRepositoryMock.Verify(r => r.FindPlatromsForLocation(targetLocation), Times.Once);
    }


    [Fact]
    public void FindPlatromsForLocation_NullLocation_ReturnsRepositoryResult()
    {
        string? targetLocation = null;
        var expectedResult = new List<string>();
        _dataRepositoryMock.Setup(r => r.FindPlatromsForLocation(targetLocation)).Returns(expectedResult);


        var result = _dataService.FindPlatromsForLocation(targetLocation);


        Assert.Equal(expectedResult, result);
        _dataRepositoryMock.Verify(r => r.FindPlatromsForLocation(targetLocation), Times.Once);
    }


    [Fact]
    public void FindPlatromsForLocation_EmptyLocation_ReturnsRepositoryResult()
    {
        string? targetLocation = "";
        var expectedResult = new List<string>();
        _dataRepositoryMock.Setup(r => r.FindPlatromsForLocation(targetLocation)).Returns(expectedResult);


        var result = _dataService.FindPlatromsForLocation(targetLocation);


        Assert.Equal(expectedResult, result);
        _dataRepositoryMock.Verify(r => r.FindPlatromsForLocation(targetLocation), Times.Once);
    }
}
