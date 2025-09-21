using AdPlatforms.Models;
using AdPlatforms.Repository;
using AdPlatforms.Services.Impl;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace AdPlatforms.Tests.DataServiceTests;

public class UploadPlatformsFromFileTests
{
    private readonly Mock<IDataRepository> _dataRepositoryMock;
    private readonly DataService _dataService;

    public UploadPlatformsFromFileTests()
    {
        _dataRepositoryMock = new Mock<IDataRepository>();
        _dataService = new DataService(_dataRepositoryMock.Object);
    }


    [Fact]
    public void UploadPlatformsFromFile_ValidFile_CallsRepositoryWithParsedData()
    {
        var fileContent = "Яндекс.Директ:/ru\nРевдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData =>
            repositoryData.Count == 2 &&
            repositoryData[0].Name == "Яндекс.Директ" &&
            repositoryData[1].Name == "Ревдинский рабочий")),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_EmptyFile_CallsRepositoryWithEmptyList()
    {
        var fileMock = CreateMockFormFile("");


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 0)),
            Times.Once);
    }



    [Fact]
    public void UploadPlatformsFromFile_FileWithInvalidLines_SkipInvalidLines()
    {
        var fileContent = "Яндекс.Директ:/ru\nINVALIDLINE\nРевдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 2)),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithEmptyLocations_SkipEmptyLocations()
    {
        var fileContent = "Яндекс.Директ:/ru,  ,/ru/svrd\nРевдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData =>
            repositoryData.Count == 2 &&
            repositoryData[0].Locations.Count == 2 &&
            repositoryData[1].Locations.Count == 2)),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithSpaces_TrimSpaces()
    {
        var fileContent = " Яндекс.Директ : /ru \n Ревдинский рабочий : /ru/svrd/revda , /ru/svrd/pervik ";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData =>
            repositoryData.Count == 2 &&
            repositoryData[0].Name == "Яндекс.Директ" &&
            repositoryData[0].Locations.SequenceEqual(new[] { "/ru" }) &&
            repositoryData[1].Name == "Ревдинский рабочий" &&
            repositoryData[1].Locations.SequenceEqual(new[] { "/ru/svrd/revda", "/ru/svrd/pervik" }))),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithOnlyEmptyLines_CallsRepositoryWithEmptyLines()
    {
        var fileContent = "\n\n \n\t\n";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 0)),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithOnlyPlatformName_CreatesEmptyTree()
    {
        var fileContent = "Яндекс.Директ:";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 0)),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithOnlyLocation_CreatesEmptyTree()
    {
        var fileContent = ":/ru,/ru/msk";
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 0)),
            Times.Once);
    }


    [Fact]
    public void UploadPlatformsFromFile_FileWithManyLines_WorksCorrectly()
    {
        var fileContent = string.Join("\n", Enumerable.Range(1, 1000)
            .Select(i => $"Platform{i}:/location{i}"));
        var fileMock = CreateMockFormFile(fileContent);


        _dataService.UploadPlatformsFromFile(fileMock.Object);


        _dataRepositoryMock.Verify(r => r.UploadData(
            It.Is<List<Platform>>(repositoryData => repositoryData.Count == 1000)),
            Times.Once);
    }



    private static Mock<IFormFile> CreateMockFormFile(string fileContent)
    {
        var fileName = "test.txt";
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        fileMock.Setup(m => m.OpenReadStream()).Returns(stream);
        fileMock.Setup(m => m.FileName).Returns(fileName);
        fileMock.Setup(m => m.Length).Returns(stream.Length);

        return fileMock;
    }
}
