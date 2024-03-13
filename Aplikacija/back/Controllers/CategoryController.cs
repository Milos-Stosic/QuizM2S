using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace back.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    public Context? Context;

    public CategoryController(Context context)
    {
        Context = context;
    }

    [HttpGet("GetCategories")]
    public async Task<ActionResult> GetCategories()
    {
        try
        {
            var categories = await Context.Categorys.Include(p => p.Quizzes).ToListAsync();

            if (categories == null)
                return BadRequest("Greska sa pribavljanjem kategorija");

            var list = categories.Select(p => p.Quizzes).ToList();
            int num = list.Count() - 1;
            if (categories.Count == 0)
                return Ok("Nema kategorija za prikaz");

            return Ok(categories.Select(p => new
            {
                Name = p.Name,
                NumberOfQuizzes = num
            }).ToList());
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "QuizMaker")]
    [HttpPost("CreateCategory/{name}")]
    public async Task<ActionResult> CreateCategory(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Moras da uneses ime kategorije");

            var cat = await Context.Categorys.ToListAsync();

            foreach (var p in cat)
            {
                if (p.Name.CompareTo(name) == 0)
                    return Ok("Ta kategorija vec postoji");
            }

            Category category = new Category();
            category.Name = name;
            category.Quizzes = new List<Quiz>();

            await Context.Categorys.AddAsync(category);

            QuCategory quCategory = new QuCategory();
            quCategory.Name = name;
            quCategory.Questions = new List<Question>();

            await Context.QuestionCategorys.AddAsync(quCategory);

            await Context.SaveChangesAsync();

            return Ok("Uspesno dodata kategorija");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetCategory/{name}")]
    public async Task<ActionResult> GetCategory(string name)
    {
        try
        {
            if (String.IsNullOrEmpty(name))
                return BadRequest("Greska sa imenom kategorije");

            var category = await Context.Categorys.Where(p => p.Name == name).FirstOrDefaultAsync();

            if (category == null)
                return BadRequest("Nema te kategorije");

            return Ok(category);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}