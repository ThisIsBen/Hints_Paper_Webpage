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
    /// Paper_MainPage 的摘要描述。
    /// 問卷系統自行編輯問卷功能的起始頁
    /// </summary>
    public partial class Paper_MainPage : AuthoringTool_BasicForm_BasicForm
    {
        //建立SqlDB物件
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
        /// 回傳PEQuestion的PaperID
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
        //載入頁面
        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Session["QuestionBankID"] != null)
            {
                Session.Remove("QuestionBankID");
            }


            this.Initiate();

            //如果是MLAS導入，先設定Sesseion值
            this.InitiateParameterFromMLAS();


            //判斷由哪個頁面轉過來去取得不同的參數
            if (Request.QueryString["Opener"] != null)
            {
                hiddenOpener.Value = Request.QueryString["Opener"].ToString();
            }
            else if (Session["PreOpener"] != null)
            {
                hiddenOpener.Value = Session["PreOpener"].ToString();
            }

            //從ORCS的課堂練習頁面接收參數
            if (hiddenOpener.Value == "SelectPaperMode")
            {
                this.getParameterFromORCSExercise();
                //設定從課堂練習頁面進來時，此頁面的控制項
                SetPageStyle();
            }
            else //從原本HINTS的Case頁面接收參數
                this.getParameter();

            //找出此Case有哪一個問卷，取出其PaperID。
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

            //MLAS考卷教學活動
            if (Request.QueryString["cComeFromActivityName"] != null)
            {
                Session["ComeFromActivityName"] = Request.QueryString["cComeFromActivityName"].ToString();
            }

            if (Request.QueryString["cActivityID"] != null)
            {
                Session["ActivityID"] = Request.QueryString["cActivityID"].ToString();
            }

            //把PaperID存入Session
            if (Session["PaperID"] != null)
            {
                Session["PaperID"] = strPaperID;
            }
            else
            {
                Session.Add("PaperID", strPaperID);
            }

            
            //取得此問卷是使用者自行編輯或是系統在呈現題目時才亂數選題
            string strSQL = mySQL.getPaperHeader(strPaperID);
            DataSet dsHeader = sqldb.getDataSet(strSQL);
            try
            {
                strGenerationMethod = dsHeader.Tables[0].Rows[0]["cGenerationMethod"].ToString();
                //選擇題含關鍵字填寫理由最大字數
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

            //把PresentType存入Session
            if (Session["PresentType"] != null)
            {
                Session["PresentType"] = strGenerationMethod;
            }
            else
            {
                Session.Add("PresentType", strGenerationMethod);
            }

            //把PresentType存入Hidden
            hiddenPresentType.Value = strGenerationMethod;

            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                this.setupQuestionTable();


                //建立Question num table
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");
                //若不是編輯模式，則隱藏此按鈕工具 朱君2012/12/25
                trSetScoreControl.Style.Add("DISPLAY", "none");
                //建立Question group number Table
                this.setupQuestionNumTable();
            }

            //第一次載入頁面事件
            if (this.IsPostBack == false)
            {
                //設定txtTitle
                this.setupPaperTitle();

                //檢查有無需要自動平均題目分數
                if (Request.QueryString["bIsAutoSetScore"] != null && Request.QueryString["bIsAutoSetScore"].Equals("true"))
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAutoScorll", "document.getElementById('btnAutoSetScore').click();", true);
                }
            }

            //加入刪除按鈕的事件
            btnDeleteQuestion.ServerClick += new EventHandler(btnDeleteQuestion_ServerClick);

            //加入修改按鈕的事件
            btnModifyQuestion.ServerClick += new EventHandler(btnModifyQuestion_ServerClick);

            #region WebPage物件管理

            //從資料表取得物件的管理型態
            DataTable dtWebPageObjectManage = clsWebPageObjectManage.WebPageObjectManage("Paper_MainPage.aspx", usi.UsingSystem);
            if (dtWebPageObjectManage != null)
            {
                //根據資料表的值做一一對應的型態管理
                btnPre.Visible = Hints.TableStyle.CheckWebPageObjectManage(dtWebPageObjectManage.Rows[0]["cObjectType"].ToString(), 0);

                //根據資料表的值做對應的物件給值
                hfPageUrl.Value = dtWebPageObjectManage.Rows[0]["cPageUrl"].ToString();
            }
            else////default hints
            {
                hfPageUrl.Value = "/Hints/Flow control/terminator.aspx";
            }

            #endregion

        }

        /// <summary>
        /// 設定問卷的Title
        /// </summary>
        private void setupPaperTitle()
        {
            //Get the title of this paper
            string strPaperTitle = myReceiver.getPaperTitle(strPaperID);

            txtPaperTitle.InnerText = strPaperTitle;
        }

        /// <summary>
        /// 建立顯示每個問題群組需要被選取的題目個數
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

            //從Paper_RandomQuestionNum取出此問卷的資料

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
                        //設定Table的Style
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

                        //get 此組別全部的問題數目
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
                        //加入CheckBox
                        TableCell tcBox = new TableCell();
                        tr.Cells.Add(tcBox);
                        tcBox.Width = Unit.Pixel(25);

                        CheckBox chBox = new CheckBox();
                        tcBox.Controls.Add(chBox);
                        chBox.ID = "ch" + strGroupID;
                        chBox.Attributes.Add("onclick","ShowbtnDelete();");
                        */

                        //組別名稱
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

                        //問題個數
                        TableCell tcQuestionNum = new TableCell();
                        tr.Cells.Add(tcQuestionNum);

                        //    TextBox txtQuestionNum = new TextBox();
                        //    tcQuestionNum.Controls.Add(txtQuestionNum);
                        //    txtQuestionNum.ID = "txt" + strGroupID;
                        //    txtQuestionNum.Text = intQuestionNum.ToString();
                        ////	txtQuestionNum.Attributes.Add("onchange","ShowbtnModify();");

                        //設定問題難易度的顯示數量
                        DataTable dtQuestionLevel = new DataTable();
                        dtQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName();
                        int iQuestionLevelCount = 0;//記錄目前為第幾個難易度
                        foreach (DataRow drQuestionLevel in dtQuestionLevel.Rows)
                        {
                            //取得設定每個難易度的題目數量
                            DataTable dtQuestionLevelNum = new DataTable();
                            dtQuestionLevelNum = PaperSystem.DataReceiver.GetQuestionLevelNum(strGroupID);
                            string strQuestionLevelName = drQuestionLevel["cLevelName"].ToString();
                            int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(strQuestionLevelName);
                            Label lbQuestionLevelName = new Label();
                            lbQuestionLevelName.Text = strQuestionLevelName;
                            tcQuestionNum.Controls.Add(lbQuestionLevelName);
                            int iQuestionLevelNum = 0;
                            //將每個難易度的數量填入ddl中
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

                        //此組別的問題總數
                        TableCell tcQuestionCount = new TableCell();
                        tr.Cells.Add(tcQuestionCount);

                        tcQuestionCount.Text = intQuestionCount.ToString();
                        tcQuestionCount.Attributes.Add("Align", "Center");

                        //建立Title的TextArea
                        TableRow trQuestionTitle = new TableRow();
                        table.Rows.Add(trQuestionTitle);

                        TableCell tcTitle = new TableCell();
                        trQuestionTitle.Cells.Add(tcTitle);
                        tcTitle.Attributes.Add("align", "right");
                        tcTitle.ColumnSpan = 3;

                        //建立Question title的TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcTitle.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strGroupID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //取出此QuestionTitle的內容
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strGroupID);

                        //建立Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //從ORCS的課堂練習頁面接收參數
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcTitle.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strGroupID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strGroupID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //建立間隔
                        Label lblCell0 = new Label();
                        lblCell0.Style.Add("WIDTH", "20px");
                        tcTitle.Controls.Add(lblCell0);

                        //建立Delete button
                        Button btnDeleteNum = new Button();
                        tcTitle.Controls.Add(btnDeleteNum);
                        btnDeleteNum.ID = "btnDeleteNum-" + strGroupID;
                        btnDeleteNum.Text = "Delete this topic";
                        btnDeleteNum.Click += new EventHandler(btnDeleteNum_Click);
                        btnDeleteNum.Style["width"] = "220px";
                        btnDeleteNum.Attributes["class"] = "button_blue";

                        //建立Empty row
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

                        //加入Table的Title
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
                //沒有資料
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
            //初始化考卷總分
            intSumScore = 0;

            #region 列出選擇題
            //取出此問卷的選擇題內容
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //取得QuestionType
                    string strQuestionType = "1";
                    try
                    {
                        strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();
                    }
                    catch
                    {
                    }

                    //取得QID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //取得問題的SQL
                    strSQL = mySQL.getQuestion(strQID);
                    DataSet dsQuestion = sqldb.getDataSet(strSQL);

                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //建立問題的內容
                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        intQuestionIndex += 1;
                        /*
                        //問題的CheckBox
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
                        //問題的題號
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(25);
                        tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ":<br/>    "+"選擇題: " ;
                        tcQuestionNum.Attributes["style"] = "color: Black";

                        //問題的內容
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
                        tcQuestion.Text =  strQuestion;
                        tcQuestion.Width = Unit.Percentage(100);

                        /*
                        //建立修改問題的Button
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

                        //建立問題的CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //建立選項
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

                                //選項編號與是否為建議選項
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
                                //選項編號
                                TableCell tcSelectionNum = new TableCell();
                                trSelection.Cells.Add(tcSelectionNum);
                                tcSelectionNum.Text = Convert.ToString((j+1)) + ".";
                                */

                                //選項內容
                                TableCell tcSelection = new TableCell();
                                trSelection.Cells.Add(tcSelection);
                                tcSelection.Text = strSelection;

                                /*
                                //Empty TableCell
                                TableCell tcEmpty = new TableCell();
                                trSelection.Cells.Add(tcEmpty);
                                */

                                //建立選項的CSS
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
                            //此問題沒有選項
                        }
                        dsSelection.Dispose();

                        //建立Modify and Delete button
                        TableRow trButton = new TableRow();
                        table.Rows.Add(trButton);

                        //新增TableCell 用來增加設定分數的TextBox
                        TableCell tcTextArea = new TableCell();
                        trButton.Cells.Add(tcTextArea);
                        tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                        tcTextArea.VerticalAlign = VerticalAlign.Top;

                        TableCell tcButton = new TableCell();
                        trButton.Cells.Add(tcButton);
                        tcButton.HorizontalAlign = HorizontalAlign.Right;
                        tcButton.VerticalAlign = VerticalAlign.Top;

                        //建立Question title的TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcButton.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strQID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //取出此QuestionTitle的內容
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                        //建立間隔
                        Label lblCell0 = new Label();
                        lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell0);

                        //建立Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //從ORCS的課堂練習頁面接收參數
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcButton.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strQID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //建立間隔
                        Label lblCell1 = new Label();
                        lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell1);

                        //建立ScoreTextBox 朱君2012/12/25
                        Label lblScore = new Label();
                        tcTextArea.Controls.Add(lblScore);
                        lblScore.Text = "分數：";
                        lblScore.Style["width"] = "50px";

                        //建立ScoreTextBox 朱君2012/12/25
                        TextBox txtScore = new TextBox();
                        txtScore.AutoPostBack = true;
                        tcTextArea.Controls.Add(txtScore);
                        txtScore.ID = "txtScore-" + strQID;
                        txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                        txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                        txtScore.Style["width"] = "80px";
                        //累加每一題目的分數
                        intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                        //建立Modify button
                        Button btnModify = new Button();
                        tcButton.Controls.Add(btnModify);
                        btnModify.ID = "btnModify" + strQID;
                        btnModify.Text = "Modify this question";
                        btnModify.Click += new EventHandler(btnModify_Click);
                        btnModify.Style["width"] = "220px";
                        btnModify.Attributes["class"] = "button_blue";

                        //建立間隔
                        Label lblCel2 = new Label();
                        lblCel2.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCel2);

                        //建立Delete button
                        Button btnDelete = new Button();
                        tcButton.Controls.Add(btnDelete);
                        btnDelete.ID = "btnDelete" + strQID;
                        btnDelete.Text = "Delete this question";
                        btnDelete.Click += new EventHandler(btnDelete_Click);
                        btnDelete.Style["width"] = "220px";
                        btnDelete.Attributes["class"] = "button_blue";

                        if (i < dsQuestionList.Tables[0].Rows.Count - 1)
                        {
                            //建立Empty row
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
                        //此問題沒有選項
                    }
                    dsQuestion.Dispose();


                }

            }
            else
            {
                //此問卷沒有選擇題的情況
            }
            dsQuestionList.Dispose();
            #endregion

            #region 列出選擇題包含關鍵字
            //取出此問卷的選擇題內容
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
                    //取得QuestionType
                    string strQuestionType = "6";
                    try
                    {
                        strQuestionType = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQuestionType"].ToString();
                    }
                    catch
                    {
                    }

                    //取得QID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //取得問題的SQL
                    strSQL = mySQL.getQuestion(strQID);
                    DataSet dsQuestion = sqldb.getDataSet(strSQL);

                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //建立問題的內容
                        TableRow trQuestion = new TableRow();
                        selectQuestionWithKeyWordstable.Rows.Add(trQuestion);

                        intQuestionIndex += 1;

                        //問題的題號
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(25);
                        tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() +":<br/>    "+"選擇題包含關鍵字: " ;
                        tcQuestionNum.Attributes["style"] = "color: Black";


                        //問題的內容
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

                        //關鍵字標題
                        TableCell tcKeyWordsTitle = new TableCell();
                        trQuestion.Cells.Add(tcKeyWordsTitle);
                        tcKeyWordsTitle.Text = "KeyWords";
                        tcKeyWordsTitle.Attributes["align"] = "center";
                        tcKeyWordsTitle.Width = Unit.Pixel(300);
                        tcKeyWordsTitle.BorderWidth = Unit.Pixel(1);
                        tcKeyWordsTitle.BorderColor = Color.Black;

                        //建立問題的CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //建立選項
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

                                //選項編號與是否為建議選項
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

                                //選項內容
                                TableCell tcSelection = new TableCell();
                                trSelection.Cells.Add(tcSelection);
                                tcSelection.Text = strSelection;


                                //在選項第一列加入關鍵字欄位
                                if (j == 0)
                                {
                                    //取得題目關鍵字
                                    string strKeyWords = "";
                                    strKeyWords = dsQuestion.Tables[0].Rows[0]["cKeyWords"].ToString();
                                    TableCell tcKeyWords = new TableCell();
                                    trSelection.Cells.Add(tcKeyWords);
                                    tcKeyWords.RowSpan = dsSelection.Tables[0].Rows.Count;
                                    tcKeyWords.Text = strKeyWords;
                                    tcKeyWords.BorderWidth = Unit.Pixel(1);
                                    tcKeyWords.BorderColor = Color.Black;
                                }

                                //建立選項的CSS
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
                            //此問題沒有選項
                        }
                        dsSelection.Dispose();

                        //建立Modify and Delete button
                        TableRow trButton = new TableRow();
                        selectQuestionWithKeyWordstable.Rows.Add(trButton);

                        //新增TableCell 用來增加設定分數的TextBox
                        TableCell tcTextArea = new TableCell();
                        trButton.Cells.Add(tcTextArea);
                        tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                        tcTextArea.VerticalAlign = VerticalAlign.Top;

                        TableCell tcButton = new TableCell();
                        trButton.Cells.Add(tcButton);
                        tcButton.HorizontalAlign = HorizontalAlign.Right;
                        tcButton.VerticalAlign = VerticalAlign.Top;
                        tcButton.ColumnSpan = 2;

                        //建立Question title的TextArea
                        HtmlTextArea txtTitle = new HtmlTextArea();
                        tcButton.Controls.Add(txtTitle);
                        txtTitle.ID = "txtTitle" + strQID;
                        txtTitle.Style.Add("WIDTH", "100%");
                        txtTitle.Rows = 5;
                        txtTitle.Style.Add("DISPLAY", "none");

                        //取出此QuestionTitle的內容
                        txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                        //建立間隔
                        Label lblCell0 = new Label();
                        lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell0);

                        //建立Question title button
                        HtmlInputButton btnTitle = new HtmlInputButton("button");
                        //從ORCS的課堂練習頁面接收參數
                        if (hiddenOpener.Value != "SelectPaperMode")
                        {
                            tcButton.Controls.Add(btnTitle);
                        }
                        btnTitle.ID = "btnTitle" + strQID;
                        btnTitle.Value = "Add a question title";
                        btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                        btnTitle.Style["width"] = "220px";
                        btnTitle.Attributes["class"] = "button_blue";

                        //建立間隔
                        Label lblCell1 = new Label();
                        lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCell1);

                        //建立ScoreTextBox 朱君2012/12/25
                        Label lblScore = new Label();
                        tcTextArea.Controls.Add(lblScore);
                        lblScore.Text = "分數：";
                        lblScore.Style["width"] = "50px";

                        //建立ScoreTextBox 朱君2012/12/25
                        TextBox txtScore = new TextBox();
                        txtScore.AutoPostBack = true;
                        tcTextArea.Controls.Add(txtScore);
                        txtScore.ID = "txtScore-" + strQID;
                        txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                        txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                        txtScore.Style["width"] = "80px";
                        //累加每一題目的分數
                        intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                        //建立Modify button
                        Button btnModify = new Button();
                        tcButton.Controls.Add(btnModify);
                        btnModify.ID = "btnModify" + strQID;
                        btnModify.Text = "Modify this question";
                        btnModify.Click += new EventHandler(btnModify_Click);
                        btnModify.Style["width"] = "220px";
                        btnModify.Attributes["class"] = "button_blue";

                        //建立間隔
                        Label lblCel2 = new Label();
                        lblCel2.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                        tcButton.Controls.Add(lblCel2);

                        //建立Delete button
                        Button btnDelete = new Button();
                        tcButton.Controls.Add(btnDelete);
                        btnDelete.ID = "btnDelete" + strQID;
                        btnDelete.Text = "Delete this question";
                        btnDelete.Click += new EventHandler(btnDelete_Click);
                        btnDelete.Style["width"] = "220px";
                        btnDelete.Attributes["class"] = "button_blue";

                        if (i < dsQuestionWithKeyWordsList.Tables[0].Rows.Count - 1)
                        {
                            //建立Empty row
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
                        //此問題沒有選項
                    }
                    dsQuestion.Dispose();
                }
            }
            else
            {
                //此問卷沒有選擇題包含關鍵字的情況
            }
            dsQuestionList.Dispose();
            #endregion

            #region 列出問答題
            //建立屬於此問卷的問答題
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            int intTextCount = 0;
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //取得此問題的QID
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
                    tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ":<br/>    " + "問答題: ";
                    tcTextNum.Attributes["style"] = "color: Black";

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

                    //建立修改按鈕的TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //新增TableCell 用來增加設定分數的TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //建立Question title的TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    //取出此QuestionTitle的內容
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                    //建立間隔
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //建立Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //從ORCS的課堂練習頁面接收參數
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //建立ScoreTextBox 朱君2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "分數：";
                    lblScore.Style["width"] = "50px";

                    //建立ScoreTextBox 朱君2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //累加每一題目的分數
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //建立間隔
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //建立修改問題的Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "220px";
                    btnModifyText.ID = "btnModifyText-" + strQID;
                    btnModifyText.Text = "Modify this question";
                    btnModifyText.Click += new EventHandler(btnModifyText_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //建立間隔
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //建立刪除問答題的Button
                    Button btnDeleteText = new Button();
                    tcModify.Controls.Add(btnDeleteText);
                    btnDeleteText.Style["width"] = "220px";
                    btnDeleteText.ID = "btnDeleteText-" + strQID;
                    btnDeleteText.Text = "Delete this question";
                    btnDeleteText.Click += new EventHandler(btnDeleteText_Click);
                    btnDeleteText.Attributes["class"] = "button_blue";

                    //加入CSS
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
                //此問卷沒有問答題的情形
            }
            dsTextList.Dispose();
            #endregion

            #region 列出情境題
            //建立屬於此問卷的問答題
            strSQL = mySQL.getPaperSituationContent(strPaperID);
            DataSet dsSituationList = sqldb.getDataSet(strSQL);
            int intSituationCount = 0;

            if (dsSituationList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsSituationList.Tables[0].Rows.Count; i++)
                {
                    //取得此問題的QID
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
                    tcSituationNum.Text = "Q" + intQuestionIndex.ToString() + ":<br/>    " + "情境題: ";
                    tcSituationNum.Attributes["style"] = "color: Black";

                    TableCell tcSituationNumInformation = new TableCell();
                    trSituationInformation.Cells.Add(tcSituationNumInformation);
                    tcSituationNumInformation.Width = Unit.Pixel(200);
                    tcSituationNumInformation.Text = "Information" + intQuestionIndex.ToString() + ": ";

                    //Question
                    TableCell tcQuestion = new TableCell();
                    trSituationQuestion.Cells.Add(tcQuestion);
                    tcQuestion.Text =strQuestion;

                    //Answer
                    TableCell tcAnswer = new TableCell();
                    trSituationInformation.Cells.Add(tcAnswer);
                    tcAnswer.Text = strInformation;

                    //建立修改按鈕的TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //新增TableCell 用來增加設定分數的TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //建立Question title的TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    //取出此QuestionTitle的內容
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);

                    //建立間隔
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //建立Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //從ORCS的課堂練習頁面接收參數
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //建立ScoreTextBox 朱君2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "分數：";
                    lblScore.Style["width"] = "50px";

                    //建立ScoreTextBox 朱君2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //累加每一題目的分數
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //建立間隔
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //建立修改問題的Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "220px";
                    btnModifyText.ID = "btnModifySituation-" + strQID;
                    btnModifyText.Text = "Modify this question";
                    btnModifyText.Click += new EventHandler(btnModifySituation_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //建立間隔
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //建立刪除問答題的Button
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
                //沒有情境題 
            }

            #endregion

            #region 列出圖形題
            //依照QuestionMode決定取出此組別的圖形題
            strSQL = mySQL.getPaperSimulationContent(strPaperID);

            DataSet dsQuestionList_simulation = sqldb.getDataSet(strSQL);
            if (dsQuestionList_simulation.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList_simulation.Tables[0].Rows.Count; i++)
                {
                    //取得QuestionType
                    string strQuestionType = "3";
                    strQuestionType = dsQuestionList_simulation.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //取得QID
                    string strQID = "";
                    strQID = dsQuestionList_simulation.Tables[0].Rows[i]["cQID"].ToString();

                    //取得問題的SQL
                    DataSet dsQuestion = null;
                    //if (hfSymptoms.Value == "All")
                    strSQL = mySQL.getQuestion(strQID);
                    //NO Symptoms
                    //else
                    //    strSQL = mySQL.getQuestionBySymptoms(strQID, hfSymptoms.Value);

                    dsQuestion = sqldb.getDataSet(strSQL);
                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //建立問題的內容
                        TableRow trQuestion = new TableRow();
                        trQuestion.Style.Add("CURSOR", "hand");
                        table.Rows.Add(trQuestion);

                        intQuestionIndex += 1;

                        //問題的題號
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(150);
                        //tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";
                        tcQuestionNum.Text = "<IMG id='img_" + strQID + "' src='../../../BasicForm/Image/minus.gif'>&nbsp;Q" + intQuestionIndex.ToString() + ":<br/>    " + "圖形題: ";
                        tcQuestionNum.Attributes["style"] = "color: Black";
                        //問題的內容
                        string strQuestion = "";
                        strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();

                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        tcQuestion.Text =strQuestion;

                        //建立問題的CSS
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");

                        //取得simulator的場景及正解還有順序
                        strSQL = mySQL.getQuestion_sim(strQID);
                        DataSet dsQuestion_sim = sqldb.getDataSet(strSQL);
                        //建立圖形題的場景

                        TableRow trScene = new TableRow();
                        trScene.Height = 290;
                        table.Rows.Add(trScene);

                        TableCell tcSceneTitle = new TableCell();
                        trScene.Cells.Add(tcSceneTitle);
                        tcSceneTitle.Text = "<font style='color:Black; font-weight:bold'>Scene :&nbsp;<font/>";
                        tcSceneTitle.Style.Add("text-align", "right");
                        tcSceneTitle.Width = Unit.Pixel(230);
                        //取得場景ID
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
                        //縮放
                        //trQuestion.Attributes.Add("onclick", "ShowDetail('" + strQID + "','img_" + strQID + "','" + hfAnswerCount.Value + "')");
                        //建立圖形題的正確解答以及順序
                        if (dsQuestion_sim.Tables[0].Rows.Count > 0)
                        {
                            int s_no = 0;
                            for (int s = 0; s < dsQuestion_sim.Tables[0].Rows.Count; s++)
                            {
                                s_no = s + 1;
                                //答案行
                                TableRow trAnswer = new TableRow();
                                table.Rows.Add(trAnswer);

                                TableCell tcAnswerTitle = new TableCell();
                                trAnswer.Cells.Add(tcAnswerTitle);
                                tcAnswerTitle.Text = "<font style='color:Black; font-weight:bold'>Answer " + s_no.ToString() + " :&nbsp;<font/>";
                                tcAnswerTitle.Style.Add("text-align", "right");
                                tcAnswerTitle.Width = Unit.Pixel(230);
                                //置入答案
                                TableCell tcAnswerValue = new TableCell();
                                trAnswer.Cells.Add(tcAnswerValue);
                                tcAnswerValue.Attributes.Add("Class", "header1_tr_even_row");
                                tcAnswerValue.Width = Unit.Percentage(81);
                                string temp_ans = dsQuestion_sim.Tables[0].Rows[s]["cAnswer"].ToString().Replace('|', ',');
                                tcAnswerValue.Text = temp_ans.Substring(0, temp_ans.Length - 1);
                                //答案順序行
                                TableRow trAnswerOrder = new TableRow();
                                table.Rows.Add(trAnswerOrder);

                                TableCell tcAns_order_Title = new TableCell();
                                trAnswerOrder.Cells.Add(tcAns_order_Title);
                                tcAns_order_Title.Text = "<font style='color:Black; font-weight:bold'>Answer " + s_no.ToString() + " order:&nbsp;<font/>";
                                tcAns_order_Title.Style.Add("text-align", "right");
                                tcAns_order_Title.Width = Unit.Pixel(230);
                                //置入順序
                                TableCell tcAnswer_Order = new TableCell();
                                trAnswerOrder.Cells.Add(tcAnswer_Order);
                                tcAnswer_Order.Attributes.Add("Class", "header1_tr_even_row");
                                tcAnswer_Order.Width = Unit.Percentage(81);
                                string temp_order = dsQuestion_sim.Tables[0].Rows[s]["cOrder"].ToString().Replace('|', ',');
                                tcAnswer_Order.Text = temp_order.Substring(0, temp_order.Length - 1);

                            }
                        }
                        //Modify and Delete 按鈕的TableRow
                        TableRow trModify = new TableRow();
                        table.Rows.Add(trModify);
                        trModify.ID = "trModify_" + strQID;

                        TableCell tcNone = new TableCell();
                        trModify.Cells.Add(tcNone);

                        //建立修改問題的Button
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

                        //建立間隔
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


                        //建立Space
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


            #region 列出程式題
            
            strSQL = mySQL.getPaperProgramQuestionQuestionContent(strPaperID);
            DataSet dsProgramList = sqldb.getDataSet(strSQL);
            int intProgramCount = 0;
            if (dsProgramList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsProgramList.Tables[0].Rows.Count; i++)
                {
                    //取得此問題的QID
                    string strQID = "";
                    strQID = dsProgramList.Tables[0].Rows[i]["cQID"].ToString();

                    //Question
                    string strQuestion = "";
                    strQuestion = dsProgramList.Tables[0].Rows[i]["cQuestion"].ToString();

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    //Answer
                    string strAnswer = "";
                    strAnswer = dsProgramList.Tables[0].Rows[i]["cAnswer"].ToString();
                    */

                    TableRow trQuestion = new TableRow();
                    table.Rows.Add(trQuestion);
                    trQuestion.Attributes.Add("Class", "header1_table_first_row");

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    TableRow trAnswer = new TableRow();
                    trAnswer.Attributes.Add("Class", "header1_tr_even_row");
                    table.Rows.Add(trAnswer);
                    */

                    //Question number
                    intQuestionIndex += 1;
                    TableCell tcTextNum = new TableCell();
                    trQuestion.Cells.Add(tcTextNum);
                    tcTextNum.Width = Unit.Pixel(25);
                    tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ":<br/>    " + "程式題: ";

                    tcTextNum.Attributes["style"] = "color: Black";

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    TableCell tcTextNumAnswer = new TableCell();
                    trAnswer.Cells.Add(tcTextNumAnswer);
                    tcTextNumAnswer.Width = Unit.Pixel(25);
                    tcTextNumAnswer.Text = "A" + intQuestionIndex.ToString() + ": ";
                    */
                    //Question
                    TableCell tcQuestion = new TableCell();
                    trQuestion.Cells.Add(tcQuestion);
                    tcQuestion.Text = strQuestion;

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    //Answer
                    TableCell tcAnswer = new TableCell();
                    trAnswer.Cells.Add(tcAnswer);
                    tcAnswer.Text = strAnswer;
                     */

                    //建立修改按鈕的TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //新增TableCell 用來增加設定分數的TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //建立Question title的TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    /*
                    //取出此QuestionTitle的內容
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);
                    */

                    //建立間隔
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //建立Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //從ORCS的課堂練習頁面接收參數
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //建立ScoreTextBox 朱君2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "分數：";
                    lblScore.Style["width"] = "50px";

                    //建立ScoreTextBox 朱君2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //累加每一題目的分數
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //建立間隔
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //建立修改問題的Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "220px";
                    btnModifyText.ID = "btnModifyText-" + strQID;
                    btnModifyText.Text = "Modify this question";
                    btnModifyText.Click += new EventHandler(btnModifyText_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //建立間隔
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //建立刪除程式題的Button
                    Button btnDeleteText = new Button();
                    tcModify.Controls.Add(btnDeleteText);
                    btnDeleteText.Style["width"] = "220px";
                    btnDeleteText.ID = "btnDeleteText-" + strQID;
                    btnDeleteText.Text = "Delete this question";
                    btnDeleteText.Click += new EventHandler(btnDeleteText_Click);
                    btnDeleteText.Attributes["class"] = "button_blue";

                    //加入CSS
                    intProgramCount += 1;

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
                //此問卷沒有程式題的情形
            }
            dsProgramList.Dispose();
            #endregion

            #region 列出AITypeQuestion

            strSQL = mySQL.getPaperAITypeQuestionContent(strPaperID);
            DataSet dsAITypeQuestionList = sqldb.getDataSet(strSQL);
            int intAITypeQuestionCount = 0;
            if (dsAITypeQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsAITypeQuestionList.Tables[0].Rows.Count; i++)
                {
                    //取得此問題的QID
                    string strQID = "";
                    strQID = dsAITypeQuestionList.Tables[0].Rows[i]["cQID"].ToString();

                    //Question
                    string strQuestion = "";
                    strQuestion = dsAITypeQuestionList.Tables[0].Rows[i]["cQuestion"].ToString();

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    //Answer
                    string strAnswer = "";
                    strAnswer = dsProgramList.Tables[0].Rows[i]["cAnswer"].ToString();
                    */

                    TableRow trQuestion = new TableRow();
                    table.Rows.Add(trQuestion);
                    trQuestion.Attributes.Add("Class", "header1_table_first_row");

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    TableRow trAnswer = new TableRow();
                    trAnswer.Attributes.Add("Class", "header1_tr_even_row");
                    table.Rows.Add(trAnswer);
                    */

                    //Question number
                    intQuestionIndex += 1;
                    TableCell tcTextNum = new TableCell();
                    trQuestion.Cells.Add(tcTextNum);
                    tcTextNum.Width = Unit.Pixel(25);
                    tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ":<br/>    " + "AITypeQuestion: ";

                    tcTextNum.Attributes["style"] = "color: Black";

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    TableCell tcTextNumAnswer = new TableCell();
                    trAnswer.Cells.Add(tcTextNumAnswer);
                    tcTextNumAnswer.Width = Unit.Pixel(25);
                    tcTextNumAnswer.Text = "A" + intQuestionIndex.ToString() + ": ";
                    */
                    //Question
                    TableCell tcQuestion = new TableCell();
                    trQuestion.Cells.Add(tcQuestion);
                    tcQuestion.Text = strQuestion;

                    //we don't need to show the answer to the program question for the time being.
                    /*
                    //Answer
                    TableCell tcAnswer = new TableCell();
                    trAnswer.Cells.Add(tcAnswer);
                    tcAnswer.Text = strAnswer;
                     */

                    //建立修改按鈕的TableRow
                    TableRow trModify = new TableRow();
                    table.Rows.Add(trModify);

                    //新增TableCell 用來增加設定分數的TextBox
                    TableCell tcTextArea = new TableCell();
                    trModify.Cells.Add(tcTextArea);
                    tcTextArea.HorizontalAlign = HorizontalAlign.Left;
                    tcTextArea.VerticalAlign = VerticalAlign.Top;

                    TableCell tcModify = new TableCell();
                    trModify.Cells.Add(tcModify);
                    tcModify.Attributes["align"] = "right";
                    //tcModify.ColumnSpan = 2;

                    //建立Question title的TextArea
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    tcModify.Controls.Add(txtTitle);
                    txtTitle.ID = "txtTitle" + strQID;
                    txtTitle.Style.Add("WIDTH", "100%");
                    txtTitle.Rows = 5;
                    txtTitle.Style.Add("DISPLAY", "none");

                    /*
                    //取出此QuestionTitle的內容
                    txtTitle.InnerText = myReceiver.getQuestionTitle(strPaperID, strQID);
                    */

                    //建立間隔
                    Label lblCell0 = new Label();
                    lblCell0.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell0);

                    //建立Question title button
                    HtmlInputButton btnTitle = new HtmlInputButton("button");
                    //從ORCS的課堂練習頁面接收參數
                    if (hiddenOpener.Value != "SelectPaperMode")
                    {
                        tcModify.Controls.Add(btnTitle);
                    }
                    btnTitle.ID = "btnTitle" + strQID;
                    btnTitle.Value = "Add a question title";
                    btnTitle.Attributes.Add("onclick", "showQuestionTitle('" + strQID + "')");
                    btnTitle.Style["width"] = "220px";
                    btnTitle.Attributes["class"] = "button_blue";


                    //建立ScoreTextBox 朱君2012/12/25
                    Label lblScore = new Label();
                    tcTextArea.Controls.Add(lblScore);
                    lblScore.Text = "分數：";
                    lblScore.Style["width"] = "50px";

                    //建立ScoreTextBox 朱君2012/12/25
                    TextBox txtScore = new TextBox();
                    txtScore.AutoPostBack = true;
                    tcTextArea.Controls.Add(txtScore);
                    txtScore.ID = "txtScore-" + strQID;
                    txtScore.Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
                    txtScore.TextChanged += new EventHandler(txtScore_TextChange);
                    txtScore.Style["width"] = "80px";
                    //累加每一題目的分數
                    intSumScore += SQLString.GetQuestionScore(strQID, strPaperID);

                    //建立間隔
                    Label lblCell1 = new Label();
                    lblCell1.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                    tcModify.Controls.Add(lblCell1);

                    //建立修改問題的Button
                    Button btnModifyText = new Button();
                    tcModify.Controls.Add(btnModifyText);
                    btnModifyText.Style["width"] = "300px";
                    btnModifyText.ID = "btnModifyText-" + strQID;
                    btnModifyText.Text = "Check/Modify this AITypeQuestion";
                    btnModifyText.Click += new EventHandler(btnModifyText_Click);
                    btnModifyText.Attributes["class"] = "button_blue";

                    //建立間隔
                    Label lblCell = new Label();
                    tcModify.Controls.Add(lblCell);
                    lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                    //建立刪除程式題的Button
                    Button btnDeleteText = new Button();
                    tcModify.Controls.Add(btnDeleteText);
                    btnDeleteText.Style["width"] = "220px";
                    btnDeleteText.ID = "btnDeleteText-" + strQID;
                    btnDeleteText.Text = "Delete this question";
                    btnDeleteText.Click += new EventHandler(btnDeleteText_Click);
                    btnDeleteText.Attributes["class"] = "button_blue";

                    //加入CSS
                    intAITypeQuestionCount += 1;

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
                //此問卷沒有程式題的情形
            }
            dsProgramList.Dispose();
            #endregion
           

            //更改考卷總分
            lblTotalScore.Text = intSumScore.ToString();
        }
        //從ORCS的課堂練習頁面接收參數
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

            //Division由於從課堂練習頁面轉來沒有分類，所以固定用0101(ncku)
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

            //ClinicNum 由於從課堂練習頁面轉來沒有看診單位，所以固定用1
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

            //加入Session ModifyType
            if (Session["ModifyType"] != null)
            {
                Session["ModifyType"] = "Paper";
            }
            else
            {
                Session["ModifyType"] = "Paper";
            }

            //加入Session PreOpener(判斷從ORCS的課堂練習來用)
            if (Session["PreOpener"] != null)
            {
                Session["PreOpener"] = "SelectPaperMode";
            }
            else
            {
                Session["PreOpener"] = "SelectPaperMode";
            }

            //加入ORCS的ExerciseID和GroupID
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

            //用於新增題目用
            if (Request.QueryString["bIsFromClassExercise"] != null)
            {
                Session["IsFromClassExercise"] = Request.QueryString["bIsFromClassExercise"].ToString();
            }

            //用於回到選擇考卷頁面用
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
        //從HINTS的Case頁面接收參數
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

            //加入Session PreOpener(清除從課堂練習來的PreOpener Session)
            if (Session["PreOpener"] != null)
            {
                Session["PreOpener"] = "";
            }
            else
            {
                Session.Add("PreOpener", "");
            }

            //清除ORCS的ExerciseID和GroupID的Session
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
        //設定從課堂練習頁面進來時，此頁面的控制項
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

            if(hiddenbIsFromSelectExistPaper.Value=="True")//從選擇考卷頁面來
            {
                string strURL = "/HINTS/Learning/Exercise/SelectExistPaper.aspx?Opener=SelectPaperMode&cCaseID=" + strCaseID + "&cSectionName=" + strSectionName + "&cPaperID=" + strPaperID + "&cExerciseIDcGroupID=" + Session["ExerciseIDGroupID"].ToString();
                strScript += "window.resizeTo(700, 800);window.location.assign('" + strURL + "');";
            }else if(hiddenOpener.Value == "SelectPaperMode")//若從課堂練習轉來的，則直接關閉頁面
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


        //檢查ORCS是否為上課狀態並回傳課程ID
        protected string CheckClassState()
        {
            clsORCSDB myDb = new clsORCSDB(); //呼叫ORCS資料庫
            string strClassID = ""; //定義正在上課的課程ID
            //抓取使用者的課程ID
            string strSQL = "SELECT * FROM " + clsGroup.TB_ORCS_MemberCourseTeacher + " WHERE cUserID = '" + usi.UserID + "'";
            DataTable dtGroupMember = new DataTable();
            dtGroupMember = myDb.GetDataSet(strSQL).Tables[0];
            if (dtGroupMember.Rows.Count > 0)
            {
                //抓取正在上課的課程ID
                foreach (DataRow drGroupMember in dtGroupMember.Rows)
                {
                    strSQL = "SELECT * FROM ORCS_SystemControl WHERE iClassGroupID = '" + drGroupMember["iGroupID"].ToString() + "' AND cSysControlName = 'SystemControl'";
                    DataTable dtSystemControl = new DataTable();
                    dtSystemControl = myDb.GetDataSet(strSQL).Tables[0];
                    if (dtSystemControl.Rows.Count > 0)
                    {
                        if (dtSystemControl.Rows[0]["iSysControlParam"].ToString() != "0") //判斷該課程是否上課("0":非上課,"1":上課,"2":上課遲到)
                            strClassID = dtSystemControl.Rows[0]["iClassGroupID"].ToString();
                    }
                }
                //若都未上課預設為使用者所擁有課程ID
                if (strClassID.Equals(""))
                {
                    strClassID = dtGroupMember.Rows[0]["iGroupID"].ToString();
                }
            }
            return strClassID;//回傳課程ID
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

       
       
        [WebMethod(EnableSession = true)]
        public static void  LatchPreviousPage(string PreviousPageURL)
        {
            
            HttpContext.Current.Session["PreviousPageURL"] = PreviousPageURL;

            //return HttpContext.Current.Session["PreviousPageURL"].ToString();
        }




        void btnModifyQuestion_ServerClick(object sender, EventArgs e)
        {
            

            //儲存此問卷的Title
            string strPaperTitle = txtPaperTitle.InnerText;
            mySQL.UpdatePaperTitleOfPaper_Header(strPaperID, strPaperTitle);

            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //選擇題含關鍵字填寫理由最大字數
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);

            //取出此問卷的選擇題內容
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //取得QuestionType
                    string strQuestionType = "1";
                    strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //取得QID
                    string strQID = "";
                    strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();

                    //儲存問題的Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strQID)));

                    if (txtTitle != null) //增加判斷問題的Title是否為空，避免出現物件無法讀取null的Bug 蕭凱 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //呼叫儲存Question title的函式
                            mySQL.SaveToPaper_ItemTitle(strPaperID, strQID, "0", strTitle);
                        }
                    }
                }
            }

            //取出此問卷的問答題內容
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //取得QuestionType
                    string strQuestionType = "1";
                    strQuestionType = dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //取得QID
                    string strQID = "";
                    strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();

                    //儲存問題的Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strQID)));

                    if (txtTitle != null) //增加判斷問題的Title是否為空，避免出現物件無法讀取null的Bug 蕭凱 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //呼叫儲存Question title的函式
                            mySQL.SaveToPaper_ItemTitle(strPaperID, strQID, "0", strTitle);
                        }
                    }
                }
            }

            //修改問題的個數
            strSQL = mySQL.getPaper_RandomQuestionNum(strPaperID);
            DataSet dsQuestionNum = sqldb.getDataSet(strSQL);
            //bool bIsNum = true;//判斷此Text欄位內的值是不是數字
            //bool bOverFlow = false;//判斷此Text欄位內的值友沒有超過上限
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

                    //儲存問題的Title
                    HtmlTextArea txtTitle = new HtmlTextArea();
                    try
                    {
                        txtTitle = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txtTitle" + strGroupID)));
                    }
                    catch
                    {
                    }
                    if (txtTitle != null) //增加判斷問題的Title是否為空，避免出現物件無法讀取null的Bug 蕭凱 2014/3/13 
                    {
                        if (txtTitle.InnerText.Trim().Length > 0)
                        {
                            string strTitle = txtTitle.InnerText;

                            //呼叫儲存Question title的函式
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

                    //    //顯示錯誤訊息，輸入的數字不正確
                    //    string strScript = "<script language='javascript'>\n";
                    //    strScript += "ShowErrorMsg('The " + strGroupName + " group has an error');\n";
                    //    strScript += "</script>\n";
                    //    Page.RegisterStartupScript("ShowErrorMsg", strScript);
                    //}

                    //if (bIsNum == true)
                    //{
                    //    //get 此組別全部的問題數目
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
                    //    //把資料存入Paper_RandomQuestion
                    //    //mySQL.saveRandomQuestionNum(strPaperID, strGroupID, intQuestionNum);
                    //}
                    //else
                    //{
                    //    bOverFlow = true;

                    //    //顯示錯誤訊息，使用者設定的值大於此組別的問題數目。
                    //    string strScript = "<script language='javascript'>\n";
                    //    strScript += "ShowErrorMsg('The questions number of " + strGroupName + " should less than " + intQuestionCount.ToString() + "');\n";
                    //    strScript += "</script>\n";
                    //    Page.RegisterStartupScript("ShowErrorMsg", strScript);
                    //}

                    //將使用者選擇問題難易度的數量存入資料表
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
                //沒有資料
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
            //呼叫goNext
            
            //}
        }

        void btnDeleteQuestion_ServerClick(object sender, EventArgs e)
        {
            //將被勾選的問題自Paper_Content刪除
            //取得此組別的選擇題(DataSet)
            string strSQL = mySQL.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //取得QID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //檢查此題目是否有被勾選
                    bool bCheck = false;
                    try
                    {
                        bCheck = ((CheckBox)(this.FindControl("Form1").FindControl("ch-" + strQID))).Checked;
                    }
                    catch
                    {
                        Response.Write("<span style='DISPLAY: none'>讀取" + strQID + "的CheckBox失敗</span>");
                    }

                    if (bCheck == true)
                    {
                        //如果有被勾選，則將資料自Paper_Conent刪除
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //此組別沒有題目的情形
            }
            dsQuestionList.Dispose();

            //取得此組別的問答題
            strSQL = mySQL.getPaperTextContent(strPaperID);
            DataSet dsTextList = sqldb.getDataSet(strSQL);
            if (dsTextList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                {
                    //取得QID
                    string strQID = "";
                    try
                    {
                        strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    //檢查此題目是否有被勾選
                    bool bCheck = false;
                    try
                    {
                        bCheck = ((CheckBox)(this.FindControl("Form1").FindControl("ch-" + strQID))).Checked;
                    }
                    catch
                    {
                        Response.Write("<span style='DISPLAY: none'>讀取" + strQID + "的CheckBox失敗</span>");
                    }

                    if (bCheck == true)
                    {
                        //有被勾選，則將資料自Paper_Conent刪除
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //此組別沒有問答題的情形
            }
            dsTextList.Dispose();

            tcQuestionTable.Controls.Clear();

            //把被選取的問題組別從Paper_RandomQuestion刪除
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

                    //檢查此組別的CheckBox是不是有被選取
                    if (bCheck == true)
                    {
                        //刪除此組別的資料
                        mySQL.deletePaper_RandomQuestionNum(strPaperID, strGroupID);
                    }
                }
            }

            dsQuestionNum.Dispose();
            tcQuestionNumTable.Controls.Clear();

            if (strGenerationMethod == "Edit")
            {
                //建立Main table
                this.setupQuestionTable();

                this.setupQuestionNumTable();
            }
            else
            {
                //建立Question group number Table
                this.setupQuestionNumTable();
            }
        }


        private void btnModify_Click(object sender, EventArgs e)
        {
            //取出ID
            string strQID = ((Button)(sender)).ID.Remove(0, 9);

            //取出此問題在QuestionMode的資料
            string strSQL = mySQL.getSingleQuestionInformation(strQID);
            DataSet dsQuestionInfo = sqldb.getDataSet(strSQL);

            string strQuestionType = "";

            if (dsQuestionInfo.Tables[0].Rows.Count > 0)
            {

                //取得QuestionType
                try
                {
                    strQuestionType = dsQuestionInfo.Tables[0].Rows[0]["cQuestionType"].ToString();
                }
                catch
                {
                }

                //取得QuestionMode
                string strQuestionMode = "";
                try
                {
                    strQuestionMode = dsQuestionInfo.Tables[0].Rows[0]["cQuestionMode"].ToString();
                }
                catch
                {
                }

                //設定QuestionMode
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
                    //找出此問題所屬的Question GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionInfo.Tables[0].Rows[0]["cQuestionGroupID"].ToString();
                    }
                    catch
                    {
                    }

                    //把QuestionGroupID存入Session
                    if (Session["GroupID"] != null)
                    {
                        Session["GroupID"] = strGroupID;
                    }
                    else
                    {
                        Session.Add("GroupID", strGroupID);
                    }

                    //找出此Group屬於的DivisionID
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

                //設定QID的Session
                if (Session["QID"] != null)
                {
                    Session["QID"] = strQID;
                }
                else
                {
                    Session.Add("QID", strQID);
                }

                //設定bModify的Session
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
                //沒有此問題的資料
            }
            dsQuestionInfo.Dispose();

            if (strQuestionType == "6")
            {
                //呼叫Common question editor
                Response.Redirect("./CommonQuestionEdit/Page/showquestionWithKeyWords.aspx?QID=" + strQID + "&Opener=Paper_MainPage");
            }
            else
            {
                Session["PreviousPageURL"]= HttpContext.Current.Request.Url.AbsoluteUri;
                //呼叫Common question editor
                Response.Redirect("./CommonQuestionEdit/Page/ShowQuestion.aspx?QID=" + strQID + "&Opener=Paper_MainPage");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            //取得要被刪除的QID
            string strQID = ((Button)(sender)).ID.Remove(0, 9);

            //呼叫刪除一個問題的SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        private void btnDeleteNum_Click(object sender, EventArgs e)
        {
            //取得要被刪除的GroupID
            string strGroupID = ((Button)(sender)).ID.Remove(0, 13);

            //呼叫刪除一個問題的SQL command
            mySQL.deletePaper_RandomQuestionNum(strPaperID, strGroupID);

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        //修改一個情境題
        private void btnModifySituation_Click(object sender, EventArgs e)
        {
            //取得QID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //取出此問題在QuestionMode的資料
            string strSQL = mySQL.getSingleQuestionInformation(strQID);
            DataSet dsQuestionInfo = sqldb.getDataSet(strSQL);

            if (dsQuestionInfo.Tables[0].Rows.Count > 0)
            {
                //取得QuestionMode
                string strQuestionMode = "";
                try
                {
                    strQuestionMode = dsQuestionInfo.Tables[0].Rows[0]["cQuestionMode"].ToString();
                }
                catch
                {
                }

                //設定QuestionMode
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
                    //找出此問題所屬的Question GroupID
                    string strGroupID = "";
                    try
                    {
                        strGroupID = dsQuestionInfo.Tables[0].Rows[0]["cQuestionGroupID"].ToString();
                    }
                    catch
                    {
                    }

                    //把QuestionGroupID存入Session
                    if (Session["GroupID"] != null)
                    {
                        Session["GroupID"] = strGroupID;
                    }
                    else
                    {
                        Session.Add("GroupID", strGroupID);
                    }

                    //找出此Group屬於的DivisionID
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

                //設定QID的Session
                if (Session["QID"] != null)
                {
                    Session["QID"] = strQID;
                }
                else
                {
                    Session.Add("QID", strQID);
                }

                //設定bModify的Session
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
                //沒有此問題的資料
            }
            dsQuestionInfo.Dispose();

            //呼叫Common question editor
            Response.Redirect("Paper_EmulationQuestion.aspx?QID=" + strQID + "&bModify=" + true + "&Opener=Paper_MainPage");
        }
        //刪除一個情境題
        private void btnDeleteSituation_Click(object sender, EventArgs e)
        {
            //取得QID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //呼叫刪除一個問題的SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }

        private void btnModifyText_Click(object sender, EventArgs e)
        {
            //修改一個問答題

            //取得QID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //設定QID的Session
            if (Session["QID"] != null)
            {
                Session["QID"] = strQID;
            }
            else
            {
                Session.Add("QID", strQID);
            }
            //另存新題Ben2017 11 3
            
            //呼叫Paper_TextQuestionEditor.aspx
            //Response.Redirect("Paper_TextQuestionEditorNew.aspx?QID=" + strQID + "&Opener=Paper_MainPage");

            if (((Button)(sender)).Text == "Check/Modify this AITypeQuestion")
            {
                //找出此Group屬於的DivisionID

                Response.Redirect("../../../../IPC/IPC_Q.aspx?Opener=Paper_QuestionTypeNew" + "&strQID=" + strQID + "&strPaperID=" + strPaperID + "&strQuestionMode=" + hiddenQuestionMode.Value + "&cCaseID=" + strCaseID + "&cSectionName=" + strSectionName + "&viewContent=Yes");
            }



            //呼叫Paper_TextQuestionEditor.aspx
            Response.Redirect("Paper_TextQuestionEditorNew.aspx?QID=" + strQID + "&Opener=Paper_MainPage&bModify=True");
        }

        private void btnDeleteText_Click(object sender, EventArgs e)
        {
            //刪除一個問答題

            //取得QID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //設定QID的Session
            if (Session["QID"] != null)
            {
                Session["QID"] = strQID;
            }
            else
            {
                Session.Add("QID", strQID);
            }

            //刪除該問答題
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }
        //刪除一個圖形題
        private void btnDeleteSimu_Click(object sender, EventArgs e)
        {
            //取得QID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //呼叫刪除一個問題的SQL command
            mySQL.DeleteFromQuestionContent(strPaperID, strQID);

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
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
                Response.Write("<script>window.alert('請輸入整數')</script>");
                ((TextBox)(sender)).Text = SQLString.GetQuestionScore(strQID, strPaperID).ToString();
            }

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }

        }

        //系統自動設定每一題目分數
        protected void btnAutoSetScore_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsPaperQuestion = SQLString.GetPaperQuestion(strPaperID);
                //考卷題目數目
                int intQuestionNum = dsPaperQuestion.Tables[0].Rows.Count;
                if (intQuestionNum > 0)
                {
                    //考卷總分
                    int intTotalScore = int.Parse(txtManualSetScore.Text);
                    int intAvgScore = intTotalScore / intQuestionNum;
                    int intRemainder = intTotalScore % intQuestionNum;

                    for (int i = 0; i < dsPaperQuestion.Tables[0].Rows.Count; i++)
                    {
                        //最後一題將餘數補上
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
                Response.Write("<script>window.alert('總分請輸入整數')</script>");
            }

            //重劃表單
            if (hiddenPresentType.Value == "Edit")
            {
                trQuestionTable.Style.Add("DISPLAY", "");

                //建立Main table
                tcQuestionTable.Controls.Clear();
                this.setupQuestionTable();

                //建立Question num table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
            else
            {
                tcQuestionTable.Controls.Clear();
                trQuestionTable.Style.Add("DISPLAY", "none");

                //建立Question group number Table
                tcQuestionNumTable.Controls.Clear();
                this.setupQuestionNumTable();
            }
        }
        //「Add A New Question」按鈕事件
        protected void btnAddNewQues_onserverclick(object sender, EventArgs e)
        {
            //進入題目型態選擇頁面(GroupID固定為計算機圖學:Group_201211141356547407559)
            //Response.Redirect("Paper_QuestionTypeNew.aspx?Opener=Paper_MainPage&GroupID=Group_201211141356547407559&bModify=False");
            string strURL = "";
            if (Request.QueryString["CaseID"] != null)
                strURL = "./QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question&CaseID=" + Request.QueryString["CaseID"].ToString();
            else
                strURL = "./QuestionGroupTree/QGroupTreeNew.aspx?Opener=SelectPaperModeAddANewQuestion&ModifyType=Question";


            //Session["PreviousPageURL"] = HttpContext.Current.Request.Url.AbsoluteUri;
            //Response.Redirect(strURL);
            string strJSCode = "window.open('" + strURL + "','開啟題庫系統新增題目','scrollbars=yes,resizable=yes,directories=0,location=1,menubar=0,status=0,titlebar=1,toolbar=0,fullscreen=yes')";
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "", "<script>" + strJSCode + "</script>");
        }
        //「Finish」按鈕事件
        protected void btnFinish2_onserverclick(object sender, EventArgs e)
        {
            if (Session["IsFromClassExercise"] != null)
                Session.Remove("IsFromClassExercise");
            if (Session["IsFromSelectExistPaper"] != null)
                Session.Remove("IsFromSelectExistPaper");
            if (Session["PreOpener"] != null)
                Session.Remove("PreOpener");

            //檢查是否將剩餘未分配分數平均分配給0分題目
            if (hiddenIsAvgScoreToZeroScore.Value.Equals("true"))
            {
                //取出考卷所有題目資訊
                DataTable dtPaperQuestion = SQLString.GetPaperQuestion(strPaperID).Tables[0];
                //考卷題目數目
                int intQuestionNum = dtPaperQuestion.Rows.Count;
                //考卷總分
                int intTotalScore = int.Parse(txtManualSetScore.Text);
                //考卷題目總分
                int intQuestionTotalScore = 0;
                //紀錄零分題目
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
                        //紀錄考卷中零分題目
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
                Page.RegisterClientScriptBlock("alert", "<script>alert('請輸入考卷標題')</script>");
            }
            else //插入Paper_Classify資料表
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
                    //考卷教學活動
                    case "ExaminationActivity":
                        clsMLASDB MLASDB = new clsMLASDB();
                        string strActivityID = Session["ActivityID"].ToString();
                        //更新隨堂測驗教學活動考卷訊息
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
                    case "EditPaper"://Hints的編輯考卷
                        script = "<script> alert('考卷編輯完成');window.close(); </script>";
                        ClientScript.RegisterClientScriptBlock(this.GetType(), "close_window", script);
                        Session.Remove("ComeFromActivityName");
                        break;
                    default:
                        Response.Redirect("../../../PushMessage/MessaeMember.aspx?Opener=Paper_MainPage" + "&cExerciseIDcGroupID=" + Session["ExerciseIDGroupID"].ToString() + "&cCaseID=" + strCaseID + "&cPaperID=" + strPaperID);
                        break;
                }
            }

        }
        //檢查考卷有無零分
        protected void btnFinish3_onserverclick(object sender, EventArgs e)
        {
            //儲存此問卷的Title
            string strPaperTitle = txtPaperTitle.InnerText;
            mySQL.UpdatePaperTitleOfPaper_Header(strPaperID, strPaperTitle);

            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //選擇題含關鍵字填寫理由最大字數
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);

            string script = "";
            DataTable dtPaperQuestion = SQLString.GetPaperQuestion(strPaperID).Tables[0];
            //考卷題目數目
            int intQuestionNum = dtPaperQuestion.Rows.Count;
            if (intQuestionNum > 0)
            {
                //考卷總分
                int intTotalScore = int.Parse(txtManualSetScore.Text);
                //考卷題目總分
                int intQuestionTotalScore = 0;
                //紀錄零分題目
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
                        //紀錄考卷中零分題目
                        ZeroPointQuestions.Add(drPaperQuestion["cQID"].ToString());
                    }
                }
                //如果題目總分大於考卷總分
                if (intQuestionTotalScore > intTotalScore)
                {
                    script = "alert('目前考卷題目分數總和大於考卷總分，請修改考卷題目分數!');";
                }
                else//小於等於
                {
                    //如果題目分數等於考卷總分
                    if (intQuestionTotalScore == intTotalScore)
                    {
                        //存在有無零分題目
                        if (ZeroPointQuestions.Count > 0)
                            script = "alert('目前考卷有設定分數為0分題目，請修改考卷題目分數!');";
                        else
                            script = "document.getElementById('btnFinish2').click();";
                    }
                    else//小於
                    {
                        if (ZeroPointQuestions.Count > 0)
                            script = "if(confirm('是否平均分配考卷剩餘分數於0分題目並完成編輯考卷?')){document.getElementById('hiddenIsAvgScoreToZeroScore').value='true';document.getElementById('btnFinish2').click();}";
                        else
                            script = "alert('目前考卷題目分數總和小於考卷總分，請修改考卷題目分數');";
                    }
                }
            }
            else
            {
                script = "alert('目前考卷無題目，請重新編輯考卷!');";
            }
            ClientScript.RegisterStartupScript(this.GetType(), "alert_window", "<script>" + script + "</script>", false);
        }

        protected void tbMaximumNumberOfWordsReasons_TextChanged(object sender, EventArgs e)
        {
            string strMaximumNumberOfWordsReasons = tbMaximumNumberOfWordsReasons.Text;
            //選擇題含關鍵字填寫理由最大字數
            mySQL.UpdatePaperMaximumNumberOfWordsReasonsOfPaper_Header(strPaperID, strMaximumNumberOfWordsReasons);
        }

        //重新整理頁面
        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            tbMaximumNumberOfWordsReasons.Text = strMaximumNumberOfWordsReasons;

            //設定txtTitle
            this.setupPaperTitle();

            //檢查有無需要自動平均題目分數
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SetAutoScorll", "document.getElementById('btnAutoSetScore').click();", true);
        }
    }
}