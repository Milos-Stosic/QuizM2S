using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using back.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private Context? Context;
    private IConfiguration configuration;
    public LoginController(Context context, IConfiguration config)
    {
        Context = context;
        configuration = config;
    }

    [AllowAnonymous]
    [HttpGet("LoginUser/{Email}/{Password}")]
    public async Task<ActionResult> LoginUser(string Email, string Password)
    {
        try
        {
            var user = await Context.Users.Where(p => p.Email == Email).FirstOrDefaultAsync();

            if (user == null)
                return BadRequest("Greska sa email-om");

            if (user.AccountVerified == false)
                return BadRequest("Morate verifikovati profil da bi ste se ulogovali. Proverite vas email kako biste pronasli link za verifikaciju");

            if (VerifyPassword(Password, user.Password, user.Salt))
                return Ok(user);
            else
                return BadRequest("Pogresna lozinka");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("SignUp/{Name}/{Email}/{Password}/{ConformPassword}")]
    public async Task<ActionResult> SignUpUser(string Name, string Email, string Password, string ConformPassword)
    {
        try
        {
            if (String.IsNullOrWhiteSpace(Name) && Name.Length > 20)
                return BadRequest("Moras da uneses korisnicko ime sa manje od 20 karaktera");
            if (String.IsNullOrWhiteSpace(Email))
                return BadRequest("Moras da uneses email");
            if (String.IsNullOrWhiteSpace(Password))
                return BadRequest("Moras da uneses lozinku");
            if (String.IsNullOrWhiteSpace(ConformPassword))
                return BadRequest("Moras da potvrdis lozinku");
            if (ConformPassword != Password)
                return BadRequest("Lozinke se ne poklapaju");

            foreach (var p in Context.Users.ToList())
            {
                if (p.Name.CompareTo(Name) == 0)
                    return BadRequest("To ime je vec zauzeto, odaberite drugo");
                if (p.Email.CompareTo(Email) == 0)
                    return BadRequest("Nalog sa ovim email-om vec postoji");
            }

            Users users = new Users();
            users.Email = Email;
            users.Name = Name;
            users.Role = "User";
            users.QuizzesMade = new List<Quiz>();
            users.NumberOfQuizzesDone = 0;
            users.Scores = new List<Score>();
            users.wantAdmin = false;
            users.wantQuizMaker = false;
            users.AccountVerified = false;
            users.QuizzesDone = new List<QuizUser>();
            Random rnd = new Random();
            users.Conf_num = rnd.Next(1, 1000000); ;

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(Password, out passwordHash, out passwordSalt);
            users.Password = passwordHash;
            users.Salt = passwordSalt;

            await Context.Users.AddAsync(users);
            await Context.SaveChangesAsync();

            var verify = await Context.Users.Where(p => p.Email == Email).FirstOrDefaultAsync();
            try
            {
                Verification(verify);
            }
            catch (Exception)
            {
                return BadRequest("Nije validna email adresa");
            }

            return Ok(null);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("ForgotPasswordEmail/{email}")]
    public async Task<ActionResult> ForgotPasswordEmail(string email){
        try{

            var user=await Context.Users.Where(u=>u.Email==email).FirstOrDefaultAsync();

            if(user==null){
                return BadRequest("Pogresan email");
            }

            String message;
        message = $"Postovani/a {email}\n\nPokusavate da izmenite loiznku na sajtu Quiz'M2S, molimo vas da potvrdite identitet klikom na link koji ce vas odvesti do forme za promenu loznike.\n" +
        "http://localhost:3000/Login/ForgotPasswordForm/"+"\n\n S postovanjem, \n kviz Quiz'M2S";

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
        MailAddress toMail = new MailAddress(email, email);
        MailMessage mesg = new MailMessage()
        {
            From = fromMail,
            Subject = "Email za promenu lozinke",
            Body = message
        };

        mesg.To.Add(toMail);
        await Client.SendMailAsync(mesg);

        return Ok("Uspesno poslat mejl");
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }

    [HttpGet("ChangePasswordEmail/{email}/{new_password}/{conf_pass}")]
    public async Task<ActionResult> ForgotPassword(string email, string new_password, string conf_pass)
    {
        try
        {
            var user = await Context.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                return BadRequest("Nema korisnika sa tom email adresom");
            }

            if (new_password.CompareTo(conf_pass) != 0)
            {
                return BadRequest("Stara sifra i nova sifra nisu iste");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(new_password, out passwordHash, out passwordSalt);
            user.Password = passwordHash;
            user.Salt = passwordSalt;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno promenjena lozinka");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "Admin, QuizMaker, User")]
    [HttpPut("ChangePassword/{old_pass}/{new_pass}/{conf_pass}")]
    public async Task<ActionResult> ChangePassword(string old_pass, string new_pass, string conf_pass)
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

            if (!VerifyPassword(old_pass, user.Password, user.Salt))
            {
                return BadRequest("Uneli ste pogresnu staru lozinku");
            }

            if (new_pass.CompareTo(conf_pass) != 0)
            {
                return BadRequest("Ne poklapaju se lozinke");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(new_pass, out passwordHash, out passwordSalt);
            user.Password = passwordHash;
            user.Salt = passwordSalt;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspeno promenjena lozinka");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }



    private static async void Verification(Users user)
    {
        String message;
        message = $"Postovani/a {user.Name}\n\nKreiran je nalog sa vasim email-om na nasem sajtu kviza Quiz'M2S, molimo vas da potvrdite identitet klikom na link.\n" +
        "http://localhost:5013/Login/VerificationOfSignUp/" + user.ID + "/" + user.Conf_num + "\n\n S postovanjem, \n kviz Quiz'M2S";

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
        MailMessage mesg = new MailMessage()
        {
            From = fromMail,
            Subject = "Verifikacioni email",
            Body = message
        };

        mesg.To.Add(toMail);
        await Client.SendMailAsync(mesg);
    }


    [HttpGet("VerificationOfSignUp/{id}/{conf}")]
    public async Task<ActionResult> VerificationOfSignUp(int id, int conf)
    {
        try
        {
            var user = await Context.Users.Where(p => p.ID == id).FirstOrDefaultAsync();

            if (user.Conf_num != conf)
                return BadRequest("Los konfirmacioni broj");

            user.AccountVerified = true;

            Context.Users.Update(user);
            await Context.SaveChangesAsync();

            return Ok("Uspesno ste verifikovali vas nalog, sada se mozete ulogovati");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("UsersEndpoint")]
    public IActionResult UsersEndpoint()
    {

        var currentUser = GetCurrentUser();

        return Ok(currentUser);
    }


    [HttpPost("GetToken")]
    public async Task<ActionResult> GetToken([FromBody] UserAuth user)
    {
        try
        {
            var u = await Context.Users.Where(p => p.Email == user.Email).FirstOrDefaultAsync();
            if (u == null)
                return BadRequest(null);

            if (VerifyPassword(user.Password, u.Password, u.Salt))
            {
                var token = Generate(u);
                return Ok(new { token = token });
            }
            else
            {
                return Ok(null);
            }
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private object Generate(Users u)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]{
            new Claim(ClaimTypes.NameIdentifier,u.Name),
            new Claim(ClaimTypes.Email,u.Email),
            new Claim(ClaimTypes.Role,u.Role),
            new Claim(ClaimTypes.Sid,u.ID.ToString())
        };

        var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private object GetCurrentUser()
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            var userClaims = identity.Claims;
            int id = int.Parse(userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Sid)!.Value);

            return new Users
            {
                Name = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)!.Value,
                Email = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Email)!.Value,
                Role = userClaims.FirstOrDefault(p => p.Type == ClaimTypes.Role)!.Value,
                ID = id
            };
        }
        return null;
    }

    [Authorize]
    [HttpGet("IsItAdmin/{id}")]
    public async Task<ActionResult> IsItAdmin(int id)
    {
        try
        {
            if (id < 0)
                return BadRequest("Greska sa id-jem");

            var admin = await Context.Users.Where(p => p.Role == "Admin" && p.ID == id).FirstOrDefaultAsync();

            if (admin == null)
                return Ok(false);

            return Ok(true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("IsItQuizMaker/{id}")]
    public async Task<ActionResult> IsItQuizMaker(int id)
    {
        try
        {
            if (id < 0)
                return BadRequest("Greska sa id-jem");

            var qm = await Context.Users.Where(p => p.Role == "QuizMaker" && p.ID == id).FirstOrDefaultAsync();

            if (qm == null)
                return Ok(false);

            return Ok(true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize]
    [HttpGet("IsItUser/{id}")]
    public async Task<ActionResult> IsItUser(int id)
    {
        try
        {
            if (id < 0)
                return BadRequest("Greska sa id-jem");

            var user = await Context.Users.Where(p => p.Role == "User" && p.ID == id).FirstOrDefaultAsync();

            if (user == null)
                return Ok(false);

            return Ok(true);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (String.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or be white space", "password");
        if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash", "passwordHash");
        if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt", "passwordHash");

        using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
        {
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != storedHash[i]) return false;
        }

        return true;
    }

    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        if (password == null) throw new ArgumentNullException("password");
        if (String.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or white space");

        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}