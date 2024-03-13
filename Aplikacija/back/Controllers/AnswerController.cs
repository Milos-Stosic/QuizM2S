using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class AnswerController : ControllerBase
{
    public Context? Context;

    public AnswerController(Context context)
    {
        Context = context;
    }

    [AllowAnonymous]
    [HttpGet("CheckIfAnswerIsCorrect/{answer}/{question}")]
    public async Task<ActionResult> CheckIfAnswerIsCorrect(string answer,string question){
        try{
            if(String.IsNullOrEmpty(answer))
                return BadRequest("Greska kod poslatog odgovora");
            if(String.IsNullOrEmpty(question))
                return BadRequest("Greska kod poslatog pitanja");

            var ans= await Context.Answers.Where(p=>p.Text==answer).FirstOrDefaultAsync();

            if(ans==null)
                return BadRequest("Taj odgovor ne postoji u bazi");

            var quest=await Context.Questions.Where(p=>p.Text==question).FirstOrDefaultAsync();

            if(quest==null)
                return BadRequest("Ne postoji to pitanje u bazi");
            
            if(quest.CorrectAnswer.CompareTo(ans.Text)==0)
                return Ok(true);       
            else
                return Ok(false);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }
}