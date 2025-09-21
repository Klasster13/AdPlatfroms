using AdPlatforms.Controllers;
using AdPlatforms.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdPlatforms.Tests.AdsControllerTests;

public class GetPlatformsTests
{
    private readonly Mock<IDataService> _dataServiceMock;
    private readonly AdsController _controller;


    public GetPlatformsTests()
    {
        _dataServiceMock = new Mock<IDataService>();
        _controller = new AdsController(_dataServiceMock.Object);
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
        _dataServiceMock.Verify(s => s.FindPlatromsForLocation("/ru/svrd"), Times.Once());
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
        Assert.Equal("Location parameter is required", badRequestResult.Value);
    }
}
