using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BottleCapApi.Models;
using Microsoft.AspNetCore.Http;


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
      //First, get the incoming request
      var request = await FormatRequest(context.Request);

      //Copy a pointer to the original response body stream
      var originalBodyStream = context.Response.Body;

      //Create a new memory stream...
      using (var responseBody = new MemoryStream())
      {
        //...and use that for the temporary response body
        context.Response.Body = responseBody;

        //Continue down the Middleware pipeline, eventually returning to this class
        await _next(context);

        //Format the response from the server
        var response = await FormatResponse(context.Response);

        //TODO: Save log to chosen datastore
        if (!(request.Contains("path:/api/Log")))
        {
          Console.WriteLine("request");
          Console.WriteLine(request);
          Console.WriteLine("response");
          Console.WriteLine(response);
          dbContext.Logs.Add(new Log
          {
            Request = request,
            Response = response
          });
          await dbContext.SaveChangesAsync();

        }


        //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
        await responseBody.CopyToAsync(originalBodyStream);
      }
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

      //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
      return $"{response.StatusCode}: {text}";
    }
  }
}