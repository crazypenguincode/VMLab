using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.CommandHandler.Credentials;
using VMLab.Contract.CredentialManager;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Credentials
{
    public class ClearCredentialHandlerTests
    {
        [Fact]
        public void When_CallingClearCredential_Should_WarnUser()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var credentialManager = fixture.Freeze<ICredentialManager>();
            var console = fixture.Freeze<IConsole>();
            A.CallTo(() => console.ReadLine()).Returns("y");
            var sut = fixture.Create<ClearCredentialHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Information(A<string>.Ignored)).MustHaveHappened();
            A.CallTo(() => credentialManager.ClearAllSecureCredentail()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingClearCredentialWithForceSwitch_Should_NotWarnUser()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ClearCredentialHandler>();

            //Act
            sut.OnHandle(new[] { "clear", "-force"});

            //Assert
            A.CallTo(() => console.Information(A<string>.Ignored)).MustNotHaveHappened();
            A.CallTo(() => console.ReadLine()).MustNotHaveHappened();
        }

        [Fact]
        public void When_CallingClearCredentialAndUserCancels_Should_NotClearCredentails()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var switchParser = fixture.Create<SwitchParser>();
            fixture.Inject<ISwitchParser>(switchParser);
            var credentialManager = fixture.Freeze<ICredentialManager>();
            var console = fixture.Freeze<IConsole>();
            A.CallTo(() => console.ReadLine()).Returns("n");
            var sut = fixture.Create<ClearCredentialHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => credentialManager.ClearAllSecureCredentail()).MustNotHaveHappened();
        }
    }
}
