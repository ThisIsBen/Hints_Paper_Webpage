using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AuthoringTool;
using suro.util;
using Hints.DB;
using ORCS.DB;
using Hints.DB.Administrator;
using MLASDB;
using System.Collections.Generic;
using System.Web.Services;

namespace PaperSystem
{
    /// <summary>
    /// Paper_MainPage ���K�n�y�z�C
    /// �ݨ��t�Φۦ�s��ݨ��\�઺�_�l��
    /// </summary>
    public partial class Paper_MainPage : AuthoringTool_BasicForm_BasicForm
    {
        //�إ�SqlDB����
        SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
        DataReceiver myReceiver = new DataReceiver();
        SQLString mySQL = new SQLString();

        string strUserID, strCaseID, strDivisionID, strClinicNum, strSectionName, strEditMode, strPaperID;
        int intQuestionIndex = 0;
        int intSumScore;
        string strGenerationMethod = "Edit";
        string strMaximumNumberOfWordsReasons = "100";
        protected System.Web.UI.HtmlControls.HtmlTableRow trQuestionNumTitle;

        /// <summary>
        /// �^��PEQuestion��PaperID
        /// </summary>
        /// <param name="strCaseID"></param>
        /// <param name="strClinicNum"></param>
        /// <param name="strSectionName"></param>
        /// <returns></returns>
        private string getPaperIDFromPaper_PEQuestion(string strCaseID, string strClinicNum, string strSectionName, string strItem)
        {
            string strReturn = "";

            string strSQL = "SELECT cPaperID FROM Paper_PEQuestion WHERE cCaseID = '" + strCaseID + "' AND sClinicNum = '" + strClinicNum + "' AND cSectionName = '" + strSectionName + "' AND cItem = '" + strItem + "' ";
            DataSet ds = sqldb.getDataSet(strSQL);
            if (ds.Tables[0].Rows.Count > 0)
            {
                strReturn = ds.Tables[0].Rows[0]["cPaperID"].ToString();
            }
            ds.Dispose();

            return strReturn;
        }
        //���J����
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Session["QuestionBankID"] != null)
            {
                Session.Remove("QuestionBankID");
            }


            this.Initiate();

            //�p�G�OMLAS�ɤJ�A���]�wSesseion��
            this.InitiateParameterFromMLAS();


            //�P�_�ѭ��ӭ�����L�ӥh���o���P���Ѽ�
            if (Request.QueryString["Opener"] != null)
            {
                hiddenOpener.Value = Request.QueryString["Opener"].ToString();
            }
            else if (Session["PreOpener"] != null)
            {
                hiddenOpener.Value = Session["PreOpener"].ToString();
            }

            //�qORCS���Ұ�m�߭��������Ѽ�
            if (hiddenOpener.Value == "SelectPaperMode")
            {
                this.getParameterFromORCSExercise();
                //�]�w�q�Ұ�m�߭����i�ӮɡA�����������
                SetPageStyle();
            }
            else //�q�쥻HINTS��Case���������Ѽ�
                this.getParameter();

            //��X��Case�����@�Ӱݨ��A���X��PaperID�C
            if (Session["PaperPurpose"] != null)
            {
                hfPaperPurpose.Value = "PEQuestion";
                btnPre.Style.Add("display", "none");
                switch (Session["PaperPurpose"].ToString())
                {
                    case "PEQuestion":
                        if (Session["Item"] != null)
                        {
                            strPaperID = this.getPaperIDFromPaper_PEQuestion(strCaseID, strClinicNum, strSectionName, Session["Item"].ToString());
                        }
                        break;
                }
            }
            else if (strCaseID != null && strCaseID != "")
            {
                strPaperID = mySQL.getPaperIDFromCase(strCaseID, strClinicNum, strSectionName);
            }
            else if (Request.QueryString["cPaperID"] != null)
            {
                strPaperID = Request.QueryString["cPaperID"].ToString();
            }

            //MLAS�Ҩ��оǬ���
            if (Request.QueryString["cComeFromActivityName"] != null)
            {
                Session["ComeFromActivityName"] = Request.QueryString["cComeFromActivityName"].ToString();
            }

            if (Request.QueryString["cActivityID"] != null)
            {
                Session["ActivityID"] = Request.QueryString["cActivityID"].ToString();
            }

            //��PaperID�s�JSession
            if (Session["PaperID"] != null)
            {
                Session["PaperID"] = strPaperID;
            }
            else
            {
                Session.Add("PaperID", strPaperID);
            }

            
            //���o���ݨ��O�ϥΪ̦ۦ�s��άO�t�Φb�e�{�D�خɤ~�üƿ��D
            string strSQL = mySQL.getPaperHeader(strPaperID);
            DataSet dsHeader = sqldb.getDataSet(strSQL);
            try
            {
                strGenerationMethod = dsHeader.Tables[0].Rows[0]["cGenerationMethod"].ToString();
                //����D�t����r��g�z�ѳ̤j�r��
                strMaximumNumberOfWordsReasons = dsHeader.Tables[0].Rows[0]["cMaximumNumberOfWordsReasons"].ToString();
                if (strMaximumNumberOfWordsReasons == "")
                {
                    strMaximumNumberOfWordsReasons = "200";
                }
            }
            catch
            {
            }
            dsHeader.Dispose();

            if (!IsPostBack)
            {
                tbMaximumNumberOfWordsReasons.Text = strMaximumNumberOfWordsReasons;
            }

            //��PresentType�s�JSession
            if (Session["PresentType"] != null)
            {
                Session["PresentType"] = strGenerationMethod;
            }
            else
            {
                Session.Add("PresentType", strGenerationMethod);
            }

            //��PresentType�s�JHidden
            hiddenPresentType.Value = strGenerationMethod;

            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                this.setupQuestionTable();


