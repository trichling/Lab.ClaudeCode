var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin();

var quizdb = postgres.AddDatabase("quizdb");

// Add API service
var api = builder.AddProject<Projects.LogicQuiz_Api>("api")
    .WithReference(quizdb)
    .WaitFor(quizdb);

// Add Frontend (npm dev server)
var frontend = builder.AddNpmApp("frontend", "../LogicQuiz.Web", "dev")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints();

builder.Build().Run();
