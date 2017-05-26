using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Config;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Config
{
    public class SetConfigHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportErrorToConsole()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<SetConfigHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithSystemScope_Should_WriteSystemConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<SetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "set", "system", "mysetting", "myvalue" });

            //Assert
            A.CallTo(() => config.WriteSetting("mysetting", "myvalue", ConfigScope.System)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUserScope_Should_WriteUserConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<SetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "set", "user", "mysetting", "myvalue" });

            //Assert
            A.CallTo(() => config.WriteSetting("mysetting", "myvalue", ConfigScope.User)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithLabScope_Should_WriteLabConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<SetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "set", "lab", "mysetting", "myvalue" });

            //Assert
            A.CallTo(() => config.WriteSetting("mysetting", "myvalue", ConfigScope.Lab)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidScope_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<SetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "set", "totallybogusscope", "mysetting", "myvalue" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
