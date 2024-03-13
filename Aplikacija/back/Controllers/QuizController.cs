using System;
using System.Collections.Generic;
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
public class QuizController : ControllerBase
{
    public Context? Context;

    public QuizController(Context context)
    {
        Context = context;
    }


    [AllowAnonymous]
    [HttpGet("GetQuizzes")]
    public async Task<ActionResult> GetQuizzes()
    {
        try
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    return BadRequest("Greska pri pribavljanju trenutnog korisnika");
                }
                var userClaims = identity.Claims;
                int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

                var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

                if (user == null)
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisniku");

                if (user.Role != "QuizMaker")
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.Category).Include(p => p.QuizRatings).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizeva");

                    if (quizzes.Count == 0)
                        return Ok("Jos uvek nema kvizova za prikaz");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Category = p.Category.Name,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Difficulty));
                }
                else
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.QuizMaker.ID != user.ID && p.Questions.Count != 0).Include(p => p.QuizRatings).Include(p => p.Category).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizeva");

                    if (quizzes.Count == 0)
                        return Ok("Jos uvek nema kvizova za prikaz");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Category = p.Category.Name,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Difficulty));
                }
            }
            catch (Exception)
            {
                var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).Include(p => p.Category).ToListAsync();

                if (quizzes == null)
                    return BadRequest("Greska pri pribavljanju kvizeva");

                if (quizzes.Count == 0)
                    return Ok("Jos uvek nema kvizova za prikaz");

                return Ok(quizzes.Select(p => new
                {
                    Title = p.Title,
                    Category = p.Category.Name,
                    Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                    TimesPlayed = p.TimesPlayed,
                    Ratings = p.QuizRatings
                }).OrderByDescending(p => p.Difficulty));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [AllowAnonymous]
    [HttpGet("GetQuizzesForCategory/{category}")]
    public async Task<ActionResult> GetQuizzesForCategory(string category)
    {
        try
        {
            if (String.IsNullOrEmpty(category))
                return BadRequest("Greska sa odabranom kategorijom");

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    return BadRequest("Greska pri pribavljanju trenutnog korisnika");
                }
                var userClaims = identity.Claims;
                int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

                var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

                if (user == null)
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisniku");

                var cat = await Context.Categorys.Where(p => p.Name == category).FirstOrDefaultAsync();

                if (cat == null)
                    return BadRequest("Nema te kategorije");

                if (user.Role != "QuizMaker")
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska sa pribavljanjem kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Jos uvek nema kvizova u ovoj kategoriji");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Difficulty));
                }
                else
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Pending == false && p.QuizExist == false && p.QuizMaker.ID != user.ID && p.Questions.Count != 0).Include(p => p.QuizRatings).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska sa pribavljanjem kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Jos uvek nema kvizova u ovoj kategoriji");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Difficulty));
                }
            }
            catch (Exception)
            {
                var cat = await Context.Categorys.Where(p => p.Name == category).FirstOrDefaultAsync();

                if (cat == null)
                    return BadRequest("Nema te kategorije");

                var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).ToListAsync();

                if (quizzes == null)
                    return BadRequest("Greska sa pribavljanjem kvizova");
                if (quizzes.Count == 0)
                    return Ok("Jos uvek nema kvizova u ovoj kategoriji");

                return Ok(quizzes.Select(p => new
                {
                    Title = p.Title,
                    Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                    TimesPlayed = p.TimesPlayed,
                    Ratings = p.QuizRatings
                }).OrderByDescending(p => p.Difficulty));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetQuizzesOfDifficulty/{difficulty}")]
    public async Task<ActionResult> GetQuizzesOfDifficulty(int difficulty)
    {
        try
        {
            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska sa odabranom tezinom");

            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    return BadRequest("Greska pri pribavljanju trenutnog korisnika");
                }
                var userClaims = identity.Claims;
                int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

                var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

                if (user == null)
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisniku");

                if (user.Role != "QuizMaker")
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).Include(p => p.Category).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska sa pribavljanjem kvizova");

                    if (quizzes.Count == 0)
                        return Ok("Nema jos uvek kvizova ove tezine");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Category = p.Category.Name,
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Title));
                }
                else
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.QuizMaker.ID != user.ID && p.Questions.Count != 0).Include(p => p.QuizRatings).Include(p => p.Category).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska sa pribavljanjem kvizova");

                    if (quizzes.Count == 0)
                        return Ok("Nema jos kvizova koje ovaj kviz maker nije napravio");

                    return Ok(quizzes.Select(p => new
                    {
                        Title = p.Title,
                        Category = p.Category.Name,
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }).OrderByDescending(p => p.Title));
                }
            }
            catch (Exception)
            {
                var quizzes = await Context.Quizzes.Where(p => p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).Include(p => p.Category).ToListAsync();

                if (quizzes == null)
                    return BadRequest("Greska sa pribavljanjem kvizova");

                if (quizzes.Count == 0)
                    return Ok("Nema jos uvek kvizova ove tezine");

                return Ok(quizzes.Select(p => new
                {
                    Title = p.Title,
                    Category = p.Category.Name,
                    TimesPlayed = p.TimesPlayed,
                    Ratings = p.QuizRatings
                }).OrderByDescending(p => p.Title));
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetQuizzesOfDifficultyAndCategory/{difficulty}/{category}")]
    public async Task<ActionResult> GetQuizzesOfDifficultyAndCategory(int difficulty, string category)
    {
        try
        {
            if (String.IsNullOrEmpty(category))
                return BadRequest("Greska sa odabranom kategorijom");
            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska sa odabranom tezinom");
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity == null)
                {
                    return BadRequest("Greska pri pribavljanju trenutnog korisnika");
                }
                var userClaims = identity.Claims;
                int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

                var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

                if (user == null)
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisniku");

                var cat = await Context.Categorys.Where(p => p.Name == category).FirstOrDefaultAsync();

                if (cat == null)
                    return BadRequest("Nema te kategorije");

                if (user.Role != "QuizMaker")
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).Select(p => p.Title).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Nema jos uvek kvizova te tezine iz te kategorije");

                    return Ok(quizzes);
                }
                else
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0 && p.QuizMaker.ID == user.ID).Include(p => p.QuizRatings).Select(p => p.Title).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Nema jos uvek kvizova te tezine iz te kategorije");

                    return Ok(quizzes);
                }
            }
            catch (Exception)
            {
                var cat = await Context.Categorys.Where(p => p.Name == category).FirstOrDefaultAsync();

                if (cat == null)
                    return BadRequest("Nema te kategorije");

                var quizzes = await Context.Quizzes.Where(p => p.Category == cat && p.Difficulty == (Difficulty)difficulty && p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).Select(p => p.Title).ToListAsync();

                if (quizzes == null)
                    return BadRequest("Greska pri pribavljanju kvizova");
                if (quizzes.Count == 0)
                    return Ok("Nema jos uvek kvizova te tezine iz te kategorije");

                return Ok(quizzes);
            }

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetQuizzesByPopularity")]
    public async Task<ActionResult> GetQuizzesByPopularity()
    {
        try
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    return BadRequest("Greska pri pribavljanju trenutnog korisnika");
                }
                var userClaims = identity.Claims;
                int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

                var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();
                if (user == null)
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisniku");

                if (user.Role != "QuizMaker")
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).OrderByDescending(p => p.TimesPlayed).Include(p => p.Category).ToListAsync();

                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Nema jos uvek kvizova");

                    var pQuizzes = new List<Quiz>();

                    if (quizzes.Count > 6)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            pQuizzes.Add(quizzes[i]);
                        }
                    }
                    else
                        foreach (var p in quizzes)
                        {
                            pQuizzes.Add(p);
                        }

                    return Ok(pQuizzes.Select(p => new
                    {
                        Title = p.Title,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        Category = p.Category.Name,
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }));
                }
                else
                {
                    var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.QuizMaker.ID != user.ID && p.Questions.Count != 0).Include(p => p.QuizRatings).OrderByDescending(p => p.TimesPlayed).Include(p => p.Category).ToListAsync();
                    if (quizzes == null)
                        return BadRequest("Greska pri pribavljanju kvizova");
                    if (quizzes.Count == 0)
                        return Ok("Nema jos uvek kvizova koje niste vi napravili");

                    var pQuizzes = new List<Quiz>();
                    if (quizzes.Count > 6)
                    {

                        for (int i = 0; i < 6; i++)
                        {
                            pQuizzes.Add(quizzes[i]);
                        }
                    }
                    else
                        foreach (var p in quizzes)
                        {
                            pQuizzes.Add(p);
                        }

                    return Ok(pQuizzes.Select(p => new
                    {
                        Title = p.Title,
                        Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                        Category = p.Category.Name,
                        TimesPlayed = p.TimesPlayed,
                        Ratings = p.QuizRatings
                    }));
                }
            }
            catch (Exception)
            {
                var quizzes = await Context.Quizzes.Where(p => p.Pending == false && p.QuizExist == false && p.Questions.Count != 0).Include(p => p.QuizRatings).OrderByDescending(p => p.TimesPlayed).Include(p => p.Category).ToListAsync();

                if (quizzes == null)
                    return BadRequest("Greska pri pribavljanju kvizova");
                if (quizzes.Count == 0)
                    return Ok("Nema jos uvek kvizova");

                var pQuizzes = new List<Quiz>();

                if (quizzes.Count > 6)
                {

                    for (int i = 0; i < 6; i++)
                    {
                        pQuizzes.Add(quizzes[i]);
                    }
                }
                else
                    foreach (var p in quizzes)
                    {
                        pQuizzes.Add(p);
                    }

                return Ok(pQuizzes.Select(p => new
                {
                    Title = p.Title,
                    Difficulty = Enum.GetName(typeof(Difficulty), p.Difficulty),
                    Category = p.Category.Name,
                    TimesPlayed = p.TimesPlayed,
                    Ratings = p.QuizRatings
                }));

            }

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("GenerateQuizOfDifficulty/{difficulty}")]
    public async Task<ActionResult> GenerateQuizCategoryAndDifficulty(int difficulty)
    {
        try
        {
            if (difficulty < 0 || difficulty > 2)
                return BadRequest("Greska sa odabranom tezinom");

            var HardQuestions = await Context.Questions.Where(p => p.Difficulty == Difficulty.tesko && p.Pending == false).Include(p => p.Answers).GroupBy(p => p.Text).Select(p => p.First()).ToListAsync();
            var MediumQuestions = await Context.Questions.Where(p => p.Difficulty == Difficulty.srednje && p.Pending == false).Include(p => p.Answers).GroupBy(p => p.Text).Select(p => p.First()).ToListAsync();
            var EasyQuestions = await Context.Questions.Where(p => p.Difficulty == Difficulty.lako && p.Pending == false).Include(p => p.Answers).GroupBy(p => p.Text).Select(p => p.First()).ToListAsync();

            if (HardQuestions == null || EasyQuestions == null || MediumQuestions == null)
                return BadRequest("Greska pri pribavljanju pitanja iz baze");

            if (HardQuestions.Count == 0)
                return BadRequest("Mora da postoji neko tesko pitanje u bazi");
            if (MediumQuestions.Count == 0)
                return BadRequest("Mora da postoji neko srednje tesko pitanje u bazi");
            if (EasyQuestions.Count == 0)
                return BadRequest("Mora da postoji neko lako pitanje u bazi");


            int numOfEasy, numOfHard, numOfMedium;

            switch (difficulty)
            {
                case 0:
                    numOfEasy = 8;
                    numOfMedium = 2;
                    numOfHard = 0;
                    break;
                case 1:
                    numOfEasy = 5;
                    numOfMedium = 3;
                    numOfHard = 2;
                    break;
                case 2:
                    numOfEasy = 0;
                    numOfMedium = 3;
                    numOfHard = 7;
                    break;
                default:
                    numOfEasy = 0;
                    numOfMedium = 0;
                    numOfHard = 0;
                    break;
            }
            var quizzes = await Context.Quizzes.ToListAsync();

            if (quizzes == null)
                return BadRequest("Greska pri pribavljanju kvizova");

            int numOfQuizzes = quizzes.Count;
            numOfQuizzes++;

            Category cat = await Context.Categorys.Where(p => p.Name == "Random").FirstOrDefaultAsync();

            if (cat == null)
                return BadRequest("Greska sa pribaljanjem kategorije");

            Users user = await Context.Users.Where(p => p.Name == "Random").FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska sa pribavljanjem kreatora za random");

            Quiz quiz = new Quiz();
            quiz.Difficulty = (Difficulty)difficulty;
            quiz.UsersPlayed = new List<QuizUser>();
            quiz.Pending = false;
            quiz.Title = "Random " + numOfQuizzes;
            quiz.Category = cat;
            quiz.QuizMaker = user;

            quiz.Questions = new List<Question>();

            var radnom = new Random();

            for (int i = 0; i < numOfEasy; i++)
            {
                if (i == 0)
                    quiz.Questions.Add(EasyQuestions[radnom.Next(0, EasyQuestions.Count)]);
                else
                {
                    var quest = EasyQuestions[radnom.Next(0, EasyQuestions.Count)];
                    if (quiz.Questions.Contains(quest))
                        continue;
                    else
                    {
                        quiz.Questions.Add(quest);
                    }
                }
            }
            for (int i = 0; i < numOfMedium; i++)
            {
                if (i == 0)
                    quiz.Questions.Add(MediumQuestions[radnom.Next(0, MediumQuestions.Count)]);
                else
                {
                    var quest = MediumQuestions[radnom.Next(0, MediumQuestions.Count)];
                    if (quiz.Questions.Contains(quest))
                        continue;
                    else
                    {
                        quiz.Questions.Add(quest);
                    }
                }
            }
            for (int i = 0; i < numOfHard; i++)
            {
                if (i == 0)
                    quiz.Questions.Add(HardQuestions[radnom.Next(0, HardQuestions.Count)]);
                else
                {
                    var quest = HardQuestions[radnom.Next(0, HardQuestions.Count)];
                    if (quiz.Questions.Contains(quest))
                        continue;
                    else
                    {
                        quiz.Questions.Add(quest);
                    }
                }
            }
            await Context.Quizzes.AddAsync(quiz);
            await Context.SaveChangesAsync();

            return Ok(quiz);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [AllowAnonymous]
    [HttpPut("AddRatingToQuiz/{rating}/{quizName}")]
    public async Task<ActionResult> AddRatingToQuiz(int rating, string quizName)
    {
        try
        {
            var quiz = await Context.Quizzes.Where(p => p.Title == quizName).FirstOrDefaultAsync();

            if (quiz == null)
            {
                return BadRequest("Kviz sa tim imenom ne postoji");
            }

            if (rating < 1 || rating > 5)
            {
                return BadRequest("Ocena moze da bude od 1 do 5");
            }

            Rating r = new Rating();
            r.Rate = rating;
            r.Quiz = quiz;

            await Context.Ratings.AddAsync(r);
            quiz.QuizRatings.Add(r);

            Context.Quizzes.Update(quiz);

            await Context.SaveChangesAsync();

            return Ok("Uspesno ocenjen kviz");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("GetAverageRating/{quizName}")]
    public async Task<ActionResult> GetAverageRating(string quizName)
    {
        try
        {
            var quiz = await Context.Quizzes.Where(q => q.Title == quizName).Include(p => p.QuizRatings).FirstOrDefaultAsync();

            if (quiz == null)
            {
                return BadRequest("Ne postoji kviz sa tim imenom");
            }

            float average = 0;
            float sum = 0;
            Console.WriteLine("CAOOOO "+ quiz.QuizRatings);
            if (quiz.QuizRatings != null && quiz.QuizRatings.Count()!=0)
            {
                foreach (Rating r in quiz.QuizRatings)
                {
                    sum += r.Rate;
                }
                average = sum / quiz.QuizRatings.Count();
                return Ok((float)System.Math.Round(average,2));
            }
            return Ok("NaN");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }




    // [HttpGet("GetScoresForQuiz/{quiz}")]
    // public async Task<ActionResult> GetScoresForQuiz(string quiz)
    // {
    //     try
    //     {
    //         if (String.IsNullOrEmpty(quiz))
    //             return BadRequest("Greska sa trazenim kvizom");

    //         var q = await Context.Quizzes.Where(p => p.Title == quiz).FirstOrDefaultAsync();

    //         if (q == null)
    //             return BadRequest("Greska sa pribavljanjem kviza");

    //         var scores = await Context.Scores.Where(p => p.Quiz == q).Include(p => p.UserID.UserQuiz.Quizzes.Where(q => q.Title == quiz)).ToListAsync();        //nzm dal ovo ima smisla

    //         if (scores == null)
    //             return BadRequest("Greska pri pribavljanju scorova za taj kviz");

    //         return Ok(scores.OrderBy(p => p.ScoreValue));
    //     }
    //     catch (Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
}