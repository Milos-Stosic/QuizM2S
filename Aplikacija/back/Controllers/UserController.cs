using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    public Context? Context;

    public UserController(Context context)
    {
        Context = context;
    }

    [HttpPost("SendEmail/{email}")]
    public async Task<ActionResult> SendEmail(string email, [FromBody] string message)
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
                    return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

                SmtpClient Client = new SmtpClient()
                {
                    Host = "smtp.outlook.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "testerswe@outlook.com",
                        Password = "projekatSWE123"
                    }
                };

                MailAddress fromMail = new MailAddress("testerswe@outlook.com", user.Name + " - " + user.Email);
                MailAddress toMail = new MailAddress("quiz.m2s@outlook.com", "Quiz'M2S");
                MailMessage mesg = new MailMessage()
                {
                    From = fromMail,
                    Subject = "Mejl korisnika",
                    Body = message
                };

                mesg.To.Add(toMail);
                await Client.SendMailAsync(mesg);

                return Ok("Uspesno poslat mejl");
            }
            catch (Exception)
            {
                Console.WriteLine(email);
                if (String.IsNullOrEmpty(email) || String.IsNullOrWhiteSpace(email) || String.Compare(email,"undefined")==0)
                {
                    return BadRequest("Morate uneti svoju email adresu");
                }
                SmtpClient Client = new SmtpClient()
                {
                    Host = "smtp.outlook.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential()
                    {
                        UserName = "testerswe@outlook.com",
                        Password = "projekatSWE123"
                    }
                };

                MailAddress fromMail = new MailAddress("testerswe@outlook.com", "Guest - " + email);
                MailAddress toMail = new MailAddress("quiz.m2s@outlook.com", "Quiz'M2S");
                MailMessage mesg = new MailMessage()
                {
                    From = fromMail,
                    Subject = "Mejl korisnika",
                    Body = message
                };

                mesg.To.Add(toMail);
                await Client.SendMailAsync(mesg);

                return Ok("Uspesno poslat mejl");
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetUsers")]
    public async Task<ActionResult> GetUsers()
    {
        try
        {
            var users = await Context.Users.Where(p => p.Role == "User").ToListAsync();

            if (users == null)
                return BadRequest("Greska kod dobavljanja korisnika");

            if (users.Count == 0)
                return Ok("Nema korsinika za prikaz");

            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "User,Admin,QuizMaker,Guest")]
    [HttpGet("GetAllUsers")]
    public async Task<ActionResult> GetAllUsers()
    {
        try
        {
            var users = await Context.Users.Where(u => u.AccountVerified == true).ToListAsync();

            if (users == null)
                return BadRequest("Greska kod dobavljanja korisnika");

            if (users.Count == 0)
                return Ok("Nema korsinika za prikaz");

            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "User,QuizMaker,Admin")]
    [HttpGet("GetCurrentUserData")]
    public async Task<ActionResult> GetCurrentUserData()
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
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "User,QuizMaker, Admin")]
    [HttpPut("ChangeBioOfUser/{bio}")]
    public async Task<ActionResult> ChangeBio(string bio)
    {
        try
        {
            if (String.IsNullOrEmpty(bio))
                return BadRequest("Ne moze bio da bude prazan");

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Greska pri pribavljanju trenutnog korisnika");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

            var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

            user.Bio = bio;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno promenjen bio");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User, QuizMaker,Admin")]
    [HttpPut("ChangeNameOfUser/{new_Name}")]
    public async Task<ActionResult> ChangeNameOfUser(string new_Name)
    {
        try
        {
            if (String.IsNullOrEmpty(new_Name))
                return BadRequest("Ne mozes da nemas ime");

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Greska pri pribavljanju trenutnog korisnika");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

            var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisnku");

            var users = await Context.Users.Where(p => p.Role == "User" || p.Role == "Admin" || p.Role == "QuizMaker").ToListAsync();

            foreach (var u in users)
            {
                if (u.Name.CompareTo(new_Name) == 0)
                    return BadRequest("To ime je vec zauzeto odaberite neko drugo");
            }

            user.Name = new_Name;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno promenjeno ime");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin,QuizMaker,User")]
    [HttpGet("GetProfilePicutre")]
    public async Task<ActionResult> GetProfilePicutre()
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
                return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisnku");

            return Ok(user.ProfilePicutre);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin, QuizMaker, User")]
    [HttpPut("ChangeProfilePhoto")]
    public async Task<ActionResult> ChangeProfilePhoto([FromBody] string url)
    {
        try
        {
            Console.WriteLine(url);
            if (String.IsNullOrEmpty(url))
                return BadRequest("Morate da unesete url za sliku na profilu");

            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity == null)
            {
                return BadRequest("Greska pri pribavljanju trenutnog korisnika");
            }
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

            var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom prijavljenom korisnku");

            user.ProfilePicutre = url;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno postavljena slika profila");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "User, Admin, QuizMaker")]
    [HttpDelete("DeleteAccount")]
    public async Task<ActionResult> DeleteAccount()
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

            var user = await Context.Users.Include(p => p.Scores).Include(p => p.QuizzesMade).Include(p => p.QuizzesDone).Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            accountDeletedNotification(user);
            return Ok(null);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async void accountDeletedNotification(Users user)
    {
        String message = $"Postovani/a {user.Name} \n\nVas profil je uspesno obrisan.\n" + "\n\nS' postovanjem\nkviz Quiz'M2S";

        SmtpClient Client = new SmtpClient()
        {
            Host = "smtp.outlook.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential()
            {
                UserName = "quiz.m2s@outlook.com",
                Password = "tester123"
            }
        };

        MailAddress fromMail = new MailAddress("quiz.m2s@outlook.com", "Quiz'M2S");
        MailAddress toMail = new MailAddress(user.Email, user.Name);
        MailMessage msg = new MailMessage()
        {
            From = fromMail,
            Subject = "Brisanje profila",
            Body = message
        };

        msg.To.Add(toMail);
        await Client.SendMailAsync(msg);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("DeleteAccountByAdmin/{id}")]
    public async Task<ActionResult> DeletedAccountByAdmin(int id)
    {
        try
        {
            var user = await Context.Users.Include(p => p.Scores).Include(p => p.QuizzesMade).Include(p => p.QuizzesDone).Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Pogresan id korinika, korinik nije pronadjen");

            Context.Users.Remove(user);
            await Context.SaveChangesAsync();
            AdminDeletedAccountNotificationAsync(user);
            return Ok(null);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task AdminDeletedAccountNotificationAsync(Users user)
    {
        String message = $"Postovani/a {user.Name} \n\nVas profil je obrisan od strane admina.\n" + "\n\nS' postovanjem\nkviz Quiz'M2S";

        SmtpClient Client = new SmtpClient()
        {
            Host = "smtp.outlook.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential()
            {
                UserName = "quiz.m2s@outlook.com",
                Password = "tester123"
            }
        };

        MailAddress fromMail = new MailAddress("quiz.m2s@outlook.com", "Quiz'M2S");
        MailAddress toMail = new MailAddress(user.Email, user.Name);
        MailMessage msg = new MailMessage()
        {
            From = fromMail,
            Subject = "Brisanje profila",
            Body = message
        };

        msg.To.Add(toMail);
        await Client.SendMailAsync(msg);
    }

    [Authorize(Roles = "User, QuizMaker,Admin")]
    [HttpPut("WantQuizMaker")]

    public async Task<ActionResult> WantQuizMaker()
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

            var user = await Context.Users.Include(p => p.Scores).Include(p => p.QuizzesMade).Include(p => p.QuizzesDone).Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");


            user.wantQuizMaker = true;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno poslat zahtev");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [Authorize(Roles = "User, QuizMaker,Admin")]
    [HttpPut("WantAdmin")]

    public async Task<ActionResult> WantAdmin()
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

            var user = await Context.Users.Include(p => p.Scores).Include(p => p.QuizzesMade).Include(p => p.QuizzesDone).Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska pri pribavljanju podataka o trenutnom user-u");


            user.wantAdmin = true;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno poslat zahtev");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "Admin,QuizMaker,User")]
    [HttpGet("GetDataForUser/{userName}")]

    public async Task<ActionResult> GetDataForUser(string userName)
    {
        try
        {
            var user = await Context.Users.Where(p => p.Name == userName).Include(p => p.Scores).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Nema tog korisnika");

            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}