using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1) Configura sessão em memória
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

var app = builder.Build();

app.UseSession();

// 2) Serve arquivos estáticos em wwwroot, incluindo .asp
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/html"
});

// 3) Endpoint POST /login
app.MapPost("/login", async context =>
{
    var form = await context.Request.ReadFormAsync();
    var u = form["username"];
    var p = form["password"];
    if (u == "admin" && p == "1234")
    {
        context.Session.SetString("user", u);
        context.Response.Redirect("/landing.asp");
        return;
    }
    context.Response.StatusCode = 401;
    await context.Response.WriteAsync("Credenciais inválidas");
});

// 4) Protege GET /landing.asp
app.MapGet("/landing.asp", async context =>
{
    if (string.IsNullOrEmpty(context.Session.GetString("user")))
    {
        context.Response.Redirect("/");
        return;
    }
    // Lê arquivo landing.asp e injeta o nome do usuário
    var path = Path.Combine(app.Environment.WebRootPath, "landing.asp");
    var html = await File.ReadAllTextAsync(path, Encoding.UTF8);
    html = html.Replace("{{USER}}", context.Session.GetString("user")!);
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

// 5) POST /logout
app.MapPost("/logout", async context =>
{
    context.Session.Clear();
    context.Response.Redirect("/goodbye.asp");
});

// 6) GET /goodbye.asp (static)
app.MapGet("/goodbye.asp", async context =>
{
    var path = Path.Combine(app.Environment.WebRootPath, "goodbye.asp");
    var html = await File.ReadAllTextAsync(path, Encoding.UTF8);
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});

app.Run();
