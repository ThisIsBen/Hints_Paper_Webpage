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
	/// SectionTreeView ���K�n�y�z�C
	/// </summary>
    public partial class SectionTreeView : AuthoringTool_BasicForm_BasicForm
	{
		string strUserLevel = "1";
		string strCaseID = "";
		string strDivisionID = "6061";

		//�إ�SqlDB����
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			//�����Ѽ�CaseID , DivisionID
			this.getParameter();

			//�إ�Section tree
			this.setupSectionTree();
		}

		private void setupSectionTree()
		{
			//�إ�tree�����njavascript
			string NodeSc="<SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/JSCookTree.js'></SCRIPT><link REL='stylesheet' HREF='../Translate tree/theme.css' TYPE='text/css'><SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/theme.js'></SCRIPT>";
			Page.RegisterStartupScript("NodeSc",NodeSc);

			//�إ�tree��string
			string NodeStr="[";

			//---------------------------------------���oCaseTree���C�@��Node----------------------------------------
			string strSQL = "";

			//�إ�Visit
			strSQL = "SELECT MAX(sClinicNum) AS 'MaxClinic' FROM UserLevelPresent WHERE cUserLevel = '"+strUserLevel+"' AND cCaseID = '"+strCaseID+"' AND cDivisionID = '"+strDivisionID+"' ";
			
			//-------------------------------------------�NCaseTreeView����Node�[��TreeView��------------------------------------

			//-----------------------------------------------�إߤ@��Hospital Node-----------------------------------------------
		//	NodeStr = NodeStr + "[null,'"+"Hospital"+"',null,'_self',null";
			
			//--------------------------------------�إ�Hospital���lNode�ADepartmentNode-------------------------------------
			DataSet dsVisit = sqldb.getDataSet(strSQL);
			foreach (DataRow drVisit in dsVisit.Tables[0].Rows)
			{
				int intVisit = 1;
				try
				{
					intVisit = Convert.ToInt32(drVisit["sClinicNum"]);
				}
				catch
				{
				}

				NodeStr = NodeStr +",[null,'" + intVisit.ToString().Trim() + "',null,null,null";

				strSQL = "SELECT cSectionName FROM UserLevelPresent WHERE cUserLevel = '"+strUserLevel+"' AND cCaseID = '"+strCaseID+"' AND cDivisionID = '"+strDivisionID+"' AND sClinicNum = '"+intVisit.ToString()+"' ORDER BY sSectionSeq ";
				DataSet dsSection=sqldb.getDataSet(strSQL);
				foreach (DataRow drSection in dsSection.Tables[0].Rows)
				{
					NodeStr = NodeStr + ",[null,'" + drSection["cSectionName"].ToString().Trim() + "',null,'frmMain',null";
					
					NodeStr = NodeStr + "]";
				}
				dsSection.Dispose();
				//�����إ�DivisionNode
				NodeStr = NodeStr + "]";

			}
			dsVisit.Dispose();

			//�����NTreeNode�[��TreeView��
			string NodePut = "<Font size='26'><script language='JavaScript'>var myMenu=" + NodeStr + "]</script></Font>";
			NodePut=NodePut + "<DIV ID=myMenuID  align='left' ></DIV>";
			NodePut = NodePut + "<SCRIPT LANGUAGE='JavaScript'>ctDraw ('myMenuID', myMenu, ctThemeXP2, '../Translate tree/ThemeXP', 1, 3);</SCRIPT>";
			Page.RegisterStartupScript("NodePut",NodePut);
		}

		private void getParameter()
		{
			//CaseID
			if(Request.QueryString["CaseID"] != null)
			{
				strCaseID = Request.QueryString["CaseID"];
				if(Session["CaseID"] != null)
				{
					Session["CaseID"] = strCaseID;
				}
				else
				{
					Session.Add("CaseID",strCaseID);
				}
			}
			else
			{
				if(Session["CaseID"] != null)
				{
					strCaseID = Session["CaseID"].ToString();
				}
				else
				{
					strCaseID = "linCase200308131525133152432";
				}
			}

			//DivisionID
			if(Request.QueryString["DivisionID"] != null)
			{
				strDivisionID = Request.QueryString["DivisionID"];
				if(Session["DivisionID"] != null)
				{
					Session["DivisionID"] = strDivisionID;
				}
				else
				{
					Session.Add("DivisionID",strDivisionID);
				}
			}
			else
			{
				if(Session["DivisionID"] != null)
				{
					strDivisionID = Session["DivisionID"].ToString();
				}
				else
				{
					strDivisionID = "6061";
				}
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
