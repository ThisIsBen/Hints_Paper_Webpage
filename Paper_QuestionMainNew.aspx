﻿<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Paper_QuestionMainNew.aspx.cs"
    Inherits="PaperSystem.Paper_QuestionMainNew" %>

<html>
<head id="HEAD1" runat="server">
    <title>Paper_QuestionMain</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <br />
    <br />
    <br />
    <table id="body_content" style="width: 500px" cellpadding="10" align="center">
        <tr>
            <td class="title">
                Please select a function:
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <%--<input id="rb1" type="radio" checked value="rb1" name="rb" runat="server" />Edit questions--%>
                <input id="rb1" type="radio" checked value="rb1" name="rb" runat="server" />Create New Questions
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb4" type="radio" value="rb4" name="rb" runat="server" />Edit Vision of Questions
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <%--<input id="rb6" type="radio" value="rb6" name="rb" runat="server" />Edit a test paper--%>
                <input id="rb6" type="radio" value="rb6" name="rb" runat="server" />Create  a New Exam Paper
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb5" type="radio" value="rb5" name="rb" runat="server" />Edit Featurevalues
            </td>
        </tr>
       
        <tr id="trPreview" align="left" runat="server" style="display:none;">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb2" type="radio" value="rb2" name="rb" runat="server" />Edit Problem Type
            </td>
        </tr>
        <!--
        <tr id="tr1" align="left" runat="server">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb3" type="radio" value="rb3" name="rb" runat="server" />Return to case
                menu
            </td>
        </tr>
        -->
        <tr align="right">
            <td>
                <input id="btnBack" style="width: 150px; height: 30px;" class="button_continue" type="button" 
                    runat="server" value="<< Back" name="btnBack"/>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="btnNext" style="width: 150px; height: 30px" class="button_continue" type="button"
                    runat="server" value="Next >>" name="btnNext" />
            </td>
        </tr>
    </table>
    <input id="hiddenOpener" type="hidden" name="hiddenOpener" runat="server" />
    </form>
</body>
</html>
