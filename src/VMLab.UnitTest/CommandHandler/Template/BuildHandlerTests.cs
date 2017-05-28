using System;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler;
using VMLab.Contract;
using VMLab.GraphModels;
using VMLab.Helper;
using VMLab.Script;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Template
{
    public class BuildHandlerTests
    {
        [Fact]
        public void When_Calling_Should_ExecuteScriptRunner()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var scriptRunner = fixture.Freeze<IScriptRunner>();
            var sut = fixture.Create<BuildHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => scriptRunner.Execute()).MustHaveHappened();
        }

        [Fact]
        public void When_CallingIfCapabilityCheckerReportsIncompatabile_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Templates).Returns(fixture.CreateMany<GraphModels.Template>());

            var capabilitychecker = fixture.Freeze<IHypervisorCapabilityChecker>();
            A.CallTo(() => capabilitychecker.CheckTemplate(A<GraphModels.Template>.Ignored)).Returns(new Tuple<bool, string>(false, "Error"));

            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<BuildHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingIfTemplateManagerCantBuild_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Templates).Returns(fixture.CreateMany<GraphModels.Template>());

            var capabilitychecker = fixture.Freeze<IHypervisorCapabilityChecker>();
            A.CallTo(() => capabilitychecker.CheckTemplate(A<GraphModels.Template>.Ignored)).Returns(new Tuple<bool, string>(true, String.Empty));

            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<BuildHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Error(A<string>.That.Contains("Unable to build template"))).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithValidTemplate_Should_CallBuildOnTemplateManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var graph = fixture.Freeze<IGraphManager>();
            A.CallTo(() => graph.Templates).Returns(fixture.CreateMany<GraphModels.Template>());

            var capabilitychecker = fixture.Freeze<IHypervisorCapabilityChecker>();
            A.CallTo(() => capabilitychecker.CheckTemplate(A<GraphModels.Template>.Ignored)).Returns(new Tuple<bool, string>(true, String.Empty));

            var templateManager = fixture.Freeze<ITemplateManager>();
            A.CallTo(() => templateManager.CanBuild(A<GraphModels.Template>.Ignored)).Returns(true);

            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<BuildHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => templateManager.Build(A<GraphModels.Template>.Ignored, A<string>.Ignored))
                .MustHaveHappened();
        }
    }
}
