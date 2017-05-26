using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest
{
    public class CommandHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArguments_Should_ReturnErrorMessage()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<VMLab.CommandHandler.CommandHandler>();

            //act
            sut.Parse(new string[]{});

            //assert
            A.CallTo(() => console.Error(A<string>.Ignored)).WhenArgumentsMatch(args => ((string)args[0]).Contains("Expected extra parameters")).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithHelp_Should_ReturnUsage()
        {
            //Arange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var usage = fixture.Freeze<IUsage>();
            var sut = fixture.Create<VMLab.CommandHandler.CommandHandler>();

            //Act
            sut.Parse(new[] {"-help"});

            A.CallTo(() => usage.WriteUsage(A<IParamHandler[]>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithValidCommandHandler_Should_CallCommandHandler()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var handler = fixture.Freeze<IParamHandler>();
            A.CallTo(() => handler.CanHandle(A<string[]>.Ignored, A<IEnumerable<IParamHandler>>.Ignored)).Returns(true);
            A.CallTo(() => handler.Group).Returns("root");
            fixture.Inject(handler);
            var sut = fixture.Create<VMLab.CommandHandler.CommandHandler>();

            //Act
            sut.Parse(fixture.Create<string[]>());

            A.CallTo(() => handler.Handle(A<string[]>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithInvalidCommand_Should_ReturnErrorMessage()
        {
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<VMLab.CommandHandler.CommandHandler>();

            sut.Parse(fixture.Create<string[]>());

            A.CallTo(() => console.Error(A<string>.Ignored))
                .WhenArgumentsMatch(args => args[0].ToString().Contains("Unknown command"))
                .MustHaveHappened();
        }
    }
}
