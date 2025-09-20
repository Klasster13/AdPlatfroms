using AdPlatforms.Controllers;
using AdPlatforms.Services;
using Moq;

namespace AdPlatforms.Tests.AdsControllerTests;

public class AdsControllerTests
{
    private readonly Mock<IDataService> _dataServiceMock;
    private readonly AdsController _controller;


    public AdsControllerTests()
    {
        _dataServiceMock = new Mock<IDataService>();
        _controller = new AdsController(_dataServiceMock.Object);
    }


    //[Fact]
    //public void Test1()
    //{

    //}
}
