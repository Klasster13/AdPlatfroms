using AdPlatforms.Models;
using AdPlatforms.Repository.Impl;

namespace AdPlatforms.Tests.DataRepositoryTests;

public class UploadDataTests
{
    private readonly DataRepository _dataRepository = new();


    [Fact]
    public void UploadData_ValidData_BuildsCorrectTree()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru"] },
            new() { Name = "Ревдинский рабочий", Locations = ["/ru/svrd/revda","/ru/svrd/pervik"] },
            new() { Name = "Газета уральских москвичей", Locations = ["/ru/msk", "/ru/permobl", "/ru/chelobl"] },
            new() { Name = "Крутая реклама", Locations = ["/ru/svrd"] }
        };


        _dataRepository.UploadData(platforms);
        var result1 = _dataRepository.FindPlatromsForLocation("/ru");
        var result2 = _dataRepository.FindPlatromsForLocation("/ru/svrd");


        Assert.Single(result1);
        Assert.Contains("Яндекс.Директ", result1);
        Assert.Equal(2, result2.Count);
        Assert.Contains("Яндекс.Директ", result2);
        Assert.Contains("Крутая реклама", result2);
    }


    [Fact]
    public void UploadData_SinglePlatformWithMultipleLocation_AddsToAllLocations()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru/msk", "/ru/permobl", "ru/chelobl"] }
        };


        _dataRepository.UploadData(platforms);
        var result1 = _dataRepository.FindPlatromsForLocation("/ru/msk");
        var result2 = _dataRepository.FindPlatromsForLocation("/ru/permobl");
        var result3 = _dataRepository.FindPlatromsForLocation("/ru/chelobl");


        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Single(result3);
        Assert.Contains("Яндекс.Директ", result1);
    }


    [Fact]
    public void UploadData_EmptyList_ClearsTree()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru/msk", "/ru/permobl", "ru/chelobl"] }
        };


        _dataRepository.UploadData(platforms);
        _dataRepository.UploadData([]);
        var result = _dataRepository.FindPlatromsForLocation("/ru/msk");


        Assert.Empty(result);
    }


    [Fact]
    public void UploadData_NullList_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => _dataRepository.UploadData(null));
    }


    [Fact]
    public void UploadData_PlatformWithEmptyLocations_SkipsEmptyLocations()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["", "  ", "    ", "/ru"] }
        };


        _dataRepository.UploadData(platforms);
        var result = _dataRepository.FindPlatromsForLocation("/ru");


        Assert.Single(result);
        Assert.Contains("Яндекс.Директ", result);
    }


    [Fact]
    public void UploadData_PlatformWithDuplicateLocations_WorksCorrect()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru", "/ru"] }
        };


        _dataRepository.UploadData(platforms);
        var result = _dataRepository.FindPlatromsForLocation("/ru");


        Assert.Single(result);
        Assert.Contains("Яндекс.Директ", result);
    }


    [Fact]
    public void UploadData_PlatformsWithSameLocation_WorksCorrect()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru"] },
            new() { Name = "Ревдинский рабочий", Locations = ["/ru"] },
            new() { Name = "Крутая реклама", Locations = ["/ru"] }
        };


        _dataRepository.UploadData(platforms);
        var result = _dataRepository.FindPlatromsForLocation("/ru");


        Assert.Equal(3, result.Count);
        Assert.Contains("Яндекс.Директ", result);
        Assert.Contains("Ревдинский рабочий", result);
        Assert.Contains("Крутая реклама", result);
    }


    [Fact]
    public void UploadData_LongLocation_WorksCorrect()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru/svrd/revda/street/building"] }
        };


        _dataRepository.UploadData(platforms);
        var result = _dataRepository.FindPlatromsForLocation("/ru/svrd/revda/street/building");


        Assert.Single(result);
        Assert.Contains("Яндекс.Директ", result);
    }


    [Fact]
    public void UploadData_ShortLocation_ReturnsEmptyList()
    {
        var platforms = new List<Platform>
        {
            new() { Name = "Яндекс.Директ", Locations = ["/ru/svrd/revda"] }
        };


        _dataRepository.UploadData(platforms);
        var result = _dataRepository.FindPlatromsForLocation("/ru/svrd");


        Assert.Empty(result);
    }
}
