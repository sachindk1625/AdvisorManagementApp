using Domain.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure;
using Moq;
using static Application.Commands.UpdateAdvisorCommand;

namespace Application.UnitTests.Commands
{
    public class UpdateAdvisorCommandTests
    {
        [Fact]
        public void Validator_Should_Have_Errors_When_Properties_Are_Invalid()
        {
            // Arrange
            var validator = new Validator();
            var invalidCommand = new Command(new UpdateAdvisorRequest
            {
                AdvisorId = Guid.Empty, // Invalid
                FullName = "",         // Invalid
                SIN = "123"            // Invalid length
            });

            // Act & Assert
            var result = validator.TestValidate(invalidCommand);
            result.ShouldHaveValidationErrorFor(x => x.Request.AdvisorId);
            result.ShouldHaveValidationErrorFor(x => x.Request.FullName);
            result.ShouldHaveValidationErrorFor(x => x.Request.SIN);
        }

        [Fact]
        public void Validator_Should_Not_Have_Errors_When_Properties_Are_Valid()
        {
            // Arrange
            var validator = new Validator();
            var validCommand = new Command(new UpdateAdvisorRequest
            {
                AdvisorId = Guid.NewGuid(),
                FullName = "John Doe",
                SIN = "123456789"
            });

            // Act & Assert
            var result = validator.TestValidate(validCommand);
            result.ShouldNotHaveValidationErrorFor(x => x.Request.AdvisorId);
            result.ShouldNotHaveValidationErrorFor(x => x.Request.FullName);
            result.ShouldNotHaveValidationErrorFor(x => x.Request.SIN);
        }

        [Fact]
        public async Task Handler_Should_Return_False_When_Advisor_Not_Found()
        {
            // Arrange
            var mockDbContext = new Mock<AdvisorDbContext>();
            mockDbContext.Setup(x => x.Advisors.FindAsync(It.IsAny<Guid>()))
                         .ReturnsAsync((AdvisorEntity?)null); // Simulate not found

            var handler = new Handler(mockDbContext.Object);
            var command = new Command(new UpdateAdvisorRequest
            {
                AdvisorId = Guid.NewGuid(),
                FullName = "John Doe",
                SIN = "123456789"
            });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Handler_Should_Update_Advisor_And_Return_True()
        {
            // Arrange
            var advisor = new AdvisorEntity
            {
                Id = Guid.NewGuid(),
                FullName = "Old Name",
                Address = "Old Address",
                PhoneNumber = "1234567890"
            };

            var mockDbContext = new Mock<AdvisorDbContext>();
            mockDbContext.Setup(x => x.Advisors.FindAsync(It.IsAny<Guid>()))
                         .ReturnsAsync(advisor);
            mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                         .ReturnsAsync(1); // Simulate successful save

            var handler = new Handler(mockDbContext.Object);
            var command = new Command(new UpdateAdvisorRequest
            {
                AdvisorId = advisor.Id,
                FullName = "New Name",
                Address = "New Address",
                PhoneNumber = "0987654321"
            });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            advisor.FullName.Should().Be("New Name");
            advisor.Address.Should().Be("New Address");
            advisor.PhoneNumber.Should().Be("0987654321");
        }
    }
}
