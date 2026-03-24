using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
}