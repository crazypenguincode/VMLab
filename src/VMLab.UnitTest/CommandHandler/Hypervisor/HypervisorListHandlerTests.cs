using System.Linq;
using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.List;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Hypervisor
{
    public class HypervisorListHandlerTests
    {
        [Fact]
        public void When_Calling_Should_PutAStarNextToCurrentSelectedHyeprvisor()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var hypervisors = fixture.CreateMany<string>().ToArray();
            var hypervisorFinder = fixture.Freeze<IHypervisorFinder>();
            A.CallTo(() => hypervisorFinder.Hypervisors).Returns(hypervisors);

            var config = fixture.Freeze<IConfig>();
            A.CallTo(() => config.GetSetting("Hypervisor", ConfigScope.System)).Returns(hypervisors[0]);

            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<HypervisorListHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Information($" * {hypervisors[0]}")).MustHaveHappened();
        }

        [Fact]
        public void When_Calling_should_PutADashNextToOtherHypervisorNames()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var hypervisors = fixture.CreateMany<string>().ToArray();
            var hypervisorFinder = fixture.Freeze<IHypervisorFinder>();
            A.CallTo(() => hypervisorFinder.Hypervisors).Returns(hypervisors);

            var config = fixture.Freeze<IConfig>();
            A.CallTo(() => config.GetSetting("Hypervisor", ConfigScope.System)).Returns(hypervisors[0]);

            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<HypervisorListHandler>();

            //Act
            sut.OnHandle(new string[] { });

            //Assert
            A.CallTo(() => console.Information($" - {hypervisors[1]}")).MustHaveHappened();
        }
    }
}
