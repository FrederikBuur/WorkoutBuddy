using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net.Http.Json;
using WorkoutBuddy.Features;

namespace WorkoutBuddy.Tests.IntegrationTests;

public class ExerciseDetailsControllerTests //: IClassFixture<WorkoutBuddyWebApplicationFactory<Program>>
{

    //private readonly HttpClient _client;
    //private readonly WorkoutBuddyWebApplicationFactory<Program> _factory;

    //public ExerciseDetailsControllerTests(
    //    WorkoutBuddyWebApplicationFactory<Program> factory)
    //{
    //    _client = factory.CreateClient();
    //    _factory = factory;
    //}

    [Fact]
    public async Task TestTest()
    {
        // var webAppFactory = new WebApplicationFactory<Program>();
        // var client = webAppFactory.CreateDefaultClient();

        // var response = await client.GetAsync("");
        // var stringResult = await response.Content.ReadAsStringAsync();

        // Assert.Equal("", stringResult);
    }

    [Fact]
    public async Task Test1()
    {
        // // Arrange
        // var webAppFactory = new WebApplicationFactory<Program>();
        // var client = webAppFactory.CreateDefaultClient();
        // var userGuid = Guid.NewGuid();
        // var createExerciseDetailRequest = new CreateExerciseDetailRequest(
        //     userGuid, userGuid, "testName", "testDescription", "testImageUrl", true, "Biceps");

        // // Act
        // var response = await client.PostAsJsonAsync("api/exercise-details", createExerciseDetailRequest);

        // // Assert
        // if(!response.IsSuccessStatusCode)
        // {
        //     var errorMsg = await response.Content.ReadAsStringAsync();
        //     throw new HttpRequestException(errorMsg);
        // }

        // var createdExerciseDetail = await response.Content.ReadFromJsonAsync<WorkoutDetailResponse>();
        // Assert.True(createdExerciseDetail is not null);
        // Assert.True(createdExerciseDetail?.Id != default);
        // Assert.True(createdExerciseDetail?.Name == createExerciseDetailRequest.Name);
    }
}
