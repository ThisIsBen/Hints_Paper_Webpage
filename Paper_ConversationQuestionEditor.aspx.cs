﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Hints.DB;
using Hints.DB.Section;
using Hints.DB.Conversation;
using System.IO;
using HtmlAgilityPack;
using System.Text;
using System.Net;
using System.Collections.Generic;


namespace PaperSystem
{
    /// <summary>
    /// 用來編輯對話題的工具
    /// </summary>
    public partial class Paper_ConversationQuestionEditor : AuthoringTool_BasicForm_BasicForm
    {
        protected string btnFinish = "";
        protected string strClientScript = "";

        bool flag;

        string strUserID, strCaseID, strDivisionID, strClinicNum, strSectionName, strPaperID, strQID, strAID;
        string strGroupID = "";
        string strGroupDivisionID = "";
        int iAnswerType = 0;
        string strAnswerTypeName = "";

        bool bNewQuestion = false;

        SQLString mySQL = new SQLString();

        //選好之ProblemType   老詹 2014/09/24
        string[] InitiateProblemType = new string[2];
        //防止Postback的同義詞tr   老詹 2015/06/04
        List<HtmlTableRow> List1;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Ajax.Utility.RegisterTypeForAjax(typeof(Paper_ConversationQuestionEditor));
            this.Initiate();

            //接收參數
            this.getParametor();

            #region 拿到Paper_EditKeyword.aspx
            if (List1 == null)
            {
                List1 = new List<HtmlTableRow>();
            }
            if (!IsPostBack)
            {
                if (Session["SynonymsRow"] != null)
                {
                    List1 = (List<HtmlTableRow>)Session["SynonymsRow"];
                    foreach (HtmlTableRow tmp1 in List1)
                    {
                        Lb_synonyms.Rows.Add(tmp1);
                    }
                }
            }

            //顯示keyword的事件
            string stringGet = Hints.Learning.Question.DataReceiver.getConversation_Question_Keyword(strQID);
            if (stringGet != "")
            {
                Lb_synonyms.Controls.Clear();
                HtmlTableRow trKeywordTitle = new HtmlTableRow();
                trKeywordTitle.Attributes.Add("class", "header1_table_first_row");
                HtmlTableCell tdKeywordTitle = new HtmlTableCell();
                tdKeywordTitle.Style["width"] = "20%";
                tdKeywordTitle.InnerHtml = "<span style='font-size:20px;'>Keyword</span>";
                trKeywordTitle.Cells.Add(tdKeywordTitle);
                HtmlTableCell tdSynonymsTitle = new HtmlTableCell();
                tdSynonymsTitle.Style["width"] = "80%";
                tdSynonymsTitle.InnerHtml = "<span style='font-size:20px;'>Synonyms</span>";
                trKeywordTitle.Cells.Add(tdSynonymsTitle);
                Lb_synonyms.Rows.Add(trKeywordTitle);
                string[] strAllKeyword = stringGet.Split('|');
                for (int i = 0; i < strAllKeyword.Length; i++)
                {
                    string[] strKeywordArr = strAllKeyword[i].Split(',');
                    if (strKeywordArr[0] != "")
                    {
                        HtmlTableRow trKeyword = new HtmlTableRow();
                        HtmlTableCell tdKeyword = new HtmlTableCell();
                        tdKeyword.Style["width"] = "20%";
                        tdKeyword.InnerHtml = "<input type='text' id='Lb_showKeyword' runat='server' value='" + strKeywordArr[0] + "' readonly='true' style='color: #FF0000; font-size: 20px; width: 100%;' />";
                        HtmlTableCell tdSynonyms = new HtmlTableCell();
                        tdSynonyms.Style["width"] = "80%";
                        Label lbAllSynonyms = new Label();
                        lbAllSynonyms.Font.Size = 16;
                        for (int j = 1; j < strKeywordArr.Length; j++)
                        {
                            lbAllSynonyms.Text += strKeywordArr[j] + ",  ";
                        }
                        if (lbAllSynonyms.Text != "")
                        { lbAllSynonyms.Text = lbAllSynonyms.Text.Remove(lbAllSynonyms.Text.LastIndexOf(',')); }
                        tdSynonyms.Controls.Add(lbAllSynonyms);                  
                        trKeyword.Cells.Add(tdKeyword);
                        trKeyword.Cells.Add(tdSynonyms);
                        Lb_synonyms.Rows.Add(trKeyword);
                        if (i % 2 == 0)
                            trKeyword.Attributes.Add("class", "header1_tr_even_row");
                        else
                            trKeyword.Attributes.Add("class", "header1_tr_odd_row");
                    }
                }
            }
            #endregion

