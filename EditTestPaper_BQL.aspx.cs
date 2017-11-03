﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Hints.DB;
using System.Drawing;

public partial class AuthoringTool_CaseEditor_Paper_EditTestPaper_BQL : System.Web.UI.Page
{
    protected clsHintsDB hintsDB = new clsHintsDB();
    bool flag;
    string strPaperID;

    protected void Page_Load(object sender, EventArgs e)
    {
        Ajax.Utility.RegisterTypeForAjax(typeof(AuthoringTool_CaseEditor_Paper_EditTestPaper_BQL));

        ConstructBQL();
        if (!IsPostBack)
        {
            if (Request.QueryString["SelectedIndex"] != null && Request.QueryString["SelectedIndex"].ToString() != "")
            {
                string strPreSQL = "SELECT cQuestionTopic FROM BasicQuestionList WHERE cQID='" + Request.QueryString["SelectedIndex"].ToString() + "'";
                DataTable dtPre = hintsDB.getDataSet(strPreSQL).Tables[0];
                if (dtPre.Rows.Count > 0)
                {
                    string[] strTopic = dtPre.Rows[0]["cQuestionTopic"].ToString().Split('/');
                    string strGetNameSQL = "SELECT cNodeName FROM QuestionGroupTree WHERE cNodeID='" + strTopic[1] + "'";
                    DataTable dtGetName = hintsDB.getDataSet(strGetNameSQL).Tables[0];
                    if (dtGetName.Rows.Count > 0)
                    {
                        for (int i = 1; i <= Convert.ToInt32(HF_RowCount.Value); i++)
                        {
                            Label LbTmp = (Label)this.FindControl("QuestionTopic_" + Convert.ToString(i));
                            if (LbTmp != null && LbTmp.Text == dtGetName.Rows[0]["cNodeName"].ToString())
                            {
                                RadioButton rbSelected = (RadioButton)this.FindControl("CheckBox_" + i);
                                if (rbSelected != null)
                                    cbEditTestPaper_Checked(rbSelected, e);
                            }
                        }
                    }
                }
            }
        }

        int NoChecked = 0;
        for (int i = 0; i < Convert.ToInt32(HF_RowCount.Value.ToString()); i++)
        {           
            RadioButton AllCheck = (RadioButton)this.FindControl("CheckBox_" + (i+1));
            if (AllCheck != null && AllCheck.Checked == false)
                NoChecked++;           
        }
        if (NoChecked == Convert.ToInt32(HF_RowCount.Value.ToString()))
        {
            RadioButton DefaultCheck = (RadioButton)this.FindControl("CheckBox_1");
            cbEditTestPaper_Checked(DefaultCheck, e);
        }

        if (!IsPostBack)
        {
            ChangeLanguage();// 多國語言函式  老詹 2015/04/14                     
        }

        DataTable dtTmp = new DataTable();
        string strSQL = "SELECT cNodeName FROM QuestionGroupTree WHERE cNodeID LIKE '" + Request.QueryString["GroupID"].ToString() + "' ORDER BY  cNodeID asc";
        dtTmp = hintsDB.getDataSet(strSQL).Tables[0];
        Lb_Title.Text = dtTmp.Rows[0]["cNodeName"].ToString();
        //topic_title2.Text = dtTmp.Rows[0]["cNodeName"].ToString();

        btnSaveNext.ServerClick += new EventHandler(btnSaveNext_ServerClick);
    }

