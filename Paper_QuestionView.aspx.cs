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
using suro.util;
using AuthoringTool.QuestionEditLevel;
using Hints.Learning.Question;
using Hints.DB.Section;
using Hints.DB;

namespace PaperSystem
{
    /// <summary>
    /// Paper_QuestionView ���K�n�y�z�C
    /// </summary>
    public partial class Paper_QuestionView : AuthoringTool_BasicForm_BasicForm
    {
        //�إ�SqlDB����
        SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
        DataReceiver myReceiver = new DataReceiver();
        SQLString mySQL = new SQLString();

        //���D�էO��ID
        protected string strGroupID;

        //���D�էO���W��
        protected string strGroupName;

        //�D�ت��s��
        int intQuestionIndex = 0;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Initiate();

            //�����Ѽ�
            this.getParameter();

            if (!IsPostBack)
            {
                hfSymptoms.Value = "All";
                ddlSymptoms.Items.Clear();
                //�إ߯f�x���U�Կ�涵�� 
                DataTable dtDiseaseSymptomsTree = DiseaseSymptomsTree_SELECT();
                ddlSymptoms.Items.Add("All");
                foreach (DataRow drDiseaseSymptomsTree in dtDiseaseSymptomsTree.Rows)
                {
                    ddlSymptoms.Items.Add(drDiseaseSymptomsTree["cNodeName"].ToString());
                }
            }

            intQuestionIndex = 0;

            //�إ߿���D���
            this.setupQuestionTable();

            //�إ߰ݵ��D���
            this.setupTextQuestionTable();

            //�[�JDelete button���ƥ�
            btnDeleteQuestion.ServerClick += new ImageClickEventHandler(btnDeleteQuestion_ServerClick);

        }

        private void setupTextQuestionTable()
        {
            tcTextQuestionTable.Controls.Clear();
            Table table = new Table();
            tcTextQuestionTable.Controls.Add(table);
            table.CellSpacing = 0;
            table.CellPadding = 5;
            table.BorderStyle = BorderStyle.Solid;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderColor = System.Drawing.Color.Black;
            table.GridLines = GridLines.Both;
            table.Width = Unit.Percentage(100);


            //�إ�Table��CSS
            table.CssClass = "header1_table";

            //�̷�QuestionMode�M�w���X���էO���ݵ��D
            string strSQL = mySQL.getGroupTextQuestion(Session["GroupID"].ToString());

            DataSet dsQuestionList = sqldb.getDataSet(strSQL);

            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    string strGetKeyword = Hints.Learning.Question.DataReceiver.getTextQuestionKeyword(dsQuestionList.Tables[0].Rows[i]["cQID"].ToString());
                    string[] arrKeyword = strGetKeyword.Split(',');

                    //���oQID
                    string strQID = "";
                    strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();

                    //���oQuestionType
                    string strQuestionType = "2";
                    strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //���oQuestion
                    string strQuestion = "";
                    strQuestion = dsQuestionList.Tables[0].Rows[i]["cQuestion"].ToString();

                    //���oAnswer
                    string strAnswer = "";
                    strAnswer = dsQuestionList.Tables[0].Rows[i]["cAnswer"].ToString();

                    //���o�f�x
                    string strSymptoms = "";
                    strSymptoms = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_SELECT_QuestionSymptoms(strQID);
                    if (hfSymptoms.Value == "All" || hfSymptoms.Value == strSymptoms)
                    {
                        intQuestionIndex += 1;
                        #region �إ߰ݵ��D�����e
                        TableRow trTextQuestionTitle = new TableRow();
                        trTextQuestionTitle.Attributes.Add("Class", "header1_table_first_row");
                        trTextQuestionTitle.Style.Add("CURSOR", "hand");
                        table.Rows.Add(trTextQuestionTitle);
                        trTextQuestionTitle.Attributes.Add("onclick", "ShowDetail('" + strQID + "','img_" + strQID + "')");

                        //�إ߰��D�����D
                        TableRow trQuestionTitle = new TableRow();
                        trQuestionTitle.Attributes.Add("Class", "header1_tr_odd_row");
                        table.Rows.Add(trQuestionTitle);
                        trQuestionTitle.ID = "trQuestionTitle_" + strQID;

                        //�إ߰��D�����e
                        TableRow trQuestion = new TableRow();
                        trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trQuestion);
                        trQuestion.ID = "trQuestion_" + strQID;

                        //�إ�Answer�����D
                        TableRow trAnswerTitle = new TableRow();
                        trAnswerTitle.Attributes.Add("Class", "header1_tr_odd_row");
                        table.Rows.Add(trAnswerTitle);
                        trAnswerTitle.ID = "trAnswerTitle_" + strQID;

                        //�إ�Answer�����e
                        TableRow trAnswer = new TableRow();
                        trAnswer.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trAnswer);
                        trAnswer.ID = "trAnswer_" + strQID;

