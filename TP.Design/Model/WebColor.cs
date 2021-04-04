namespace TP.Design.Model
{
  public static class WebColor
  {
    public static string FromColorName(string color)
    {
      switch(color)
      {
        case "VeryLightRed"     /**/: return "#FF0000";
        case "LightRed"         /**/: return "#DF0000";
        case "Red"              /**/: return "#BF0000";
        case "MediumRed"        /**/: return "#9F0000";
        case "DarkRed"          /**/: return "#7F0000";
        case "VeryDarkRed"      /**/: return "#5F0000";

        case "VeryLightOrange"  /**/: return "#FF8000";
        case "LightOrange"      /**/: return "#DF7000";
        case "Orange"           /**/: return "#BF6000";
        case "MediumOrange"     /**/: return "#9F5000";
        case "DarkOrange"       /**/: return "#7F4000";
        case "VeryDarkOrange"   /**/: return "#5F3000";

        case "VeryLightYellow"  /**/: return "#FFFF00";
        case "LightYellow"      /**/: return "#DFDF00";
        case "Yellow"           /**/: return "#BFBF00";
        case "MediumYellow"     /**/: return "#9F9F00";
        case "DarkYellow"       /**/: return "#7F7F00";
        case "VeryDarkYellow"   /**/: return "#5F5F00";

        case "VeryLightLime"    /**/: return "#80FF00";
        case "LightLime"        /**/: return "#70DF00";
        case "Lime"             /**/: return "#60BF00";
        case "MediumLime"       /**/: return "#509F00";
        case "DarkLime"         /**/: return "#407F00";
        case "VeryDarkLime"     /**/: return "#305F00";

        case "VeryLightGreen"   /**/: return "#00FF00";
        case "LightGreen"       /**/: return "#00DF00";
        case "Green"            /**/: return "#00BF00";
        case "MediumGreen"      /**/: return "#009F00";
        case "DarkGreen"        /**/: return "#007F00";
        case "VeryDarkGreen"    /**/: return "#005F00";

        case "VeryLightMint"    /**/: return "#00FF80";
        case "LightMint"        /**/: return "#00DF70";
        case "Mint"             /**/: return "#00BF60";
        case "MediumMint"       /**/: return "#009F50";
        case "DarkMint"         /**/: return "#007F40";
        case "VeryDarkMint"     /**/: return "#005F30";

        case "VeryLightCyan"    /**/: return "#00FFFF";
        case "LightCyan"        /**/: return "#00DFDF";
        case "Cyan"             /**/: return "#00BFBF";
        case "MediumCyan"       /**/: return "#009F9F";
        case "DarkCyan"         /**/: return "#007F7F";
        case "VeryDarkCyan"     /**/: return "#005F5F";

        case "VeryLightAqua"    /**/: return "#0080FF";
        case "LightAqua"        /**/: return "#0070DF";
        case "Aqua"             /**/: return "#0060BF";
        case "MediumAqua"       /**/: return "#00509F";
        case "DarkAqua"         /**/: return "#00407F";
        case "VeryDarkAqua"     /**/: return "#00305F";

        case "VeryLightBlue"    /**/: return "#0000FF";
        case "LightBlue"        /**/: return "#0000DF";
        case "Blue"             /**/: return "#0000BF";
        case "MediumBlue"       /**/: return "#00009F";
        case "DarkBlue"         /**/: return "#00007F";
        case "VeryDarkBlue"     /**/: return "#00005F";

        case "VeryLightPurple"  /**/: return "#8000FF";
        case "LightPurple"      /**/: return "#7000DF";
        case "Purple"           /**/: return "#6000BF";
        case "MediumPurple"     /**/: return "#50009F";
        case "DarkPurple"       /**/: return "#40007F";
        case "VeryDarkPurple"   /**/: return "#30005F";

        case "VeryLightMagenta" /**/: return "#FF00FF";
        case "LightMagenta"     /**/: return "#DF00DF";
        case "Magenta"          /**/: return "#BF00BF";
        case "MediumMagenta"    /**/: return "#9F009F";
        case "DarkMagenta"      /**/: return "#7F007F";
        case "VeryDarkMagenta"  /**/: return "#5F005F";

        case "VeryLightPink"    /**/: return "#FF0080";
        case "LightPink"        /**/: return "#DF0070";
        case "Pink"             /**/: return "#BF0060";
        case "MediumPink"       /**/: return "#9F0050";
        case "DarkPink"         /**/: return "#7F0040";
        case "VeryDarkPink"     /**/: return "#5F0030";

        case "White"  /**/: return "#FFFFFF";
        case "Grey1"  /**/: return "#EEEEEE";
        case "Grey2"  /**/: return "#DDDDDD";
        case "Grey3"  /**/: return "#CCCCCC";
        case "Grey4"  /**/: return "#BBBBBB";
        case "Grey5"  /**/: return "#AAAAAA";
        case "Grey6"  /**/: return "#999999";
        case "Grey7"  /**/: return "#888888";
        case "Grey8"  /**/: return "#777777";
        case "Grey9"  /**/: return "#666666";
        case "Grey10" /**/: return "#555555";
        case "Grey11" /**/: return "#444444";
        case "Grey12" /**/: return "#333333";
        case "Grey13" /**/: return "#222222";
        case "Grey14" /**/: return "#111111";
        case "Black"  /**/: return "#000000";

        case "Transparent": return "transparent";
      }

      // IE -> Remove Alphacolor
      // #AEFFFFFF
      return color?.Substring(0, 7);
    }
  }
}
