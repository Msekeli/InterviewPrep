using InterviewPrep.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace InterviewPrep.Api.Controllers;

[ApiController]
[Route("api/sessions")]
public class InterviewSessionsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateSession([FromBody] CreateSessionRequest request)
    {
        return Ok(new
        {
            message = "Session endpoint is working",
            request.CvText,
            request.JobSpecText,
            request.CompanyText,
            request.TargetLevel
        });
    }
}