using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models;
public class QuCategory{

    [Key]
    public int ID { get; set; }
    public string Name { get; set; }=null!;
    public List<Question>? Questions { get; set; }
}