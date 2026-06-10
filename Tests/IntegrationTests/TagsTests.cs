using System.Net;
using System.Net.Http.Json;
using Application.DataTransferObjects.TagsDto;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public class TagsTests : IClassFixture<TasksServiceApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JwtTokenGenerator _tokenGenerator;
    
    public TagsTests(TasksServiceApplicationFactory factory)
    {
        var jwtSettings = factory.Services.GetRequiredService<IConfiguration>().GetSection("JwtSettings");
        var secretKey = jwtSettings["validIssuer"];
        var issuer = jwtSettings["ValidIssuer"];
        var audience = jwtSettings["ValidAudience"];
        
        _tokenGenerator = new JwtTokenGenerator(secretKey, issuer, audience);

        _client = factory.CreateClient();
        
        var token = _tokenGenerator.GenerateToken(Guid.NewGuid().ToString());
        
        _client.DefaultRequestHeaders.Authorization 
            = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }
    
    [Fact]
    public async Task CreateTag_ShouldReturnOK_WhenTagIsValid()
    {
        // Arrange
        var tagDto = new TagDto
        {
            TagName = "NewTag"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/tags/addTag", tagDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateTag_ShouldReturnConflict_WhenTagAlreadyExists()
    {
        // Arrange
        var tagDto = new TagDto
        {
            TagName = "ExistingTag" 
        };

        // Act
        await _client.PostAsJsonAsync("/api/tags/addTag", tagDto);
        var response = await _client.PostAsJsonAsync("/api/tags/addTag", tagDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    
    [Fact]
    public async Task DeleteTag_ShouldReturnNoContent_WhenTagExists()
    {
        // Arrange
        var tagDto = new TagDto
        {
            TagName = "ExistingTag" 
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/tags/addTag", tagDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var createdTagId = await createResponse.Content.ReadAsStringAsync();
        
        var response = await _client.DeleteAsync($"/api/tags/deleteTag/{createdTagId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var tagId = Guid.NewGuid(); 

        // Act
        var response = await _client.DeleteAsync($"/api/tags/deleteTag/{tagId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task UpdateTag_ShouldReturnNoContent_WhenTagIsUpdated()
    {
        // Arrange
        var tagId = Guid.NewGuid(); 
        var updatedTagDto = new TagDto
        {
            TagName = "UpdatedTagName"
        };

        var tagDto = new TagDto
        {
            TagName = "ExistingTag" 
        };

        // Act
        var createResponse = await _client.PostAsJsonAsync("/api/tags/addTag", tagDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var createdTagId = await createResponse.Content.ReadAsStringAsync();
        
        var response = await _client.PutAsJsonAsync($"/api/tags/updateTag/{createdTagId}", updatedTagDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnNotFound_WhenTagDoesNotExist()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var updatedTagDto = new TagDto
        {
            TagName = "NonExistentTag"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tags/updateTag/{tagId}", updatedTagDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTag_ShouldReturnNotFound_WhenValidationFails()
    {
        // Arrange
        var tagId = Guid.NewGuid(); 
        var updatedTagDto = new TagDto
        {
            TagName = ""
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/tags/updateTag/{tagId}", updatedTagDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}