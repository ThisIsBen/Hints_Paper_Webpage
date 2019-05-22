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
using Hints.DB;

namespace PaperSystem
{
    /// <summary>
    /// Paper_SelectQuestion ���K�n�y�z�C
    /// �ϥΪ̦ۦ�Ŀ��D�ضi�J���ݨ��������C
    /// </summary>
    public partial class Paper_SelectQuestion : AuthoringTool_BasicForm_BasicForm
    {
        //�إ�SqlDB����
        SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationManager.AppSettings["connstr"]);
        //SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
        DataReceiver myReceiver = new DataReceiver();
        SQLString mySQL = new SQLString();

        string SessionQuestionType = "";
        string strUserID, strCaseID, strClinicNum, strSectionName, strEditMode, strPaperID;
        string strGroupID = "";
        string strGroupDivisionID = "";
        bool bAllowSelect = true;

        int intQuestionIndex = 0;

        //�s��Ҩ��ɤ~�|�Ψ�(�D����)
        string strCourseID = "";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Initiate();

            //�����Ѽ�
            this.getParameter();

            if (Session["PaperID"] != null)
            {
                strPaperID = Session["PaperID"].ToString();
            }

            //strPaperID = "wyt20060510150619";

            //���Title
            string strDivisionName = mySQL.getDivisionName(strGroupDivisionID);
            spanDivisionName.InnerText = strDivisionName;

            //string strGroupName = mySQL.getQuestionGroupName(Session["GroupID"].ToString());
            string strGroupName = mySQL.getQuestionGroupName(strGroupID);
            spanGroupName.InnerText = strGroupName;
            if (Session["fromVRSimulator"] != null)
            {
                if (Session["fromVRSimulator"].ToString().Contains("vr"))
                    SessionQuestionType = "5";
            }
            //��ܦ��էO�U���Ҧ����D
            setupQuestionTable();

            //�إ�Finish button���ƥ�
            btnSubmit.ServerClick += new ImageClickEventHandler(btnSubmit_ServerClick);
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

            //�إ�Table��CSS
            //table.Attributes.Add("Class","header1_table");

            //�̷�QuestionMode�M�w���X���էO��General�άOSpecific���Ҧ�����D
            string strSQL = "";
            if (hiddenQuestionMode.Value == "Specific")
            {
                strSQL = mySQL.getSpecificSelectionQuestion(strPaperID);
            }
            else if (SessionQuestionType == "1")
            {
                //strSQL = mySQL.getGroupSelectionQuestion(Session["GroupID"].ToString());
                strSQL = mySQL.getGroupSelectionQuestion(strGroupID);
            }
            else if (SessionQuestionType == "6")
            {
                //strSQL = mySQL.getGroupSelectionQuestion(Session["GroupID"].ToString());
                strSQL = mySQL.getGroupSelectionWithKeyWordsQuestion(strGroupID);
            }

            /*
             * //alert the QuestionType that is currently stored in the session var,'SessionQuestionType'
            ////use JS alert() in C#
            ScriptManager.RegisterStartupScript(
             this,
             typeof(Page),
             "Alert",
            "<script>alert('SessionQuestionType: " + SessionQuestionType + "');</script>",
             false);
            */
            //Ben test 
            //SessionQuestionType = "2";

