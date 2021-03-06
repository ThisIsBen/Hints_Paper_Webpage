﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Paper_EmulationQuestion.aspx.cs" Inherits="AuthoringTool_CaseEditor_Paper_Paper_EmulationQuestion" %>
<%@ Reference Page="~/AuthoringTool/CaseEditor/Paper/CommonQuestionEdit/Page/EntryDiagnosisAndQuestion.aspx" %>
<%@ Reference Page="~/AuthoringTool/CaseEditor/Paper/CommonQuestionEdit/Page/remotingscripting.aspx" %>
<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <title></title>

     <!--<meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link href="~/bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen">
        <script src="http://code.jquery.com/jquery.js"></script>
        <script src="~/bootstrap/js/bootstrap.min.js"></script>-->

       <%-- <link rel="stylesheet" href="/../code.jquery.com/ui/1.11.2/themes/smoothness/jquery-ui.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.11.2/jquery-ui.js"></script>
  <link rel="stylesheet" href="/resources/demos/style.css">--%>
  <script src="../../../Scripts/jquery-1.11.2.min.js"></script>
  <script src="../../../Scripts/CSSExtractor.js"></script>

  <script>
      $(function () {
          $("#divSetAnswerType").draggable();
      });
  </script>

    <style type="text/css">
        .style1
        {
            width: 100%;
        }
        #btnNext
        {
            height: 26px;
        }
    </style>