            //加入btnSaveNextQuestion的事件
            btnSaveNextQuestion.ServerClick += new EventHandler(btnSaveNextQuestion_ServerClick);
            btnDeleteAudioServer.ServerClick += new EventHandler(ImgB_DeleteExCorrectsentence_Click);

            //加入btnSaveNext的事件
            btnSaveNext.ServerClick += new EventHandler(btnSaveNext_ServerClick);

            string strGetKeyword = Hints.Learning.Question.DataReceiver.getTextQuestionKeyword(strQID);

            if (strGetKeyword != "N/A")
            {
                lbKeyword.Text = strGetKeyword;
                btnEditKeyword.CssClass = "button_press_enable";
                //btnEditKeyword.Text = "Edit the Keyword";
            }

            #region 設定病徵的下拉選單項目
            //建立病徵的下拉選單項目
            DataTable dtProblemTypeTree = ProblemTypeTree_SELECT();
            ddlSymptoms.Items.Add("All");
            foreach (DataRow drProblemTypeTree in dtProblemTypeTree.Rows)
            {
                ddlSymptoms.Items.Add(drProblemTypeTree["cNodeName"].ToString());
            }
            #endregion

            // 以Label顯示教師選定好的Problem Type，而ddlSymtoms沒刪是因為讓Insert更準確。  老詹 2014/09/24
            if (Session["ProblemTypeConfirm"] != null)
            {
                btnClose.Visible = false;
                string strGetProblemTypeConfirm = Session["ProblemTypeConfirm"].ToString();
                InitiateProblemType = strGetProblemTypeConfirm.Split('/');
                if (InitiateProblemType[1] != "Select a Child")
                    LbCurrentSymptoms.Text = InitiateProblemType[1].ToString();
                else
                    LbCurrentSymptoms.Text = InitiateProblemType[0].ToString();                  
            }
            else
            {
                if (Request.QueryString["Browse"] == null)
                {
                    btnClose.Visible = false;
                    trShowCurProblemType.Visible = false;
                }
                else
                {
                    trShowCurProblemType.Visible = false;
                    btnBack.Visible = false;
                    btnNext.Visible = false;
                    btnEditKeyword.Visible = false;
                    btnClose.Visible = true;
                    lbDeleteRecordHints.Visible = false;
                    ImgB_DeleteExCorrectsentence.Visible = false;
                    trANswerType.Attributes.Add("style","display:none;");
                }
            }

            #region 設定病徵的下拉選單選擇的項目 與答案型態
            if (!IsPostBack)
            {
                clsHintsDB hintsDB = new clsHintsDB();
                //GroupName
                string strGroupName = DataReceiver.getQuestionGroupNameByQuestionGroupID(strGroupID);
                string strGetParnetNameSQL = "SELECT cNodeName FROM QuestionGroupTree WHERE cNodeID=(SELECT cParentID FROM QuestionGroupTree WHERE cNodeID='" + strGroupID + "')";
                DataTable dtGetParnetName = hintsDB.getDataSet(strGetParnetNameSQL).Tables[0];
                lbCurrentGroupName.Text = dtGetParnetName.Rows[0]["cNodeName"].ToString() + "/" + strGroupName;
                string strQuestionSymptoms = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_SELECT_QuestionSymptoms(strQID);
                if (strQuestionSymptoms != "-1")
                    ddlSymptoms.SelectedValue = strQuestionSymptoms;

                //新建的問題 則答案型態設成第一個
                if (bNewQuestion)
                {
                    DataTable dtConversation_AnswerType = clsConversation.Conversation_AnswerType_SELECT_AllAnswerType(Convert.ToInt32(hfGroupSerialNum.Value));
                    if (dtConversation_AnswerType.Rows.Count > 0)
                    {
                        lbAnswerType.Text = dtConversation_AnswerType.Rows[0]["cAnswerTypeName"].ToString();
                        ViewState.Add("AnswerType",dtConversation_AnswerType.Rows[0]["cAnswerTypeNum"].ToString());
                    }
                    else
                    {
                        lbAnswerType.Text = "NULL";
                    }
                }
                else
                {
                    DataTable dtConversation_AnswerType = clsConversation.Conversation_AnswerType_SELECT_AssignedAnswerType(Convert.ToInt32(hfGroupSerialNum.Value), iAnswerType);
                    if (dtConversation_AnswerType.Rows.Count > 0)
                    {
                        lbAnswerType.Text = dtConversation_AnswerType.Rows[0]["cAnswerTypeName"].ToString();
                    }
                }

            }
            #endregion           

            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            //BulidInterrogation("Question");
            BulidAnswerTypeContent("Answer");

