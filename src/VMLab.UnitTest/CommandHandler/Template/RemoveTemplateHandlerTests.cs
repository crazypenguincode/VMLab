using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Template;
using VMLab.Contract;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Template
{
    public class RemoveTemplateHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<RemoveTemplateHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithGoodArgs_Should_CallRemoteTemplateOnTemplateManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var templateManager = fixture.Freeze<ITemplateManager>();
            var sut = fixture.Create<RemoveTemplateHandler>();

            //Act
            sut.OnHandle(new [] { "remove", "mytemplate"});

            //Assert
            A.CallTo(() => templateManager.RemoveTemplate("mytemplate")).MustHaveHappened();
        }
    }
}
