using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BottleCapApi.Models;
using Microsoft.AspNetCore.Http;


namespace BottleCapApi.Services
{
  public class DmService
  {


    public DmService(DatabaseContext _context)
    {
      this.context = _context;
    }

    public DatabaseContext context { get; }


    public bool IsUserDm(string user)
    {
      return user.Contains("UAJ4NHEKG");
    }



  }
}