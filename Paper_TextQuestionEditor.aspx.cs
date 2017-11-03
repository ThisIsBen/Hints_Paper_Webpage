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
using Hints.DB.Section;
using AuthoringTool.CommonQuestionEdit;

namespace PaperSystem
{
    /// <summary>
    /// �Ψӽs��ݵ��D���u��
    /// </summary>
    public partial class Paper_TextQuestionEditor : AuthoringTool_BasicForm_BasicForm
    {
        protected string title_Bold = "Bold";
        protected string title_Italic = "Italic";
        protected string title_Underline = "Underline";
        protected string title_Strikethrough = "Strikethrough";
        protected string title_Subscript = "Subscript";
        protected string title_Superscript = "Superscript";
        protected string title_JustifyLeft = "Justify Left";
        protected string title_JustifyCenter = "Justify Center";
        protected string title_JustifyRight = "Justify Right";
        protected string title_OrderedList = "Ordered List";
        protected string title_BulletedList = "Bulleted List";
        protected string title_DecreaseIndent = "Decrease Indent";
        protected string title_IncreaseIndent = "Increase Indent";
        protected string title_FontColor = "Font Color";
        protected string title_BackgroundColor = "Background Color";
        protected string title_HorizontalRule = "Horizontal Rule";
        protected string title_UploadFile = "Upload File";
        protected string title_InsertWebLink = "Insert Web Link";
        protected string title_InsertImage = "Insert Image";
        protected string title_InsertTable = "Insert Table";
        protected string title_ViewHTMLSource = "View HTML Source";
        protected string title_EnlargeEditor = "Enlarge Editor";
        protected string title_Aboutthiseditor = "About this editor";

        protected string btnFinish = "";
        protected string strClientScript = "";

        bool bModify = false;

        string strUserID, strCaseID, strDivisionID, strClinicNum, strSectionName, strPaperID, strQID;
        string strGroupID = "";
        string strGroupDivisionID = "";
        protected System.Web.UI.WebControls.Table LayoutTableForFeature = new Table();
        SQLString mySQL = new SQLString();

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Initiate();

            //�����Ѽ�
            this.getParametor();

            //�[�JbtnSaveNextQuestion���ƥ�
            btnSaveNextQuestion.ServerClick += new EventHandler(btnSaveNextQuestion_ServerClick);

            //�[�JbtnSaveNext���ƥ�
            btnSaveNext.ServerClick += new EventHandler(btnSaveNext_ServerClick);

            //��l��Layout
            InititateLayoutTableForFeature();
            //���g �W�[�s��S�x���ݩʪ��
            this.FindControl("PanelFeature").Controls.Add(LayoutTableForFeature);

            string strGetKeyword = Hints.Learning.Question.DataReceiver.getTextQuestionKeyword(strQID);
            if (strGetKeyword != "N/A")
            {
                lbKeyword.Text = strGetKeyword;
                btnEditKeyword.CssClass = "button_press_enable";
                //btnEditKeyword.Text = "Edit the Keyword";
            }

            #region �]�w���DLevel�P�f�x���U�Կ�涵��
            DataTable dtQuestionLevel = new DataTable();
            dtQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName();
            foreach (DataRow drQuestionLevel in dtQuestionLevel.Rows)
            {
                ddlQuestionLevel.Items.Add(drQuestionLevel["cLevelName"].ToString());
            }

            //�إ߯f�x���U�Կ�涵��
            DataTable dtDiseaseSymptomsTree = DiseaseSymptomsTree_SELECT();
            ddlSymptoms.Items.Add("All");
            foreach (DataRow drDiseaseSymptomsTree in dtDiseaseSymptomsTree.Rows)
            {
                ddlSymptoms.Items.Add(drDiseaseSymptomsTree["cNodeName"].ToString());
            }
            #endregion

            #region �]�w���DLevel�P�f�x���U�Կ���ܶ���
            if (!IsPostBack)
            {
                int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelValue(strQID);
                if (iQuestionLevel != -1)
                    ddlQuestionLevel.SelectedValue = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_LevelName(iQuestionLevel);
                string strQuestionSymptoms = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_SELECT_QuestionSymptoms(strQID);
                if (strQuestionSymptoms != "-1")
                    ddlSymptoms.SelectedValue = strQuestionSymptoms;
            }
            #endregion


            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            BulidInterrogation("Question");
            BulidInterrogation("Answer");
        }

