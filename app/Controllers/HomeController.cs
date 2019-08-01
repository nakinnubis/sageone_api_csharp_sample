﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using app.Models;

namespace app.Controllers
{
  public class HomeController : Controller
  {
    ContentModel model = new ContentModel();


    public IActionResult Index()
  
    {
      Console.WriteLine("index controller");
      this.tokenfileRead(HttpContext);

      HttpContext.Session.SetString("BaseUrl", "http://" + HttpContext.Request.Host);


      if (this.configFileExist())
      {
        model.clientApplicationConfigNotFound = "0";
      }
      else
      {
        model.clientApplicationConfigNotFound = "1";
      }

      // default values
      var sessionName = new Byte[20];
      if (!HttpContext.Session.TryGetValue("reqEndpoint", out sessionName))
        HttpContext.Session.SetString("reqEndpoint", "user");


      if (!HttpContext.Session.TryGetValue("access_token", out sessionName))
      {
        // access_token field is empty
        model.partialAccessTokenAvailable = "0";
        model.partialResposeIsAvailable = "0";
      }
      else if (HttpContext.Session.TryGetValue("access_token", out sessionName) && !HttpContext.Session.TryGetValue("responseContent", out sessionName))
      {
        // access_token filled and responseContent is empty
        model.partialAccessTokenAvailable = "1";
        model.partialResposeIsAvailable = "0";
      }
      else
      {
        model.partialAccessTokenAvailable = "1";
        model.partialResposeIsAvailable = "1";
      }

      return View(model);
    } 

    public IActionResult Guide()
    {
      return View();
    }

    public IActionResult Req()
    {
      return View();
    }

    public IActionResult Resp()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View();
    }

    public void tokenfileRead(HttpContext context)
    {
      if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "access_token.json")))
      {
        using (StreamReader file = System.IO.File.OpenText(Path.Combine(Directory.GetCurrentDirectory(), "access_token.json")))
        using (JsonTextReader reader = new JsonTextReader(file))
        {
          JObject jsonObj = (JObject)JToken.ReadFrom(reader);
          context.Request.HttpContext.Session.SetString("access_token", (string)jsonObj["access_token"]);
          context.Request.HttpContext.Session.SetString("expires_at", (string)jsonObj["expires_at"]);
          context.Request.HttpContext.Session.SetString("refresh_token", (string)jsonObj["refresh_token"]);
          context.Request.HttpContext.Session.SetString("refresh_token_expires_at", (string)jsonObj["refresh_token_expires_at"]);
        }
      }
    }
    public bool configFileExist()
    {
      if (System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "client_application.json")))
      {
        return true;
      }

      return false;
    }
  }
}
