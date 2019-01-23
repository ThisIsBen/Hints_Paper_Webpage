//===========================================================================
// ASP.NET 2.0 Web �M�ת��ഫ�L�{���A�ק�F�o���ɮסC
// ���O�W�٤w�g�ܧ�A���O�]�w�g�ק�A�H�~���ɮ� 'App_Code\Migrated\commonquestionedit\page\Stub_showquestion_aspx_cs.cs' ������H�����O
//�C
// �b���涥�q���A�o�i�H�� Web ���ε{��������L���O�A�ϥΩ�H�����Oô���Φs��
// �{���X��m�����C
// ���������e���� 'commonquestionedit\page\showquestion.aspx' �]�w�g�ק�A�H�Ѧҷs�����O�W�١C
// �p�ݦ��{���X�Ҧ����ԲӸ�T�A�аѾ\ http://go.microsoft.com/fwlink/?LinkId=46995 
//===========================================================================
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using suro.util;
using PaperSystem;
using Ajax;
using AuthoringTool.CommonQuestionEdit;
using AuthoringTool.CaseEditor.DiagnosisAndQuestion;
using Hints.DB;
using System.IO;

namespace AuthoringTool.CommonQuestionEdit
{
	/// <summary>
	/// EditSortedAskItem ���K�n�y�z�C
	/// </summary>
	public partial class Migrated_EditSortedAskItem : AuthoringTool_BasicForm_BasicForm
	{
		DataReceiver myReceiver = new DataReceiver();
        DataTable dtFeatureItem = new DataTable();
		DropDownList Question_Edit_Type_DDL = null;
		protected string Question_Edit_Type = "";//Question���s�諬�A;���ݶE���s�諬�A:Interrogation_Enquiry,����D���s�諬�A:Choice_Question,Script���D���s�諬�A:Script_Question	
		protected Table LayoutTable = new Table();
        protected System.Web.UI.WebControls.Table LayoutTableForFeature = new Table();
        Hints.DB.clsHintsDB sqlDB = new Hints.DB.clsHintsDB();
		private string CaseID;
		private string ClinicNum;
		private string SectionName;
		private string UserID;
        private string strCaseID, strSectionName, strPaperID, strAddNewQID;//2014/3/24 ����
		private QuestionAccessor qAccessor = null;
		private QuestionSelectionAccessor qsAccessor = null;
        public System.Web.UI.WebControls.Label Label1;
        /// <summary>
        /// �`�`�|�ۤv��^protected�A�����Opublic�sĶ�~���|�����~(�O�@�h�Ū����~)
        /// </summary>
        public string Language = "0";
        private string strQID = "";
        private string strGroupID = "";
        
