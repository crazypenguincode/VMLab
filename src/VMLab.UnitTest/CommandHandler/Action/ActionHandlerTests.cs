using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Shouldly;
using VMLab.CommandHandler.Actions;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Action
{
    public class ActionHandlerTests
    {
        [Fact]
        public void When_CallingWithoutArgs_Should_ReturnError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<ActionHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_RunScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptrunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<ActionHandler>();

            //Act
            sut.OnHandle(new[] { "action", "myaction" });

            //Assert
            A.CallTo(() => scriptrunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_Should_RunMatchingAction()
        {
            //Arrange
            var actionRan = false;
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var action = fixture.Create<GraphModels.Action>();
            action.OnAction = (strings, session) => actionRan = true;
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Actions).Returns(new[] {action});
            var sut = fixture.Create<ActionHandler>();

            //Act
            sut.OnHandle(new[] { "action", action.Name });

            //Assert
            actionRan.ShouldBe(true);
        }
    }
}
