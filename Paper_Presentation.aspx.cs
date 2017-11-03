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
using Hints.DB;
using Hints.HintsUtility;
using suro.util;

namespace PaperSystem
{
    /// <summary>
    /// Paper_Presentation ���K�n�y�z�C
    /// </summary>
    public partial class Paper_Presentation : AuthoringTool_BasicForm_BasicForm
    {
        string strUserID, strCaseID, strDivisionID, strClinicNum, strSectionName, strPaperID;
        string strBackText, strNextText, strTitleText;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            //�����������\������ �зj��20110324 �ç���Ѩ���
            this.Initiate();

            Ajax.Utility.RegisterTypeForAjax(typeof(Paper_Presentation));

            //�����Ѽ�
            this.getParameter();

            //�إ߼��D�M���s���h��y��
            this.setupText();

            //�إ߿ﶵ�ϰ�
            this.setupPresentationSelection();

            //�O�_�P�ɦs�btableContent�MtableRandom
            bool bExist = true;

            //�إ�Paper_Content
            clsPresentTable myTable = new clsPresentTable();
            Table tableContent = new Table();
            //�NOLD�����D�P�^�X�������� 20110324
            //tableContent = myTable.setupSuggestedQuestionTableByPaperID(strPaperID, strCaseID, Convert.ToInt32(strClinicNum), strSectionName);
            if (tableContent.Rows.Count > 0)
            {
                tdMainTable.Controls.Add(tableContent);
            }
            else
            {
                trMainTable.Style["DISPLAY"] = "none";
                trMainTableHR.Style["DISPLAY"] = "none";
                bExist = false;
            }

            //�إ�Paper_RandomNum
            Table tableRandom = myTable.getListTableFromRandomNum(strPaperID);
            if (tableRandom.Rows.Count > 0)
            {
                tdRandomTable.Controls.Add(tableRandom);
            }
            else
            {
                trRandomTable.Style["DISPLAY"] = "none";
                bExist = false;
            }

            //�p�G��Ӫ��S���P�ɥX�{�A�h���è�Ӫ�椤����<HR>
            if (bExist == false)
            {
                trHR.Style["DISPLAY"] = "none";
            }

            btnNext.ServerClick += new EventHandler(btnNext_ServerClick);

            //�ȮɱNASSIGN QUESTION�\������ �����D�\��γ~20110324
            btnPreAssign.Style["DISPLAY"] = "none";
        }

        /// <summary>
        /// �إ߿ﶵ�ϰ�
        /// </summary>
        private void setupPresentationSelection()
        {
            clsPresentTable myTable = new clsPresentTable();

            Table table = new Table();
            tdSelect.Controls.Add(table);

            //Switch
            myTable.getSwitchSelectionTable(table, strPaperID);

            //Mark
            myTable.getMarkSelectionTable(table, strPaperID);

            //Modify
            myTable.getModifySelectionTable(table, strPaperID);

            //Assign method
            myTable.getAssignSelectionTable(table, strPaperID);

            //Random select
            myTable.getRandomSelectionSelectionTable(table, strPaperID);

            //Test Duration
            myTable.getTestDurationTable(table, strPaperID);

            //��ܥ��T����
            myTable.getCorrectAnswerTable(table, strPaperID);

            //Present method
            myTable.getPresentMethodSelectionTable(table, strPaperID, strCaseID, Convert.ToInt32(strClinicNum), strSectionName);

            //���@����]�w
            myTable.getAnsProcess(table, strPaperID, strCaseID);

            //Table Driven
            //myTable.getTableDriven(table, strPaperID, strCaseID);

            //��ܱ����D�Ҧ��]�w
            myTable.getQuestionSituationMode(table, strPaperID, strCaseID);


            //���ݵ��D�ɧ_��ܾǥͩm�W
            myTable.getStudentNameVisibleMode(table, strPaperID, strCaseID);

            //�]�wCSS
            TableAttribute.setupTopHeaderTableStyle(table, "TableName", 1, "70%", 5, 0, GridLines.Both, false);
        }

        private void setupText()
        {
            strTitleText = "Please setup the presentation for this examination";
            strBackText = "<< Back";
            strNextText = "Finish and save";

            tdTitle.InnerText = strTitleText;
            btnBack.Value = strBackText;
            btnNext.Value = strNextText;
        }