            if (Session["bDisplayQuestionList"] != null)
            {
                if (Session["bDisplayQuestionList"].ToString() == " Yes ")
                {
                    Title_Answer_Type.Text = "<span style='color:red;'>Hints: 若您不想輸入題目內容，也可以直接設定提供學生聆聽的聲音例句，即點擊下方「Record Question Contents」按鈕開啟錄音頁面錄製。<br/>(請注意您的Google Chrome瀏覽器是否已允許此網站IP的彈出式視窗。)</span><br/>";
                    lbAnswerType.Visible = false;
                    btAddAnswer.Visible = false;
                    btRecordAnswer.Visible = true;
                }
            }

            CheckAudioID();
        }

        protected void CheckAudioID()
        {
            // 判斷是否有已錄好的正確例句   老詹 2014/12/17
            clsHintsDB hintsDB = new clsHintsDB();
            string strRecordSQL = "SELECT cAudioID FROM Conversation_Question WHERE cQID = '" + strQID + "'";
            DataTable dtRecordinfo = hintsDB.getDataSet(strRecordSQL).Tables[0];
            if (Request.QueryString["bModify"].ToString() == "True")
            {
                if (dtRecordinfo.Rows[0]["cAudioID"].ToString() != "")
                {
                    flag = true;
                    AudioIframe.InnerHtml = "<iframe id='ifEXPlayer' scrolling='no' runat='server' width='402px' height='42px' src='../../../Learning/InterrogationEnquiry/Voice_Inquiry/AudioPlayFlv.aspx?AudioID=" + dtRecordinfo.Rows[0]["cAudioID"].ToString() + "'></iframe>";
                }
                else
                {
                    flag = false;
                    ExCorrectsentence.Visible = false;
                }
            }
            else
            {
                flag = false;
                ExCorrectsentence.Visible = false;
            }
        }

        /// <summary>
        /// 接收參數
        /// </summary>
        private void getParametor()
        {
            //UserID
            if (Session["UserID"] != null)
            {
                strUserID = Session["UserID"].ToString();
            }

            //CaseID
            if (Session["CaseID"] != null)
            {
                strCaseID = Session["CaseID"].ToString();
            }

            //Division
            if (Session["DivisionID"] != null)
            {
                strDivisionID = Session["DivisionID"].ToString();
            }

            //ClinicNum
            if (Session["ClinicNum"] != null)
            {
                strClinicNum = Session["ClinicNum"].ToString();
            }

            //SectionName
            if (Session["SectionName"] != null)
            {
                strSectionName = Session["SectionName"].ToString();
            }

            hfPaperID.Value = "";
            /*//PaperID
            if (Session["PaperID"] != null)
            {
                strPaperID = Session["PaperID"].ToString();
                hfPaperID.Value = strPaperID;
            }
            else
            {
                SQLString mySQL = new SQLString();
                strPaperID = mySQL.getPaperIDFromCase(strCaseID, strClinicNum.ToString(), strSectionName);
                hfPaperID.Value = strPaperID;
            }*/

            //Opener
            if (Request.QueryString["Opener"] != null)
            {
                hiddenOpener.Value = Request.QueryString["Opener"].ToString();
            }

            //QuestionMode
            if (Session["QuestionMode"] != null)
            {
                hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
            }

            //PresentType
            if (Session["PresentType"] != null)
            {
                hiddenPresentType.Value = Session["PresentType"].ToString();
            }

            //Edit method
            if (Session["EditMode"] != null)
            {
                hiddenEditMode.Value = Session["EditMode"].ToString();
            }

            //GroupID
            if (Request.QueryString["GroupID"] != null)
            {
                strGroupID = Request.QueryString["GroupID"].ToString();
                hfGroupID.Value = strGroupID;
                hfGroupSerialNum.Value = DataReceiver.getQuestionGroupSerialNumByQuestionGroupID(strGroupID).ToString();

                if (Session["GroupID"] != null)
                {
                    Session["GroupID"] = strGroupID;
                }
                else
                {
                    Session.Add("GroupID", strGroupID);
                }
            }

            //QID
            if (Request.QueryString["QID"] != null)
            {
                strQID = Request.QueryString["QID"].ToString();

                //把QID的題目內容寫入txtData中
                string strQuestion = clsConversation.Conversation_Question_SELECT_Question(strQID);
                if (this.IsPostBack == false)
                {
                    txtQuestionEdit.Text = strQuestion;
                }
                bNewQuestion = false;
            }
            else
            {
                //問題流水號
                int iQuestionSerialNum = Conversation_Question_Select_MaxiSerialNum();
                //問題所在群組分類的流水號
                int iQuestionGroupSerialNum = Convert.ToInt32(DataReceiver.getQuestionGroupSerialNumByQuestionGroupID(strGroupID).ToString());
                //建立QID
                strQID = iQuestionSerialNum + "_" + iQuestionGroupSerialNum;
                bNewQuestion = true;
            }
            hfQID.Value = strQID;

            //AnswerType
            if (Request.QueryString["AnswerType"] != null )
            {
                iAnswerType = Convert.ToInt32(Request.QueryString["AnswerType"].ToString());
            }
            else if (ViewState["AnswerType"] != null)//新建問題時iAnswerType初始值設定為1(第一個答案類型)
            {
                iAnswerType = Convert.ToInt32(ViewState["AnswerType"]);
            }

            //GroupDivisionID
            if (strGroupID != null)
            {
                if (strGroupID.Trim().Length > 0)
                {
                    DataReceiver myReceiver = new DataReceiver();
                    strGroupDivisionID = myReceiver.getGroupDivisionID(strGroupID);

                    if (Session["GroupDivisionID"] != null)
                    {
                        Session["GroupDivisionID"] = strGroupDivisionID;
                    }
                    else
                    {
                        Session.Add("GroupDivisionID", strGroupDivisionID);
                    }
                }
            }
            //hfFilePath.Value = Request.QueryString["bModify"].ToString();
        }

