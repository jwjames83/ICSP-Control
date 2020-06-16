using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using ICSP.Core.Logging;
using ICSP.Core.Model;
using ICSP.WebProxy.Json;
using ICSP.WebProxy.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ICSP.WebProxy.Proxy
{
  public class FileTransferPostProcessor
  {
    private Dictionary<string, string> mJsonList;

    public FileTransferPostProcessor(ProxyClient client)
    {
      Client = client;
    }

    public ProxyClient Client { get; private set; }

    public void ProcessFiles()
    {
      CreateJsonList();

      ProccessJsonList();
    }

    private void CreateJsonList()
    {
      try
      {
        var lDir = new DirectoryInfo(Path.Combine(Client.Manager.FileManager.BaseDirectory, "AMXPanel"));

        Client.LogDebug("CreateJsonList: Directory={0}", lDir.FullName);

        mJsonList = new Dictionary<string, string>();

        var lXmlDoc = new XmlDocument();

        var lFiles = lDir
          .EnumerateFiles()
          .Where(file => file.Extension.Equals(".xml", StringComparison.OrdinalIgnoreCase) || file.Extension.Equals(".xma", StringComparison.OrdinalIgnoreCase))
          .ToList();

        foreach(var fileInfo in lFiles)
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

            lXmlDoc.Load(fileInfo.FullName);

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
            // JSON.NET and Replacing @ Sign in XML to JSON converstion
            lJson = (Regex.Replace(lJson, "(?<=\")(@)(?!.*\":\\s )", string.Empty, RegexOptions.IgnoreCase));

            mJsonList.Add(fileInfo.Name, lJson);
          }
          catch(Exception ex)
          {
            Logger.LogError(ex);

            Client.LogDebug("CreateJsonList: Error, Message={0:l}", ex.Message);
          }
        }
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
      }
    }

    private void ProccessJsonList()
    {
      try
      {
        var lDir = new DirectoryInfo(Path.Combine(Client.Manager.FileManager.BaseDirectory, "AMXPanel"));

        Client.LogDebug("ProccessJsonList: Directory={0}", lDir.FullName);

        if((mJsonList?.Count ?? 0) == 0)
          return;

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
        }
        catch(Exception ex)
        {
          Client.LogError(ex.Message);
        }

        try
        {
          // Add System Fonts (Font 1-31)
          ((JObject)lJsonProject["fonts"]).Merge(JObject.Parse(Resources.SystemFonts));
        }
        catch(Exception ex)
        {
          Client.LogError(ex.Message);
        }

        foreach(var keyValue in mJsonList)
        {
          var lJsonObj = JObject.Parse(keyValue.Value).SelectToken("", false);

          // Omit root object
          if((lJsonObj.First as JProperty)?.Name == "root")
            lJsonObj = lJsonObj.SelectToken("root", false);

          var lProperty = lJsonObj.First as JProperty;

          // Resources (cm/bm/sm/..)
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
            case "fnt.xma":
            {
              JsonProccessFonts(lJsonObj, (JObject)lJsonProject["fonts"]);
              break;
            }
            default:
            {
              switch(lProperty?.Name)
              {
                case "page":
                {
                  /*
                  {
                    "page": {
                      "type": "page",
                      "pageID": "1",
                      "name": "Page 1",
                      "width": "1024",
                      "height": "600",
                      "button": [
                        {
                          "type": "general",
                  */
                  var lJson = lProperty?.Value?.ToString();

                  // In work, debug stuff for Serialization ...
                  switch((string)lJsonObj["page"]?["type"])
                  {
                    case "page":
                    {
                      try
                      {
                        var lStr = lProperty?.Value?.ToString();

                        var lPage = JsonConvert.DeserializeObject<Page>(lProperty?.Value?.ToString());
                        
                        var lJsonNew = JsonConvert.SerializeObject(lPage, Newtonsoft.Json.Formatting.Indented);
                      }
                      catch(Exception ex)
                      {
                        Console.WriteLine(ex.Message);
                      }

                      break;
                    }
                    case "subpage":
                    {
                      try
                      {
                        var lStr = lProperty?.Value?.ToString();

                        var lPage = JsonConvert.DeserializeObject<SubPage>(lProperty?.Value?.ToString());

                        var lJsonNew = JsonConvert.SerializeObject(lPage, Newtonsoft.Json.Formatting.Indented);
                      }
                      catch(Exception ex)
                      {
                        Console.WriteLine(ex.Message);
                      }

                      break;
                    }
                  }

                  JsonProccessPage(lJsonObj, lJsonProject);

                  break;
                }
                case "paletteData":
                {
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

        // Read all chameleon images
        JsonGenerateChameleonImages(lJsonProject, Path.Combine(lDir.FullName, "images"));

        var lJSonProjStr = lJsonProject.ToString(Newtonsoft.Json.Formatting.Indented, new WebControlJsonConverter());

        // JavaScript
        lJSonProjStr = $"var project = {lJSonProjStr}";

        var lFileNameJsProject = string.Format(@"{0}\..\js\{1}", lDir.FullName, "project.js");

        File.WriteAllText(lFileNameJsProject, lJSonProjStr);
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
      }
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

        /*
        /rootresourceList/resource
        /root/tableList/tableEntry
        /root/tableList/tableEntry/row
        */

        var lXPath = string.Join("|", new[] {
          "/root/panelSetup/powerUpPopup",    // PowerUpPopup's
          "/root/pageList",                   // Pages/SubPages
          "/root/pageList/pageEntry",         // Pages/SubPages
          "/root/page/button",                // Buttons
          "/root/page/button/pf",             // G4:PageFlips
          "/root/page/button/sr/bitmapEntry", // G5:Bitmaps

          // G5: Button Events
          "/root/page/button/*/pgFlip",       // Events: Pageflips
          "/root/page/button/*/launch",       // Events: Actions -> Launch
          "/root/page/button/*/command",      // Events: Actions -> Command
          "/root/page/button/*/string",       // Events: Actions -> String
          "/root/page/button/*/custom",       // Events: Actions -> Custom
          
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
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
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
        Client.LogError(ex.Message);
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
        Client.LogError(ex.Message);
      }
    }

    private void JsonProccessFonts(JToken source, JObject fonts)
    {
      try
      {
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
        Client.LogError(ex.Message);
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
        Client.LogError(ex.Message);
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
            // "na": "Logo_AVS",
            if((string)button["na"] == "Logo_AVS")
            {
              var lStr = button.ToString();

              Console.WriteLine(button);
            }

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
            var lEvents = button.SelectTokens("$.ep..pgFlip");

            foreach(var evt in lEvents)
            {
              try
              {
                evt["item"]?.Parent.Remove();

                ((JArray)button["pf"]).Add(evt);
              }
              catch(Exception ex)
              {
                // Console.WriteLine(ex.Message);
              }
            }

            // Button Release
            lEvents = button.SelectTokens("$.er..pgFlip");

            foreach(var evt in lEvents)
            {
              try
              {
                evt["item"]?.Parent.Remove();

                ((JArray)button["pf"]).Add(evt);
              }
              catch(Exception ex)
              {
                // Console.WriteLine(ex.Message);
              }
            }

            button["ep"]?.Parent.Remove();
            button["er"]?.Parent.Remove();

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
        Client.LogError(ex.Message);
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
              Client.LogError(ex.Message);
            }
          }
        }
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
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

            sr["number"].Parent.Remove();

            ((JObject)lStatesObj["states"]).Add(new JProperty(lNumber, sr));
          }

          parent["sr"].Parent.Remove();

          parent.Add(lStatesObj.Property("states"));
        }
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
      }
    }
  }
}
