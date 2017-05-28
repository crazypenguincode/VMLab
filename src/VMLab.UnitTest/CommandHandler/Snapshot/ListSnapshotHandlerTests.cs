using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.Snapshot;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Snapshot
{
    public class ListSnapshotHandlerTests
    {
        [Fact]
        public void When_Calling_Should_CallExecuteOnScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<ListSnapshotHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNoArgs_Should_ListSnapshotsForAllVMs()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.GetSnapshots()).Returns(fixture.CreateMany<string>());
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmmanger = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmmanger.GetVM(A<VM>.Ignored)).Returns(control);
            var sut = fixture.Create<ListSnapshotHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => control.GetSnapshots()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithVMArgThatDoesntExist_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);
            
            var sut = fixture.Create<ListSnapshotHandler>();

            //Act
            sut.OnHandle(new [] { "list", "-vm", "vmthaqtdoesntexist"});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
