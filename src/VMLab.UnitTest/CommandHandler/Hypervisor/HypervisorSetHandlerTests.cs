using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Hypervisor;
using VMLab.Helper;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Hypervisor
{
    public class HypervisorSetHandlerTests
    {
        [Fact]
        public void When_CallingWithNoArguments_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var console = fixture.Freeze<IConsole>();
            var sut = fixture.Create<HypervisorSetHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void When_CallingWithNonExistingHypervisor_Should_ReportError()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var finder = fixture.Freeze<IHypervisorFinder>();
            A.CallTo(() => finder.Hypervisors).Returns(new string[] { });

            var console = fixture.Freeze<IConsole>();

            var sut = fixture.Create<HypervisorSetHandler>();

            //Act
            sut.OnHandle(new []{ "set", "nonexistinghypervisor"});

            //Assert
            A.CallTo(() => console.Error(A<string>.Ignored))
                .WhenArgumentsMatch(args => args[0].ToString().Contains("Invalid hypervisor name."))
                .MustHaveHappened();

        }

        [Fact]
        public void When_CAllingWithExistingHypervisor_Should_WriteToConfig()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());

            var finder = fixture.Freeze<IHypervisorFinder>();
            A.CallTo(() => finder.Hypervisors).Returns(new string[] { "existinghypervisor" });

            var config = fixture.Freeze<IConfig>();

            var sut = fixture.Create<HypervisorSetHandler>();

            //Act
            sut.OnHandle(new[] { "set", "existinghypervisor" });

            //Assert
            A.CallTo(() => config.WriteSetting("Hypervisor", "existinghypervisor", ConfigScope.System)).MustHaveHappened();

        }
    }
}
