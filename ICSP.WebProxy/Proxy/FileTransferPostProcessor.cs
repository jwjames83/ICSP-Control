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

            /*
            // Import System Fonts
            // G4: %CommonProgramFiles(x86)%\AMXShare\G4SupportFiles\__system\graphics\fnt.xma
            if(fileInfo.Name.Equals("fonts.xma", StringComparison.OrdinalIgnoreCase))
            {
              var lXmlDocSysFonts = new XmlDocument();

              lXmlDocSysFonts.LoadXml(Resources.SysFonts);

              var lTargetNode = lXmlDoc.DocumentElement.SelectSingleNode("/root/fontList");

              if(lTargetNode.ChildNodes.Count == 0)
              {
                foreach(XmlNode node in lXmlDocSysFonts.DocumentElement.SelectNodes("/root/fontList/font"))
                {
                  var lNode = lXmlDoc.ImportNode(node, true);

                  lTargetNode.AppendChild(lNode);
                }
              }
              else
              {
                XmlNode lRefChild = null;

                foreach(XmlNode node in lXmlDocSysFonts.DocumentElement.SelectNodes("/root/fontList/font"))
                {
                  var lNode = lXmlDoc.ImportNode(node, true);

                  // Order by Font-Index
                  if(lRefChild == null)
                    lRefChild = lTargetNode.InsertBefore(lNode, lTargetNode.FirstChild);
                  else
                    lRefChild = lTargetNode.InsertAfter(lNode, lRefChild);
                }
              }
            }
            */

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
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["panelSetup"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["supportFileList"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["projectInfo"]);
                  ((JObject)lJsonProject["settings"]).Merge(lJsonObj["versionInfo"]);
                }
                catch(Exception ex)
                {
                  Console.WriteLine(ex.Message);
                }

                break;
              }
              case "fonts.xma":
              {
                // TODO ..
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
                    var lPageName = (string)lJsonObj["page"]["name"];
                    var lPageType = (string)lJsonObj["page"]["type"];

                    lJsonObj["page"]["name"].Parent.Remove();
                    lJsonObj["page"]["type"].Parent.Remove();

                    var lStatesObj = new JObject(new JProperty("states", new JObject()));

                    // https://stackoverflow.com/questions/51547673/json-path-expression-not-working-without-array
                    var lStates = lJsonObj.SelectTokens("$.page.sr..[?(@.number)]");

                    foreach(var sr in lStates)
                    {
                      var lNumber = (string)sr["number"];

                      sr["number"].Parent.Remove();

                      ((JObject)lStatesObj["states"]).Add(new JProperty(lNumber, sr));
                    }

                    lJsonObj["page"]["sr"].Parent.Remove();

                    var lPageObj = new JObject(new JProperty(lPageName, lJsonObj["page"]));

                    ((JObject)lPageObj[lPageName]).Add(lStatesObj.Property("states"));

                    switch(lPageType)
                    {
                      case "page"    /**/: ((JObject)lJsonProject["pages"]).Add(lPageObj.Property(lPageName)); break;
                      case "subpage" /**/: ((JObject)lJsonProject["subpages"]).Add(lPageObj.Property(lPageName)); break;
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

          var lJSonProjStr = lJsonProject.ToString(Newtonsoft.Json.Formatting.Indented, new NullStringConverter());

          // JavaScript
          lJSonProjStr = $"var project = {lJSonProjStr}";

          var lFileNameJsProject = string.Format(@"{0}\..\js\{1}", lDir.FullName, "project.web.V02.js");

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

    private void JsonArrrayHelper(XmlDocument xmlDoc)
    {
      var lNamespace = "http://james.newtonking.com/projects/json";

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

      xmlDoc.DocumentElement.SetAttribute("xmlns:json", lNamespace);

      // Pages    : /root/pageList/pageEntry[]
      // PageFlips: /root/page/button/pf
      // var lElements = xmlDoc.SelectNodes("/root/pageList/pageEntry|/rootresourceList/resource|/root/page/button|/root/page/button/pf|/root/tableList/tableEntry|/root/tableList/tableEntry/row");
      var lElements = xmlDoc.SelectNodes("/root/panelSetup/powerUpPopup");

      foreach(XmlElement element in lElements)
      {
        var lAttr = xmlDoc.CreateAttribute("Array", lNamespace);

        lAttr.Value = "true";

        element.Attributes.Append(lAttr);
      }
    }
  }
}
