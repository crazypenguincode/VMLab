using System.Collections.Generic;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler
{
    public class UsageTests
    {
        [Fact]
        public void When_CallingWriteUsage_Should_WriteTextFromParamHandlersInRootGroup()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.Group).Returns("root");
            A.CallTo(() => handler.UsageName).Returns("TestCommandName");
            var sut = fixture.Create<Usage>();

            //Act
            sut.WriteUsage(new []{ handler});

            //Assert
            A.CallTo(() => console.Information(A<string>.Ignored, "TestCommandName", A<string>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void When_CallingWriteUsage_Should_NotWriteTextFromParamHandlersNotInRootGroup()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.Group).Returns("notroot");
            A.CallTo(() => handler.UsageName).Returns("TestCommandName");
            var sut = fixture.Create<Usage>();

            //Act
            sut.WriteUsage(new[] { handler });

            //Assert
            A.CallTo(() => console.Information(A<string>.Ignored, "TestCommandName", A<string>.Ignored))
                .MustNotHaveHappened();
        }

        [Fact]
        public void When_CallingWriteCommandUsageOnRootGroupHandler_Should_WriteShortVmlabUsageString()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.Group).Returns("root");
            var sut = fixture.Create<Usage>();

            //act
            sut.WriteCommandUsage(handler);

            //Assert
            A.CallTo(() => console.Information("vmlab {subcommand} [switches]", A<string>.Ignored)).MustHaveHappened();

        }

        [Fact]
        public void When_CallingWriteCommandUsageOnNonRootGroupHandler_Should_WriteLongVMLabUsageString()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.Group).Returns("notroot");
            var sut = fixture.Create<Usage>();

            //act
            sut.WriteCommandUsage(handler);

            //Assert
            A.CallTo(() => console.Information("vmlab {command} {subcommand} [switches]", A<string>.Ignored, A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWriteCommandUsageWithSwitches_Should_WriteSwitchUsageString()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.UsageItems).Returns(new Dictionary<string, string> {{"itemname", "help"}});
            var sut = fixture.Create<Usage>();

            //act
            sut.WriteCommandUsage(handler);

            //Assert
            A.CallTo(() => console.Information("\t{command}\t - {help}", A<string>.Ignored, A<string>.Ignored))
                .MustHaveHappened();
        }

        [Fact]
        public void When_CallingWriteHubUsage_Should_WriteUsageForHandlersInSameGroup()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var hub = fixture.Create<FakeHub>();
            hub.UsageNameOverride = "subgroup";
            var handler = fixture.Create<IParamHandler>();
            A.CallTo(() => handler.Group).Returns("subgroup");
            A.CallTo(() => handler.UsageName).Returns("commandname");
            var sut = fixture.Create<Usage>();

            //Act
            sut.WriteHubUsage(hub, new []{handler});

            //Assert
            A.CallTo(() => console.Information("\t{command}\t - {help}", "commandname", A<string>.Ignored))
                .MustHaveHappened();

        }

        public class FakeHub : HubParamHandler
        {
            public FakeHub(IUsage usage) : base(usage)
            {
            }

            public string GroupOverride;
            public string UsageDescriptionOverride;
            public string[] HandlesOverride;
            public string SubGroupOverride;
            public string UsageNameOverride;

            public override string UsageName => UsageNameOverride;
            public override string Group => GroupOverride;
            public override string UsageDescription => UsageDescriptionOverride;
            public override string[] Handles => HandlesOverride;
            public override string SubGroup => SubGroupOverride;
        }
    }
}
