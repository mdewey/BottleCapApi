using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BottleCapApi.Models;
using Microsoft.AspNetCore.Http;
using BottleCapApi.Models;

namespace BottleCapApi.Middleware
{
  public class RequestResponseLoggingMiddleware
  {
    private readonly RequestDelegate _next;


    public RequestResponseLoggingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context, DatabaseContext dbContext)
    {
      //Continue down the Middleware pipeline, eventually returning to this class
      var request = await FormatRequest(context.Request);
      await _next(context);
      var response = String.Empty;

      // //TODO: Save log to chosen datastore
      dbContext.Logs.Add(new Log
      {
        Request = request,
        Response = response
      });
      await dbContext.SaveChangesAsync();

    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
      var bodyAsText = String.Empty;
      var headers = request.Headers;
      foreach (var header in headers)
      {
        Console.WriteLine($"header: {header.Key} = {header.Value}");
      }

      if ((request?.ContentType?.Contains("form")).GetValueOrDefault() && request.Form.Count > 0)
      {
        Console.WriteLine("form values");
        foreach (var key in request.Form.Keys)
        {
          var text = $" ^ {key}|{request.Form[key]} ";
          Console.WriteLine(text);
          bodyAsText += text;
        }
      }
      return $"| path:{request.Path} | qs: {request.QueryString} |body: {bodyAsText}";
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
      //We need to read the response stream from the beginning...
      response.Body.Seek(0, SeekOrigin.Begin);

      //...and copy it into a string
      string text = await new StreamReader(response.Body).ReadToEndAsync();

      //We need to reset the reader for the response so that the client can read it.
      response.Body.Seek(0, SeekOrigin.Begin);


      return $"{response.StatusCode}: {text}";
    }
  }
}