</head>
<body onload="mname()" onclick="onTop()" onUnload="closeb()">
<script language="javascript">

    var newwin = null;

    function mname() {
        self.opener = 'aa'; 
    }

    function showVMSelectMenuWindow() {
        newwin = window.open("../../../AuthoringTool/CaseEditor/VirtualMicroscope/VRQuestionSceneSelected.aspx", "full", "fullscreen, resizable = yes , scrollbars=yes");
    }

    function showVMWindow() {
        newwin = window.open("../../../AuthoringTool/CaseEditor/VirtualMicroscope/VMSimulatorAuthoring.aspx", "full", "fullscreen, resizable = yes");
    }

    function showConceptMapWindow(userid, Qid) {
        //須將下方的IP位置改成當server的電腦IP，因為屬於不同專案 
        newwin = window.open("http://140.116.72.28/shape/Hubs/ShapeShare/Dragflow.aspx?userId=" + userid + "&groupID=VR&chairman=2&name=" + userid + "&QID=" + Qid, "full", "fullscreen, resizable = yes");
    }

    function showSetLearningPage() {
        newwin = window.open("setLearningPoint.aspx", "full", "fullscreen, resizable = yes");
    }

    function showSetTextBookPage() {
        newwin = window.open("SetVRTextBook.aspx", "full", "fullscreen, resizable = yes");
    }


    function onTop() {
        if (newwin != null && newwin.open) {
            newwin.focus(); 
        }
    }

    function closeb() {
        if (newwin != null && newwin.open) {
            newwin.close();
            newwin = null;
        }
    }

   

    function goBack() {

       



       
        if (window.confirm("您的資料將不會被儲存，您確定要繼續嗎?")) {
            var strQuestionMode = "";
            if (document.getElementById("hiddenQuestionMode") != null) {
                strQuestionMode = document.getElementById("hiddenQuestionMode").value;
            }
            var strModifyType = "";
            if (document.getElementById("hiddenModifyType") != null) {
                strModifyType = document.getElementById("hiddenModifyType").value;
            }
            var strBModify = "";
            if (document.getElementById("hiddenBModify") != null) {
                strBModify = document.getElementById("hiddenBModify").value;
            }
            var strOpener = "";
            if (document.getElementById("hiddenOpener") != null) {
                //Ben check
                //產生新題目流程中的回上一頁機制 若上一頁為Paper_QuestionTypeNew，則直接返回該頁
                var BackUrl = document.getElementById("hiddenOpener").value;
               
                if (BackUrl == "Paper_QuestionTypeNew") {
                   
                    var varGroupID = document.getElementById("hfGroupID").value;
                    strOpener = BackUrl + ".aspx?Opener=./QuestionGroupTree/QGroupTreeNew&bModify=False&GroupID=" + varGroupID;

                    //直接返回Paper_QuestionTypeNew頁
                   
                    window.location.href = strOpener;
                }

                else
                    strOpener = BackUrl;


                //strOpener = document.getElementById("hiddenOpener").value;
            }

            else if (strModifyType == "Paper") {
                //編輯考卷
                if (strBModify == "True") {
                    //修改題目
                    if (document.getElementById("hiddenPreOpener").value == "SelectPaperMode")
                        location.href = "Paper_MainPage.aspx?Opener=SelectPaperMode&cCaseID=" + document.getElementById("hiddenCaseID").value + "&cSectionName=" + document.getElementById("hiddenSectionName").value + "&cPaperID=" + document.getElementById("hiddenPaperID").value;
                    else
                        location.href = "Paper_MainPage.aspx";
                }
                else {
                    //新增問題
                    if (strQuestionMode == "General") {
                        //General問題編輯模式
                        if (document.getElementById("hiddenPreOpener").value == "SelectPaperMode")
                            location.href = "Paper_MainPage.aspx?Opener=SelectPaperMode&cCaseID=" + document.getElementById("hiddenCaseID").value + "&cSectionName=" + document.getElementById("hiddenSectionName").value + "&cPaperID=" + document.getElementById("hiddenPaperID").value;
                        else
                            location.href = "Paper_MainPage.aspx";
                    }
                    else {
                        //Specific問題編輯模式
                        if (strOpener == "Paper_QuestionMode") {
                            location.href = "Paper_QuestionMode.aspx";
                        }
                        else {
                            
                            location.href = "Paper_OtherQuestion.aspx";
                        }
                    }
                }
            }
            else {

                alert("in paper question view");

                var varGroupID = document.getElementById("hfGroupID").value;
                //編輯題目
                if (strBModify == "True") {
                    //修改題目				
                    location.href = "Paper_QuestionViewNew.aspx?GroupID=" + varGroupID;
                }
                else {
                    //新增問題
                    location.href = "Paper_QuestionViewNew.aspx?GroupID=" + varGroupID;
                }
            }
            
        }
    }

 
	function saveQuestion() {
				if(document.getElementById("finishBtn") != null)
				{
					document.getElementById("finishBtn").click();
				}
			}
			
 
 </script>

    <script type="text/javascript">
//        window.onload = function () {
//            var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
//            document.getElementById("<%=scrollArea.ClientID%>").scrollTop = h.value;
//  }
//  function SetDivPosition() {
//      var intY = document.getElementById("<%=scrollArea.ClientID%>").scrollTop; var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
//            h.value = intY;
//        }

