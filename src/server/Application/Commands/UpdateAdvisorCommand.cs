using FluentValidation;
using Infrastructure;
using MediatR;

namespace Application.Commands
{
    public static class UpdateAdvisorCommand
    {
        public record Command(UpdateAdvisorRequest Request) : IRequest<bool>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Request.AdvisorId)
                    .NotEmpty().WithMessage("Advisor Id cannot be Empty.")
                    .NotNull().WithMessage("Advisor Id cannot be null.");

                RuleFor(x => x.Request.FullName)
                    .NotEmpty().WithMessage("Full name is required.")
                    .NotNull().WithMessage("Full name cannot be null.")
                    .MaximumLength(255).WithMessage("Full name must not exceed 255 characters.");

                RuleFor(x => x.Request.SIN)
                    .NotEmpty().WithMessage("SIN is required.")
                    .NotNull().WithMessage("SIN cannot be null.")
                    .Must(sin => long.TryParse(sin, out _)).WithMessage("SIN must be a numeric value.")
                    .Length(9).WithMessage("SIN should be 9 numbers");
            }
        }

        public class Handler : IRequestHandler<Command, bool>
        {
            private readonly AdvisorDbContext _context;

            public Handler(AdvisorDbContext context)
            {
                _context = context;
            }

            public async Task<bool> Handle(Command request, CancellationToken cancellationToken)
            {
                var advisor = await _context.Advisors.FindAsync(request.Request.AdvisorId);
                if (advisor == null) return false;

                advisor.FullName = request.Request.FullName;
                advisor.Address = request.Request.Address;
                advisor.PhoneNumber = request.Request.PhoneNumber;

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }

        public record UpdateAdvisorRequest
        {
            public Guid AdvisorId { get; set; }
            public string FullName { get; set; } = default!;
            public string SIN { get; set; } = default!;
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
        }
    }
}
