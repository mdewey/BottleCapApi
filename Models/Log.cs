using System;

namespace BottleCapApi.Models
{
  public class Log
  {
    public int Id { get; set; }

    public string Request { get; set; }

    public string Response { get; set; }

    public DateTime When { get; set; } = DateTime.UtcNow.AddDays(-20);


  }
}