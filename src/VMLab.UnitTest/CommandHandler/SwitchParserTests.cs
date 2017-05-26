using System;
using System.Collections.Generic;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoFakeItEasy;
using Shouldly;
using VMLab.CommandHandler;
using Xunit;

namespace VMLab.UnitTest.CommandHandler
{
    public class SwitchParserTests
    {
        [Fact]
        public void When_PassingInEmptyArray_Should_ReturnEmptyDictionary()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Parse(new string[] { });

            //Assert
            result.Keys.Count.ShouldBe(0);
        }

        [Fact]
        public void When_PassingInNoSwitchAsFirst_Should_Throw()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act & Assert
            Assert.Throws<ArgumentException>(() => sut.Parse(new[] {"notaswitch"}));
        }

        [Fact]
        public void When_PassingASwitch_Should_BeInResultDictionary()
        {
            //Arramge
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Parse(new string[] { "-myswitch" });

            //Assert
            result.ContainsKey("myswitch").ShouldBe(true);
        }

        [Fact]
        public void When_PassingADoubleSwitch_Should_BeInResultDictionary()
        {
            //Arramge
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Parse(new string[] { "--myswitch" });

            //Assert
            result.ContainsKey("myswitch").ShouldBe(true);
        }

        [Fact]
        public void When_PassingSwitchWithExtraValuesAfterIt_Should_StoreExtraValuesInDictionary()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Parse(new string[] { "-myswitch", "myvalue1", "myvalue2" });

            //Act
            result["myswitch"].ShouldContain(v => v == "myvalue1");
            result["myswitch"].ShouldContain(v => v == "myvalue2");
        }

        [Fact]
        public void When_PassingSwitchRequirementsIn_Should_ReturnTurnIfNonUnexpectedSwitchesExist()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var parsedResults = new Dictionary<string,string[]>
            {
                {"test", new []{"1", "2"} }
            };
            var listOfRequirements = new List<SwitchRequirementDefinition>
            {
                new SwitchRequirementDefinition{ Name = new []{"test"}, Min = 2, Max = 2} 
            };
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Validate(parsedResults, listOfRequirements);

            //Assert
            result.ShouldBe(true);
        }

        [Fact]
        public void When_PassingSwitchRequirementsIn_Should_ReturnFalseIfNonExpectedSwitchExists()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var parsedResults = new Dictionary<string, string[]>
            {
                {"unexpected", new []{"1", "2"} }
            };
            var listOfRequirements = new List<SwitchRequirementDefinition>
            {
                new SwitchRequirementDefinition{ Name = new []{"expected"}, Min = 1, Max = 1}
            };
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Validate(parsedResults, listOfRequirements);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void When_PassingSwitchRequirementsIn_Should_ReturnFalseIfNumberOfValuesIsLargerThanMax()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var parsedResults = new Dictionary<string, string[]>
            {
                {"test", new []{"1", "2"} }
            };
            var listOfRequirements = new List<SwitchRequirementDefinition>
            {
                new SwitchRequirementDefinition{ Name = new []{"test"}, Min = 1, Max = 1}
            };
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Validate(parsedResults, listOfRequirements);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void When_PassingSwitchRequirementsIn_Should_ReturnFalseIFnumberOfValuesIsLessThanMin()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var parsedResults = new Dictionary<string, string[]>
            {
                {"test", new string[]{} }
            };
            var listOfRequirements = new List<SwitchRequirementDefinition>
            {
                new SwitchRequirementDefinition{ Name = new []{"test"}, Min = 1, Max = 1}
            };
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Validate(parsedResults, listOfRequirements);

            //Assert
            result.ShouldBe(false);
        }

        [Fact]
        public void When_PassingSwitchWithMultipleSwitches_Should_StoreExtraValuesInDictionary()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization());
            var sut = fixture.Create<SwitchParser>();

            //Act
            var result = sut.Parse(new string[] { "-myswitch1", "myvalue1", "-myswitch2", "myvalue2" });

            //Act
            result["myswitch1"].ShouldContain(v => v == "myvalue1");
            result["myswitch2"].ShouldContain(v => v == "myvalue2");
        }
    }
}
