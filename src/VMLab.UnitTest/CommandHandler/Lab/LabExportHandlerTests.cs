using SystemInterface.IO;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Lab;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Lab
{
    public class LabExportHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArguments_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<LabExportHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonExistingvmlabcsx_should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var file = fixture.Freeze<IFile>();
            A.CallTo(() => file.Exists(A<string>.Ignored)).Returns(false);
            var sut = fixture.Create<LabExportHandler>();

            //Act
            sut.OnHandle(new []{ "export", "c:\\myfile.zip"});

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("vmlab.csx"))).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithExistingVMLabCSX_should_CallScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var file = fixture.Freeze<IFile>();
            A.CallTo(() => file.Exists(A<string>.Ignored)).Returns(true);
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<LabExportHandler>();

            //Act
            sut.OnHandle(new[] { "export", "c:\\myfile.zip" });

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithPoweredOnVM_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();


            var file = fixture.Freeze<IFile>();
            A.CallTo(() => file.Exists(A<string>.Ignored)).Returns(true);

            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.PowerState).Returns(VMPower.Ready);

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);
            
            var sut = fixture.Create<LabExportHandler>();

            //Act
            sut.OnHandle(new[] { "export", "c:\\myfile.zip" });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("stop all of the virtual machines")))
                .MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNoProblems_Should_CallExportLab()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var file = fixture.Freeze<IFile>();
            A.CallTo(() => file.Exists(A<string>.Ignored)).Returns(true);
            var labManager = fixture.Freeze<ILabManager>();
            var sut = fixture.Create<LabExportHandler>();

            //Act
            sut.OnHandle(new[] { "export", "c:\\myfile.zip" });

            //Assert
            A.CallTo(() => labManager.ExportLab("c:\\myfile.zip")).MustHaveHappened();
        }
    }
}