        /// <summary>
        /// �����Ѽ�
        /// </summary>
        private void getParametor()
        {
            //UserID
            if (Session["UserID"] != null)
            {
                strUserID = Session["UserID"].ToString();
            }
            //strUserID = "swakevin";

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
            //PaperID
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
            }
            //strPaperID = "wyt20060510150619";

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
            //hiddenQuestionMode.Value = "General";

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

            //bModify
            if (Session["bModify"] != null)
            {
                bModify = Convert.ToBoolean(Session["bModify"]);
            }

            //QID
            if (Request.QueryString["QID"] != null)
            {
                strQID = Request.QueryString["QID"].ToString();

                //��QID���D�ؤ��e�g�JtxtData , txtEdit��
                string strQuestionAnswer = DataReceiver.getTextQuestionContentByQID(strQID);
                if (this.IsPostBack == false)
                {
                    txtQuestionData.Text = strQuestionAnswer.Split('$')[0];
                    txtAnswerData.Text = strQuestionAnswer.Split('$')[1];
                }
            }
            else
            {
                //�إ�QID
                DataReceiver myReceiver = new DataReceiver();
                strQID = strUserID + "_" + myReceiver.getNowTime();
            }

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
        }

        private void InititateLayoutTableForFeature()
        {
            LayoutTableForFeature.ID = "LayoutTableForFeature";
            //�Ĥ@�C
            LayoutTableForFeature.Width = Unit.Parse("100%");
            //LayoutTable.BorderWidth = Unit.Parse("3px");
            //LayoutTable.BorderColor = Color.Green;
            LayoutTableForFeature.ID = "LayoutTableForFeature_1";
            LayoutTableForFeature.Rows.Add(new TableRow());
            LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells.Add(new TableCell());
            Label FeatureTitle = new Label();
            FeatureTitle.Text = "SetFeature";

            LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells[0].Controls.Add(FeatureTitle);
            LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells[0].CssClass = "title";

            //�ĤG�C
            LayoutTableForFeature.Rows.Add(new TableRow());
            LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells.Add(new TableCell());
            Table containerForFeature = CommonQuestionUtility.get_HTMLTable(1, 2);

            LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells[0].Controls.Add(containerForFeature);

            FeatureItemControl qFeatureItemControlTable = new FeatureItemControl(this, 1, strGroupID, strQID); //FeatureItemControlTable  
            qFeatureItemControlTable.ID = "FeatureItemControlTable";
            this.LayoutTableForFeature.Rows.Add(new TableRow());
            this.LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells.Add(new TableCell());
            this.LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells[0].Controls.Add(qFeatureItemControlTable);

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

            //�x�s�D��
            clsTextQuestion myText = new clsTextQuestion();
            string strTextQContent = txtQuestionData.Text;
            string strTextAContent = txtAnswerData.Text;
            strTextQContent = strTextQContent.Replace("&lt;", "<");
            strTextQContent = strTextQContent.Replace("&gt;", ">");
            strTextAContent = strTextAContent.Replace("&lt;", "<");
            strTextAContent = strTextAContent.Replace("&gt;", ">");
            myText.saveTextQuestion(strQID, strTextQContent, strTextAContent, strUserID, strPaperID, strGroupDivisionID, strGroupID, hiddenQuestionMode.Value);

            //�x�s���D������
            int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(ddlQuestionLevel.SelectedValue);
            AuthoringTool.QuestionEditLevel.QuestionLevel.INSERT_QuestionLevel(strQID, iQuestionLevel);

            //�x�s���D���f�x
            AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_INSERT_QuestionSymptoms(strQID, ddlSymptoms.SelectedValue);

            //�p�G�OSpecific�D�ثh���x�s�@����Ʀ�Paper_Content
            if (hiddenQuestionMode.Value == "Specific")
            {
                int intContentSeq = myReceiver.getPaperContentMaxSeq(strPaperID) + 1;
                SQLString mySQL = new SQLString();
                mySQL.SaveToQuestionContent(strPaperID, strQID, "0", "2", hiddenQuestionMode.Value, intContentSeq.ToString());
            }

            //Redirect�ܦ�����
            //Response.Redirect("Paper_TextQuestionEditor.aspx?Opener=" + hiddenOpener.Value);

            //�إ�QID
            strQID = strUserID + "_" + myReceiver.getNowTime();

            //�M��TextArea
            txtQuestionData.Text = "";
            txtAnswerData.Text = "";
            string strScript = "<script language='javascript'>\n";
            strScript += "Clear()\n";
            strScript += "</script>\n";
            Page.RegisterStartupScript("Clear", strScript);

            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            BulidInterrogation("Question");
            BulidInterrogation("Answer");
        }

        /// <summary>
        /// Save text question
        /// </summary>
        private void SaveQuestionText()
        {
            //�x�s�D��
            clsTextQuestion myText = new clsTextQuestion();
            string strQTextContent = txtQuestionData.Text;
            string strATextContent = txtAnswerData.Text;
            strQTextContent = strQTextContent.Replace("&lt;", "<");
            strQTextContent = strQTextContent.Replace("&gt;", ">");
            strATextContent = strATextContent.Replace("&lt;", "<");
            strATextContent = strATextContent.Replace("&gt;", ">");

            myText.saveTextQuestion(strQID, strQTextContent, strATextContent, strUserID, strPaperID, strGroupDivisionID, strGroupID, hiddenQuestionMode.Value);

            //�x�s���D������
            int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(ddlQuestionLevel.SelectedValue);
            AuthoringTool.QuestionEditLevel.QuestionLevel.INSERT_QuestionLevel(strQID, iQuestionLevel);

            //�x�s���D���f�x
            AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_INSERT_QuestionSymptoms(strQID, ddlSymptoms.SelectedValue);

            //�p�G�OSpecific�D�ثh���x�s�@����Ʀ�Paper_Content
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
            SaveQuestionText();

            //���g 2012/11/25 �N�ϥΪ̩ҿ�ܪ��S�x�Ȧs�J�Ȧs�}�C���A���x�s���Ʈw���C
            clsFeaturevalue clsSaveFeature = new clsFeaturevalue();
            clsSaveFeature.update_FeatureItemIntoDataBase(clsSaveFeature.get_dtFeatureItem_Data(strGroupID, this));

            //Redirect�ܤU�@�Ӻ���
            string strSystemFunction = "";
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
                        Response.Redirect("Paper_QuestionView.aspx?Opener=Paper_TextQuestionEditor");
                    }
                    else
                    {
                        Response.Redirect("Paper_QuestionMain.aspx?Opener=Paper_TextQuestionEditor");
                    }
                    break;
                case "PreviewPaper":
                    Response.Redirect("Paper_MainPage.aspx?Opener=Paper_TextQuestionEditor");
                    break;
                default:
                    Response.Redirect("Paper_QuestionMain.aspx?Opener=Paper_TextQuestionEditor");
                    break;
            }

            //			if(hiddenQuestionMode.Value == "Specific")
            //			{
            //				//Paper_OtherQuestion.aspx
            //				Response.Redirect("Paper_OtherQuestion.aspx?Opener=Paper_TextQuestionEditor");
            //			}
            //			else
            //			{
            //				if(hiddenOpener.Value == "Paper_QuestionView")
            //				{
            //					Response.Redirect("Paper_QuestionMain.aspx?Opener=Paper_TextQuestionEditor");
            //				}
            //				else
            //				{
            //					Response.Redirect("./QuestionGroupTree/QuestionGroupTree.aspx?Opener=Paper_QuestionType");
            //				}
            //			}
        }

        protected void btnEditKeyword_Click(object sender, EventArgs e)
        {
            SaveQuestionText();
            Response.Redirect("~/AuthoringTool/CaseEditor/Paper/Paper_EditQuestionKeyword.aspx?QID=" + strQID + "&GroupID=" + strGroupID + "&CaseID=" + usi.CaseID + "&ClinicNum=" + usi.ClinicNum + "&Section=" + usi.Section + " ");
        }

        protected void btAddSynQuestion_Click(object sender, EventArgs e)
        {
            SaveQuestionText();
            Response.Redirect("../Interrogation/EditAddAskQuestionAnswer.aspx?Mode=AddQuestion&RecentItemID=" + strQID + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "");
        }

        protected void btAddSynAnswer_Click(object sender, EventArgs e)
        {
            SaveQuestionText();
            Response.Redirect("../Interrogation/EditAddAskQuestionAnswer.aspx?Mode=AddAnswer&RecentItemID=" + strQID + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "");
        }

        //�إߦP�q���D�ε���
        private void BulidInterrogation(string strType)
        {
            DataTable dtSyn = new DataTable();
            if (strType == "Question")
            {
                tbInterrogationClassQutstion.Controls.Clear();
                tbInterrogationClassQutstion.Style.Add("background-color", "#F8F0E7");
                dtSyn = clsInterrogationEnquiry.GetSynQuestion(strQID);
            }
            else if (strType == "Answer")
            {
                tbInterrogationClassAnswer.Controls.Clear();
                tbInterrogationClassAnswer.Style.Add("background-color", "#F8F0E7");
                dtSyn = clsInterrogationEnquiry.GetSynAnswer(strQID);
            }

            if (dtSyn.Rows.Count > 0)
            {
                #region �P�q���D�ε��ת����D
                TableRow trSynT = new TableRow();
                trSynT.Attributes.Add("style", "border:0px black solid;background:#7BC2FA; font-weight:bold;color:black");
                trSynT.Style.Add("CURSOR", "hand");
                trSynT.Style.Add("TEXT-ALIGN", "left");

                TableCell tcSynT = new TableCell();
                trSynT.Cells.Add(tcSynT);

                tcSynT.Text = "<IMG id='img" + strType + "_" + strQID + "' src='../../../BasicForm/Image/minus.gif'>&nbsp;Synonymous " + strType;

                TableRow trSynTable = new TableRow();
                trSynTable.ID = "tr" + strType + "" + strQID;
                trSynT.Attributes.Add("onclick", "ShowDetail('" + trSynTable.ID + "' , 'img" + strType + "_" + strQID + "')");

                if (strType == "Question")
                {
                    tbInterrogationClassQutstion.Rows.Add(trSynT);
                    tbInterrogationClassQutstion.Rows.Add(trSynTable);
                    hrQuestion.Style.Add("display", "");
                }
                else if (strType == "Answer")
                {
                    tbInterrogationClassAnswer.Rows.Add(trSynT);
                    tbInterrogationClassAnswer.Rows.Add(trSynTable);
                    hrAnswer.Style.Add("display", "");
                }


                TableCell tcSynTable = new TableCell();
                trSynTable.Cells.Add(tcSynTable);

                Table tbSynTable = new Table();
                tcSynTable.Controls.Add(tbSynTable);
                tbSynTable.Width = Unit.Percentage(100);
                tbSynTable.GridLines = GridLines.Both;

                #endregion

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
                        TableRow trSynItemT = new TableRow();
                        trSynItemT.CssClass = "header1_tr_even_row";
                        tbSynTable.Rows.Add(trSynItemT);

                        TableCell tcSynItemT = new TableCell();
                        trSynItemT.Cells.Add(tcSynItemT);
                        tcSynItemT.Text = "Syn." + (iSyn + 1) + ":";
                        tcSynItemT.Width = Unit.Percentage(8);

                        string strSynItemValue = dtSynonymousItem.Rows[iSyn]["cItemValue"].ToString();
                        TableCell tcSynContext = new TableCell();
                        trSynItemT.Cells.Add(tcSynContext);
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
                                tcSynContext.Controls.Add(lbFlash);
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
                                tcSynContext.Controls.Add(lbMedia);
                            }
                            else if (strTemp == "jpg" || strTemp == "bmp" || strTemp == "gif" || strTemp == "png")
                            {
                                System.Web.UI.WebControls.Image imgPic = new System.Web.UI.WebControls.Image();
                                imgPic.ImageUrl = "../../MultiMediaDB/Upload/Image/" + strFileName + "";
                                imgPic.Width = Unit.Pixel(320);
                                imgPic.Height = Unit.Pixel(240);
                                tcSynContext.Controls.Add(imgPic);
                            }
                        }
                        else
                        {
                            tcSynContext.Text = strSynItemValue;
                        }

                        tcSynContext.Width = Unit.Percentage(82);
                        #endregion

                        #region �P�q���ت��s��
                        TableCell tcSynItemModify = new TableCell();
                        trSynItemT.Cells.Add(tcSynItemModify);
                        tcSynItemModify.Style.Add("text-align", "center");
                        tcSynItemModify.Width = Unit.Percentage(5);

                        Button btModifySynItem = new Button();
                        btModifySynItem.Click += new EventHandler(btModifySynItem_Click);
                        btModifySynItem.CssClass = "button_Edit";
                        btModifySynItem.CommandArgument = "btnModify$" + strMode + "$" + strQID + "$" + dtSynonymousItem.Rows[iSyn][strDataName].ToString();
                        tcSynItemModify.Controls.Add(btModifySynItem);


                        TableCell tcSynItemDelete = new TableCell();
                        trSynItemT.Cells.Add(tcSynItemDelete);
                        tcSynItemDelete.Style.Add("text-align", "center");
                        tcSynItemDelete.Width = Unit.Percentage(5);

                        Button btnDeleteSyn = new Button();
                        btnDeleteSyn.ID = "btnDelete|" + dtSynonymousItem.Rows[iSyn]["sSeq"].ToString() + "|" + strQID;
                        btnDeleteSyn.CssClass = "button_Delete";
                        btnDeleteSyn.Click += new EventHandler(btnDeleteSyn_Click);
                        btnDeleteSyn.CommandArgument = "btnDelete$" + strMode + "$" + strQID + "$" + dtSynonymousItem.Rows[iSyn][strDataName].ToString();
                        tcSynItemDelete.Controls.Add(btnDeleteSyn);
                        #endregion
                    }
                }
                #endregion
            }
        }

        //�R������
        void btnDeleteSyn_Click(object sender, EventArgs e)
        {
            string strTempItem = "";
            string strTempDataKind = "";
            string strTempMode = "";
            string strTempTarget = "";
            Button btnDelete = new Button();
            btnDelete = (Button)(sender);

            strTempMode = btnDelete.CommandArgument.Split('$')[0];//mode
            strTempTarget = btnDelete.CommandArgument.Split('$')[1];//Syn Question or syn Answer 
            strTempItem = btnDelete.CommandArgument.Split('$')[2];//ItemID
            strTempDataKind = btnDelete.CommandArgument.Split('$')[3];//Datakind

            clsInterrogationEnquiry.DeleteSynonymousItem(strTempTarget, strTempItem, strTempDataKind);

            hrQuestion.Style.Add("display", "none");
            hrAnswer.Style.Add("display", "none");
            BulidInterrogation("Question");
            BulidInterrogation("Answer");
        }

        //�קﶵ��
        void btModifySynItem_Click(object sender, EventArgs e)
        {
            string strArgument = "";
            string strTempItem = "";
            string strTempDataKind = "";
            string strTempMode = "";
            string strTempTarget = "";
            Button btnModify = new Button();
            btnModify = (Button)(sender);

            strArgument = btnModify.CommandArgument.ToString();
            strTempMode = strArgument.Split('$')[0];
            strTempTarget = strArgument.Split('$')[1];
            strTempItem = strArgument.Split('$')[2];
            strTempDataKind = strArgument.Split('$')[3];

            Response.Redirect("../Interrogation/EditAddAskQuestionAnswer.aspx?Mode=Modify" + strTempTarget + "&RecentItemID=" + strTempItem + "&DataKind=" + strTempDataKind + "&EditPositation=QuestionDatabase&GroupID=" + strGroupID + "");
        }

        protected void btSaveNew_Click(object sender, EventArgs e)
        {
            //�إ�QID
            DataReceiver myReceiver = new DataReceiver();
            strQID = strUserID + "_" + myReceiver.getNowTime();

            //�x�s�D��
            clsTextQuestion myText = new clsTextQuestion();
            string strQTextContent = txtQuestionData.Text;
            string strATextContent = txtAnswerData.Text;
            strQTextContent = strQTextContent.Replace("&lt;", "<");
            strQTextContent = strQTextContent.Replace("&gt;", ">");
            strATextContent = strATextContent.Replace("&lt;", "<");
            strATextContent = strATextContent.Replace("&gt;", ">");

            myText.saveTextQuestion(strQID, strQTextContent, strATextContent, strUserID, strPaperID, strGroupDivisionID, strGroupID, hiddenQuestionMode.Value);

            //�x�s���D������
            int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(ddlQuestionLevel.SelectedValue);
            AuthoringTool.QuestionEditLevel.QuestionLevel.INSERT_QuestionLevel(strQID, iQuestionLevel);

            //�x�s���D���f�x
            AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevel_INSERT_QuestionSymptoms(strQID, ddlSymptoms.SelectedValue);

            //�p�G�OSpecific�D�ثh���x�s�@����Ʀ�Paper_Content
            if (hiddenQuestionMode.Value == "Specific")
            {
                int intContentSeq = myReceiver.getPaperContentMaxSeq(strPaperID) + 1;
                SQLString mySQL = new SQLString();
                mySQL.SaveToQuestionContent(strPaperID, strQID, "0", "2", hiddenQuestionMode.Value, intContentSeq.ToString());
            }

            //Redirect�ܤU�@�Ӻ���
            string strSystemFunction = "";
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
                        Response.Redirect("Paper_QuestionView.aspx?Opener=Paper_TextQuestionEditor");
                    }
                    else
                    {
                        Response.Redirect("Paper_QuestionMain.aspx?Opener=Paper_TextQuestionEditor");
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