    protected void RecoverChecked()
    {
        DataTable dtPrefix = new DataTable();
        string strPrefixSQL = "SELECT cNodeID FROM QuestionGroupTree WHERE cNodeName = '" + HF_SelectedQuesTopic.Value.ToString() + "' AND cParentID LIKE '" + Request.QueryString["GroupID"].ToString() + "'";
        dtPrefix = hintsDB.getDataSet(strPrefixSQL).Tables[0];

        DataTable dtCheck = new DataTable();
        string strcheckSQL = "SELECT * FROM BasicQuestionList WHERE cCaseID='"+ Request.QueryString["CaseID"].ToString() +"' AND cQuestionTopic = '" + (Request.QueryString["GroupID"].ToString() + "/" + dtPrefix.Rows[0]["cNodeID"].ToString()) + "'";
        dtCheck = hintsDB.getDataSet(strcheckSQL).Tables[0];
        if (dtCheck.Rows.Count > 0)
        {
            string[] strQIDArray = dtCheck.Rows[0]["cQID"].ToString().Split(',');
            for (int i = 0; i < strQIDArray.Length; i++)
            {
                HtmlInputRadioButton SelectedQID = (HtmlInputRadioButton)tbSelectConversation.FindControl("CheckBoxforConversation-" + strQIDArray[i]);
                if (SelectedQID != null)
                {
                    SelectedQID.Checked = true;
                    HF_SelectedIndex.Value = strQIDArray[i];
                }
            }
            HF_SelectedProType.Value = dtCheck.Rows[0]["cTestAnswerType"].ToString();
        }
        else
            HF_SelectedIndex.Value = "";
    }

    protected void ConstructBQL()
    {
        tbQuestionTopic.Controls.Clear();

        DataTable dtChildren = new DataTable();
        string strSQL = "SELECT * FROM QuestionGroupTree WHERE cParentID LIKE '" + Request.QueryString["GroupID"].ToString() + "' ORDER BY  cNodeID asc";
        dtChildren = hintsDB.getDataSet(strSQL).Tables[0];
        HF_RowCount.Value = Convert.ToString(dtChildren.Rows.Count);

        for (int i = 0; i < dtChildren.Rows.Count; i++)
        {
            TableRow QuestionTopicRow = new TableRow();
            QuestionTopicRow.Attributes.Add("style", "background:#DFDFDF;");
            TableCell QuestionTopicCell = new TableCell();
            RadioButton CheckBox1 = new RadioButton();
            CheckBox1.ID = "CheckBox_" + Convert.ToString(i + 1);
            CheckBox1.InputAttributes.Add("class","bigcheck");
            CheckBox1.CheckedChanged += new EventHandler(cbEditTestPaper_Checked);
            CheckBox1.AutoPostBack = true;
            Label Lb_Space = new Label();
            Lb_Space.Text = "&nbsp;&nbsp;&nbsp;";
            Label Lb_QuestionTopic = new Label();
            Lb_QuestionTopic.ID = "QuestionTopic_" + Convert.ToString(i + 1);
            Lb_QuestionTopic.Text = dtChildren.Rows[i]["cNodeName"].ToString();
            Lb_QuestionTopic.Font.Bold = true;
            Lb_QuestionTopic.Font.Size = 20;
            QuestionTopicCell.Controls.Add(CheckBox1);
            QuestionTopicCell.Controls.Add(Lb_Space);
            QuestionTopicCell.Controls.Add(Lb_QuestionTopic);
            QuestionTopicRow.Cells.Add(QuestionTopicCell);
            tbQuestionTopic.Rows.Add(QuestionTopicRow);
            QuestionTopicRow.Attributes.Add("class", "border");
        }
    }

    protected void cbEditTestPaper_Checked(object sender, EventArgs e)
    {
        string[] strIDArray = ((RadioButton)(sender)).ID.Split('_');
        string strSelectedIndex = strIDArray[1];
        Label SelectedQT = (Label)tbQuestionTopic.FindControl("QuestionTopic_" + strSelectedIndex);
        HF_SelectedQuesTopic.Value = SelectedQT.Text.ToString();

        for (int i = 0; i < Convert.ToInt32(HF_RowCount.Value); i++)
        {
            RadioButton checkclear = (RadioButton)tbQuestionTopic.FindControl("CheckBox_" + Convert.ToString(i + 1));
            if (checkclear.ID == "CheckBox_" + strSelectedIndex)
            {
                checkclear.Checked = true;
                ConstructConversation(Convert.ToString(i + 1));
                HF_CurrentRowIndex.Value = Convert.ToString(i + 1);
            }
            else
                checkclear.Checked = false;
        }
        flag = false;
        RecoverChecked();
    }

