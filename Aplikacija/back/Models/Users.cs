using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models;

public class Users
{

    [Key]
    public int ID { get; set; }
    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public byte[] Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public bool wantQuizMaker { get; set; }

    public bool wantAdmin { get; set; }

    public string? Bio { get; set; }

    public int NumberOfQuizzesDone { get; set; }

    public bool AccountVerified { get; set; }

    public int Conf_num { get; set; }

    public byte[] Salt { get; set; } = null!;

    public List<QuizUser>? QuizzesDone {get;set;}

    public string? ProfilePicutre { get; set; }     //mirgacije opet

    [JsonIgnore]
    public List<Score>? Scores { get; set; }

    [JsonIgnore]
    public List<Quiz>? QuizzesMade { get; set; }
}