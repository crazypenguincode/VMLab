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
    public class AddSnapshotHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddSnapshotHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithCorrectArgs_Should_CallScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<AddSnapshotHandler>();

            //Act
            sut.OnHandle(new [] { "add", "mysnapshot"});

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonExistingVMNamePassedViaSwitch_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var sut = fixture.Create<AddSnapshotHandler>();

            //Act
            sut.OnHandle(new[] { "add", "mysnapshot", "-vm", "nonexistingvm" });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("doesn't exist"))).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithGoodArgs_Should_CallNewSnapshot()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);

            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var sut = fixture.Create<AddSnapshotHandler>();

            //Act
            sut.OnHandle(new[] { "add", "mysnapshot" });

            //Assert
            A.CallTo(() => control.NewSnapshot("mysnapshot")).MustHaveHappened();

        }
    }
}
