using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ICSP.Core.Model.ProjectProperties
{
  public class Project
  {
    [JsonExtensionData]
    private IDictionary<string, JToken> mAdditionalData;

    public Project()
    {
      mAdditionalData = new Dictionary<string, JToken>();

      PageList = new List<PageEntry>();

      SubPageSets = new List<SubPageSet>();

      DropGroups = new List<DropGroup>();

      ResourceList = new List<Resource>();

      FwFeatureList = new List<FwFeature>();

      PaletteList = new List<Palette>();
    }

    [JsonProperty("versionInfo", Order = 1)]
    public VersionInfo VersionInfo { get; set; }

    [JsonProperty("projectInfo", Order = 2)]
    public ProjectInfo ProjectInfo { get; set; }

    [JsonProperty("supportFileList", Order = 3)]
    public SupportFileList SupportFileList { get; set; }

    [JsonProperty("panelSetup", Order = 4)]
    public PanelSetup PanelSetup { get; set; }

    [JsonIgnore]
    public List<PageEntry> PageList { get; set; }

    [JsonIgnore]
    public List<SubPageSet> SubPageSets { get; set; }

    [JsonIgnore]
    public List<DropGroup> DropGroups { get; set; }

    [JsonIgnore]
    public List<Resource> ResourceList { get; set; }

    [JsonIgnore]
    public List<FwFeature> FwFeatureList { get; set; }

    [JsonIgnore]
    public List<Palette> PaletteList { get; set; }

    [OnDeserialized]
    private void OnDeserializedMethod(StreamingContext context)
    {
      try
      {
        if(mAdditionalData.TryGetValue("pageList", out var pageListToken))
        {
          foreach(var pageList in pageListToken)
          {
            var lType = pageList["type"]?.ToString();

            if(pageList["pageEntry"] != null)
            {
              foreach(var page in pageList["pageEntry"])
              {
                var lPage = page?.ToObject<PageEntry>();

                lPage.Type = lType;

                PageList?.Add(lPage);
              }
            }
          }

          if(mAdditionalData.TryGetValue("subPageSets", out var subPageSetsToken))
          {
            if(subPageSetsToken["subPageSetEntry"] != null)
              SubPageSets.AddRange(subPageSetsToken["subPageSetEntry"].ToObject<List<SubPageSet>>());
          }

          if(mAdditionalData.TryGetValue("dropGroups", out var dropGroupsToken))
          {
            if(dropGroupsToken["dropGroup"] != null)
              DropGroups.AddRange(dropGroupsToken["dropGroup"].ToObject<List<DropGroup>>());
          }

          if(mAdditionalData.TryGetValue("resourceList", out var resourceListToken))
          {
            foreach(var resourceList in resourceListToken)
            {
              var lType = resourceList["type"]?.ToString();

              if(resourceList["resource"] != null)
              {
                foreach(var resource in resourceList["resource"])
                {
                  var lResource = resource?.ToObject<Resource>();

                  lResource.Type = lType;

                  ResourceList?.Add(lResource);
                }
              }
            }
          }

          if(mAdditionalData.TryGetValue("fwFeatureList", out var fwFeatureListToken))
          {
            if(fwFeatureListToken["feature"] != null)
              FwFeatureList.AddRange(fwFeatureListToken["feature"].ToObject<List<FwFeature>>());
          }

          if(mAdditionalData.TryGetValue("paletteList", out var paletteToken))
          {
            if(paletteToken["palette"] != null)
              PaletteList.AddRange(paletteToken["palette"].ToObject<List<Palette>>());
          }
        }
      }
      catch(Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }
  }
}