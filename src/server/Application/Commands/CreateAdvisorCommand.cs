using Domain.Models;
using FluentValidation;
using Infrastructure;
using MediatR;

namespace Application.Commands
{
    /// <summary>
    /// Creates Advisor 
    /// </summary>
    public static class CreateAdvisorCommand
    {
        public record Command(CreateAdvisorRequest Request) : IRequest<Response>;

        /// <summary>
        /// Validates the request for the command
        /// </summary>
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Request.FullName)
                    .NotEmpty()
                    .WithMessage("Full name is required.")
                    .NotNull()
                    .WithMessage("Full name cannot be null.")
                    .MaximumLength(255)
                    .WithMessage("Full name must not exceed 255 characters.");

                RuleFor(x => x.Request.SIN)
                    .NotEmpty()
                    .WithMessage("SIN is required.")
                    .NotNull()
                    .WithMessage("SIN cannot be null.")
                    .Must(sin => long.TryParse(sin, out _))
                    .WithMessage("SIN must be a numeric value.")
                    .Length(9)
                    .WithMessage("SIN should be 9 numbers");
            }
        }

        /// <summary>
        /// Command Handler
        /// </summary>
        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly AdvisorDbContext _context;

            public Handler(AdvisorDbContext context)
            {
                _context = context;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var healthStatus = GenerateHealthStatus();
                var advisor = new AdvisorEntity()
                {
                    Id = Guid.NewGuid(), //we can have another service to generate typed GUID, for simplicity using this now
                    FullName = request.Request.FullName,
                    SIN = request.Request.SIN,
                    Address = request.Request.Address,
                    PhoneNumber = request.Request.PhoneNumber,
                    HealthStatus = healthStatus
                };

                _context.Advisors.Add(advisor);
                await _context.SaveChangesAsync(cancellationToken);

                return new Response()
                {
                    AdvisorId = advisor.Id.ToString()
                };
            }

            private string GenerateHealthStatus()
            {
                var rand = new Random();
                int value = rand.Next(1, 101);
                if (value <= 60) return "Green";
                if (value <= 80) return "Yellow";
                return "Red";
            }
        }

        public record CreateAdvisorRequest
        {
            public string FullName { get; set; } = default!;
            public string SIN { get; set; } = default!;
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
        }

        public class Response
        {
            public string AdvisorId { get; set; } = default!;
        }
    }
}
