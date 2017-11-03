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
	/// Paper_NewOrNot ���K�n�y�z�C
	/// </summary>
    public partial class Paper_NewOrNot : AuthoringTool_BasicForm_BasicForm
	{
		string strUserID , strCaseID , strClinicNum , strSectionName , strPaperID;

		DataReceiver myReceiver = new DataReceiver();

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			this.getParameter();
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

			//Opener
			if(Session["Opener"] != null)
			{
				hiddenOpener.Value = Session["Opener"].ToString();
			}

			//Setup opener
			if(Session["Opener"] != null)
			{
				Session["Opener"] = "Paper_NewOrNot";
			}
			else
			{
				Session.Add("Opener","Paper_NewOrNot");
			}

			//PresentType
			if(Session["PresentType"] != null)
			{
				hiddenPresentType.Value = Session["PresentType"].ToString();
			}
		
			//QuestionMode
			if(Session["QuestionMode"] != null)
			{
				hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
			}

			//EditMode
			if(Session["EditMode"] != null)
			{
				hiddenEditMode.Value = Session["EditMode"].ToString();
			}

			/* �ק��Paper_SelectQuestion �M Paper_RandomSelect
			//GroupID
			string strGroupID = "";
			if(Request.QueryString["GroupID"] != null)
			{
				strGroupID = Request.QueryString["GroupID"].ToString();
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
			*/

			//bModify
			if(Session["bModify"] != null)
			{
				Session["bModify"] = "false";
			}
			else
			{
				Session.Add("bModify" , "false");
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
	}
}
