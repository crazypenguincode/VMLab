using FakeItEasy;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using VMLab.CommandHandler.Template;
using VMLab.Contract;
using VMLab.Contract.GraphModels;
using Xunit;

namespace VMLab.UnitTest.CommandHandler.Template
{
    public class ListTemplateHandlerTests
    {
        [Fact]
        public void When_Calling_Should_RetriveListFromManifestManager()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var manifestManager = fixture.Freeze<IManifestManager>();
            A.CallTo(() => manifestManager.GetInstalledTemplateManifests())
                .Returns(fixture.CreateMany<TemplateManifest>());

            var sut = fixture.Create<ListTemplateHandler>();

            //Act
            sut.OnHandle(new string[]{});

            //Assert
            A.CallTo(() => manifestManager.GetInstalledTemplateManifests()).MustHaveHappened();
        }
    }
}
