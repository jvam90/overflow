using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using QuestionService.Models;

namespace QuestionService;

public class QuestionsDbContext(DbContextOptions<QuestionsDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions { get; set; }
    public DbSet<Tag> Tags { get; set; }
}