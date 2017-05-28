using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Exec;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Exec
{
    public class ExecHandlerTests
    {

        [Fact]
        public void When_CallingWithoutArgs_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ExecHandler>();

            //Act
            sut.OnHandle(new string[] {});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonExistantVM_Should_ReportError()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ExecHandler>();

            //Act
            sut.OnHandle(new []{ "exec" ,"nonexistingvm", "somecommand"});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored, "nonexistingvm")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingExistingVMThatIsNotProvisioned_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(vm)).Returns(null);
            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<ExecHandler>();

            //Act
            sut.OnHandle(new[] { "exec", vm.Name, "somecommand" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).WhenArgumentsMatch( args => args[0].ToString().Contains("provisioned")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingExistingVMThatIsProvisioned_Should_CallExecute()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(vm)).Returns(control);

            var sut = fixture.Create<ExecHandler>();

            //Act
            sut.OnHandle(new [] {"exec", vm.Name, "command", "args"});

            //Arrange
            A.CallTo(() => control.Exec("command", "args", true)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingExistingVMThatIsProvisionedAndNoCommandParameters_Should_CallExecute()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(vm)).Returns(control);

            var sut = fixture.Create<ExecHandler>();

            //Act
            sut.OnHandle(new[] { "exec", vm.Name, "command" });

            //Arrange
            A.CallTo(() => control.Exec("command", string.Empty, true)).MustHaveHappened();
        }
    }
}
