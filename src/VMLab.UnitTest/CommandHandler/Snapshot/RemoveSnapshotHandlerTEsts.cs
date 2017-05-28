using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.Snapshot;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Snapshot
{
    public class RemoveSnapshotHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<RemoveSnapshotHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidVMName_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);


            var sut = fixture.Create<RemoveSnapshotHandler>();

            //Act
            sut.OnHandle(new [] { "remove", "mysnapshot", "-vm", "nonexistingvm"});

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("doesn't exist"))).MustHaveHappened();
        }

        [Fact]
        public void When_CallingAgainstExistingSnapshot_Should_CallRemoveSnapshot()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.GetSnapshots()).Returns(new[] {"mysnapshot"});
            
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);

            var sut = fixture.Create<RemoveSnapshotHandler>();


            //Act
            sut.OnHandle(new [] {"remove", "mysnapshot"});

            //Assert
            A.CallTo(() => control.RemoveSnapshot("mysnapshot")).MustHaveHappened();
        }
    }
}
