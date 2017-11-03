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

namespace PaperSystem
{
	/// <summary>
	/// Paper_OtherQuestion ���K�n�y�z�C
	/// </summary>
    public partial class Paper_OtherQuestion : AuthoringTool_BasicForm_BasicForm
	{
		SQLString mySQL = new SQLString();
		string strUserID , strCaseID , strClinicNum , strSectionName , strPaperID;

		
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			this.getParameter();

			//�qQuestionMode�����ݩ󦹦Ҩ���Specific���D�s�JPaper_Content�C
			mySQL.saveSpecificToPaper_Content(strPaperID);

			//�[�JNext���ƥ�
			btnNext.ServerClick+=new EventHandler(btnNext_ServerClick);

			//�p�G�O�w�����D�������tr1�Mtr3�A�p�G�O�e�{�ɶüƿ��D�h����ܡC
			if(hiddenPresentType.Value == "Edit")
			{
				//tr1.Style.Add("DISPLAY","");
				//tr3.Style.Add("DISPLAY","");
			}
			else
			{
				//tr1.Style.Add("DISPLAY","none");
				//tr3.Style.Add("DISPLAY","none");
			}
		}

		private void getParameter()
		{
			//UserID
			if(Session["UserID"] != null)
			{
				strUserID = Session["UserID"].ToString();
			}
			
			//CaseID kyhCase200505301448128593750
			if(Session["CaseID"] != null)
			{
				strCaseID = Session["CaseID"].ToString();
			}
			
			//ClinicNum
			if(Session["ClinicNum"] != null)
			{
				strClinicNum = Session["ClinicNum"].ToString();
			}
			
			//SectionName ����
			if(Session["SectionName"] != null)
			{
				strSectionName = Session["SectionName"].ToString();
			}

			//PaperID
			if(Session["PaperID"] != null)
			{
				strPaperID = Session["PaperID"].ToString();
			}

			//QuestionGroupID
			if(Session["GroupID"] != null)
			{
				hiddenGroupID.Value = Session["GroupID"].ToString();
			}

			//Opener
			if(Session["Opener"] != null)
			{
				hiddenOpener.Value = Session["Opener"].ToString();
			}

			//Setup opener
			if(Session["Opener"] != null)
			{
				Session["Opener"] = "Paper_OtherQuestion";
			}
			else
			{
				Session.Add("Opener","Paper_OtherQuestion");
			}

			//QuestionMode
			if(Session["QuestionMode"] != null)
			{
				hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
			}

			//PresentType
			if(Session["PresentType"] != null)
			{
				hiddenPresentType.Value = Session["PresentType"].ToString();
			}
		
			//Edit method
			if(Session["EditMode"] != null)
			{
				hiddenEditMode.Value = Session["EditMode"].ToString();
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
			if(hiddenPresentType.Value == "Edit")
			{
				if(rb1.Checked == true)
				{
					if(Session["EditMode"] != null)
					{
						Session["EditMode"] = "Manual";
					}
					else
					{
						Session.Add("EditMode","Manual");
					}

					//�]�wQuestionMode
					if(Session["QuestionMode"] != null)
					{
						Session["QuestionMode"] = "General";
					}
					else
					{
						Session.Add("QuestionMode","General");
					}

					//�qGeneral�D�w����ʿ��D
					//Response.Redirect("./QuestionGroupTree/QuestionGroupTree.aspx?Opener=Paper_OtherQuestion");
                    Response.Redirect("Paper_EditMethod.aspx?Opener=Paper_OtherQuestion");
				}
//				else if(rb2.Checked == true)
//				{
//					if(Session["EditMode"] != null)
//					{
//						Session["EditMode"] = "Automatic";
//					}
//					else
//					{
//						Session.Add("EditMode","Automatic");
//					}
//
//					//�]�wQuestionMode
//					if(Session["QuestionMode"] != null)
//					{
//						Session["QuestionMode"] = "General";
//					}
//					else
//					{
//						Session.Add("QuestionMode","General");
//					}
//
//					//�qGeneral�D�w���üƿ��D
//					Response.Redirect("./QuestionGroupTree/QuestionGroupTree.aspx?Opener=Paper_OtherQuestion");
//				}
//				else if(rb3.Checked == true)
//				{
//					//�]�wQuestionMode
//					if(Session["QuestionMode"] != null)
//					{
//						Session["QuestionMode"] = "Specific";
//					}
//					else
//					{
//						Session.Add("QuestionMode","Specific");
//					}
//
//					//�s��䥦��Specific���D
//					Response.Redirect("./CommonQuestionEdit/Page/ShowQuestion.aspx?Opener=Paper_OtherQuestion");
//				}
				else if(rb4.Checked == true)
				{
					//�����Ҩ��s��
					Response.Redirect("Paper_MainPage.aspx?Opener=Paper_OtherQuestion");
				}
			}
			else
			{
//				if(rb2.Checked == true)
//				{
//					Response.Redirect("./QuestionGroupTree/QuestionGroupTree.aspx?Opener=Paper_OtherQuestion");
//				}
//				else if(rb4.Checked == true)
//				{
					Response.Redirect("Paper_MainPage.aspx?Opener=Paper_OtherQuestion");
//				}
			}
		}
	}
}
