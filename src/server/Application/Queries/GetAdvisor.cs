using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries
{
    public static class GetAdvisor
    {
        public record GetAdvisorByIdCommand(Guid AdvisorId) : IRequest<Response?>;
        public record GetAllAdvisorCommand() : IRequest<List<Response>?>;

        public class Validator : AbstractValidator<GetAdvisorByIdCommand>
        {
            public Validator()
            {
                RuleFor(x => x.AdvisorId)
                    .NotEmpty().WithMessage("AdvisorId should not be empty.")
                    .NotNull().WithMessage("AdvisorId can not be null.");
            }
        }

        public class GetAdvisorByIdCommandHandler : IRequestHandler<GetAdvisorByIdCommand, Response?>
        {
            private readonly AdvisorDbContext _context;

            public GetAdvisorByIdCommandHandler(AdvisorDbContext context)
            {
                _context = context;
            }

            public async Task<Response?> Handle(GetAdvisorByIdCommand request, CancellationToken cancellationToken)
            {
                var advisor = await _context.Advisors.FindAsync(request.AdvisorId);

                if (advisor == null) return null;

                //we can use Mapper for models which have complex properties
                return new Response()
                {
                    AdvisorId = advisor.Id,
                    FullName = advisor.FullName,
                    SIN = MaskSIN(advisor.SIN),
                    Address = advisor.Address,
                    PhoneNumber = MaskPhoneNumber(advisor.PhoneNumber),
                    HealthStatus = advisor.HealthStatus
                };
            }
        }

        public class GetAllAdvisorCommandHandler : IRequestHandler<GetAllAdvisorCommand, List<Response>?>
        {
            private readonly AdvisorDbContext _context;

            public GetAllAdvisorCommandHandler(AdvisorDbContext context)
            {
                _context = context;
            }

            public async Task<List<Response>?> Handle(GetAllAdvisorCommand request, CancellationToken cancellationToken)
            {
                return await _context.Advisors
                    .Select(a => new Response()
                    {
                        AdvisorId = a.Id,
                        FullName = a.FullName,
                        SIN = MaskSIN(a.SIN),
                        Address = a.Address,
                        PhoneNumber = MaskPhoneNumber(a.PhoneNumber),
                        HealthStatus = a.HealthStatus
                    })
                    .ToListAsync(cancellationToken);
            }
        }

        private static string MaskSIN(string sin) => new string('*', sin.Length - 3) + sin[^3..];

        private static string? MaskPhoneNumber(string? phoneNumber) => string.IsNullOrEmpty(phoneNumber) ? null : new string('*', phoneNumber.Length - 4) + phoneNumber[^4..];

        public class Response
        {
            public Guid AdvisorId { get; set; } = default!;
            public string FullName { get; set; } = default!;
            public string SIN { get; set; } = default!;
            public string? Address { get; set; }
            public string? PhoneNumber { get; set; }
            public string HealthStatus { get; set; } = default!;
        }
    }
}
