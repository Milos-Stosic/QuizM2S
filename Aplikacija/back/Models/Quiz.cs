using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models;
public enum Difficulty
{
    lako,
    srednje,
    tesko
}

public class Quiz
{

    [Key]
    public int ID { get; set; }
    public string Title { get; set; } = null!;
    public bool Pending { get; set; }
    public bool QuizExist { get; set; }
    public int TimesPlayed { get; set; }

    public Difficulty Difficulty { get; set; }

    public List<QuizUser>? UsersPlayed {get;set;}

    public Category? Category { get; set; }

    public List<Question>? Questions { get; set; }

    [JsonIgnore]
    public List<Score>? Scores { get; set; }

    [JsonIgnore]
    public Users? QuizMaker { get; set; }

    public List<Rating>? QuizRatings { get; set; }
}