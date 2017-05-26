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
    public class AddCredentialHandlerTests
    {
        [Fact]
        public void When_MissingVMSwitch_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new []{ "add", "-group", "mygroup", "-username", "myuser", "-password", "mypassword"});

            //Assert
            A.CallTo(() => console.Error("Missing -vm switch")).MustHaveHappened();
        }

        [Fact]
        public void When_MissingGroupSwitch_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new[] { "add", "-vm", "myvm", "-username", "myuser", "-password", "mypassword" });

            //Assert
            A.CallTo(() => console.Error("Missing -group switch!")).MustHaveHappened();
        }

        [Fact]
        public void When_MissingUsernameSwitch_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new[] { "add", "-vm", "myvm", "-group", "mygroup", "-password", "mypassword" });

            //Assert
            A.CallTo(() => console.Error("Missing -username switch")).MustHaveHappened();
        }

        [Fact]
        public void When_MissingPasswordSwitch_Should_PromptUserForPassword()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new[] { "add", "-vm", "myvm", "-group", "mygroup", "-username", "myuser" });

            //Assert
            A.CallTo(() => console.ReadPassword()).MustHaveHappened();
        }

        [Fact]
        public void When_AllSwitchesPassed_Should_NoThrowError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new[] { "add", "-vm", "myvm", "-group", "mygroup", "-username", "myuser", "-password", "mypassword" });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void When_AllSwitchesPassed_Should_CallScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<AddCredentialHandler>();


            //Act
            sut.OnHandle(new[] { "add", "-vm", "myvm", "-group", "mygroup", "-username", "myuser", "-password", "mypassword" });

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_AllSwitchesPassed_Should_StoreSecurePasswordForVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var credentialManager = fixture.Freeze<ICredentialManager>();
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] {vm});
            var sut = fixture.Create<AddCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "add", "-vm", vm.Name, "-group", "mygroup", "-username", "myuser", "-password", "mypassword" });

            A.CallTo(() => credentialManager.AddSecureCredential(A<Credential>.Ignored, vm)).MustHaveHappened();
        }

        [Fact]
        public void When_AllSwitchesPassedButVMCaseIsDifferent_Should_StoreSecurePasswordForVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var credentialManager = fixture.Freeze<ICredentialManager>();
            var vm = fixture.Create<VM>();
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });
            var sut = fixture.Create<AddCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "add", "-vm", vm.Name.ToUpper(), "-group", "mygroup", "-username", "myuser", "-password", "mypassword" });

            A.CallTo(() => credentialManager.AddSecureCredential(A<Credential>.Ignored, vm)).MustHaveHappened();
        }
    }
}
