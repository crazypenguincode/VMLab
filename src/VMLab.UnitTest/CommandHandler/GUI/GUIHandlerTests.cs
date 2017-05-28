using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.GUI;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.GUI
{
    public class GUIHandlerTests
    {
        [Fact]
        public void When_Calling_Should_CallScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<GUIHandler>();

            //Act
            sut.OnHandle(new string[]{});

            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithVMString_Should_OnlyCallShowUIOnTargetVMs()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vms = fixture.CreateMany<VM>().ToArray();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(vms);

            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(vms[1])).Returns(control);

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var sut = fixture.Create<GUIHandler>();

            //Act
            sut.OnHandle(new []{ "gui", "-vm", vms[1].Name});

            //Assert
            A.CallTo(() => vmManager.GetVM(vms[0])).MustNotHaveHappened();
            A.CallTo(() => control.ShowUI()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithoutArgs_Should_CallShowUIOnAllVMs()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vms = fixture.CreateMany<VM>().ToArray();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(vms);

            var control = fixture.Freeze<IVMControl>();
            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);

            var sut = fixture.Create<GUIHandler>();

            //Act
            sut.OnHandle(new[] { "gui" });

            //Assert
            A.CallTo(() => control.ShowUI()).MustHaveHappened(Repeated.Like(c => c == vms.Length));
        }
    }
}
