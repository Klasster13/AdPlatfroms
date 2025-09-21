using AdPlatforms.Controllers;
using AdPlatforms.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace AdPlatforms.Tests.AdsControllerTests;

public class GetPlatformsTests
{
    private readonly Mock<IDataService> _dataServiceMock;
    private readonly Mock<ILogger<AdsController>> _loggerMock;
    private readonly AdsController _controller;


    public GetPlatformsTests()
    {
        _dataServiceMock = new Mock<IDataService>();
        _loggerMock = new Mock<ILogger<AdsController>>();
        _controller = new AdsController(_dataServiceMock.Object, _loggerMock.Object);
    }


    [Fact]
    public void GetPlatforms_ValidLocation_ReturnsOkWithPlatformsList()
    {
        var expectedPlatforms = new List<string> { "Яндекс.Директ", "Крутая реклама" };
        _dataServiceMock.Setup(s => s.FindPlatromsForLocation("/ru/svrd"))
            .Returns(expectedPlatforms);


        var response = _controller.GetPlatforms("/ru/svrd");


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);

        Assert.Equal(expectedPlatforms, result.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation("/ru/svrd"),
            Times.Once());
    }


    [Fact]
    public void GetPlatforms_NullLocation_ReturnsBadRequest()
    {
        var response = _controller.GetPlatforms(null);


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Valid location parameter is required.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation(It.IsAny<string>()),
            Times.Never);

    }


    [Fact]
    public void GetPlatforms_EmptyLocation_ReturnsBadRequest()
    {
        var response = _controller.GetPlatforms("");


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Valid location parameter is required.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation(It.IsAny<string>()),
            Times.Never);
    }


    [Fact]
    public void GetPlatforms_WhitespaceLocation_ReturnsBadRequest()
    {
        var response = _controller.GetPlatforms("    ");


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("Valid location parameter is required.", badRequestResult.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation(It.IsAny<string>()),
            Times.Never);
    }


    [Fact]
    public void GetPlatforms_ServiceThrowsException_ReturnsBadRequest()
    {
        _dataServiceMock.Setup(s => s.FindPlatromsForLocation(It.IsAny<string>()))
            .Throws(new Exception("Internal error."));


        var response = _controller.GetPlatforms("/ru");


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.NotNull(result.Result);
        Assert.Null(result.Value);

        var objResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objResult.StatusCode);
        Assert.Equal("Error getting data: Internal error.", objResult.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation(It.IsAny<string>()),
            Times.Once);
    }


    [Fact]
    public void GetPlatforms_ServiceReturnEmptyList_ReturnsOkWithEmptyList()
    {
        _dataServiceMock.Setup(s => s.FindPlatromsForLocation("/non/exist"))
            .Returns([]);


        var response = _controller.GetPlatforms("/non/exist");


        var result = Assert.IsType<ActionResult<List<string>>>(response);
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);

        _dataServiceMock.Verify(s => s.FindPlatromsForLocation("/non/exist"),
            Times.Once);
    }
}
