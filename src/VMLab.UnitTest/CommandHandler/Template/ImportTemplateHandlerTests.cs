using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Import;
using VMLab.Contract;
using VMLab.Helper;
using VMLab.Script;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Template
{
    public class ImportTemplateHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ImportTemplateHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithValidArchivePath_Should_CallImportOnTemplateManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var templateManager = fixture.Freeze<ITemplateManager>();
            var sut = fixture.Create<ImportTemplateHandler>();

            //Act
            sut.OnHandle(new []{"import", "c:\\myarchive.zip"});

            //Assert
            A.CallTo(() => templateManager.ImportTemplate("c:\\myarchive.zip")).MustHaveHappened();
        }
    }
}
