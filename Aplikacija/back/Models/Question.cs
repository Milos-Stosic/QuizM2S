using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models;
public class Question
{
    [Key]
    public int ID { get; set; }
    public string Text { get; set; } = null!;
    public string CorrectAnswer { get; set; } = null!;
    public bool Pending { get; set; }
    public Difficulty Difficulty { get; set; }
    [JsonIgnore]
    public QuCategory? QuestionCategory { get; set; } 
    public List<Answer>? Answers { get; set; }
    [JsonIgnore]
    public List<Quiz>? Quizzes { get; set; }
}