using App.Controllers;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
ScraperController scraperController = new ScraperController();
string json = File.ReadAllText("links.json");
List<string> links = JsonConvert.DeserializeObject<List<string>>(json);
var formatedTutors = await scraperController.Scrape();
foreach (var tutor in formatedTutors)
{
    Console.WriteLine($"Tutor: {tutor.Name}");
    Console.WriteLine($"Course: {tutor.Course.courseShortName}");
    Console.WriteLine($"Full course name: {tutor.Course.courseFullName}");
    Console.WriteLine($"Type: {tutor.Course.type}");
    Console.WriteLine(tutor.IsLead ? "Lead Tutor" : "Not lead");
    Console.WriteLine("*********************");
}
Console.WriteLine("Enter subject name: ");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();