using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.VMControl;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.VMControl
{
    public class StatusHandlerTests
    {
        [Fact]
        public void When_Calling_Should_ExecuteScriptWithScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptrunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<StatusHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => scriptrunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUnprovisionedVM_Should_ReportAsUnprovisioned()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());
            

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(null);
            var sut = fixture.Create<StatusHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Information("Unprovisioned")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithProvisionedVM_Should_ReportVMsPowerState()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);
            var sut = fixture.Create<StatusHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Information("Power State: {state}", control.PowerState)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingAndLocksAreSet_Should_ReportLocks()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Locks).Returns(new[] {"myLock"});
            var sut = fixture.Create<StatusHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Information(" - {lock}", "myLock")).MustHaveHappened();
        }
    }
}
