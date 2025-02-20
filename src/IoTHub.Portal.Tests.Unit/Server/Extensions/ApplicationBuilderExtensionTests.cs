// Copyright (c) CGI France. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace IoTHub.Portal.Tests.Unit.Server.Extensions
{
    public class ApplicationBuilderExtensionTests
    {
        [Test]
        public void UseIfElseMustBranchCodeExecutionWithGivenPredicate()
        {
            // Arrange
            var ifConditionResult = "";
            var expectedIfConditionResult = Guid.NewGuid().ToString();

            var elseConditionResult = "";
            var expectedElseConditionResult = Guid.NewGuid().ToString();

            bool predicate(HttpContext s) => true;

            void ifCondition(IApplicationBuilder condition) => ifConditionResult = expectedIfConditionResult;

            void elseCondition(IApplicationBuilder condition) => elseConditionResult = expectedElseConditionResult;

            var serviceCollection = new ServiceCollection();

            var applicationBuilder = new ApplicationBuilder(serviceCollection.BuildServiceProvider());

            // Act
            applicationBuilder.UseIfElse(predicate, ifCondition, elseCondition);

            // Assert
            _ = ifConditionResult.Should().Be(expectedIfConditionResult);
            _ = elseConditionResult.Should().Be(expectedElseConditionResult);
        }
    }
}
