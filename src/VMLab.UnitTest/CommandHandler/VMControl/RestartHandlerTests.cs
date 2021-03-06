﻿using System.Linq;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.VMControl;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.VMControl
{
    public class RestartHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArgs_Should_CallRestartOnAllVMs()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);
            var sut = fixture.Create<RestartHandler>();

            //Act
            sut.OnHandle(new string[]{ });

            //Assert
            A.CallTo(() => control.Restart(false)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithForceSwitch_Should_CallRestartWithForce()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());
            
            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);
            var sut = fixture.Create<RestartHandler>();
            
            //Act
            sut.OnHandle(new [] { "restart", "-force"});

            //Assert
            A.CallTo(() => control.Restart(true)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithVMSwitch_Should_OnlyCallRestartOnTargetVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            var vms = fixture.CreateMany<VM>().ToArray();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(vms);

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);
            var sut = fixture.Create<RestartHandler>();

            //Act
            sut.OnHandle(new[] { "restart", "-vm",  vms[1].Name});

            //Assert
            A.CallTo(() => vmManager.GetVM(vms[1])).MustHaveHappened();
            A.CallTo(() => vmManager.GetVM(vms[0])).MustNotHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_ExecuteScriptWithScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptrunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<RestartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => scriptrunner.Execute()).MustHaveHappened();
        }
    }
}
