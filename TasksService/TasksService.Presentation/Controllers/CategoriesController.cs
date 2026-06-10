using Application.DataTransferObjects.CategoriesDto;
using Application.UseCases.Commands.CategoryCommands.CreateCategory;
using Application.UseCases.Commands.CategoryCommands.DeleteCategory;
using Application.UseCases.Commands.CategoryCommands.UpdateCategory;
using Application.UseCases.Queries.CategoryQueries.GetAllCategories;
using Application.UseCases.Queries.CategoryQueries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Presentation.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController(IMediator mediator) : Controller
{
    [HttpGet("getCategory/{categoryId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetCategoryById(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery { CategoryId = categoryId };
        var category = await mediator.Send(query, cancellationToken);
        return Ok(category);
    }

    [HttpGet("getAllCategories")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken)
    {
        var query = new GetAllCategoriesQuery();
        var categories = await mediator.Send(query, cancellationToken);
        return Ok(categories);
    }

    [HttpPost("addCategory")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> AddCategory(
        [FromBody] CategoryDto categoryDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand { CategoryName = categoryDto.CategoryName };
        var created = await mediator.Send(command, cancellationToken);
        return Ok(created);
    }

    [HttpPut("updateCategory/{categoryId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> UpdateCategory(
        [FromBody] CategoryDto categoryDto,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand
        {
            CategoryId = categoryId,
            CategoryName = categoryDto.CategoryName
        };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("deleteCategory/{categoryId}")]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> DeleteCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { CategoryId = categoryId };
        await mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
