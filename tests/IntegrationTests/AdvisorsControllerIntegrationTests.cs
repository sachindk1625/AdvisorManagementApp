using System.Net;
using System.Net.Http.Json;
using Application.Commands;
using Application.Queries;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
namespace IntegrationTests
{
    public class AdvisorsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly IFixture _fixture = new Fixture();
        private readonly HttpClient _client;
        public AdvisorsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }
        

        [Fact]
        public async Task ListAdvisors_ReturnsOk_WhenAdvisorsExist()
        {
            //Arrange

            await _client.PostAsJsonAsync("api/Advisors/CreateAdvisor",
                _fixture.Build<CreateAdvisorCommand.CreateAdvisorRequest>()
                    .With(x => x.SIN, "123456789")
                    .Create());


            //Act

            var response = await _client.GetAsync("api/Advisors/ListAdvisors");
            //Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType!.ToString());
        }

        [Fact]
        public async Task GetAdvisorById_ReturnsOk_WhenAdvisorExists()
        {
            // Arrange
            var advisorName = "Test";
            var postResponse = await _client.PostAsJsonAsync("api/Advisors/CreateAdvisor",
                _fixture.Build<CreateAdvisorCommand.CreateAdvisorRequest>()
                    .With(x => x.FullName, advisorName)
                    .With(x => x.SIN, "123456789")
                    .Create());


            // Act
            var response = await _client.GetAsync($"{postResponse.Headers.Location}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<GetAdvisor.Response>();
            Assert.NotNull(result);
            Assert.Equal(advisorName, result.FullName);
        }

        [Fact]
        public async Task GetAdvisorById_ReturnsNotFound_WhenAdvisorDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"api/Advisors/GetAdvisorById/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateAdvisor_ReturnsCreated()
        {
            // Act
            var response = await _client.PostAsJsonAsync("api/Advisors/CreateAdvisor",
                _fixture.Build<CreateAdvisorCommand.CreateAdvisorRequest>()
                    .With(x => x.SIN, "123456789")
                    .Create());

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task UpdateAdvisor_ReturnsAccepted_WhenAdvisorIsUpdated()
        {
            // Arrange
            var postResponse = await _client.PostAsJsonAsync("api/Advisors/CreateAdvisor",
                _fixture.Build<CreateAdvisorCommand.CreateAdvisorRequest>()
                    .With(x => x.SIN, "123456789")
                    .Create());

            var advisorId= postResponse.Headers.Location!.ToString().Split("/").Last();

            // Act
            var response = await _client.PutAsJsonAsync("api/Advisors/UpdateAdvisor", _fixture.Build<UpdateAdvisorCommand.UpdateAdvisorRequest>()
                .With(x=>x.AdvisorId,Guid.Parse(advisorId))
                .With(x=>x.SIN,"123456789")
                .Create());

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
        {
            // Arrange

            // Act
            var response = await _client.PutAsJsonAsync("api/Advisors/UpdateAdvisor", _fixture.Build<UpdateAdvisorCommand.UpdateAdvisorRequest>()
                .With(x => x.AdvisorId, Guid.NewGuid)
                .With(x => x.SIN, "123456789").Create());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAdvisor_ReturnsAccepted_WhenAdvisorIsDeleted()
        {
            // Arrange
            var postResponse = await _client.PostAsJsonAsync("api/Advisors/CreateAdvisor",
                _fixture.Build<CreateAdvisorCommand.CreateAdvisorRequest>()
                    .With(x => x.SIN, "123456789")
                    .Create());
            var advisorId = postResponse.Headers.Location!.ToString().Split("/").Last();

            // Act
            var response = await _client.DeleteAsync($"api/Advisors/DeleteAdvisor/{advisorId}");

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        }

        [Fact]
        public async Task DeleteAdvisor_ReturnsNotFound_WhenAdvisorDoesNotExist()
        {
            // Arrange
            var advisorId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"api/Advisors/DeleteAdvisor/{advisorId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}