using Microsoft.AspNetCore.Mvc;
using FreelancerDirectory.Application.Interfaces;
using FreelancerDirectory.Domain.Entities;
using FreelancerDirectory.Application.DTOs;

namespace FreelancerDirectory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreelancersController : ControllerBase
{
    private readonly IFreelancerRepository _repo;
    public FreelancersController(IFreelancerRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool archived = false)
    {
        var freelancers = await _repo.GetAllAsync(archived);
        var dtos = freelancers.Select(f => new FreelancerDto
        {
            Id = f.Id,
            Username = f.Username,
            Email = f.Email,
            PhoneNumber = f.PhoneNumber,
            IsArchived = f.IsArchived,
            Skillsets = f.Skillsets.Select(s => s.Skill).ToList(),
            Hobbies = f.Hobbies.Select(h => h.HobbyName).ToList()
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var f = await _repo.GetByIdAsync(id);
        if (f == null) return NotFound();
        var dto = new FreelancerDto
        {
            Id = f.Id,
            Username = f.Username,
            Email = f.Email,
            PhoneNumber = f.PhoneNumber,
            IsArchived = f.IsArchived,
            Skillsets = f.Skillsets.Select(s => s.Skill).ToList(),
            Hobbies = f.Hobbies.Select(h => h.HobbyName).ToList()
        };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FreelancerDto dto)
    {
        var freelancer = new Freelancer
        {
            Username = dto.Username,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Skillsets = dto.Skillsets.Select(s => new SkillSet { Skill = s }).ToList(),
            Hobbies = dto.Hobbies.Select(h => new Hobby { HobbyName = h }).ToList()
        };
        var id = await _repo.CreateAsync(freelancer);
        return CreatedAtAction(nameof(Get), new { id }, null);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FreelancerDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        existing.Username = dto.Username;
        existing.Email = dto.Email;
        existing.PhoneNumber = dto.PhoneNumber;
        existing.Skillsets = dto.Skillsets.Select(s => new SkillSet { Skill = s }).ToList();
        existing.Hobbies = dto.Hobbies.Select(h => new Hobby { HobbyName = h }).ToList();
        var success = await _repo.UpdateAsync(existing);
        return success ? NoContent() : BadRequest();
    }

    [HttpPatch("{id}/archive")]
    public async Task<IActionResult> Archive(int id)
    {
        var success = await _repo.ArchiveAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpPatch("{id}/unarchive")]
    public async Task<IActionResult> Unarchive(int id)
    {
        var success = await _repo.UnarchiveAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repo.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return BadRequest("Query required");
        var results = await _repo.SearchAsync(q);
        var dtos = results.Select(f => new FreelancerDto
        {
            Id = f.Id,
            Username = f.Username,
            Email = f.Email,
            PhoneNumber = f.PhoneNumber,
            IsArchived = f.IsArchived,
            Skillsets = f.Skillsets.Select(s => s.Skill).ToList(),
            Hobbies = f.Hobbies.Select(h => h.HobbyName).ToList()
        });
        return Ok(dtos);
    }
}