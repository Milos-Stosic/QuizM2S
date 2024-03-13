using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
    public Context? Context;
    public QuestionController(Context context)
    {
        Context = context;
    }

    [HttpGet("GetQuestions")]
    public async Task<ActionResult> GetQuestions()
    {
        try
        {
            var questions = await Context.Questions.GroupBy(p => p.Text).Select(p => p.First()).ToListAsync();

            if (questions == null)
                return BadRequest("Greska sa pribavljanjem pitanja");

            if (questions.Count == 0)
                return Ok("Nema pitanja za prikaz");

            return Ok(questions);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker,User")]
    [HttpGet("GetQuestionsOfCategory/{category}")]
    public async Task<ActionResult> GetQuestionsOfCategory(string category)
    {
        try
        {
            if (String.IsNullOrEmpty(category))
                return BadRequest("Greska sa kategorijom");

            var cat = await Context.QuestionCategorys.Where(p => p.Name == category).FirstOrDefaultAsync();

            if (cat == null)
                return BadRequest("Nema te kategorije");

            var questions = await Context.Questions.Where(p => p.QuestionCategory == cat).GroupBy(p => p.Text).Select(p => p.First()).Include(p => p.Answers).ToListAsync();

            if (questions == null)
                return BadRequest("Greska sa pribavljanjem pitanja");

            if (questions.Count == 0)
                return Ok("Jos nema pitanja za tu kategoriju");

            return Ok(questions);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker,User")]
    [HttpGet("GetQuestionsOfCategoryAndDifficulty/{category}/{difficulty}")]
    public async Task<ActionResult> GetQuestionsOfCategoryAndDifficulty(string category, int difficulty)
    {
        try
        {
            if (String.IsNullOrEmpty(category))
                return BadRequest("Greska sa kategorijom");

            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska sa tezinom");

            var cat = await Context.QuestionCategorys.Where(p => p.Name == category).FirstOrDefaultAsync();

            if (cat == null)
                return BadRequest("Nema te kategorije");

            var questions = await Context.Questions.Where(p => p.QuestionCategory == cat && p.Difficulty == (Difficulty)difficulty).GroupBy(p => p.Text).Select(p => p.First()).Include(p => p.Answers).ToListAsync();

            if (questions == null)
                return BadRequest("Greska sa pribavljanjem pitanja");

            if (questions.Count == 0)
                return Ok("Nema pitanja za prikaz");

            return Ok(questions);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetQuestionsForQuiz/{name}")]
    public async Task<ActionResult> GetQuestionsForQuiz(string name)
    {
       try
        {
            var pendingQuizzes = await Context.Quizzes
                .Where(q => !q.Pending && !q.QuizExist && q.Title==name)
                .Include(q => q.Category)
                .Include(q => q.QuizMaker)
                .Include(q => q.Scores)
                .Include(q=>q.Questions)
                .ThenInclude(q=>q.Answers)
                // .Select(q => new Quiz
                // {
                //     ID = q.ID,
                //     TimesPlayed=q.TimesPlayed,

                //     Category = q.Category,
                //     QuizMaker = q.QuizMaker,
                //     Scores = q.Scores,
                //     Title=q.Title,

                //     Questions = q.Questions.Select(question => new Question
                //     {
                //         ID = question.ID,
                //         Text=question.Text,
                //         CorrectAnswer=question.CorrectAnswer,
                //         Answers = Context.Answers
                //             .Where(answer => answer.Question == question)
                //             .ToList()
                //     }).ToList()
                //})
                .FirstOrDefaultAsync();
            if (pendingQuizzes == null)
                return BadRequest("Greska sa pribavljanjem kvizova");
            
            pendingQuizzes.TimesPlayed++;

            Context.Quizzes.Update(pendingQuizzes);
            await Context.SaveChangesAsync();

            return Ok(pendingQuizzes);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}