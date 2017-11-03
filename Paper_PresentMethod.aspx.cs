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

namespace PaperSystem
{
	/// <summary>
	/// Paper_PresentMethod ���K�n�y�z�C
	/// </summary>
    public partial class Paper_PresentMethod : AuthoringTool_BasicForm_BasicForm
	{
		//�إ�SqlDB����
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);

		string strUserID , strCaseID , strClinicNum , strSectionName , strPaperID;

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			this.getParameter();

			btnNext.ServerClick+=new EventHandler(btnNext_ServerClick);

            //�p�G�O�qpe�s����D�� �hback ����
            if (Session["PaperPurpose"] != null)
            {
                if (Session["PaperPurpose"].ToString() == "PEQuestion")
                {
                    btnPre.Style.Add("display", "none");
                }
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

			//Opener
			if(Session["Opener"] != null)
			{
				hiddenOpener.Value = Session["Opener"].ToString();
			}

			//Setup opener
			if(Session["Opener"] != null)
			{
				Session["Opener"] = "Paper_PresentMethod";
			}
			else
			{
				Session.Add("Opener","Paper_PresentMethod");
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
			string strGeneration = "";
			if(rb1.Checked == true)
			{
				strGeneration = "Edit";
			}
			else
			{
				strGeneration = "Present";
			}

			//�[�JSession
			if(Session["PresentType"] != null)
			{
				Session["PresentType"] = strGeneration;
			}
			else
			{
				Session.Add("PresentType" , strGeneration);
			}
			
			//�ק���Ʈw
			string strSQL = "UPDATE Paper_Header SET cGenerationMethod = '" + strGeneration + "' "+
							"WHERE cPaperID = '" + strPaperID + "' ";

			sqldb.ExecuteNonQuery(strSQL);

			if(strGeneration == "Edit")
			{
				Response.Redirect("Paper_QuestionMode.aspx");
			}
			else
			{
				//Response.Redirect("./QuestionGroupTree/QuestionGroupTree.aspx");  // modified by dolphin @ 2006-07-29, new Question Group Tree
                Response.Redirect("./QuestionGroupTree/QGroupTree.aspx");
			}
		}
	}
}
