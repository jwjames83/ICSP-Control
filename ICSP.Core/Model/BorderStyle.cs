using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ICSP.Core.Model
{
  public static class BorderStyle
  {
    public static int GetBorderWidth(string borderStyle)
    {
      switch(borderStyle)
      {
        case "Single Line" /**/: return 1;
        case "Double Line" /**/: return 2;
        case "Quad Line"   /**/: return 4;
      }

      return 0;
    }
  }
}