                        //�إ�Keyword�����D
                        TableRow trKeywordTitle = new TableRow();
                        trKeywordTitle.Attributes.Add("Class", "header1_tr_odd_row");
                        table.Rows.Add(trKeywordTitle);
                        trKeywordTitle.ID = "trKeywordTitle_" + strQID;

                        //�إ�Keyword�����e
                        TableRow trKeyword = new TableRow();
                        trKeyword.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trKeyword);
                        trKeyword.ID = "trKeyword_" + strQID;

                        //�إߦP�q���D�����D
                        TableRow trSynQuestionTitle = new TableRow();
                        trSynQuestionTitle.Attributes.Add("Class", "header1_tr_odd_row");
                        table.Rows.Add(trSynQuestionTitle);
                        trSynQuestionTitle.ID = "trSynQuestionTitle_" + strQID;
                        trSynQuestionTitle.Style.Add("CURSOR", "hand");

                        //�إߦP�q���D�����e
                        TableRow trSynQuestion = new TableRow();
                        trSynQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trSynQuestion);
                        trSynQuestion.ID = "trSynQuestion_" + strQID;
                        trSynQuestionTitle.Attributes.Add("onclick", "ShowSynDetail('" + trSynQuestion.ID + "' , 'imgSynQuestion_" + strQID + "')");
                        trSynQuestion.Style.Add("display", "none");

                        //�إߦP�q���ת����D
                        TableRow trSynAnswerTitle = new TableRow();
                        trSynAnswerTitle.Attributes.Add("Class", "header1_tr_odd_row");
                        table.Rows.Add(trSynAnswerTitle);
                        trSynAnswerTitle.ID = "trSynAnswerTitle_" + strQID;
                        trSynAnswerTitle.Style.Add("CURSOR", "hand");

                        //�إߦP�q���ת����e
                        TableRow trSynAnswer = new TableRow();
                        trSynAnswer.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trSynAnswer);
                        trSynAnswer.ID = "trSynAnswer_" + strQID;
                        trSynAnswerTitle.Attributes.Add("onclick", "ShowSynDetail('" + trSynAnswer.ID + "' , 'imgSynAnswer_" + strQID + "')");
                        trSynAnswer.Style.Add("display", "none");

                        #region �ޱ��P�q������ܻP����
                        DataTable dtSynQuestion = new DataTable();
                        DataTable dtSynAnswer = new DataTable();
                        dtSynQuestion = clsInterrogationEnquiry.GetSynQuestion(strQID);
                        dtSynAnswer = clsInterrogationEnquiry.GetSynAnswer(strQID);
                        if (dtSynQuestion.Rows.Count == 0)
                        {
                            trSynQuestionTitle.Style.Add("display", "none");
                            trSynQuestion.Style.Add("display", "none");
                        }
                        if (dtSynAnswer.Rows.Count == 0)
                        {
                            trSynAnswerTitle.Style.Add("display", "none");
                            trSynAnswer.Style.Add("display", "none");
                        }
                        #endregion

                        //���D���D��
                        //					TableCell tcQuestionNum = new TableCell();
                        //					trQuestion.Cells.Add(tcQuestionNum);
                        //					tcQuestionNum.Style["width"] = "50px";
                        //					tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";



                        TableCell tcTextQuestionTitle = new TableCell();
                        trTextQuestionTitle.Cells.Add(tcTextQuestionTitle);
                        tcTextQuestionTitle.Text = "Q" + intQuestionIndex.ToString() + ": ";
                        tcTextQuestionTitle.Text = "<IMG id='img_" + strQID + "' src='../../../BasicForm/Image/minus.gif'>&nbsp;Q" + intQuestionIndex.ToString() + " : ";

                        TableCell tcQuestionTitle = new TableCell();
                        trQuestionTitle.Cells.Add(tcQuestionTitle);
                        tcQuestionTitle.Text = "<font style='color:Black; font-weight:bold'>Question : <font/>";

                        TableCell tcSynQuestionTitle = new TableCell();
                        trSynQuestionTitle.Cells.Add(tcSynQuestionTitle);
                        tcSynQuestionTitle.Text = "<IMG id='imgSynQuestion_" + strQID + "' src='../../../BasicForm/Image/plus.gif'><font style='color:Black; font-weight:bold'>&nbsp;Synonymous Question : <font/>";

                        TableCell tcAnswerTitle = new TableCell();
                        trAnswerTitle.Cells.Add(tcAnswerTitle);
                        tcAnswerTitle.Text = "<font style='color:Black; font-weight:bold'>Answer :<font/>";

                        TableCell tcSynAnswerTitle = new TableCell();
                        trSynAnswerTitle.Cells.Add(tcSynAnswerTitle);
                        tcSynAnswerTitle.Text = "<IMG id='imgSynAnswer_" + strQID + "' src='../../../BasicForm/Image/plus.gif'><font style='color:Black; font-weight:bold'>&nbsp;Synonymous Answer : <font/>";

                        TableCell tcKeywordTitle = new TableCell();
                        trKeywordTitle.Cells.Add(tcKeywordTitle);
                        tcKeywordTitle.Text = "<font style='color:Black; font-weight:bold'>Keyword : <font/>";

                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);

                        if (arrKeyword.Length > 0)
                        {
                            for (int kcount = 0; kcount < arrKeyword.Length; kcount++)
                            {
                                strQuestion = strQuestion.Replace(arrKeyword[kcount], "<span class='span_keyword' >" + arrKeyword[kcount] + "</span>"); ;
                                tcQuestion.Text = strQuestion;
                            }
                        }

                        TableCell tcSynQuestion = new TableCell();
                        trSynQuestion.Cells.Add(tcSynQuestion);
                        BulidInterrogation("Question", strQID, tcSynQuestion);

                        TableCell tcAnswer = new TableCell();
                        trAnswer.Cells.Add(tcAnswer);

                        for (int kcount = 0; kcount < arrKeyword.Length; kcount++)
                        {
                            strAnswer = strAnswer.Replace(arrKeyword[kcount], "<span class='span_keyword' >" + arrKeyword[kcount] + "</span>"); ;
                        }

                        tcAnswer.Text = strAnswer;

                        TableCell tcSynAnswer = new TableCell();
                        trSynAnswer.Cells.Add(tcSynAnswer);
                        BulidInterrogation("Answer", strQID, tcSynAnswer);


                        TableCell tcKeyword = new TableCell();
                        trKeyword.Cells.Add(tcKeyword);
                        tcKeyword.Text = strGetKeyword;


                        //�إ߭ק���s��TableRow
                        TableRow trModify = new TableRow();
                        //trModify.Attributes.Add("Class", "header1_tr_even_row");
                        table.Rows.Add(trModify);
                        trModify.ID = "trModify_" + strQID;

                        TableCell tcModify = new TableCell();
                        trModify.Cells.Add(tcModify);
                        tcModify.Attributes["align"] = "right";
                        //					tcModify.ColumnSpan = 2;

                        //���D��������
                        Label lbQuestionLevel = new Label();
                        tcModify.Controls.Add(lbQuestionLevel);
                        int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelValue(strQID);
                        if (iQuestionLevel != -1)
                            lbQuestionLevel.Text = "Question Level�G" + AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_LevelName(iQuestionLevel) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        //���D���f�x
                        Label lbQuestionSymptoms = new Label();
                        tcModify.Controls.Add(lbQuestionSymptoms);
                        lbQuestionSymptoms.Text = "Question Symptoms�G" + AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_SELECT_QuestionSymptoms(strQID) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        //�إ߭ק���D��Button
                        Button btnModifyText = new Button();
                        tcModify.Controls.Add(btnModifyText);
                        btnModifyText.Style["width"] = "150px";
                        btnModifyText.ID = "btnModifyText-" + strQID;
                        btnModifyText.Text = "Modify";
                        btnModifyText.Click += new EventHandler(btnModifyText_Click);
                        btnModifyText.CssClass = "button_continue";

                        //�إ߶��j
                        Label lblCell = new Label();
                        tcModify.Controls.Add(lblCell);
                        lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        //�إߧR���ݵ��D��Button
                        Button btnDeleteText = new Button();
                        tcModify.Controls.Add(btnDeleteText);
                        btnDeleteText.Style["width"] = "150px";
                        btnDeleteText.ID = "btnDeleteText-" + strQID;
                        btnDeleteText.Text = "Delete";
                        btnDeleteText.Click += new EventHandler(btnDeleteText_Click);
                        btnDeleteText.CssClass = "button_continue";


                        //�إ�Space
                        TableRow trSpace = new TableRow();
                        trSpace.Style.Add("background-color", "#EBECED");
                        table.Rows.Add(trSpace);

                        TableCell tcSpace = new TableCell();
                        tcSpace.Style.Add("height", "7px");
                        trSpace.Cells.Add(tcSpace);

                        //if (intQuestionIndex % 2 == 0)
                        //{
                        //    trQuestion.Attributes.Add("Class", "header1_tr_odd_row");
                        //    trModify.Attributes.Add("Class", "header1_tr_odd_row");
                        //}
                        //else
                        //{
                        //    trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        //    trModify.Attributes.Add("Class", "header1_tr_even_row");
                        //}

                        #endregion
                    }
                }
            }
            else
            {
                //���ݨ��S������ݵ��D�����p
                trTextQuestionTable.Style["display"] = "none";
            }
            dsQuestionList.Dispose();
        }

        private void setupQuestionTable()
        {
            tcQuestionTable.Controls.Clear();
            Table table = new Table();
            tcQuestionTable.Controls.Add(table);
            table.CellSpacing = 0;
            table.CellPadding = 2;
            table.BorderStyle = BorderStyle.Solid;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderColor = System.Drawing.Color.Black;
            table.GridLines = GridLines.Both;
            table.Width = Unit.Percentage(100);

            //�إ�Table��CSS
            table.CssClass = "header1_table";

            //�̷�QuestionMode�M�w���X���էO������D
            string strSQL = mySQL.getGroupSelectionQuestion(Session["GroupID"].ToString());


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

                    //���o���D��SQL
                    DataSet dsQuestion = null;
                    if (hfSymptoms.Value == "All")
                        strSQL = mySQL.getQuestion(strQID);
                    else
                        strSQL = mySQL.getQuestionBySymptoms(strQID, hfSymptoms.Value);

                    dsQuestion = sqldb.getDataSet(strSQL);
                    if (dsQuestion.Tables[0].Rows.Count > 0)
                    {
                        //�إ߰��D�����e
                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        intQuestionIndex += 1;

                        //���D��CheckBox
                        //						TableCell tcCheckBox = new TableCell();
                        //						trQuestion.Cells.Add(tcCheckBox);
                        //						tcCheckBox.Width = Unit.Pixel(25);
                        //
                        //						CheckBox chQuestion = new CheckBox();
                        //						tcCheckBox.Controls.Add(chQuestion);
                        //						tcCheckBox.Attributes.Add("onclick","ShowbtnDelete();");
                        //						string strID = "";
                        //						strID = "ch-" + dsQuestion.Tables[0].Rows[0]["cQID"].ToString();
                        //						
                        //						chQuestion.ID = strID;

                        //���D���D��
                        TableCell tcQuestionNum = new TableCell();
                        trQuestion.Cells.Add(tcQuestionNum);
                        tcQuestionNum.Width = Unit.Pixel(25);
                        tcQuestionNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //���D�����e
                        string strQuestion = "";
                        strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();

                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        tcQuestion.Text = strQuestion;

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
                                strSeq = dsSelection.Tables[0].Rows[j]["sSeq"].ToString();

                                //Selection
                                string strSelection = "";
                                strSelection = dsSelection.Tables[0].Rows[j]["cSelection"].ToString();

                                //bCaseSelect
                                bool bCaseSelect = false;
                                bCaseSelect = Convert.ToBoolean(dsSelection.Tables[0].Rows[j]["bCaseSelect"]);

                                TableRow trSelection = new TableRow();
                                table.Rows.Add(trSelection);

                                //�O�_����ĳ�ﶵ
                                TableCell tcSuggest = new TableCell();
                                trSelection.Cells.Add(tcSuggest);
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

                                //�ﶵ�s��
                                //								TableCell tcSelectionNum = new TableCell();
                                //								trSelection.Cells.Add(tcSelectionNum);
                                //								tcSelectionNum.Text = Convert.ToString((j+1)) + ".";

                                //�ﶵ���e
                                TableCell tcSelection = new TableCell();
                                trSelection.Cells.Add(tcSelection);
                                tcSelection.Text = strSelection;

                                //Empty TableCell
                                //								TableCell tcEmpty = new TableCell();
                                //								trSelection.Cells.Add(tcEmpty);

                                //�إ߿ﶵ��CSS
                                if ((Convert.ToInt32(strSeq) % 2) != 0)
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_odd_row");
                                }
                                else
                                {
                                    trSelection.Attributes.Add("Class", "header1_tr_even_row");
                                }
                            }
                        }
                        else
                        {
                            //�����D�S���ﶵ
                        }
                        dsSelection.Dispose();

                        //Modify and Delete ���s��TableRow
                        TableRow trModify = new TableRow();
                        table.Rows.Add(trModify);

                        //�إ߭ק���D��Button
                        TableCell tcModify = new TableCell();
                        trModify.Cells.Add(tcModify);
                        tcModify.Attributes["align"] = "right";
                        tcModify.ColumnSpan = 2;

                        //���D������
                        Label lbQuestionGrade = new Label();
                        tcModify.Controls.Add(lbQuestionGrade);
                        string strQuestionGrade = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_SELECT_Grade(strQID);
                        if (strQuestionGrade != "-1")
                            lbQuestionGrade.Text = "Question Grade�G" + strQuestionGrade + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        //���D��������
                        Label lbQuestionLevel = new Label();
                        tcModify.Controls.Add(lbQuestionLevel);
                        int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelValue(strQID);
                        if (iQuestionLevel != -1)
                            lbQuestionLevel.Text = "Question Level�G" + AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_LevelName(iQuestionLevel) + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        Button btnModifySelection = new Button();
                        tcModify.Controls.Add(btnModifySelection);
                        btnModifySelection.ID = "btnModifySelection-" + strQID;
                        btnModifySelection.Text = "Modify";
                        btnModifySelection.Click += new EventHandler(btnModifySelection_Click);
                        btnModifySelection.Style["width"] = "150px";

                        //�إ߶��j
                        Label lblCell = new Label();
                        tcModify.Controls.Add(lblCell);
                        lblCell.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

                        Button btnDeleteSelection = new Button();
                        tcModify.Controls.Add(btnDeleteSelection);
                        btnDeleteSelection.ID = "btnDeleteSelection-" + strQID;
                        btnDeleteSelection.Text = "Delete";
                        btnDeleteSelection.Click += new EventHandler(btnDeleteSelection_Click);
                        btnDeleteSelection.Style["width"] = "150px";


                        //�إ�Space
                        TableRow trSpace = new TableRow();
                        trSpace.Style.Add("background-color", "#EBECED");
                        table.Rows.Add(trSpace);

                        TableCell tcSpace = new TableCell();
                        tcSpace.ColumnSpan = 2;
                        tcSpace.Style.Add("height", "7px");
                        trSpace.Cells.Add(tcSpace);


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
                //���ݨ��S���������D�����p
                trQuestionTable.Style["display"] = "none";
            }
            dsQuestionList.Dispose();
        }

        private void getParameter()
        {
            //GroupID
            string strGroupID = "";

            if (Session["GroupID"] != null)
                strGroupID = Session["GroupID"].ToString();

            if (Request.QueryString["GroupID"] != null)
            {
                strGroupID = Request.QueryString["GroupID"].ToString();
                if (Session["GroupID"] != null)
                {
                    Session["GroupID"] = strGroupID;
                }
                else
                {
                    Session.Add("GroupID", strGroupID);
                }
            }

            //GroupDivisionID
            if (strGroupID.Trim().Length > 0)
            {
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

            //GroupName
            strGroupName = DataReceiver.getQuestionGroupNameByQuestionGroupID(strGroupID);
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

        /// <summary>
        /// �N�Q�Ŀ諸���D��Question group���R��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteQuestion_ServerClick(object sender, ImageClickEventArgs e)
        {
            //���o���էO������D(DataSet)
            string strSQL = mySQL.getGroupSelectionQuestion(Session["GroupID"].ToString());
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
                        //�p�G���Q�Ŀ�A�h�N��Ʀ�QuestionIndex , QuestionSelectionIndex , QuestionMode �R��
                        mySQL.DeleteGeneralQuestion(strQID);
                    }
                }
            }
            else
            {
                //���էO�S���D�ت�����
            }
            dsQuestionList.Dispose();

            tcQuestionTable.Controls.Clear();

            intQuestionIndex = 0;

            //�إ�Main table
            this.setupQuestionTable();
        }

        private void btnModifySelection_Click(object sender, EventArgs e)
        {
            //�ק�@�ӿ���D

            //���XID
            //string strQID = ((Button)(sender)).ID;
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

            string strScript = "<script language='javascript'>\n";
            strScript += "alert('Save successfully!');";
            strScript += "</script>\n";
            Page.RegisterStartupScript("ShowErrorMsg", strScript);

            //�I�sCommon question editor
            Response.Redirect("./CommonQuestionEdit/Page/ShowQuestion.aspx?QID=" + strQID + "&GroupID=" + Session["GroupID"].ToString());
        }

        private void btnDeleteSelection_Click(object sender, EventArgs e)
        {
            //���XID
            string[] strIDArray = ((Button)(sender)).ID.Split('-');
            string strQID = strIDArray[1];

            //�R���ӿ���D
            mySQL.DeleteGeneralQuestion(strQID);

            intQuestionIndex = 0;

            //�إ�Main table
            tcQuestionTable.Controls.Clear();
            this.setupQuestionTable();
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
            SQLString.deleteTextQuestionByQID(strQID);

            //�R���Ӱݵ��D���P�q���D�P����
            clsInterrogationEnquiry.DeleteQuestionItem(strQID);

            //�M��
            tcQuestionTable.Controls.Clear();
            tcTextQuestionTable.Controls.Clear();

            intQuestionIndex = 0;

            //�إ߿���D
            this.setupQuestionTable();

            //�إ߰ݵ��D
            this.setupTextQuestionTable();
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

            //�I�sPaper_TextQuestionEditor.aspx
            Response.Redirect("Paper_TextQuestionEditor.aspx?QID=" + strQID + "&GroupID=" + Session["GroupID"].ToString());
        }

        //�إߦP�q���D�ε���
        private void BulidInterrogation(string strType, string strQID, TableCell tcSynQuestion)
        {
            DataTable dtSyn = new DataTable();
            if (strType == "Question")
            {
                dtSyn = clsInterrogationEnquiry.GetSynQuestion(strQID);
            }
            else if (strType == "Answer")
            {
                dtSyn = clsInterrogationEnquiry.GetSynAnswer(strQID);
            }

            if (dtSyn.Rows.Count > 0)
            {
                #region �P�q���D�ε��פ��e
                DataTable dtSynonymousItem = clsInterrogationEnquiry.GetSynonymousItem(strType, "synonymous", strQID);
                if (dtSynonymousItem.Rows.Count > 0)
                {
                    string strDataName = "";//��ܸ�ƪ����W��
                    string strMode = "";//��ܬO�P�q���D�ε��ת��Ҧ�
                    if (strType == "Question")
                    {
                        strDataName = "cQDataKind";
                        strMode = "SynQ";
                    }
                    else if (strType == "Answer")
                    {
                        strDataName = "cADataKind";
                        strMode = "SynA";
                    }

                    for (int iSyn = 0; iSyn < dtSynonymousItem.Rows.Count; iSyn++)
                    {
                        #region �P�q���ت��Ǹ�
                        Label lbSynItemT = new Label();
                        if (iSyn == 0)
                        {
                            lbSynItemT.Text = "Syn. " + (iSyn + 1) + ": ";
                        }
                        else
                        {
                            lbSynItemT.Text = "<br/>Syn. " + (iSyn + 1) + ": ";
                        }
                        tcSynQuestion.Controls.Add(lbSynItemT);
                        #endregion

                        #region �P�q���ت����e
                        if (dtSynonymousItem.Rows[iSyn][strDataName].ToString().IndexOf("Multimedia") != -1)
                        {
                            string strItemSeq = dtSynonymousItem.Rows[iSyn]["sSeq"].ToString();
                            string filePath = dtSynonymousItem.Rows[iSyn]["cItemValue"].ToString();
                            string strFileName = filePath.Split('/')[2];//�ɦW(�t���ɦW)
                            string strTemp = strFileName.Split('.')[1];//���ɦW
                            string strFileNameNoVice = strFileName.Split('.')[0]; //�ɦW
                            if (strTemp == "swf")
                            {
                                Label lbFlash = new Label();
                                lbFlash.Text = "<object classid='clsid:D27CDB6E-AE6D-11cf-96B8-444553540000' id='FlashPlayer" + strItemSeq + strFileNameNoVice + "' width='320' height='240' " +
                                 " codebase='http://fpdownload.macromedia.com/get/flashplayer/current/swflash.cab'> " +
                                 " <param name='pluginspage' value='http://www.macromedia.com/go/getflashplayer' /> " +
                                 " <param name='movie' value='../../MultiMediaDB/Upload/Image/" + strFileName + "' /> " +
                                 " <param name='quality' value='high' /> " +
                                 " <param name='bgcolor' value='#869ca7' /> " +
                                 " <param name='allowScriptAccess' value='always' /> " +
                                 " <param name='Play' value='false' /> " +
                                  "</object>" +
                                  "<input id='btnPlay' type='button' value='Play' onclick=\"PlayFlash('" + strItemSeq + "','" + strFileNameNoVice + "')\"' style='width:100px' />";
                                tcSynQuestion.Controls.Add(lbFlash);
                            }
                            else if (strTemp == "mp3" || strTemp == "wav" || strTemp == "wmv" || strTemp == "avi")
                            {
                                Label lbMedia = new Label();
                                lbMedia.Text = " <object id='MediaPlayer' height='240' width='320' classid='CLSID:6BF52A52-394A-11d3-B153-00C04F79FAA6'> " +
                                               " <param name='AutoStart' value='false' />" +
                                               " <param name='uiMode' value='Full' />" +
                                               " <param name='enabled' value='true' />" +
                                               " <param name='URL' value='../../MultiMediaDB/Upload/Image/" + strFileName + "'>" +
                                               " </object> ";
                                tcSynQuestion.Controls.Add(lbMedia);
                            }
                            else if (strTemp == "jpg" || strTemp == "bmp" || strTemp == "gif" || strTemp == "png")
                            {
                                System.Web.UI.WebControls.Image imgPic = new System.Web.UI.WebControls.Image();
                                imgPic.ImageUrl = "../../MultiMediaDB/Upload/Image/" + strFileName + "";
                                imgPic.Width = Unit.Pixel(320);
                                imgPic.Height = Unit.Pixel(240);
                                tcSynQuestion.Controls.Add(imgPic);
                            }
                        }
                        else
                        {
                            Label lbSynItemValue = new Label();
                            lbSynItemValue.Text = dtSynonymousItem.Rows[iSyn]["cItemValue"].ToString();

                            tcSynQuestion.Controls.Add(lbSynItemValue);
                        }

                        #endregion


                    }
                }
                #endregion
            }


        }

        protected void ddlSymptoms_SelectedIndexChanged(object sender, EventArgs e)
        {
            hfSymptoms.Value = ddlSymptoms.SelectedValue;

            intQuestionIndex = 0;

            //�إ߿���D���
            this.setupQuestionTable();

            //�إ߰ݵ��D���
            this.setupTextQuestionTable();

        }

        //���o�f�x����
        private DataTable DiseaseSymptomsTree_SELECT()
        {
            clsHintsDB HintsDB = new clsHintsDB();
            DataTable dtDiseaseSymptomsTree = new DataTable();
            string strSQL_DiseaseSymptomsTree = "SELECT  DISTINCT cNodeName FROM DiseaseSymptomsTree WHERE cParentID != 'Diseaseroot' ORDER BY cNodeName ASC";
            dtDiseaseSymptomsTree = HintsDB.getDataSet(strSQL_DiseaseSymptomsTree).Tables[0];
            return dtDiseaseSymptomsTree;
        }
    }
}
