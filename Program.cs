using ExtractApkApi.Controllers;
using ExtractApkApi.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

            

app.MapGet("/", () => "Hello World!");

//upload file and extract with ZipArchive
app.MapPost("/api/upload", (IFormFile file)=>{
    ExtractController extract = new ExtractController();
    ResponseApi result = extract.extractFile(file);
    if(result.status == 200){
        return Results.Ok(result);
    } else {
        return Results.BadRequest(result);
    
    }
}).DisableAntiforgery();



app.Run();
