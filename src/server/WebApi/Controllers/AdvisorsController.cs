using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Application.Queries;


namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvisorsController : ControllerBase
    {
        private readonly ISender _mediatr;

        public AdvisorsController(ISender mediatr)
        {
            _mediatr = mediatr;
        }

        /// <summary>
        /// List all the Advisors
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("ListAdvisors")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListAdvisors(CancellationToken cancellationToken)
        {
            var response = await _mediatr.Send(new GetAdvisor.GetAllAdvisorCommand(),
                cancellationToken: cancellationToken);

            if (response == null || response.Count == 0)
            {
                return NotFound();
            }

            return Ok(response);
        }

        /// <summary>
        /// Gets Advisor By Id
        /// </summary>
        /// <param name="advisorId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("GetAdvisorById/{advisorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdvisorById(Guid advisorId, CancellationToken cancellationToken)
        {
            var response = await _mediatr.Send(new GetAdvisor.GetAdvisorByIdCommand(advisorId),
                cancellationToken: cancellationToken);

            if (response == null)
            {
                return NotFound("Advisor not found");
            }

            return Ok(response);
        }

        /// <summary>
        /// Creates Advisor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("CreateAdvisor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAdvisor([FromBody] CreateAdvisorCommand.CreateAdvisorRequest value, CancellationToken cancellationToken)
        {
            var response = await _mediatr.Send(new CreateAdvisorCommand.Command(value),
                cancellationToken: cancellationToken);

            var url = Url.Action(nameof(GetAdvisorById), new { advisorId = response.AdvisorId });

            return Created(url, null);
        }

        /// <summary>
        /// Updates the Advisor
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("UpdateAdvisor")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateAdvisor([FromBody] UpdateAdvisorCommand.UpdateAdvisorRequest value, CancellationToken cancellationToken)
        {
            var response = await _mediatr.Send(new UpdateAdvisorCommand.Command(value),
                cancellationToken: cancellationToken);

            if (!response)
            {
                return NotFound("Advisor not found");
            }

            return Accepted();
        }


        /// <summary>
        /// Deletes Advisor
        /// </summary>
        /// <param name="advisorId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("DeleteAdvisor/{advisorId}")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid advisorId, CancellationToken cancellationToken)
        {
            var response = await _mediatr.Send(new DeleteAdvisorCommand.Command(advisorId),
                cancellationToken: cancellationToken);

            if (!response)
            {
                return NotFound("Advisor not found");
            }

            return Accepted();
        }
    }
}