        #region Web Form 設計工具產生的程式碼
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 此為 ASP.NET Web Form 設計工具所需的呼叫。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 此為設計工具支援所必須的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        private DataTable GetKeyword(string strQID)
        {
            clsHintsDB sqldb = new clsHintsDB();
            string strSQL = "Select * From Paper_TextQuestionKeyword Where cQID = '" + strQID + "' ";
            DataTable dtTemp = sqldb.getDataSet(strSQL).Tables[0];
            return dtTemp;
        }   

        private void btnSaveNextQuestion_ServerClick(object sender, EventArgs e)
        {
            DataReceiver myReceiver = new DataReceiver();

            //儲存題目
            clsTextQuestion myText = new clsTextQuestion();
            string strTextQContent = txtQuestionEdit.Text;
            strTextQContent = strTextQContent.Replace("&lt;", "<");
            strTextQContent = strTextQContent.Replace("&gt;", ">");

            //myText.saveQuestionAnswer(strQID, strAID, strTextQContent, strTextAContent, strUserID, strPaperID, strGroupDivisionID, strGroupID, hiddenQuestionMode.Value);

            //儲存問題的病徵
            AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_INSERT_QuestionSymptoms(strQID, LbCurrentSymptoms.Text.ToString());

            //如果是Specific題目則需儲存一筆資料至Paper_Content
            if (hiddenQuestionMode.Value == "Specific")
            {
                int intContentSeq = myReceiver.getPaperContentMaxSeq(strPaperID) + 1;
                SQLString mySQL = new SQLString();
                mySQL.SaveToQuestionContent(strPaperID, strQID, "0", "2", hiddenQuestionMode.Value, intContentSeq.ToString());
            }
            //建立QID
            strQID = strUserID + "_" + myReceiver.getNowTime();

            //清除TextArea
            txtQuestionEdit.Text = "";
            string strScript = "<script language='javascript'>\n";
            strScript += "Clear()\n";
            strScript += "</script>\n";
            Page.RegisterStartupScript("Clear", strScript);

            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            //BulidInterrogation("Question");
            BulidAnswerTypeContent("Answer");
        }

