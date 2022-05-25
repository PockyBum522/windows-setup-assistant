namespace WindowsPostSetupAssistant.UnitTests.Main.Tests;

public class CommandLineInterfaceTests
{
    private ICommandLineInterface? _sut;
    private readonly Mock<ICommandLineInterface> _commandLineMock = new();

    [SetUp]
    public void SetUp()
    {
        var fakeArgs = new[] { "/hello", "/thisWorld", "fakeValue", "/lastCommand" };

        _commandLineMock.Setup(x => x.GetCommandLineArgs())
            .Returns(fakeArgs);

        _sut = _commandLineMock.Object;
    }
    
    [Test]
    public void Sut_AfterSetup_ShouldNotBeNull()
    {
        _sut.Should().NotBeNull();
    }
    
    [Test]
    public void Test_ShouldSimulate_CommandLineArguments()
    {
        var result = _sut?.GetCommandLineArgs();

        result.Should().Equal("/hello", "/thisWorld", "fakeValue", "/lastCommand");
    }
}