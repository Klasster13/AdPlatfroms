using AdPlatforms.Controllers;
using AdPlatforms.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdPlatforms.Tests.AdsControllerTests;

public class UploadTests
{
    private readonly Mock<IDataService> _dataServiceMock;
    private readonly AdsController _controller;


    public UploadTests()
    {
        _dataServiceMock = new Mock<IDataService>();
        _controller = new AdsController(_dataServiceMock.Object);
    }


    [Fact]
    public void Upload_ValidFile_ReturnsOk()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        fileMock.Setup(f => f.Name).Returns("test.txt");


        var response = _controller.Upload(fileMock.Object);


        var result = Assert.IsType<ActionResult<string>>(response);
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Equal("Successful uploading data.", result.Value);

        _dataServiceMock.Verify(s => s.UploadPlatformsFromFile(fileMock.Object),
            Times.Once);
    }


    [Fact]
    public void Upload_ValidFile_ReturnsBadRequest()
    {
        var response = _controller.Upload(null);


        var result = Assert.IsType<ActionResult<string>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("File data is invalid.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.UploadPlatformsFromFile(It.IsAny<IFormFile>()),
            Times.Never());
    }


    [Fact]
    public void Upload_EmptyFile_ReturnsBadRequest()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);


        var response = _controller.Upload(fileMock.Object);


        var result = Assert.IsType<ActionResult<string>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("File data is invalid.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.UploadPlatformsFromFile(It.IsAny<IFormFile>()),
            Times.Never());
    }


    [Fact]
    public void Upload_ServiceThrowsException_ReturnsBadRequest()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(100);
        _dataServiceMock.Setup(s => s.UploadPlatformsFromFile(It.IsAny<IFormFile>()))
            .Throws(new Exception("Error while parsing file."));


        var response = _controller.Upload(fileMock.Object);


        var result = Assert.IsType<ActionResult<string>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Error loading data: Error while parsing file.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.UploadPlatformsFromFile(It.IsAny<IFormFile>()),
            Times.Once());
    }
}
