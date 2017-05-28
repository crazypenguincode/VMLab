using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Lab;
using VMLab.Contract;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Lab
{
    public class CleanHandlerTests
    {
        [Fact]
        public void When_Calling_Should_CallCleanOnLabManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var labManager = fixture.Freeze<ILabManager>();
            var sut = fixture.Create<CleanHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => labManager.Clean()).MustHaveHappened();
        }
    }
}