    protected void ConstructConversation(string SelectedIndex)
    {
        tbSelectConversation.Controls.Clear();
        Label LbSelectedTopic = (Label)tbQuestionTopic.FindControl("QuestionTopic_" + SelectedIndex);
        DataTable dtCheckProblemType = new DataTable();

        // 若是DDL事件，則SQL則要判斷選擇的ProblemType，不能直接列出所有ProblemType的情況  老詹 2014/01/13
        if (flag == false || (ddlProblemType.SelectedItem.Text=="Select a Problem Type"))
        {
            string strSQL = "SELECT M.cQID, Q.cNodeID, Q.cNodeName, L.cQuestionSymptoms, C.cQuestion FROM QuestionMode As M, QuestionLevel As L, Conversation_Question As C, QuestionGroupTree AS Q WHERE M.cQID=L.cQID AND M.cQID=C.cQID AND Q.cNodeID = M.cQuestionGroupID AND Q.cParentID='" + Request.QueryString["GroupID"].ToString() + "' AND Q.cNodeName='" + LbSelectedTopic.Text.ToString() + "'";
            dtCheckProblemType = hintsDB.getDataSet(strSQL).Tables[0];
        }
        else
        {
            string strSQL = "SELECT M.cQID, Q.cNodeID, Q.cNodeName, L.cQuestionSymptoms, C.cQuestion FROM QuestionMode As M, QuestionLevel As L, Conversation_Question As C, QuestionGroupTree AS Q WHERE M.cQID=L.cQID AND M.cQID=C.cQID AND Q.cNodeID = M.cQuestionGroupID AND Q.cParentID='" + Request.QueryString["GroupID"].ToString() + "' AND Q.cNodeName='" + LbSelectedTopic.Text.ToString() + "' AND L.cQuestionSymptoms='" + ddlProblemType.SelectedItem.Text.ToString() + "'";
            dtCheckProblemType = hintsDB.getDataSet(strSQL).Tables[0];
        }

        if (flag == false)
        {
            ddlProblemType.Items.Clear();
            ddlProblemType.Items.Add(new ListItem("Select a Problem Type", "Select a Problem Type"));
            for (int i = 0; i < dtCheckProblemType.Rows.Count; i++)
            {
                if (i > 0) //預防重複Problem Type的BUG
                {
                    if (dtCheckProblemType.Rows[i]["cQuestionSymptoms"].ToString() == dtCheckProblemType.Rows[i - 1]["cQuestionSymptoms"].ToString())
                    {
                        continue;
                    }
                }
                ddlProblemType.Items.Add(new ListItem(dtCheckProblemType.Rows[i]["cQuestionSymptoms"].ToString(), dtCheckProblemType.Rows[i]["cQuestionSymptoms"].ToString()));
                ddlProblemType.SelectedIndex = 0;
            }
        }

        for (int i = 0; i < dtCheckProblemType.Rows.Count; i++)
        {
            TableRow ConversationRow = new TableRow();
            ConversationRow.Height = 50;      
            if (i % 2 == 0)
            { ConversationRow.Attributes.Add("style", "background:#E1C4C4; vertical-align:middle; text-align: left;"); }
            else
            { ConversationRow.Attributes.Add("style", "background:#FFD306; vertical-align:middle; text-align: left;"); }
            TableCell ConversationPreCell = new TableCell();
            ConversationPreCell.Width = 30;
            TableCell ConversationCell = new TableCell();
            HtmlInputRadioButton CheckBox1 = new HtmlInputRadioButton();
            CheckBox1.ID = "CheckBoxforConversation-" + dtCheckProblemType.Rows[i]["cQID"].ToString();
            CheckBox1.Name = "Conversation";
            CheckBox1.Attributes.Add("class", "bigcheck");
            CheckBox1.Attributes.Add("onclick", "SaveSelected('" + dtCheckProblemType.Rows[i]["cQuestionSymptoms"].ToString() + "', '" + HF_SelectedQuesTopic.Value.ToString() + "', '" + Request.QueryString["GroupID"].ToString() + "', '" + Request.QueryString["CaseID"].ToString() + "','" + CheckBox1.ID + "');");
            Label Lb_Conversation = new Label();
            Lb_Conversation.ID = "Conversation" + Convert.ToString(i + 1);
            Lb_Conversation.Text = dtCheckProblemType.Rows[i]["cQuestion"].ToString();
            Lb_Conversation.Font.Size = 14;
            TableCell ConversationCell2 = new TableCell();
            ConversationCell2.Width = 50;
            HtmlInputButton btnBrowse = new HtmlInputButton();
            btnBrowse.ID = "btnBrowse_" + dtCheckProblemType.Rows[i]["cQID"].ToString();
            btnBrowse.Value = "Browse";
            string strCareer = Request.QueryString["Career"];
            btnBrowse.Attributes.Add("style", "cursor:pointer;");
            btnBrowse.Attributes.Add("onclick", "BrowseConversation('" + dtCheckProblemType.Rows[i]["cQID"].ToString() + "','" + dtCheckProblemType.Rows[i]["cNodeID"].ToString() + "','" + strCareer + "','" + dtCheckProblemType.Rows[i]["cQuestionSymptoms"].ToString() + "');");           
            ConversationPreCell.Controls.Add(CheckBox1);
            ConversationRow.Cells.Add(ConversationPreCell);
            ConversationCell.Controls.Add(Lb_Conversation);
            ConversationCell2.Controls.Add(btnBrowse);
            ConversationRow.Cells.Add(ConversationCell);
            ConversationRow.Cells.Add(ConversationCell2);
            tbSelectConversation.Rows.Add(ConversationRow);
            ConversationRow.Attributes.Add("class", "border");
            ConversationRow.Cells[0].Attributes.Add("class", "left");
            ConversationRow.Cells[ConversationRow.Cells.Count - 1].Attributes.Add("class", "right");
        }
    }

