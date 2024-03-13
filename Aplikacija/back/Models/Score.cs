using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.Text.Json.Serialization;

namespace Models;

public class Score{
    [Key]
    public int ID { get; set; } 
    public int ScoreValue { get; set; }
    public DateTime Timestamp { get; set; }
    public Quiz? Quiz { get; set; }
    //[JsonIgnore]
    public Users? UserID { get; set; }
    
}