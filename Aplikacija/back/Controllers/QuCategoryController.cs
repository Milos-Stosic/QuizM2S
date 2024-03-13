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
public class QuCategoryController : ControllerBase
{
    public Context? Context;

    public QuCategoryController(Context context)
    {
        Context = context;
    }

    [HttpGet("GetQuestionCategories")]
    public async Task<ActionResult> GetQuestionCategories(){
        try{
            var categories=await Context.QuestionCategorys.Include(p=>p.Questions).ThenInclude(p=>p.Answers).ToListAsync();

            if(categories==null)
                return BadRequest("Greska sa pribavljanjem kategorija");
            
            if(categories.Count==0)
                return Ok("Nema kategorija");

            return Ok(categories);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }

    [HttpGet("GetQuestionCategory/{name}")]
    public async Task<ActionResult> GetQuestionCategory(string name){
        try{
            if(String.IsNullOrEmpty(name))
                return BadRequest("Greska sa imenom kategorije");

            var category=await Context.QuestionCategorys.Where(p=>p.Name==name).Include(p=>p.Questions).ThenInclude(p=>p.Answers).FirstOrDefaultAsync();

            if(category==null)
                return BadRequest("Nema te kategorije");
            
            return Ok(category);
        }
        catch(Exception e){
            return BadRequest(e.Message);
        }
    }

    // [Authorize(Roles ="QuizMaker")]
    // [HttpPost("CreateCategory/{name}")]
    // public async Task<ActionResult> CreateCategory(string name){
    //     try{
    //         if(String.IsNullOrEmpty(name))
    //             return BadRequest("Moras da uneses ime kategorije");
            
    //         var cat=await Context.QuestionCategorys.ToListAsync();

    //         foreach(var p in cat)
    //         {
    //             if(p.Name.CompareTo(name)==0)
    //                 return Ok("Ta kategorija vec postoji");
    //         }

    //         QuCategory category=new QuCategory();
    //         category.Name=name;
    //         category.Questions=new List<Question>();

    //         await Context.QuestionCategorys.AddAsync(category);
    //         await Context.SaveChangesAsync();

    //         return Ok("Uspesno dodata kategorija");
    //     }
    //     catch(Exception e)
    //     {
    //         return BadRequest(e.Message);
    //     }
    // }
}