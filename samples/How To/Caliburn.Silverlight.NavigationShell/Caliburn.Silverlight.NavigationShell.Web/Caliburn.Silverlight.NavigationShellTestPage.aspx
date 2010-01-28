﻿<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Caliburn.Silverlight.NavigationShell</title>
    <style type="text/css">
        html, body
        {
            height: 100%;
            overflow: auto;
        }
        body
        {
            padding: 0;
            margin: 0;
        }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
              appSource = sender.getHost().Source;
            }
            
            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
              return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " +  appSource + "\n" ;

            errMsg += "Code: "+ iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {           
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " +  args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%; width: 100%;">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableHistory="true" EnableSecureHistoryState="false">
    </asp:ScriptManager>
    <div style="height: 100%; width: 100%;">
        <div id="silverlightControlHost" style="width: 100%; height: 100%">
            <object data="data:application/x-silverlight-2,"
                    type="application/x-silverlight-2"
                    style="position: fixed; height: 100%; width: 100%; z-index: 1;"
                    id="host">
                <param name="source" value="ClientBin/Caliburn.Silverlight.NavigationShell.xap"/>
                <param name="onerror" value="onSilverlightError" />
                <param name="background" value="white" />
                <param name="minRuntimeVersion" value="3.0.40723.0" />
                <param name="autoUpgrade" value="true" />
                <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=3.0.40723.0" style="text-decoration: none;">
                     <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style: none"/>
                </a>
            </object>
        </div>
    </div>
    </form>
</body>
</html>