        /// <summary>
        /// Save text question
        /// </summary>
        private void SaveQuestionText()
        {
            //儲存題目
            clsTextQuestion myText = new clsTextQuestion();
            string strQTextContent = txtQuestionEdit.Text;
            strQTextContent = strQTextContent.Replace("&lt;", "<");
            strQTextContent = strQTextContent.Replace("&gt;", ">");

            clsConversation.saveConversation_Question(strQID, strQTextContent, strPaperID, strGroupDivisionID, strGroupID, hiddenQuestionMode.Value);

            if (Request.QueryString["OpenGroupID"] != null) // 若從Search頁面開啟，儲存時要額外Insert一筆資料到QuestionMode，表示在OpenGroupID中也要有這類題目  老詹 2015/09/06
            {
                clsHintsDB sqldb = new clsHintsDB();
                string strGetOpenGroupName = "SELECT cNodeName FROM QuestionGroupTree WHERE cNodeID='" + Request.QueryString["OpenGroupID"].ToString() + "'";
                DataTable dtGetOpenGroupName = sqldb.getDataSet(strGetOpenGroupName).Tables[0];
                if (dtGetOpenGroupName.Rows.Count > 0)
                {
                    string strCheckRepeat = "SELECT * FROM QuestionMode WHERE cQID='" + strQID + "' AND cQuestionGroupID = '" + Request.QueryString["OpenGroupID"].ToString() + "'";
                    DataTable dtCheckRepeat = sqldb.getDataSet(strCheckRepeat).Tables[0];
                    if (dtCheckRepeat.Rows.Count <= 0)
                    {
                        string strInsertOtherGroupSQL = "INSERT INTO QuestionMode(cQID,cQuestionGroupID,cQuestionGroupName,cQuestionMode,cQuestionType) VALUES ('" + strQID + "','" + Request.QueryString["OpenGroupID"].ToString() + "','" + dtGetOpenGroupName.Rows[0]["cNodeName"].ToString() + "','General','4')";
                        sqldb.ExecuteNonQuery(strInsertOtherGroupSQL);
                    }
                }
            }

            //儲存Problem Type
            AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_INSERT_QuestionSymptoms(strQID, "All");

            //如果是Specific題目則需儲存一筆資料至Paper_Content
            if (hiddenQuestionMode.Value == "Specific")
            {
                DataReceiver myReceiver = new DataReceiver();
                int intContentSeq = myReceiver.getPaperContentMaxSeq(strPaperID) + 1;
                SQLString mySQL = new SQLString();
                mySQL.SaveToQuestionContent(strPaperID, strQID, "0", "2", hiddenQuestionMode.Value, intContentSeq.ToString());
            }

        }

        private void btnSaveNext_ServerClick(object sender, EventArgs e)
        {
            Session["SynonymsRow"] = null;

            SaveQuestionText();

            //Redirect至下一個網頁
            string strSystemFunction = "";
            string strCareer = Request.QueryString["Career"];  //OK後仍要判斷職業  老詹 2014/08/26
            if (Session["SystemFunction"] != null)
            {
                strSystemFunction = Session["SystemFunction"].ToString();
            }

            switch (strSystemFunction)
            {
                case "EditPaper":
                    Response.Redirect("Paper_OtherQuestion.aspx?Opener=Paper_TextQuestionEditor");
                    break;
                case "EditQuestion":
                    if (Request.QueryString["QID"] != null)
                    {
                        if (Request.QueryString["OpenGroupID"] != null)
                            Response.Redirect("Paper_QuestionViewNew_VoiceInquiry.aspx?Opener=Paper_TextQuestionEditor&Career=" + strCareer + "&bDisplayBQL=" + Session["bDisplayQuestionList"].ToString() + "&GroupID=" + Request.QueryString["OpenGroupID"].ToString());
                        else
                            Response.Redirect("Paper_QuestionViewNew_VoiceInquiry.aspx?Opener=Paper_TextQuestionEditor&Career=" + strCareer + "&bDisplayBQL=" + Session["bDisplayQuestionList"].ToString());
                    }
                    else
                    {
                        strGroupID = Session["GroupID"].ToString();
                        Response.Redirect("Paper_QuestionViewNew_VoiceInquiry.aspx?GroupID=" + strGroupID + "&Career=" + strCareer + "&bDisplayBQL=" + Session["bDisplayQuestionList"].ToString());
                    }
                    break;
                case "PreviewPaper":
                    Response.Redirect("Paper_MainPage.aspx?Opener=Paper_TextQuestionEditor");
                    break;
                default:
                    Response.Redirect("Paper_QuestionMain.aspx?Opener=Paper_TextQuestionEditor");
                    break;
            }
        }

