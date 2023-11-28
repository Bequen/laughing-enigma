using System.Reflection;
using GraphQL.AspNet.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using stag.Controllers;
using stag.Database;
using stag.Database.Models;
using Stag.Auth;
using Stag;
using Stag.Controllers;
using Stag.Database.Models;
using Model.Request;

Console.WriteLine("Starting up");

for (int i = 0; i < args.Length; i++) {
    Console.WriteLine(args[i]);
    if(args[i].Equals("-c")) {
        if(i + 1 < args.Count()) {
            Console.WriteLine($"Config path: {args[i + 1]}");
            Config.ConfigPath = args[i + 1];
        } else {
            throw new Exception("-c must be followed by a filename");
        }
    }
}

Config config = Config.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGraphQL();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Stag Api",
        Description = "Simple ASP.NET project to manage university.",
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddDbContext<AuthContext>();
builder.Services.AddDbContext<StagContext>();
builder.Services.AddScoped<TokenService, TokenService>();
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<AuthContext>();

if(String.IsNullOrEmpty(config.Secret)) {
    throw new Exception("Secret cannot be empty string. Go to Stag config file and set it.");
}

// from: https://medium.com/geekculture/how-to-add-jwt-authentication-to-an-asp-net-core-api-84e469e9f019
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "apiWithAuthBackend",
            ValidAudience = "apiWithAuthBackend",
            IssuerSigningKey = config.GetSecurityKey()
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler, TimetableOwnerHandler>();
builder.Services.AddTransient<IAuthorizationHandler, DepartmentAuthorizationHandler>();
builder.Services.AddTransient<IAuthorizationHandler, SubjectAuthorizationHandler>();


builder.Services.AddAuthorization(options => {
    options.AddPolicy("IsLecturer", policy => policy.AddRequirements(new SubjectRelationPermission(RelationType.Lecturer)));
    options.AddPolicy("IsEnrolled", policy => policy.AddRequirements(new SubjectRelationPermission(RelationType.Enrolled)));
    options.AddPolicy("IsGarant", policy => policy.AddRequirements(new SubjectRelationPermission(RelationType.Garant)));
    options.AddPolicy("IsPracticioner", policy => policy.AddRequirements(new SubjectRelationPermission(RelationType.Practicioner)));
    options.AddPolicy("IsTimetableEventOwner", policy => policy.AddRequirements(new TimetableOwnerRequirement()));
    options.AddPolicy("CreateDepartmentSubjectPermission", policy => policy.AddRequirements(new DepartmentSubjectCreatePermission()));
    options.AddPolicy("SetSubjectGarantPermission", policy => policy.AddRequirements(new SubjectSetGarantPermission()));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if(config.Initialize && false) {
    try {
        Console.WriteLine("Initializing");
        var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            if(userManager is null) {
                throw new Exception("Cannot find user manager service, make sure Asp Net Identity is turned on");
            }

            var identityUser = new IdentityUser("admin") {
                Email = "admin@admin"
            };

            await userManager.CreateAsync(identityUser, "admin123");

            var userId = userManager.Users.First(x => x.Email.Equals("admin@admin")).Id;
            Console.WriteLine($"UserId: {userId}");

            using(var context = new StagContext()) {
                context.Persons.Add(new Person() {
                    PersonId = userId,
                    FirstName = "Admin",
                    LastName = "Admin"
                });

                context.Semesters.Add(new Semester() {
                    StartsAt = new DateTime(2023, 2, 13).ToUniversalTime(),
                    EndsAt = new DateTime(2023, 5, 12).ToUniversalTime(),
                    Season = Season.Summer
                });

                var department = context.Departments.Add(new Department() {
                    LongName = "Katedra Informatiky",
                    ShortName = "inf"
                });

                await context.SaveChangesAsync();
                context.DepartmentRelations.Add(new DepartmentRelation() {
                    DepartmentId = department.Entity.DepartmentId,
                    UserId = userId
                });

                var departmentService = new DepartmentService(context);
                var algebra = await departmentService.CreateSubject(department.Entity.DepartmentId, new Model.Request.SubjectPutRequest() {
                    Name = "Algebra 2",
                    ShortName = "AL2",
                    Description = "Some description of algebra",
                    GarantUserId = userId
                });

                var jcs = await departmentService.CreateSubject(department.Entity.DepartmentId, new Model.Request.SubjectPutRequest() {
                    Name = "JCS 2",
                    ShortName = "JCS2",
                    Description = "Some description of C#",
                    GarantUserId = userId
                });

                await context.SaveChangesAsync();

                var subjectService = new SubjectService(context);

                await subjectService.AddLecturer(jcs.SubjectId, userId);
                await subjectService.AddPracticioner(algebra.SubjectId, userId);

                var lectureEvent = await subjectService.CreateLectureTimetable(userId, jcs.SubjectId);
                await context.SaveChangesAsync();

                await subjectService.SetTimes(jcs.SubjectId, lectureEvent.TimetableEventId, new List<SubjectSetTimeRequest>() {
                    new SubjectSetTimeRequest() {
                        StartsAt = new DateTime(2023, 2, 14, 18, 30, 0),
                        EndsAt = new DateTime(2023, 2, 14, 20, 0, 0),
                        Frequence = 1
                    }
                });

                var practiseEvent = await subjectService.CreatePracticeTimetable(userId, algebra.SubjectId);
                await context.SaveChangesAsync();
                await subjectService.SetTimes(algebra.SubjectId, practiseEvent.TimetableEventId, new List<SubjectSetTimeRequest>() {
                    new SubjectSetTimeRequest() {
                        StartsAt = new DateTime(2023, 2, 14, 8, 0, 0),
                        EndsAt = new DateTime(2023, 2, 14, 9, 30, 0),
                        Frequence = 1
                    }
                });

                await context.SaveChangesAsync();
            }

        }
        config.Initialize = false;
        config.Save();
    } catch {
        Console.WriteLine("Failed to initialize");
        throw;
    }
}

config.Save();

Console.WriteLine("Running");

app.UseGraphQL();
app.Run();