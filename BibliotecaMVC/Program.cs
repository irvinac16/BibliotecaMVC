using BibliotecaMVC.Services;
using BibliotecaMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaDB")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<BibliotecaContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<IAutorService, AutorServiceAlternativo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


const string rolAdministrador = "Administrador";
const string rolUsuario = "Usuario";
const string nombreUsuarioAdministrador = "Jarvis";

using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;

    var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = servicios.GetRequiredService<UserManager<IdentityUser>>();

    foreach (var nombreRol in new[] { rolAdministrador, rolUsuario })
    {
        if (!await roleManager.RoleExistsAsync(nombreRol))
        {
            var resultadoRol = await roleManager.CreateAsync(
                new IdentityRole(nombreRol));

            if (!resultadoRol.Succeeded)
            {
                throw new Exception(
                    "No se pudo crear el rol: " +
                    string.Join(", ", resultadoRol.Errors.Select(e => e.Description)));
            }
        }
    }

    var usuarios = await userManager.Users.ToListAsync();

    foreach (var usuario in usuarios)
    {
        var rolesAsignados = await userManager.GetRolesAsync(usuario);

        if (rolesAsignados.Count == 0)
        {
            var resultadoUsuario = await userManager.AddToRoleAsync(
                usuario, rolUsuario);

            if (!resultadoUsuario.Succeeded)
            {
                throw new Exception(
                    "No se pudo asignar el rol Usuario a " + usuario.UserName);
            }
        }
    }

    var administrador = await userManager.FindByNameAsync(
        nombreUsuarioAdministrador);

    if (administrador == null)
    {
        throw new Exception(
            "No se encontró el usuario '" +
            nombreUsuarioAdministrador +
            "'. Verifica el nombre exacto en la base de datos.");
    }

    if (await userManager.IsInRoleAsync(administrador, rolUsuario))
    {
        var resultadoEliminar = await userManager.RemoveFromRoleAsync(
            administrador, rolUsuario);

        if (!resultadoEliminar.Succeeded)
        {
            throw new Exception(
                "No se pudo quitar el rol Usuario de Jarvis.");
        }
    }

    if (!await userManager.IsInRoleAsync(administrador, rolAdministrador))
    {
        var resultadoAdmin = await userManager.AddToRoleAsync(
            administrador, rolAdministrador);

        if (!resultadoAdmin.Succeeded)
        {
            throw new Exception(
                "No se pudo asignar el rol Administrador a Jarvis.");
        }
    }
}
app.Run();
