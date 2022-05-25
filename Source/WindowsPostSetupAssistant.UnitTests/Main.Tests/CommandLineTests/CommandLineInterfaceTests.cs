using WindowsPostSetupAssistant.Main.Interfaces;

namespace WindowsPostSetupAssistant.UnitTests.Main.Tests.CommandLineTests;

public class CommandLineInterfaceTests
{
    private readonly ICommandLineInterface _sut = Substitute.For<ICommandLineInterface>();

    [SetUp]
    public void SetUp()
    {
        var fakeArgs = new[] { "/hello", "/thisWorld", "fakeValue", "/lastCommand" };

        _sut.GetCommandLineArgs().Returns(fakeArgs);
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