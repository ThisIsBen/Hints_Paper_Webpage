<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>

<%@ Page Language="C#" EnableEventValidation="false" AutoEventWireup="true" CodeFile="QGroupTreeNew.aspx.cs" Inherits="AuthoringTool_CaseEditor_Paper_QuestionGroupTree_QGroupTreeNew" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">

    <title>Question Group Tree</title>
    <link href="../../../../bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../../../../Scripts/jquery-1.11.2.min.js"></script>
    <script src="../../../../Scripts/bootstrap.min.js"></script>

   
   
    <script type="text/javascript">

        function openModal() {
            $('#modal_featureSearch').modal('show');
        }


        function displayFeatureSearchOraddQuestion() {
            var url = new URL(window.location.href);
            var Opener = url.searchParams.get("Opener");


            //if user wants to close the 'Feature Search or Display all questions in the problem database' modal
            //,just close this tab
            $('#BT_CancelP').click(function () {
                //window.location.href = '/Project/GroupProject.aspx';
                $('#modal_featureSearch').modal('hide');
            });



            //hide submit button when adding a new question to problem database(題庫)
            if (Opener == "Paper_QuestionType") {
                $(".addOrModifyQuestion").hide();
                //$("#modal_featureSearch").css("text-align", "center");
                userGuide = "Please select a problem database below to search exisiting questions in it.";
                document.getElementById('userGuide').innerText = userGuide;

            }


                //hide feaure search button when adding a new question to problem database(題庫)
            else if (Opener == null || Opener == "SelectPaperModeAddANewQuestion") {
                $(".addOrModifyQuestion").css("text-align", "left");
                $("#modal_featureSearch").hide();
                userGuide = "Please select a problem database below to set up a new question in it.";
                document.getElementById('userGuide').innerText = userGuide;

            }
        }
        $(document).ready(function () {

            //use URL parameter Opener as an indicator to decide displaying the input fields for Feature Search or Add/ModifyQuestion. 
            displayFeatureSearchOraddQuestion();

            currentNodeID = document.getElementById('hiddenQuestionBankName').value;
            document.getElementById('selectedProblemDatabase').innerText = currentNodeID;



            //make Feature Search modal draggable
            //$('#modal_featureSearch').draggable();


        });



        var currentNodeID = "";
        var oldNodeID = "";

        function SelectNode(nodeID, nodeAuthor) {
            window.status = "Current selected node: " + nodeID;
            currentNodeID = nodeID;
            document.getElementById("strCurrentNodeID").value = currentNodeID;

            if (oldNodeID != "")
                document.getElementById(oldNodeID).className = "";

            document.getElementById("display_" + nodeID).className = "ItemMouseOver";
            oldNodeID = "display_" + nodeID;

            selected_changed();
            document.getElementById("tdNodeID").innerText = currentNodeID;
        }

        function AddGroup() {
            currentNodeID = document.getElementById('hiddenQuestionBankID').value;
            if (currentNodeID != "") {
                showDisplay("divAddGroup");
                //$('#modal_featureSearch').show();

            }
            else {
                alert("Please select a node first.");
            }
        }
        function ModifyGroup() {
            currentNodeID = document.getElementById('hiddenQuestionBankID').value;
            if (currentNodeID != "") {
                showDisplay("divModifyGroup");
                //$('#modal_featureSearch').show();

            }
            else {
                alert("Please select a node first.");
            }
        }
        function showDisplay(id) {
            document.getElementById(id).style.display = "";
        }
        function hideDisplay(id) {
            currentOperation = 4;   // select operation
            document.getElementById(id).style.display = "none";
        }

        function confirm_delete() {
            if (!window.confirm("Do you want to delete this node?")) {
                event.returnValue = false;
                return false;
            }
            else {
                oldNodeID = "";
            }
        }

        function OpenSearchResult() {
            //alert("hi ghost")
            var varSearchKeyword = document.getElementById("tbSearchQuestionKeyword").value;
            //alert("varSearchKeyword");
            window.open("SearchQuestionResult.aspx?SearchType=1&SearchKeyword=" + varSearchKeyword + "", '_blank', 'directories=0, height=600, menubar=0, resizable=1, scrollbars=1, status=0, titlebar=1, toolbar=0, width=1000');
        }
        function OpenFeatureSearchResult() {
            window.open("SearchQuestionResult.aspx?SearchType=2", '_blank', 'directories=0, height=600, menubar=0, resizable=1, scrollbars=1, status=0, titlebar=1, toolbar=0, width=1000');
        }

        var mouseX, mouseY; var objX, objY; var isDowm = false; var ClickID = "";  //是否按下鼠标  
        function mouseDown(obj, e) {
            ClickID = obj.id;
            obj.style.cursor = "move";
            if (ClickID == "divAddGroup") {
                objX = divAddGroup.style.left;
                objY = divAddGroup.style.top;
            }
            else {
                objX = divModifyGroup.style.left;
                objY = divModifyGroup.style.top;
            }
            mouseX = e.clientX;
            mouseY = e.clientY;
            isDowm = true;
        }
        function mouseMove(e) {
            var div = null;
            if (ClickID == "divAddGroup")
                div = document.getElementById("divAddGroup");
            else
                div = document.getElementById("divModifyGroup");
            var x = e.clientX;
            var y = e.clientY;
            if (isDowm) {
                div.style.left = parseInt(objX) + parseInt(x) - parseInt(mouseX) + "px";
                div.style.top = parseInt(objY) + parseInt(y) - parseInt(mouseY) + "px";
            }
        }
        function mouseUp(e) {
            if (isDowm) {
                var x = e.clientX;
                var y = e.clientY;
                var div = null;
                if (ClickID == "divAddGroup")
                    div = document.getElementById("divAddGroup");
                else
                    div = document.getElementById("divModifyGroup");
                div.style.left = (parseInt(x) - parseInt(mouseX) + parseInt(objX)) + "px";
                div.style.top = (parseInt(y) - parseInt(mouseY) + parseInt(objY)) + "px";
                mouseX = x;
                rewmouseY = y;
                if (ClickID == "divAddGroup")
                    divAddGroup.style.cursor = "default";
                else
                    divModifyGroup.style.cursor = "default";
                isDowm = false;
            }
        }


        /*
        function LatchPreviousPage() {
           
            //set current URL to a session to allow next page to redirect back. 
            $.ajax({
                url: "../Paper_MainPage.aspx/LatchPreviousPage",
                data: '{PreviousPageURL: "' + window.location.href + '" }',
                dataType: "json",
                type: "POST",
                contentType: "application/json; charset=utf-8",
                async: false,
                success: function (result) {


                }
            });


        }
        */

        function pickSearchMethod() {

            if (document.getElementById('radio_featureSearch').checked) {
                document.getElementById('featureSearchPanel').style.display = 'block';
                document.getElementById('searchAllPanel').style.display = 'none';
                document.getElementById('div_searchAll').style.backgroundColor = 'orange';
                document.getElementById('div_featureSearch').style.backgroundColor = 'white';
                
                //Male radio button is checked
            } else if (document.getElementById('radio_SearchAll').checked) {
                //Female radio button is checked
                document.getElementById('searchAllPanel').style.display = 'block';
                document.getElementById('featureSearchPanel').style.display = 'none';
                document.getElementById('div_featureSearch').style.backgroundColor = 'red';
                document.getElementById('div_searchAll').style.backgroundColor = 'white';
            }
           // alert("hibb");
        }

        /*
        function mouseoverChangeDivBKC() {

            
            if (event.srcElement.id == 'div_featureSearch') {
                document.getElementById('div_featureSearch').style.backgroundColor = 'green';
                alert("in fS");
            }

            else if (event.srcElement.id == 'div_searchAll')
                document.getElementById('div_searchAll').style.backgroundColor = 'red';
        }



        function mouseoutChangeDivBKC() {

            
            document.getElementById(event.srcElement.id).style.backgroundColor = 'black';
        }


        document.getElementById('modal_searchMethod').onmouseover = function () { mouseoverChangeDivBKC() };
        document.getElementById('modal_searchMethod').onmouseout = function () { mouseoutChangeDivBKC() };
        */
    </script>

    <style type="text/css">
        .style1 {
            width: 100%;
        }

       

        .hideSearchPanel {
            display: none;
        }



       input[type=radio] {
    
            width: 100%;
            height: 2em;
        }
       .panel {
            border: 0;
        }
    </style>

