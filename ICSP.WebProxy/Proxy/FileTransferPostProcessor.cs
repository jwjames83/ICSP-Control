using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using ICSP.Core.Logging;
using ICSP.WebProxy.Json;
using ICSP.WebProxy.Properties;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            JsonArrrayHelper(lXmlDoc);

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
        try
        {
          var lJsonProject = new JObject
          {
            // Main-Properties:
            // --------------------------------------------
            new JProperty("settings", new JObject()
            {
              new JProperty("pageList", new JArray()),
              new JProperty("paletteList", new JArray()),
              new JProperty("resourceList", new JObject()),
              new JProperty("iconList", new JObject())
            }),
            new JProperty("pages", new JObject()),
            new JProperty("subpages", new JObject()),
            new JProperty("palettes", new JObject()),
            new JProperty("fonts", new JObject()),
            new JProperty("chameleons", new JObject()),
          };

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
                try
                {
                  // <root><pageList type="page"><pageEntry>...
                  // <root><pageList type="subpage"><pageEntry>...
                  var lPageEntries = lJsonObj["pageList"].SelectMany(s => s.SelectToken("pageEntry"));

                  ((JArray)lJsonProject["settings"]["pageList"]).Merge(lPageEntries);

                  // <root><paletteList><palette>...
                  var lPalettes = lJsonObj.SelectTokens("paletteList.palette");

                  ((JArray)lJsonProject["settings"]["paletteList"]).Merge(lPalettes);

                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["panelSetup"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["supportFileList"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["projectInfo"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["versionInfo"]);

                  // PowerUpPopup: Add empty array if not exists (needed)
                  // ----------------------------------------------------
                  if(lJsonProject["settings"]["powerUpPopup"] == null)
                    lJsonProject["settings"]["powerUpPopup"] = new JArray();
                }
                catch(Exception ex)
                {
                  Client.LogError(ex.Message);
                }

                break;
              }
              case "fnt.xma":
              {
                try
                {
                  var lFontsObj = new JObject(new JProperty("fonts", new JObject()));

                  // <root><fontList><font number="{n}">...
                  var lFonts = lJsonObj.SelectTokens("$.fontList.font..[?(@.number)]");

                  foreach(var font in lFonts)
                  {
                    var lNumber = (string)font["number"];

                    font["number"].Parent.Remove();
                    font["faceIndex"].Parent.Remove();

                    ((JObject)lFontsObj["fonts"]).Add(new JProperty(lNumber, font));
                  }

                  ((JObject)lJsonProject["fonts"]).Merge(lFontsObj["fonts"]);
                }
                catch(Exception ex)
                {
                  Client.LogError(ex.Message);
                }

                break;
              }
              case "pal_001.xma":
              {
                var lPaletteName = (string)lJsonObj["paletteData"]["name"];

                var lPaletteObj = new JObject(new JProperty(lPaletteName, new JObject()));

                var lColors = lJsonObj.SelectTokens("$.paletteData.color[?(@.name)]");

                foreach(var color in lColors)
                {
                  var lIndex = (string)color["index"];

                  color["index"].Parent.Remove();
                  color["#text"].Rename("color");

                  ((JObject)lPaletteObj[lPaletteName]).Add(new JProperty(lIndex, color));
                }

                if(lJsonProject["palettes"][lPaletteName] == null)
                  ((JObject)lJsonProject["palettes"]).Add(lPaletteObj.Property(lPaletteName));
                else
                  lJsonProject["palettes"][lPaletteName] = lPaletteObj[lPaletteName];

                break;
              }
              default:
              {
                switch(lProperty?.Name)
                {
                  case "page":
                  {
                    var lPage = (JObject)lJsonObj["page"];

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

                    // --------------------------------------------------------
                    // Page: Convert states (sr -> states)
                    // --------------------------------------------------------
                    ConvertStates((JObject)lPage);

                    try
                    {
                      // --------------------------------------------------------
                      // Buttons: Convert states (sr -> states)
                      // --------------------------------------------------------
                      var lButtons = lJsonObj.SelectTokens("$.page.buttons.[*]");

                      foreach(var button in lButtons)
                      {
                        ConvertStates((JObject)button);

                        // PageFlips: Add empty array if not exists (needed)
                        if(button["pf"] == null)
                          button["pf"] = new JArray();
                      }

                      // --------------------------------------------------------
                      // Buttons: PageFlips (rename #text -> value)
                      // --------------------------------------------------------
                      var lPageFlips = lJsonObj.SelectTokens("$.page.buttons..pf").Cast<JArray>();

                      foreach(var pageFlip in lPageFlips)
                      {
                        foreach(var item in pageFlip)
                          item["#text"].Rename("value");
                      }
                    }
                    catch(Exception ex)
                    {
                      Console.WriteLine(ex.Message);
                    }

                    switch(lPageType)
                    {
                      case "page"    /**/: ((JObject)lJsonProject["pages"]).Add(new JProperty(lPageName, lJsonObj["page"])); break;
                      case "subpage" /**/: ((JObject)lJsonProject["subpages"]).Add(new JProperty(lPageName, lJsonObj["page"])); break;
                      default:
                      {
                        break;
                      }
                    }

                    break;
                  }
                  // case "cm"          /**/:
                  // case "bm"          /**/:
                  // case "sm"          /**/: lJsonProject["map"] = lJsonObj; break;
                  // case "paletteData" /**/: ((JArray)lJsonProject["palettes"]).Add(lJsonObj); break;
                  // default            /**/: lJsonProject.Merge(lJsonObj); break;
                }

                break;
              }
            }
          }

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

          // Read all chameleon images
          /*
          "chameleons": {
            "{imageName}": "data:image/png;base64,{base64}",
            ...
          */

          // <root><pageList type="page"><states>...
          /*
            "pages": {
              "Main": {
                "buttons": [
                  {
                    "states": {
                      "1": {
                        "mi": "bt_66x40_chamel.png",
          */

          var lImages = lJsonProject.SelectTokens("$.pages..buttons..states..mi")
            .Union(lJsonProject.SelectTokens("$.subpages..buttons..states..mi"))
            .Distinct().Select(s => (string)s);

          foreach(var imageName in lImages)
          {
            var lFileName = Path.Combine(lDir.FullName, "images", imageName);

            if(File.Exists(lFileName))
            {
              try
              {
                var lBase64 = $"data:image/png;base64,{Convert.ToBase64String(File.ReadAllBytes(lFileName))}";

                ((JObject)lJsonProject["chameleons"]).Add(new JProperty(imageName, lBase64));
              }
              catch(Exception ex)
              {
                Client.LogError(ex.Message);
              }
            }
          }

          var lJSonProjStr = lJsonProject.ToString(Newtonsoft.Json.Formatting.Indented, new NullStringConverter());

          // JavaScript
          lJSonProjStr = $"var project = {lJSonProjStr}";

          var lFileNameJsProject = string.Format(@"{0}\..\js\{1}", lDir.FullName, "project.js");

          File.WriteAllText(lFileNameJsProject, lJSonProjStr);
        }
        catch(Exception ex)
        {
          Client.LogDebug("CreateJson: Error, Message={0:l}", ex.Message);
        }
      }
      catch(Exception ex)
      {
        Client.LogError(ex.Message);
      }
    }

    private void ConvertStates(JObject parent)
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
        Console.WriteLine(ex.Message);
      }
    }

    private void JsonArrrayHelper(XmlDocument xmlDoc)
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
        "/root/panelSetup/powerUpPopup",  // PowerUpPopup's
        "/root/pageList/pageEntry",       // Pages/SubPages
        "/root/page/button",              // Buttons
        "/root/page/button/pf",           // PageFlips
      });

      var lElements = xmlDoc.SelectNodes(lXPath);

      foreach(XmlElement element in lElements)
      {
        var lAttr = xmlDoc.CreateAttribute("Array", lNamespace);

        lAttr.Value = "true";

        element.Attributes.Append(lAttr);
      }
    }
  }
}
