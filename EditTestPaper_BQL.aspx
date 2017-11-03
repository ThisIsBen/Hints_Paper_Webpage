﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EditTestPaper_BQL.aspx.cs" Inherits="AuthoringTool_CaseEditor_Paper_EditTestPaper_BQL" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html>

 <style type="text/css">
        input.bigcheck
        {
            height: 30px;
            width: 30px;
            cursor: pointer;
        }

        table {
			border-collapse:collapse;
		}
		table tr.border td {
			border-top: Solid 1px Black;
			border-bottom: Solid 1px Black;
		}
		.border .left {
			border-left: Solid 1px Black;
		}		
		.border .right {
			border-right: Solid 1px Black;
		}	
</style>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>基本版題目表-考題編輯</title>
</head>
<script type="text/javascript">
    function BrowseConversation(strQID, strGroupID, strCareer, ProType) {
        window.open("Paper_ConversationQuestionEditor.aspx?QID=" + strQID + "&AnswerType=1&GroupID=" + strGroupID + "&Career=" + strCareer + "&bModify=True&Browse=" + ProType ,'_blank', 'directories=0, height=450, menubar=0, resizable=1, scrollbars=1, status=0, titlebar=1, toolbar=0, width=700');
    }

    function NextFunction() {
        var Temp = document.getElementsByTagName("input");
        var strTmp = "";
        for (var i = 0; i < Temp.length; i++) {
            if (Temp[i].type == "checkbox") {
                var strArray = Temp[i].id.split("-");
                if (strArray[0] == "CheckBoxforConversation") {
                    if (Temp[i].checked == true) {
                        strTmp += (strArray[1] + ",");
                    }
                }
            }
        }
        document.getElementById("btnSaveNext").click();
    }

    function SaveSelected(strSelectedProtype, strPaperID, strGroupID, strCaseID, strcbID) {
        if (strSelectedProtype != '') {          
            document.getElementById("HF_SelectedProType").value = strSelectedProtype;
        }

        var Temp = document.getElementsByTagName("input");
        var strTmp = "";
        for (var i = 0; i < Temp.length; i++) {
            if (Temp[i].type == "radio") {
                var strArray = Temp[i].id.split("-");
                if (strArray[0] == "CheckBoxforConversation") {
                    if (Temp[i].checked == true) {
                        strTmp += (strArray[1] + ",");
                    }
                }
            }
        }
        AuthoringTool_CaseEditor_Paper_EditTestPaper_BQL.ConfirmSave(document.getElementById("HF_SelectedProType").value, strTmp, strPaperID, strGroupID, strCaseID, strcbID);
    }

</script>
<body>
    <form id="form1" runat="server">
    <div style="text-align: right">
        <asp:DropDownList ID="ddl_MutiLanguage" runat="server" AutoPostBack="true" Font-Size="14px" OnSelectedIndexChanged="ddl_MutiLanguage_SelectedIndexChanged">
            <asp:ListItem Value="en-US">en-US</asp:ListItem>
            <asp:ListItem Value="zh-TW">zh-TW</asp:ListItem>         
        </asp:DropDownList>
    </div>
    <table style="border: 2px solid #000000; width: 1250px; height: 800px" align="center">
      <tr>
       <td style="height: 50px; background-color: #00FFFF;" colspan="2" align="center">
           <asp:Label ID="Lb_Topic" runat="server" Text="" Font-Bold="True" Font-Size="30px" ></asp:Label>&nbsp;
           <asp:Label ID="Lb_Title" runat="server" Text="" Font-Bold="True" ForeColor="Red" Font-Size="30px"></asp:Label>&nbsp;
           <asp:Label ID="Lb_TopicZh" runat="server" Text="" Font-Bold="True" Font-Size="30px" ></asp:Label>
       </td>
      </tr>
      <tr style="height: 50px" class="border">
       <td style="background-color: #CCFFFF" align="center" class="right">
           <asp:Label ID="Lb_QuesTopic" runat="server" Text="" Font-Bold="True" ForeColor="Red" Font-Size="30px"></asp:Label>
       </td>
       <td style="background-color: #CCFFFF" align="center">
           <asp:Label ID="Lb_ConQues" runat="server" Text="" Font-Bold="True" ForeColor="Red" Font-Size="30px"></asp:Label><br/>
           <asp:DropDownList ID="ddlProblemType" runat="server" AutoPostBack="true" Font-Size="24px" OnSelectedIndexChanged="ddlProblemType_SelectedIndexChanged" Visible="false">
               <asp:ListItem>Select a Problem Type</asp:ListItem>
           </asp:DropDownList>
       </td>
      </tr>
      <tr style="background-color: #FFFFCC; height:650px">
        <td style="margin: 0px; width: 50%">
           <asp:Table ID="tbQuestionTopic" runat="server" Width="100%" Height="100%" BorderColor="Black" BorderWidth="2px"></asp:Table>
        </td>
        <td style="width: 50%">
         <table style="height: 100%; width: 100%">
           <tr>
             <td style="border: 2px solid #000000; height: 100%; text-align: center; vertical-align: top;">
                 <asp:Label ID="Lb_Conversation" runat="server" Text="" Font-Bold="True" ForeColor="Blue" Font-Size="30px"></asp:Label>
                 <hr/>
                 <asp:Table ID="tbSelectConversation" runat="server" Width="100%"></asp:Table>
             </td>
           </tr>
          </table>
        </td>
      </tr>
      <tr>
        <td colspan="2" style="background-color: #CCFFFF; height:50px" align="right">
             <input id="btnEditDescription" type="button" value="" style="font-size: x-large; background-color: #0000FF; color: #FFFFFF; cursor:pointer; display:none;" onclick="ShowEditDiv()" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="BtnBack" runat="server" Text="" Font-Size="X-Large" OnClick="BtnBack_Click" Style="cursor:pointer;" BackColor="Blue" ForeColor="White" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <input type="button" ID="BtnNext" runat="server" Value="" onclick="NextFunction()"  style="font-size: x-large; background-color: #0000FF; cursor:pointer; color: #FFFFFF;" />&nbsp;&nbsp;
             <input id="btnSaveNext" style="display:none" type="button" name="btnSaveNext" runat="server" />
             <input id="btnTempSave" style="display:none" type="button" name="btnTempSave" runat="server" />
        </td>
      </tr>
    </table>   
        <asp:HiddenField ID="HF_RowCount" runat="server" />
        <asp:HiddenField ID="HF_SelectedIndex" runat="server" />
        <asp:HiddenField ID="HF_SelectedQuesTopic" runat="server" />
        <asp:HiddenField ID="HF_SelectedProType" runat="server" />
        <asp:HiddenField ID="HF_CurrentRowIndex" runat="server" />       
    </form>
</body>
</html>
