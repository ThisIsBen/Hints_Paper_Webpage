﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SetVRTextBook.aspx.cs" Inherits="AuthoringTool_CaseEditor_Paper_SetVRTextBook" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>編輯VR學習模式教材</title>
    <!-- Bootstrap -->
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <link href="~/bootstrap/css/bootstrap.min.css" rel="stylesheet" media="screen">
    <script src="http://code.jquery.com/jquery.js"></script>
    <script src="~/bootstrap/js/bootstrap.min.js"></script>
    <!--Tab-->
     <link rel="stylesheet" href="~/jquery-ui(BlueThemes)-1.10.4.custom/jquery-ui-1.10.4.custom/css/redmond/jquery-ui-1.10.4.custom.min.css">
  <script src="//code.jquery.com/jquery-1.10.2.js"></script>
  <script src="//code.jquery.com/ui/1.10.4/jquery-ui.js"></script>
  <!--<link rel="stylesheet" href="/resources/demos/style.css">-->
   <!--<script type='text/javascript' src='http://ajax.googleapis.com/ajax/libs/jqueryui/1.9.1/jquery-ui.js'></script><!--神奇的一行-->
   
    <style>
  .sortable { list-style-type: none; margin: 15px; padding: 0; width: 90%; }
  .sortable li { margin: 3px 3px 3px 3px; padding: 0.4em; padding-left: 1.5em; font-size: 1.4em; height: 30px; }
  .sortable li span { position: absolute; margin-left: -1.3em; }
        .auto-style3
        {
            height: 100px;
        }
        .auto-style4
        {
            height: 52px;
        }
  </style>

     <script type="text/javascript">
         function divSaveToStepjsButton()
         {
             __doPostBack('btnCancel','');
         }

         function showVMWindow() {
             //newwin = window.open("../AuthoringTool/CaseEditor/VirtualMicroscope/VMSimulatorAuthoring.aspx", "full", "fullscreen=yes,resizable=yes");
             //window.open('', '_self', '');
             window.close();
         }

      </script>
    <script type="text/javascript">
       
            function showVMWindow() {
                //newwin = window.open("../AuthoringTool/CaseEditor/VirtualMicroscope/VMSimulatorAuthoring.aspx", "full", "fullscreen=yes,resizable=yes");
                //window.open('', '_self', '');
                window.close();
            }

      </script>

    <script>
        $(function () {
            $("#divSaveToStep").draggable({ revert: true });

        });
        
        $(function () {
            //根據目前所選的record還原
            var a = document.getElementById('HiddenFieldAccordionIndex').value;
            a = parseInt(a , 10)
            $(".classAccordion").accordion({
                collapsible: true,
                heightStyle: "content",
                active: a,
                //改變accordion後紀錄目前所選的
                activate: function (event, ui) { 
                    var activeIndex = $(".classAccordion").accordion("option", "active");
                    document.getElementById('HiddenFieldAccordionIndex').value = activeIndex.toString();
                    //alert(document.getElementById('HiddenFieldAccordionIndex').value);
                }

            });

        });

        $(function () {
            $(".sortable").sortable(
                {
                    cursor: "move",
                    //revert: true,
                    opacity: 0.6,
                    update: function (event, ui) {       //更新排序之后
                        var CurrentOrder = "";
                        $("#"+$(this).attr("id")+" div").each(function () {
                            CurrentOrder = CurrentOrder + $(this).attr("id") + "@";
                            //CurrentOrder = $(this).attr("id");
                        })
                        //document.getElementById('HiddenFieldCurrentSortRecord').value = $(this).attr("id");
                        document.getElementById('HiddenFieldCurrentOrder').value = CurrentOrder.toString();
                        document.getElementById("btnHidden").click();       //排序後重新寫入DB
                    }
                });
            $(".sortable").disableSelection();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table width="98%" class="table" style="height: 1200px">
            <asp:Label ID="Label53" runat="server" Text="**在頁面左側點選要進行編輯的教材頁(須讓教材頁變黃色才表示選則成功)" ForeColor="Red" Font-Size="Larger"></asp:Label><br>
            <asp:Label ID="Label57" runat="server" Text="**若要移除該頁教材，則點擊[X]，便可以移除該頁教材" ForeColor="Red" Font-Size="Larger"></asp:Label><br>
            <asp:Label ID="Label58" runat="server" Text="**若要新增教材頁，則點擊[+]，便可以新增教材頁" ForeColor="Red" Font-Size="Larger"></asp:Label><br>
                
            
            <tr>
                <td width="25%">
                    <table style=" border-style: groove; border-width: thin;" class="table">
                        <tr>
                            <td align="center" valign="top" style="border-style: groove; border-width: thin; background-color: #0066FF; color: #FFFFFF; font-weight: bold;" class="auto-style4">
                                <asp:Label ID="Label1" runat="server" Text="Step" Font-Size="X-Large"></asp:Label>
                            </td>
                        </tr>

                        <tr>
                            <td style="border-style: groove; border-width: thin; ">
                                <asp:Panel ID="PanelStep" runat="server" Height="900px" ScrollBars="Vertical"></asp:Panel>
                            </td>
                        </tr>
                    </table>
                    
                </td>

                <td width="75%">
                    <table width="100%" style=" border-style: groove; border-width: thin; " class="table">
                        <tr>
                            <td align="center" valign="top" style="border-style: groove; border-width: thin; background-color: #CC0000; color: #FFFFFF; font-weight: bold;" colspan="2">
                                <asp:Label ID="Label5" runat="server" Text="編輯教材內容" Font-Size="X-Large"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-top: 10px; padding-left: 10px" width="50%" class="auto-style3">
                                <asp:Label ID="Label2" runat="server" Text="教材名稱 : " Font-Bold="True" Font-Size="X-Large"></asp:Label>
                                <asp:TextBox ID="tbxTextBookName"  runat="server" CssClass="form-control" Width="100%" Text="TextBook" OnTextChanged="tbxTextBookName_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <!--<asp:Label ID="Label3" runat="server" Text="開啟舊檔 : " Font-Bold="True" Font-Size="X-Large"></asp:Label>
                                <asp:DropDownList ID="ddlOpen" runat="server" AutoPostBack="True"  class="form-control" Width="100%">

                                </asp:DropDownList>-->

                            </td>
                            <td width="50%" style="padding-top: 10px; padding-left: 10px" class="auto-style3">
                               <!--<asp:Button ID="btnNext" runat="server" Text="<<上一頁" class="btn btn-info" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnPre" runat="server" Text="下一頁>>" class="btn btn-info" />-->
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:Button ID="btnAdd" runat="server" Text="另存新檔" class="btn btn-primary" OnClick="SaveToOrtherStep_Click" />
                                &nbsp;&nbsp;
                                <asp:Button ID="btnSave" runat="server" Text="儲存" class="btn btn-success" OnClick="btnSave_Click" Visible="false" />

                                <asp:Button ID="btnHidden" runat="server" Text="隱藏按鈕" class="btn btn-success" Style="display: none" OnClick="btnReUpdateOrder_Click" />


                               <!-- <div id="divBtnSchoolGrouping" class="btn-group" >
               <a class="btn btn-primary" data-toggle="dropdown" href="#">分組
                   <span class="caret"></span>
               </a>
               <ul class="dropdown-menu">
                  <li><a id="A1"  runat="server">加入</a></li>
                  <li><a id="A2" runat="server">學校自動</a></li>
                  <li><a id="A3" runat="server">審查委員</a></li>
               </ul>
            </div>  -->
                            </td>
                        </tr>

                        <tr>
                            <td style="padding-left: 10px" colspan="2">
                                <table width="100%" class="table table-bordered">
                                    <tr>
                                        <td>
                                            <asp:Label ID="Label4" runat="server" Text="教材內容" Font-Bold="True" Font-Size="XX-Large"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:Label ID="Label10" runat="server" Text="預覽" Font-Bold="True" Font-Size="XX-Large"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <!--編輯頁面的左半部-->
                                        <td >
                                            <table class="table table-hover">
                                                <!--上傳圖片-->
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label11" runat="server" Text="圖片" Font-Bold="True" Font-Size="X-Large" ForeColor="Gray"></asp:Label>
                                                    </td>
                                                    <td style="vertical-align: bottom; text-align: right;">
                                                        <asp:Image ID="Image2" runat="server"  ImageUrl="~/AuthoringTool/CaseEditor/Paper/images/led-icons/1420640680_edit_property.png" Visible="false"  />
                                                        <asp:LinkButton ID="lbtnEdit" runat="server" Font-Bold="True" Font-Size="Large" OnClick="btnEdit_Click" Visible="false">編輯</asp:LinkButton>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Label ID="lbImageName" runat="server" Text="請點選編輯新增圖片" Visible="false" Font-Size="Large" ForeColor="Silver" Width="200px"></asp:Label>
                                                        <asp:FileUpload ID="UploadImage" runat="server"  Visible="true"/>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Button ID="btnImportImage" runat="server" Text="匯入" class="btn" OnClick="imageUploadFile_Click" Visible ="true"/>
                                                        &nbsp;&nbsp;<asp:Button ID="btnRemoveImage" runat="server" Text="移除" class="btn btn-danger" OnClick="btnRemove_Click" Visible ="true" />
                                                    </td>
                                                </tr>



                                                <!--Video-->
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label12" runat="server" Text="影片" Font-Bold="True" Font-Size="X-Large" ForeColor="Gray" ></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    
                                                    <td valign="top">
                                                        <asp:Label ID="lbVideoName" runat="server" Text="請點選編輯新增影片" Font-Size="Large" ForeColor="Silver" Visible="false"></asp:Label>
                                                        <asp:FileUpload ID="VideoUpload" runat="server" Visible ="true" />
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Button ID="btnVideoImport" runat="server" Text="匯入" class="btn" OnClick="videoUploadFile_Click" Visible ="true" />
                                                        &nbsp;&nbsp;<asp:Button ID="btnRemoveVideo" runat="server" Text="移除" class="btn btn-danger" OnClick="btnRemove_Click" Visible ="true"/>
                                                    </td>
                                                </tr>

                                                <div onmouseover=""></div>
                                                <!--Audio-->
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label13" runat="server" Text="音訊" Font-Bold="True" Font-Size="X-Large" ForeColor="Gray"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Label ID="lbAudioName" runat="server" Text="請點選編輯新增音訊檔" Font-Size="Large" ForeColor="Silver" Visible="false"></asp:Label>
                                                        <asp:FileUpload ID="AudioUpload" runat="server" Visible ="true"/>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Button ID="btnImportAudio" runat="server" Text="匯入" class="btn" OnClick="AudioUploadFile_Click" Visible ="true"/>
                                                        &nbsp;&nbsp;<asp:Button ID="btnRemoveAudio" runat="server" Text="移除" class="btn btn-danger" OnClick="btnRemove_Click" Visible ="true"/>
                                                    </td>
                                                </tr>

                                                <!--PDF-->
                                               <%-- <tr>
                                                    <td class="auto-style2">
                                                        <asp:Label ID="Label14" runat="server" Text="PDF" Font-Bold="True" Font-Size="X-Large" ForeColor="Gray"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top">
                                                        <asp:Label ID="lbPDFName" runat="server" Text="請點選編輯新增PDF" Font-Size="Large" ForeColor="Silver"></asp:Label>
                                                        <asp:FileUpload ID="PDFUpload" runat="server" Visible ="false"/>
                                                    </td>
                                                    <td valign="top">
                                                        <asp:Button ID="btnPDFbImport" runat="server" Text="匯入" class="btn" OnClick="PDFUploadFile_Click" Visible ="false"/>
                                                        &nbsp;&nbsp;<asp:Button ID="btnRemovePDF" runat="server" Text="移除" class="btn btn-danger" OnClick="btnRemove_Click" Visible ="false" />
                                                    </td>
                                                </tr>--%>

                                                <!--網頁連結-->
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Labelfff" runat="server" Text="網頁連結" Font-Bold="True" Font-Size="Larger" ForeColor="Gray"></asp:Label>
                                                    </td>
                                                    <td>
                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" colspan="2">
                                                        <a id="lbWebName" runat="server" href="" style="font-size: larger; color: #C0C0C0" Visible="false">請點選編輯新增網頁連結</a>
                                                        <asp:TextBox ID="tbxWeb" runat="server" Visible="true" Width="100%" OnTextChanged="tbxWeb_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                    </td>
                                                </tr>


                                                <!--文字-->
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label6" runat="server" Text="文字說明" Font-Bold="True" Font-Size="X-Large" ForeColor="Gray"></asp:Label>
                                                    </td>
                                                    <td>
                                                        
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td valign="top" colspan="2">
                                                        <asp:Label ID="lbTextName" runat="server" Text="請點選編輯新增文字說明" Font-Size="Large" ForeColor="Silver" Visible="false"></asp:Label>
                                                        <asp:TextBox ID="tbxEditTextContent" runat="server" TextMode="MultiLine" Visible="true" Width="100%" Height="200" OnTextChanged="tbxEditTextContent_TextChanged" AutoPostBack="true" ></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <!--右半部顯示目前內容-->
                                        <td>
                                            <table style="width: 100%">
                                                <tr>
                                                    <td align="center" style="text-align: center" style="width: 100%">

                                                        <table align="center" class="table table-bordered" style="border-width: 2px; width: 95%">
                                                            <tr>
                                                    <td align="center" style="padding-top: 10px">
                                                        <asp:Panel ID="Panel1" runat="server" Width="320px">
                                                            <asp:Image ID="imgUpLoadImage" runat="server"  ImageUrl="~/AuthoringTool/CaseEditor/Paper/UpLoadImage/123.jpg" Height="157px" />
                                                        </asp:Panel> 
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td align="center"  valign="middle" style="padding-top: 10px">
                                                     <video width="320" height="240" controls id="vdUploadvideo" runat="server" >
                                                     <!--<source src="UpLoadVideo/movie.mp4" type="video/mp4">-->
                                                     </video>
                                                   </td>
                                                </tr>
                                                <tr>
                                                     <td align="center"  valign="middle" style="padding-top: 10px">
                                                     <audio id="audUploadAudio"  preload="auto" controls runat="server" width="320" height="240"></audio>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center"  valign="middle" style="padding-top: 10px" >
                                                        <asp:TextBox ID="tbxTextContent" runat="server" Width="700" Height="200px" TextMode="MultiLine" Font-Size="X-Large" CssClass="form-control" Enabled="false"></asp:TextBox>
                                                   </td>
                                                </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                
                                            </table>
                                        </td>
                                    </tr>
                                    <!---->
                                </table>
                            </td>
                        </tr>
                        
                    </table>
                   
                </td>
            </tr>
            <tr>
               <td align="right" colspan="2">
                    <asp:Button ID="btCancelEdit"  OnClick="btCancelEdit_Click" runat="server" class="btn btn-danger"
                         style="width: 150px;  height: 30px" 
                        Text="Cancel" />
                    <asp:Button ID="btnFinishEdit" OnClick="btFinishEdit_Click" runat="server" class="btn btn-success" 
                         style="width: 150px;  height: 30px" 
                        Text="Finish" />
                </td>
            </tr>
        </table>
    </div>
        

         <!--選擇新加入的步驟-->
    <div id="divSaveToStep" class="modal-content" 
            style="background-color: #FFFFFF;  display:none;  position: absolute; z-index: 102; left:50%; top:50%; margin-left: -475px; margin-top: -400px; font-family: 'Microsoft YaHei';" 
            runat="server">
      <div class="modal-content">
     
          <div class="modal-header">
            <button type="button" class="close" onclick="divSaveToStepjsButton()" >&times;</button>
            <h4 class="modal-title" id="H7">另存新檔</h4>
          </div>
      
          <div class="modal-body">
              <table width="500px" class="table table-hover">
                  <tr>
                      <td>
                          <asp:Label ID="lbStep" runat="server" Text="步驟 :  " Font-Size="X-Large"></asp:Label>
                      </td>
                      <td>
                          <asp:DropDownList ID="ddlNewStep" runat="server" Width="400px" Font-Size="X-Large"></asp:DropDownList>
                      </td>
                  </tr>

                  <tr>
                      <td style="margin-top: 20px; padding-top: 20px;">
                          <asp:Label ID="lbType" runat="server" Text="類型 :  " Font-Size="X-Large"></asp:Label>
                      </td>
                      <td style="margin-top: 20px; padding-top: 20px;">
                          <asp:DropDownList ID="ddlType" runat="server" Width="400px" Font-Size="X-Large">
                              <asp:ListItem Value="Text">說明</asp:ListItem>
                              <asp:ListItem Value="Suggest">暗示</asp:ListItem>
                          </asp:DropDownList>
                      </td>
                  </tr>

              </table>
          </div>
      
          <div class="modal-footer">
             <asp:Button ID="Button1" runat="server" Text="確定" class="btn btn-success" OnClick="SaveToStepConfirm_Click"/>
              <asp:Button ID="btnCancel" runat="server" Text="取消" class="btn btn-danger" OnClick="SaveToStepCancel_Click"/>
          </div>
    
        </div>
    </div>


        <!--記錄目前accordion哪一個-->
        <asp:HiddenField ID="HiddenFieldAccordionIndex" runat="server" Value="0"/>
        <!--記錄目前所選擇的textBook-->
        <asp:HiddenField ID="HiddenFieldSelectTextBook" runat="server" Value="0"/>
        <asp:HiddenField ID="HiddenFieldTextBook" runat="server" Value="0"/>
        <!--記錄目前texBook的圖片-->
        <asp:HiddenField ID="HiddenFieldImage" runat="server" Value=""/>
        <!--記錄目前texBook的影片-->
        <asp:HiddenField ID="HiddenFieldVideo" runat="server" Value=""/>
        <!--記錄目前texBook的聲音-->
        <asp:HiddenField ID="HiddenFieldAudio" runat="server" Value=""/>
        <!--記錄目前texBook的PDF-->
        <asp:HiddenField ID="HiddenFieldPDF" runat="server" Value=""/>
        <!--記錄目前texBook的PDF-->
        <asp:HiddenField ID="HiddenField1" runat="server" Value=""/>
        <!--記錄排序過後的textBook順序-->
        <asp:HiddenField ID="HiddenFieldCurrentOrder" runat="server" Value=""/>
        <!--記錄目前被排序的textBook所在的Record-->
        <asp:HiddenField ID="HiddenFieldCurrentSortRecord" runat="server" Value=""/>
        <!--記錄目前編輯頁面上是否為編輯狀態 0:目前不為編輯狀態-->
        <asp:HiddenField ID="HiddenFieldEdit" runat="server" Value="0"/>
        <!--記錄另存新檔的ID-->
        <asp:HiddenField ID="HiddenSaveToID" runat="server" Value=""/>
    </form>
</body>
</html>
