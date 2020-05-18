using System;
using System.Diagnostics;
using System.Windows.Forms;

using ICSPControl.Environment;

namespace ICSPControl.Dialogs
{
  public static class ErrorMessageBox
  {
    public static void Show(Exception ex)
    {
      var lMethod = string.Format("{0}.{1}", ex.TargetSite.DeclaringType, ex.TargetSite.Name);

      var lCallerMethod = new StackTrace().GetFrame(1).GetMethod();

      var lCallerName = string.Format("{0}.{1}", lCallerMethod.DeclaringType, lCallerMethod.Name);

      var lMessage = string.Format("{0}\r\nSource: {1}\r\nExceptionType: {2}\r\nTargetSite: {3}\r\nCaller: {4}", ex.Message, ex.Source, ex.GetType(), lMethod, lCallerName);

      MessageBoxBase.Show(null, lMessage, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static void Show(IWin32Window owner, Exception ex)
    {
      var lMethod = string.Format("{0}.{1}", ex.TargetSite.DeclaringType, ex.TargetSite.Name);

      var lCallerMethod = new StackTrace().GetFrame(1).GetMethod();

      var lCallerName = string.Format("{0}.{1}", lCallerMethod.DeclaringType, lCallerMethod.Name);

      var lMessage = string.Format("{0}\r\nSource: {1}\r\nType: {2}\r\nTargetSite: {3}\r\nCaller: {4}", ex.Message, ex.Source, ex.GetType(), lMethod, lCallerName);

      MessageBoxBase.Show(owner, lMessage, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static void Show(string message)
    {
      MessageBoxBase.Show(null, message, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static void Show(string message, Exception ex)
    {
      var lMethod = string.Format("{0}.{1}", ex.TargetSite.DeclaringType, ex.TargetSite.Name);

      var lCallerMethod = new StackTrace().GetFrame(1).GetMethod();

      var lCallerName = string.Format("{0}.{1}", lCallerMethod.DeclaringType, lCallerMethod.Name);

      var lMessage = string.Format("{0}\r\n{1}\r\nSource: {2}\r\nExceptionType: {3}\r\nTargetSite: {4}\r\nCaller: {5}", message, ex.Message, ex.Source, ex.GetType(), lMethod, lCallerName);

      MessageBoxBase.Show(null, lMessage, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static void Show(string message, params object[] args)
    {
      MessageBoxBase.Show(null, string.Format(message, args), ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static void Show(IWin32Window owner, string message)
    {
      MessageBoxBase.Show(owner, message, ProgramProperties.Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }

    public static DialogResult ShowOkCancel(IWin32Window owner, string message)
    {
      return MessageBoxBase.Show(null, message, ProgramProperties.Title, MessageBoxButtons.OKCancel, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
    }
  }
}