    #region 按Next後存入資料庫，暫時註解  老詹 2015/04/01
    /*protected void SaveToDatabase()
    {
        if (Request.QueryString["GroupID"] != null)
        {
            DataTable dtCheck = new DataTable();
            string strcheckSQL = "SELECT B.cPaperID FROM BasicQuestionList AS B, QuestionGroupTree AS Q WHERE B.cCaseID='" + Request.QueryString["CaseID"].ToString() + "' AND Q.cNodeName='" + HF_SelectedQuesTopic.Value.ToString() + "' AND B.cQuestionTopic='" + Request.QueryString["GroupID"].ToString() + "' + '/' + Q.cNodeID";
            dtCheck = hintsDB.getDataSet(strcheckSQL).Tables[0];
            if (dtCheck.Rows.Count > 0)
            {
                strPaperID = dtCheck.Rows[0]["cPaperID"].ToString();
            }
            else
            {
                string iQuestionSerialNum = "Paper";
                //以當下日期時間作為流水號
                DateTime now = DateTime.Now;
                string temp = now.ToString("yyyyMMddHHmmssFFFFF");
                //建立QID
                strPaperID = iQuestionSerialNum + "_" + temp;
            }
        }

        string strSaveQID = HF_ConversationSelected.Value.ToString(); // 選取的QID有哪些，組成一字串存入資料庫  老詹 2015/01/14
        string strNewSaveQID = strSaveQID.TrimEnd(',');

        DataTable dtCheckPaper = new DataTable();
        string strSQL = "SELECT * FROM BasicQuestionList WHERE cPaperID = '" + strPaperID + "' AND cCaseID = '"+ Request.QueryString["CaseID"].ToString() +"'";
        dtCheckPaper = hintsDB.getDataSet(strSQL).Tables[0];
        if (dtCheckPaper.Rows.Count > 0)
        {
            string strUpdateSQL = "UPDATE BasicQuestionList SET cQID = '" + strNewSaveQID + "', cTestAnswerType='" + HF_SelectedProType.Value.ToString() + "' WHERE cPaperID = '" + dtCheckPaper.Rows[0]["cPaperID"].ToString() + "'";
            hintsDB.ExecuteNonQuery(strUpdateSQL);
        }
        else
        {
            DataTable dtGetNodeID = new DataTable();
            string strGetIDSQL = "SELECT cNodeID FROM QuestionGroupTree WHERE cNodeName = '" + HF_SelectedQuesTopic.Value.ToString() + "'";
            dtGetNodeID = hintsDB.getDataSet(strGetIDSQL).Tables[0];

            string strInsertSQL = "INSERT INTO BasicQuestionList (cPaperID, cQuestionTopic, cQID, cVPAID, cTestAnswerType, cCaseID) " +
                    "VALUES ('" + strPaperID + "' , '" + (Request.QueryString["GroupID"].ToString() + "/" + dtGetNodeID.Rows[0]["cNodeID"].ToString()) + "' , '" + strNewSaveQID + "', '' , '"+ HF_SelectedProType.Value.ToString() +"', '" + Request.QueryString["CaseID"].ToString() + "') ";
            hintsDB.ExecuteNonQuery(strInsertSQL);
        }
    }*/
    #endregion

