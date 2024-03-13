using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class ScoreController : ControllerBase
{
    public Context? Context;

    public ScoreController(Context context)
    {
        Context = context;
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetScores")]
    public async Task<ActionResult> GetScores()
    {
        try
        {
            var scores = await Context.Scores.Include(p => p.UserID).Where(p => p.UserID.Role == "User" || p.UserID.Role == "QuizMaker" || p.UserID.Role == "Admin").ToListAsync();

            if (scores == null)
                return BadRequest("Greska pri pribavljanju skorova");

            if (scores.Count == 0)
                return Ok("Jos nema skorova za prikaz");

            return Ok(scores.OrderByDescending(p => p.ScoreValue));

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetScoresOfUser/{name}")]
    public async Task<ActionResult> GetScoresOfUser(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Greska sa pribavljanjem imena korisnika");

            var userScores = await Context.Scores.Where(p => p.UserID.Name == name).Include(p => p.UserID).Include(p => p.Quiz).ToListAsync();
            if (userScores == null)
                return BadRequest("Greska sa pribavljanjem scorova korisnika");

            if (userScores.Count == 0)
                return Ok("Korisnik jos nije odigrao ni jedan kviz");

            return Ok(userScores.OrderByDescending(p => p.ScoreValue));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetScoresOfUserByID/{id}")]
    public async Task<ActionResult> GetScoresOfUserByID(int id)
    {
        try
        {
            if (id < 0)
                return BadRequest("Greska sa pribavljanjem imena korisnika");

            var userScores = await Context.Scores.Where(p => p.UserID.ID == id).Include(p => p.UserID).Include(p => p.Quiz).ToListAsync();

            if (userScores == null)
                return BadRequest("Greska sa pribavljanjem scorova korisnika");

            if (userScores.Count == 0)
                return Ok("Korisnik jos nije odigrao ni jedan kviz");

            return Ok(userScores.OrderByDescending(p => p.ScoreValue));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetAllScoresOfQuiz/{quizName}")]
    public async Task<ActionResult> GetAllScoresOfQuiz(string quizName)
    {
        try
        {
            if (String.IsNullOrEmpty(quizName))
                return BadRequest("Greska sa imenom kviza");

            var scores = await Context.Scores.Where(p => p.Quiz.Title == quizName).Include(p => p.Quiz).Include(p => p.UserID).ToListAsync();

            if (scores == null)
                return BadRequest("Greska pri pribavljanju scorova za kviz");

            if (scores.Count == 0)
                return Ok("Jos uvek niko nije odigrao ovaj kviz");

            return Ok(scores.OrderByDescending(p => p.ScoreValue));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpPut("AddScoreToUser/{score}/{quizName}")]
    public async Task<ActionResult> AddScoreToUser(int score, string quizName)
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

            var u = await Context.Users.Where(p => p.ID == id).Include(p => p.Scores).FirstOrDefaultAsync();

            if (String.IsNullOrEmpty(quizName))
                return BadRequest("Mora da se zna za koji kviz je ovaj skor");

            if (score < 0)
                return BadRequest("Ne mogu bodovi da budu negativni");

            var quiz = await Context.Quizzes.Where(p => p.Title == quizName).Include(p => p.Scores).Include(p => p.UsersPlayed).FirstOrDefaultAsync();
            if (quiz == null)
                return BadRequest("Greska pri pribavljanju ovog kviza");

            if (u == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutno ulogovanom korisniku");

            var user = await Context.Users.Where(p => p.ID == u.ID).Include(p => p.QuizzesDone).Include(p => p.Scores).FirstOrDefaultAsync();
            if (user.QuizzesDone == null)
            {
                Console.WriteLine("Caooooooo" + user);
                QuizUser qu = new QuizUser();
                qu.QuizzesID = quiz.ID;
                qu.Quiz = quiz;
                qu.UsersID = user.ID;
                qu.User = user;

                quiz.UsersPlayed.Add(qu);
                user.QuizzesDone.Add(qu);
                user.NumberOfQuizzesDone++;

                await Context.QuizzesUsers.AddAsync(qu);
            }
            else
            {
                var played = false;
                Console.WriteLine("Heej " + user.QuizzesDone.Count());
                foreach (var qu in user.QuizzesDone)
                {
                    Console.WriteLine("HEEEEJ " + qu.UsersID + " " + user.ID + " " + qu.QuizzesID + " " + quiz.ID);
                    if (qu.UsersID == user.ID && qu.QuizzesID == quiz.ID)
                    {
                        played = true;
                    }
                }

                if (played == false)
                {
                    Console.WriteLine("CAOOOOOOOOO");
                    QuizUser qu = new QuizUser();
                    qu.QuizzesID = quiz.ID;
                    qu.Quiz = quiz;
                    qu.UsersID = user.ID;
                    qu.User = user;

                    quiz.UsersPlayed.Add(qu);
                    user.QuizzesDone.Add(qu);
                    user.NumberOfQuizzesDone++;

                    await Context.QuizzesUsers.AddAsync(qu);
                }
            }
            Context.Users.Update(user);


            var sc = user.Scores.Where(p => p.Quiz == quiz).FirstOrDefault();

            switch (quiz.Difficulty)
            {
                case Difficulty.lako:
                    score = score;
                    break;
                case Difficulty.srednje:
                    score *= 2;
                    break;
                case Difficulty.tesko:
                    score *= 3;
                    break;
            }

            if (sc == null)
            {
                Score s = new Score();
                s.ScoreValue = score;
                s.UserID = user;
                s.Quiz = quiz;
                quiz.Scores.Add(s);
                user.Scores.Add(s);

                await Context.Scores.AddAsync(s);
                Context.Users.Update(user);
                Context.Quizzes.Update(quiz);

                await Context.SaveChangesAsync();

                return Ok("Upesno dodat score");
            }
            else
            {
                if (sc.ScoreValue < score)
                {
                    sc.ScoreValue = score;
                    Context.Scores.Update(sc);
                    await Context.SaveChangesAsync();

                    Console.WriteLine("Ovde je ok");
                }
                else
                {
                    await Context.SaveChangesAsync();
                    return Ok("Niste ostvarili vise poena na ovom kvizu nego sto ste vec imali");
                }

                return Ok("Cao");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("Leaderboard")]
    public async Task<ActionResult> Leaderboard()
    {
        try
        {
            var users = await Context.Users.Where(p => p.Role == "User" || p.Role == "QuizMaker" || p.Role == "Admin").Include(p => p.Scores).ToListAsync();

            if (users == null)
                return BadRequest("Greska pri pribavljanju svih igraca");

            if (users.Count == 0)
                return Ok("Niko jos uvek nije odigrao ni jedan kviz");

            int sum;
            List<int> sume = new List<int>();
            foreach (var p in users)
            {
                sum = 0;
                foreach (var s in p.Scores.ToList())
                {
                    sum += s.ScoreValue;
                }
                sume.Add(sum);
            }
            int i = 0;
            return Ok(users.Select(p => new
            {
                Name = p.Name,
                NumberOfQuizzesPlayed = p.NumberOfQuizzesDone,
                Score = sume[i++]
            }).OrderByDescending(s => s.Score));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin,QuizMaker,User")]
    [HttpGet("GetSumOfScores")]
    public async Task<ActionResult> GetSumOfScores()
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

            var user = await Context.Users.Include(p => p.Scores).Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

            int sum = 0;
            foreach (var p in user.Scores.ToList())
            {
                sum += p.ScoreValue;
            }

            return Ok(sum);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin,QuizMaker,User")]
    [HttpGet("GetSumOfScoresOfUser/{Username}")]
    public async Task<ActionResult> GetSumOfScoresOfUser(string Username)
    {
        try
        {

            var user = await Context.Users.Include(p => p.Scores).Where(p => p.Name == Username).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

            int sum = 0;
            foreach (var p in user.Scores.ToList())
            {
                sum += p.ScoreValue;
            }

            return Ok(sum);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}