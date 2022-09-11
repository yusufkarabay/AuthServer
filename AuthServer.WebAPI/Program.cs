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



//!!!!!!!!!!!!!! bu k�sm� autofact ile d�� d�nyaya kapat, buna �al��

//Tek bir istekte 1 nesne �rne�i olu�turur. Ayn� istekte birden fazla interfacle kar��la��rsa  ayn� nesne �rne�ini kullan�r
//E�er addscope yerine  AddTransient kullansayd�k her interface ile kar��l�amas�nda yeni bir nesne �rne�i olu�tururdu
//AddSingleton kulansayd�k uygulama boyunca tek bir nesne �rne�i ile �al���rd�

builder.Services.AddScoped<IAuthServerAuthenticationService, AuthServerAuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


builder.Services.AddScoped<IUserRefreshTokenRepoistory, UserRefreshTokenRepository>();

//genericservice kulnnamayaca��m. uygulama sonunda kald�raca��m  
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));


//bu k�s�mda �ifrenin tipini belirleyebilirsin. rakam olsun. b�y�k k���k olsun gibi
//bu k�sm�n detay� identiy kursunda var!!!!
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






//kimlik do�rulama i�lemi
//
builder.Services.AddAuthentication(options =>
{

    //�yelik sisteminde 2 ayr� �yelik tipi olabilir
    //�rnek bayiler ayr� . kullan�c� olarak ayr� �ekilde    
    options.DefaultAuthenticateScheme=JwtBearerDefaults.AuthenticationScheme;

    //alttaki �emayla ileti�im kurmas� i�in bu methodu ekledik
    options.DefaultChallengeScheme=JwtBearerDefaults.AuthenticationScheme;
}).
//json web token ile Authentication ile ayn� �emay� se�meliyiz ki birbirleriyle ileti�im halinde olsunlar
AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    var tokenOptions = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>();

    //token�n parametreleri appjsonsettingsten gelenleri e�le�tiriyoruz
    opts.TokenValidationParameters=new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidIssuer=tokenOptions.Issuer,

        //burada o. indeksin sebebi. appjsonsettingste auidnece bir dizin. Fakat ilki tokenn'� da��tan api
        //daha sonrakilerde ula�abilecekleri apilerin adresleri
        ValidAudience=tokenOptions.Audience[0],

        //SignService'den �retti�imiz anahtar� e�ledik
        IssuerSigningKey=SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
        ValidateIssuerSigningKey=true,
        ValidateLifetime=true,
        ValidateAudience=true,

        //biz token'a s�re verdi�imizde �zerine defalut olarak 5 dk ekler.
        //fakat biz burda o default k�sm� kald�rd�k
        //bu fark� apileri farkl� zaman aral�klar�nda ki serverlara kurma ihitimalimizden dolay� veriyor

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