    private void btnSaveNext_ServerClick(object sender, EventArgs e)
    {
        //SaveToDatabase();
        string strCareer = Request.QueryString["Career"];
        Response.Redirect("EditTestPaper_VPAns.aspx?GroupID=" + Request.QueryString["GroupID"].ToString() + "&Career=" + strCareer + "&CaseID=" + Request.QueryString["CaseID"].ToString() + "&NewTopic=No&SelectedIndex=" + HF_SelectedIndex.Value.ToString());
    }
    protected void BtnBack_Click(object sender, EventArgs e)
    {
       string strCareer = Request.QueryString["Career"];
       Response.Redirect("QuestionGroupTree/QGroupTreeNew.aspx?Career=" + strCareer);
    }
    protected void ddlProblemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        flag = true;
        ConstructConversation(HF_SelectedIndex.Value.ToString());
        RecoverChecked();
    }

    [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
    public void ConfirmSave(string SelectedProType, string strQID, string loacalQuestopic, string localGroupID, string strCaseID, string strcbID)
    {
        string strNewSaveQID = strQID.TrimEnd(',');
        string[] strSelectedcb = strcbID.Split('-');

        DataTable dtCheckPaper = new DataTable();
        string strSQL = "SELECT B.cPaperID FROM BasicQuestionList AS B, QuestionGroupTree AS Q WHERE B.bIsOriginal='1' AND B.cCaseID = '" + strCaseID + "' AND Q.cNodeName='" + loacalQuestopic + "' AND B.cQuestionTopic='" + localGroupID + "' + '/' + Q.cNodeID";
        dtCheckPaper = hintsDB.getDataSet(strSQL).Tables[0];
        if (dtCheckPaper.Rows.Count > 0)
        {
            if (strNewSaveQID == "") // 防止編入QID為空時，ProblemType仍不空
            { SelectedProType = ""; }
            else if (strNewSaveQID.IndexOf(strSelectedcb[1]) == -1) // 防止編入QID與ProblemType不符  老詹 2015/04/01
            {
                string[] checkQIDRealPT = strNewSaveQID.Split(',');
                string strAllPT = "";
                for(int i=0;i<checkQIDRealPT.Length;i++)
                {
                    string strGetRealPTSQL = "SELECT cQuestionSymptoms FROM QuestionLevel WHERE cQID = '"+ checkQIDRealPT[i] +"'";
                    DataTable dtCheckRealPT = hintsDB.getDataSet(strGetRealPTSQL).Tables[0];
                    strAllPT += dtCheckRealPT.Rows[i]["cQuestionSymptoms"].ToString() + ",";
                    if (strAllPT.IndexOf(SelectedProType) == -1)
                    { SelectedProType = dtCheckRealPT.Rows[i]["cQuestionSymptoms"].ToString(); }
                }
            }
            string strUpdateSQL = "UPDATE BasicQuestionList SET cQID = '" + strNewSaveQID + "', cTestAnswerType='" + SelectedProType + "' WHERE cPaperID = '" + dtCheckPaper.Rows[0]["cPaperID"].ToString() + "'";
            hintsDB.ExecuteNonQuery(strUpdateSQL);
        }
        else
        {
            string iQuestionSerialNum = "Paper";
            //以當下日期時間作為流水號
            DateTime now = DateTime.Now;
            string temp = now.ToString("yyyyMMddHHmmssFFFFF");
            //建立QID
            strPaperID = iQuestionSerialNum + "_" + temp;

            DataTable dtGetNodeID = new DataTable();
            string strGetIDSQL = "SELECT cNodeID FROM QuestionGroupTree WHERE cParentID LIKE '" + localGroupID + "' AND cNodeName = '" + loacalQuestopic + "'";
            dtGetNodeID = hintsDB.getDataSet(strGetIDSQL).Tables[0];

            string strInsertSQL = "INSERT INTO BasicQuestionList (cPaperID, cQuestionTopic, cQID, cVPAID, cTestAnswerType, cCaseID, bIsOriginal) " +
                    "VALUES ('" + strPaperID + "' , '" + (localGroupID + "/" + dtGetNodeID.Rows[0]["cNodeID"].ToString()) + "' , '" + strNewSaveQID + "', '' , '" + SelectedProType + "', '" + strCaseID + "', '1') ";
            hintsDB.ExecuteNonQuery(strInsertSQL);
        }
    }   

    #region 多國語言所有Function  老詹 2015/04/14
    protected string MultiLanguage(string text)
    {
        string ret_text = MultiLanguage(text, ddl_MutiLanguage.SelectedItem.Text.ToString());
        ret_text = (ret_text == "" || ret_text == null) ? text : ret_text;
        return ret_text;
    }

    public static string MultiLanguage(string text, string culture)
    {
        string ret = "";

        System.Resources.ResourceManager res = Resources.BQL.Resource.ResourceManager;
        //setLocalName(language);
        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(culture);//zh-TW

        try
        {
            ret = res.GetString(text, ci);
        }
        catch
        {
            ret = text;
        }
        return ret;
    }
    protected void ddl_MutiLanguage_SelectedIndexChanged(object sender, EventArgs e)
    {
        ChangeLanguage();
        RadioButton DefaultCheck = (RadioButton)this.FindControl("CheckBox_" + HF_CurrentRowIndex.Value.ToString());
        cbEditTestPaper_Checked(DefaultCheck, e);
    }
    protected void ChangeLanguage()
    {
        //if (ddl_MutiLanguage.SelectedItem.Text == "en-US")
        {
            Lb_Topic.Text = this.MultiLanguage("PaperTitle");
            Lb_TopicZh.Text = "";
        }
        /*else
        {
            Lb_TopicZh.Text = this.MultiLanguage("PaperTitle");
            Lb_Topic.Text = "";
        }*/
        Lb_QuesTopic.Text = this.MultiLanguage("QuestionTopic");
        Lb_ConQues.Text = this.MultiLanguage("QuesTitle");
        Lb_Conversation.Text = this.MultiLanguage("Conversation");
        btnEditDescription.Value = this.MultiLanguage("BtnEditDescription");
        BtnBack.Text = this.MultiLanguage("BtnBack");
        BtnNext.Value = this.MultiLanguage("BtnNextToEditVPAns");
    }
    #endregion
}