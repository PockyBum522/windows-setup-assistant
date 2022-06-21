using WindowsPostSetupAssistant.Main.CommandLine;
using WindowsPostSetupAssistant.Main.Interfaces;

namespace WindowsPostSetupAssistant.UnitTests.Main.Tests.CommandLineTests;

public class ArgumentsParserTests
{
    private ArgumentsParser? _sut;
    private readonly ICommandLineInterface _commandLineInterfaceMock = Substitute.For<ICommandLineInterface>();

    [SetUp]
    public void Setup()
    {
        _sut = new ArgumentsParser(_commandLineInterfaceMock);

        var fakeCommandLineInput = 
            new[] { "/chooseProfile", "/secondArgument", "secondArgumentValue", "/lastArgument" };

        _commandLineInterfaceMock.GetCommandLineArgs().Returns(fakeCommandLineInput);
    }
    
    [Test]
    public void Sut_OnSetup_ShouldNotBeNull()
    {
        _sut.Should().NotBeNull();
    }
    
    [Test]
    public void ArgumentPresent_WhenArgumentPresent_ShouldReturnTrue()
    {
        var result = _sut?.ArgumentPresent("/chooseProfile");

        result.Should().Be(true);
    }

    [Test]
    public void ArgumentPresent_WhenArgumentPresent_ShouldReturnTrueRegardlessOfCapitalization()
    {
        var result = _sut?.ArgumentPresent("/cHOOsePrOfIlE");
        
        result.Should().Be(true);
    }

    [Test]
    public void ArgumentPresent_WhenArgumentNotPresent_ShouldReturnFalse()
    {
        var result = _sut?.ArgumentPresent("/garbageArgument");
        
        result.Should().Be(false);
    }
    
    [Test]
    public void GetArgumentValue_WhenArgumentNotPresent_ShouldReturnEmpty()
    {
        var result = _sut?.GetArgumentValue("/garbageArgument");
        
        result.Should().Be("");
    }
    
    [Test]
    public void GetArgumentValue_WhenArgumentFollowedByString_ShouldReturnArgumentValue()
    {
        var result = _sut?.GetArgumentValue("/secondArgument");
        
        result.Should().Be("secondArgumentValue");
    }
    
    [Test]
    public void GetArgumentValue_WhenArgumentIsCapitalizedStrangely_ShouldReturnArgumentValue()
    {
        var result = _sut?.GetArgumentValue("/sEcOnDArGuMent");
        
        result.Should().Be("secondArgumentValue");
    }    
    
    [Test]
    public void GetArgumentValue_WhenArgumentFollowedByAnotherArgument_ShouldReturnEmpty()
    {
        var result = _sut?.GetArgumentValue("/chooseProfile");
        
        result.Should().Be("");
    }
    
    [Test]
    public void GetArgumentValue_WhenCalledOnLastArgument_ShouldReturnEmpty()
    {
        var result = _sut?.GetArgumentValue("/lastArgument");
        
        result.Should().Be("");
    }
}