            #region �C�X����D
            if (SessionQuestionType == "" || SessionQuestionType == "1")
            {
                DataTable dsQuestionList = new DataTable();

                //�P�_�O�Q�Φ�ؼҦ��j�M�AGroup�������Q�θs�շj�M�A�_�h���S�x�ȷj�M
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    dsQuestionList = sqldb.getDataSet(strSQL).Tables[0];
                }
                else
                {
                    //���o�W�@�ӭ����ҷj�M�����G���
                    dsQuestionList = (DataTable)Session["dtSelectedFeatureItemResult"];
                }
                if (dsQuestionList.Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuestionList.Rows.Count; i++)
                    {
                        //�YcQuestionType������1(���������D)�A�h���L���D��
                        if (dsQuestionList.Rows[i]["cQuestionType"].ToString() != "1")
                            continue;
                        //���oQuestionType
                        string strQuestionType = "1";
                        try
                        {
                            strQuestionType = dsQuestionList.Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //���oQID
                        string strQID = "";
                        try
                        {
                            strQID = dsQuestionList.Rows[i]["cQID"].ToString();
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

                            //���D��CheckBox
                            TableCell tcCheckBox = new TableCell();
                            trQuestion.Cells.Add(tcCheckBox);
                            tcCheckBox.Width = Unit.Pixel(25);

                            if (bAllowSelect == true)
                            {
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
                                //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                                chQuestion.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                            }

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
                                    TableCell tcSelectionNum = new TableCell();
                                    trSelection.Cells.Add(tcSelectionNum);
                                    tcSelectionNum.Text = Convert.ToString((j + 1)) + ".";

                                    //�ﶵ���e
                                    TableCell tcSelection = new TableCell();
                                    trSelection.Cells.Add(tcSelection);
                                    tcSelection.Text = strSelection;

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
                }
                dsQuestionList.Dispose();
            }
            #endregion

            #region �C�X����D�]�t����r
            if (SessionQuestionType == "6")
            {
                DataTable dsQuestionList = new DataTable();

                //�P�_�O�Q�Φ�ؼҦ��j�M�AGroup�������Q�θs�շj�M�A�_�h���S�x�ȷj�M
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    dsQuestionList = sqldb.getDataSet(strSQL).Tables[0];
                }
                else
                {
                    //���o�W�@�ӭ����ҷj�M�����G���
                    dsQuestionList = (DataTable)Session["dtSelectedFeatureItemResult"];
                }
                if (dsQuestionList.Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuestionList.Rows.Count; i++)
                    {
                        //�YcQuestionType������1(���������D�]�t����r)�A�h���L���D��
                        if (dsQuestionList.Rows[i]["cQuestionType"].ToString() != "6")
                            continue;
                        //���oQuestionType
                        string strQuestionType = "6";
                        try
                        {
                            strQuestionType = dsQuestionList.Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //���oQID
                        string strQID = "";
                        try
                        {
                            strQID = dsQuestionList.Rows[i]["cQID"].ToString();
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

                            //���D��CheckBox
                            TableCell tcCheckBox = new TableCell();
                            trQuestion.Cells.Add(tcCheckBox);
                            tcCheckBox.Width = Unit.Pixel(25);

                            if (bAllowSelect == true)
                            {
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
                                //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                                chQuestion.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                            }

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

                            //����r���D
                            TableCell tcKeyWordsTitle = new TableCell();
                            trQuestion.Cells.Add(tcKeyWordsTitle);
                            tcKeyWordsTitle.Text = "KeyWords";
                            tcKeyWordsTitle.Attributes["align"] = "center";
                            tcKeyWordsTitle.Width = Unit.Pixel(300);
                            tcKeyWordsTitle.BorderWidth = Unit.Pixel(2);
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
                                    TableCell tcSelectionNum = new TableCell();
                                    trSelection.Cells.Add(tcSelectionNum);
                                    tcSelectionNum.Text = Convert.ToString((j + 1)) + ".";

                                    //�ﶵ���e
                                    TableCell tcSelection = new TableCell();
                                    trSelection.Cells.Add(tcSelection);
                                    tcSelection.Text = strSelection;


                                    //�b�ﶵ�Ĥ@�C�[�J����r���
                                    if (j == 0)
                                    {
                                        //���o�D������r
                                        string strKeyWords = "";
                                        strKeyWords = dsQuestionList.Rows[i]["cKeyWords"].ToString();
                                        TableCell tcKeyWords = new TableCell();
                                        trSelection.Cells.Add(tcKeyWords);
                                        tcKeyWords.RowSpan = dsSelection.Tables[0].Rows.Count;
                                        tcKeyWords.Text = strKeyWords;
                                        tcKeyWords.Width = Unit.Pixel(300);
                                        tcKeyWords.BorderWidth = Unit.Pixel(2);
                                        tcKeyWords.BorderColor = Color.Black;
                                    }

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
                }
                dsQuestionList.Dispose();
            }
            #endregion

            #region �C�X�ݵ��D
               
           
            else if (SessionQuestionType == "" || SessionQuestionType == "2")
            {





                //�إ��ݩ󦹲էO���ݵ��D
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = mySQL.getGroupTextQuestion(strGroupID);
                }
                else
                {
                    strSQL = mySQL.getFeatureTextQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                    // (DataTable)Session["dtSelectedFeatureItemResult"]
                }

                DataSet dsTextList = sqldb.getDataSet(strSQL);

                int intTextCount = 0;
                if (dsTextList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                    {
                        //���o�����D��QID
                        string strQID = "";

                        /*
                        //Ben ���o�����D��cQuestionType
                        string cQuestionType="";
                        //Ben
                        */

                        try
                        {
                            strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                            /*
                            //Ben ���o�����D��cQuestionType
                            cQuestionType=dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                            //Ben
                            */
                        }
                        catch
                        {
                        }

                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        //CheckBox
                        TableCell tcCheck = new TableCell();
                        trQuestion.Cells.Add(tcCheck);

                        if (bAllowSelect == true)
                        {
                            CheckBox chText = new CheckBox();

                            

                            tcCheck.Controls.Add(chText);
                            string strID = "";
                            strID = "ch-" + strQID;
                            chText.ID = strID;
                            
                            //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                            chText.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                            /*
                            //Ben add checkedChange event to each �ݵ��Dcheckbox
                            chText.AutoPostBack = true;
                            chText.CheckedChanged += new EventHandler(testchbox_CheckedChanged);
                            //Ben
                            */
                            
                        }

                        //Question number
                        intQuestionIndex += 1;
                        TableCell tcTextNum = new TableCell();
                        trQuestion.Cells.Add(tcTextNum);
                        tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //Question
                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }
                        tcQuestion.Text = strQuestion;

                        //�[�JCSS
                        intTextCount += 1;
                        if ((intTextCount % 2) != 0)
                        {
                            trQuestion.Attributes.Add("Class", "header1_table_first_row");
                        }
                        else
                        {
                            trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        }
                    }
                }
                else
                {
                    //���ݨ��S���ݵ��D������
                }
                dsTextList.Dispose();
            }
            #endregion

            #region �C�X�ϧ��D
            else if (SessionQuestionType == "3")
            {
                this.setupSimulatorQuestionTable();
            }
            #endregion

            #region �C�X�����D
            else if (SessionQuestionType == "5")
            {
                this.setupSituationQuestionTable();
            }
            #endregion



            #region �C�X�{���D


            else if (SessionQuestionType == "" || SessionQuestionType == "7")
            {



                clsProgramQuestion clsProgramQuestionObj = new clsProgramQuestion();

                //�إ��ݩ󦹲էO���{���D
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = clsProgramQuestionObj.getAllProgramTypeQuestion(strGroupID);
                }
                else
                {
                    strSQL = clsProgramQuestionObj.getFeatureProgramQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                    // (DataTable)Session["dtSelectedFeatureItemResult"]
                }

                //error occurs here, because QuestionMode is in NewVersionHintsDB, while Program_Question is in CorrectStuHWDB
                //maybe we can move Program_Question and Program_Answer table to NewVersionHintsDB
                SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
                //error occurs here, because QuestionMode is in NewVersionHintsDB, while Program_Question is in CorrectStuHWDB
                //maybe we can move Program_Question and Program_Answer table to NewVersionHintsDB

                DataSet dsProgramList = myDB.getDataSet(strSQL);
               
                int intTextCount = 0;
                if (dsProgramList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsProgramList.Tables[0].Rows.Count; i++)
                    {
                        //���o�����D��QID
                        string strQID = "";

                        /*
                        //Ben ���o�����D��cQuestionType
                        string cQuestionType = "";
                        //Ben
                        */

                        try
                        {
                            strQID = dsProgramList.Tables[0].Rows[i]["cQID"].ToString();

                            /*
                            //Ben ���o�����D��cQuestionType
                            cQuestionType = dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                            //Ben
                            */
                        }
                        catch
                        {
                        }

                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        //CheckBox
                        TableCell tcCheck = new TableCell();
                        trQuestion.Cells.Add(tcCheck);

                        if (bAllowSelect == true)
                        {
                            CheckBox chText = new CheckBox();



                            tcCheck.Controls.Add(chText);
                            string strID = "";
                            strID = "ch-" + strQID;
                            chText.ID = strID;

                            //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                            chText.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                            /*show the similar questions when a question is picked.
                            //Ben add checkedChange event to each �ݵ��Dcheckbox
                            chText.AutoPostBack = true;
                            chText.CheckedChanged += new EventHandler(testchbox_CheckedChanged);
                            //Ben
                            */

                        }

                        //Question number
                        intQuestionIndex += 1;
                        TableCell tcTextNum = new TableCell();
                        trQuestion.Cells.Add(tcTextNum);
                        tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //Question
                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsProgramList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }
                        tcQuestion.Text = strQuestion;

                        //�[�JCSS
                        intTextCount += 1;
                        if ((intTextCount % 2) != 0)
                        {
                            trQuestion.Attributes.Add("Class", "header1_table_first_row");
                        }
                        else
                        {
                            trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        }
                    }
                }
                else
                {
                    //���ݨ��S���ݵ��D������
                }
                dsProgramList.Dispose();
            }
            #endregion




            #region �C�X����D


            else if (SessionQuestionType == "" || SessionQuestionType == "10")
            {





                //�إ��ݩ󦹲էO������D
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = mySQL.getGroupFillOutBlankQuestion(strGroupID);
                }
                else
                {
                    strSQL = mySQL.getFeatureFillOutBlankQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                    // (DataTable)Session["dtSelectedFeatureItemResult"]
                }

                DataSet dsTextList = sqldb.getDataSet(strSQL);

                int intTextCount = 0;
                if (dsTextList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                    {
                        //���o�����D��QID
                        string strQID = "";

                        /*
                        //Ben ���o�����D��cQuestionType
                        string cQuestionType="";
                        //Ben
                        */

                        try
                        {
                            strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                            /*
                            //Ben ���o�����D��cQuestionType
                            cQuestionType=dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                            //Ben
                            */
                        }
                        catch
                        {
                        }

                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        //CheckBox
                        TableCell tcCheck = new TableCell();
                        trQuestion.Cells.Add(tcCheck);

                        if (bAllowSelect == true)
                        {
                            CheckBox chText = new CheckBox();



                            tcCheck.Controls.Add(chText);
                            string strID = "";
                            strID = "ch-" + strQID;
                            chText.ID = strID;

                            //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                            chText.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                            /*
                            //Ben add checkedChange event to each ����Dcheckbox
                            chText.AutoPostBack = true;
                            chText.CheckedChanged += new EventHandler(testchbox_CheckedChanged);
                            //Ben
                            */

                        }

                        //Question number
                        intQuestionIndex += 1;
                        TableCell tcTextNum = new TableCell();
                        trQuestion.Cells.Add(tcTextNum);
                        tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //Question
                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }
                        tcQuestion.Text = strQuestion;

                        //�[�JCSS
                        intTextCount += 1;
                        if ((intTextCount % 2) != 0)
                        {
                            trQuestion.Attributes.Add("Class", "header1_table_first_row");
                        }
                        else
                        {
                            trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        }
                    }
                }
                else
                {
                    //���ݨ��S������D������
                }
                dsTextList.Dispose();
            }
            #endregion
        }







        //Ben add checkedChange event to each �ݵ��Dcheckbox
        protected void testchbox_CheckedChanged(object sender, EventArgs e)
        {
           

            //CheckBox chk = (CheckBox)sender;
            CheckBox chk = sender as CheckBox;
            if (chk.Checked)
            {  
                //get �ݵ��Dsimilar question
                if (SessionQuestionType == "2")
                {
                    //Ben check trim result of the chk ID to get cQID to get its similarID
                    string chkID="";
                   
                    int index = chk.ID.IndexOf("-");
                    if (index > 0)
                        chkID = chk.ID.Substring(index+1);
                    testID.InnerText = chkID;
                    //Ben end check



                    //get �ݵ��Dsimilar question
                    //setupQuesAnsSimilarQuestionList(chk.ID);
                   
                }

                //get ����Dsimilar question
                if (SessionQuestionType == "1")
                {

                    //get  ����Dsimilar question

                }
                
                //show similarQuestionList modal
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
            }

            //else
            //do nothing

        }
        //Ben
        private void setupQuesAnsSimilarQuestionList(string chkID)
        {
            Table table = new Table();
            similarTable.Controls.Add(table);
            table.CellSpacing = 0;
            table.CellPadding = 2;
            table.BorderStyle = BorderStyle.Solid;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderColor = System.Drawing.Color.Black;
            table.GridLines = GridLines.None;
            table.Width = Unit.Percentage(100);

            //remove the the needless title from chk's ID to get the QID
            string QID = "";

            int index = chkID.IndexOf("-");
            if (index > 0)
                QID = chkID.Substring(index + 1);
           
           




             #region �C�X�ݵ��D





                string strSQL = "";

                //�إ��ݩ󦹲էO���ݵ��D
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = mySQL.getGroupTextQuestion(strGroupID);
                }
                else
                {
                    strSQL = mySQL.getFeatureTextQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                    // (DataTable)Session["dtSelectedFeatureItemResult"]
                }

                DataSet dsTextList = sqldb.getDataSet(strSQL);

                int intTextCount = 0;
                if (dsTextList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                    {
                        //���o�����D��QID
                        string strQID = "";

                        //Ben ���o�����D��cQuestionType
                        string cQuestionType="";
                        //Ben

                        try
                        {
                            strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                            
                            //Ben ���o�����D��cQuestionType
                            cQuestionType=dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                            //Ben
                        }
                        catch
                        {
                        }

                        TableRow trQuestion = new TableRow();
                        table.Rows.Add(trQuestion);

                        //CheckBox
                        TableCell tcCheck = new TableCell();
                        trQuestion.Cells.Add(tcCheck);

                        if (bAllowSelect == true)
                        {
                            CheckBox chText = new CheckBox();

                            

                            tcCheck.Controls.Add(chText);
                            string strID = "";
                            strID = "ch-" + strQID;
                            chText.ID = strID;
                            //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                            chText.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);

                             /* show similar question when a question is picked
                            //Ben add checkedChange event to each �ݵ��Dcheckbox
                            chText.AutoPostBack = true;
                            chText.CheckedChanged += new EventHandler(testchbox_CheckedChanged);
                            //Ben
                            */
                            
                        }

                        //Question number
                        intQuestionIndex += 1;
                        TableCell tcTextNum = new TableCell();
                        trQuestion.Cells.Add(tcTextNum);
                        tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                        //Question
                        TableCell tcQuestion = new TableCell();
                        trQuestion.Cells.Add(tcQuestion);
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }
                        tcQuestion.Text = strQuestion;

                        //�[�JCSS
                        intTextCount += 1;
                        if ((intTextCount % 2) != 0)
                        {
                            trQuestion.Attributes.Add("Class", "header1_table_first_row");
                        }
                        else
                        {
                            trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                        }
                    }
                }
                else
                {
                    //���ݨ��S���ݵ��D������
                }
                dsTextList.Dispose();
            
            #endregion
        }


        private void getParameter()
        {
            //GroupID
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

            //UserID
            if (Session["UserID"] != null)
            {
                strUserID = Session["UserID"].ToString();
            }
            //strUserID = "swakevin";

            //CaseID kyhCase200505301448128593750
            if (Session["CaseID"] != null)
            {
                strCaseID = Session["CaseID"].ToString();
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

            //GenerationMethod
            if (Request.QueryString["GenerationMethod"] != null)
            {
                string strGenerationMethod = Request.QueryString["GenerationMethod"].ToString();
                if (strGenerationMethod == "Edit")
                {
                    bAllowSelect = true;
                }
                else
                {
                    bAllowSelect = false;
                }
            }

            //Opener
            if (Session["Opener"] != null)
            {
                hiddenOpener.Value = Session["Opener"].ToString();
            }

            //Setup opener
            if (Session["Opener"] != null)
            {
                Session["Opener"] = "Paper_SelectQuestion";
            }
            else
            {
                Session.Add("Opener", "Paper_SelectQuestion");
            }

            //QuestionMode
            if (Session["QuestionMode"] != null)
            {
                hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
            }

            //QuestionType
            if (Session["QuestionType"] != null)
                SessionQuestionType = Session["QuestionType"].ToString();

            //PreOpener
            if (Session["PreOpener"] != null)
            {
                hiddenPreOpener.Value = Session["PreOpener"].ToString();
            }

            //CourseID
            if (Request.QueryString["cCourseID"] != null)
            {
                strCourseID = Request.QueryString["cCourseID"].ToString();
            }
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

        private void btnSubmit_ServerClick(object sender, ImageClickEventArgs e)
        {
            //���USubmit���s���ƥ�

            //����D�M�ݵ��D�U��F�X�D
            int intSelectionCount = 0;
            int intTextCount = 0;
            int intSimulationCount = 0;
            int intSituationCount = 0;

            //���o���էO������D(DataSet)
            //string strSQL = mySQL.getGroupSelectionQuestion(strGroupID);

            #region �N�ҤĿ諸����D�x�s
            //�̷�QuestionMode�M�w���X���էO��General�άOSpecific���Ҧ�����D
            string strSQL = "";
            if (hiddenQuestionMode.Value == "Specific")
            {
                strSQL = mySQL.getSpecificSelectionQuestion(strPaperID);
            }
            else
            {
                //�YSearchMode��Group�h�Q��Group������j�M�A�_�h�H�W�@�����j�M���G������
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = mySQL.getGroupSelectionQuestion(Session["GroupID"].ToString());
                }
                else
                {
                    strSQL = mySQL.getFeatureSelectionQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                }
            }

            string strQuestionGroupQIDList = "";
            DataSet dsQuestionList = sqldb.getDataSet(strSQL);

            if (dsQuestionList.Tables[0].Rows.Count > 0 && (SessionQuestionType == "" || SessionQuestionType == "1"))
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
                        //���Q�Ŀ�A�N��Ʀs�J��Ʈw���C

                        intSelectionCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsQuestionList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsQuestionList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strQuestion, strSeq);
                        //mySQL.SaveToQuestionContent(strPaperID , strQID , strScore , strQuestionType , strQuestionMode , strSeq);

                        //�ˬd�����D�O�_���D�� �O�h�۰ʷs�W�D���D�ئܸ�ƪ�
                        DataTable dtQuestionGroup = CheckQuestionGroup(strQID);
                        if (dtQuestionGroup.Rows.Count > 0)
                        {
                            strQuestionGroupQIDList += strQID + "|";
                            foreach (DataRow drQuestionGroup in dtQuestionGroup.Rows)
                            {
                                string strSelectionRelatedQID = drQuestionGroup["cQID"].ToString();
                                //Seq
                                string strSelectionRelatedQSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);
                                //QuestionType
                                string strSelectionRelatedQType = "";

                                DataTable dtQuestionMode = Hints.Learning.Question.SQLString.getQuestionClassifyName(strSelectionRelatedQID);
                                strSelectionRelatedQType = dtQuestionMode.Rows[0]["cQuestionType"].ToString();

                                mySQL.SaveToQuestionContent(strPaperID, strSelectionRelatedQID, strScore, strSelectionRelatedQType, "General", strSelectionRelatedQSeq);

                            }
                        }

                    }
                    else
                    {
                        //�ˬd���D�O�_�w�b�D�ո�
                        bool bExistQGQuestion = false;
                        bExistQGQuestion = CheckQuestionExistGroup(strQuestionGroupQIDList, strQID);

                        if (!bExistQGQuestion)
                        {
                            //�N��Ʀ�Paper_Conent�R��
                            mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                        }
                    }
                }
            }
            else
            {
                //���էO�S���D�ت�����
            }
            dsQuestionList.Dispose();
            #endregion

            #region �N�ҤĿ諸����D�]�t����r�x�s
            //�̷�QuestionMode�M�w���X���էO��General�άOSpecific���Ҧ�����D
            if (hiddenQuestionMode.Value == "Specific")
            {
                strSQL = mySQL.getSpecificSelectionQuestion(strPaperID);
            }
            else
            {
                //�YSearchMode��Group�h�Q��Group������j�M�A�_�h�H�W�@�����j�M���G������
                if (Request.QueryString["SearchMode"] == "Group")
                {
                    strSQL = mySQL.getGroupSelectionWithKeyWordsQuestion(Session["GroupID"].ToString());
                }
                else
                {
                    strSQL = mySQL.getFeatureSelectionWithKeyWordsQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                }
            }

            string strQuestionGroupWithKeyWordsQIDList = "";
            DataSet dsQuestionWithKeyWordsList = sqldb.getDataSet(strSQL);

            if (dsQuestionWithKeyWordsList.Tables[0].Rows.Count > 0 && SessionQuestionType == "6")
            {
                for (int i = 0; i < dsQuestionWithKeyWordsList.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQID"].ToString();
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
                        //���Q�Ŀ�A�N��Ʀs�J��Ʈw���C

                        intSelectionCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsQuestionWithKeyWordsList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strQuestion, strSeq);

                        //�ˬd�����D�O�_���D�� �O�h�۰ʷs�W�D���D�ئܸ�ƪ�
                        DataTable dtQuestionGroup = CheckQuestionGroup(strQID);
                        if (dtQuestionGroup.Rows.Count > 0)
                        {
                            strQuestionGroupWithKeyWordsQIDList += strQID + "|";
                            foreach (DataRow drQuestionGroup in dtQuestionGroup.Rows)
                            {
                                string strSelectionRelatedQID = drQuestionGroup["cQID"].ToString();
                                //Seq
                                string strSelectionRelatedQSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);
                                //QuestionType
                                string strSelectionRelatedQType = "";

                                DataTable dtQuestionMode = Hints.Learning.Question.SQLString.getQuestionClassifyName(strSelectionRelatedQID);
                                strSelectionRelatedQType = dtQuestionMode.Rows[0]["cQuestionType"].ToString();

                                mySQL.SaveToQuestionContent(strPaperID, strSelectionRelatedQID, strScore, strSelectionRelatedQType, "General", strSelectionRelatedQSeq);

                            }
                        }

                    }
                    else
                    {
                        //�ˬd���D�O�_�w�b�D�ո�
                        bool bExistQGQuestion = false;
                        bExistQGQuestion = CheckQuestionExistGroup(strQuestionGroupWithKeyWordsQIDList, strQID);

                        if (!bExistQGQuestion)
                        {
                            //�N��Ʀ�Paper_Conent�R��
                            mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                        }
                    }
                }
            }
            else
            {
                //���էO�S���D�ت�����
            }
            dsQuestionWithKeyWordsList.Dispose();
            #endregion

            #region �N�ҤĿ諸�ݵ��D�x�s
            //���o���էO���ݵ��D
            if (Request.QueryString["SearchMode"] == "Group")
            {
                strSQL = mySQL.getGroupTextQuestion(Session["GroupID"].ToString());
            }
            else
            {
                strSQL = mySQL.getFeatureTextQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
            }


            DataSet dsTextList = sqldb.getDataSet(strSQL);

            if (dsTextList.Tables[0].Rows.Count > 0 && (SessionQuestionType == "" || SessionQuestionType == "2"))
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
                        intTextCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsTextList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strQuestion, strSeq);
                    }
                    else
                    {
                        //�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //���էO�S���ݵ��D������
            }
            dsTextList.Dispose();
            #endregion
            #region �N�ҤĿ諸�ϧ��D�x�s
            //���o���էO���ϧ��D
            strSQL = mySQL.getGroupSimulationQuestion(Session["GroupID"].ToString());
            DataSet ds_simulation_List = sqldb.getDataSet(strSQL);
            if (ds_simulation_List.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds_simulation_List.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = ds_simulation_List.Tables[0].Rows[i]["cQID"].ToString();
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
                        intSimulationCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = ds_simulation_List.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = ds_simulation_List.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = ds_simulation_List.Tables[0].Rows[i]["cContent"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        //mySQL.SaveToQuestionContent(strPaperID , strQID , strScore , strQuestionType , strQuestionMode , strQuestion , strSeq);
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strSeq);
                        //�ˬd�����D�O�_���D�� �O�h�۰ʷs�W�D���D�ئܸ�ƪ�
                        DataTable dtQuestionGroup = CheckQuestionGroup(strQID);
                        if (dtQuestionGroup.Rows.Count > 0)
                        {
                            strQuestionGroupQIDList += strQID + "|";
                            foreach (DataRow drQuestionGroup in dtQuestionGroup.Rows)
                            {
                                string strSelectionRelatedQID = drQuestionGroup["cQID"].ToString();
                                //Seq
                                string strSelectionRelatedQSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);
                                //QuestionType
                                string strSelectionRelatedQType = "";

                                DataTable dtQuestionMode = Hints.Learning.Question.SQLString.getQuestionClassifyName(strSelectionRelatedQID);
                                strSelectionRelatedQType = dtQuestionMode.Rows[0]["cQuestionType"].ToString();

                                mySQL.SaveToQuestionContent(strPaperID, strSelectionRelatedQID, strScore, strSelectionRelatedQType, "General", strSelectionRelatedQSeq);

                            }
                        }
                    }
                    else
                    {
                        //�ˬd���D�O�_�w�b�D�ո�
                        bool bExistQGQuestion = false;
                        bExistQGQuestion = CheckQuestionExistGroup(strQuestionGroupQIDList, strQID);

                        if (!bExistQGQuestion)
                        {
                            //�N��Ʀ�Paper_Conent�R��
                            mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                        }
                        ////�N��Ʀ�Paper_Conent�R��
                        // mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            #endregion
            #region �N�ҤĿ諸�����D�x�s
            //���o���էO���ݵ��D
            /*if (Request.QueryString["SearchMode"] == "Group")
            {
                strSQL = mySQL.getGroupTextQuestion(Session["GroupID"].ToString());
            }
            else
            {
                strSQL = mySQL.getFeatureTextQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
            }*/

            strSQL = mySQL.getGroupSituationQuestion(Session["GroupID"].ToString());
            DataSet dsSituationList = sqldb.getDataSet(strSQL);
            if (dsSituationList.Tables[0].Rows.Count > 0 && (SessionQuestionType == "" || SessionQuestionType == "5"))
            {
                for (int i = 0; i < dsSituationList.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    strQID = dsSituationList.Tables[0].Rows[i]["cQID"].ToString();
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
                        intSituationCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsSituationList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsSituationList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsSituationList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        //mySQL.SaveToQuestionContent(strPaperID , strQID , strScore , strQuestionType , strQuestionMode , strQuestion , strSeq);
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strSeq);
                    }
                    else
                    {
                        //�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }

            #endregion


            #region �N�ҤĿ諸�{���D�x�s
            //���o���էO���ݵ��D
            clsProgramQuestion clsProgramQuestionObj = new clsProgramQuestion();

            //�إ��ݩ󦹲էO���ݵ��D
            if (Request.QueryString["SearchMode"] == "Group")
            {
                strSQL = clsProgramQuestionObj.getAllProgramTypeQuestion(strGroupID);
            }
            else
            {
                strSQL = clsProgramQuestionObj.getFeatureProgramQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                // (DataTable)Session["dtSelectedFeatureItemResult"]
            }

            //error occurs here, because QuestionMode is in NewVersionHintsDB, while Program_Question is in CorrectStuHWDB
            //maybe we can move Program_Question and Program_Answer table to NewVersionHintsDB
            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            //error occurs here, because QuestionMode is in NewVersionHintsDB, while Program_Question is in CorrectStuHWDB
            //maybe we can move Program_Question and Program_Answer table to NewVersionHintsDB

            DataSet dsProgramList = myDB.getDataSet(strSQL);

            if (dsProgramList.Tables[0].Rows.Count > 0 && (SessionQuestionType == "" || SessionQuestionType == "7"))
            {
                for (int i = 0; i < dsProgramList.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsProgramList.Tables[0].Rows[i]["cQID"].ToString();
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
                        intTextCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsProgramList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsProgramList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsProgramList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strQuestion, strSeq);
                    }
                    else
                    {
                        //�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //���էO�S���ݵ��D������
            }
            dsProgramList.Dispose();
            #endregion

            #region �N�ҤĿ諸����D�x�s
            //���o���էO������D
           
            if (Request.QueryString["SearchMode"] == "Group")
            {
                strSQL = mySQL.getGroupFillOutBlankQuestion(strGroupID);
            }
            else
            {
                strSQL = mySQL.getFeatureFillOutBlankQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                // (DataTable)Session["dtSelectedFeatureItemResult"]
            }

            dsTextList = sqldb.getDataSet(strSQL);

            if (dsTextList.Tables[0].Rows.Count > 0 && (SessionQuestionType == "" || SessionQuestionType == "10"))
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
                        intTextCount += 1;

                        //Standard score
                        string strScore = "0";

                        //QuestionType
                        string strQuestionType = "";
                        try
                        {
                            strQuestionType = dsTextList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsTextList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //�N���D�ت���Ʀs�J��Ʈw
                        mySQL.SaveToQuestionContent(strPaperID, strQID, strScore, strQuestionType, strQuestionMode, strQuestion, strSeq);
                    }
                    else
                    {
                        //�N��Ʀ�Paper_Conent�R��
                        mySQL.DeleteFromQuestionContent(strPaperID, strQID);
                    }
                }
            }
            else
            {
                //���էO�S������D������
            }
            dsTextList.Dispose();
            #endregion



            #region 20110407 �ȮɳB�z�Ҧ�
            //20110407 �]���{���B�@�Ҧ� �b�s�W�D�خ� �Y�P�@�s�ըS���@���粒�D�� ���D���Ƿ|�L�k�Ӷ��Ǳ�
            //�ҥH��s�W���D�ث� ���s�]�w�Ƨ�
            //�����������R�s��g�{�� 
            //�Y���ݭn���s�Ƨ� �бN���q����
            DataTable dtPaper_Content = new DataTable();
            string strPaper_Content = "SELECT * FROM Paper_Content WHERE cPaperID = '" + strPaperID + "' ORDER BY cQID";
            clsHintsDB HintsDB = new clsHintsDB();
            dtPaper_Content = HintsDB.getDataSet(strPaper_Content).Tables[0];
            if (dtPaper_Content.Rows.Count > 0)
            {
                int iNewSeq = 1;
                foreach (DataRow drPaper_Content in dtPaper_Content.Rows)
                {
                    string strQID = drPaper_Content["cQID"].ToString();
                    strPaper_Content = "UPDATE Paper_Content SET sSeq = '" + iNewSeq + "' WHERE cPaperID = '" + strPaperID + "' AND cQID = '" + strQID + "'";
                    HintsDB.ExecuteNonQuery(strPaper_Content);
                    iNewSeq++;
                }

            }
            #endregion

            //��ܦ@��ܤF�X�D����D�P�X�D�ݵ��D
            /*
			string strScript="<script language='javascript'>\n";
			strScript+="showFinishMsg('" + intSelectionCount.ToString() + "','" + intTextCount.ToString() + "');\n";
			strScript+="</script>\n";
			Page.RegisterStartupScript("showFinishMsg",strScript);
			*/
            if (hiddenPreOpener.Value == "SelectPaperMode")
                Response.Redirect("Paper_MainPage.aspx?Opener=SelectPaperMode&cCaseID=" + strCaseID + "&cSectionName=" + strSectionName + "&cPaperID=" + strPaperID + "&bIsAutoSetScore=true");
            else
                Response.Redirect("Paper_OtherQuestion.aspx?Opener=Paper_SelectQuestion");
        }

        //�ˬd�O�_���D��
        protected DataTable CheckQuestionGroup(string strQID)
        {
            DataTable dtQuestionGroup = new DataTable();
            string strSQL_Paper_QuestionSelectionGroupIDJoinPaper_QuestionSelectionGroupItem = "SELECT dbo.Paper_QuestionSelectionGroupID.cSelectionID, " +
            "dbo.Paper_QuestionSelectionGroupID.cQID AS cQGroupID, dbo.Paper_QuestionSelectionGroupItem.cGroupID, " +
            "dbo.Paper_QuestionSelectionGroupItem.cQID, dbo.Paper_QuestionSelectionGroupItem.cSequence " +
            "FROM dbo.Paper_QuestionSelectionGroupID INNER JOIN " +
            "dbo.Paper_QuestionSelectionGroupItem ON dbo.Paper_QuestionSelectionGroupID.cGroupID = dbo.Paper_QuestionSelectionGroupItem.cGroupID " +
            "WHERE (dbo.Paper_QuestionSelectionGroupID.cQID = '" + strQID + "') " +
            "ORDER BY dbo.Paper_QuestionSelectionGroupID.cSelectionID";
            clsHintsDB HintsDB = new clsHintsDB();
            DataTable dtPaper_QuestionSelectionGroupID = HintsDB.getDataSet(strSQL_Paper_QuestionSelectionGroupIDJoinPaper_QuestionSelectionGroupItem).Tables[0];
            if (dtPaper_QuestionSelectionGroupID.Rows.Count > 0)
            {
                dtQuestionGroup = dtPaper_QuestionSelectionGroupID;
            }
            return dtQuestionGroup;
        }


        //�ˬd�n�R�������DID�O�_�w�O�D�ժ��D��
        protected bool CheckQuestionExistGroup(string strQuestionGroupQIDList, string strQID)
        {
            bool bExistQGQuestion = false;
            string[] strArrayQuestionGroupQID = strQuestionGroupQIDList.Split('|');
            for (int iQGCount = 0; iQGCount < strArrayQuestionGroupQID.Length; iQGCount++)
            {
                if (strArrayQuestionGroupQID[iQGCount] != "")
                {
                    string strSQL_Paper_QuestionSelectionGroupIDJoinPaper_QuestionSelectionGroupItem = "SELECT dbo.Paper_QuestionSelectionGroupID.cSelectionID, " +
                    "dbo.Paper_QuestionSelectionGroupID.cQID AS cQGroupID, dbo.Paper_QuestionSelectionGroupItem.cGroupID, " +
                    "dbo.Paper_QuestionSelectionGroupItem.cQID, dbo.Paper_QuestionSelectionGroupItem.cSequence " +
                    "FROM dbo.Paper_QuestionSelectionGroupID INNER JOIN " +
                    "dbo.Paper_QuestionSelectionGroupItem ON dbo.Paper_QuestionSelectionGroupID.cGroupID = dbo.Paper_QuestionSelectionGroupItem.cGroupID " +
                    "WHERE (dbo.Paper_QuestionSelectionGroupID.cQID = '" + strArrayQuestionGroupQID[iQGCount].ToString() + "') AND " +
                    "(dbo.Paper_QuestionSelectionGroupItem.cQID = '" + strQID + "')" +
                    "ORDER BY dbo.Paper_QuestionSelectionGroupID.cSelectionID";
                    clsHintsDB HintsDB = new clsHintsDB();
                    DataTable dtPaper_QuestionSelectionGroupID = HintsDB.getDataSet(strSQL_Paper_QuestionSelectionGroupIDJoinPaper_QuestionSelectionGroupItem).Tables[0];
                    if (dtPaper_QuestionSelectionGroupID.Rows.Count > 0)
                    {
                        bExistQGQuestion = true;
                    }
                    dtPaper_QuestionSelectionGroupID.Dispose();
                }
            }
            return bExistQGQuestion;
        }
        protected void setupSimulatorQuestionTable()
        {
            tcSimQuestionTable.Controls.Clear();
            Table table = new Table();
            tcSimQuestionTable.Controls.Add(table);
            table.CellSpacing = 0;
            table.CellPadding = 2;
            table.BorderStyle = BorderStyle.Solid;
            table.BorderWidth = Unit.Pixel(1);
            table.BorderColor = System.Drawing.Color.Black;
            table.GridLines = GridLines.Both;
            table.Width = Unit.Percentage(100);

            //�إ�Table��CSS
            table.CssClass = "header1_table";

            //�̷�QuestionMode�M�w���X���էO���ϧ��D
            string strSQL = mySQL.getGroupQuestionSimulator(Session["GroupID"].ToString());


            DataSet dsQuestionList = sqldb.getDataSet(strSQL);
            if (dsQuestionList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                {
                    //���oQuestionType
                    string strQuestionType = "3";
                    strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();

                    //���oQID
                    string strQID = "";
                    strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();

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
                        //CheckBox
                        //TableCell tcCheck = new TableCell();
                        //trQuestion.Cells.Add(tcCheck);

                        if (bAllowSelect == true)
                        {
                            CheckBox chText = new CheckBox();
                            tcQuestionNum.Controls.Add(chText);
                            string strID = "";
                            strID = "ch-" + strQID;
                            chText.ID = strID;
                            //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                            chText.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);
                        }
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
                        if (dsQuestion_sim.Tables[0].Rows.Count > 0)
                        {
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
                            //DataTable dtTemp = sqldb.getDataSet(strSQL).Tables[0];

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
                        }
                    }
                }
            }

        }

        #region �C�X�����D
        //�гy�����D
        protected void setupSituationQuestionTable()
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

            string strSQL;
            /* //�إ��ݩ󦹲էO���ݵ��D
             if (Request.QueryString["SearchMode"] == "Group")
             {
                 strSQL = mySQL.getGroupTextQuestion(strGroupID);
             }
             else
             {
                 strSQL = mySQL.getFeatureTextQuestion((DataTable)Session["dtSelectedFeatureItemResult"]);
                 // (DataTable)Session["dtSelectedFeatureItemResult"]
             }*/

            strSQL = mySQL.getGroupSituationQuestion(strGroupID);
            DataSet dsSituationList = sqldb.getDataSet(strSQL);

            int intSituationCount = 0;
            if (dsSituationList.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsSituationList.Tables[0].Rows.Count; i++)
                {
                    //���o�����D��QID
                    string strQID = "";
                    try
                    {
                        strQID = dsSituationList.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    TableRow trQuestion = new TableRow();
                    table.Rows.Add(trQuestion);

                    //CheckBox
                    TableCell tcCheck = new TableCell();
                    trQuestion.Cells.Add(tcCheck);

                    if (bAllowSelect == true)
                    {
                        CheckBox chSituation = new CheckBox();
                        tcCheck.Controls.Add(chSituation);
                        string strID = "";
                        strID = "ch-" + strQID;
                        chSituation.ID = strID;
                        //�p�G�����D�v�g�s�b��Paper_Content�A�hCheckBox�Q�Ŀ�C
                        chSituation.Checked = myReceiver.checkExistPaperContent(strPaperID, strQID);
                    }

                    //Question number
                    intQuestionIndex += 1;
                    TableCell tcTextNum = new TableCell();
                    trQuestion.Cells.Add(tcTextNum);
                    tcTextNum.Text = "Q" + intQuestionIndex.ToString() + ": ";

                    //Question
                    TableCell tcQuestion = new TableCell();
                    trQuestion.Cells.Add(tcQuestion);
                    string strQuestion = "";
                    try
                    {
                        strQuestion = dsSituationList.Tables[0].Rows[i]["cQuestion"].ToString();
                    }
                    catch
                    {
                    }
                    tcQuestion.Text = strQuestion;

                    //�[�JCSS
                    intSituationCount += 1;
                    if ((intSituationCount % 2) != 0)
                    {
                        trQuestion.Attributes.Add("Class", "header1_table_first_row");
                    }
                    else
                    {
                        trQuestion.Attributes.Add("Class", "header1_tr_even_row");
                    }
                }
            }
            else
            {
                if (Session["fromVRSimulator"] != null)
                    //���ݨ��S���ݵ��D������
                    Response.Redirect("Paper_MainPage.aspx?Opener=Paper_Main&CaseID=" + Session["fromVRSimulator"].ToString().Replace("vr", ""));
            }
            dsSituationList.Dispose();
        }
        #endregion
    }
}