        private void getParameter()
        {
            //PaperID
            if (Session["PaperID"] != null)
            {
                strPaperID = Session["PaperID"].ToString();
                hfPaperID.Value = strPaperID;
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
                hfCaseID.Value = strCaseID;
            }


            //Division 9801
            if (Session["DivisionID"] != null)
            {
                strDivisionID = Session["DivisionID"].ToString();
                hfDivisionID.Value = strDivisionID;
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

            //PreOpener
            if (Session["PreOpener"] != null)
            {
                hiddenPreOpener.Value = Session["PreOpener"].ToString();
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

        private void btnNext_ServerClick(object sender, EventArgs e)
        {
            //�x�s���ݨ���Presentation���

            //�x�sPaper_Header
            this.savePresentationIntoPaperHeader(this, strPaperID, strCaseID, Convert.ToInt32(strClinicNum), strSectionName);

            //�x�sPaper_NextStepBySelectionID
            this.saveNextStepByPaperID(this, strPaperID);
        }

        /// <summary>
        /// �x�s�e�{�]�w��paper_Header
        /// </summary>
        /// <param name="WebPage"></param>
        /// <param name="strPaperID"></param>
        private void savePresentationIntoPaperHeader(System.Web.UI.Page WebPage, string strPaperID, string strCaseID, int intClinicNum, string strSectionName)
        {
            //Switch
            this.saveSwitchPresentation(this, strPaperID);

            //Mark
            this.saveMarkPresentation(this, strPaperID);

            //Modify
            this.saveModifyPresentation(this, strPaperID);

            //Present
            this.savePresentPresentation(this, strPaperID, strCaseID, intClinicNum, strSectionName);

            //Assign
            this.saveAssignPresentation(this, strPaperID);

            //Random selection
            this.saveRandomSelection(this, strPaperID);

            //Test duration
            this.saveTestDurationPresentation(this, strPaperID);

            //Correct Answer
            this.saveCorrectAnswerPresentation(this, strPaperID);

            //SituationQuestionMode
            this.saveSituationQuestionModePresentation(this, strPaperID);

            //StudentNameVisibleMode
            this.saveStudentNameVisibleModePresentation(this, strPaperID);
        }

        /// <summary>
        /// �x�sSwitch���]�w
        /// </summary>
        public void saveSwitchPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Switch-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bSwitch = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sMark���]�w
        /// </summary>
        public void saveMarkPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Mark-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bMarkable = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sSectionSummaryShowCorrectAnswer���]�w
        /// </summary>
        /// <param name="WebPage"></param>
        /// <param name="strPaperID"></param>
        public void saveCorrectAnswerPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-CorrectAnswer-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bSectionSummaryShowCorrectAnswer = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sSituationQuestionMode���]�w
        /// </summary>
        public void saveSituationQuestionModePresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbExam = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-SituationMode-Exam")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbExam.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bIsSituationExam = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sStudentNameVisibleMode���]�w
        /// </summary>
        public void saveStudentNameVisibleModePresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbVisible = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Visible")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbVisible.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bIsStudentNameVisible = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sModify���]�w
        /// </summary>
        public void saveModifyPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Modify-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bModify = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sAssign���]�w
        /// </summary>
        public void saveAssignPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Assign-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 2;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET sAssignMethod = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sRandomSelection���]�w
        /// </summary>
        public void saveRandomSelection(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-RandomSelection-Yes")));

            //���\��O�_���Q�}��
            int intSelect = 1;

            if (rbYes.Checked)
            {
                intSelect = 1;
            }
            else
            {
                intSelect = 0;
            }

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET bRandomSelection = '" + intSelect.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sTestDuration���]�w
        /// </summary>
        public void saveTestDurationPresentation(System.Web.UI.Page WebPage, string strPaperID)
        {
            RadioButton rbYes = ((RadioButton)(WebPage.FindControl("Form1").FindControl("rb-Duration-Yes")));

            //TestDuration���ɶ�
            int intDuration = 0;

            DropDownList dlDuration = new DropDownList();
            if (rbYes.Checked)
            {
                //���o�ɶ�
                dlDuration = ((DropDownList)(WebPage.FindControl("Form1").FindControl("dlTestDuration")));

                intDuration = Convert.ToInt32(dlDuration.SelectedItem.Value);
            }
            else
            {
                intDuration = 0;
            }

            //			if(intDuration > 0)
            //			{
            //				dlDuration.Attributes["disabled"] = "";
            //			}
            //			else
            //			{
            //				dlDuration.Attributes["disabled"] = "disabled";
            //			}

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE Paper_Header SET sTestDuration = '" + intDuration.ToString() + "' WHERE cPaperID = '" + strPaperID + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�sPresentMethod���]�w
        /// </summary>
        /// <param name="WebPage"></param>
        /// <param name="strPaperID"></param>
        public void savePresentPresentation(System.Web.UI.Page WebPage, string strPaperID, string strCaseID, int intClinicNum, string strSectionName)
        {
            DropDownList dlPresent = ((DropDownList)(WebPage.FindControl("Form1").FindControl("dlPresent")));

            string strSelectWorkType = dlPresent.SelectedItem.Value;

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            string strSQL = "UPDATE UserLevelPresent SET sWorkType = '" + strSelectWorkType + "' WHERE cCaseID = '" + strCaseID + "' AND sClinicNum = '" + intClinicNum.ToString() + "' AND cSectionName = '" + strSectionName + "' ";
            myDB.ExecuteNonQuery(strSQL);
        }

        /// <summary>
        /// �x�s��Ʀ�Paper_NextStepBySelectionID
        /// </summary>
        public void saveNextStepByPaperID(System.Web.UI.Page WebPage, string strPaperID)
        {
            DataReceiver myReceiver = new DataReceiver();

            //���o���ݨ����Ҧ��D��(Paper_Content)
            int intContentQuestionCount = myReceiver.getPaperContentQuestionNum(strPaperID);

            //�ثe��ܪ��D��
            int intQuestionIndex = 1;

            //���o���ݨ������Ҧ����D
            for (int i = 1; i <= intContentQuestionCount; i++)
            {
                //���o���D�����(From Paper_Content)
                DataRow[] drQuestion = myReceiver.getQIDFromPaper_ContentByQuestionIndex(strPaperID, intQuestionIndex);

                if (drQuestion.Length > 0)
                {
                    //���o�n��ܰ��D��QID
                    string strQID = drQuestion[0]["cQID"].ToString();

                    //QuestionType
                    string strQuestionType = drQuestion[0]["cQuestionType"].ToString();

                    //QuestionMode
                    string strQuestionMode = drQuestion[0]["cQuestionMode"].ToString();

                    //���D�O����D�٬O�ݵ��D?
                    switch (drQuestion[0]["cQuestionType"].ToString())
                    {
                        case "2":
                            //�ݵ��D

                            break;
                        default:
                            //����D
                            //�NOLD�����D�P�^�X��������20110324
                            //this.saveNextStepBySelectionID(WebPage, strPaperID, strQID, intQuestionIndex);
                            break;
                    }
                }

                intQuestionIndex += 1;
            }

            //����x�s���\�T���A�åB�^��MainPage
            string strScript = "<script language='javascript'>\n";
            strScript += "goNext();\n";
            strScript += "</script>\n";
            Page.RegisterStartupScript("goNext", strScript);
        }

        /// <summary>
        /// �x�s�@���D�ؤU�Ҧ��ﶵ����Ʀ�Paper_NextStepBySelectionID
        /// </summary>
        /// <param name="WebPage"></param>
        /// <param name="strPaperID"></param>
        /// <param name="strQID"></param>
        /// <param name="strSelectionID"></param>
        /// <param name="intQuestionIndex"></param>
        public void saveNextStepBySelectionID(System.Web.UI.Page WebPage, string strPaperID, string strQID, int intQuestionIndex)
        {
            //DataReceiver myReceiver = new DataReceiver();

            //�ﶵ
            SQLString mySQL = new SQLString();
            string strSQL = mySQL.getAllSelections(strQID);

            SqlDB myDB = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
            DataSet dsSelection = myDB.getDataSet(strSQL);
            if (dsSelection.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsSelection.Tables[0].Rows.Count; i++)
                {
                    string strSelectionID = dsSelection.Tables[0].Rows[i]["cSelectionID"].ToString();
                    int intSelectionIndex = Convert.ToInt32(dsSelection.Tables[0].Rows[i]["sSeq"]);
                    string strSelection = dsSelection.Tables[0].Rows[i]["cSelection"].ToString();
                    bool bSuggested = Convert.ToBoolean(dsSelection.Tables[0].Rows[i]["bCaseSelect"]);

                    //���o�D���U�Ԧ����Q��������e
                    string strDlIndexID = "dlSelection-Index-" + strSelectionID;
                    DropDownList dlIndex = ((DropDownList)(WebPage.FindControl("Form1").FindControl(strDlIndexID)));
                    int intNextIndex = Convert.ToInt32(dlIndex.SelectedItem.Value);

                    //���oSection�U�Ԧ����Q��������e
                    string strDlSectionID = "dlSelection-Section-" + strSelectionID;
                    DropDownList dlSection = ((DropDownList)(WebPage.FindControl("Form1").FindControl(strDlSectionID)));
                    string strNextSection = "";
                    if (dlSection.SelectedItem != null)
                        strNextSection = dlSection.SelectedItem.Value;

                    //���ﶵ��NextStep�OBy Section�٬O�D��
                    int intNextMethod = 2;

                    //�ˬd���ﶵ��NextStep�OSection�٬O�D��
                    string strRbIndexID = "rbIndex-" + strSelectionID;
                    RadioButton rbIndex = ((RadioButton)(WebPage.FindControl("Form1").FindControl(strRbIndexID)));

                    string strRbSectionID = "rbSection-" + strSelectionID;
                    RadioButton rbSection = ((RadioButton)(WebPage.FindControl("Form1").FindControl(strRbSectionID)));

                    string strRbDefaultID = "rbDefault-" + strSelectionID;
                    RadioButton rbDefault = ((RadioButton)(WebPage.FindControl("Form1").FindControl(strRbDefaultID)));

                    if (rbIndex.Checked == true)
                    {
                        if (intNextIndex == 0)
                        {
                            //����Section
                            intNextMethod = 0;
                        }
                        else
                        {
                            //�D��
                            intNextMethod = 2;
                        }
                    }
                    else if (rbSection.Checked == true)
                    {
                        //Section
                        intNextMethod = 1;
                    }
                    else
                    {
                        //Default
                        intNextMethod = 9999;
                        intNextIndex = 9999;
                    }

                    //���Ʀs�JPaper_NextStepBySelectionID
                    SQLString.SaveToPaper_NextStepBySelectionID(strPaperID, strQID, intQuestionIndex, strSelectionID, intSelectionIndex, intNextMethod, strNextSection, intNextIndex);
                }
            }
            dsSelection.Dispose();


        }

        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public void DeleteTDTemplate(string strPaperID, string strCaseID)
        {
            Hints.DB.TableDriven.TD_QandA.TD_QandA_DELETE(strPaperID, strCaseID);
        }

        /// <summary>
        /// ���oAnsProcess_Paper��AnsProcessID
        /// �Y�L�h�s�W�@��
        /// </summary>
        /// <param name="strPaperID"></param>
        /// <param name="strCaseID"></param>
        [Ajax.AjaxMethod(Ajax.HttpSessionStateRequirement.ReadWrite)]
        public string GetPaperAnsProcessID(string strPaperID, string strCaseID)
        {
            string strAnsProcessID = "";
            string strSQL = "";
            clsHintsDB HintsDB = new clsHintsDB();
            DataTable dt = new DataTable();
            strSQL = "SELECT * FROM AnsProcess_Paper WHERE cCaseID = '" + strCaseID + "' AND cPaperID = '" + strPaperID + "'";
            dt = HintsDB.getDataSet(strSQL).Tables[0];
            if (dt.Rows.Count > 0)
            {
                strAnsProcessID = dt.Rows[0]["cAnsProcessID"].ToString();
            }
            else
            {
                strAnsProcessID = "AnsProcess" + DateTime.Now.ToString("yyyyMMddHHmmss");
                strSQL = "INSERT INTO AnsProcess_Paper (cAnsProcessID, cCaseID, cPaperID) " +
                    " VALUES ('" + strAnsProcessID + "', '" + strCaseID + "', '" + strPaperID + "')";
                HintsDB.ExecuteNonQuery(strSQL);
            }

            return strAnsProcessID;

            dt.Dispose(); 
        }
    }
}
