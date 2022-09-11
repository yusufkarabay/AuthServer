using AuthServer.Core;
using AuthServer.Core.Configration;
using AuthServer.Core.Entities;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Repositories;
using AuthServer.Repositories.Repositories;
using AuthServer.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

//**********************************************************************************************************
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<Client>>(builder.Configuration.GetSection("Clients"));
//**********************************************************************************************************



//!!!!!!!!!!!!!! bu kýsmý autofact ile dýþ dünyaya kapat, buna çalýþ

//Tek bir istekte 1 nesne örneði oluþturur. Ayný istekte birden fazla interfacle karþýlaþýrsa  ayný nesne örneðini kullanýr
//Eðer addscope yerine  AddTransient kullansaydýk her interface ile karþýlþamasýnda yeni bir nesne örneði oluþtururdu
//AddSingleton kulansaydýk uygulama boyunca tek bir nesne örneði ile çalýþýrdý

builder.Services.AddScoped<IAuthServerAuthenticationService, AuthServerAuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


builder.Services.AddScoped<IUserRefreshTokenRepoistory, UserRefreshTokenRepository>();

//genericservice kulnnamayacaðým. uygulama sonunda kaldýracaðým  
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));


//bu kýsýmda þifrenin tipini belirleyebilirsin. rakam olsun. büyük küçük olsun gibi
//bu kýsmýn detayý identiy kursunda var!!!!
builder.Services.AddIdentity<UserApp, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail=true;
    options.Password.RequireUppercase=true;
    options.Password.RequiredLength=8;
    options.Password.RequireLowercase=true;
}).AddEntityFrameworkStores<AuthServerDbContext>().AddDefaultTokenProviders();






builder.Services.AddDbContext<AuthServerDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(Assembly.GetAssembly(typeof(AuthServerDbContext)).GetName().Name);

        // sqlOptions.MigrationsAssembly("AuthServer.Repositories");
    });
});






//kimlik doðrulama iþlemi
//
builder.Services.AddAuthentication(options =>
{

    //üyelik sisteminde 2 ayrý üyelik tipi olabilir
    //örnek bayiler ayrý . kullanýcý olarak ayrý þekilde    
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;

    //alttaki þemayla iletiþim kurmasý için bu methodu ekledik
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).
//json web token ile Authentication ile ayný þemayý seçmeliyiz ki birbirleriyle iletiþim halinde olsunlar
AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

    //tokenýn parametreleri appjsonsettingsten gelenleri eþleþtiriyoruz
    opts.TokenValidationParameters=new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer=tokenOptions.Issuer,

        //burada o. indeksin sebebi. appjsonsettingste auidnece bir dizin. Fakat ilki tokenn'ý daðýtan api
        //daha sonrakilerde ulaþabilecekleri apilerin adresleri
        ValidAudience=tokenOptions.Audience[0],

        //SignService'den ürettiðimiz anahtarý eþledik
        IssuerSigningKey=SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidateIssuerSigningKey=true,
        ValidateLifetime=true,
        ValidateAudience=true,

        //biz token'a süre verdiðimizde üzerine defalut olarak 5 dk ekler.
        //fakat biz burda o default kýsmý kaldýrdýk
        //bu farký apileri farklý zaman aralýklarýnda ki serverlara kurma ihitimalimizden dolayý veriyor

        ClockSkew=TimeSpan.Zero,


    };
});

var app = builder.Build();











// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthServer.WebAPI");
        c.RoutePrefix = "";
    });
}



app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
