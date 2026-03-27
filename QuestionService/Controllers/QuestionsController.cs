using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Models;

namespace QuestionService.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionsController(QuestionsDbContext context) : ControllerBase
{

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionDTO createQuestionDto)
    {

        var validTags = await context.Tags.Where(x => createQuestionDto.Tags.Contains(x.Slug)).ToListAsync();
        var missingTags = createQuestionDto.Tags.Except(validTags.Select(x => x.Slug)).ToList();

        if (missingTags.Count != 0)
        {
            return BadRequest($"Invalid tags: {string.Join(", ", missingTags)}");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue("name");

        if (userId is null || userName is null)
        {
            return BadRequest("Cannot get user details.");
        }

        var question = new Question
        {
            Title = createQuestionDto.Title,
            Content = createQuestionDto.Content,
            TagSlugs = createQuestionDto.Tags,
            AskerId = userId,
            AskerDisplayName = userName,
        };
        
        context.Questions.Add(question);
        await context.SaveChangesAsync();
        return Created($"/questions/{question.Id}", question);
        
    }

    [HttpGet]
    public async Task<ActionResult<List<Question>>> GetQuestions(string? tag)
    {
        var query = context.Questions.AsQueryable();

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(x => x.TagSlugs.Contains(tag));
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetQuestionById(string id)
    {
        var question = await context.Questions.FindAsync(id);

        if (question is null)
        {
            return NotFound();
        }

        await context.Questions.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.ViewCount, x => x.ViewCount + 1));
        
        return question;
    }
}