</head>
<body onmousemove="mouseMove(event)" onmouseup="mouseUp(event)">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <table>
            <tr height="20">
                <td colspan="2" class="title">題庫群組選擇&nbsp;&nbsp;<asp:Label ID="lbQModeANDFunction" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr height="20">
                <td colspan="2">
                    <asp:Label ID="lbSelectQuestion" runat="server" Text="選擇何種題庫的題目："></asp:Label>
                </td>
            </tr>
            <tr height="20">
                <td colspan="2">
                    <asp:DropDownList ID="ddlSelectQuestion" Width="150" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSelectQuestion_SelectedIndexChanged">
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:Panel ID="Panel1" runat="server">
            <div style="display: none;">

                <script type="text/javascript">
                    var message = "";
                    function selected_changed() {
                        //alert(""+document.getElementById("_ctl0:MainDisplay:ddlProvider").value);
                        message = currentNodeID;
                    <%=sCallBackFunctionInvocation %>
                    }

                    function show_callback(msg, context) {
                        //document.getElementById("_ctl0:MainDisplay:tAuthor").value = msg;
                        //alert(msg);
                        document.getElementById("tdParentNodeName").innerText = msg;
                        document.getElementById("tNodeName").value = msg;
                    }


                    function checkPanelFeatureItem_Empty() {

                        var rowCountOfSelectedFeatureTable = $('#PanelFeatureItem > table tr').length;

                        if (rowCountOfSelectedFeatureTable < 1) {

                            alert("No feature is selected!\nPlease select at least one feature before search.");
                            return false;
                        }
                       

                    }
                </script>

                <input type="hidden" id="strCurrentNodeID" runat="server" value="" />
            </div>
            <div>

                <h3 id="userGuide"></h3>
            </div>

            <div id="divMain" align="left">
                <table id='body_content' height="85%">
                    <tr height="30">
                        <td width="44%">
                            <br />
                            <input type="button" id="btAddGroup" value="Add Children" onclick="AddGroup()" class="button_blue" />&nbsp;&nbsp;
                            <input type="button" id="btModifyGroup" value="Modify a node name" onclick="ModifyGroup()"
                                class="button_blue" />&nbsp;&nbsp;
                            <asp:Button ID="btDeletNodeSubmit" runat="server" Text="Delete a node" CssClass="button_blue"
                                OnClientClick="return confirm_delete()" OnClick="btDeletNodeSubmit_Click" />&nbsp;&nbsp;
                        </td>
                        <td style="display: none;">
                            <asp:Label ID="lbSearchQuestion" runat="server" Text="Enter the keyword for search questions"></asp:Label>
                            <br />
                            <asp:TextBox ID="tbSearchQuestionKeyword" runat="server" Width="80%"></asp:TextBox>&nbsp;
                            <input id="btSearch" type="button" value="Search" class="button_blue" onclick="OpenSearchResult()" />
                        </td>
                    </tr>
                    <tr>
                        <td align="left" valign="top">
                            <div style="overflow: auto; height: 100%;">
                                <asp:TreeView ID="tvQuestionGroup" runat="server" ShowLines="true" OnSelectedNodeChanged="tvQuestionGroup_SelectedNodeChanged">
                                    <SelectedNodeStyle BackColor="LightBlue" />
                                    <HoverNodeStyle BackColor="LightPink" />
                                    <RootNodeStyle ImageUrl="~/BasicForm/Image/diffFolder.gif" />
                                    <NodeStyle ImageUrl="~/BasicForm/Image/folderclosed.gif" ForeColor="Black" />
                                </asp:TreeView>
                            </div>
                            <asp:Button ID="Button1" runat="server" Text="Back" CssClass=" button_continue" OnClick="btBack_Click" />&nbsp;&nbsp;&nbsp;
                        </td>
                        <td>
                            <!--prompt user to select feature search query if he has picked a problem database!-->
                            <div id="modal_featureSearch" class="modal"
                                style="display: none;"
                                runat="server">
                                <div class="modal-dialog modal-lg">
                                  
                                    <div class="modal-content">
                                        <div class="panel panel-success">
                                       <%-- <div class="modal-header text-left">--%>
                                           
                                        <div class="panel-heading"><h3 class="modal-title">Use Feature Search to Search desired questions,<br />
                                                        or<br />
                                                        Display all the questions in the <b id="selectedProblemDatabase"></b> problem database</h3></div>
                                                    
                                               <%-- </div>--%>

                                                

                                        <div class="modal-body text-center " id="modal_searchMethod">

                                          
                                                <div class="  panel-body  " id="div_featureSearch">
                                                    <input type="radio" name="optradio" id="radio_SearchAll" onclick="pickSearchMethod()"><h3>List All Questions in the selected 題庫 without picking features：</h3>
                                               
                                                     <div id="searchAllPanel" class="hideSearchPanel">
                                                        <asp:Button ID="btnSearchALLQ_withoutPickingFeature" runat="server" Text="List All Questions" CssClass="btn btn-success" OnClick="btSubmit_Click" />
                                                    </div>
                                                </div>
                                                <hr>
                                                <div class=" panel-body  " id="div_searchAll">
                                                    <input type="radio" name="optradio" id="radio_featureSearch" onclick="pickSearchMethod()"><h3>Feature Search(特徵值搜尋)：</h3>
                                                </div>

                                         
                                            


                                           
                                            <div id="featureSearchPanel" class="hideSearchPanel" >
                                                <asp:Panel ID="SearchMethodPanel" runat="server" Height="200px">

                                                    <asp:UpdatePanel ID="uppanelFeatureItem" runat="server" UpdateMode="Conditional">
                                                        <ContentTemplate>
                                                            <asp:Panel ID="PanelFeatureItem" runat="server">
                                                            </asp:Panel>
                                                        </ContentTemplate>
                                                    </asp:UpdatePanel>
                                                </asp:Panel>

                                                <asp:UpdatePanel ID="upDDLFeatureSelect" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:DropDownList ID="DDLFeatureSet" runat="server" Font-Size="13pt"
                                                            OnSelectedIndexChanged="DDLFeatureSet_SelectedIndexChanged" AutoPostBack="True">
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="DDLFeatureItem" runat="server" Font-Size="13pt">
                                                        </asp:DropDownList>
                                                        <asp:DropDownList ID="DDLSearchMode" runat="server" Font-Size="13pt">
                                                        </asp:DropDownList>
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>


                                                <asp:UpdatePanel ID="upBtnFeature" runat="server" UpdateMode="Conditional">
                                                    <ContentTemplate>
                                                        <asp:Button ID="btnAddFeature" runat="server" class="button_blue" Text="加入特徵條件"
                                                            OnClick="btnAddFeature_Click" />
                                                        <asp:Button ID="btnFeatureDelete" runat="server" class="button_blue"
                                                            Text="取消" OnClick="btnFeatureDelete_Click" Visible="False" />
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                
                                                    </ContentTemplate>
                                                </asp:UpdatePanel>

                                                 <br />
                                                <asp:Button ID="btFeatureSearch" CssClass="btn btn-success " runat="server" OnClientClick="return checkPanelFeatureItem_Empty();" OnClick="btFeatureSearch_Click" Text="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Search&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"  />
                                               
                                            </div>
                                        </div>


                                        <div class="modal-footer text-center">
                                        <input type="button" id="BT_CancelP" value="Close" class="btn btn-danger btn-lg" />

                                    </div>
                                        </div>
                                    </div>
                                   
                                </div>
                   
            </div>
            <!--end copy group prompt!-->












            <!--end feature search prompt!-->

            </td>
                        <td>
                            <br />

                            <asp:Button ID="btSubmit" runat="server" Text="Set Up New Question In the Selected 題庫" CssClass="addOrModifyQuestion  button_continue" OnClick="btSubmit_Click" />
                        </td>

            </tr>

                </table>
            </div>
            <div id="divAddGroup" style="position: absolute; z-index: 101; width: 70%; display: none; left: 15%; top: 20%; background-color: #446699; text-align: center;"
                onmousedown="mouseDown(this,event)">
                <br />
                <table style="width: 100%; text-align: center">
                    <tr>
                        <td align="center">
                            <table width="100%">
                                <tr class="header1_table_first_row">
                                    <td colspan="2">Add children
                                    </td>
                                </tr>
                                <tr class="header1_tr_odd_row">
                                    <td width="20%">parent node
                                    </td>
                                    <td id="tdParentNodeName" runat="server" width="80%"></td>
                                </tr>
                                <tr class="header1_tr_even_row">
                                    <td colspan="2">
                                        <span style="color: red">Hints: If you want to add more siblings, please segment the children with commas in the textbox below.</span>
                                    </td>
                                </tr>
                                <tr class="header1_tr_odd_row">
                                    <td width="20%">new node name
                                    </td>
                                    <td width="80%">
                                        <asp:TextBox ID="tNewNodeName" runat="server" Width="100%" Text="New node"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <hr />
                            <asp:Button ID="btAddGroupSubmit" runat="server" Text="submit" CssClass="button_gray"
                                OnClick="btAddGroupSubmit_Click" />&nbsp;&nbsp;&nbsp;
                            <input type="button" onclick="hideDisplay('divAddGroup')" value="canel" class="button_gray" />
                        </td>
                    </tr>
                </table>
                <br />
            </div>
            <div id="divModifyGroup" style="position: absolute; z-index: 102; width: 70%; display: none; left: 15%; top: 30%; background-color: #446699; text-align: center;"
                onmousedown="mouseDown(this,event)">
                <br />
                <table width="100%">
                    <tr>
                        <td align="center">
                            <table width="100%">
                                <tr class="header1_table_first_row">
                                    <td colspan="2">Modify the node
                                    </td>
                                </tr>
                                <tr class="header1_tr_odd_row">
                                    <td>current node id
                                    </td>
                                    <td id="tdNodeID" runat="server"></td>
                                </tr>
                                <tr class="header1_tr_even_row">
                                    <td>new node name
                                    </td>
                                    <td>
                                        <input type="text" id="tNodeName" runat="server" style="width: 100%;" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="right">
                            <hr />
                            <asp:Button ID="btModifyGroupSubmit" runat="server" Text="submit" CssClass="button_gray"
                                OnClick="btModifyGroupSubmit_Click" />&nbsp;&nbsp;&nbsp;
                            <input type="button" onclick="hideDisplay('divModifyGroup')" value="canel" class="button_gray" />
                            <input type="button" value="move to" onclick="MoveGroup()" class="button_gray" style="display: none;" />
                        </td>
                    </tr>
                </table>
                <br />
            </div>
            <div id="divMoveGroup" style="position: absolute; z-index: 103; width: 60%; display: none; left: 20%; top: 10%; background-color: #6688aa; text-align: center; height: 60%;">
                <br />
                <table width="90%" height="100%">
                    <tr>
                        <td align="center">
                            <table width="100%" height="100%">
                                <tr class="header1_table_first_row" height="10">
                                    <td colspan="2">Move the group
                                    </td>
                                </tr>
                                <tr class="header1_tr_even_row">
                                    <td>
                                        <div style="height: 100%; overflow: auto;">
                                            <asp:TreeView ID="tvMoveGroup" runat="server" ShowLines="true" ExpandDepth="2">
                                            </asp:TreeView>
                                        </div>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr height="20">
                        <td align="right">
                            <hr />
                            <input type="button" onclick="hideDisplay('divMoveGroup')" value="back" class="button_gray" />
                        </td>
                    </tr>
                </table>
                <br />
            </div>
            <div style="display: none;">
                <input id="hiddenQuestionBankName" type="hidden" name="hiddenQuestionBankName" value="" runat="server">
                <input id="hiddenOpener" type="hidden" name="hiddenOpener" value="" runat="server">
                <input id="hiddenEditMode" type="hidden" name="hiddenEditMode" runat="server">
                <input id="hiddenQuestionMode" type="hidden" name="hiddenQuestionMode" runat="server">
                <input id="hiddenQuestionType" type="hidden" name="hiddenQuestionType" runat="server">
                <input id="hiddenPresentType" type="hidden" name="hiddenPresentType" runat="server">
                <input id="hiddenModifyType" type="hidden" name="hiddenModifyType" runat="server">
                <input id="hiddenQuestionFunction" type="hidden" name="hiddenQuestionFunction" runat="server">
                <input id="hiddenPreOpener" type="hidden" name="hiddenPreOpener" value="" runat="server">
                <input id="hiddenQuestionBankID" type="hidden" name="hiddenQuestionBankID" value="" runat="server">
                <asp:HiddenField ID="hfRoot" runat="server" />
            </div>
        </asp:Panel>
        <asp:Panel ID="Panel2" runat="server">
            <iframe id="ifPanel2" runat="server" style="height: 85%; width: 100%;"></iframe>
        </asp:Panel>
        <asp:HiddenField ID="hf_Career" runat="server" />
    </form>
</body>
</html>
