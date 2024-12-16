using AutoFixture;
using Domain.Models;
using FluentValidation.TestHelper;
using Infrastructure;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using static Application.Commands.CreateAdvisorCommand;

namespace Application.UnitTests.Commands
{
    public class CreateAdvisorCommandTests
    {
        private readonly Validator _validator = new();
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<AdvisorDbContext> _mockContext;
        private readonly Handler _handler;

        public CreateAdvisorCommandTests()
        {
            // Mock the DbContext and DbSet
            _mockContext = new Mock<AdvisorDbContext>();
            _handler = new Handler(_mockContext.Object);
        }

        #region ValidationTest

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Empty()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.FullName, " ").Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.FullName)
                  .WithErrorMessage("Full name is required.");
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Is_Null()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().Without(x => x.FullName).Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.FullName)
                  .WithErrorMessage("Full name cannot be null.");
        }

        [Fact]
        public void Should_Have_Error_When_FullName_Exceeds_MaxLength()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.FullName, new string('a', 256))
                .Create());
            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.FullName)
                  .WithErrorMessage("Full name must not exceed 255 characters.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_FullName_Is_Valid()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.FullName, "John Doe")
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Request.FullName);
        }

        [Fact]
        public void Should_Have_Error_When_SIN_Is_Empty()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.SIN, string.Empty)
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.SIN)
                  .WithErrorMessage("SIN is required.");
        }

        [Fact]
        public void Should_Have_Error_When_SIN_Is_Null()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().Without(x => x.SIN)
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.SIN)
                  .WithErrorMessage("SIN cannot be null.");
        }

        [Fact]
        public void Should_Have_Error_When_SIN_Is_Not_Numeric()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.SIN, "ABC123456")
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.SIN)
                  .WithErrorMessage("SIN must be a numeric value.");
        }

        [Fact]
        public void Should_Have_Error_When_SIN_Is_Not_9_Characters_Long()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.SIN, "1234567")
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Request.SIN)
                  .WithErrorMessage("SIN should be 9 numbers");
        }

        [Fact]
        public void Should_Not_Have_Error_When_SIN_Is_Valid()
        {
            var command = new Command(_fixture.Build<CreateAdvisorRequest>().With(x => x.SIN, "123456789")
                .Create());

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Request.SIN);
        }
        #endregion

        #region HandlerTest

        [Fact]
        public async Task Handle_ShouldAddAdvisorEntityToContext()
        {
            // Arrange
            var mockAdvisorsDbSet = _fixture.CreateMany<AdvisorEntity>().BuildMock().BuildMockDbSet();

            _mockContext.Setup(c => c.Advisors).Returns(mockAdvisorsDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new Command(_fixture.Build<CreateAdvisorRequest>().Create());

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(response.AdvisorId);
        }

        #endregion
    }

}
