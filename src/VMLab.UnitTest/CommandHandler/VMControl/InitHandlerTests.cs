using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.Contract;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.VMControl
{
    public class InitHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<InitHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithTemplateName_Should_CallLabManagerWithInit()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var labManager = fixture.Freeze<ILabManager>();
            var sut = fixture.Create<InitHandler>();

            //Act
            sut.OnHandle(new []{"init", "myTemplate"});

            //Assert
            A.CallTo(() => labManager.Init("myTemplate")).MustHaveHappened();

        }
    }
}
