using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Exec;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Exec
{
    public class PowershellHandlerTests
    {
        [Fact]
        public void When_CallingWithoutArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<PowershellHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_ExecuteScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<PowershellHandler>();

            //Act
            sut.OnHandle(new [] { "powershell", "vmname", "script.ps1"});

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidVMName_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new VM[]{});

            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<PowershellHandler>();

            //Act
            sut.OnHandle(new[] { "powershell", "vmname", "script.ps1" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored, "vmname")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonProvisionedVM_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            var vm = fixture.Create<VM>();
            A.CallTo(() => graph.VMs).Returns(new [] { vm });

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(null);

            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<PowershellHandler>();

            //Act
            sut.OnHandle(new[] { "powershell", vm.Name, "script.ps1" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).WhenArgumentsMatch(args => args[0].ToString().Contains("provisioned"))  .MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithProvisionedVMAndCorrectArgs_Should_CallPowershell()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            var vm = fixture.Create<VM>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var control = fixture.Freeze<IVMControl>();

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);

            var sut = fixture.Create<PowershellHandler>();

            //Act
            sut.OnHandle(new[] { "powershell", vm.Name, "script.ps1" });

            //Assert
            A.CallTo(() => control.Powershell("script.ps1", true)).MustHaveHappened();
        }
    }
}
