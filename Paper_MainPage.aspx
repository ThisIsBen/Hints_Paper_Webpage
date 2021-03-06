<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>
<%@ Page language="c#" Inherits="PaperSystem.Paper_MainPage" ValidateRequest="false" enableViewState="False" CodeFile="Paper_MainPage.aspx.cs" %>

<html>
	<head runat="server">
		<title>Paper_MainPage</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE"/>
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="Table.css" rel="stylesheet" />
        <script src="../../../Scripts/jquery-1.11.2.min.js"></script>
         <%--<script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.0/jquery.min.js"></script>--%>
		<script type="text/javascript">
		//上一頁
		function goBack()
		{
		        var Back = "";
		        if(document.getElementById("hiddenOpener") != null)
		        {
		            Back = document.getElementById("hiddenOpener").value + ".aspx";
		        }

		        alert(document.getElementById('hiddenbIsFromSelectExistPaper').value);

		        if (document.getElementById('hiddenbIsFromSelectExistPaper').value == 'True') {//從選擇考卷頁面來
		            window.resizeTo(700, 800);
		            window.history.go(-1);
		        }
		        else if (document.getElementById("hiddenOpener").value == "SelectPaperMode") {//若從課堂練習轉來的，則直接關閉頁面
		            window.close();
		        }
		        else {
		            location.href = Back;
		        }
		}
		
		//進階設定 Paper_Presentation.aspx
		function AdvancedSetting()
		{
		    location.href = "Paper_Presentation.aspx?Opener=Paper_MainPage";
		}
		
		function ShowErrorMsg(strMsg)
		{
			alert(strMsg);
		}
		
		function ShowbtnDelete()
		{
			//CheckBox被選取後顯示刪除按鈕
			if(document.getElementById("btnDeleteQuestion") != null)
			{
				document.getElementById("btnDeleteQuestion").style.display = "";
			}
		}
		
		function ShowbtnModify()
		{
			//TextBox被修改後顯示修改按鈕
			if(document.getElementById("btnModifyQuestion") != null)
			{
				document.getElementById("btnModifyQuestion").style.display = "";
			}
		}
		
		function clickModifyButton()
		{
			if(document.getElementById("btnModifyQuestion") != null)
			{
				document.getElementById("btnModifyQuestion").click();
			}
		}
		
		function showQuestionTitle(strQID)
		{
			var strTitleID = "txtTitle" + strQID;
				
			if(document.getElementById(strTitleID) != null)
			{
				if(document.getElementById(strTitleID).style.display == "none")
				{
					document.getElementById(strTitleID).style.display = "";
				}
				else
				{
					document.getElementById(strTitleID).style.display = "none";
				}
			}
		}
		</script>
	</head>
    <script type="text/javascript">

        //set current URL to a session to allow next page to redirect back. 
        function LatchPreviousPage() {
            //set current URL to a session to allow next page to redirect back. 
            $.ajax({
                url: "Paper_MainPage.aspx/LatchPreviousPage",
                data: '{PreviousPageURL: "' + window.location.href + '" }',
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (result) {


                }
            });
        }


        //下一步(新增題目)
		function goNext()
		{
			if(document.getElementById("rb1") != null && document.getElementById("rb2") != null)
			{
				if(document.getElementById("rb1").checked == true)
				{
					//從ORCS的課堂練習頁面接收參數
				    if (document.getElementById("hiddenOpener").value == "SelectPaperMode")
				    {
					    location.href = "../../../PushMessage/MessaeMember.aspx?Opener=Paper_MainPage";
					}
					else    //從原本HINTS的Case頁面接收參數
					{
					    alert('Save successfully!');
					    //判斷是否由pe編輯問題來 是則儲存後關閉室窗 否則照原流程走
					    var varPaperPurpose = document.getElementById("hfPaperPurpose").value;
					    if(varPaperPurpose == "PEQuestion")
					        window.close();
                        else
					        window.location.href = document.getElementById("hfPageUrl").value;
					}
				}
				else if(document.getElementById("rb2").checked == true)
				{
                    //set current URL to a session to allow next page to redirect back. 
				    LatchPreviousPage();
				    
                   
				           
                    
				    //從ORCS的課堂練習頁面接收參數
				    if (document.getElementById("hiddenOpener").value == "SelectPaperMode")
				        location.href = "Paper_QuestionTypeNew.aspx?Opener=Paper_MainPage" ;
				    else    //從原本HINTS的Case頁面接收參數
				        location.href = "Paper_EditMethod.aspx?Opener=Paper_MainPage";
                        
                    

				   
				}
			}
			
		}
    </script>
	<body">
		<form id="Form1" method="post" runat="server">
			<table id="body_content" style="WIDTH: 99%;" align="center" runat="server">
				<tr id="trFunctionList">
					<td id="tcFunctionList" align="center">
						<span id="spanFunctionList" class="Title">
							編輯或選擇此考卷題目</span>&nbsp;
						<hr>
					</td>
				</tr>
				<tr id="trQuestionTitle" runat="server">
					<td id="tcQuestionTitle" runat="server" class="header1_table_first_row">
						Title <textarea id="txtPaperTitle" name="txtPaperTitle" style="WIDTH: 100%" rows="5" runat="server"></textarea>
					</td>
				</tr>
				<tr>
					<td style="HEIGHT:10px"><hr>
					</td>
				</tr>
				<tr id="trSetScoreControl" runat="server" valign>
				    <td>
                        <asp:Button ID="btnAutoSetScore" runat="server" Text="Auto Set Score" 
                            class="button_blue" onclick="btnAutoSetScore_Click"/>
                        &nbsp;&nbsp;
                        <asp:Label ID="lblManualSetScore" runat="server" Text="設定考卷總分："></asp:Label>
				        <asp:TextBox ID="txtManualSetScore" runat="server">100</asp:TextBox>
                        &nbsp;&nbsp;
                        <asp:Label ID="lbMaximumNumberOfWordsReasons" runat="server" Text="設定學生答選擇題含理由時，理由填寫最多字數："/>
                        <asp:TextBox ID="tbMaximumNumberOfWordsReasons" runat="server" Text="150" OnTextChanged="tbMaximumNumberOfWordsReasons_TextChanged" AutoPostBack="true" ></asp:TextBox>
				    </td>
				</tr>
				<tr id="trQuestionTable" runat="server">
					<td id="tcQuestionTable" runat="server"></td>
				</tr>
				<tr>
                    <td>
                        <asp:Label ID="lblTotal" runat="server" Text="目前總分為："></asp:Label>
                        <asp:Label ID="lblTotalScore" runat="server" Text="0"></asp:Label>
                    </td>
				</tr>
				<tr>
					<td style="HEIGHT: 20px"><hr>
					</td>
				</tr>
				<tr id="trQuestionNumTable" runat="server">
					<td id="tcQuestionNumTable" runat="server"></td>
				</tr>
				<tr style="DISPLAY: none">
					<td align="right">
						<input id="btnModifyQuestion" style="DISPLAY: none; CURSOR: hand" type="button" value="Modify"
							name="btnModifyQuestion" runat="server">&nbsp;&nbsp;&nbsp; 
						<input id="btnDeleteQuestion" style="DISPLAY: none; CURSOR: hand" type="button" value="Delete"
							name="btnDeleteQuestion" runat="server">
					</td>
				</tr>
				<tr>
					<td align="center">
						<HR>
						<table style="DISPLAY: none">
							<tr>
								<td align="left"><span id="spanFunction" class="title">Please 
										select a function:</span><br>
								</td>
							</tr>
							<tr id="tr1" align="left" runat="server">
								<td class="subtitle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id="rb2" type="radio" CHECKED value="rb2" name="rb" runat="server" onclick="document.getElementById('btnFinish').value = 'Next'">Advanced 
									setting
								</td>
								
							</tr>
							<tr id="tr2" align="left" runat="server">
								<td class="subtitle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<input id="rb1" type="radio" value="rb1" name="rb" runat="server" onclick="document.getElementById('btnFinish').value = 'Finish'">Save 
									and return
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td align="right">
					    <input id="btnAddNewQues" style="WIDTH: 300px; HEIGHT: 30px" class="button_continue" type="button"
							value="Add A New Question Into DataBase" name="btnAddNewQues" runat="server" onserverclick="btnAddNewQues_onserverclick"/>&nbsp;&nbsp;&nbsp;&nbsp;
						<input id="btnSelectExistQues" style="WIDTH: 360px; HEIGHT: 30px" class="button_continue" onclick="document.getElementById('rb2').checked = true; clickModifyButton();" type="button"
							value="Select Existing Questions Into Test Paper" name="btnSelectExistQues"/>&nbsp;&nbsp;&nbsp;&nbsp;
						<input id="btnAdvSet" style="WIDTH: 155px; HEIGHT: 30px;" class="button_continue" onclick="AdvancedSetting();" type="button"
							value="Advanced Setting" name="btnAdvSet"/>&nbsp;&nbsp;&nbsp;&nbsp;
					    <input id="btnFinish" style="WIDTH: 150px; HEIGHT: 30px" class="button_continue" onclick="document.getElementById('rb1').checked = true; clickModifyButton();" type="button"
							value="Finish" name="btnFinish" runat="server"/>
					    <input id="btnFinish3" runat="server" style="WIDTH: 150px; HEIGHT: 30px" class="button_continue"  onserverclick="btnFinish3_onserverclick" type="button"
							value="Finish" name="btnFinish2" visible="false" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <input id="btnFinish2" runat="server" style="WIDTH: 150px; HEIGHT: 30px;display:none;" class="button_continue"  onserverclick="btnFinish2_onserverclick" type="button"
							value="Finish" name="btnFinish3" />
						<input id="btnPre" style="WIDTH: 150px; HEIGHT: 30px" class="button_continue" type="button"
							value="Back" name="btnPre" runat="server"/>
                        <asp:Button ID="btnRefresh" Text="Refresh" runat="server" OnClick="btnRefresh_Click" style="display:none;" />
					</td>
				</tr>
			</table>
			<div style="POSITION: absolute; TOP: 600px">
				<input id="hiddenOpener" type="hidden" name="hiddenOpener" value="" runat="server"/> <input id="hiddenQuestionMode" type="hidden" name="hiddenQuestionMode" runat="server"/>
				<input id="hiddenPresentType" type="hidden" name="hiddenPresentType" runat="server"/>
				<input id="hiddenEditMode" type="hidden" name="hiddenEditMode" runat="server"/>
                <input id="hiddenbIsFromSelectExistPaper" type="hidden" name="hiddenbIsFromSelectExistPaper" runat="server"/>
                <input id="hiddenIsAvgScoreToZeroScore" type="hidden" name="hiddenIsAvgScoreToZeroScore" value="false" runat="server"/>
			</div>
        <asp:HiddenField ID="hfPageUrl" runat="server" />
        <asp:HiddenField ID="hfPaperPurpose" runat="server" />
		</form>
	</body>
</html>
