﻿using NBomber.Contracts;
using NBomber.CSharp;
using System.Text;
using System.Text.Json;


namespace NBomberTest
{
    //接口压测工具
    class Program
    {
        static void Main(string[] args)
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("fetch_login", async context =>
            {
                var jsonData = new
                {
                    Account = "system",
                    Password = "MDHAtW1/S4XoV1QucqvZt/33pT1uK0agu0ZH0BfGNVpo",
                    LoginPlatform = 1,
                    CaptchaCode = "vyck",
                    CaptchaId = "91b222edfba64fe1b3d7683c7bc3afa4"
                };

                var requestContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:8000/Dev/auth/login", requestContent);

                return response.IsSuccessStatusCode
                    ? Response.Ok()
                    : Response.Fail();
            });

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();


            // var jsonData = new { CaptchaCode = "vyck", CaptchaId = "91b222edfba64fe1b3d7683c7bc3afa4" };

            // var requestContent = new StringContent(JsonSerializer.Serialize(jsonData), Encoding.UTF8, "application/json");

            // var step = Step.Create("fetch_loginapi",
            //clientFactory: HttpClientFactory.Create(),
            //execute: context =>
            //{

            //    var request = Http.CreateRequest("Post", "http://localhost:8000/Dev/auth/login")
            //        .WithBody(requestContent);

            //    return Http.Send(request, context);
            //});

            // var scenario = ScenarioBuilder
            //     .CreateScenario("loginapi", step)
            //     .WithWarmUpDuration(TimeSpan.FromSeconds(5))
            //     .WithLoadSimulations(
            //         Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
            //     );

            // // creates ping plugin that brings additional reporting data
            // var pingPluginConfig = PingPluginConfig.CreateDefault(new[] { "127.0.0.1" });
            // var pingPlugin = new PingPlugin(pingPluginConfig);

            // NBomberRunner
            //     .RegisterScenarios(scenario)
            //     .WithWorkerPlugins(pingPlugin)
            //     .Run();

        }
    }
}