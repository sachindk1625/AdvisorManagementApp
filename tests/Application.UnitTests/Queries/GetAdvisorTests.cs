using AutoFixture;
using Domain.Models;
using FluentValidation.TestHelper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using static Application.Queries.GetAdvisor;

namespace Application.UnitTests.Queries
{
    public class GetAdvisorTests
    {
        private readonly Mock<AdvisorDbContext> _mockContext = new();
        private readonly IFixture _fixture = new Fixture();

        [Fact]
        public async Task GetAdvisorByIdCommandHandler_ShouldReturnAdvisor_WhenAdvisorExists()
        {
            // Arrange
            var advisorId = Guid.NewGuid();
            var advisor = new AdvisorEntity()
            {
                Id = advisorId,
                FullName = "John Doe",
                SIN = "123456789",
                Address = "123 Main St",
                PhoneNumber = "1234567890",
                HealthStatus = "Good"
            };

            var dbSetMock = new Mock<DbSet<AdvisorEntity>>();
            dbSetMock.Setup(m => m.FindAsync(advisorId)).ReturnsAsync(advisor);
            _mockContext.Setup(c => c.Advisors).Returns(dbSetMock.Object);

            var handler = new GetAdvisorByIdCommandHandler(_mockContext.Object);
            var command = new GetAdvisorByIdCommand(advisorId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(advisorId, result.AdvisorId);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal("******789", result.SIN);
            Assert.Equal("123 Main St", result.Address);
            Assert.Equal("******7890", result.PhoneNumber);
            Assert.Equal("Good", result.HealthStatus);
        }

        [Fact]
        public async Task GetAdvisorByIdCommandHandler_ShouldReturnNull_WhenAdvisorDoesNotExist()
        {
            // Arrange
            var advisorId = Guid.NewGuid();

            var dbSetMock = new Mock<DbSet<AdvisorEntity>>();
            dbSetMock.Setup(m => m.FindAsync(advisorId)).ReturnsAsync((AdvisorEntity?)null);
            _mockContext.Setup(c => c.Advisors).Returns(dbSetMock.Object);

            var handler = new GetAdvisorByIdCommandHandler(_mockContext.Object);
            var command = new GetAdvisorByIdCommand(advisorId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAdvisorCommandHandler_ShouldReturnAllAdvisors()
        {
            // Arrange
            var advisors = _fixture.CreateMany<AdvisorEntity>();
            var mockAdvisorsDbSet = advisors.BuildMock().BuildMockDbSet();
 

            _mockContext.Setup(c => c.Advisors).Returns(mockAdvisorsDbSet.Object);

            var handler = new GetAllAdvisorCommandHandler(_mockContext.Object);
            var command = new GetAllAdvisorCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(advisors.Count(), result.Count);
        }

        [Fact]
        public void Validator_ShouldHaveValidationError_WhenAdvisorIdIsEmpty()
        {
            // Arrange
            var validator = new Validator();
            var command = new GetAdvisorByIdCommand(Guid.Empty);

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(c => c.AdvisorId)
                  .WithErrorMessage("AdvisorId should not be empty.");
        }

        [Fact]
        public void Validator_ShouldNotHaveValidationError_WhenAdvisorIdIsValid()
        {
            // Arrange
            var validator = new Validator();
            var command = new GetAdvisorByIdCommand(Guid.NewGuid());

            // Act
            var result = validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(c => c.AdvisorId);
        }
    }
}
