<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Contact Manager</title>
    <style type="text/css">
        html, body
        {
            height: 100%;
            overflow: auto;
            background:black;
        }
        body
        {
            padding: 0;
            margin: 0;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" style="height: 100%; width: 100%;">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableHistory="true" EnableSecureHistoryState="false">
    </asp:ScriptManager>
    <div style="height: 100%; width: 100%;">
        <div id="silverlightControlHost" style="width: 100%; height: 100%">
            <object data="data:application/x-silverlight-2,"
                    type="application/x-silverlight-2"
                    style="height: 100%; width: 100%" 
                    id="host">
                <param name="source" value="/ClientBin/ContactManager-SL3.xap"/>
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