        //Ben 2017 11 3 �s�ߤ���
        private void ShowSaveAsNewBtn()
        {

            // show "Save as new question" if the page is  currently used to modify existing question
            if ((Request.QueryString["bModify"] == "True") || (Session["bModify"].ToString() == "True"))
            {
                btSaveNew.Visible = true;

            }

        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();
            Ajax.Utility.RegisterTypeForAjax(typeof(AuthoringTool.CommonQuestionEdit.RemotingScripting));
            //�]�w�����һݤ��Ѽ�
            init();
            //�}�l�ܸ�ƮwŪ�����D�H�ΰ��D���ﶵ,�ñN���D���Ӥ��l���Y�e�{��HTML��Table��,�H�e�{�ܫe��
            ProcessData();
            this.FindControl("PanelQuestion").Controls.Add(LayoutTable);
            this.FindControl("PanelQuestion").Controls.Add(this.ParseControl("<hr>"));  
            //���g �W�[�s��S�x���ݩʪ��
            this.FindControl("PanelFeature").Controls.Add(LayoutTableForFeature);
            this.FindControl("Form1").Controls.Add(this.getEditQuestionButton());

            //Ben 2017 11 3 �s�s����
            ShowSaveAsNewBtn();
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
		/// �]�w�����һݤ��Ѽ�
		/// </summary>
		private void init()
		{
			//UserID
            if (Session["UserID"] != null)
            {
                UserID = Session["UserID"].ToString();
            }
            else
            {
                UserID = usi.UserID;
            }

            //CaseID
            if (Session["CaseID"] != null)
            {
                strCaseID = Session["CaseID"].ToString();
                hiddenCaseID.Value = Session["CaseID"].ToString();
            }

            //SectionName
            if (Session["SectionName"] != null)
            {
                strSectionName = Session["SectionName"].ToString();
                hiddenSectionName.Value = Session["SectionName"].ToString();
            }

            //PaperID
            if (Session["PaperID"] != null)
            {
                strPaperID = Session["PaperID"].ToString();
                hiddenPaperID.Value = Session["PaperID"].ToString();
            }
            
            //Opener
			if(Session["Opener"] != null)
			{
				hiddenOpener.Value = Session["Opener"].ToString();
                //Response.Write("<script>alert(" + Session["Opener"].ToString() + ") </script>");
               
            }

            if (Request.QueryString["Opener"]!=null)
            {

            
                //When generating a new ����D (if the Opener is Paper_QuestionTypeNew), set Previous page  as Paper_QuestionTypeNew
                if (Request.QueryString["Opener"].ToString() == "Paper_QuestionTypeNew")
                {


                    hiddenOpener.Value = Request.QueryString["Opener"].ToString();
                    /*
                    ////use JS alert() in C#
                    ScriptManager.RegisterStartupScript(
                     this,
                     typeof(Page),
                     "Alert",
                     "<script>alert('" + Session["PreviousPageURL"].ToString() + "');</script>",
                     false);
                    ///////
                    */
                }

                //If it's opened for modification in the state that this question is added in an exam paper 
                if (Request.QueryString["Opener"].ToString() == "Paper_MainPage")
                {
                    hiddenOpener.Value = Session["PreviousPageURL"].ToString();
                }
            }

                //Setup opener
                if (Session["Opener"] != null)
			{
				Session["Opener"] = "./CommonQuestionEdit/Page/ShowQuestion";
			}
			else
			{
				Session.Add("Opener","./CommonQuestionEdit/Page/ShowQuestion");
			}

			//PresentType
			if(Session["PresentType"] != null)
			{
				hiddenPresentType.Value = Session["PresentType"].ToString();
			}
		
			//QuestionType
			if(Session["QuestionMode"] != null)
			{
				hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
			}

			//EditMode
			if(Session["EditMode"] != null)
			{
				hiddenEditMode.Value = Session["EditMode"].ToString();
			}

			//ModifyType
			if(Session["ModifyType"] != null)
			{
				hiddenModifyType.Value = Session["ModifyType"].ToString();
			}

			//QuestionFunction
			if(Session["QuestionFunction"] != null)
			{
				hiddenQuestionFunction.Value = Session["QuestionFunction"].ToString();
			}

            //PreOpener
            if (Session["PreOpener"] != null)
            {
                if (Request.QueryString["Opener"] != null)
                    if (Request.QueryString["Opener"].ToString() == "Paper_MainPage")
                        hiddenPreOpener.Value = Session["PreOpener"].ToString();
                    else
                        hiddenPreOpener.Value = "";
            }

			//bModify
            //if(Session["bModify"] != null)
            //{
            //    hiddenBModify.Value = Session["bModify"].ToString();
            //    if (Session["bModify"] != null)
            //    {
            //        Session["bModify"] = hiddenBModify.Value;
            //    }
            //    else
            //    {
            //        Session.Add("bModify", hiddenBModify.Value);
            //    }
            //}
            if (Request.QueryString["bModify"] != null)
            {
                hiddenBModify.Value = Request.QueryString["bModify"].ToString();
                if (Session["bModify"] != null)
                {
                    Session["bModify"] = hiddenBModify.Value;
                }
                else
                {
                    Session.Add("bModify", hiddenBModify.Value);
                }
            }

			//GroupID
			strGroupID = "";
			if(Request.QueryString["GroupID"] != null)
			{
				strGroupID = Request.QueryString["GroupID"].ToString();
                hiddenGroupID.Value = strGroupID;
				if(Session["GroupID"] != null)
				{
					Session["GroupID"] = strGroupID;
				}
				else
				{
					Session.Add("GroupID",strGroupID);
				}
			}

			//GroupDivisionID
			if(strGroupID.Trim().Length > 0)
			{
				string strGroupDivisionID = myReceiver.getGroupDivisionID(strGroupID);
				if(Session["GroupDivisionID"] != null)
				{
					Session["GroupDivisionID"] = strGroupDivisionID;
				}
				else
				{
					Session.Add("GroupDivisionID",strGroupDivisionID);
				}
			}            
            Question_Edit_Type = this.getQuestion_Edit_Type();
            //�]�w�D�ظ��
            SetDataAccessor();
            //��l��layout
            InititateLayoutTable();
            //��l��Layout
            InititateLayoutTableForFeature();
            recordDisplayItemID.Value = "";			
			if(Question_Edit_Type=="Group_Question" || Question_Edit_Type=="Choice_Question")
			{
				if(Session["CurrentEditQuestionNum"]==null)
				{
					Session.Add("CurrentEditQuestionNum",0);//�O���ثe�ϥΪ̥��n�s����Ӱ��D
					recordCurrentEditQuestionNum.Value = Session["CurrentEditQuestionNum"].ToString();
				}
				else if(recordCurrentEditQuestionNum.Value!="" && recordCurrentEditQuestionNum.Value.ToUpper()!="NAN")
				{
					Session["CurrentEditQuestionNum"] = Convert.ToInt32(recordCurrentEditQuestionNum.Value);
				}
			}


            //�q�s��Ҩ��s�W�D�إ\��Ӫ� ������ܤW�@�D�U�@�D
            if (Session["IsFromClassExercise"] != null)
            {
                PreQusBtn.Attributes["style"] = "display:none;";
                NextQusBtn.Attributes["style"] = "display:none;";
            }
               
		}

        private void SetDataAccessor()
        {
            if (!IsPostBack)
            {
                ViewState.Add("Question_Edit_Type", Question_Edit_Type);
                initQuestion_Selection_Accessor();
            }
            else
            {
                if (Question_Edit_Type == ViewState["Question_Edit_Type"].ToString())
                {
                    qAccessor = (QuestionAccessor)Session["QuestionAccessor"];
                    qsAccessor = (QuestionSelectionAccessor)Session["QuestionSelectionAccessor"];
                    Session["QuestionAccessorParameter"] = getQuestionAccessParameter();
                }
                else
                {
                    ViewState["Question_Edit_Type"] = Question_Edit_Type;
                    initQuestion_Selection_Accessor();
                }
            }
        }
		
		/// <summary>
		/// //�غc���D��ܪ���M���D�ﶵ��ܪ���
		/// </summary>
		private void initQuestion_Selection_Accessor()
		{
            //if (Session["QuestionAccessorParameter"] == null)
            //{ // removed by dolphin @ 2006-08-10
            // modified @ 2006-08-10 by dolphin, update the Session variable in any case.
            // Fix the bug if using Question Editor first, then use the Diagnosis no update the Session, and vice versa.
            Session["QuestionAccessorParameter"] = getQuestionAccessParameter();
            //}
            qAccessor = new QuestionAccessor(Question_Edit_Type, (Hashtable)Session["QuestionAccessorParameter"],this);
			qsAccessor = new QuestionSelectionAccessor(qAccessor,Question_Edit_Type);				
			Session.Add("QuestionSelectionAccessor",qsAccessor);
			Session.Add("QuestionAccessor",qAccessor);
		}

		/// <summary>
		/// ���o�ܸ�ƪ��^�����D�һݪ�����Hashtable
		/// </summary>
		private Hashtable getQuestionAccessParameter()
		{
			if(this.Question_Edit_Type=="Script_Question")
			{
				Hashtable ret = new Hashtable();
				ArrayList fieldIndex = new ArrayList();//�ܸ�ƪ��^�����D����ƪ�������
				ret.Add("cScriptField","Present illness");
				fieldIndex.Add("cScriptField");
				ret.Add("cLanguage","English");
				fieldIndex.Add("cLanguage");
				ret.Add("FieldIndex",fieldIndex);
				return ret;
			}
			else if(this.Question_Edit_Type=="Group_Question")
			{
				Hashtable ret = new Hashtable();
				ArrayList fieldIndex = new ArrayList();//�ܸ�ƪ��^�����D����ƪ�������				
				if(Session["QuestionMode"].ToString().Equals("General"))
				{
					ret.Add("cDivisionID",Session["GroupDivisionID"].ToString());
					fieldIndex.Add("cDivisionID");
					ret.Add("cQuestionGroupID",Session["GroupID"].ToString());
					fieldIndex.Add("cQuestionGroupID");
				}
				else if(Session["QuestionMode"].ToString().Equals("Specific"))
				{
					ret.Add("cPaperID",Session["PaperID"].ToString());
					fieldIndex.Add("cPaperID");
				}
				ret.Add("cQuestionType","1");
				fieldIndex.Add("cQuestionType");
				ret.Add("cQuestionMode",Session["QuestionMode"].ToString());
				fieldIndex.Add("cQuestionMode");
				ret.Add("FieldIndex",fieldIndex);
				return ret;
			}
			else if(this.Question_Edit_Type=="Choice_Question")
			{
                //���oEntryDiagnosisAndQuestion.aspx�Ҷǻ��L�Ӧ���Diagnosis���Ѽ�
                //�]�tCaseID,SectionName,ClinicNum
                //EntryDiagnosisAndQuestion entryDiagnosisAndQuestion = null;
                //entryDiagnosisAndQuestion = (EntryDiagnosisAndQuestion)this.Page.PreviousPage;
                //Hashtable diagnosisParameter = entryDiagnosisAndQuestion.diagnosisParameter;
                CaseID = usi.CaseID;
                SectionName = usi.Section;
                ClinicNum = usi.ClinicNum.ToString();

				Hashtable ret = new Hashtable();
				ArrayList fieldIndex = new ArrayList();//�ܸ�ƪ��^�����D����ƪ�������
				ret.Add("cCaseID",CaseID);
				fieldIndex.Add("cCaseID");
				ret.Add("cSectionName",SectionName);
				fieldIndex.Add("cSectionName");
				ret.Add("sClinicNum",ClinicNum);
				fieldIndex.Add("sClinicNum");
				ret.Add("FieldIndex",fieldIndex);				
				return ret;
			}
			else
			{
				Hashtable ret = new Hashtable();
				ArrayList fieldIndex = new ArrayList();//�ܸ�ƪ��^�����D����ƪ�������
				ret.Add("cCaseID",CaseID);
				fieldIndex.Add("cCaseID");
				ret.Add("cSectionName",SectionName);
				fieldIndex.Add("cSectionName");
				ret.Add("sClinicNum",ClinicNum);
				fieldIndex.Add("sClinicNum");
				ret.Add("FieldIndex",fieldIndex);
				return ret;
			}
		}

		/// <summary>
		/// ���o�ҭn�s����D�����A
		/// �ݶE���s�諬�A:Interrogation_Enquiry,����D���s�諬�A:Choice_Question,Script���D���s�諬�A:Script_Question
		/// </summary>
		private string getQuestion_Edit_Type()
		{			
			string Question_Edit_Type = "Group_Question";
            if (!IsPostBack)
            {
                if (Request.QueryString["PreviousPage"] != null && Request.QueryString["PreviousPage"].IndexOf("entrydiagnosisandquestion") >= 0)
                {
                    Question_Edit_Type = "Choice_Question";
                }
                else
                {
                    Question_Edit_Type = "Group_Question";
                }
            }
            else 
            {
                Question_Edit_Type = ViewState["Question_Edit_Type"].ToString();
            }
			return Question_Edit_Type;
		}

		/// <summary>
		/// �}�l�ܸ�ƮwŪ�����D�H�ΰ��D���ﶵ,�ñN���D���Ӥ��l���Y�e�{��HTML��Table��,�H�e�{�ܫe��
		/// </summary>
		private void ProcessData()
		{	
			recordDisplayItemID.Value = this.Request.Form["recordDisplayItemID"];
            DataRow[] drs = qAccessor.QuestionIndex.Select("sLevel=1"); //���o�Ĥ@���h���_�l���D            
			string QID = "";
            if (this.Question_Edit_Type == "Group_Question" && !Convert.ToBoolean(Session["bModify"]) && !this.IsPostBack || (this.Question_Edit_Type == "Choice_Question" && !this.IsPostBack && drs.Length==0))
			{
				QID = CommonQuestionUtility.GetNewID(this.UserID,"Question");    //�U���^��ҳB�z���DID		
                this.qAccessor.add_New_Question(QID);
				string[] new_SelectionID = CommonQuestionUtility.GetNewID(this.UserID,"Selection",4);//�s�W���ﶵID
				for(int i=0;i<4;i++)
				{
					this.qsAccessor.add_new_selection(QID,new_SelectionID[i]);
				}				
			}

            
            drs = qAccessor.QuestionIndex.Select("sLevel=1"); //���o�Ĥ@���h���_�l���D
            QuestionItemControlTable qItemControlTable = null;//QuestionItemControlTable		
			//			string LevelAndRank = "";
			//�U���^��ҳB�z���DID
			string QuestionText = "";
            string KeyWordsText = "";
			if(this.Question_Edit_Type=="Group_Question" || this.Question_Edit_Type=="Choice_Question")
			{				
				if(Session["totalQuestionNum"]==null)
				{
					Session.Add("totalQuestionNum",drs.Length);
				}
				else
				{
					Session["totalQuestionNum"] = drs.Length;
				}
				if(!Convert.ToBoolean(Session["bModify"]))
				{
					if( this.Page.IsPostBack==false)
					{
						Session["CurrentEditQuestionNum"] = Convert.ToInt32(Session["totalQuestionNum"])-1;
					}
				}
				else
				{
					if((Request.QueryString["QID"] + "").Length>0)
					{
                        strQID = Request.QueryString["QID"].ToString();
                        Session["CurrentEditQuestionNum"] = getCurrentEditQuestionNumByQID(drs,Request.QueryString["QID"] + "");//�b�ק�Ҧ��U,���o�O���Ӱ��D�n�ק�
					}
					else
					{
						Session["CurrentEditQuestionNum"] = Convert.ToInt32(Session["totalQuestionNum"])-1;
					}
				}
			}
			
			for(int i=0;i<drs.Length;i++)//�}�l�w��C�Ӱ��D�@�B�z
			{
				QID = drs[i]["cQID"].ToString();                //���DID
				set_recordDisplayItemID(QID);
				QuestionText = drs[i]["cQuestion"].ToString();  //���D���e		
                KeyWordsText = drs[i]["cKeyWords"].ToString();  //����r���e
                qItemControlTable = new QuestionItemControlTable(this.Question_Edit_Type, this.UserID, QID, QuestionText, this, 1, this.recordDisplayItemID, CaseID);				
				this.LayoutTable.Rows.Add(new TableRow());
				this.LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells.Add(new TableCell());
				this.LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells[0].Controls.Add(qItemControlTable);
				if(this.Question_Edit_Type=="Group_Question" || this.Question_Edit_Type=="Choice_Question")
				{
					this.LayoutTable.Rows[LayoutTable.Rows.Count-1].Style.Add("display","none");
				}
			}
            
            //���g �[�J�S�x��TABLE
            if (strQID == "")
            {
                strQID = QID;//���g �קK�s���D�خ�strQID����
            }
            FeatureItemControl qFeatureItemControlTable = new FeatureItemControl(this, 1, strGroupID, strQID); //FeatureItemControlTable  
            qFeatureItemControlTable.ID = "FeatureItemControlTable";
            this.LayoutTableForFeature.Rows.Add(new TableRow());
            this.LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells.Add(new TableCell());
            this.LayoutTableForFeature.Rows[LayoutTableForFeature.Rows.Count - 1].Cells[0].Controls.Add(qFeatureItemControlTable);

            //�s�W�D�خɪ�QID ����2014/3/25
            strAddNewQID = QID;
		}


		/// <summary>
		/// �ھ�QID���o���D���Ǹ�
		/// </summary>
		/// <returns></returns>
		private int getCurrentEditQuestionNumByQID(DataRow[] drs,string QID)
		{
			int ret = 0;
			for(int i=0;i<drs.Length;i++)
			{
				if(drs[i]["cQID"].ToString()==QID)
				{
					ret = i;
					break;
				}
			}
			return ret;
		}

		private void set_recordDisplayItemID(string strQID)
		{
            if (this.Question_Edit_Type == "Group_Question" || this.Question_Edit_Type == "Choice_Question")
			{
				if(recordDisplayItemID.Value=="")
				{
					recordDisplayItemID.Value = "QuestionItemTable@" + strQID + ";";
				}
				else
				{
					recordDisplayItemID.Value = recordDisplayItemID.Value + "QuestionItemTable@" + strQID + ";";
				}
			}
		}		

		/// <summary>
		/// ��l�ƭn�e�{���ϥΪ̾ާ@������,�]���ϥΪ̭n�ާ@�������ƪ���n�O�Q��LayoutTable�Ӧw�Ʀ�m
		/// </summary>
		private void InititateLayoutTable()
		{
			//LayoutTable���]�w
			//�Ĥ@�C
            LayoutTable.Width = Unit.Parse("100%");
            //LayoutTable.BorderWidth = Unit.Parse("3px");
            //LayoutTable.BorderColor = Color.Green;
			LayoutTable.ID="LayoutTable_1";
			LayoutTable.Rows.Add(new TableRow());
			LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells.Add(new TableCell());
			Label Title = new Label();
            if (ViewState["TitleText"] == null)
            {
                if (this.Question_Edit_Type == "Choice_Question")
                {
                    ViewState["TitleText"] = usi.Section;
                }
                else
                {
                    ViewState["TitleText"] = "Question Editor";
                }
            }
           
            Title.Text = ViewState["TitleText"].ToString();
			//Title.Font.Size = FontUnit.Parse("25");
			//Title.ForeColor = Color.DarkRed;  // removed by dolphin @ 2006-07-28
			LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells[0].Controls.Add(Title);
			LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells[0].CssClass = "title";


			//�ĤG�C
			LayoutTable.Rows.Add(new TableRow());
			LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells.Add(new TableCell());
			Table container = CommonQuestionUtility.get_HTMLTable(1,2);

			Label questionTypeLabel = new Label();
			questionTypeLabel.Text = "<b>Question Type:&nbsp;</b>";
            questionTypeLabel.Visible = false;
            
            ////��ܽs��Ҧ����U�Ԧ����
            //Question_Edit_Type_DDL = getQuestion_Edit_Type_DDL();

			
			//�s�W���D��Button
			Button btnNew = new Button();
			btnNew.BackColor = Color.FromName("#FF9933");
			btnNew.Width = Unit.Pixel(140);
			btnNew.Text ="Add New Question";
			btnNew.ID = "NewItemButton_1";
			btnNew.Click += new EventHandler(btnNew_Click);			

            //container.Rows[container.Rows.Count-1].Cells[0].Controls.Add(questionTypeLabel);
            //container.Rows[container.Rows.Count-1].Cells[0].Controls.Add(Question_Edit_Type_DDL);
			container.Rows[container.Rows.Count-1].Cells[1].Controls.Add(btnNew);
			container.Rows[container.Rows.Count-1].Cells[1].Width = Unit.Parse("10%");

			LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells[0].Controls.Add(container);
		}
        
        /// <summary>
        /// ��l�ƭn�e�{���ϥΪ̳]�w�S�x�Ȫ��ާ@������
        /// </summary>
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
           
        }

