using System;
using System.Linq;
using System.Threading.Tasks;

using ICSP.Core.Logging;
using ICSP.WebProxy.Configuration;
using ICSP.WebProxy.Configuration.Options;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ICSP.WebProxy.Controllers
{
  public class SetupController : Controller
  {
    // GET: Setup
    public ActionResult Index([FromServices] IOptionsSnapshot<ProxyConfig> config)
    {
      return View(config.Value);
    }

    // GET: Setup/Details/5
    public ActionResult Details(int id, [FromServices] IOptionsSnapshot<ProxyConfig> config)
    {
      var lConfig = config.Value.Connections.Values.FirstOrDefault(p => p.ID == id);

      return View(lConfig);
    }

    // GET: Setup/Create
    public ActionResult Create()
    {
      return View(new ProxyConnectionConfig());
    }

    // POST: Setup/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAsync(ProxyConnectionConfig model, [FromServices] IWritableOptions<ProxyConfig> config)
    {
      try
      {
        if(ModelState.IsValid)
        {
          await config.UpdateAsync(1000, config =>
          {
            var lID = config?.Connections?.Keys.Select(ushort.Parse).DefaultIfEmpty().Max() + 1 ?? 1;

            config.Connections[lID.ToString()] = model;
          });

          return RedirectToAction(nameof(Index));
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return View();
    }

    // GET: Setup/Edit/5
    public ActionResult Edit(int id, [FromServices] IOptionsSnapshot<ProxyConfig> config)
    {
      var lConfig = config.Value.Connections.Values.FirstOrDefault(p => p.ID == id);

      return View(lConfig);
    }

    // POST: Setup/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditAsync(ProxyConnectionConfig model, [FromServices] IWritableOptions<ProxyConfig> config)
    {
      try
      {
        if(ModelState.IsValid)
        {
          await config.UpdateAsync(1000, config => { config.Connections[model.ID.ToString()] = model; });

          return RedirectToAction(nameof(Index));
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return View();
    }

    // GET: Setup/Delete/5
    public ActionResult Delete(int id, [FromServices] IOptionsSnapshot<ProxyConfig> config)
    {
      var lConfig = config.Value.Connections.Values.FirstOrDefault(p => p.ID == id);

      return View(lConfig);
    }

    // POST: Setup/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteAsync(int id, [FromServices] IWritableOptions<ProxyConfig> config)
    {
      try
      {
        if(ModelState.IsValid)
        {
          await config.UpdateAsync(1000, config =>
          {
            if(config.Connections.ContainsKey(id.ToString()))
              config.Connections.Remove(id.ToString());
          });

          return RedirectToAction(nameof(Index));
        }
      }
      catch(Exception ex)
      {
        Logger.LogError(ex.Message);
      }

      return View();
    }
  }
}
