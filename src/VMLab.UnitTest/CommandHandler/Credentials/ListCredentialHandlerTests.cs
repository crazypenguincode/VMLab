using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Credentials;
using VMLab.Contract.CredentialManager;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Credentials
{
    public class ListCredentialHandlerTests
    {
        [Fact]
        public void When_Calling_Should_CallScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<ListCredentialHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_CallLoadSecureCredentailsForVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var credMan = fixture.Freeze<ICredentialManager>();
            var graph = fixture.Freeze<IGraphManager>();
            var vm = fixture.Create<VM>();
            A.CallTo(() => graph.VMs).Returns(new[] {vm});
            var sut = fixture.Create<ListCredentialHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => credMan.LoadSecureCredentials(vm)).MustHaveHappened();

        }

        [Fact]
        public void When_Calling_Should_CallAllCredentailsForVM()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var credMan = fixture.Freeze<ICredentialManager>();
            var graph = fixture.Freeze<IGraphManager>();
            var vm = fixture.Create<VM>();
            var cred = fixture.Create<Credential>();
            A.CallTo(() => graph.VMs).Returns(new[] { vm });
            A.CallTo(() => credMan.AllCredentials(vm)).Returns(new[] { cred });
            var sut = fixture.Create<ListCredentialHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Information(A<string>.Ignored, cred.Group, cred.Username, cred.Secure))
                .MustHaveHappened();

        }
    }
}
