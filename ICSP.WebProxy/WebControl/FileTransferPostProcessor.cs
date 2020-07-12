using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using ICSP.Core.Constants;
using ICSP.Core.Model;
using ICSP.Core.Model.ProjectProperties;
using ICSP.WebProxy.Json;
using ICSP.WebProxy.Properties;
using ICSP.WebProxy.Proxy;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.WebProxy.WebControl
{
  public class FileTransferPostProcessor
  {
    private readonly Dictionary<string, string> mJsonList;

    private readonly List<Font> mFontsG4;

    private readonly List<Font> mFontsG5;

    private readonly List<PaletteData> mPalettes;

    private WebControlProject mProject;

    public FileTransferPostProcessor(ProxyClient client)
    {
      Client = client;

      mJsonList = new Dictionary<string, string>();

      mFontsG4 = new List<Font>();

      mFontsG5 = new List<Font>();

      mPalettes = new List<PaletteData>();
    }

    public ProxyClient Client { get; private set; }

    public void ProcessFiles()
    {
      try
      {
        var lDirectory = new DirectoryInfo(Path.Combine(Client?.Manager?.FileManager?.BaseDirectory ?? string.Empty, "AMXPanel"));

        Client?.LogInformation(string.Format("Directory={0:l}", lDirectory.FullName));

        mProject = new WebControlProject();

        mJsonList.Clear();

        mFontsG4.Clear();

        mFontsG5.Clear();

        mPalettes.Clear();

        if(CreateJsonList(lDirectory))
          ProccessJsonList(lDirectory);
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private bool CreateJsonList(DirectoryInfo directory)
    {
      try
      {
        Client?.LogDebug(string.Format("Directory={0:l}", directory.FullName));

        var lEncoding = false;

        var lFiles = new Dictionary<string, FileInfo>();

        var lProjectFile = Path.Combine(directory.FullName, "prj.xma");

        var lJson = ReadFileToJson(lProjectFile, false);

        mProject.Settings = JsonConvert.DeserializeObject<Project>(lJson);

        var lPanelInfo = Panels.GetPanelByDeviceType(mProject.Settings.PanelType);

        lEncoding = lPanelInfo.Generation == Core.PanelGeneration.G5;

        // PanelType G5 => Read again with correct encoding
        if(lEncoding)
        {
          lJson = ReadFileToJson(lProjectFile, lEncoding);

          mProject.Settings = JsonConvert.DeserializeObject<Project>(lJson);

          lPanelInfo = Panels.GetPanelByDeviceType(mProject.Settings.PanelType);

          lEncoding = lPanelInfo.Generation == Core.PanelGeneration.G5;
        }

        var lFile = new FileInfo(Path.Combine(directory.FullName, mProject.Settings.FontFile ?? string.Empty));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // G4: fnt.xma
        lFile = new FileInfo(Path.Combine(directory.FullName, "fnt.xma"));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // G5: fonts.xma
        lFile = new FileInfo(Path.Combine(directory.FullName, "fonts.xma"));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // PageList
        foreach(var file in mProject.Settings.PageList)
        {
          lFile = new FileInfo(Path.Combine(directory.FullName, file.File));

          if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
            lFiles.Add(lFile.FullName, lFile);
        }

        // prj.xma
        lFile = new FileInfo(Path.Combine(directory.FullName, "prj.xma"));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // icon.xma
        lFile = new FileInfo(Path.Combine(directory.FullName, mProject.Settings.IconFile ?? string.Empty));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // pal_001.xma / pal_002.xma / ...
        lFile = new FileInfo(Path.Combine(directory.FullName, mProject.Settings.ColorFile ?? string.Empty));
        if(lFile.Exists && !lFiles.ContainsKey(lFile.FullName))
          lFiles.Add(lFile.FullName, lFile);

        // Create index.html
        var lHtml = Resources.MainPage;
        var lProjectPath = new DirectoryInfo(Client?.Manager?.FileManager?.BaseDirectory ?? string.Empty);

        lHtml = lHtml?
          .Replace("{title}", mProject.Settings.JobName)
          .Replace("{width}", mProject.Settings.ScreenWidth.ToString())
          .Replace("{height}", mProject.Settings.ScreenHeight.ToString())
          .Replace("{projectPath}", lProjectPath.Name);

        var lFileNameMainPage = string.Format(@"{0}\..\{1}", directory.FullName, "index.html");

        File.WriteAllText(lFileNameMainPage, lHtml);

        foreach(var fileInfo in lFiles.Values)
        {
          try
          {
            // Ignore ...
            switch(fileInfo.Name.ToLower())
            {
              case "g5apps.xma":
              case "logs.xma":
              case "manifest.xma": continue;
            }

            lJson = ReadFileToJson(fileInfo.FullName, lEncoding);

            if(!mJsonList.ContainsKey(fileInfo.Name))
              mJsonList.Add(fileInfo.Name, lJson);
          }
          catch(Exception ex)
          {
            Client?.LogError(string.Format("FileName={0:l}, Message={1:l}", fileInfo.Name, ex.Message));

            return false;
          }
        }
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);

        return false;
      }

      return true;
    }

    private string ReadFileToJson(string path, bool encoding)
    {
      var lXml = File.ReadAllText(path);

      // Wrong encoded xml
      // G5: Convert Windows-1252 to UTF-8 (ZurÃ¼ck -> Zurück, LÃ¶schen -> Löschen, etc.)
      if(encoding)
        lXml = Encoding.UTF8.GetString(Encoding.GetEncoding(1252).GetBytes(lXml));

      var lXmlDoc = new XmlDocument();

      lXmlDoc.LoadXml(lXml);

      // Remove XmlDeclaration
      foreach(XmlNode node in lXmlDoc)
      {
        if(node.NodeType == XmlNodeType.XmlDeclaration)
          lXmlDoc.RemoveChild(node);
      }

      // Force nodes to be rendered as an Array
      JsonConfigureArrays(lXmlDoc);

      var lJson = JsonConvert.SerializeXmlNode(lXmlDoc, Newtonsoft.Json.Formatting.Indented, true);

      // <page type="page">
      // JSON.NET and Replacing @ Sign in XML to JSON conversion
      return (Regex.Replace(lJson, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase));
    }

    private void JsonConfigureArrays(XmlDocument xmlDoc)
    {
      try
      {
        /*
        From Json.NET documentation: http://james.newtonking.com/projects/json/help/?topic=html/ConvertingJSONandXML.htm
        You can force a node to be rendered as an Array by adding the attribute json:Array='true' to the XML node you are converting to JSON.

        Also, you need to declare the json prefix namespace at the XML header xmlns:json='http://james.newtonking.com/projects/json' or 
        else you will get an XML error stating that the json prefix is not declared.

        The next example is provided by the documentation:
        xml = @"<person xmlns:json='http://james.newtonking.com/projects/json' id='1'>
              <name>Alan</name>
              <url>http://www.google.com</url>
              <role json:Array='true'>Admin</role>
            </person>";

        */

        var lNamespace = "http://james.newtonking.com/projects/json";

        xmlDoc.DocumentElement.SetAttribute("xmlns:json", lNamespace);

        var lXPath = string.Join("|", new[] {
          
          // Project
          "/root/panelSetup/powerUpPopup",                // PowerUpPopup's
          "/root/pageList",                               // Pages/SubPages (type: page, subpage)
          "/root/pageList/pageEntry",                     // Pages/SubPages
          "/root/tableList/tableEntry",                   // G4: List-Tables
          "/root/tableList/tableEntry/row",               // G4: List-Tables
          "/root/subPageSets/subPageSetEntry",            // Sub-Page Sets
          "/root/subPageSets/subPageSetEntry/items/item", // Sub-Page Sets -> Items
          "/root/dropGroups/dropGroup",                   // G5: DropGroups
          "/root/dropGroups/dropGroup/dgItems/dgItem",    // G5: DropGroup -> Items
          "/root/resourceList",                           // Resources (type: image, dataSource)
          "/root/resourceList/resource",                  // Resources
          "/root/fwFeatureList/feature",                  // Features
          "/root/paletteList/palette",                    // Palettes
          
          "/root/page/sr",                                // Page States
          "/root/page/button",                            // Buttons
          "/root/page/button/sr",                         // Buttons States (Button-Type Joystick has only one)
          "/root/page/button/pf",                         // G4:PageFlips
          "/root/page/button/sr/bitmapEntry",             // G5:Bitmaps

          // G5: Button Events
          "/root/page/button/*/pgFlip",                   // Events: Pageflips
          "/root/page/button/*/launch",                   // Events: Actions -> Launch
          "/root/page/button/*/command",                  // Events: Actions -> Command
          "/root/page/button/*/string",                   // Events: Actions -> String
          "/root/page/button/*/custom",                   // Events: Actions -> Custom
          
          /*
          "/root/page/button/ep",           // Events: Button Press
          "/root/page/button/er",           // Events: Button Release
          "/root/page/button/ga",           // Events: Gesture Any
          "/root/page/button/gu",           // Events: Gesture Up
          "/root/page/button/gd",           // Events: Gesture Down
          "/root/page/button/gr",           // Events: Gesture Right
          "/root/page/button/gl",           // Events: Gesture Left
          "/root/page/button/gt",           // Events: Gesture Double-Tap
          "/root/page/button/tu",           // Events: Gesture 2-Finger Up
          "/root/page/button/td",           // Events: Gesture 2-Finger Down
          "/root/page/button/tr",           // Events: Gesture 2-Finger Right
          "/root/page/button/tl",           // Events: Gesture 2-Finger Left
          "/root/page/button/dst",          // Events
          "/root/page/button/dca",          // Events
          "/root/page/button/den",          // Events
          "/root/page/button/dex",          // Events
          "/root/page/button/ddr",          // Events
          */
        });

        var lElements = xmlDoc.SelectNodes(lXPath);

        foreach(XmlElement element in lElements)
        {
          var lAttr = xmlDoc.CreateAttribute("Array", lNamespace);

          lAttr.Value = "true";

          element.Attributes.Append(lAttr);
        }

        /*
        lXPath = "/root/page/button/sr/bitmapEntry/fileName";

        /*
        "bitmapEntry": {
          "fileName
          }

        lElements = xmlDoc.SelectNodes(lXPath);

        foreach(XmlElement element in lElements)
        {
          var lAttr = xmlDoc.CreateAttribute("Object", lNamespace);

          lAttr.Value = "true";

          element.Attributes.Append(lAttr);
        }
        */
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void ProccessJsonList(DirectoryInfo directory)
    {
      try
      {
        Client?.LogDebug("ProccessJsonList: Directory={0}", directory.FullName);

        if((mJsonList?.Count ?? 0) < 5)
        {
          Client?.LogInformation(string.Format("ProcessFiles: FileCount={0}, processing was aborted because Readable FileCount < 6", mJsonList?.Count));
          Client?.LogInformation(string.Format("ProcessFiles: Important for G5 Panels: XML-Files in G5-Designs are always encrypted. Excecute from Menu: [Panel]/[Verify Function Maps] in TPDesign5 before send."));

          return;
        }

        /*
        WebControl 1.7.0 JavaScript Project-Structure:
        ----------------------------------------------
        var project = {
          "settings": {
		        "pageList": [
			        {
				        "name": "Main",
				        "pageID": "1",
				        "file": "Main.xml",
				        "isValid": "-1"
			        },
			        {
				        "name": "PP_OP_AP00_TUEREN_BRANDFALL",
				        "pageID": "502",
				        "file": "PP_OP_AP00_TUEREN_BRANDFALL.xml",
				        "group": "GrpOP",
				        "isValid": "-1",
				        "popupType": "1"
			        },
            ...
            ],
            "paletteList": [
			        {
				        "name": "Palette 1",
				        "file": "pal_001.xma",
				        "paletteID": "1"
			        }
		        ],
            "resourceList": {},
            "iconList": {},
            "powerUpPopup": [
              "PP_MAINNAVI_AP00"
              ],
              
          // Merged
		      "portCount": "17",
          ...
		      "designVersion": null
          // Merged
          },
          "pages": {
            "Main": { ... },
            ...
          },
          "subpages": {
            "PP_SideNavi_3": { ... },
            ...
          },
          "palettes": {
		        "default": {
			        "0": {
				        "name": "VeryLightRed",
				        "color": "#FF0000FF"
			        },
			        "1": {
				        "name": "LightRed",
				        "color": "#DF0000FF"
			        },
              ...
          },
          "fonts": {
            "34": {
			      "file": "arialbd.ttf",
			      "fileSize": "980756",
			      "name": "Arial",
			      "subfamilyName": "Bold",
			      "fullName": "Arial Bold",
			      "size": "12",
			      "usageCount": "110"
		        },
            ...
          },
          "chameleons": {
            "bt_66x40_chamel.png": "data:image/png;base64,...",
            ...
            }
          }
        */

        var lJsonProject = new JObject
        {
          // Main-Properties:
          // --------------------------------------------
          new JProperty("settings", new JObject()
            {
              new JProperty("pageList", new JArray()),
              new JProperty("paletteList", new JArray()),
              new JProperty("resourceList", new JObject()),
              new JProperty("iconList", new JObject()),
              new JProperty("powerUpPopup", new JArray()),
            }),
          new JProperty("pages", new JObject()),
          new JProperty("subpages", new JObject()),
          new JProperty("palettes", new JObject()),
          new JProperty("fonts", new JObject()),
          new JProperty("chameleons", new JObject()),
        };

        var lSettingsObj = (JObject)lJsonProject["settings"];

        try
        {
          // Add default palette
          ((JObject)lJsonProject["palettes"]).Add(new JProperty("default", JObject.Parse(Resources.DefaultPalette)));

          var lDefaultPalette = new PaletteData() { Name = "default" };

          var lPalettes = JsonConvert.DeserializeObject<Dictionary<int, WebControlPaletteDataItem>>(Resources.DefaultPalette);

          foreach(var palette in lPalettes)
          {
            palette.Value.Index = palette.Key;

            lDefaultPalette.PaletteDataItems.TryAdd(palette.Key, palette.Value);
          }

          mPalettes.Add(lDefaultPalette);
        }
        catch(Exception ex)
        {
          Client?.LogError(ex.Message);
        }

        try
        {
          // Add System Fonts (Font 1-31)
          ((JObject)lJsonProject["fonts"]).Merge(JObject.Parse(Resources.SystemFonts));

          var lFonts = JsonConvert.DeserializeObject<Dictionary<int, Font>>(Resources.SystemFonts);

          foreach(var font in lFonts)
          {
            font.Value.Number = font.Key;

            mFontsG4.Add(font.Value);
          }
        }
        catch(Exception ex)
        {
          Client?.LogError(ex.Message);
        }

        foreach(var keyValue in mJsonList)
        {
          var lJsonObj = JObject.Parse(keyValue.Value).SelectToken("", false);

          // Omit root object
          if((lJsonObj.First as JProperty)?.Name == "root")
            lJsonObj = lJsonObj.SelectToken("root", false);

          var lProperty = lJsonObj.First as JProperty;

          switch(keyValue.Key.ToLower())
          {
            case "prj.xma":
            {
              JsonProccessProject(lJsonObj, lSettingsObj);
              break;
            }
            case "icon.xma":
            {
              JsonProccessIcons(lJsonObj, lSettingsObj);
              break;
            }
            case "fnt.xma": // G4
            {
              JsonProccessFontsG4(lJsonObj, (JObject)lJsonProject["fonts"]);
              break;
            }
            case "fonts.xma": // G5
            {
              JsonProccessFontsG5(lJsonObj);
              break;
            }
            default:
            {
              switch(lProperty?.Name)
              {
                case "page":
                {
                  try
                  {
                    var lPage = JsonConvert.DeserializeObject<SubPage>(lProperty?.Value?.ToString());

                    // Replace \r\n -> &#13&#10
                    if(lPage.States != null)
                    {
                      foreach(var state in lPage.States)
                        state.Text = state.Text?.Replace("\r", "&#13")?.Replace("\n", "&#10");
                    }

                    // Replace \r\n -> &#13&#10
                    if(lPage.Buttons != null)
                    {
                      foreach(var button in lPage.Buttons)
                      {
                        if(button.States != null)
                        {
                          foreach(var state in button.States)
                            state.Text = state.Text?.Replace("\r", "&#13")?.Replace("\n", "&#10");
                        }
                      }
                    }

                    switch(lPage.Type)
                    {
                      case PageType.Page    /**/: mProject.Pages.TryAdd(lPage.Name, lPage); break;
                      case PageType.SubPage /**/: mProject.SubPages.TryAdd(lPage.Name, lPage); break;
                    }
                  }
                  catch(Exception ex)
                  {
                    Client?.LogError(ex.Message);
                  }

                  JsonProccessPage(lJsonObj, lJsonProject);

                  break;
                }
                case "paletteData":
                {
                  try
                  {
                    var lPaletteList = JsonConvert.DeserializeObject<PaletteData>(keyValue.Value);

                    mPalettes.Add(lPaletteList);
                  }
                  catch(Exception ex)
                  {
                    Client?.LogError(ex.Message);
                  }

                  JsonProccessPalette(lJsonObj, (JObject)lJsonProject["palettes"]);

                  break;
                }
                // case "cm" /**/:
                // case "bm" /**/:
                // case "sm" /**/: lJsonProject["map"] = lJsonObj; break;
                // default   /**/: lJsonProject.Merge(lJsonObj); break;
              }

              break;
            }
          }
        }

        // Process Fonts
        JsonConvertButtonFonts(lJsonProject);

        // Read all chameleon images
        JsonGenerateChameleonImages(lJsonProject, Path.Combine(directory.FullName, "images"));

        Client?.LogInformation("Project Serialize Start ...");

        var lJSonProjStr = lJsonProject.ToString(Newtonsoft.Json.Formatting.Indented, new WebControlJsonConverter());

        Client?.LogInformation("Project Serialize End ...");

        // JavaScript
        lJSonProjStr = $"var project = {lJSonProjStr}";

        var lFileNameJsProject = string.Format(@"{0}\..\js\{1}", directory.FullName, "project.js");

        File.WriteAllText(lFileNameJsProject, lJSonProjStr);

        // ======================================================================================================
        // WebControl (In work ...)
        // ======================================================================================================

        foreach(var palette in mPalettes)
          mProject.Palettes.TryAdd(palette.Name, palette.PaletteDataItems.ToDictionary(k => k.Key, e => (WebControlPaletteDataItem)e.Value));

        foreach(var font in mFontsG4)
          mProject.Fonts.TryAdd(font.Number, font);

        var lImages = lJsonProject.SelectTokens("$.pages..buttons..states..mi")
          .Union(lJsonProject.SelectTokens("$.subpages..buttons..states..mi"))
          .Distinct().Select(s => (string)s);

        foreach(var imageName in lImages)
        {
          var lFileName = Path.Combine(Path.Combine(directory.FullName, "images"), imageName);

          if(File.Exists(lFileName))
          {
            try
            {
              var lBase64 = $"data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes(lFileName))}";

              mProject.Chameleons.Add(imageName, lBase64);
            }
            catch(Exception ex)
            {
              Client?.LogError(ex.Message);
            }
          }
        }

        lJSonProjStr = JsonConvert.SerializeObject(mProject, Newtonsoft.Json.Formatting.Indented, new WebControlJsonConverter());

        // JavaScript
        lJSonProjStr = $"var project = {lJSonProjStr}";

        lFileNameJsProject = string.Format(@"{0}\..\js\{1}", directory.FullName, "project.wc.js");

        File.WriteAllText(lFileNameJsProject, lJSonProjStr);

        Client?.LogInformation($"Project Write: FileName={lFileNameJsProject} End ...");
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonProccessProject(JToken source, JObject settings)
    {
      try
      {
        // <root><pageList type="page"><pageEntry>...
        // <root><pageList type="subpage"><pageEntry>...
        var lPageEntries = source["pageList"].SelectMany(s => s.SelectToken("pageEntry"));

        ((JArray)settings["pageList"]).Merge(lPageEntries);

        // <root><paletteList><palette>...
        var lPalettes = source.SelectTokens("$.paletteList.palette..[?(@.name)]");

        ((JArray)settings["paletteList"]).Merge(lPalettes);

        // <root><resourceList type="image"><resource>...
        var lResources = source.SelectTokens("$.resourceList.resource..[?(@.name)]");

        foreach(var resource in lResources)
        {
          var lName = (string)resource["name"];

          resource["name"].Parent.Remove();

          ((JObject)settings["resourceList"]).Add(new JProperty(lName, resource));
        }

        settings.Merge(source["panelSetup"]);
        settings.Merge(source["supportFileList"]);
        settings.Merge(source["projectInfo"]);
        settings.Merge(source["versionInfo"]);

        settings["password"] = "";
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonProccessIcons(JToken source, JObject settings)
    {
      try
      {
        // <root><iconList><icon number="1">...
        var lIcons = source.SelectTokens("$.iconList.icon..[?(@.number)]");

        foreach(var icon in lIcons)
        {
          var lNumber = (string)icon["number"];

          icon["number"].Parent.Remove();

          ((JObject)settings["iconList"]).Add(new JProperty(lNumber, icon));
        }
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonProccessFontsG4(JToken source, JObject fonts)
    {
      try
      {
        try
        {
          if(source["fontList"]?["font"] is JArray allfonts)
            mFontsG4.AddRange(JsonConvert.DeserializeObject<List<Font>>(allfonts.ToString()));
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }

        var lFontsObj = new JObject(new JProperty("fonts", new JObject()));

        // <root><fontList><font number="{n}">...
        var lFonts = source.SelectTokens("$.fontList.font..[?(@.number)]");

        foreach(var font in lFonts)
        {
          var lNumber = (string)font["number"];

          font["number"].Parent.Remove();
          font["faceIndex"].Parent.Remove();

          ((JObject)lFontsObj["fonts"]).Add(new JProperty(lNumber, font));
        }

        fonts.Merge(lFontsObj["fonts"]);
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonProccessFontsG5(JToken source)
    {
      try
      {
        if(source["fontList"]?["font"] is JArray fonts)
          mFontsG5.AddRange(JsonConvert.DeserializeObject<List<Font>>(fonts.ToString()));
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void JsonConvertButtonFonts(JObject project)
    {
      try
      {
        var lMaxFontIndex = mFontsG4.Max(s => s.Number);

        // Fonts
        var lStates = project.SelectTokens("$..states..[?(@ff)]").ToList();

        foreach(var state in lStates)
        {
          var lFontFace = (string)state["ff"];
          var lFontSize = (int)(state["fs"] ?? 0);

          state["ff"]?.Parent?.Remove();
          state["fs"]?.Parent?.Remove();

          var lG4Font = mFontsG4.FirstOrDefault(p => p.File == lFontFace && p.Size == lFontSize);

          if(lG4Font != null)
          {
            state["fi"] = lG4Font.Number;
          }
          else
          {
            var lFullName = mFontsG5.FirstOrDefault(p => p.File == lFontFace)?.FullName;

            if(lFullName == null)
              lFullName = lFontFace.Replace(".ttf", "");

            var lFont = new Font()
            {
              Number = ++lMaxFontIndex,
              File = lFontFace,
              Size = lFontSize,
              Name = lFullName,
              SubFamilyName = "Regular",
              FullName = lFullName,
            };

            mFontsG4.Add(lFont);

            var lFontObj = JObject.Parse(JsonConvert.SerializeObject(lFont));

            lFontObj["number"]?.Parent?.Remove();
            lFontObj["faceIndex"]?.Parent?.Remove();

            ((JObject)project["fonts"]).Merge(new JObject(new JProperty(lFont.Number.ToString(), lFontObj)));

            state["fi"] = lFont.Number.ToString();
          }
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private void JsonProccessPalette(JToken source, JObject palettes)
    {
      try
      {
        var lPaletteName = (string)source["paletteData"]["name"];

        var lPaletteObj = new JObject(new JProperty(lPaletteName, new JObject()));

        var lColors = source.SelectTokens("$.paletteData.color[?(@.name)]");

        foreach(var color in lColors)
        {
          var lIndex = (string)color["index"];

          color["index"].Parent.Remove();
          color["#text"].Rename("color");

          ((JObject)lPaletteObj[lPaletteName]).Add(new JProperty(lIndex, color));
        }

        if(palettes[lPaletteName] == null)
          palettes.Add(lPaletteObj.Property(lPaletteName));
        else
          palettes[lPaletteName] = lPaletteObj[lPaletteName];
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonProccessPage(JToken source, JObject project)
    {
      try
      {
        var lPage = (JObject)source["page"];

        var lPageName = (string)lPage["name"];
        var lPageType = (string)lPage["type"];

        lPage.Property("name").Remove();
        lPage.Property("type").Remove();

        // --------------------------------------------------------
        // Page: rename button -> buttons
        //       Add empty array if not exists (needed)
        // --------------------------------------------------------
        if(lPage["button"] != null)
          lPage["button"].Rename("buttons");
        else
          lPage["buttons"] = new JArray();

        // Move to first (not relevant)
        var lButtonsProperty = lPage.Property("buttons");

        lButtonsProperty.Remove();

        lPage.AddFirst(lButtonsProperty);

        // --------------------------------------------------------
        // Page: Convert states (sr -> states)
        // --------------------------------------------------------
        JsonConvertStates((JObject)lPage);

        try
        {
          // --------------------------------------------------------
          // Buttons: Convert states (sr -> states)
          // --------------------------------------------------------
          var lButtons = source.SelectTokens("$.page.buttons.[*]");

          foreach(var button in lButtons)
          {
            JsonConvertStates((JObject)button);

            // Set null or add (not needed)
            // <ac di="0" />
            if(button["ac"] != null)
            {
              // Set null (not needed)
              button["ac"] = "";
            }
            else
            {
              // Add after rh (Range High) for G4 compatibility (not needed)
              if(button["rh"] != null)
                button["rh"].Parent.AddAfterSelf(new JProperty("ac", ""));
              else
                button["ac"] = "";
            }

            // PageFlips: Add empty array if not exists (needed)
            if(button["pf"] == null)
            {
              // Add after ac for G4 compatibility (not needed)
              if(button["ac"] != null)
                button["ac"].Parent.AddAfterSelf(new JProperty("pf", new JArray()));
              else
                button["pf"] = new JArray();
            }

            // G5: Convert back to G4 ...
            // --------------------------------------------------------
            /*
            <button type="general">
              <bi>1</bi>                 ==> Index
              <na>Button A</na>          ==> Name
              <lt>40</lt>                ==> Position Left
              <tp>34</tp>                ==> Position Top
              <wt>207</wt>               ==> Width
              <ht>109</ht>               ==> Height
              <zo>1</zo>                 ==> Z-Order ([<]Back, [>]:Top)
              <bs></bs>                  ==> Border Style (-> TAB General)
              <rl>0</rl>                 ==> Range Low
              <rh>255</rh>               ==> Range High
              <sr number="1">            ==> State Off
                <bs></bs>                ==> Border Style
                <cb>#FFFF00FF</cb>       ==> Border Color
                <cf>#0000DFFF</cf>       ==> Fill Color
                <ct>#FFFFFFFF</ct>       ==> Text Color
                <ec>#000000FF</ec>       ==> Text Effect color
                <bitmapEntry>
                  <fileName>{filename}</fileName>
                </bitmapEntry>
                <ff>arial.ttf</ff>       ==> Font
                <fs>10</fs>              ==> Font Size
              </sr>
              <sr number="2">
                <bs>
                </bs>
                <cb>#0000DFFF</cb>
                <cf>#FFFF00FF</cf>
                <ct>#000000FF</ct>
                <ec>#000000FF</ec>
                <bitmapEntry>
                  <fileName>realvista_webdesign_3d_design_16.png</fileName>
                </bitmapEntry>
                <ff>arial.ttf</ff>
                <fs>10</fs>
              </sr>
              <ep /> ==> Events: Button Press
              <er /> ==> Events: Button Release
              <ga /> ==> Events: Gesture Any
              <gu /> ==> Events: Gesture Up
              <gd /> ==> Events: Gesture Down
              <gr /> ==> Events: Gesture Right
              <gl /> ==> Events: Gesture Left
              <gt /> ==> Events: Gesture Double-Tap
              <tu /> ==> Events: Gesture 2-Finger Up
              <td /> ==> Events: Gesture 2-Finger Down
              <tr /> ==> Events: Gesture 2-Finger Right
              <tl /> ==> Events: Gesture 2-Finger Left
              <dst / ==> Events: 
              <dca / ==> Events: 
              <den / ==> Events: 
              <dex / ==> Events: 
              <ddr / ==> Events: 
            </button>
            */

            /*
              "bitmapEntry": {
                "fileName": "realvista_webdesign_3d_design_16.png"
              },
            */
            var lBitmaps = button.SelectTokens("$.states..bitmapEntry..fileName");

            foreach(var bitmap in lBitmaps)
            {
              // Ugly ... :-)
              var lState = bitmap?.Parent?.Parent?.Parent?.Parent?.Parent;

              // Only the first bitmap ...
              if(lState["bm"] == null)
              {
                // Add after ec (Text Effect color) for G4 compatibility (not needed)
                if(lState["ec"] != null)
                  lState["ec"].Parent.AddAfterSelf(new JProperty("bm", (string)bitmap));
                else
                  lState["bm"] = (string)bitmap;
              }
            }

            lBitmaps = button.SelectTokens("$.states..bitmapEntry").ToList();

            foreach(var item in lBitmaps)
              item?.Parent?.Remove();

            // Button Press
            var lEvents = button.SelectTokens("$.ep..pgFlip.[*]");

            foreach(var evt in lEvents)
            {
              evt["item"]?.Parent.Remove();

              ((JArray)button["pf"]).Add(evt);
            }

            // Button Release
            lEvents = button.SelectTokens("$.er..pgFlip.[*]");

            foreach(var evt in lEvents)
            {
              evt["item"]?.Parent.Remove();

              ((JArray)button["pf"]).Add(evt);
            }

            button["ep"]?.Parent.Remove();
            button["er"]?.Parent.Remove();

            // Remove poperties where not used in WebControl
            button["ac"]?.Parent.Remove();

            // G5: Gesture poperties
            button["ga"]?.Parent.Remove();
            button["gu"]?.Parent.Remove();
            button["gd"]?.Parent.Remove();
            button["gr"]?.Parent.Remove();
            button["gl"]?.Parent.Remove();
            button["gt"]?.Parent.Remove();
            button["tu"]?.Parent.Remove();
            button["td"]?.Parent.Remove();
            button["tr"]?.Parent.Remove();
            button["tl"]?.Parent.Remove();
            button["dst"]?.Parent.Remove();
            button["dca"]?.Parent.Remove();
            button["den"]?.Parent.Remove();
            button["dex"]?.Parent.Remove();
            button["ddr"]?.Parent.Remove();

            /*
            Convert G5 FontNamily & FontSize to G4 FontIndex
            ------------------------------------------------
            "ff": "avenirltstd-book.ttf",
            "fs": "10",
            "fi": "12",
            */

            // PageFlips: Add empty array if not exists (needed)
            if(button["pf"] == null)
            {
              // Add after ac for G4 compatibility (not needed)
              if(button["ac"] != null)
                button["ac"].Parent.AddAfterSelf(new JProperty("pf", new JArray()));
              else
                button["pf"] = new JArray();
            }
          }

          // --------------------------------------------------------
          // Buttons: PageFlips (rename #text -> value)
          // --------------------------------------------------------

          var lPageFlips = source.SelectTokens("$.page.buttons..pf").Cast<JArray>();

          foreach(var pageFlip in lPageFlips)
          {
            foreach(var item in pageFlip)
              item["#text"]?.Rename("value");
          }

          // Remove (not needed)
          // "popupType": "popup",
          lPage["popupType"]?.Parent?.Remove();
        }
        catch(Exception ex)
        {
          Console.WriteLine(ex.Message);
        }

        var lPg = source["page"].ToString();

        switch(lPageType)
        {
          case "page"    /**/: ((JObject)project["pages"]).Add(new JProperty(lPageName, source["page"])); break;
          case "subpage" /**/: ((JObject)project["subpages"]).Add(new JProperty(lPageName, source["page"])); break;
          default:
          {
            break;
          }
        }
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonGenerateChameleonImages(JObject project, string directory)
    {
      try
      {
        /*
        <button type="general">
          <bi>1</bi>                 ==> Index
          <na>Button A</na>          ==> Name
          <li>1</li>                 ==>
          <lt>16</lt>                ==> Position Left
          <tp>16</tp>                ==> Position Top
          <wt>125</wt>               ==> Width
          <ht>125</ht>               ==> Height
          <zo>1</zo>                 ==> Z-Order ([<]Back, [>]:Top)
          <hs>bounding</hs>          ==> Touch Style  (bounding, passThru)
          <bs></bs>                  ==> Border Style (-> TAB General)
          <da>1</da>                 ==> Disabled
          <hd>1</hd>                 ==> Hidden
          <fb>momentary</fb>         ==> Feedback
          <ap>2</ap>                 ==> Address Port
          <ad>22</ad>                ==> Address Code
          <cp>3</cp>                 ==> Channel Port
          <ch>33</ch>                ==> Channel Code
          <vt>rel</vt>               ==> Level Control Type
          <lp>4</lp>                 ==> Level Port
          <lv>44</lv>                ==> Level Code
          <rl>0</rl>                 ==> Range Low
          <rh>255</rh>               ==> Range High
          <ac di="0" />              ==> 
          <sr number="1">            ==> State Off
            <bs>Quad Line</bs>       ==> Border Style
            <mi>Image.jpg</mi>       ==> Chameleon Image
            <cb>Grey7</cb>           ==> Border Color
            <cf>VeryLightOrange</cf> ==> Fill Color
            <ct>Black</ct>           ==> Text Color
            <ec>#000000FF</ec>       ==> Text Effect color
            <oo>100</oo>             ==> Overall Opacity
            <bm>Image.png</bm>       ==> Bitmap
            <jb>1</jb>               ==> Bitmap Justification (Top-Left)
            <bx>20</bx>              ==> Bitmap X Offset
            <by>20</by>              ==> Bitmap Y Offset
            <fi>36</fi>              ==> Font Number (Ref -> $fnt.xml)
            <te>Button A</te>        ==> Text
            <jt>1</jt>               ==> Text Justification (Top-Left)
            <tx>20</tx>              ==> Text X Offset
            <ty>20</ty>              ==> Text Y Offset
            <ww>1</ww>               ==> Word Wrap
          </sr>
          <sr number="2">            ==> State On
          ...
          </sr>
        </button>
        */

        /*
        Reference
        -------------------------------------------------
        "pages": {
          "Main": {
            "buttons": [
              {
                "states": {
                  "1": {
                    "mi": "bt_66x40_chamel.png",

        JSON:
        "chameleons": {
          "{imageName}": "data:image/png;base64,{base64}",
          ...
        */

        var lImages = project.SelectTokens("$.pages..buttons..states..mi")
          .Union(project.SelectTokens("$.subpages..buttons..states..mi"))
          .Distinct().Select(s => (string)s);

        foreach(var imageName in lImages)
        {
          var lFileName = Path.Combine(directory, imageName);

          if(File.Exists(lFileName))
          {
            try
            {
              var lBase64 = $"data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes(lFileName))}";

              ((JObject)project["chameleons"]).Add(new JProperty(imageName, lBase64));
            }
            catch(Exception ex)
            {
              Client?.LogError(ex.Message);
            }
          }
        }
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonConvertStates(JObject parent)
    {
      try
      {
        var lStatesObj = new JObject(new JProperty("states", new JObject()));

        // https://stackoverflow.com/questions/51547673/json-path-expression-not-working-without-array
        var lStates = parent.SelectTokens("sr..[?(@.number)]");

        if(lStates.Count() > 0)
        {
          foreach(var sr in lStates)
          {
            var lNumber = (string)sr["number"];

            sr["number"]?.Parent?.Remove();

            if(sr["te"] != null)
              sr["te"] = ((string)sr["te"])?.Replace("\r", "&#13").Replace("\n", "&#10").Replace(" ", "&nbsp;");

            ((JObject)lStatesObj["states"]).Add(new JProperty(lNumber, sr));
          }

          parent["sr"].Parent.Remove();

          parent.Add(lStatesObj.Property("states"));
        }

        // --------------------------------------------------------
        // popupType="appwindow"
        // states: Add empty array if not exists (needed)
        // --------------------------------------------------------
        if(parent["states"] == null)
          parent["states"] = JObject.Parse("{ '1': { } }");
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }
  }
}
