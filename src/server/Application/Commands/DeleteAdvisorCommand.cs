using FluentValidation;
using Infrastructure;
using MediatR;

namespace Application.Commands
{
    public static class DeleteAdvisorCommand
    {
        public record Command(Guid AdvisorId) : IRequest<bool>;

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.AdvisorId)
                    .NotEmpty().WithMessage("AdvisorId should not be empty.")
                    .NotNull().WithMessage("AdvisorId can not be null.");
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
                var advisor = await _context.Advisors.FindAsync(request.AdvisorId);
                if (advisor == null) return false;

                _context.Advisors.Remove(advisor);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
        }
    }
}
