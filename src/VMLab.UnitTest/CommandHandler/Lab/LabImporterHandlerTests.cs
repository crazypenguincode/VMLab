using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemInterface.IO;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.Lab;
using VMLab.Contract;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Lab
{
    public class LabImporterHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<LabImportHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CalllingWithNonEmptyDirectory_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var directory = fixture.Freeze<IDirectory>();
            A.CallTo(() => directory.GetFiles(A<string>.Ignored)).Returns(fixture.CreateMany<string>().ToArray());

            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<LabImportHandler>();

            //Act
            sut.OnHandle(new[] { "import", "c:\\mylab.zip" });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("import on a empty directory"))).MustHaveHappened();
        }

        [Fact]
        public void When_CalllingWithSubDirectories_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var directory = fixture.Freeze<IDirectory>();
            A.CallTo(() => directory.GetDirectories(A<string>.Ignored)).Returns(fixture.CreateMany<string>().ToArray());

            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<LabImportHandler>();

            //Act
            sut.OnHandle(new[] { "import", "c:\\mylab.zip" });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("import on a empty directory"))).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonEmptyDirectoryButForceSwitch_Should_CallImportLab()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var directory = fixture.Freeze<IDirectory>();
            A.CallTo(() => directory.GetFiles(A<string>.Ignored)).Returns(fixture.CreateMany<string>().ToArray());

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var labManager = fixture.Freeze<ILabManager>();
            var sut = fixture.Create<LabImportHandler>();

            //Act
            sut.OnHandle(new[] { "import", "c:\\mylab.zip", "-force" });

            //Assert
            A.CallTo(() => labManager.ImportLab("c:\\mylab.zip")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithEmptyDirectory_Should_CallImportLab()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var directory = fixture.Freeze<IDirectory>();
            A.CallTo(() => directory.GetFiles(A<string>.Ignored)).Returns(new string[]{});
            A.CallTo(() => directory.GetDirectories(A<string>.Ignored)).Returns(new string[] { });

            var labManager = fixture.Freeze<ILabManager>();
            var sut = fixture.Create<LabImportHandler>();

            //Act
            sut.OnHandle(new[] { "import", "c:\\mylab.zip" });

            //Assert
            A.CallTo(() => labManager.ImportLab("c:\\mylab.zip")).MustHaveHappened();
        }
    }
}