        //返回後仍要判斷職業  老詹 2014/08/26
        protected void GobackCareer(object sender, EventArgs e)
        {
            
          

           //When generating a new 對話題 (if the Opener is Paper_QuestionTypeNew), set Previous page  as Paper_QuestionTypeNew,and return to it right away.
           if (Request.QueryString["Opener"].ToString() == "Paper_QuestionTypeNew")
           {

               
               Response.Redirect(Request.QueryString["Opener"].ToString() + ".aspx?Opener=./QuestionGroupTree/QGroupTreeNew&bModify=False&GroupID=" + strGroupID);
           }


               clsHintsDB sqldb = new clsHintsDB();
           string strCheckNull = "SELECT * FROM Conversation_Question WHERE cQID='"+ strQID +"'";
           DataTable dtTemp = sqldb.getDataSet(strCheckNull).Tables[0];
           if (dtTemp.Rows.Count > 0)
           {
               if (dtTemp.Rows[0]["cQuestion"].ToString() == "" && dtTemp.Rows[0]["cKeyword"].ToString() == "" && dtTemp.Rows[0]["cAudioID"].ToString() == "")
               {
                   string strDeleteSQL = "DELETE FROM Conversation_Question WHERE cQID='" + strQID + "'";
                   sqldb.ExecuteNonQuery(strDeleteSQL);
               }
           }
           Session["SynonymsRow"] = null;
           string strCareer = Request.QueryString["Career"];
           RegisterStartupScript("", "<script language='javascript'>goBack('" + strCareer + "', '"+ Session["bDisplayQuestionList"].ToString() +"');</script>");
           
        }

        protected void btnEditKeyword_Click(object sender, EventArgs e)
        {
            Session["SynonymsRow"] = null;
            SaveQuestionText();
            string strCareer = Request.QueryString["Career"];
            Response.Redirect("~/AuthoringTool/CaseEditor/Paper/Paper_EditQuestionKeyword.aspx?QID=" + strQID + "&GroupID=" + strGroupID + "&CaseID=" + usi.CaseID + "&ClinicNum=" + usi.ClinicNum + "&Section=" + usi.Section + "&AID=" + strAID + "&Career=" + strCareer);
        }

        protected void btAddSynQuestion_Click(object sender, EventArgs e)
        {
            SaveQuestionText();
            Response.Redirect("../Interrogation/EditAddAskQuestionAnswer.aspx?Mode=AddQuestion&RecentItemID=" + strQID + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "&AID=" + strAID + "");
        }

        protected void btAddAnswer_Click(object sender, EventArgs e)
        {
            SaveQuestionText();
            string strCareer = Request.QueryString["Career"];
            Response.Redirect("EditConversationAnswer.aspx?Mode=AddAnswer&QID=" + strQID + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "&AnswerType=" + iAnswerType + "&Career=" + strCareer + "");
        }

