
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class QuizMakerController : ControllerBase
{
    public Context? Context;
    public QuizMakerController(Context context)
    {
        Context = context;
    }

    //NA FRONTU SE RACUNA PROSECNA OCENA ZA KVIZ

    [Authorize(Roles = "Admin")]
    [HttpGet("GetQuizMakers")]
    public async Task<ActionResult> GetQuizMakers()
    {
        try
        {
            var qm = await Context.Users.Where(p => p.Role == "QuizMaker").ToListAsync();

            if (qm == null)
                return Ok("Nema profila koji prave kvizove");

            return Ok(qm);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetQuizMakersQuizzes/{name}")]
    public async Task<ActionResult> GetQuizMakersQuizzes(string name)
    {
        try
        {
            if (name == null || name == " ")
                return BadRequest("Greska pri pretazi imena");

            var quizMaker = await Context.Users.Where(p => p.Name == name).Include(p => p.QuizzesMade).ThenInclude(q => q.Category).ToListAsync();

            if (quizMaker == null)
                return Ok("Nema kreatora sa tim imenom");

            if (quizMaker.Count == 0)
                return Ok("Taj kreator jos nije napravio ni jedan kviz");

            return Ok(quizMaker);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpGet("QuizzesOfCurrentQuizMaker")]
    public async Task<ActionResult> QuizzesOfCurrentQuizMaker()
    {
        try
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Greska pri pribavljanju trenutnog kreatora kviza");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

            var quizMaker = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (quizMaker == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom quiz maker-u");

            var quizzes = await Context.Quizzes.Where(p => p.QuizMaker.ID == quizMaker.ID).Include(p => p.Category).Include(p => p.Questions).ThenInclude(p => p.Answers).ToListAsync();

            if (quizzes == null)
                return BadRequest("Greska pri pribavljanju kvizeva trenutnog quiz makera");

            if (quizzes.Count == 0)
                return Ok("Jos nema napravljenih kvizova");

            return Ok(quizzes);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPost("CreateQuiz/{title}/{difficulty}/{cat}")]
    public async Task<ActionResult> CreateQuiz(string title, int difficulty, string cat)
    {
        try
        {
            if (String.IsNullOrEmpty(title) || title.Length > 50)
                return BadRequest("Greska kod imenovanja kviza");
            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska pri postavljanju tezine kviza");

            if (string.IsNullOrEmpty(cat))
                return BadRequest("Greska pri odabiru kategorije");

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Greska pri pribavljanju trenutnog kreatora kviza");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid).Value);

            var quizMaker = await Context.Users.Where(p => p.ID == id).Include(p => p.QuizzesMade).FirstOrDefaultAsync();
            if (quizMaker == null)
                return BadRequest("Nije pronadjen kreator kviza");

            var category = await Context.Categorys.Where(p => p.Name == cat).FirstOrDefaultAsync();
            
            if (category == null)
                return BadRequest("Nema te kategorije");

            foreach (var p in Context.Quizzes.ToList())
            {
                if (p.Title.CompareTo(title) == 0)
                    return Ok("To ime za kviz je vec zauzeto odaberite drugo");
            }

            Quiz q = new Quiz();
            q.Questions = new List<Question>();
            q.Category = category;
            q.Difficulty = (Difficulty)difficulty;
            q.Pending = true;
            q.Title = title;
            q.QuizMaker = quizMaker;
            q.QuizExist = false;
            q.TimesPlayed = 0;
            q.UsersPlayed=new List<QuizUser>();
            q.QuizRatings=new List<Rating>();
            quizMaker.QuizzesMade.Add(q);
            Console.WriteLine(category.Quizzes);
            //category.Quizzes.Add(q);        //proveri dal radi 


            await Context.Quizzes.AddAsync(q);
            await Context.SaveChangesAsync();

            return Ok(q.Title);
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return BadRequest(e);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpGet("CheckIfQuizExist/{name}")]
    public async Task<ActionResult> CheckIfQuizExist(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Morate da unesete ime kviza");

            var quiz = await Context.Quizzes.Where(p => p.Title == name).Include(p => p.Questions).FirstOrDefaultAsync();

            if (quiz == null)
                return Ok(false);

            return BadRequest(true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPut("AddQuestionToQuiz/{quizTitle}/{questionText}")]
    public async Task<ActionResult> AddQuestionsToQuiz(string quizTitle, string questionText)
    {
        try
        {
            if (String.IsNullOrEmpty(quizTitle) || String.IsNullOrEmpty(questionText))
                return BadRequest("Greska sa id-jem kviza ili pitanja");

            var quizz = await Context.Quizzes.Include(p => p.Questions).Where(p => p.Title == quizTitle).Include(p => p.Questions).FirstOrDefaultAsync();

            if (quizz == null)
                return BadRequest("Nema tog kviza / Greska sa id-jem kviza");

            var question = await Context.Questions.Where(p => p.Text == questionText).Include(p => p.Quizzes).FirstOrDefaultAsync();

            if (question == null)
                return BadRequest("Nema tog pitanja / Greska sa id-jem pitanja");

            foreach (var p in quizz.Questions.ToList())
            {
                if (p.Text.CompareTo(question.Text) == 0)
                    return Ok("To pitanje vec postoji u kvizu");
            }
            question.Quizzes.Add(quizz);
            quizz.Questions.Add(question);

            Context.Questions.Update(question);
            Context.Quizzes.Update(quizz);
            await Context.SaveChangesAsync();

            return Ok("Uspesno dodato pitanje");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPost("CreateQuestion/{tekst}/{corrAnswer}/{difficulty}/{qCategory}")]
    public async Task<ActionResult> CreateQuestion(string tekst, string corrAnswer, int difficulty, string qCategory)
    {
        try
        {
            if (String.IsNullOrEmpty(tekst) || tekst.Length > 250)
                return BadRequest("Greska sa tekstom pitanja");
            if (String.IsNullOrEmpty(corrAnswer))
                return BadRequest("Greska sa tacnim odgovorom");
            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska sa tezinom pitanja");
            if (String.IsNullOrEmpty(qCategory))
                return BadRequest("Greska sa kategorijom pitanja");

            var question = new Question();
            question.CorrectAnswer = corrAnswer;
            question.Difficulty = (Difficulty)difficulty;
            question.Text = tekst;
            question.Pending = true;

            var category = await Context.QuestionCategorys.Where(p => p.Name == qCategory).FirstOrDefaultAsync();

            if (category == null)
                return BadRequest("Nema te kategorije za to pitanje");

            question.QuestionCategory = category;

            await Context.Questions.AddAsync(question);
            category.Questions.Add(question);
            Context.QuestionCategorys.Update(category);
            await Context.SaveChangesAsync();

            var answer = new Answer();
            answer.Text = corrAnswer;
            answer.Question = question;
            question.Answers = new List<Answer>();
            question.Answers.Add(answer);

            await Context.Answers.AddAsync(answer);
            Context.Questions.Update(question);
            await Context.SaveChangesAsync();

            return Ok("Uspesno dodato pitanje u bazu");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "QuizMaker")]
    [HttpPost("CreateAnswerAndConnectToQuestion/{text}/{question}")]
    public async Task<ActionResult> CreateAnswerAndConnectToQuestion(string text, string question)
    {
        try
        {
            if (String.IsNullOrEmpty(text) || text.Length > 80)
                return BadRequest("Greska sa tekstom odgovora");

            if (String.IsNullOrEmpty(question))
                return BadRequest("Greska sa pitanjem na koje se odnosi odgovor");

            var q = await Context.Questions.Where(p => p.Text == question).FirstOrDefaultAsync();               //iako mozda postoji vec odgovor u bazi moze da ga napravi opet, videcemo da to ipsravimo mozda

            if (q == null)
                return BadRequest("Nema tog pitanja");

            Answer a = new Answer();
            a.Text = text;
            a.Question = q;

            await Context.Answers.AddAsync(a);
            q.Answers.Add(a);
            Context.Questions.Update(q);
            await Context.SaveChangesAsync();

            return Ok("Uspesno dodat odgovor");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPost("CreateAnswer/{text}")]
    public async Task<ActionResult> CreateAnswer(string text)
    {
        try
        {
            if (String.IsNullOrEmpty(text) || text.Length > 80)
                return BadRequest("Greska sa tekstom odgovora");

            Answer a = new Answer();
            a.Text = text;

            await Context.Answers.AddAsync(a);
            await Context.SaveChangesAsync();

            return Ok("Uspesno kreiran odgovor");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPut("AddAnswerToQuestion/{id}/{answer}")]
    public async Task<ActionResult> AddAnswerToQuestion(int id, string answer)
    {
        try
        {
            if (id < 0 || answer == null || answer == " ")
                return BadRequest("Greska sa id-jem ili odgovorom");

            var question = await Context.Questions.Where(p => p.ID == id).FirstOrDefaultAsync();
            var ans = await Context.Answers.Where(p => p.Text == answer).FirstOrDefaultAsync();

            if (question == null)
                return BadRequest("Nema tog pitanja");
            if (ans == null)
                return BadRequest("Nema tog odgovora");

            ans.Question = question;
            question.Answers.Add(ans);

            Context.Answers.Update(ans);
            Context.Questions.Update(question);
            await Context.SaveChangesAsync();

            return Ok("Uspesno dodeljen odgovor pitanju");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}