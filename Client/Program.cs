



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();



}



app.UseHttpsRedirection();
app.UseStaticFiles();  // This line enables serving static files

app.UseRouting();

app.MapGet("/", () => "Hello World!");

app.Run();