//        function afterpostback() {
//            var h = document.getElementById("<%=hfScrollPosition.ClientID%>");
//            document.getElementById("<%=scrollArea.ClientID%>").scrollTop = h.value;
//        }
  </script>
    
    <form id="form1" runat="server">
        
        <asp:HiddenField ID="hfScrollPosition" runat="server" Value ="0" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
    <input id="hiddenOpener" type="hidden" name="hiddenOpener" runat="server">
    <input id="hiddenQuestionMode" type="hidden" name="hiddenQuestionMode" runat="server">
    <input id="hiddenModifyType" type="hidden" name="hiddenModifyType" runat="server">
    <input id="hiddenBModify" type="hidden" name="hiddenBModify" runat="server">
    <input id="hiddenGroupID" type="hidden" name="hiddenGroupID" runat="server">
    <input id="hiddenCaseID" type="hidden" name="hiddenCaseID" value="" runat="server">
    <input id="hiddenSectionName" type="hidden" name="hiddenSectionName" value="" runat="server">
    <input id="hiddenPaperID" type="hidden" name="hiddenPaperID" value="" runat="server">
    <input id="hiddenPreOpener" type="hidden" name="hiddenPreOpener" value="" runat="server">
    <asp:HiddenField ID="hfGroupID" runat="server" />
        
    </div>
        
        <div id="scrollArea" onscroll="SetDivPosition()"   runat="server" style="overflow:auto;overflow-x:hidden;">
            
            <table id='body_content'>
            <tr>
                <td class="title">
                    <asp:Label ID="lbQuestionTittle" runat="server" Text="Edit Emulation Question" ></asp:Label>
                </td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
                 &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2" style="border: 2px groove #000000;">
                    <table class="style1">
                        <tr>
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="lbQuestionText" runat="server" Text="Question" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                            :</td>
                                        <td style="border-style: inset; border-width: 2px; height: 150px;">
                                            <asp:TextBox ID="txtQuestion" runat="server" Width="100%" Height="150px" 
                                                TextMode="MultiLine"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="lbDescriptionTittle" runat="server" Text="Description:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="border-style: inset; border-width: 2px; height: 50px;">
                                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" 
                                                Width="100%" Height="50"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                                                                      
                        <tr>
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                           <asp:Label ID="lbVMQuestionTittle" runat="server" Text="Edit VR Scene:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label></td>
                                        <td style="border-style: inset; border-width: 2px; height: 30px;">
                                            <asp:Button ID="btnVMQuestion" runat="server" 
                                                Text="Edit VitualReality Scene" Height="30px" class="button_blue"
                                                Width="100%" onclick="btnVMQuestion_Click" />
                                                </td>
                                    </tr>
                                </table>
                            </td>
                            
                        </tr>
                        <tr id="trAnswerType" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="lbMemoryFunction" runat="server" Text="Answer Type:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="border-style: inset; border-width: 2px; height: 50px;">
                                                <asp:RadioButton ID="rbNone" Text="None" runat="server" GroupName="record" Checked="false" Visible ="false"
                                                    oncheckedchanged="rb_CheckedChanged" AutoPostBack=true/>&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp
                                                <asp:RadioButton ID="rbRecordStep" runat="server" Text="步驟題" 
                                                    GroupName="record" Checked="true" oncheckedchanged="rb_CheckedChanged" AutoPostBack=true/><br />&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp
                                                <asp:RadioButton ID="rbEssayQuestion" runat="server" Text="申論題" 
                                                    GroupName="record" Checked="false" oncheckedchanged="rb_CheckedChanged" AutoPostBack=true/><br />&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp
                                                <asp:RadioButton ID="rbCircleItems" runat="server" Text="圈選題"
                                                    GroupName="record" Checked="false" oncheckedchanged="rb_circleCheckedChanged" AutoPostBack=true/>&nbsp;&nbsp;&nbsp;
                                            <asp:RadioButton ID="rbNormal" runat="server" Text="一般型"
                                                    GroupName="circleType" Checked="true"  Visible="false" AutoPostBack=true/>&nbsp;&nbsp;&nbsp;
                                            <asp:RadioButton ID="rbRange" runat="server" Text="範圍型"
                                                    GroupName="circleType" Checked="false"  Visible="false" AutoPostBack=true/>&nbsp;&nbsp;&nbsp;
                                            <asp:RadioButton ID="rbSelect" runat="server" Text="選擇型"
                                                    GroupName="circleType" Checked="false" Visible="false" AutoPostBack=true/>&nbsp;&nbsp;&nbsp;

                                        </td>
                                    </tr>
                                </table>
                            </td>        
                        </tr>
                        <tr id="trSetAns" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="lblSetAnswer" runat="server" Text="Set Answer:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="width:40%; border-style: inset; border-width: 2px; text-align: center;">
                                                <asp:Button ID="btnSetAnswer" runat="server" 
                                                Text="Set Answer" Height="30px" class="button_blue"
                                                Width="100%" onclick="btnVMQuestion_Click"/>
                                        </td>
                                         <td valign=bottom align=left > 
                                            <font color="ff0000" size="2">*設定參考解答</font>
                                         </td>                                       
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>
                        <tr id="trSetLearningPoint" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="lbLearningPointer" runat="server" Text="Set Learning Pointer:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="width:40%; border-style: inset; border-width: 2px; text-align: center;">
                                                <asp:Button ID="btnLearningPointer" runat="server" 
                                                Text="Set Learning Points" Height="30px" class="button_blue"
                                                Width="100%" onclick="btnLearningPointer_Click"/>
                                        </td>  
                                         <td valign=bottom align=left > 
                                            <font color="ff0000" size="2">*設定答案學習重點與權重</font>
                                         </td>                                
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>

                        <tr id="trsetTextBook" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="Label1" runat="server" Text="Set TextBook:" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="width:40%; border-style: inset; border-width: 2px; text-align: center;">
                                                <asp:Button ID="btnTextBook" runat="server" 
                                                Text="Set TextBook" Height="30px" class="button_blue"
                                                Width="100%" onclick="btnTextBook_Click"/>
                                        </td>  
                                         <td valign=bottom align=left > 
                                            <font color="ff0000" size="2">*設定在「學習模式」、「練習模式」的說明教材</font>
                                         </td>                                
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>


                        <tr id="trSetSensitivity" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="Label3" runat="server" Text="Set Sensitivity :" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="width:40%; border-style: inset; border-width: 2px; text-align: left;">
                                            <!--<asp:RadioButton ID="rbtSensitivityYes" runat="server" Text="學生自行設定"  Checked="false" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large" OnCheckedChanged="rbtSensitivityYes_Click"  AutoPostBack="true" />
                                            &nbsp;&nbsp;&nbsp
                                            <asp:RadioButton ID="rbtSensitivityNo" runat="server" Text="教師設定"  Font-Bold="True"  Checked="false"
                                                ForeColor="Blue" Font-Size="Large" OnCheckedChanged="rbtSensitivityNo_Click" AutoPostBack="true" />&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp-->

                                            
                                            <asp:Label ID="Label11" runat="server" Text="幫助學習靈敏度" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>&nbsp;&nbsp;&nbsp&nbsp;&nbsp;&nbsp
                                            <asp:DropDownList ID="ddlSensitivity" Width="50" runat="server"  OnSelectedIndexChanged="ddlRight_Click" AutoPostBack="true">
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                                <asp:ListItem>4</asp:ListItem>
                                                <asp:ListItem>5</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp
                                           <!-- <asp:Label ID="Label10" runat="server" Text="提醒" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large" Enabled="true"></asp:Label>
                                            <asp:DropDownList ID="ddlHintSensitivity" runat="server"  OnSelectedIndexChanged="ddlRight_Click" AutoPostBack="true">
                                                <asp:ListItem>1</asp:ListItem>
                                                <asp:ListItem>2</asp:ListItem>
                                                <asp:ListItem>3</asp:ListItem>
                                            </asp:DropDownList>
                                            &nbsp;&nbsp;&nbsp&nbsp;
                                            <asp:Label ID="Label4" runat="server" Text="的靈敏度" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>-->
                                        </td>  
                                         <td valign=bottom align=left > 
                                            <font color="ff0000" size="2">*設定在「練習模式」中學生答題錯誤量到達何值會出現「小精靈」給予學生幫助</font>
                                         </td>                                
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>


                        <tr id="trMode" runat="server">
                            <td style="border: 2px groove #800080">
                                <table class="style1">
                                    <tr>
                                        <td style="width:15%; border-style: inset; border-width: 2px; text-align: center;">
                                            <asp:Label ID="Label2" runat="server" Text="Set Student's Right :" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"></asp:Label>
                                        </td>
                                        <td style="width:40%; border-style: inset; border-width: 2px; text-align: left;">
                                            <asp:RadioButton ID="rbtYes" runat="server" Text="學習模式"  Checked="true" GroupName="mode" Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large"  OnCheckedChanged="rbtRightYes_Click" AutoPostBack=true />
                                            <br />
                                            <asp:RadioButton ID="rbtNo" runat="server" Text="練習模式"  Font-Bold="True" AutoPostBack =true 
                                                ForeColor="Blue" Font-Size="Large"  GroupName="mode" OnCheckedChanged="rbtRightNo_Click" Checked="false"/>
                                            &nbsp;&nbsp;&nbsp
                                            
                                            <%--<asp:CheckBox ID="cbxText" runat="server" Text="說明教材" Enabled="false"  Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large" OnCheckedChanged="cbxRight_Click" AutoPostBack="true"/>
                                            &nbsp;&nbsp;
                                            <asp:CheckBox ID="cbxSuggest" runat="server" Text="暗示教材" Enabled="false"  Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large" OnCheckedChanged="cbxRight_Click" AutoPostBack="true"/>
                                            <asp:CheckBox ID="cbxHint" runat="server" Text="提醒" Enabled="false"  Font-Bold="True" 
                                                ForeColor="Blue" Font-Size="Large" OnCheckedChanged="cbxRight_Click" AutoPostBack="true"/>--%>
                                            
                                        </td>  
                                         <td valign=bottom align=left > 
                                            <font color="ff0000" size="2">*設定學生在作答時所顯示的模式。學習模式:學習及練習模式均會出現；練習模式：僅出現練習模式。</font>
                                         </td>                                
                                    </tr>
                                    
                                </table>
                            </td>
                        </tr>


                        </table>
                </td>
            </tr>
            </table>
            &nbsp;
           
        <table width="100%">
        <tr>
        <td> 
        <asp:Panel ID="PanelFeature" runat="server">
        </asp:Panel>
        </td>
        <tr>
        </table>
       
        <table width="100%">
            <tr>
                <td align="right">&nbsp;
                    <input ID="btnBack" class="button_continue" name="btnBack" onclick="goBack()" 
                        style="width: 150px; cursor: hand; height: 30px" type="button" 
                        value="&lt;&lt; Quit" />
                   <%-- <asp:Button ID="btnTrBack" runat="server" class="button_continue" 
                        onclick="btnTrBack_Click" style="width: 150px; cursor: hand; height: 30px" 
                        Text="&lt;&lt; Back" />--%>
                    <asp:Button ID="btnTrNext" runat="server" class="button_continue" 
                        onclick="btnTrNext_Click" style="width: 150px; cursor: hand; height: 30px" 
                        Text="Next &gt;&gt; " />
                    <asp:Button ID="btnFinish" runat="server" class="button_continue" 
                        onclick="btnFinish_Click" style="width: 150px;  height: 30px" 
                        Text="Finish" />
                </td>
            </tr>
        </table>
        </div>
        
        <asp:HiddenField ID="HiddenFieldTrStatusValue" runat="server" Value="0"/>

        
        <!--選擇設定答案的方式-->
    <div id="divSetAnswerType" class="modal-content"  
            style="border: 1px groove #000000; background-color: #FFFFFF; display:none;   position: absolute; z-index: 102; left:60%; top:75%; margin-left: -475px; margin-top: -400px; font-family: 'Microsoft YaHei';" 
            runat="server">
      <div class="modal-content">
     
          <div class="modal-header" style="border-bottom-style: groove; border-bottom-width: thin; border-bottom-color: #000000; height: 40px; background-color: #3366CC; vertical-align: middle; text-align: center; padding-top: 10px;">
               <asp:Label ID="Label5" runat="server" Text="選擇設定答案的方式" Font-Bold="True" Font-Size="X-Large" ForeColor="White"></asp:Label>
          </div>
         
      
          <div class="modal-body" style="margin-top: 20px;">
              <table  class="table table-hover" style=" border-width: 1px; border-color: #C0C0C0; padding: 20px; margin: 20px; border-top-style: groove; border-bottom-style: groove;" width="500px">
                  <tr>
                      <td>
                          <asp:LinkButton ID="LinkButton1" runat="server" Font-Size="Larger" OnClick="lbtnConceptMapEdit_Click" >使用Concept Map編輯</asp:LinkButton>
                      </td>
                  </tr>

                  <tr>
                      <td>
                          <hr />
                          <asp:Label ID="Label6" runat="server" Text="選取「使用Concept Map編輯」可以利用繪製Concept Map的方式來編輯情境題的標準答案。" ForeColor="Gray"></asp:Label>
                      </td>
                      
                  </tr>
                  <tr>
                      <td style="margin-top: 40px; padding-top: 60px;">
                          <asp:LinkButton ID="LinkButton2" runat="server" Font-Size="Larger" OnClick="lbtnSituationalEdit_Click" >使用場景編輯</asp:LinkButton>
                      </td>
                  </tr>

                  <tr>
                      <td>
                           <hr />
                          <asp:Label ID="Label7" runat="server" Text="選取「使用場景編輯」將以到場景中點擊註記的方式來編設標準答案，即在場景中所點的每個步驟將成為標準答案。" ForeColor="Gray"></asp:Label>
                      </td>
                      
                  </tr>
              </table>
          </div>
      
          <div class="modal-footer" style="vertical-align: bottom; text-align: right; padding-bottom: 20px; padding-right: 20px;">
              <asp:Button ID="Button1" runat="server" Text="取消" CssClass="button_continue" Font-Size="Large" OnClick="btSetAnswerTypeCancel_Click"  />
          </div>
          
    
        </div>
    </div>


         <!--尚未使用Concept Map編輯過答案-->
    <div id="divIsUseConcept" class="modal-content"  
            style="border: 1px groove #000000; background-color: #FFFFFF; display:none;   position: absolute; z-index: 102; left:60%; top:80%; margin-left: -475px; margin-top: -400px; font-family: 'Microsoft YaHei';" 
            runat="server">
      <div class="modal-content">
     
          <div class="modal-header" style="border-bottom-style: groove; border-bottom-width: thin; border-bottom-color: #000000; height: 40px; background-color: #FF0000; vertical-align: middle; text-align: center; padding-top: 10px;">
               <asp:Label ID="Label8" runat="server" Text="尚未使用Concept Map 編設過答案" Font-Bold="True" Font-Size="X-Large" ForeColor="White"></asp:Label>
          </div>
         
      
          <div class="modal-body" style="margin-top: 20px;">
              <table  class="table table-hover" style=" border-width: 1px; border-color: #C0C0C0; padding: 20px; margin: 20px; border-top-style: groove; border-bottom-style: groove;" width="500px">
                 
                  <tr>
                      <td>
                          <asp:Label ID="Label9" runat="server" Text="*該情境題尚未使用Concept Map編設過答案，是否直接使用場景編設答案 ?" ForeColor="Red"></asp:Label>
                      </td>
                      
                  </tr> 
              </table>
          </div>
      
          <div class="modal-footer" style="vertical-align: bottom; text-align: right; padding-bottom: 20px; padding-right: 20px;">
              <asp:Button ID="Button3" runat="server" Text="是"  Font-Size="Large"　width ="100px"   BackColor="Red" ForeColor="White" OnClick="btnUseSituationalEditOK_Click" />
              <asp:Button ID="Button2" runat="server" Text="否"  Font-Size="Large" width ="100px"   BackColor="#0066FF" ForeColor="White" OnClick="btnUseSituationalEditNO_Click" />
          </div>
          
    
        </div>
    </div>

    </form>
</body>
</html>
