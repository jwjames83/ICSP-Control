using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using ICSP.Core.Constants;
using ICSP.Core.Environment;
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

        try
        {
          // If exists, remove XML-Root folder from WebControl Extraction
          lDirectory = new DirectoryInfo(Path.Combine(Client?.Manager?.FileManager?.BaseDirectory ?? string.Empty, "xml"));

          if(lDirectory.Exists)
            lDirectory.Delete(true);
        }
        catch(Exception ex)
        {
          Client?.LogError(ex.Message);
        }
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

        // icon.xma
        lFile = new FileInfo(Path.Combine(directory.FullName, mProject.Settings.IconFile ?? "icon.xma"));
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
          .Replace("{height}", mProject.Settings.ScreenHeight.ToString());

        // LastBuild & Version
        lHtml = lHtml?
          .Replace("{LastBuild}", ProgramProperties.CompileDate.ToString("yyyy-MM-dd HH:mm:ss:ffffff")) // 2020-08-27 12:03:37.946386
          .Replace("{Title}", ProgramProperties.Title)
          .Replace("{version}", ProgramProperties.Version.ToString());

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
          "/root/iconList/icon",                          // Icons

          "/root/page/sr",                                // Page States
          "/root/page/sr/bitmapEntry",                    // G5:Bitmaps
          "/root/page/button",                            // Buttons
          "/root/page/button/sr",                         // Buttons States (Button-Type Joystick has only one)
          "/root/page/button/pf",                         // G4:PageFlips
          "/root/page/button/sr/bitmapEntry",             // G5:Bitmaps

          "/root/page/button/op",                         // G4:StringOutput
          "/root/page/button/cm",                         // G4:CommandOutput
          
          // G5: Page Events (eventShow, eventHide)
          "/root/page/*/pgFlip",                          // Events: Pageflips
          "/root/page/*/launch",                          // Events: Actions -> Launch
          "/root/page/*/command",                         // Events: Actions -> Command
          "/root/page/*/string",                          // Events: Actions -> String
          "/root/page/*/custom",                          // Events: Actions -> Custom
          
          // G5: Page Events (gestureAny, ...)
          "/root/page/*/*/pgFlip",                        // Events: Pageflips
          "/root/page/*/*/launch",                        // Events: Actions -> Launch
          "/root/page/*/*/command",                       // Events: Actions -> Command
          "/root/page/*/*/string",                        // Events: Actions -> String
          "/root/page/*/*/custom",                        // Events: Actions -> Custom

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

        /*
        if((mJsonList?.Count ?? 0) < 5)
        {
          Client?.LogInformation(string.Format("ProcessFiles: FileCount={0}, processing was aborted because Readable FileCount < 6", mJsonList?.Count));
          Client?.LogInformation(string.Format("ProcessFiles: Important for G5 Panels: XML-Files in G5-Designs are always encrypted. Excecute from Menu: [Panel]/[Verify Function Maps] in TPDesign5 before send."));

          return;
        }
        */

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

        try
        {
          // Add default palette
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
            case "icon.xma":
            {
              try
              {
                var lIcons = JsonConvert.DeserializeObject<IconList>(lProperty?.Value?.ToString());

                mProject.Settings.IconList = lIcons.Icons?.ToDictionary(k => k.Number, e => new IconItem() { File = e.File }) ?? new Dictionary<int, IconItem>();
              }
              catch(Exception ex)
              {
                Client?.LogError(ex.Message);
              }

              break;
            }
            case "fnt.xma": // G4
            {
              try
              {
                if(lJsonObj["fontList"]?["font"] is JArray allfonts)
                  mFontsG4.AddRange(JsonConvert.DeserializeObject<List<Font>>(allfonts.ToString()));
              }
              catch(Exception ex)
              {
                Client?.LogError(ex.Message);
              }

              break;
            }
            case "fonts.xma": // G5
            {
              try
              {
                if(lJsonObj["fontList"]?["font"] is JArray fonts)
                  mFontsG5.AddRange(JsonConvert.DeserializeObject<List<Font>>(fonts.ToString()));
              }
              catch(Exception ex)
              {
                Client?.LogError(ex.Message);
              }

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
                    var lStr = lProperty?.Value?.ToString();

                    var lPage = JsonConvert.DeserializeObject<SubPage>(lProperty?.Value?.ToString());

                    var lStates = lPage.States?.Select(s => s)
                      .Union(lPage.Buttons?.SelectMany(s => s.States) ?? new List<State>());

                    foreach(var state in lStates)
                    {
                      // Replace \r\n -> &#13&#10
                      // state.Text = state.Text?.Replace("\r", "&#13")?.Replace("\n", "&#10");

                      // Remove \r\
                      state.Text = state.Text?.Replace("\r", "");

                      // ============================================
                      // G5: Convert Bitmaps back to G4 ...
                      // ============================================
                      if(state.Bitmap == null && state.Bitmaps != null)
                      {
                        // Only the first bitmap ...
                        if(state.Bitmaps?.FirstOrDefault() is BitmapEntry bitmap)
                        {
                          state.Bitmap = bitmap.FileName;
                          state.BitmapJustification = bitmap.Justification;
                          state.BitmapOffsetX = bitmap.OffsetX;
                          state.BitmapOffsetY = bitmap.OffsetY;
                        }

                        state.Bitmaps = null;
                      }

                      // ==============================================================================================================
                      // Add ChameleonImage as base64-Resource to Project
                      // Reason: JavsScript function "getImageData" requires that the image originate on the same domain as the webpage
                      //         or else you will get a cross - domain security error.
                      // ==============================================================================================================
                      if(state.ChameleonImage != null && !mProject.Chameleons.ContainsKey(state.ChameleonImage))
                      {
                        var lFileName = Path.Combine(Path.Combine(directory.FullName, "images"), state.ChameleonImage);

                        if(File.Exists(lFileName))
                        {
                          try
                          {
                            var lBase64 = $"data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes(lFileName))}";

                            mProject.Chameleons.Add(state.ChameleonImage, lBase64);
                          }
                          catch(Exception ex)
                          {
                            Client?.LogError(ex.Message);
                          }
                        }
                      }
                    }

                    // ============================================
                    // G5: Convert Events back to G4 Pageflips ...
                    // ============================================

                    foreach(var button in lPage.Buttons ?? new List<Button>())
                    {
                      // ButtonPress
                      if(button.EventsButtonPress?.PageFlips != null)
                      {
                        if(button.PageFlips == null)
                          button.PageFlips = new List<PageFlip>();

                        button.PageFlips.AddRange(button.EventsButtonPress.PageFlips.Select(s => (PageFlip)s));
                      }

                      // ButtonRelease
                      if(button.EventsButtonRelease?.PageFlips != null)
                      {
                        if(button.PageFlips == null)
                          button.PageFlips = new List<PageFlip>();

                        button.PageFlips.AddRange(button.EventsButtonRelease.PageFlips.Select(s => (PageFlip)s));
                      }

                      // G5: Remaining events not supported -> ignore
                      button.EventsButtonPress = null;
                      button.EventsButtonRelease = null;
                      button.EventsGestureAny = null;
                      button.EventsGestureUp = null;
                      button.EventsGestureDown = null;
                      button.EventsGestureRight = null;
                      button.EventsGestureLeft = null;
                      button.EventsGestureDoubleTap = null;
                      button.EventsGesture2FingerUp = null;
                      button.EventsGesture2FingerDown = null;
                      button.EventsGesture2FingerRight = null;
                      button.EventsGesture2FingerLeft = null;
                    }

                    // G5 Page: Remaining events not supported -> ignore
                    lPage.EventsShowPage = null;
                    lPage.EventsHidePage = null;
                    lPage.EventsGestureAny = null;
                    lPage.EventsGestureUp = null;
                    lPage.EventsGestureDown = null;
                    lPage.EventsGestureRight = null;
                    lPage.EventsGestureLeft = null;
                    lPage.EventsGestureDoubleTap = null;
                    lPage.EventsGesture2FingerUp = null;
                    lPage.EventsGesture2FingerDown = null;
                    lPage.EventsGesture2FingerRight = null;
                    lPage.EventsGesture2FingerLeft = null;

                    switch(lPage.Type)
                    {
                      case PageType.Page    /**/: mProject.Pages.TryAdd(lPage.Name, lPage); break;
                      case PageType.SubPage /**/: mProject.SubPages.TryAdd(lPage.Name, lPage); break;
                    }
                  }
                  catch(Exception ex)
                  {
                    Client?.LogError(string.Format("Page: {0}, {1}", keyValue.Key, ex.Message));
                  }

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

                  // JsonProccessPalette(lJsonObj, (JObject)lJsonProject["palettes"]);

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

        // Convert G5 Fonts to G4 Fonts (ff & fs => fi)
        JsonConvertG5ButtonFonts();

        // Add Palettes to Project
        foreach(var palette in mPalettes)
          mProject.Palettes.TryAdd(palette.Name, palette.PaletteDataItems.ToDictionary(k => k.Key, e => (WebControlPaletteDataItem)e.Value));

        // Add G4-Fonts to Project
        foreach(var font in mFontsG4)
          mProject.Fonts.TryAdd(font.Number, font);

        // Serialize Project to JSON-string
        var lJSonProjStr = JsonConvert.SerializeObject(mProject, Newtonsoft.Json.Formatting.Indented, new WebControlJsonConverter());

        // Generate JavaScript variable
        lJSonProjStr = $"var project = {lJSonProjStr}";

        var lFileNameJsProject = string.Format(@"{0}\..\js\{1}", directory.FullName, "project.js");

        Client?.LogInformation($"Project Write: FileName={lFileNameJsProject}");

        File.WriteAllText(lFileNameJsProject, lJSonProjStr);
      }
      catch(Exception ex)
      {
        Client?.LogError(ex.Message);
      }
    }

    private void JsonConvertG5ButtonFonts()
    {
      try
      {
        var lMaxG4FontIndex = mFontsG4.Max(s => s.Number);

        var lStates = mProject.Pages.Values.SelectMany(s => s.States.Values)
          .Union(mProject.SubPages.Values.SelectMany(s => s.States.Values))
          .Union(mProject.Pages.Values.SelectMany(b => b.Buttons.SelectMany(bs => bs.States.Values)))
          .Union(mProject.SubPages.Values.SelectMany(b => b.Buttons.SelectMany(bs => bs.States.Values)))
          .Where(p => p.FontIndex == 0);

        foreach(var state in lStates)
        {
          // Invalid Font -> Set arial.ttf as default ...
          if(state.Font == null)
            state.Font = "arial.ttf";

          var lG4Font = mFontsG4.FirstOrDefault(p => p.File == state.Font && p.Size == state.FontSize);

          if(lG4Font != null)
          {
            state.FontIndex = lG4Font.Number;
          }
          else
          {
            var lFullName = mFontsG5.FirstOrDefault(p => p.File == state.Font)?.FullName;

            if(lFullName == null)
              lFullName = state.Font?.Replace(".ttf", "");

            var lFont = new Font()
            {
              Number = ++lMaxG4FontIndex,
              File = state.Font,
              Size = state.FontSize,
              Name = lFullName,
              SubFamilyName = "Regular",
              FullName = lFullName,
            };

            mFontsG4.Add(lFont);

            state.FontIndex = lFont.Number;
          }

          state.Font = null;
          state.FontSize = 0;
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}
