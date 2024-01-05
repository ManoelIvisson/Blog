using Blog.Data;
using Blog.Models;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ViewModels;

namespace Blog.Controllers {
    [ApiController]
    public class CategoryController : ControllerBase{
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context){
            try {
                var categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            } catch {
                return StatusCode(500, new ResultViewModel<List<Category>>("Erro interno no servidor"));
            }
            
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context){
            try {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null) {
                    return NotFound(new ResultViewModel<Category>("Categoria não encontrada"));
                }

                return Ok(new ResultViewModel<Category>(category));
            } catch {
                return StatusCode(500, new ResultViewModel<Category>("Erro interno no servidor"));
            }
            
        }

        [HttpPost("v1/categories/")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context){
            if (!ModelState.IsValid) {
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            }

            try {
                var category = new Category() {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            } catch (DbUpdateException ex) {
                return StatusCode(500, "Não foi possível adicionar a categoria");
            } catch (Exception ex) {
                return StatusCode(500, "Erro interno no servidor");
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context){
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            try {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null) {
                    return NotFound();
                }

                category.Name = model.Name;
                category.Slug = model.Slug;
                
                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(category);
            } catch (DbUpdateException ex) {
                return StatusCode(500, "Não foi possível alterar a categoria");
            } catch (Exception ex) {
                return StatusCode(500, "Erro interno no servidor");
            }
            
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id, [FromServices] BlogDataContext context){
            try {
               var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null) {
                    return NotFound();
                }
                
                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(category); 
            } catch (DbUpdateException ex) {
                return StatusCode(500, "Não foi possível alterar a categoria");
            } catch (Exception ex) {
                return StatusCode(500, "Erro interno no servidor");
            }

        }
    }
}