using System.Linq;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.VMControl;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.VMControl
{
    public class DestroyHandlerTests
    {
        [Fact]
        public void When_CallingDestroyButLabHasDestroyLock_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Locks).Returns(new[] {"destroy"});
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new string[] {});

            //Arrange
            A.CallTo(() => console.Error(A<string>.That.Contains("lock"))).MustHaveHappened();
            A.CallTo(() => graph.VMs).MustNotHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_ExecuteScriptWithScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptrunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new string[] {});

            //Assert
            A.CallTo(() => scriptrunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_PromptUserIfTheyAreSure()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.ReadLine()).MustHaveHappened();
        }

        [Fact]
        public void When_UserSelectsY_Should_DestroyVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            A.CallTo(() => console.ReadLine()).Returns("y");

            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmManager = fixture.Freeze<IVMManager>();
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new string[] {});

            //Assert
            A.CallTo(() => vmManager.DestroyVM(A<VM>.Ignored, A<IVMControl>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_UserSelectsN_Should_NotCallDestroyVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            A.CallTo(() => console.ReadLine()).Returns("n");

            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmManager = fixture.Freeze<IVMManager>();
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => vmManager.DestroyVM(A<VM>.Ignored, A<IVMControl>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void When_UserPassesForceSwitch_Should_NotAskUser()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());
            
            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new[] { "destroy", "-force" });

            //Assert
            A.CallTo(() => console.ReadLine()).MustNotHaveHappened();
        }

        [Fact]
        public void When_UserPassesVMSwitch_Should_OnlyDestroyVMsSpecified()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

            var vms = fixture.CreateMany<VM>().ToArray();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(vms);
            
            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var vmManager = fixture.Freeze<IVMManager>();
            var sut = fixture.Create<DestroyHandler>();

            //Act
            sut.OnHandle(new[] { "destroy", "-force" , "-vm", vms[0].Name});

            //Assert
            A.CallTo(() => vmManager.DestroyVM(vms[0], A<IVMControl>.Ignored)).MustHaveHappened();
            A.CallTo(() => vmManager.DestroyVM(vms[1], A<IVMControl>.Ignored)).MustNotHaveHappened();
        }
    }
}
