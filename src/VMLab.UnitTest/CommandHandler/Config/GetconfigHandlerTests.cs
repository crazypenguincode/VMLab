using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Config;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Config
{
    public class GetconfigHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportErrorToConsole()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithSystemScope_Should_ReturnSystemConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "get", "system", "mysetting" });

            //Assert
            A.CallTo(() => config.GetSetting("mysetting", ConfigScope.System)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUserScope_Should_ReturnUserConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "get", "user", "mysetting" });

            //Assert
            A.CallTo(() => config.GetSetting("mysetting", ConfigScope.User)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithLabScope_Should_ReturnLabConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "get", "lab", "mysetting" });

            //Assert
            A.CallTo(() => config.GetSetting("mysetting", ConfigScope.Lab)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithMergedScope_Should_ReturnMergedConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var config = fixture.Freeze<IConfig>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "get", "merged", "mysetting" });

            //Assert
            A.CallTo(() => config.GetSetting("mysetting", ConfigScope.Merged)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidScope_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<GetConfigHandler>();

            //Act
            sut.OnHandle(new[] { "get", "totallybogusscope", "mysetting" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
