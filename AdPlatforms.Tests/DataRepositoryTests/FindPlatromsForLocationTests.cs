using AdPlatforms.Models;
using AdPlatforms.Repository.Impl;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdPlatforms.Tests.DataRepositoryTests;

public class FindPlatromsForLocationTests
{
    private readonly DataRepository _dataRepository;
    private readonly Mock<ILogger<DataRepository>> _loggerMock;


    public FindPlatromsForLocationTests()
    {
        _loggerMock = new Mock<ILogger<DataRepository>>();
        _dataRepository = new DataRepository(_loggerMock.Object);

        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru"] },
            new() { Name = "Ревдинский рабочий", Locations = ["/ru/svrd/revda","/ru/svrd/pervik"] },
            new() { Name = "Газета уральских москвичей", Locations = ["/ru/msk", "/ru/permobl", "/ru/chelobl"] },
            new() { Name = "Крутая реклама", Locations = ["/ru/svrd"] }
        };

        _dataRepository.UploadData(platforms);
    }


    [Fact]
    public void FindPlatromsForLocation_BaseLocation_ReturnsAllPlatformsForBaseLocation()
    {
        var result = _dataRepository.FindPlatromsForLocation("/ru");


        Assert.Single(result);
        Assert.Contains("Яндекс.Директ", result);
    }


    [Fact]
    public void FindPlatromsForLocation_IntermediateLocation_ReturnsAllPlatformsForLocation()
    {
        var result = _dataRepository.FindPlatromsForLocation("/ru/svrd");


        Assert.Equal(2, result.Count);
        Assert.Contains("Яндекс.Директ", result);
        Assert.Contains("Крутая реклама", result);
    }


    [Fact]
    public void FindPlatromsForLocation_LongLocation_ReturnsAllPlatformsForLocation()
    {
        var result = _dataRepository.FindPlatromsForLocation("/ru/svrd/revda");


        Assert.Equal(3, result.Count);
        Assert.Contains("Яндекс.Директ", result);
        Assert.Contains("Крутая реклама", result);
        Assert.Contains("Ревдинский рабочий", result);
    }


    [Fact]
    public void FindPlatromsForLocation_NonExistingLocation_ReturnsEmptyList()
    {
        var result = _dataRepository.FindPlatromsForLocation("/non/exist");


        Assert.Empty(result);
    }


    [Fact]
    public void FindPlatromsForLocation_NullLocation_ReturnsEmptyList()
    {
        var result = _dataRepository.FindPlatromsForLocation(null);


        Assert.Empty(result);
    }


    [Fact]
    public void FindPlatromsForLocation_EmptyLocation_ReturnsEmptyList()
    {
        var result = _dataRepository.FindPlatromsForLocation("");


        Assert.Empty(result);
    }


    [Fact]
    public void FindPlatromsForLocation_WhitespaceLocation_ReturnsEmptyList()
    {
        var result = _dataRepository.FindPlatromsForLocation("   ");


        Assert.Empty(result);
    }


    [Fact]
    public void FindPlatformsForLocation_LocationWithEndingSlash_WorksCorretly()
    {
        var result1 = _dataRepository.FindPlatromsForLocation("/ru/svrd");
        var result2 = _dataRepository.FindPlatromsForLocation("/ru/svrd/");


        Assert.Equal(result1, result2);
    }


    [Fact]
    public void FindPlatformsForLocation_UpperCaseLocation_ReturnsEmptyList()
    {
        var result = _dataRepository.FindPlatromsForLocation("/RU/SVRD");


        Assert.Empty(result);
    }
}
