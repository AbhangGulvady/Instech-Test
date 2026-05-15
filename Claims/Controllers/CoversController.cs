using Claims.Services;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers;

// Prefer to have api version for controllers
// skipping this for now since we consider this is early stage of development.
// need to install Microsoft.AspNetCore.Mvc.Versioning package if you want to add api versioning in the future.
// [ApiVersion("1.0")]
[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    private readonly ICoversService _coversService;
    private readonly ILogger<CoversController> _logger;


    public CoversController(ICoversService coversService, ILogger<CoversController> logger)
    {
        _coversService = coversService;
        _logger = logger;
    }

 
    [HttpPost("compute")]
    public ActionResult ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Ok(_coversService.ComputePremium(startDate, endDate, coverType));
    }

 
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
        var results = await _coversService.GetAllAsync();
        return Ok(results);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Cover?>> GetAsync(string id)
    {
        var cover = await _coversService.GetByIdAsync(id);
        return Ok(cover);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        var created = await _coversService.CreateAsync(cover);
        return Ok(created);
    }


    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _coversService.DeleteAsync(id);
    }
}
