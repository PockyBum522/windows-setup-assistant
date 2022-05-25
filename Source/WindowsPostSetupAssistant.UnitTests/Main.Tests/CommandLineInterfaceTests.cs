namespace WindowsPostSetupAssistant.UnitTests.Main.Tests;

public class CommandLineInterfaceTests
{
    private readonly ICommandLineInterface _commandLineMock = Substitute.For<ICommandLineInterface>();

    [SetUp]
    public void SetUp()
    {
        var fakeArgs = new[] { "/hello", "/thisWorld", "fakeValue", "/lastCommand" };

        _commandLineMock.GetCommandLineArgs().Returns(fakeArgs);
    }
    
    [Test]
    public void Sut_AfterSetup_ShouldNotBeNull()
    {
        _commandLineMock.Should().NotBeNull();
    }
    
    [Test]
    public void Test_ShouldSimulate_CommandLineArguments()
    {
        var result = _commandLineMock?.GetCommandLineArgs();

        result.Should().Equal("/hello", "/thisWorld", "fakeValue", "/lastCommand");
    }
}