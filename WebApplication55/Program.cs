using System.Collections.Concurrent;
using System.Security.Cryptography;
using Microsoft.Playwright;
using WebApplication55;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.Start();

using var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});

var context = await browser.NewContextAsync();
var page = await context.NewPageAsync();


page.SetDefaultTimeout(500);

do
{
    try
    {
        await page.GotoAsync("http://localhost:5216/");

        var testVisibleFalse = await page.IsVisibleAsync("#test");

        if (testVisibleFalse)
        {
            throw new Exception("weird");
        }
        
        await page.Locator("#btn").ClickAsync(new LocatorClickOptions()
        {
            Delay = 100,
        });
        
        await Task.Delay(100);
        
        var testVisible = await page.IsVisibleAsync("#test");
        if (!testVisible)
        {
            throw new SuccessException();
        }
        
        Console.WriteLine("OK - showed");
    }
    catch (SuccessException e)
    {
        // THIS SHOULD NOT HAPPEN,
        // ClickAsync should throw with timeout 
        // OR element should be visible
        throw;
    }
    catch (Exception e)
    {
        Console.WriteLine("OK - click threw");
        // ignored
    }

    await Task.Delay(100);
} while (true);