                //�إ�Question num table
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");
                //�Y���O�s��Ҧ��A�h���æ����s�u�� ���g2012/12/25
                trSetScoreControl.Style.Add("DISPLAY", "none");
                //�إ�Question group number Table
                this.setupQuestionNumTable();
            }

            //�Ĥ@�����J�����ƥ�
            if (this.IsPostBack == false)
            {
                //�]�wtxtTitle
                this.setupPaperTitle();

                //�ˬd���L�ݭn�۰ʥ����D�ؤ���
                if (Request.QueryString["bIsAutoSetScore"] != null && Request.QueryString["bIsAutoSetScore"].Equals("true"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAutoScorll", "document.getElementById('btnAutoSetScore').click();", true);
                }
            }

            //�[�J�R�����s���ƥ�
            btnDeleteQuestion.ServerClick += new EventHandler(btnDeleteQuestion_ServerClick);

            //�[�J�ק���s���ƥ�
            btnModifyQuestion.ServerClick += new EventHandler(btnModifyQuestion_ServerClick);

            #region WebPage����޲z

            //�q��ƪ���o���󪺺޲z���A
            DataTable dtWebPageObjectManage = clsWebPageObjectManage.WebPageObjectManage("Paper_MainPage.aspx", usi.UsingSystem);
            if (dtWebPageObjectManage != null)
            {
                //�ھڸ�ƪ��Ȱ��@�@���������A�޲z
                btnPre.Visible = Hints.TableStyle.CheckWebPageObjectManage(dtWebPageObjectManage.Rows[0]["cObjectType"].ToString(), 0);

                //�ھڸ�ƪ��Ȱ����������󵹭�
                hfPageUrl.Value = dtWebPageObjectManage.Rows[0]["cPageUrl"].ToString();
            }
            else////default hints
            {
                hfPageUrl.Value = "/Hints/Flow control/terminator.aspx";
            }

            #endregion

        }

        /// <summary>
        /// �]�w�ݨ���Title
        /// </summary>
        private void setupPaperTitle()
        {
            //Get the title of this paper
            string strPaperTitle = myReceiver.getPaperTitle(strPaperID);

            txtPaperTitle.InnerText = strPaperTitle;
        }

        /// <summary>
        /// �إ���ܨC�Ӱ��D�s�ջݭn�Q������D�حӼ�
        /// </summary>
        private void setupQuestionNumTable()
        {
            Table table = new Table();
            tcQuestionNumTable.Controls.Clear();
            tcQuestionNumTable.Controls.Add(table);
            table.Attributes.Add("Class", "header1_table");
            table.Style.Add("WIDTH", "100%");
            tcQuestionTable.Align = "center";
            table.CellSpacing = 0;
            table.CellPadding = 2;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderStyle = BorderStyle.Solid;
            table.BorderColor = System.Drawing.Color.Black;

            //�qPaper_RandomQuestionNum���X���ݨ������

            string strSQL = mySQL.getPaper_RandomQuestionNum(strPaperID);
            DataSet dsQuestionNum = sqldb.getDataSet(strSQL);

            if (dsQuestionNum.Tables[0].Rows.Count > 0)
            {
                string strControl = "";
                for (int i = 0; i < dsQuestionNum.Tables[0].Rows.Count; i++)
                {
                    if (dsQuestionNum.Tables[0].Rows[i]["cQuestionGroupID"].ToString() != strControl)
                    {
                        strControl = dsQuestionNum.Tables[0].Rows[i]["cQuestionGroupID"].ToString();
                        //�]�wTable��Style
                        string strStyle = "header1_tr_even_row";
                        if ((i % 2) != 0)
                        {
                            strStyle = "header1_tr_even_row";
                        }
                        else
                        {
                            strStyle = "header1_tr_odd_row";
                        }

                        TableRow tr = new TableRow();
                        tr.CssClass = strStyle;
                        table.Rows.Add(tr);

                        //get GroupID
                        string strGroupID = "";
                        strGroupID = dsQuestionNum.Tables[0].Rows[i]["cQuestionGroupID"].ToString();

                        //get Question number
                        int intQuestionNum = 0;
                        intQuestionNum = Convert.ToInt32(dsQuestionNum.Tables[0].Rows[i]["sQuestionNum"]);

                        //get ���էO���������D�ƥ�
                        int intQuestionCount = 0;
                        if (strGroupID == "Specific")
                        {
                            strSQL = mySQL.getSpecificSelectionQuestion(strPaperID);
                        }
                        else
                        {
                            strSQL = mySQL.getGroupSelectionQuestion(strGroupID);
                        }

                        DataSet dsQuestionCount = sqldb.getDataSet(strSQL);
                        try
                        {
                            intQuestionCount = dsQuestionCount.Tables[0].Rows.Count;
                        }
                        catch
                        {
                        }
                        dsQuestionCount.Dispose();

                        /*
                        //�[�JCheckBox
                        TableCell tcBox = new TableCell();
                        tr.Cells.Add(tcBox);
                        tcBox.Width = Unit.Pixel(25);

                        CheckBox chBox = new CheckBox();
                        tcBox.Controls.Add(chBox);
                        chBox.ID = "ch" + strGroupID;
                        chBox.Attributes.Add("onclick","ShowbtnDelete();");
                        */

                        //�էO�W��
                        TableCell tcGroupName = new TableCell();
                        tr.Cells.Add(tcGroupName);

                        string strGroupName = "";
                        if (strGroupID == "Specific")
                        {
                            strGroupName = "Specific questions";
                        }
                        else
                        {
                            strGroupName = mySQL.getQuestionGroupName(strGroupID);
                        }
                        tcGroupName.Text = strGroupName;

                        //���D�Ӽ�
                        TableCell tcQuestionNum = new TableCell();
                        tr.Cells.Add(tcQuestionNum);

                        //    TextBox txtQuestionNum = new TextBox();
                        //    tcQuestionNum.Controls.Add(txtQuestionNum);
                        //    txtQuestionNum.ID = "txt" + strGroupID;
                        //    txtQuestionNum.Text = intQuestionNum.ToString();
                        ////	txtQuestionNum.Attributes.Add("onchange","ShowbtnModify();");

                        //�]�w���D�����ת���ܼƶq
                        DataTable dtQuestionLevel = new DataTable();
                        dtQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName();
                        int iQuestionLevelCount = 0;//�O���ثe���ĴX��������
                        foreach (DataRow drQuestionLevel in dtQuestionLevel.Rows)
                        {
                            //���o�]�w�C�������ת��D�ؼƶq
                            DataTable dtQuestionLevelNum = new DataTable();
                            dtQuestionLevelNum = PaperSystem.DataReceiver.GetQuestionLevelNum(strGroupID);
                            string strQuestionLevelName = drQuestionLevel["cLevelName"].ToString();
                            int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(strQuestionLevelName);
                            Label lbQuestionLevelName = new Label();
                            lbQuestionLevelName.Text = strQuestionLevelName;
                            tcQuestionNum.Controls.Add(lbQuestionLevelName);
                            int iQuestionLevelNum = 0;
                            //�N�C�������ת��ƶq��Jddl��
                            foreach (DataRow drQuestionLevelNum in dtQuestionLevelNum.Rows)
                            {
                                if (Convert.ToInt16(drQuestionLevelNum["cQuestionLevel"].ToString()) == iQuestionLevel)
                                    iQuestionLevelNum = Convert.ToInt16(drQuestionLevelNum["QuestionLevelNum"].ToString());
                            }
                            DropDownList ddlQuestionLevelNum = new DropDownList();
                            for (int iNum = 0; iNum <= iQuestionLevelNum; iNum++)
                            {
                                ddlQuestionLevelNum.Items.Add(iNum.ToString());
                            }
                            ddlQuestionLevelNum.SelectedValue = dsQuestionNum.Tables[0].Rows[iQuestionLevelCount]["sQuestionNum"].ToString();
                            iQuestionLevelCount++;
                            ddlQuestionLevelNum.ID = "ddlQuestionLevelNum_" + iQuestionLevel;
                            tcQuestionNum.Controls.Add(ddlQuestionLevelNum);
                        }

                        //���էO�����D�`��
                        TableCell tcQuestionCount = new TableCell();
                        tr.Cells.Add(tcQuestionCount);

                        tcQuestionCount.Text = intQuestionCount.ToString();
                        tcQuestionCount.Attributes.Add("Align", "Center");

                        //�إ�Title��TextArea
                        TableRow trQuestionTitle = new TableRow();
                        table.Rows.Add(trQuestionTitle);

                        TableCell tcTitle = new TableCell();
                        trQuestionTitle.Cells.Add(tcTitle);
                        tcTitle.Attributes.Add("align", "right");
                        tcTitle.ColumnSpan = 3;

                        //�إ�Question title��TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcTitle.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strGroupID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //���X��QuestionTitle�����e
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strGroupID);

                        //�إ�Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //�qORCS���Ұ�m�߭��������Ѽ�
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcTitle.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strGroupID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strGroupID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //�إ߶��j
                        Label lblCell0 = new Label();
                        lblCell0.Style.Add("WIDTH", "20px");
                        tcTitle.Controls.Add(lblCell0);

                        //�إ�Delete button
                        Button btnDeleteNum = new Button();
                        tcTitle.Controls.Add(btnDeleteNum);
                        btnDeleteNum.ID = "btnDeleteNum-" + strGroupID;
                        btnDeleteNum.Text = "Delete this topic";
                        btnDeleteNum.Click += new EventHandler(btnDeleteNum_Click);
                        btnDeleteNum.Style["width"] = "220px";
                        btnDeleteNum.Attributes["class"] = "button_blue";

                        //�إ�Empty row
                        if (i < dsQuestionNum.Tables[0].Rows.Count - 1)
                        {
                            TableRow trEmpty = new TableRow();
                            table.Rows.Add(trEmpty);
                            trEmpty.BackColor = System.Drawing.Color.White;

                            TableCell tcEmpty1 = new TableCell();
                            trEmpty.Cells.Add(tcEmpty1);
                            tcEmpty1.Style.Add("Height", "10px");
                            tcEmpty1.ColumnSpan = 3;
                        }

                        //�[�JTable��Title
                        TableRow trTitle = new TableRow();
                        table.Rows.AddAt(0, trTitle);
                        trTitle.Attributes.Add("Class", "header1_table_first_row");

                        TableCell tcGroupTitle = new TableCell();
                        trTitle.Cells.Add(tcGroupTitle);
                        tcGroupTitle.Text = "Question topic";

                        TableCell tcQuestionTitle = new TableCell();
                        trTitle.Cells.Add(tcQuestionTitle);
                        tcQuestionTitle.Text = "Selected questions number";

                        TableCell tcQuestionCountTitle = new TableCell();
                        trTitle.Cells.Add(tcQuestionCountTitle);
                        tcQuestionCountTitle.Text = "Maximum questions count";
                    }


                }
            }
            else
            {
                //�S�����
            }
            dsQuestionNum.Dispose();
        }

        private void setupQuestionTable()
        {
            Table table = new Table();
            tcQuestionTable.Controls.Add(table);
            table.CellSpacing = 0;
            table.CellPadding = 2;
            table.BorderStyle = BorderStyle.Solid;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderColor = System.Drawing.Color.Black;
            table.GridLines = GridLines.None;
            table.Width = Unit.Percentage(100);
            table.Attributes.Add("Class", "header1_table");

            intQuestionIndex = 0;
            //��l�ƦҨ��`��
            intSumScore = 0;

            #region �C�X����D
            //���X���ݨ�������D���e
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "1";
                    try
                    {
                        strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();
                    }
                    catch
                    {
                    }

                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //���o���D��SQL
                    strSQL = mySQL.getQuestion(strQID);
                    DataSet dsQuestion = sqldb.getDataSet(strSQL);

                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //�إ߰��D�����e
                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        intQuestionIndex += 1;
                        /*
                        //���D��CheckBox
                        TableCell tcCheckBox = new TableCell();
                        trQuestion.Cells.Add(tcCheckBox);
                        tcCheckBox.Width = Unit.Pixel(25);
                        tcCheckBox.Attributes.Add("onclick","ShowbtnDelete();");

                        CheckBox chQuestion = new CheckBox();
                        tcCheckBox.Controls.Add(chQuestion);
                        string strID = "";
                        try
                        {
                            strID = "ch-" + dsQuestion.Tables[0].Rows[0]["cQID"].ToString();
                        }
                        catch
                        {
                        }
                        chQuestion.ID = strID;
                        */
                        //���D���D��
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(25);
                        tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //���D�����e
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }
                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        tcQuestion.Text = strQuestion;
                        tcQuestion.Width = Unit.Percentage(100);

                        /*
                        //�إ߭ק���D��Button
                        TableCell tcModify = new TableCell();
                        trQuestion.Cells.Add(tcModify);
                        tcModify.Width = Unit.Pixel(30);
                        tcModify.HorizontalAlign = HorizontalAlign.Right;

                        Button btnModify = new Button();
                        tcModify.Controls.Add(btnModify);
                        btnModify.ID = strQID;
                        btnModify.Text = "Modify";
                        btnModify.Click+=new EventHandler(btnModify_Click);
                        */

                        //�إ߰��D��CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //�إ߿ﶵ
                        strSQL = mySQL.getAllSelections(strQID);
                        DataSet dsSelection = sqldb.getDataSet(strSQL);
                        if (dsSelection.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsSelection.Tables[0].Rows.Count; j++)
                            {
                                //Seq
                                string strSeq = "";
                                try
                                {
                                    strSeq = dsSelection.Tables[0].Rows[j]["sSeq"].ToString();
                                }
                                catch
                                {
                                }

                                //Selection
                                string strSelection = "";
                                try
                                {
                                    strSelection = dsSelection.Tables[0].Rows[j]["cSelection"].ToString();
                                }
                                catch
                                {
                                }

                                //bCaseSelect
                                bool bCaseSelect = false;
                                try
                                {
                                    bCaseSelect = Convert.ToBoolean(dsSelection.Tables[0].Rows[j]["bCaseSelect"]);
                                }
                                catch
                                {
                                }

                                TableRow trSelection = new TableRow();
                                table.Rows.Add(trSelection);

                                //�ﶵ�s���P�O�_����ĳ�ﶵ
                                TableCell tcSuggest = new TableCell();
                                trSelection.Cells.Add(tcSuggest);
                                //tcSuggest.Text = Convert.ToString((j+1)) + ".";
                                HtmlImage imgSuggest = new HtmlImage();
                                tcSuggest.Controls.Add(imgSuggest);
                                if (bCaseSelect == true)
                                {
                                    imgSuggest.Src = "/Hints/Summary/Images/smiley4.gif";
                                }
                                else
                                {
                                    imgSuggest.Src = "/Hints/Summary/Images/smiley11.gif";
                                }

                                /*
                                //�ﶵ�s��
                                TableCell tcSelectionNum = new TableCell();
                                trSelection.Cells.Add(tcSelectionNum);
                                tcSelectionNum.Text = Convert.ToString((j+1)) + ".";
                                */

                                //�ﶵ���e
                                TableCell tcSelection = new TableCell();
                                trSelection.Cells.Add(tcSelection);
                                tcSelection.Text = strSelection;

                                /*
                                //Empty TableCell
                                TableCell tcEmpty = new TableCell();
                                trSelection.Cells.Add(tcEmpty);
                                */

                                //�إ߿ﶵ��CSS
                                if ((Convert.ToInt32(strSeq) % 2) != 0)
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_even_row");
                                }
                                else
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_odd_row");
                                }
                            }
                        }
                        else
                        {
                            //�����D�S���ﶵ
                        }
                        dsSelection.Dispose();

                        //�إ�Modify and Delete button
                        TableRow trButton = new TableRow();
                        table.Rows.Add(trButton);

                        //�s�WTableCell �ΨӼW�[�]�w���ƪ�TextBox
                        TableCell tcTextArea = new TableCell();
                        trButton.Cells.Add(tcTextArea);
                        tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                        tcTextArea.VerticalAlign = VerticalAlign.Top;

                        TableCell tcButton = new TableCell();
                        trButton.Cells.Add(tcButton);
                        tcButton.HorizontalAlign = HorizontalAlign.Right;
                        tcButton.VerticalAlign = VerticalAlign.Top;

                        //�إ�Question title��TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcButton.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strQID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //���X��QuestionTitle�����e
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                        //�إ߶��j
                        Label lblCell0 = new Label();
                        lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell0);

                        //�إ�Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //�qORCS���Ұ�m�߭��������Ѽ�
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcButton.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strQID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //�إ߶��j
                        Label lblCell1 = new Label();
                        lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell1);

                        //�إ�ScoreTextBox ���g2012/12/25
                        Label lblScore = new Label();
                        tcTextArea.Controls.Add(lblScore);
                        lblScore.Text = "���ơG";
                        lblScore.Style["width"] = "50px";

                        //�إ�ScoreTextBox ���g2012/12/25
                        TextBox txtScore = new TextBox();
                        txtScore.AutoPostBack = true;
                        tcTextArea.Controls.Add(txtScore);
                        txtScore.ID = "txtScore-" + strQID;
                        txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                        txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                        txtScore.Style["width"] = "80px";
                        //�֥[�C�@�D�ت�����
                        intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                        //�إ�Modify button
                        Button btnModify = new Button();
                        tcButton.Controls.Add(btnModify);
                        btnModify.ID = "btnModify" + strQID;
                        btnModify.Text = "Modify this question";
                        btnModify.Click += new EventHandler(btnModify_Click);
                        btnModify.Style["width"] = "220px";
                        btnModify.Attributes["class"] = "button_blue";

                        //�إ߶��j
                        Label lblCel2 = new Label();
                        lblCel2.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCel2);

                        //�إ�Delete button
                        Button btnDelete = new Button();
                        tcButton.Controls.Add(btnDelete);
                        btnDelete.ID = "btnDelete" + strQID;
                        btnDelete.Text = "Delete this question";
                        btnDelete.Click += new EventHandler(btnDelete_Click);
                        btnDelete.Style["width"] = "220px";
                        btnDelete.Attributes["class"] = "button_blue";

                        if (i < dsQuestionList.Tables[0].Rows.Count - 1)
                        {
                            //�إ�Empty row
                            TableRow trEmpty = new TableRow();
                            table.Rows.Add(trEmpty);
                            trEmpty.BackColor = System.Drawing.Color.White;

                            TableCell tcEmpty = new TableCell();
                            trEmpty.Cells.Add(tcEmpty);
                            tcEmpty.ColumnSpan = 2;
                            tcEmpty.Style.Add("Height", "30px");
                        }
                    }
                    else
                    {
                        //�����D�S���ﶵ
                    }
                    dsQuestion.Dispose();


                }

            }
            else
            {
                //���ݨ��S������D�����p
            }
            dsQuestionList.Dispose();
            #endregion

            #region �C�X����D�]�t����r
            //���X���ݨ�������D���e
            TableRow trQuestionWithKeyWords = new TableRow();
            TableCell tcQuestionWithKeyWords = new TableCell();
            tcQuestionWithKeyWords.Attributes["cellspacing"] = "0";
            table.Rows.Add(trQuestionWithKeyWords);
            trQuestionWithKeyWords.Cells.Add(tcQuestionWithKeyWords);
            Table selectQuestionWithKeyWordstable = new Table();
            tcQuestionTable.Controls.Add(table);
            selectQuestionWithKeyWordstable.CellSpacing = 0;
            selectQuestionWithKeyWordstable.CellPadding = 0;
            selectQuestionWithKeyWordstable.BorderStyle = BorderStyle.Solid;
            selectQuestionWithKeyWordstable.BorderWidth = Unit.Pixel(0);
            selectQuestionWithKeyWordstable.Width = Unit.Percentage(100);
            selectQuestionWithKeyWordstable.Attributes.Add("Class", "header1_table");
            strSQL = mySQL.getPaperSelectionWithKeyWordsContent(strPaperID);
            tcQuestionWithKeyWords.Controls.Add(selectQuestionWithKeyWordstable);
            tcQuestionWithKeyWords.ColumnSpan = 2;

            DataSet dsQuestionWithKeyWordsList = sqldb.getDataSet(strSQL);
            if (dsQuestionWithKeyWordsList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionWithKeyWordsList.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "6";
                    try
                    {
                        strQuestionType = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQuestionType"].ToString();
                    }
                    catch
                    {
                    }

                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //���o���D��SQL
                    strSQL = mySQL.getQuestion(strQID);
                    DataSet dsQuestion = sqldb.getDataSet(strSQL);

                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //�إ߰��D�����e
                        TableRow trQuestion = new TableRow();
                        selectQuestionWithKeyWordstable.Rows.Add(trQuestion);

                        intQuestionIndex += 1;

                        //���D���D��
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(25);
                        tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //���D�����e
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        tcQuestion.Text = strQuestion;
                        tcQuestion.Width = Unit.Percentage(75);

                        //����r���D
                        TableCell tcKeyWordsTitle = new TableCell();
                        trQuestion.Cells.Add(tcKeyWordsTitle);
                        tcKeyWordsTitle.Text = "KeyWords";
                        tcKeyWordsTitle.Attributes["align"] = "center";
                        tcKeyWordsTitle.Width = Unit.Pixel(300);
                        tcKeyWordsTitle.BorderWidth = Unit.Pixel(1);
                        tcKeyWordsTitle.BorderColor = Color.Black;

                        //�إ߰��D��CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //�إ߿ﶵ
                        strSQL = mySQL.getAllSelections(strQID);
                        DataSet dsSelection = sqldb.getDataSet(strSQL);
                        if (dsSelection.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsSelection.Tables[0].Rows.Count; j++)
                            {
                                //Seq
                                string strSeq = "";
                                try
                                {
                                    strSeq = dsSelection.Tables[0].Rows[j]["sSeq"].ToString();
                                }
                                catch
                                {
                                }

                                //Selection
                                string strSelection = "";
                                try
                                {
                                    strSelection = dsSelection.Tables[0].Rows[j]["cSelection"].ToString();
                                }
                                catch
                                {
                                }

                                //bCaseSelect
                                bool bCaseSelect = false;
                                try
                                {
                                    bCaseSelect = Convert.ToBoolean(dsSelection.Tables[0].Rows[j]["bCaseSelect"]);
                                }
                                catch
                                {
                                }

                                TableRow trSelection = new TableRow();
                                selectQuestionWithKeyWordstable.Rows.Add(trSelection);

                                //�ﶵ�s���P�O�_����ĳ�ﶵ
                                TableCell tcSuggest = new TableCell();
                                trSelection.Cells.Add(tcSuggest);
                                //tcSuggest.Text = Convert.ToString((j+1)) + ".";
                                HtmlImage imgSuggest = new HtmlImage();
                                tcSuggest.Controls.Add(imgSuggest);
                                if (bCaseSelect == true)
                                {
                                    imgSuggest.Src = "/Hints/Summary/Images/smiley4.gif";
                                }
                                else
                                {
                                    imgSuggest.Src = "/Hints/Summary/Images/smiley11.gif";
                                }

                                //�ﶵ���e
                                TableCell tcSelection = new TableCell();
                                trSelection.Cells.Add(tcSelection);
                                tcSelection.Text = strSelection;


                                //�b�ﶵ�Ĥ@�C�[�J����r���
                                if (j == 0)
                                {
                                    //���o�D������r
                                    string strKeyWords = "";
                                    strKeyWords = dsQuestion.Tables[0].Rows[0]["cKeyWords"].ToString();
                                    TableCell tcKeyWords = new TableCell();
                                    trSelection.Cells.Add(tcKeyWords);
                                    tcKeyWords.RowSpan = dsSelection.Tables[0].Rows.Count;
                                    tcKeyWords.Text = strKeyWords;
                                    tcKeyWords.BorderWidth = Unit.Pixel(1);
                                    tcKeyWords.BorderColor = Color.Black;
                                }

                                //�إ߿ﶵ��CSS
                                if ((Convert.ToInt32(strSeq) % 2) != 0)
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_even_row");
                                }
                                else
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_odd_row");
                                }
                            }
                        }
                        else
                        {
                            //�����D�S���ﶵ
                        }
                        dsSelection.Dispose();

                        //�إ�Modify and Delete button
                        TableRow trButton = new TableRow();
                        selectQuestionWithKeyWordstable.Rows.Add(trButton);

                        //�s�WTableCell �ΨӼW�[�]�w���ƪ�TextBox
                        TableCell tcTextArea = new TableCell();
                        trButton.Cells.Add(tcTextArea);
                        tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                        tcTextArea.VerticalAlign = VerticalAlign.Top;

                        TableCell tcButton = new TableCell();
                        trButton.Cells.Add(tcButton);
                        tcButton.HorizontalAlign = HorizontalAlign.Right;
                        tcButton.VerticalAlign = VerticalAlign.Top;
                        tcButton.ColumnSpan = 2;

                        //�إ�Question title��TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcButton.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strQID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //���X��QuestionTitle�����e
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                        //�إ߶��j
                        Label lblCell0 = new Label();
                        lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell0);

                        //�إ�Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //�qORCS���Ұ�m�߭��������Ѽ�
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcButton.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strQID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //�إ߶��j
                        Label lblCell1 = new Label();
                        lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell1);

                        //�إ�ScoreTextBox ���g2012/12/25
                        Label lblScore = new Label();
                        tcTextArea.Controls.Add(lblScore);
                        lblScore.Text = "���ơG";
                        lblScore.Style["width"] = "50px";

                        //�إ�ScoreTextBox ���g2012/12/25
                        TextBox txtScore = new TextBox();
                        txtScore.AutoPostBack = true;
                        tcTextArea.Controls.Add(txtScore);
                        txtScore.ID = "txtScore-" + strQID;
                        txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                        txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                        txtScore.Style["width"] = "80px";
                        //�֥[�C�@�D�ت�����
                        intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                        //�إ�Modify button
                        Button btnModify = new Button();
                        tcButton.Controls.Add(btnModify);
                        btnModify.ID = "btnModify" + strQID;
                        btnModify.Text = "Modify this question";
                        btnModify.Click += new EventHandler(btnModify_Click);
                        btnModify.Style["width"] = "220px";
                        btnModify.Attributes["class"] = "button_blue";

                        //�إ߶��j
                        Label lblCel2 = new Label();
                        lblCel2.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCel2);

                        //�إ�Delete button
                        Button btnDelete = new Button();
                        tcButton.Controls.Add(btnDelete);
                        btnDelete.ID = "btnDelete" + strQID;
                        btnDelete.Text = "Delete this question";
                        btnDelete.Click += new EventHandler(btnDelete_Click);
                        btnDelete.Style["width"] = "220px";
                        btnDelete.Attributes["class"] = "button_blue";

                        if (i < dsQuestionWithKeyWordsList.Tables[0].Rows.Count - 1)
                        {
                            //�إ�Empty row
                            TableRow trEmpty = new TableRow();
                            selectQuestionWithKeyWordstable.Rows.Add(trEmpty);
                            trEmpty.BackColor = System.Drawing.Color.White;

                            TableCell tcEmpty = new TableCell();
                            trEmpty.Cells.Add(tcEmpty);
                            tcEmpty.ColumnSpan = 3;
                            tcEmpty.Style.Add("Height", "30px");
                        }
                    }
                    else
                    {
                        //�����D�S���ﶵ
                    }
                    dsQuestion.Dispose();
                }
            }
            else
            {
                //���ݨ��S������D�]�t����r�����p
            }
            dsQuestionList.Dispose();
            #endregion

            #region �C�X�ݵ��D
            //�إ��ݩ󦹰ݨ����ݵ��D
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            int intTextCount = 0;
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //���o�����D��QID
                    string strQID = "";
                    strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();

                    //Question
                    string strQuestion = "";
                    strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();

                    //Answer
                    string strAnswer = "";
                    strAnswer = dsTextList.Tables[0].Rows[i]["cAnswer"].ToString();


                    TableRow trQuestion = new TableRow();
                    table.Rows.Add(trQuestion);
                    trQuestion.Attributes.Add("Class", "header1_table_first_row");

                    TableRow trAnswer = new TableRow();
                    trAnswer.Attributes.Add("Class", "header1_tr_even_row");
                    table.Rows.Add(trAnswer);


                    //Question number
                    intQuestionIndex += 1;
                    TableCell tcTextNum = new TableCell();
                    trQuestion.Cells.Add(tcTextNum);
                    tcTextNum.Width = Unit.Pixel(25);
                    tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                    TableCell tcTextNumAnswer = new TableCell();
                    trAnswer.Cells.Add(tcTextNumAnswer);
                    tcTextNumAnswer.Width = Unit.Pixel(25);
                    tcTextNumAnswer.Text = "A" + intQuestionIndex.ToString() + ": ";

                    //Question
                    TableCell tcQuestion = new TableCell();
                    trQuestion.Cells.Add(tcQuestion);
                    tcQuestion.Text = strQuestion;

                    //Answer
                    TableCell tcAnswer = new TableCell();
                    trAnswer.Cells.Add(tcAnswer);
                    tcAnswer.Text = strAnswer;

                    //�إ߭ק���s��TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //�s�WTableCell �ΨӼW�[�]�w���ƪ�TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //�إ�Question title��TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    //���X��QuestionTitle�����e
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                    //�إ߶��j
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //�إ�Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //�qORCS���Ұ�m�߭��������Ѽ�
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //�إ�ScoreTextBox ���g2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "���ơG";
                    lblScore.Style["width"] = "50px";

                    //�إ�ScoreTextBox ���g2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //�֥[�C�@�D�ت�����
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //�إ߶��j
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //�إ߭ק���D��Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "220px";
                    btnModifyText.ID = "btnModifyText-" + strQID;
                    btnModifyText.Text = "Modify this question";
                    btnModifyText.Click += new EventHandler(btnModifyText_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //�إ߶��j
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //�إߧR���ݵ��D��Button
                    Button btnDeleteText = new Button();
                    tcModify.Controls.Add(btnDeleteText);
                    btnDeleteText.Style["width"] = "220px";
                    btnDeleteText.ID = "btnDeleteText-" + strQID;
                    btnDeleteText.Text = "Delete this question";
                    btnDeleteText.Click += new EventHandler(btnDeleteText_Click);
                    btnDeleteText.Attributes["class"] = "button_blue";

                    //�[�JCSS
                    intTextCount += 1;

                    //if((intTextCount % 2) != 0)
                    //{
                    //    trQuestion.Attributes.Add("Class","header1_tr_even_row");	
                    //}
                    //else
                    //{
                    //    trQuestion.Attributes.Add("Class","header1_tr_odd_row");
                    //}
                }
            }
            else
            {
                //���ݨ��S���ݵ��D������
            }
            dsTextList.Dispose();
            #endregion

            #region �C�X�����D
            //�إ��ݩ󦹰ݨ����ݵ��D
            strSQL = mySQL.getPaperSituationContent(strPaperID);
            DataSet dsSituationList = sqldb.getDataSet(strSQL);
            int intSituationCount = 0;

            if (dsSituationList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsSituationList.Tables[0].Rows.Count; i++)
                {
                    //���o�����D��QID
                    string strQID = "";
                    strQID = dsSituationList.Tables[0].Rows[i]["cQID"].ToString();

                    //Question
                    string strQuestion = "";
                    strQuestion = dsSituationList.Tables[0].Rows[i]["cQuestion"].ToString();

                    //Information
                    string strInformation = "";
                    strInformation = dsSituationList.Tables[0].Rows[i]["strInformation"].ToString();

                    TableRow trSituationQuestion = new TableRow();
                    table.Rows.Add(trSituationQuestion);
                    trSituationQuestion.Attributes.Add("Class", "header1_table_first_row");

                    TableRow trSituationInformation = new TableRow();
                    trSituationInformation.Attributes.Add("Class", "header1_tr_even_row");
                    table.Rows.Add(trSituationInformation);

                    //Question number
                    intQuestionIndex += 1;
                    TableCell tcSituationNum = new TableCell();
                    trSituationQuestion.Cells.Add(tcSituationNum);
                    tcSituationNum.Width = Unit.Pixel(200);
                    tcSituationNum.Text = "�����D Question" + intQuestionIndex.ToString() + ": ";

                    TableCell tcSituationNumInformation = new TableCell();
                    trSituationInformation.Cells.Add(tcSituationNumInformation);
                    tcSituationNumInformation.Width = Unit.Pixel(200);
                    tcSituationNumInformation.Text = "Information" + intQuestionIndex.ToString() + ": ";

                    //Question
                    TableCell tcQuestion = new TableCell();
                    trSituationQuestion.Cells.Add(tcQuestion);
                    tcQuestion.Text = strQuestion;

                    //Answer
                    TableCell tcAnswer = new TableCell();
                    trSituationInformation.Cells.Add(tcAnswer);
                    tcAnswer.Text = strInformation;

                    //�إ߭ק���s��TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //�s�WTableCell �ΨӼW�[�]�w���ƪ�TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //�إ�Question title��TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    //���X��QuestionTitle�����e
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                    //�إ߶��j
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //�إ�Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //�qORCS���Ұ�m�߭��������Ѽ�
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //�إ�ScoreTextBox ���g2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "���ơG";
                    lblScore.Style["width"] = "50px";

                    //�إ�ScoreTextBox ���g2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //�֥[�C�@�D�ت�����
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //�إ߶��j
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //�إ߭ק���D��Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "220px";
                    btnModifyText.ID = "btnModifySituation-" + strQID;
                    btnModifyText.Text = "Modify this question";
                    btnModifyText.Click += new EventHandler(btnModifySituation_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //�إ߶��j
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //�إߧR���ݵ��D��Button
                    Button btnDeleteText = new Button();
                    tcModify.Controls.Add(btnDeleteText);
                    btnDeleteText.Style["width"] = "220px";
                    btnDeleteText.ID = "btnDeleteText-" + strQID;
                    btnDeleteText.Text = "Delete this question";
                    btnDeleteText.Click += new EventHandler(btnDeleteSituation_Click);
                    btnDeleteText.Attributes["class"] = "button_blue";

                    intSituationCount += 1;
                }
            }
            else
            {
                //�S�������D 
            }

            #endregion

            #region �C�X�ϧ��D
            //�̷�QuestionMode�M�w���X���էO���ϧ��D
            strSQL = mySQL.getPaperSimulationContent(strPaperID);

            DataSet dsQuestionList_simulation = sqldb.getDataSet(strSQL);
            if (dsQuestionList_simulation.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList_simulation.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "3";
                    strQuestionType = dsQuestionList_simulation.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //���oQID
                    string strQID = "";
                    strQID = dsQuestionList_simulation.Tables[0].Rows[i]["cQID"].ToString();

                    //���o���D��SQL
                    DataSet dsQuestion = null;
                    //if (hfSymptoms.Value == "All")
                    strSQL = mySQL.getQuestion(strQID);
                    //NO Symptoms
                    //else
                    //    strSQL = mySQL.getQuestionBySymptoms(strQID, hfSymptoms.Value);

                    dsQuestion = sqldb.getDataSet(strSQL);
                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //�إ߰��D�����e
                        TableRow trQuestion = new TableRow();
                        trQuestion.Style.Add("CURSOR", "hand");
                        table.Rows.Add(trQuestion);

                        intQuestionIndex += 1;

                        //���D���D��
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(150);
                        //tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";
                        tcQuestionNum.Text = "<IMG id='img_" + strQID + "' src='../../../BasicForm/Image/minus.gif'>&nbsp;Q" + intQuestionIndex.ToString() + " : ";

                        //���D�����e
                        string strQuestion = "";
                        strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();

                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        tcQuestion.Text = strQuestion;

                        //�إ߰��D��CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //���osimulator�������Υ����٦�����
                        strSQL = mySQL.getQuestion_sim(strQID);
                        DataSet dsQuestion_sim = sqldb.getDataSet(strSQL);
                        //�إ߹ϧ��D������

                        TableRow trScene = new TableRow();
                        trScene.Height = 290;
                        table.Rows.Add(trScene);

                        TableCell tcSceneTitle = new TableCell();
                        trScene.Cells.Add(tcSceneTitle);
                        tcSceneTitle.Text = "<font style='color:Black; font-weight:bold'>Scene :&nbsp;<font/>";
                        tcSceneTitle.Style.Add("text-align", "right");
                        tcSceneTitle.Width = Unit.Pixel(230);
                        //���o����ID
                        string strSimuID = dsQuestion_sim.Tables[0].Rows[0]["cSimulatorID"].ToString();
                        string str_URL = "";
                        DataTable dtTemp = new DataTable();
                        if (strSimuID.Contains("Internal Medicine|General|1"))
                        {
                            string str_VRID = "Simulator_20100928144239";
                            strSQL = "SELECT * FROM SimulatorBackground WHERE SimulatorID LIKE '" + str_VRID + "'";
                            dtTemp = sqldb.getDataSet(strSQL).Tables[0];
                            str_URL = dtTemp.Rows[0]["bgUrl"].ToString();
                        }
                        else if (strSimuID.Contains("Internal Medicine|General|2"))
                        {
                            str_URL = "http://140.116.72.28/HintsCase/FileCollection/0101/201108/File20110817120244.JPG";
                        }
                        else
                        {
                            strSQL = "SELECT * FROM SimulatorBackground WHERE SimulatorID LIKE '" + strSimuID + "'";
                            dtTemp = sqldb.getDataSet(strSQL).Tables[0];
                            str_URL = dtTemp.Rows[0]["bgUrl"].ToString();
                        }
                        //strSQL = "SELECT * FROM SimulatorBackground WHERE SimulatorID LIKE '" + strSimuID + "'";
                        // DataTable dtTemp = sqldb.getDataSet(strSQL).Tables[0];

                        TableCell tcSceneImg = new TableCell();
                        trScene.Cells.Add(tcSceneImg);
                        HtmlImage h_image = new HtmlImage();
                        h_image.Src = str_URL;
                        h_image.Width = 320;
                        h_image.Height = 280;
                        tcSceneImg.Controls.Add(h_image);
                        tcSceneImg.Attributes.Add("Class", "header1_tr_even_row");
                        tcSceneImg.Width = Unit.Percentage(81);
                        //�Y��
                        //trQuestion.Attributes.Add("onclick", "ShowDetail('" + strQID + "','img_" + strQID + "','" + hfAnswerCount.Value + "')");
                        //�إ߹ϧ��D�����T�ѵ��H�ζ���
                        if (dsQuestion_sim.Tables[0].Rows.Count > 0)
                        {
                            int s_no = 0;
                            for (int s = 0; s < dsQuestion_sim.Tables[0].Rows.Count; s++)
                            {
                                s_no = s + 1;
                                //���צ�
                                TableRow trAnswer = new TableRow();
                                table.Rows.Add(trAnswer);

                                TableCell tcAnswerTitle = new TableCell();
                                trAnswer.Cells.Add(tcAnswerTitle);
                                tcAnswerTitle.Text = "<font style='color:Black; font-weight:bold'>Answer " + s_no.ToString() + " :&nbsp;<font/>";
                                tcAnswerTitle.Style.Add("text-align", "right");
                                tcAnswerTitle.Width = Unit.Pixel(230);
                                //�m�J����
                                TableCell tcAnswerValue = new TableCell();
                                trAnswer.Cells.Add(tcAnswerValue);
                                tcAnswerValue.Attributes.Add("Class", "header1_tr_even_row");
                                tcAnswerValue.Width = Unit.Percentage(81);
                                string temp_ans = dsQuestion_sim.Tables[0].Rows[s]["cAnswer"].ToString().Replace('|', ',');
                                tcAnswerValue.Text = temp_ans.Substring(0, temp_ans.Length - 1);
                                //���׶��Ǧ�
                                TableRow trAnswerOrder = new TableRow();
                                table.Rows.Add(trAnswerOrder);

                                TableCell tcAns_order_Title = new TableCell();
                                trAnswerOrder.Cells.Add(tcAns_order_Title);
                                tcAns_order_Title.Text = "<font style='color:Black; font-weight:bold'>Answer " + s_no.ToString() + " order:&nbsp;<font/>";
                                tcAns_order_Title.Style.Add("text-align", "right");
                                tcAns_order_Title.Width = Unit.Pixel(230);
                                //�m�J����
                                TableCell tcAnswer_Order = new TableCell();
                                trAnswerOrder.Cells.Add(tcAnswer_Order);
                                tcAnswer_Order.Attributes.Add("Class", "header1_tr_even_row");
                                tcAnswer_Order.Width = Unit.Percentage(81);
                                string temp_order = dsQuestion_sim.Tables[0].Rows[s]["cOrder"].ToString().Replace('|', ',');
                                tcAnswer_Order.Text = temp_order.Substring(0, temp_order.Length - 1);

                            }
                        }
                        //Modify and Delete ���s��TableRow
                        TableRow trModify = new TableRow();
                        table.Rows.Add(trModify);
                        trModify.ID = "trModify_" + strQID;

                        TableCell tcNone = new TableCell();
                        trModify.Cells.Add(tcNone);

                        //�إ߭ק���D��Button
                        TableCell tcModify = new TableCell();
                        trModify.Cells.Add(tcModify);
                        tcModify.Attributes["align"] = "right";

                        //Button btnModifySim = new Button();
                        //tcModify.Controls.Add(btnModifySim);
                        //btnModifySim.ID = "btnModifySim-" + strQID;
                        //btnModifySim.Text = "Modify";
                        //btnModifySim.Click += new EventHandler(btnModifySim_Click);
                        //btnModifySim.CommandArgument = strQID;
                        //btnModifySim.Style["width"] = "150px";
                        //btnModifySim.CssClass = "button_continue";

                        //�إ߶��j
                        //Label lblCell = new Label();
                        //tcModify.Controls.Add(lblCell);
                        //lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        Button btnDeleteSimu = new Button();
                        tcModify.Controls.Add(btnDeleteSimu);
                        btnDeleteSimu.ID = "btnDeleteSelection-" + strQID;
                        btnDeleteSimu.Text = "Delete";
                        btnDeleteSimu.CommandArgument = strQID;
                        btnDeleteSimu.Click += new EventHandler(btnDeleteSimu_Click);
                        btnDeleteSimu.Style["width"] = "150px";
                        btnDeleteSimu.CssClass = "button_continue";


                        //�إ�Space
                        TableRow trSpace = new TableRow();
                        trSpace.Style.Add("background-color", "#EBECED");
                        table.Rows.Add(trSpace);

                        TableCell tcSpace = new TableCell();
                        tcSpace.ColumnSpan = 2;
                        tcSpace.Style.Add("height", "7px");
                        trSpace.Cells.Add(tcSpace);

                    }
                }
            }
            #endregion
            //���Ҩ��`��
            lblTotalScore.Text = intSumScore.ToString();
        }
        //�qORCS���Ұ�m�߭��������Ѽ�
        private void getParameterFromORCSExercise()
        {
            //SystemFunction
            if (Session["SystemFunction"] != null)
            {
                Session["SystemFunction"] = "PreviewPaper";
            }
            else
            {
                Session.Add("SystemFunction", "PreviewPaper");
            }

            //UserID
            if (Request.QueryString["UserID"] != null)
            {
                strUserID = Request.QueryString["UserID"].ToString();
                Session["UserID"] = strUserID;
            }
            else if (Session["UserID"] != null)
            {
                strUserID = Session["UserID"].ToString();
            }

            //CaseID
            if (Request.QueryString["cCaseID"] != null)
            {
                if (Session["CaseID"] != null)
                {
                    Session["CaseID"] = Request.QueryString["cCaseID"].ToString();
                    strCaseID = Request.QueryString["cCaseID"].ToString();
                }
                else
                {
                    Session["CaseID"] = Request.QueryString["cCaseID"].ToString();
                    strCaseID = Session["CaseID"].ToString();
                }
            }
            else if (Session["CaseID"] != null)
            {
                strCaseID = Session["CaseID"].ToString();
            }

            //Division�ѩ�q�Ұ�m�߭�����ӨS�������A�ҥH�T�w��0101(ncku)
            if (Session["DivisionID"] != null)
            {
                Session["DivisionID"] = "0101";
                strDivisionID = Session["DivisionID"].ToString();
            }
            else
            {
                Session["DivisionID"] = "0101";
                strDivisionID = Session["DivisionID"].ToString();
            }

            //ClinicNum �ѩ�q�Ұ�m�߭�����ӨS���ݶE���A�ҥH�T�w��1
            if (Session["ClinicNum"] != null)
            {
                Session["ClinicNum"] = "1";
                strClinicNum = Session["ClinicNum"].ToString();
            }
            else
            {
                Session["ClinicNum"] = "1";
                strClinicNum = Session["ClinicNum"].ToString();
            }

            //SectionName
            if (Request.QueryString["cSectionName"] != null)
            {
                if (Session["SectionName"] != null)
                {
                    Session["SectionName"] = Request.QueryString["cSectionName"].ToString();
                    strSectionName = Session["SectionName"].ToString();
                }
                else
                {
                    Session["SectionName"] = Request.QueryString["cSectionName"].ToString();
                    strSectionName = Session["SectionName"].ToString();
                }
            }
            else if (Session["SectionName"] != null)
            {
                strSectionName = Session["SectionName"].ToString();
            }

            //QuestionMode
            if (Session["QuestionMode"] != null)
            {
                Session["QuestionMode"] = "General";
                hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
            }
            else
            {
                Session["QuestionMode"] = "General";
                hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
            }

            //PresentType
            if (Session["PresentType"] != null)
            {
                Session["PresentType"] = "Edit";
                hiddenPresentType.Value = Session["PresentType"].ToString();
            }
            else
            {
                Session["PresentType"] = "Edit";
                hiddenPresentType.Value = Session["PresentType"].ToString();
            }

            //Edit method
            if (Session["EditMode"] != null)
            {
                Session["EditMode"] = "Manual";
                hiddenEditMode.Value = Session["EditMode"].ToString();
            }
            else
            {
                Session["EditMode"] = "Manual";
                hiddenEditMode.Value = Session["EditMode"].ToString();
            }

            //�[�JSession ModifyType
            if (Session["ModifyType"] != null)
            {
                Session["ModifyType"] = "Paper";
            }
            else
            {
                Session["ModifyType"] = "Paper";
            }

            //�[�JSession PreOpener(�P�_�qORCS���Ұ�m�ߨӥ�)
            if (Session["PreOpener"] != null)
            {
                Session["PreOpener"] = "SelectPaperMode";
            }
            else
            {
                Session["PreOpener"] = "SelectPaperMode";
            }

            //�[�JORCS��ExerciseID�MGroupID
            if (Session["ExerciseIDGroupID"] != null)
            {
                if (Request.QueryString["cExerciseIDcGroupID"] != null)
                    Session["ExerciseIDGroupID"] = Request.QueryString["cExerciseIDcGroupID"].ToString();
            }
            else
            {
                if (Request.QueryString["cExerciseIDcGroupID"] != null)
                    Session.Add("ExerciseIDGroupID", Request.QueryString["cExerciseIDcGroupID"].ToString());
            }

            //�Ω�s�W�D�إ�
            if (Request.QueryString["bIsFromClassExercise"] != null)
            {
                Session["IsFromClassExercise"] = Request.QueryString["bIsFromClassExercise"].ToString();
            }

            //�Ω�^���ܦҨ�������
            if (Request.QueryString["bIsFromSelectExistPaper"] != null)
            {
                Session["IsFromSelectExistPaper"] = Request.QueryString["bIsFromSelectExistPaper"].ToString();
                hiddenbIsFromSelectExistPaper.Value = Request.QueryString["bIsFromSelectExistPaper"].ToString();
            }
            else if (Session["IsFromSelectExistPaper"] != null)
            {
                hiddenbIsFromSelectExistPaper.Value = Session["IsFromSelectExistPaper"].ToString();
            }
        }
        //�qHINTS��Case���������Ѽ�
        private void getParameter()
        {
            //SystemFunction
            if (Session["SystemFunction"] != null)
            {
                Session["SystemFunction"] = "PreviewPaper";
            }
            else
            {
                Session.Add("SystemFunction", "PreviewPaper");
            }

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

            //�[�JSession PreOpener(�M���q�Ұ�m�ߨӪ�PreOpener Session)
            if (Session["PreOpener"] != null)
            {
                Session["PreOpener"] = "";
            }
            else
            {
                Session.Add("PreOpener", "");
            }

            //�M��ORCS��ExerciseID�MGroupID��Session
            if (Session["ExerciseIDGroupID"] != null)
            {
                Session["ExerciseIDGroupID"] = "";
            }
            else
            {
                Session.Add("ExerciseIDGroupID", "");
            }
        }


        private void InitiateParameterFromMLAS()
        {
            if (Request.QueryString["UserID"] != null)
            {
                Session["UserID"] = Request.QueryString["UserID"].ToString();
            }
            if (Request.QueryString["CaseID"] != null)
            {
                Session["CaseID"] = Request.QueryString["CaseID"].ToString();
                Session["fromVRSimulator"] = Request.QueryString["CaseID"].ToString();
                Session["ModifyType"] = "Paper";
            }
            if (Request.QueryString["ClinicNum"] != null)
            {
                Session["ClinicNum"] = Request.QueryString["ClinicNum"].ToString();
            }
            if (Request.QueryString["DivisionID"] != null)
            {
                Session["DivisionID"] = Request.QueryString["DivisionID"].ToString();
            }
            if (Request.QueryString["SectionName"] != null)
            {
                Session["SectionName"] = Request.QueryString["SectionName"].ToString();
            }

        }
        //�]�w�q�Ұ�m�߭����i�ӮɡA�����������
        private void SetPageStyle()
        {
            btnPre.Value = "Cancel";
            btnFinish3.Visible = true;
            btnFinish.Visible = false;
            btnPre.ServerClick += btnPre_ServerClick;
        }


        private void btnPre_ServerClick(object sender, EventArgs e)
        {
            string strScript = "<script language='javascript'>";

            string strHiddenOpener = hiddenOpener.Value;

            string strBack = hiddenOpener.Value;

            if(hiddenbIsFromSelectExistPaper.Value=="True")//�q��ܦҨ�������
            {
                string strURL = "/HINTS/Learning/Exercise/SelectExistPaper.aspx?Opener=SelectPaperMode&cCaseID=" + strCaseID + "&cSectionName=" + strSectionName + "&cPaperID=" + strPaperID + "&cExerciseIDcGroupID=" + Session["ExerciseIDGroupID"].ToString();
                strScript += "window.resizeTo(700, 800);window.location.assign('" + strURL + "');";
            }else if(hiddenOpener.Value == "SelectPaperMode")//�Y�q�Ұ�m����Ӫ��A�h������������
            {
                strScript += "window.close();";
            }else{
                strScript += "location.href = Back;";
            }
            //strURL = "/HINTS/AuthoringTool/CaseEditor/Paper/Paper_MainPage.aspx?Opener=SelectPaperMode&cCaseID=" + drPaperClassify["cCaseID"].ToString() + "&cSectionName=" + drPaperClassify["cSectionName"].ToString() + "&cPaperID=" + drPaperClassify["cPaperID"].ToString() + "&cExerciseIDcGroupID=" + strExerciseIDstrGroupID + "&bIsFromClassExercise = True&bIsFromSelectExistPaper=True";
            //string js = "javascript:window.moveTo(0,0);window.resizeTo(screen.width,screen.height);window.location.assign('" + strURL + "')";
            strScript += "</script>";

            if (Session["IsFromSelectExistPaper"] != null)
            {
                Session.Remove("IsFromSelectExistPaper");
            }

            Page.RegisterStartupScript("ShowMsg", strScript);
        }


        //�ˬdORCS�O�_���W�Ҫ��A�æ^�ǽҵ{ID
        protected string CheckClassState()
        {
            clsORCSDB myDb = new clsORCSDB(); //�I�sORCS��Ʈw
            string strClassID = ""; //�w�q���b�W�Ҫ��ҵ{ID
            //����ϥΪ̪��ҵ{ID
            string strSQL = "SELECT * FROM " + clsGroup.TB_ORCS_MemberCourseTeacher + " WHERE cUserID = '" + usi.UserID + "'";
            DataTable dtGroupMember = new DataTable();
            dtGroupMember = myDb.GetDataSet(strSQL).Tables[0];
            if (dtGroupMember.Rows.Count > 0)
            {
                //������b�W�Ҫ��ҵ{ID
                foreach (DataRow drGroupMember in dtGroupMember.Rows)
                {
                    strSQL = "SELECT * FROM ORCS_SystemControl WHERE iClassGroupID = '" + drGroupMember["iGroupID"].ToString() + "' AND cSysControlName = 'SystemControl'";
                    DataTable dtSystemControl = new DataTable();
                    dtSystemControl = myDb.GetDataSet(strSQL).Tables[0];
                    if (dtSystemControl.Rows.Count > 0)
                    {
                        if (dtSystemControl.Rows[0]["iSysControlParam"].ToString() != "0") //�P�_�ӽҵ{�O�_�W��("0":�D�W��,"1":�W��,"2":�W�ҿ��)
                            strClassID = dtSystemControl.Rows[0]["iClassGroupID"].ToString();
                    }
                }
                //�Y�����W�ҹw�]���ϥΪ̩Ҿ֦��ҵ{ID
                if (strClassID.Equals(""))
                {
                    strClassID = dtGroupMember.Rows[0]["iGroupID"].ToString();
                }
            }
            return strClassID;//�^�ǽҵ{ID
        }

        #region Web Form �]�p�u�㲣�ͪ��{���X
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: ���� ASP.NET Web Form �]�p�u��һݪ��I�s�C
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// �����]�p�u��䴩�ҥ�������k - �ФŨϥε{���X�s�边�ק�
        /// �o�Ӥ�k�����e�C
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

       
       
        [WebMethod(EnableSession = true)]
        public static void  LatchPreviousPage(string PreviousPageURL)
        {
            
            HttpContext.Current.Session["PreviousPageURL"] = PreviousPageURL;

            //return HttpContext.Current.Session["PreviousPageURL"].ToString();
        }




        void btnModifyQuestion_ServerClick(object sender, EventArgs e)
        {
            

            //�x�s���ݨ���Title
            string strPaperTitle = txtPaperTitle.InnerText;
            mySQL.UpdatePaperTitleOfPaper_Header(strPaperID, strPaperTitle);

            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //����D�t����r��g�z�ѳ̤j�r��
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);

            //���X���ݨ�������D���e
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "1";
                    strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //���oQID
                    string strQID = "";
                    strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();

                    //�x�s���D��Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strQID)));

                    if (txtTitle != null) //�W�[�P�_���D��Title�O�_���šA�קK�X�{����L�kŪ��null��Bug ���� 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //�I�s�x�sQuestion title���禡
                            mySQL.SaveToPaper_ItemTitle(strPaperID, strQID, "0", strTitle);
                        }
                    }
                }
            }

            //���X���ݨ����ݵ��D���e
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "1";
                    strQuestionType = dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //���oQID
                    string strQID = "";
                    strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();

                    //�x�s���D��Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strQID)));

                    if (txtTitle != null) //�W�[�P�_���D��Title�O�_���šA�קK�X�{����L�kŪ��null��Bug ���� 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //�I�s�x�sQuestion title���禡
                            mySQL.SaveToPaper_ItemTitle(strPaperID, strQID, "0", strTitle);
                        }
                    }
                }
            }

            //�ק���D���Ӽ�
            strSQL = mySQL.getPaper_RandomQuestionNum(strPaperID);
            DataSet dsQuestionNum = sqldb.getDataSet(strSQL);
            //bool bIsNum = true;//�P�_��Text��줺���ȬO���O�Ʀr
            //bool bOverFlow = false;//�P�_��Text��줺���ȤͨS���W�L�W��
            if (dsQuestionNum.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionNum.Tables[0].Rows.Count; i++)
                {
                    //get GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionNum.Tables[0].Rows[i]["cQuestionGroupID"].ToString();
                    }
                    catch
                    {
                    }

                    //�x�s���D��Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    try
                    {
                        txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strGroupID)));
                    }
                    catch
                    {
                    }
                    if (txtTitle != null) //�W�[�P�_���D��Title�O�_���šA�קK�X�{����L�kŪ��null��Bug ���� 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //�I�s�x�sQuestion title���禡
                            mySQL.SaveToPaper_ItemTitle(strPaperID, strGroupID, "1", strTitle);
                        }
                    }

                    //get GroupName
                    string strGroupName = mySQL.getQuestionGroupName(strGroupID);

                    //get txtQuestionNum
                    //int intQuestionNum = 0;
                    //try
                    //{
                    //    intQuestionNum = Convert.ToInt32(((TextBox)(this.FindControl("Form1").FindControl("txt" + strGroupID))).Text);
                    //}
                    //catch
                    //{
                    //    bIsNum = false;

                    //    //��ܿ��~�T���A��J���Ʀr�����T
                    //    string strScript = "<script language='javascript'>\n";
                    //    strScript += "ShowErrorMsg('The " + strGroupName + " group has an error');\n";
                    //    strScript += "</script>\n";
                    //    Page.RegisterStartupScript("ShowErrorMsg", strScript);
                    //}

                    //if (bIsNum == true)
                    //{
                    //    //get ���էO���������D�ƥ�
                    //    int intQuestionCount = 0;
                    //    strSQL = mySQL.getGroupSelectionQuestion(strGroupID);
                    //    DataSet dsQuestionCount = sqldb.getDataSet(strSQL);
                    //    try
                    //    {
                    //        intQuestionCount = dsQuestionCount.Tables[0].Rows.Count;
                    //    }
                    //    catch
                    //    {
                    //    }
                    //    dsQuestionCount.Dispose();

                    //if (intQuestionCount >= intQuestionNum)
                    //{
                    //    //���Ʀs�JPaper_RandomQuestion
                    //    //mySQL.saveRandomQuestionNum(strPaperID, strGroupID, intQuestionNum);
                    //}
                    //else
                    //{
                    //    bOverFlow = true;

                    //    //��ܿ��~�T���A�ϥΪ̳]�w���Ȥj�󦹲էO�����D�ƥءC
                    //    string strScript = "<script language='javascript'>\n";
                    //    strScript += "ShowErrorMsg('The questions number of " + strGroupName + " should less than " + intQuestionCount.ToString() + "');\n";
                    //    strScript += "</script>\n";
                    //    Page.RegisterStartupScript("ShowErrorMsg", strScript);
                    //}

                    //�N�ϥΪ̿�ܰ��D�����ת��ƶq�s�J��ƪ�
                    for (int iForm = 0; iForm < Request.Form.Count; iForm++)
                    {
                        if (Request.Form.Keys[iForm].ToString().IndexOf("ddlQuestionLevelNum_") != -1)
                        {
                            string[] strQuestionLevelNum = Request.Form.Keys[iForm].ToString().Split('_');
                            int iQuestionLevel = Convert.ToInt16(strQuestionLevelNum[1]);
                            int iQuestionLevelNum = Convert.ToInt16(Request.Form[iForm].ToString());
                            mySQL.saveRandomQuestionNum(strPaperID, strGroupID, iQuestionLevelNum, iQuestionLevel);
                        }
                    }

                    //}
                }
            }
            else
            {
                //�S�����
            }

            if (Session["fromVRSimulator"] != null && rb1.Checked == true)
            {
                Response.Redirect("/Hints/RedirectAndClose.aspx?Msg=Finish");
            }
            else if (Session["fromVRSimulator"] != null && rb2.Checked == true)
            {
                Session["fromVRSimulator"] = "vr" + Session["fromVRSimulator"].ToString();
                string strScriptString = "<script language='javascript'>\n";
                strScriptString += "goNext();\n";
                strScriptString += "</script>\n";
                Page.RegisterStartupScript("goNext", strScriptString);
            }
            else
            {
                string strScriptString = "<script language='javascript'>\n";
                strScriptString += "goNext();\n";
                strScriptString += "</script>\n";
                Page.RegisterStartupScript("goNext", strScriptString);
            }
            //if (bIsNum == true && bOverFlow == false)
            //{
            //�I�sgoNext
            
            //}
        }

        void btnDeleteQuestion_ServerClick(object sender, EventArgs e)
        {
            //�N�Q�Ŀ諸���D��Paper_Content�R��
            //���o���էO������D(DataSet)
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //�ˬd���D�جO�_���Q�Ŀ�
                    bool bCheck = false;
                    try
                    {
                        bCheck = ((CheckBox)(this.FindControl("Form1").FindControl("ch-" + strQID))).Checked;
                    }
                    catch
                    {
                        Response.Write("<span style='DISPLAY: none'>Ū��" + strQID + "��CheckBox����</span>");
                    }

                    if (bCheck == true)
                    {
                        //�p�G���Q�Ŀ�A�h�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //���էO�S���D�ت�����
            }
            dsQuestionList.Dispose();

            //���o���էO���ݵ��D
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //�ˬd���D�جO�_���Q�Ŀ�
                    bool bCheck = false;
                    try
                    {
                        bCheck = ((CheckBox)(this.FindControl("Form1").FindControl("ch-" + strQID))).Checked;
                    }
                    catch
                    {
                        Response.Write("<span style='DISPLAY: none'>Ū��" + strQID + "��CheckBox����</span>");
                    }

                    if (bCheck == true)
                    {
                        //���Q�Ŀ�A�h�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //���էO�S���ݵ��D������
            }
            dsTextList.Dispose();

            tcQuestionTable.Controls.Clear();

            //��Q��������D�էO�qPaper_RandomQuestion�R��
            strSQL = mySQL.getPaper_RandomQuestionNum(strPaperID);
            DataSet dsQuestionNum = sqldb.getDataSet(strSQL);

            if (dsQuestionNum.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionNum.Tables[0].Rows.Count; i++)
                {
                    //get GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionNum.Tables[0].Rows[i]["cQuestionGroupID"].ToString();

                    }
                    catch
                    {
                    }

                    bool bCheck = false;
                    try
                    {
                        bCheck = ((CheckBox)(this.FindControl("Form1").FindControl("ch" + strGroupID))).Checked;
                    }
                    catch
                    {
                    }

                    //�ˬd���էO��CheckBox�O���O���Q���
                    if (bCheck == true)
                    {
                        //�R�����էO�����
                        mySQL.deletePaper_RandomQuestionNum(strPaperID, strGroupID);
                    }
                }
            }

            dsQuestionNum.Dispose();
            tcQuestionNumTable.Controls.Clear();

            if (strGenerationMethod == "Edit")
            {
                //�إ�Main table
                this.setupQuestionTable();

                this.setupQuestionNumTable();
            }
            else
            {
                //�إ�Question group number Table
                this.setupQuestionNumTable();
            }
        }


        private void btnModify_Click(object sender, EventArgs e)
        {
            //���XID
            string strQID = ((Button)(sender)).ID.Remove(0, 9);

            //���X�����D�bQuestionMode�����
            string strSQL = mySQL.getSingleQuestionInformation(strQID);
            DataSet dsQuestionInfo = sqldb.getDataSet(strSQL);

            string strQuestionType = "";

            if (dsQuestionInfo.Tables[0].Rows.Count > 0)
            {

                //���oQuestionType
                try
                {
                    strQuestionType = dsQuestionInfo.Tables[0].Rows[0]["cQuestionType"].ToString();
                }
                catch
                {
                }

                //���oQuestionMode
                string strQuestionMode = "";
                try
                {
                    strQuestionMode = dsQuestionInfo.Tables[0].Rows[0]["cQuestionMode"].ToString();
                }
                catch
                {
                }

                //�]�wQuestionMode
                if (Session["QuestionMode"] != null)
                {
                    Session["QuestionMode"] = strQuestionMode;
                }
                else
                {
                    Session.Add("QuestionMode", strQuestionMode);
                }

                if (strQuestionMode == "General")
                {
                    //��X�����D���ݪ�Question GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionInfo.Tables[0].Rows[0]["cQuestionGroupID"].ToString();
                    }
                    catch
                    {
                    }

                    //��QuestionGroupID�s�JSession
                    if (Session["GroupID"] != null)
                    {
                        Session["GroupID"] = strGroupID;
                    }
                    else
                    {
                        Session.Add("GroupID", strGroupID);
                    }

                    //��X��Group�ݩ�DivisionID
                    string strGroupDivisionID = myReceiver.getGroupDivisionID(strGroupID);
                    if (Session["GroupDivisionID"] != null)
                    {
                        Session["GroupDivisionID"] = strGroupDivisionID;
                    }
                    else
                    {
                        Session.Add("GroupDivisionID", strGroupDivisionID);
                    }
                }

                //�]�wQID��Session
                if (Session["QID"] != null)
                {
                    Session["QID"] = strQID;
                }
                else
                {
                    Session.Add("QID", strQID);
                }

                //�]�wbModify��Session
                if (Session["bModify"] != null)
                {
                    Session["bModify"] = true;
                }
                else
                {
                    Session.Add("bModify", true);
                }
            }
            else
            {
                //�S�������D�����
            }
            dsQuestionInfo.Dispose();

            if (strQuestionType == "6")
            {
                //�I�sCommon question editor
                Response.Redirect("./CommonQuestionEdit/Page/showquestionWithKeyWords.aspx?QID=" + strQID + "&Opener=Paper_MainPage");
            }
            else
            {
                Session["PreviousPageURL"]= HttpContext.Current.Request.Url.AbsoluteUri;
                //�I�sCommon question editor
                Response.Redirect("./CommonQuestionEdit/Page/ShowQuestion.aspx?QID=" + strQID + "&Opener=Paper_MainPage");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //���o�n�Q�R����QID
            string strQID = ((Button)(sender)).ID.Remove(0, 9);

            //�I�s�R���@�Ӱ��D��SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        private void btnDeleteNum_Click(object sender, EventArgs e)
        {
            //���o�n�Q�R����GroupID
            string strGroupID = ((Button)(sender)).ID.Remove(0, 13);

            //�I�s�R���@�Ӱ��D��SQL command
            mySQL.deletePaper_RandomQuestionNum(strPaperID, strGroupID);

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        //�ק�@�ӱ����D
        private void btnModifySituation_Click(object sender, EventArgs e)
        {
            //���oQID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //���X�����D�bQuestionMode�����
            string strSQL = mySQL.getSingleQuestionInformation(strQID);
            DataSet dsQuestionInfo = sqldb.getDataSet(strSQL);

            if (dsQuestionInfo.Tables[0].Rows.Count > 0)
            {
                //���oQuestionMode
                string strQuestionMode = "";
                try
                {
                    strQuestionMode = dsQuestionInfo.Tables[0].Rows[0]["cQuestionMode"].ToString();
                }
                catch
                {
                }

                //�]�wQuestionMode
                if (Session["QuestionMode"] != null)
                {
                    Session["QuestionMode"] = strQuestionMode;
                }
                else
                {
                    Session.Add("QuestionMode", strQuestionMode);
                }

                if (strQuestionMode == "General")
                {
                    //��X�����D���ݪ�Question GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionInfo.Tables[0].Rows[0]["cQuestionGroupID"].ToString();
                    }
                    catch
                    {
                    }

                    //��QuestionGroupID�s�JSession
                    if (Session["GroupID"] != null)
                    {
                        Session["GroupID"] = strGroupID;
                    }
                    else
                    {
                        Session.Add("GroupID", strGroupID);
                    }

                    //��X��Group�ݩ�DivisionID
                    string strGroupDivisionID = myReceiver.getGroupDivisionID(strGroupID);
                    if (Session["GroupDivisionID"] != null)
                    {
                        Session["GroupDivisionID"] = strGroupDivisionID;
                    }
                    else
                    {
                        Session.Add("GroupDivisionID", strGroupDivisionID);
                    }
                }

                //�]�wQID��Session
                if (Session["QID"] != null)
                {
                    Session["QID"] = strQID;
                }
                else
                {
                    Session.Add("QID", strQID);
                }

                //�]�wbModify��Session
                if (Session["bModify"] != null)
                {
                    Session["bModify"] = true;
                }
                else
                {
                    Session.Add("bModify", true);
                }
            }
            else
            {
                //�S�������D�����
            }
            dsQuestionInfo.Dispose();

            //�I�sCommon question editor
            Response.Redirect("Paper_EmulationQuestion.aspx?QID=" + strQID + "&bModify=" + true + "&Opener=Paper_MainPage");
        }
        //�R���@�ӱ����D
        private void btnDeleteSituation_Click(object sender, EventArgs e)
        {
            //���oQID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //�I�s�R���@�Ӱ��D��SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        private void btnModifyText_Click(object sender, EventArgs e)
        {
            //�ק�@�Ӱݵ��D

            //���oQID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //�]�wQID��Session
            if (Session["QID"] != null)
            {
                Session["QID"] = strQID;
            }
            else
            {
                Session.Add("QID", strQID);
            }
            //�t�s�s�DBen2017 11 3
            
            //�I�sPaper_TextQuestionEditor.aspx
            //Response.Redirect("Paper_TextQuestionEditorNew.aspx?QID=" + strQID + "&Opener=Paper_MainPage");
            
            //�I�sPaper_TextQuestionEditor.aspx
            Response.Redirect("Paper_TextQuestionEditorNew.aspx?QID=" + strQID + "&Opener=Paper_MainPage&bModify=True");
        }

        private void btnDeleteText_Click(object sender, EventArgs e)
        {
            //�R���@�Ӱݵ��D

            //���oQID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //�]�wQID��Session
            if (Session["QID"] != null)
            {
                Session["QID"] = strQID;
            }
            else
            {
                Session.Add("QID", strQID);
            }

            //�R���Ӱݵ��D
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }
        //�R���@�ӹϧ��D
        private void btnDeleteSimu_Click(object sender, EventArgs e)
        {
            //���oQID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //�I�s�R���@�Ӱ��D��SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        protected void txtScore_TextChange(object sender, EventArgs e)
        {
            string[] strIDArray = ((TextBox)(sender)).ID.Split('-');
            string strQID = strIDArray[1];
            try
            {
                int intScore = int.Parse(((TextBox)(sender)).Text);
                SQLString.UpdateQuestionScore(strQID, intScore, strPaperID);
            }
            catch (Exception x)
            {
                Response.Write("<script>window.alert('�п�J���')</script>");
                ((TextBox)(sender)).Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
            }

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }

        }

        //�t�Φ۰ʳ]�w�C�@�D�ؤ���
        protected void btnAutoSetScore_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsPaperQuestion = SQLString.GetPaperQuestion(strPaperID);
                //�Ҩ��D�ؼƥ�
                int intQuestionNum = dsPaperQuestion.Tables[0].Rows.Count;
                if (intQuestionNum > 0)
                {
                    //�Ҩ��`��
                    int intTotalScore = int.Parse(txtManualSetScore.Text);
                    int intAvgScore = intTotalScore / intQuestionNum;
                    int intRemainder = intTotalScore % intQuestionNum;

                    for (int i = 0; i < dsPaperQuestion.Tables[0].Rows.Count; i++)
                    {
                        //�̫�@�D�N�l�ƸɤW
                        if ((i + 1) == dsPaperQuestion.Tables[0].Rows.Count)
                        {
                            intAvgScore = intAvgScore + intRemainder;
                        }
                        SQLString.UpdateQuestionScore(dsPaperQuestion.Tables[0].Rows[i]["cQID"].ToString(), intAvgScore, strPaperID);
                    }
                }
            }
            catch (Exception x)
            {
                Response.Write("<script>window.alert('�`���п�J���')</script>");
            }

            //�������
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //�إ�Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //�إ�Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //�إ�Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }
        //�uAdd A New Question�v���s�ƥ�
        protected void btnAddNewQues_onserverclick(object sender, EventArgs e)
        {
            //�i�J�D�ث��A��ܭ���(GroupID�T�w���p����Ͼ�:Group_201211141356547407559)
            //Response.Redirect("Paper_QuestionTypeNew.aspx?Opener=Paper_MainPage&GroupID=Group_201211141356547407559&bModify=False");
            string strURL = "";
            if (Request.QueryString["CaseID"] != null)
                strURL = "./QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question&CaseID=" + Request.QueryString["CaseID"].ToString();
            else
                strURL = "./QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question";


            //Session["PreviousPageURL"] = HttpContext.Current.Request.Url.AbsoluteUri;
            //Response.Redirect(strURL);
            string strJSCode = "window.open('" + strURL + "','�}���D�w�t�ηs�W�D��','scrollbars=yes,resizable=yes,directories=0,location=1,menubar=0,status=0,titlebar=1,toolbar=0,fullscreen=yes')";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>" + strJSCode + "</script>");
        }
        //�uFinish�v���s�ƥ�
        protected void btnFinish2_onserverclick(object sender, EventArgs e)
        {
            if (Session["IsFromClassExercise"] != null)
                Session.Remove("IsFromClassExercise");
            if (Session["IsFromSelectExistPaper"] != null)
                Session.Remove("IsFromSelectExistPaper");
            if (Session["PreOpener"] != null)
                Session.Remove("PreOpener");

            //�ˬd�O�_�N�Ѿl�����t���ƥ������t��0���D��
            if (hiddenIsAvgScoreToZeroScore.Value.Equals("true"))
            {
                //���X�Ҩ��Ҧ��D�ظ�T
                DataTable dtPaperQuestion = SQLString.GetPaperQuestion(strPaperID).Tables[0];
                //�Ҩ��D�ؼƥ�
                int intQuestionNum = dtPaperQuestion.Rows.Count;
                //�Ҩ��`��
                int intTotalScore = int.Parse(txtManualSetScore.Text);
                //�Ҩ��D���`��
                int intQuestionTotalScore = 0;
                //�����s���D��
                List<string> ZeroPointQuestions = new List<string>();
                foreach (DataRow drPaperQuestion in dtPaperQuestion.Rows)
                {
                    int QuestionPoint = int.Parse(drPaperQuestion["cQuestionScore"].ToString());
                    if (QuestionPoint > 0)
                    {
                        intQuestionTotalScore += QuestionPoint;
                    }
                    else
                    {
                        //�����Ҩ����s���D��
                        ZeroPointQuestions.Add(drPaperQuestion["cQID"].ToString());
                    }
                }
                int intAvgRemaining = (intTotalScore - intQuestionTotalScore) / ZeroPointQuestions.Count;
                int intRemainder = (intTotalScore - intQuestionTotalScore) % ZeroPointQuestions.Count;
                for (int i = 0; i < ZeroPointQuestions.Count; i++)
                {
                    if (ZeroPointQuestions.Count == (i + 1))
                    {
                        intAvgRemaining = intAvgRemaining + intRemainder;
                    }
                    SQLString.UpdateQuestionScore(ZeroPointQuestions[i], intAvgRemaining, strPaperID);
                }
            }

            if (txtPaperTitle.InnerText == "")
            {
                Page.RegisterClientScriptBlock("alert", "<script>alert('�п�J�Ҩ����D')</script>");
            }
            else //���JPaper_Classify��ƪ�
            {

                string strComeFromActivityName = "";
                if (Session["ComeFromActivityName"] != null)
                {
                    strComeFromActivityName = Session["ComeFromActivityName"].ToString();
                    Session.Remove("ComeFromActivityName");
                }

                string strSQL = " INSERT INTO Paper_Classify (cCaseID, cSectionName, cPaperID, iClassID) " +
                   " VALUES ('" + strCaseID + "' , '" + txtPaperTitle.InnerText + "'  ,'" + strPaperID + "' , '" + CheckClassState() + "') ";
                try
                {
                    sqldb.ExecuteNonQuery(strSQL);
                }
                catch
                { }
                string script = "";
                switch (strComeFromActivityName)
                {
                    //�Ҩ��оǬ���
                    case "ExaminationActivity":
                        clsMLASDB MLASDB = new clsMLASDB();
                        string strActivityID = Session["ActivityID"].ToString();
                        //��s�H�����оǬ��ʦҨ��T��
                        string strSQLSave =
                            "UPDATE MLAS_ActivityExaminationSetting SET " +
                            "cPaperID =@cPaperID, " +
                            "cPaperName =@cPaperName WHERE cActivityID LIKE @cActivityID";
                        object[] pList2 = { strPaperID, txtPaperTitle.InnerText, strActivityID };
                        try
                        {
                            MLASDB.ExecuteNonQuery(strSQLSave, pList2);
                        }
                        catch { }
                        Session.Remove("ComeFromActivityName");
                        Session.Remove("ActivityID");

                        script = "<script> try{window.opener.document.getElementById('ctl00_cphContentArea_btReload').click();}catch(e){};window.close(); </script>";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "close_window", script);
                        break;
                    case "EditPaper"://Hints���s��Ҩ�
                        script = "<script> alert('�Ҩ��s�觹��');window.close(); </script>";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "close_window", script);
                        Session.Remove("ComeFromActivityName");
                        break;
                    default:
                        Response.Redirect("../../../PushMessage/MessaeMember.aspx?Opener=Paper_MainPage" + "&cExerciseIDcGroupID=" + Session["ExerciseIDGroupID"].ToString() + "&cCaseID=" + strCaseID + "&cPaperID=" + strPaperID);
                        break;
                }
            }

        }
        //�ˬd�Ҩ����L�s��
        protected void btnFinish3_onserverclick(object sender, EventArgs e)
        {
            //�x�s���ݨ���Title
            string strPaperTitle = txtPaperTitle.InnerText;
            mySQL.UpdatePaperTitleOfPaper_Header(strPaperID, strPaperTitle);

            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //����D�t����r��g�z�ѳ̤j�r��
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);

            string script = "";
            DataTable dtPaperQuestion = SQLString.GetPaperQuestion(strPaperID).Tables[0];
            //�Ҩ��D�ؼƥ�
            int intQuestionNum = dtPaperQuestion.Rows.Count;
            if (intQuestionNum > 0)
            {
                //�Ҩ��`��
                int intTotalScore = int.Parse(txtManualSetScore.Text);
                //�Ҩ��D���`��
                int intQuestionTotalScore = 0;
                //�����s���D��
                List<string> ZeroPointQuestions = new List<string>();
                foreach (DataRow drPaperQuestion in dtPaperQuestion.Rows)
                {
                    int QuestionPoint = int.Parse(drPaperQuestion["cQuestionScore"].ToString());
                    if (QuestionPoint > 0)
                    {
                        intQuestionTotalScore += QuestionPoint;
                    }
                    else
                    {
                        //�����Ҩ����s���D��
                        ZeroPointQuestions.Add(drPaperQuestion["cQID"].ToString());
                    }
                }
                //�p�G�D���`���j��Ҩ��`��
                if (intQuestionTotalScore > intTotalScore)
                {
                    script = "alert('�ثe�Ҩ��D�ؤ����`�M�j��Ҩ��`���A�Эק�Ҩ��D�ؤ���!');";
                }
                else//�p�󵥩�
                {
                    //�p�G�D�ؤ��Ƶ���Ҩ��`��
                    if (intQuestionTotalScore == intTotalScore)
                    {
                        //�s�b���L�s���D��
                        if (ZeroPointQuestions.Count > 0)
                            script = "alert('�ثe�Ҩ����]�w���Ƭ�0���D�ءA�Эק�Ҩ��D�ؤ���!');";
                        else
                            script = "document.getElementById('btnFinish2').click();";
                    }
                    else//�p��
                    {
                        if (ZeroPointQuestions.Count > 0)
                            script = "if(confirm('�O�_�������t�Ҩ��Ѿl���Ʃ�0���D�بç����s��Ҩ�?')){document.getElementById('hiddenIsAvgScoreToZeroScore').value='true';document.getElementById('btnFinish2').click();}";
                        else
                            script = "alert('�ثe�Ҩ��D�ؤ����`�M�p��Ҩ��`���A�Эק�Ҩ��D�ؤ���');";
                    }
                }
            }
            else
            {
                script = "alert('�ثe�Ҩ��L�D�ءA�Э��s�s��Ҩ�!');";
            }
            ClientScript.RegisterStartupScript(this.GetType(), "alert_window", "<script>" + script + "</script>", false);
        }

        protected void tbMaximumNumberOfWordsReasons_TextChanged(object sender, EventArgs e)
        {
            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //����D�t����r��g�z�ѳ̤j�r��
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);
        }

        //���s��z����
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            tbMaximumNumberOfWordsReasons.Text = strMaximumNumberOfWordsReasons;

            //�]�wtxtTitle
            this.setupPaperTitle();

            //�ˬd���L�ݭn�۰ʥ����D�ؤ���
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAutoScorll", "document.getElementById('btnAutoSetScore').click();", true);
        }
    }
}