		/// <summary>
		/// ���oFinish Button
		/// </summary>
		private Control getEditQuestionButton()
		{
			PreQusBtn.Visible = true;
			NextQusBtn.Visible = true;
			ImageButton finishBtn = new ImageButton();
			finishBtn.ID = "finishBtn";
			finishBtn.ImageAlign = ImageAlign.Right;
			finishBtn.ImageUrl = "../Image/finish.gif";
			finishBtn.Click +=new ImageClickEventHandler(finishBtn_Click);
			if(this.Question_Edit_Type=="Group_Question" || this.Question_Edit_Type=="Choice_Question")
			{
				Table container = new Table();
				container.Rows.Add(new TableRow());
				container.Rows[container.Rows.Count-1].Cells.Add(new TableCell());
				container.Rows[container.Rows.Count-1].Cells.Add(new TableCell());
				container.Rows[container.Rows.Count-1].Cells.Add(new TableCell());
				container.Rows[container.Rows.Count-1].Cells.Add(new TableCell());
				container.Rows[container.Rows.Count-1].Cells.Add(new TableCell());
                container.Rows[container.Rows.Count - 1].Cells.Add(new TableCell());
				if(this.Question_Edit_Type=="Choice_Question")
				{
					btnPre.Visible = false;
                    btnNext.Value = "Submit";//"Continue";

                    // added by dolphin @ 2006-08-02
                    HtmlInputButton btBack = new HtmlInputButton();
                    btBack.Value = "Back";
                    btBack.Attributes.Add("class", "button_continue");
                    btBack.Attributes.Add("onclick", "toSectionMenuAuthoring()");
                    container.Rows[container.Rows.Count - 1].Cells[0].Controls.Add(btBack);
                    //container.Rows[container.Rows.Count - 1].Cells[0].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));
				}
                container.Rows[container.Rows.Count - 1].Cells[0].Controls.Add(btSaveNew);
                container.Rows[container.Rows.Count - 1].Cells[0].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));

