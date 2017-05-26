using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.Credentials;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Credentials
{
    public class RemoveCredentialHandlerTests
    {
        [Fact]
        public void When_CallingMissingVMArgument_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var sut = fixture.Create<RemoveCredentialHandler>();

            //Act
            sut.OnHandle(new []{ "remove", "-group", "mygroup"});

            //Assert
            A.CallTo(() => console.Error("Missing -vm switch")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingMissingGroupArgument_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var sut = fixture.Create<RemoveCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "remove", "-vm", "myvm" });

            //Assert
            A.CallTo(() => console.Error("Missing -group switch!")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingAllArgs_Should_RunScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var sut = fixture.Create<RemoveCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "remove", "-vm", "myvm", "-group", "mygroup" });

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingAllARgs_Should_CallRemoveSecureCredentialOnVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            var credman = fixture.Freeze<ICredentialManager>();
            A.CallTo(() => graph.VMs).Returns(new[] {vm});
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var sut = fixture.Create<RemoveCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "remove", "-vm", vm.Name, "-group", "mygroup" });

            //Assert
            A.CallTo(() => credman.RemoveSecureCredential(A<string>.Ignored, vm)).MustHaveHappened();
        }
    }
}
