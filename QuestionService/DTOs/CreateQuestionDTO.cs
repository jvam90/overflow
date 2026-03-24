namespace QuestionService.DTOs;

public record CreateQuestionDTO(string Title, string Content, List<string> Tags);