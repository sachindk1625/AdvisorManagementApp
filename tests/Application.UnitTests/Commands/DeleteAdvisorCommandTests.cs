using Domain.Models;
using FluentAssertions;
using FluentValidation.TestHelper;
using Infrastructure;
using Moq;
using static Application.Commands.DeleteAdvisorCommand;
namespace Application.UnitTests.Commands
{
    public class DeleteAdvisorCommandTests
    {
        private readonly Validator _validator = new ();
        private readonly Mock<AdvisorDbContext> _mockDbContext;
        private readonly Handler _handler;

        public DeleteAdvisorCommandTests()
        {
            _mockDbContext = new Mock<AdvisorDbContext>();
            _handler = new Handler(_mockDbContext.Object);
        }

        [Fact]
        public void Should_Have_Error_When_AdvisorId_Is_Empty()
        {
            var command = new Command(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.AdvisorId)
                .WithErrorMessage("AdvisorId should not be empty.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_AdvisorId_Is_Valid()
        {
            var command = new Command(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.AdvisorId);
        }

        [Fact]
        public async Task Should_Return_False_When_Advisor_Not_Found()
        {
            // Arrange
            var advisorId = Guid.NewGuid();
            _mockDbContext.Setup(db => db.Advisors.FindAsync(advisorId))
                .ReturnsAsync((AdvisorEntity?)null);

            var command = new Command(advisorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Remove_Advisor_And_Return_True_When_Advisor_Found()
        {
            // Arrange
            var advisorId = Guid.NewGuid();
            var advisor = new AdvisorEntity() { Id = advisorId };

            _mockDbContext.Setup(db => db.Advisors.FindAsync(advisorId))
                .ReturnsAsync(advisor);
            _mockDbContext.Setup(db => db.Advisors.Remove(advisor));
            _mockDbContext.Setup(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new Command(advisorId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            _mockDbContext.Verify(db => db.Advisors.Remove(advisor), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
