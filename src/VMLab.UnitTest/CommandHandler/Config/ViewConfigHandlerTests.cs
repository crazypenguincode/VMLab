using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Config;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Config
{
    public class ViewConfigHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportErrorToConsole()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ViewConfigHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithSystemScope_Should_DumpSystemConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<ViewConfigHandler>();

            //Act
            sut.OnHandle(new[] { "view", "system"});

            //Assert
            A.CallTo(() => config.Dump(ConfigScope.System)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUserScope_Should_DumpUserConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<ViewConfigHandler>();

            //Act
            sut.OnHandle(new[] {"view", "user"});
            //Assert
            A.CallTo(() => config.Dump(ConfigScope.User)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithLabScope_Should_DumpLabConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<ViewConfigHandler>();

            //Act
            sut.OnHandle(new[] { "view", "lab" });
            //Assert
            A.CallTo(() => config.Dump(ConfigScope.Lab)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidScope_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ViewConfigHandler>();

            //Act
            sut.OnHandle(new[] { "view", "totallybogusscope" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
