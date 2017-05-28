using System.Linq;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using VMLab.Script.FluentInterface;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.VMControl
{
    public class StartHandlerTests
    {
        [Fact]
        public void When_Calling_Should_ExecuteScriptWithScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptrunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => scriptrunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUnprovisionedVMWithNonExistingVM_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(fixture.CreateMany<VM>());

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(null);

            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("doesn't exist"), A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithUnPRovisionedVM_Should_CallBuildFromVMTemplateOnTemplateManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var manifests = fixture.CreateMany<TemplateManifest>().ToArray();
            var vm = fixture.Create<VM>();
            vm.Template = manifests[0].Name;
            vm.Version = "latest";
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new []{vm});

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(null);

            
            var manifestManager = fixture.Freeze<IManifestManager>();
            A.CallTo(() => manifestManager.GetInstalledTemplateManifests())
                .Returns(manifests);

            var templateManager = fixture.Freeze<ITemplateManager>();
            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => templateManager.BuildVMFromTemplate(vm)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithProvisionedVM_Should_PowerItOn()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.PowerState).Returns(VMPower.Off);

            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);


            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => control.Start(true)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithPoweredOnVMShould_ShouldNotTryToStartIt()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.PowerState).Returns(VMPower.Ready);

            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);


            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => control.Start(true)).MustNotHaveHappened();
        }

        [Fact]
        public void When_CallingWithVMSwitch_Should_ShouldOnlyPowerOnTargetVM()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var control = fixture.Freeze<IVMControl>();
            A.CallTo(() => control.PowerState).Returns(VMPower.Off);

            var vms = fixture.CreateMany<VM>().ToArray();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(vms);

            var vmManager = fixture.Freeze<IVMManager>();
            A.CallTo(() => vmManager.GetVM(A<VM>.Ignored)).Returns(control);

            var switchParser = new SwitchParser();
            fixture.Inject<ISwitchParser>(switchParser);

            var sut = fixture.Create<StartHandler>();

            //Act
            sut.OnHandle(new[] { "start", "-vm", vms[0].Name});

            //Assert
            A.CallTo(() => vmManager.GetVM(vms[0])).MustHaveHappened();
            A.CallTo(() => vmManager.GetVM(vms[1])).MustNotHaveHappened();
        }

    }
}
