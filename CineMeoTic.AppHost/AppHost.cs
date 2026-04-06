using Scalar.Aspire;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedis("cache");

//var rabitMq = builder.AddRabbitMQ("rabbitMq")
//    .WithManagementPlugin();

////khởi tạo container postgres map với port 5432
//var postgres = builder.AddPostgres("postgres")
//                      .WithHostPort(5432)
//                      .WithDataVolume()
//                      .WithLifetime(ContainerLifetime.Persistent);

////thêm database user vào trong postgres container ... chỗ này có nhiều database thì thêm nhiều dòng y chang, thay tên chuỗi bên trong cho phù hợp
//var userDb = postgres.AddDatabase("user");


//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);


//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

//builder.AddProject<Projects.CineMeoTic_AuthenticationService>("authentication-service")
//       //.WithReference(userDb)
//       //.WaitFor(userDb)
//       .WithHttpHealthCheck("/health");



//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);


//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

var user_service = builder.AddProject<Projects.CineMeoTic_UserService_API>("cinemeotic-userservice-api");



//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);


//var apiService = builder.AddProject<Projects.CineMeoTic_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");

//builder.AddProject<Projects.CineMeoTic_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithHttpHealthCheck("/health")
//    .WithReference(cache)
//    .WaitFor(cache)
//    .WithReference(apiService)
//    .WaitFor(apiService);

var scalar = builder.AddScalarApiReference();

scalar
    .WithApiReference(user_service);

builder.AddProject<Projects.Cinemeotic_MovieService_API>("cinemeotic-movieservice-api");

builder.Build().Run();