                container.Rows[container.Rows.Count-1].Cells[1].Controls.Add(btnPre);
                container.Rows[container.Rows.Count - 1].Cells[1].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));

				container.Rows[container.Rows.Count-1].Cells[2].Controls.Add(PreQusBtn);
                if (hiddenPreOpener.Value == "SelectPaperMode")//�qORCS���Ұ�m�߭��������Ѽ� 2014/3/24 ����
                    container.Rows[container.Rows.Count - 1].Cells[2].Controls.Add(this.ParseControl(""));
                else
                    container.Rows[container.Rows.Count - 1].Cells[2].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));

				container.Rows[container.Rows.Count-1].Cells[3].Controls.Add(NextQusBtn);
                if (hiddenPreOpener.Value == "SelectPaperMode")//�qORCS���Ұ�m�߭��������Ѽ� 2014/3/24 ����
                    container.Rows[container.Rows.Count - 1].Cells[3].Controls.Add(this.ParseControl(""));
                else
                    container.Rows[container.Rows.Count - 1].Cells[3].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));

				container.Rows[container.Rows.Count-1].Cells[4].Controls.Add(btnNext);
                container.Rows[container.Rows.Count - 1].Cells[4].Controls.Add(this.ParseControl("&nbsp;&nbsp;&nbsp;"));
				container.Rows[container.Rows.Count-1].Cells[5].Controls.Add(finishBtn);
				finishBtn.Style.Add("display","none");
				
				return container;
			}
			else
			{				
				return finishBtn;
			}		
		}		

		/// <summary>
		/// ��ܽs��Ҧ����U�Ԧ����
		/// </summary>
		private DropDownList getQuestion_Edit_Type_DDL()
		{
			DropDownList ret = new DropDownList();
			ret.ID = "Question_Edit_Type_DDL";
			ret.AutoPostBack = true;
			//Question���s�諬�A;���ݶE���s�諬�A:Interrogation_Enquiry,����D���s�諬�A:Choice_Question,Script���D���s�諬�A:Script_Question
			ret.Items.Add(new ListItem("Interrogation Enquiry","Interrogation_Enquiry"));
			ret.Items.Add(new ListItem("GroupQuestion","Group_Question"));
			ret.Items.Add(new ListItem("Choice Question","Choice_Question"));
			ret.Items.Add(new ListItem("Script Question","Script_Question"));
            if (Request.QueryString["PreviousPage"].IndexOf("entrydiagnosisandquestion") >= 0)
            {
                //�q�������ɹL�Ӫ�ܬODiagnosis
                ret.SelectedIndex = 2;
            }
            else 
            {
                ret.SelectedIndex = 1;
            }
			ret.Style.Add("display","none");
			return ret;
		}
        //�x�s������A�U�ӭ������s���]�w
		private void FinishSave()
		{
            //�Y�q�s��Ҩ��ӡA��s�Ҩ�����
            if (Session["IsFromClassExercise"] != null && Session["IsFromClassExercise"].ToString().Equals("True") && Session["PreOpener"] != null && Session["PreOpener"] == "SelectPaperModeAddANewQuestion")
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Refresh", "opener.document.getElementById('btnRefresh').click();window.close();", true);
                return;
            }

			string strScriptExecuteCommand = "";//�ھڤ��P���s��覡�M�w�e�ݵ{���X
			string strScriptID = "";            //��X�ܫe�ݪ��e�ݵ{���XID
			if(this.Question_Edit_Type=="Group_Question")
			{
				strScriptExecuteCommand = "goNext();\n";
				strScriptID = "goNextScript";				
			}
			else
			{
				//strScriptExecuteCommand = "alert('Save successfully!');\n";
				//strScriptID = "finishScript";
                Response.Redirect("/Hints/Flow Control/terminator.aspx");
			}
			if(!Page.IsStartupScriptRegistered("FinishScript"))
			{
				string scriptStr = "\n<script language=JavaScript>\n";					
				scriptStr += strScriptExecuteCommand;
				scriptStr += "<";
				scriptStr += "/";
				scriptStr += "script>";
				Page.RegisterStartupScript(strScriptID, scriptStr);
			}
		}

		private void newFirstLevelQuestion()
		{
			QuestionItemControlTable qItemControlTable = null;//QuestionItemControlTable
			string QID = CommonQuestionUtility.GetNewID(this.UserID,"Question");    //�U���^��ҳB�z���DID						
            qItemControlTable = new QuestionItemControlTable(this.Question_Edit_Type, this.UserID, QID, "", this, 1, recordDisplayItemID, CaseID);
			this.LayoutTable.Rows.Add(new TableRow());
			this.LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells.Add(new TableCell());
			this.LayoutTable.Rows[LayoutTable.Rows.Count-1].Cells[0].Controls.Add(qItemControlTable);
			this.qAccessor.add_New_Question(QID);
			qItemControlTable.addQuestionAnswer(this.Page);

			if((Question_Edit_Type=="Group_Question" || Question_Edit_Type=="Choice_Question")&& !Convert.ToBoolean(Session["bModify"]))
			{
				Session["CurrentEditQuestionNum"] = Convert.ToInt32(Session["totalQuestionNum"]);
			}
			Session["totalQuestionNum"] = Convert.ToInt32(Session["totalQuestionNum"]) + 1;
		}

		/// <summary>
		/// �s�W�@�Ӱ��D���ƥ�B�z�禡
		/// </summary>
		private void btnNew_Click(object sender, EventArgs e)
		{			
			newFirstLevelQuestion();			
		}	
	
		private void finishBtn_Click(object sender, ImageClickEventArgs e)
		{
            //2017 11 06 Ben test strAddNewQID
            //strAddNewQID = Request.QueryString["QID"];

            //���g 2012/11/25 �N�ϥΪ̩ҿ�ܪ��S�x�Ȧs�J�Ȧs�}�C���A���x�s���Ʈw���C
            clsFeaturevalue clsSaveFeature = new clsFeaturevalue();
            clsSaveFeature.update_FeatureItemIntoDataBase(clsSaveFeature.get_dtFeatureItem_Data(strGroupID, this));
            this.qAccessor.update_DataTableIntoDatabase();//�P���D���������D��Ʀs�J��Ʈw
            this.qsAccessor.update_DataTableIntoDatabase();//�P���D�ﶵ���������D�ﶵ��Ʀs�J��Ʈw
            if(this.Question_Edit_Type!="Choice_Question")
			{
				this.qAccessor.update_QuestionSource_In_Database();//�N�����D���������λ���Ʀs�J������ƪ�
			}

            //�ˬd���e�O�_����
            clsHintsDB hintsDB = new clsHintsDB();
            string strScript ="";
            string strSQL = "";
            strSQL = "SELECT [cQuestion] FROM [NewVersionHintsDB].[dbo].[QuestionIndex] where cQID = '" + strAddNewQID + "'";
            string strQuestionContent = hintsDB.getDataSet(strSQL).Tables[0].Rows[0]["cQuestion"].ToString();
            if(strQuestionContent.Equals("")){
                strScript += "�D�ؤ��e����g";
            }

            strSQL = "SELECT cSelection , bCaseSelect FROM [NewVersionHintsDB].[dbo].[QuestionSelectionIndex] where cQID='" + strAddNewQID + "'";
            DataTable dtQuestionSelection = hintsDB.getDataSet(strSQL).Tables[0];
            bool isSelectionNull = false;
            bool isSelectionCheckNull = true;
            foreach (DataRow drSelection in dtQuestionSelection.Rows)
            {
                if (drSelection["cSelection"].ToString().Trim().Equals(""))
                    isSelectionNull = true;
                string s = drSelection["bCaseSelect"].ToString();
                if (drSelection["bCaseSelect"].ToString().Equals("True"))
                    isSelectionCheckNull = false;
            }
            if (isSelectionNull)
            {
                if (!strScript.Equals(""))
                    strScript += ",";
                strScript += "����D�ﶵ���e����g";
            }
            if (isSelectionCheckNull)
            {
                if (!strScript.Equals(""))
                    strScript += ",";
                strScript += "����D�ﶵ���ץ���";
            }
            if (!strScript.Equals(""))
            {
                strSQL = "DELETE FROM [NewVersionHintsDB].[dbo].[QuestionIndex] WHERE cQID = '" + strAddNewQID + "'";
                hintsDB.ExecuteNonQuery(strSQL);
                strSQL = "DELETE FROM [NewVersionHintsDB].[dbo].[QuestionSelectionIndex] WHERE cQID = '" + strAddNewQID + "'";
                hintsDB.ExecuteNonQuery(strSQL);
                ClientScript.RegisterClientScriptBlock(this.GetType(), "Refresh", "alert('" + strScript + ",�ж�g����A�s��')", true);
                return;
            }

            Session["QuestionAccessorParameter"] = null;
            Session["QuestionAccessor"] = null;
            Session["QuestionSelectionAccessor"] = null;
            //�Y�q�s��Ҩ��ӡA�����N���D�s�W�ܦҨ���
            if (Session["IsFromClassExercise"] != null && Session["IsFromClassExercise"].ToString().Equals("True"))
            {
                /*write SQL to file to 
                //to inspect the SQL cmd when something went wrong with SQL cmd 
                // Create a file to write to.              
                //File.WriteAllText("D:/Hints_on_60/Hints/App_Code/AuthoringTool/CaseEditor/Paper/updateSimilarIDSQL.txt", "I am krew.");
                */
                 
                DataReceiver myReceiver = new DataReceiver();
                SQLString mySQL = new SQLString();
                //���o�Ҩ��D��
                string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);
                mySQL.SaveToQuestionContent(strPaperID, strAddNewQID, "0", "1", "General", strSeq);
            }
			FinishSave();

            //����Session
            Session.Remove("PreOpener");

            bool bIsZero = true;//�ΨӧP�_�ϥΪ̬O�_���]�w����

            for (int i = 0; i < Request.Form.Count; i++)
            {

                if (Request.Form.Keys[i].ToString().IndexOf("editLevelDdl@") != -1)
                {
                   string strQuestionLevel = Request.Form[i].ToString();
                   string[] strQuestionID = Request.Form.Keys[i].ToString().Split('@');
                   int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(strQuestionLevel);
                   AuthoringTool.QuestionEditLevel.QuestionLevel.INSERT_QuestionLevel(strQuestionID[1], iQuestionLevel);
                }

                if (Request.Form.Keys[i].ToString().IndexOf("editGradeTensDdl@") != -1)
                {
                    string strQuestionGradeTens = Request.Form[i].ToString();
                    string strQuestionGradeUnits = Request.Form[i+1].ToString();
                    string[] strQuestionID = Request.Form.Keys[i].ToString().Split('@');
                    AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionGrade_INSERT(strQuestionID[1], strQuestionGradeTens + strQuestionGradeUnits);

                    if (strQuestionGradeUnits != "0" || strQuestionGradeTens != "0")
                        bIsZero = false;
                }
            }

            //�p�G�ϥΪ̳��L�]�w���� �t�Φۤv�H�D�ƥh��C�D����
            if (bIsZero)
            {
                string strQuestionGradeTens = "";
                string strQuestionGradeUnits = "";
                int iQuestionCount =0;
                if (Session["GroupID"] != null)
                {
                    DataTable dtQuestionLevelNum = PaperSystem.DataReceiver.GetQuestionLevelNum(Session["GroupID"].ToString());
                    if (dtQuestionLevelNum.Rows.Count > 0)
                        iQuestionCount = Convert.ToInt32(dtQuestionLevelNum.Rows[0]["QuestionLevelNum"].ToString());
                }
                else
                {
                    iQuestionCount = qAccessor.QuestionIndex.Rows.Count;
                }


                for (int i = 0; i < Request.Form.Count; i++)
                {
                    if (Request.Form.Keys[i].ToString().IndexOf("editGradeTensDdl@") != -1)
                    {
                        string[] strQuestionID = Request.Form.Keys[i].ToString().Split('@');
                        AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionGrade_INSERT(strQuestionID[1], strQuestionGradeTens + strQuestionGradeUnits);
                    }
                }
            }

		}

        protected void btSaveNew_Click(object sender, EventArgs e)
        {
            string strQuestion = "";
            string strSelection = "";
            string strResponse = "";
            int strCorrectAnswer = 0;
            string[] strQuestionID = null;
            string strNewQID = CommonQuestionUtility.GetNewID(this.UserID, "Question");
            string strNewSID = CommonQuestionUtility.GetNewID(this.UserID, "Selection");
            string strSID = "";
            string strRID = "";
            string strCID = "";

            //get current(the parent of the new question) QID from URL. Ben
            string templateQuestionQID = Request.QueryString["QID"];

            for (int i = 0; i < Request.Form.Count; i++)
            {
                if (Request.Form.Keys[i].ToString().IndexOf("QuestionTextBox@" + strQID) != -1)
                {
                    strQuestion = Request.Form[i].ToString();
                    this.qAccessor.QuestionIndex_INSERT(strNewQID, strQuestion);
                    //�x�s�@����Ʀ�QuestionMode
                    SQLString mySQL = new SQLString();

                    //Ben add similarID 
                    //mySQL.saveIntoQuestionMode(strNewQID, "", "", strGroupID, "General", "1");
                    mySQL.saveIntoQuestionMode(strNewQID, "", "", strGroupID, "General", "1", templateQuestionQID, this.UserID);
                }
                if (Request.Form.Keys[i].ToString().IndexOf("editLevelDdl@" + strQID) != -1)
                {
                    string strQuestionLevel = Request.Form[i].ToString();
                    int iQuestionLevel = AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionLevelName_SELECT_QuestionLevel(strQuestionLevel);
                    AuthoringTool.QuestionEditLevel.QuestionLevel.INSERT_QuestionLevel(strNewQID, iQuestionLevel);
                }
                if (Request.Form.Keys[i].ToString().IndexOf("editGradeTensDdl@" + strQID) != -1)
                {
                    string strQuestionGradeTens = Request.Form[i].ToString();
                    string strQuestionGradeUnits = Request.Form[i + 1].ToString();

                    AuthoringTool.QuestionEditLevel.QuestionLevel.QuestionGrade_INSERT(strNewQID, strQuestionGradeTens + strQuestionGradeUnits);
                }
                DataTable dtQuestionSelectionIndex = Hints.Learning.Question.SQLString.getSelectionListFromQuestionSelectionIndexByQID(strQID);
                int iSCount = 0;
                foreach (DataRow drQuestionSelectionIndex in dtQuestionSelectionIndex.Rows)
                {
                    if (Request.Form.Keys[i].ToString().IndexOf("SelectionTextBox@" + drQuestionSelectionIndex["cSelectionID"].ToString()) != -1)
                    {
                        strSelection = Request.Form[i].ToString();
                        strSID = drQuestionSelectionIndex["cSelectionID"].ToString();

                    }
                    if (Request.Form.Keys[i].ToString().IndexOf("SelectionResponseTextBox@" + drQuestionSelectionIndex["cSelectionID"].ToString()) != -1)
                    {
                        strResponse = Request.Form[i].ToString();
                        strRID = drQuestionSelectionIndex["cSelectionID"].ToString();
                    }
                    strCorrectAnswer = 0;
                    if (Request.Form.Keys[i].ToString().IndexOf("IsCorrectChkBox@" + strQID + "#" + drQuestionSelectionIndex["cSelectionID"].ToString()) != -1)
                    {
                        strCorrectAnswer = 1;
                        strCID = drQuestionSelectionIndex["cSelectionID"].ToString();
                    }
                    else
                    {
                        strCID = drQuestionSelectionIndex["cSelectionID"].ToString();
                    }

                    if (strSID != "" && strRID != "" && strCID != "")
                    {
                        if ((strSID == strRID) && (strSID == strCID) && (strCID == strRID))
                        {
                            iSCount = DataReceiver.QuestionSelectionIndex_SELECT_Seq(strQID, drQuestionSelectionIndex["cSelectionID"].ToString());
                            this.qAccessor.QuestionSelectionIndex_INSERT(strNewQID, strNewSID + "_" + iSCount, iSCount, strSelection, strResponse, strCorrectAnswer);
                        }
                    }
                   //���g 2012/11/25 �N�ϥΪ̩ҿ�ܪ��S�x�Ȧs�J�Ȧs�}�C���A���x�s���Ʈw���C
                   // this.qAccessor.update_FeatureItemIntoDataBase(strQID, get_dtFeatureItem_Data(strGroupID));
                }
            }
            //�Y�q�s��Ҩ��ӡA�����N���D�s�W�ܦҨ��� ���� 2014/3/25
            if (hiddenPreOpener.Value == "SelectPaperMode")
            {
                DataReceiver myReceiver = new DataReceiver();
                SQLString mySQL = new SQLString();
                //���o�Ҩ��D��
                string strSeq = Convert.ToString(myReceiver.getPaperContentMaxSeq(strPaperID) + 1);
                mySQL.SaveToQuestionContent(strPaperID, strNewQID, "0", "1", "General", strSeq);
            }
            FinishSave();
        }
    }
}