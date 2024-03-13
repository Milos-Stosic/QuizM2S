using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    public Context? Context;

    public AdminController(Context context)
    {
        Context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("QuizMakerRequests")]
    public async Task<ActionResult> GetQuizMakerRequests()
    {
        try
        {
            var qmRequest = await Context.Users.Where(p => p.wantQuizMaker == true).ToListAsync();

            if (qmRequest == null)
                return BadRequest("Greksa pri pribavljanju korisnika koji hoce da budu kreatori kviza");

            if (qmRequest.Count == 0)
                return Ok("Nema korisnika koji zele da budu kreatori kviza");

            return Ok(qmRequest);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetAdminRequests")]
    public async Task<ActionResult> GetAdminRequests()
    {
        try
        {
            var adminRequests = await Context.Users.Where(p => p.wantAdmin == true).ToListAsync();

            if (adminRequests == null)
                return BadRequest("Greska pri pribavljanju zahteva");

            if (adminRequests.Count() == 0)
                return Ok("Nema zahteva za ulogu admina");

            return Ok(adminRequests);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles ="Admin")]
    [HttpPut("RoleChange/{id}/{role}")]
    public async Task<ActionResult> ChangeRole(int id, string role)
    {
        try
        {
            if (String.IsNullOrEmpty(role) || id < 0 || (role != "Admin" && role != "QuizMaker" && role != "Guest" && role != "User"))
            {
                return BadRequest("Greska sa id-jem korisnika ili je id uloge pogresan!");
            }

            var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Ne postoji korisnik sa tim id-jem");

            user.Role = role;
            if(role=="QuizMaker")
                user.wantQuizMaker=false;
            if(role=="Admin")
                user.wantAdmin=false;
            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno promenjena uloga");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("AcceptQuiz/{name}")]
    public async Task<ActionResult> AcceptQuiz(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Morate da unesete ime kviza");

            var quiz = await Context.Quizzes.Where(p => p.Title == name).Include(q=>q.Questions).FirstOrDefaultAsync();

            if (quiz == null)
                return BadRequest("Greska sa pribavljanjem tog kviza");

            quiz.Pending = false;

            foreach(var question in quiz.Questions.ToList()){
                question.Pending=false;
                Context.Questions.Update(question);
            }

            Context.Quizzes.Update(quiz);
            await Context.SaveChangesAsync();

            return Ok("Uspesno prihvacen kviz");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteQuiz/{name}")]
    public async Task<ActionResult> DeleteQuiz(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Morate da unesete ime kviza");

            var quiz = await Context.Quizzes.Where(p => p.Title == name).Include(p => p.Category).Include(p => p.QuizMaker).Include(p=>p.QuizRatings).Include(p => p.Scores).Include(p => p.Questions).ThenInclude(p=>p.Answers).FirstOrDefaultAsync();
            
            if (quiz == null)
                return BadRequest("Kviz sa tim imenom ne postoji");
            
            foreach(Question q in quiz.Questions){
                foreach(Answer a in q.Answers){
                    Context.Answers.Remove(a);
                }
                Context.Questions.Remove(q);
            }
            Context.Quizzes.Remove(quiz);
            await Context.SaveChangesAsync();

            return Ok("Uspesno obrisan kviz");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    
    [Authorize(Roles = "Admin")]
    [HttpGet("GetPendingQuizzes")]
    public async Task<ActionResult> GetPendingQuizzes()
    {
        try
        {
            var pendingQuizzes = await Context.Quizzes
                .Where(q => q.Pending && !q.QuizExist)
                .Include(q => q.Category)
                .Include(q => q.QuizMaker)
                .Include(q => q.Scores)
                .Select(q => new Quiz
                {
                    ID = q.ID,
                    // Include any other properties you need from the Quiz entity

                    Category = q.Category,
                    QuizMaker = q.QuizMaker,
                    Scores = q.Scores,
                    Title=q.Title,
                    
                    Questions = q.Questions.Select(question => new Question
                    {
                        ID = question.ID,
                        Text=question.Text,
                        CorrectAnswer=question.CorrectAnswer,
                        Answers = Context.Answers
                            .Where(answer => answer.Question == question)
                            .ToList()
                    }).ToList()
                })
                .ToListAsync();
            if (pendingQuizzes == null)
                return BadRequest("Greska sa pribavljanjem kvizova");

            if (pendingQuizzes.Count == 0)
                return Ok("Nema kvizova koji cekaju odobrenje");
            return Ok(pendingQuizzes);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles ="Admin")]
    [HttpPut("RejectAdminRequest/{id}")]
    public async Task<ActionResult> RejectAdminRequest(int id){
        try{
            if(id<0){
                return BadRequest("Los id admina");
            }
            var admin = await Context.Users.Where(a=>a.ID==id).FirstOrDefaultAsync();

            if(admin==null){
                return BadRequest("Nema admina sa tim id-jem");
            }

            admin.wantAdmin=false;

            Context.Users.Update(admin);
            await Context.SaveChangesAsync();

            return Ok("Uspesno odbijen zahtev");
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles ="Admin")]
    [HttpPut("RejectQMRequest/{id}")]

    public async Task<ActionResult> RejectQMRequest(int id){
        try{
            if(id<0){
                return BadRequest("Los id admina");
            }
            var admin = await Context.Users.Where(a=>a.ID==id).FirstOrDefaultAsync();

            if(admin==null){
                return BadRequest("Nema admina sa tim id-jem");
            }

            admin.wantQuizMaker=false;
            
            Context.Users.Update(admin);
            await Context.SaveChangesAsync();

            return Ok("Uspesno odbijen zahtev");
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }
}