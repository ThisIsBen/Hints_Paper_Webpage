﻿<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Paper_QuestionTypeNew.aspx.cs"
    Inherits="PaperSystem.Paper_QuestionTypeNew" %>

<html>
<head id="HEAD1" runat="server">
    <title>Paper_QuestionType</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="C#">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">

   

</head>

    <script type="text/javascript">
        function goBack() {


			    //Ben check
			    //回上一頁
			    //window.history.go(-1);


			    // the original 回上一頁 mechanism  
                /*
				var strOpener = "";
				if(document.getElementById("hiddenOpener") != null)
				{
				    
				    strOpener = document.getElementById("hiddenOpener").value;

				}
                */
            
            //alert(document.getElementById("hiddenOpener").value);
            
            
            
                   if (document.getElementById("hiddenOpener").value == "Paper_MainPage" || document.getElementById("hiddenOpener").value == "Paper_QuestionViewNew"||document.getElementById("hiddenOpener").value =="./QuestionGroupTree/QGroupTreeNew") {
			      
                    if ('<%=Session["PreviousPageURL"]%>' != null)
                        location.href = '<%=Session["PreviousPageURL"].ToString()%>';

			        }
			            //if (document.getElementById("hiddenPreOpener").value == "SelectPaperMode" || document.getElementById("hiddenPreOpener").value == "SelectPaperModeAddANewQuestion") {
			            else if ( document.getElementById("hiddenPreOpener").value == "SelectPaperModeAddANewQuestion") {
                            location.href = 'QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question';
			            }
                            
                            //else if( )
                             //{
				                    //location.href = 'QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question';
			        
                             //}
                            
                    
                    


           
			      
                    
                
                
			}
    </script>
<body>
    <form id="Form1" method="post" runat="server">
    <br />
    <br />
    <br />
    <table id="body_content" align="center" style="width: 400px" cellpadding="10">
        <tr>
            <td class="title">
                Question type:
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb1" type="radio" checked value="rb1" name="rb" runat="server" />選擇題
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rbSelectionWithKeyWords" type="radio" value="selectionWithKeyWords" name="rb" runat="server" />選擇題含關鍵字
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb2" type="radio" value="rb2" name="rb" runat="server" />問答題
            </td>
        </tr>
        <tr align="left">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rb3" type="radio" value="rb3" name="rb" runat="server">圖形題
            </td>
        </tr>
     
        <tr align="left" id="trConversation" runat="server">
            <td class="subtitle">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rbConversation" type="radio" value="rbConversation" name="rb" runat="server">對話題
            </td>
        </tr>
        
        <tr align="left">
            <td class="subtitle">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rbSituation" type="radio" value="rbSituation" name="rb" runat="server">情境題
            </td>
        </tr>


         <tr align="left">
            <td class="subtitle">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rbProgram" type="radio" value="rbProgram" name="rb" runat="server">程式題
            </td>
        </tr>
        

        <tr align="left">
            <td class="subtitle">
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="rbAI" type="radio" value="rbAI" name="rb" runat="server">Anatomy Image 題
            </td>
        </tr>


        <tr align="right">
            <td>
                <input id="btnPre" class="button_continue" style="width: 150px; height: 30px" onclick="goBack()"
                    type="button" value="<< Back" name="btnPre">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <input id="btnNext" class="button_continue" style="width: 150px; height: 30px" type="submit"
                    value="Next >>" name="btnNext" runat="server">
            </td>
        </tr>
    </table>
    <input id="hiddenOpener" type="hidden" name="hiddenOpener" runat="server">
    
    <div style="position: absolute; top: 400px">
        <input id="Hidden1" type="hidden" name="hiddenOpener" runat="server">
        <input id="hiddenQuestionMode" type="hidden" name="hiddenQuestionMode" runat="server">
        <input id="hiddenPresentType" type="hidden" name="hiddenPresentType" runat="server">
        <input id="hiddenEditMode" type="hidden" name="hiddenEditMode" runat="server">
        <input id="hiddenCaseID" type="hidden" name="hiddenCaseID" value="" runat="server">
        <input id="hiddenSectionName" type="hidden" name="hiddenSectionName" value="" runat="server">
        <input id="hiddenPaperID" type="hidden" name="hiddenPaperID" value="" runat="server">
        <input id="hiddenPreOpener" type="hidden" name="hiddenPreOpener" value="" runat="server">
    </div>
    </form>
</body>
</html>