        //建立答案型態的答案
        private void BulidAnswerTypeContent(string strType)
        {
            strAnswerTypeName = clsConversation.Conversation_AnswerType_SELECT_AssignedAnswerTypeName(Convert.ToInt32(hfGroupSerialNum.Value), iAnswerType);
            DataTable dtConversation = new DataTable();
            if (strType == "Answer")
            {
                tbInterrogationClassAnswer.Controls.Clear();
                tbInterrogationClassAnswer.Style.Add("background-color", "#F8F0E7");
                dtConversation = clsConversation.Conversation_Answer_SELECT_Answer(strQID, strAnswerTypeName);
            }

            if (dtConversation.Rows.Count > 0)
            {
                #region 答案的標題
                TableRow trAnswerTypeContentT = new TableRow();
                trAnswerTypeContentT.Attributes.Add("style", "border:0px black solid;background:#7BC2FA; font-weight:bold;color:black");
                trAnswerTypeContentT.Style.Add("CURSOR", "hand");
                trAnswerTypeContentT.Style.Add("TEXT-ALIGN", "left");

                TableCell tcAnswerTypeContentT = new TableCell();
                trAnswerTypeContentT.Cells.Add(tcAnswerTypeContentT);

                tcAnswerTypeContentT.Text = "<IMG id='img" + strType + "_" + strQID + "' src='../../../BasicForm/Image/minus.gif'>&nbsp; " + strType;

                TableRow trAnswerTypeContentTable = new TableRow();
                trAnswerTypeContentTable.ID = "tr" + strType + "" + strQID;
                trAnswerTypeContentT.Attributes.Add("onclick", "ShowDetail('" + trAnswerTypeContentTable.ID + "' , 'img" + strType + "_" + strQID + "')");

                if (strType == "Answer")
                {
                    tbInterrogationClassAnswer.Rows.Add(trAnswerTypeContentT);
                    tbInterrogationClassAnswer.Rows.Add(trAnswerTypeContentTable);
                    hrAnswer.Style.Add("display", "");
                }


                TableCell tcAnswerTypeContentTable = new TableCell();
                trAnswerTypeContentTable.Cells.Add(tcAnswerTypeContentTable);

                Table tbAnswerTypeContentTable = new Table();
                tcAnswerTypeContentTable.Controls.Add(tbAnswerTypeContentTable);
                tbAnswerTypeContentTable.Width = Unit.Percentage(100);
                tbAnswerTypeContentTable.GridLines = GridLines.Both;

                #endregion

                #region 同義問題或答案內容
                for (int iAnswerTypeContent = 0; iAnswerTypeContent < dtConversation.Rows.Count; iAnswerTypeContent++)
                {
                    #region 同義項目的序號
                    TableRow trAnswerTypeContentItemT = new TableRow();
                    trAnswerTypeContentItemT.CssClass = "header1_tr_even_row";
                    tbAnswerTypeContentTable.Rows.Add(trAnswerTypeContentItemT);

                    TableCell tcAnswerTypeContentItemT = new TableCell();
                    trAnswerTypeContentItemT.Cells.Add(tcAnswerTypeContentItemT);
                    tcAnswerTypeContentItemT.Text = "Answer." + (iAnswerTypeContent + 1) + " :";
                    tcAnswerTypeContentItemT.Width = Unit.Percentage(8);

                    string strAnswerTypeContentType = dtConversation.Rows[iAnswerTypeContent]["cAnswerContentType"].ToString();
                    string strAnswerTypeContentValue = dtConversation.Rows[iAnswerTypeContent]["cAnswer"].ToString();
                    TableCell tcAnswerTypeContent = new TableCell();
                    trAnswerTypeContentItemT.Cells.Add(tcAnswerTypeContent);
                    #endregion

                    #region 答案項目的內容
                    tcAnswerTypeContent.Text = "( " + strAnswerTypeContentType + " ) " + strAnswerTypeContentValue;
                    tcAnswerTypeContent.Width = Unit.Percentage(82);
                    #endregion

                    #region 答案項目的編輯
                    TableCell tcAnswerTypeContentModify = new TableCell();
                    trAnswerTypeContentItemT.Cells.Add(tcAnswerTypeContentModify);
                    tcAnswerTypeContentModify.Style.Add("text-align", "center");
                    tcAnswerTypeContentModify.Width = Unit.Percentage(5);

                    Button btModifyAnswerTypeContent = new Button();
                    btModifyAnswerTypeContent.Click += new EventHandler(btModifyAnswerTypeContent_Click);
                    btModifyAnswerTypeContent.CssClass = "button_Edit";
                    btModifyAnswerTypeContent.CommandArgument = "btnModify$" + strQID + "$" + dtConversation.Rows[iAnswerTypeContent]["cAID"].ToString();
                    tcAnswerTypeContentModify.Controls.Add(btModifyAnswerTypeContent);


                    TableCell tcAnswerTypeContentDelete = new TableCell();
                    trAnswerTypeContentItemT.Cells.Add(tcAnswerTypeContentDelete);
                    tcAnswerTypeContentDelete.Style.Add("text-align", "center");
                    tcAnswerTypeContentDelete.Width = Unit.Percentage(5);

                    Button btnDeleteAnswerTypeContent = new Button();
                    btnDeleteAnswerTypeContent.ID = "btnDelete|" + strQID + "|" + dtConversation.Rows[iAnswerTypeContent]["cAID"].ToString();
                    btnDeleteAnswerTypeContent.CssClass = "button_Delete";
                    btnDeleteAnswerTypeContent.Click += new EventHandler(btnDeleteAnswerTypeContent_Click);
                    btnDeleteAnswerTypeContent.CommandArgument = "btnDelete$" + strQID + "$" + dtConversation.Rows[iAnswerTypeContent]["cAID"].ToString();
                    tcAnswerTypeContentDelete.Controls.Add(btnDeleteAnswerTypeContent);
                    #endregion
                }

                #endregion
            }
        }

        //刪除項目
        void btnDeleteAnswerTypeContent_Click(object sender, EventArgs e)
        {
            string strTempQID = "";
            string strTempAID = "";
            Button btnDelete = new Button();
            btnDelete = (Button)(sender);

            strTempQID = btnDelete.CommandArgument.Split('$')[1];//QID
            strTempAID = btnDelete.CommandArgument.Split('$')[2];//AID

            clsConversation.Conversation_Answer_DELETE_AssignedAnswer(strTempQID, strTempAID);

            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            //BulidInterrogation("Question");
            BulidAnswerTypeContent("Answer");
        }

        //修改項目
        void btModifyAnswerTypeContent_Click(object sender, EventArgs e)
        {
            string strTempQID = "";
            string strTempAID = "";
            Button btnDelete = new Button();
            btnDelete = (Button)(sender);

            strTempQID = btnDelete.CommandArgument.Split('$')[1];//QID
            strTempAID = btnDelete.CommandArgument.Split('$')[2];//AID

            Response.Redirect("EditConversationAnswer.aspx?Mode=ModifyAnswer&QID=" + strTempQID + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "&AID=" + strTempAID + "&AnswerType=" + iAnswerType + "");
        }

        //取得病徵項目
        private DataTable ProblemTypeTree_SELECT()
        {
            clsHintsDB HintsDB = new clsHintsDB();
            DataTable dtProblemTypeTree = new DataTable();
            string strSQL_ProblemTypeTree = "SELECT  DISTINCT cNodeName FROM ProblemTypeTree WHERE cParentID != 'Diseaseroot' ORDER BY cNodeName ASC";
            dtProblemTypeTree = HintsDB.getDataSet(strSQL_ProblemTypeTree).Tables[0];
            return dtProblemTypeTree;
        }

        //取得Conversation_Question目前的流水號
        private int Conversation_Question_Select_MaxiSerialNum()
        {
            int iMaxSerialNum = 0;
            clsHintsDB HintsDB = new clsHintsDB();
            string strSQL_Conversation_Question = "SELECT MAX(iSerialNum) AS iSerialNum FROM Conversation_Question";
            DataTable dtConversation_Question = HintsDB.getDataSet(strSQL_Conversation_Question).Tables[0];
            if (dtConversation_Question.Rows.Count > 0)
            {
                if (dtConversation_Question.Rows[0]["iSerialNum"].ToString() != "")
                {
                    iMaxSerialNum = (Convert.ToInt32(dtConversation_Question.Rows[0]["iSerialNum"].ToString()) + 1);
                }
                else
                {
                    iMaxSerialNum = 1;
                }

            }
            else
            {
                iMaxSerialNum = 1;
            }
            return iMaxSerialNum;
        }

        protected void btRecordAnswer_Click(object sender, EventArgs e)
        {
            if (flag == true)
            {
                RegisterStartupScript("", "<script language='javascript'>alert('你已經設定好正確例句，不能重複錄製！');</script>");
            }
            else
            {
                TimerCheck.Enabled = true;
                // 開啟Google Chrome 頁面(ReplayPage.aspx為中繼頁面，以此開啟錄音頁面)   老詹 2014/11/12
                string strScriptURL = "var WshShell = new ActiveXObject('Wscript.Shell');WshShell.run('chrome.exe " + "http://140.116.72.28/Hints/ReplayPage.aspx" + "?QID=" + strQID + "&UserID=&NodeName=&CaseID=" + strCaseID + "&SectionName=&ActionTime=&StartTime=&Division=', 1, false);WshShell.Quit;";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "MyScript", strScriptURL, true);              
            }
        }
        private void ImgB_DeleteExCorrectsentence_Click(object sender, EventArgs e)
        {
            clsHintsDB hintsDB = new clsHintsDB();
            string strDeleteAudioSQL = "UPDATE Conversation_Question SET cAudioID = '" + null + "' WHERE cQID='" + strQID + "'";
            hintsDB.ExecuteNonQuery(strDeleteAudioSQL);
            CheckAudioID();
        }
        protected void TimerCheck_Tick(object sender, EventArgs e)
        {
            CheckAudioID();
            if (flag == true)
            {
                //PostBack  老詹 2015/04/30
                Response.Redirect(Request.Url.ToString());
                TimerCheck.Enabled = false;
            }
        }
}
}
