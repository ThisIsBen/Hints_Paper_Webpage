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
	/// ���CaseTree or CaseSection tree�A�éI�s�x�s���������Ʀs�JPaper_CaseDivisionSection 
	/// </summary>
    public partial class TreeView : AuthoringTool_BasicForm_BasicForm
	{
		string strUserLevel = "1";
		string strPresentType = "Case";

		//�إ�SqlDB����
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			//�إ�Case tree
			this.setupCaseTree();
		}

		private void setupCaseSectionTree()
		{
			//�إ�tree�����njavascript
			string NodeSc="<SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/JSCookTree.js'></SCRIPT><link REL='stylesheet' HREF='../Translate tree/theme.css' TYPE='text/css'><SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/theme.js'></SCRIPT>";
			Page.RegisterStartupScript("NodeSc",NodeSc);

			//�إ�tree��string
			string NodeStr="[";

			//---------------------------------------���oCaseTree���C�@��Node----------------------------------------
			string strSQL = "";

			//-----------------------------�̷Ӥ��P��ViewType�ӨM�wCaseTree���n��ܪ����e----------------------------
			strSQL = "SELECT DISTINCT Department.cDepartmentName , Department.cDepartmentNameIndex, Division.cDepartmentID FROM Department INNER JOIN Division ON Department.cDepartmentID = Division.cDepartmentID INNER JOIN PresentServer ON Division.cDivisionID = PresentServer.cDivisionID ORDER BY cDepartmentNameIndex";
			
			//-------------------------------------------�NCaseTreeView����Node�[��TreeView��------------------------------------

			//-----------------------------------------------�إߤ@��Hospital Node-----------------------------------------------
			NodeStr = NodeStr + "[null,'"+"Hospital"+"',null,'_self',null";
			
			//--------------------------------------�إ�Hospital���lNode�ADepartmentNode-------------------------------------
			DataSet dsDepartment=sqldb.getDataSet(strSQL);
			int DepartmentCounter = 0;
			string strDepartmentCounter = "";
			foreach (DataRow drDepartmentName in dsDepartment.Tables[0].Rows)
			{
				DepartmentCounter = DepartmentCounter + 1;
				strDepartmentCounter = DepartmentCounter.ToString();
				NodeStr = NodeStr +",[null,'" + drDepartmentName["cDepartmentName"].ToString().Trim() + "',null,null,null";

				//�إ�Department���lNode�ADivisionNode				
				string strDivisionSQL = "";
				strDivisionSQL = "SELECT Distinct Division.cDivisionName AS DivisionName , Division.cDivisionID AS DivisionID FROM Division INNER JOIN PresentServer ON Division.cDivisionID = PresentServer.cDivisionID INNER JOIN UserDivision ON Division.cDivisionID = UserDivision.cDivisionID INNER JOIN HintsUser ON UserDivision.cUserID = HintsUser.cUserID WHERE PresentServer.sUseLevel IN (0,'1') AND Division.cDepartmentID ='" + drDepartmentName["cDepartmentID"].ToString() + "' ORDER BY DivisionName";
				DataSet dsDivision=sqldb.getDataSet(strDivisionSQL);
				foreach (DataRow drDivisionName in dsDivision.Tables[0].Rows)
				{
					NodeStr = NodeStr + ",[null,'" + drDivisionName[0].ToString().Trim() + "',null,'_self',null";
					//�إ�Division���lNode�ACaseNode
					string strCaseSQL = "SELECT DISTINCT TeachingCase.cCaseID, TeachingCase.cCaseName, DivIDtoURL.cURL AS cServer, TeachingCase.cURL FROM PresentServer INNER JOIN TeachingCase ON PresentServer.cCaseID = TeachingCase.cCaseID INNER JOIN DivIDtoURL ON PresentServer.cDivisionID = DivIDtoURL.cDivisionID WHERE PresentServer.cDivisionID='" + drDivisionName["DivisionID"].ToString() + "' ORDER BY cCaseName";
					DataSet dsCase=sqldb.getDataSet(strCaseSQL);
					
					foreach (DataRow drCaseName in dsCase.Tables[0].Rows)
					{
						string strCaseID = drCaseName["cCaseID"].ToString();
						//strCaseName = drCaseName["cCaseName"].ToString();
						string strDivisionID = drDivisionName["DivisionID"].ToString();
						
						NodeStr = NodeStr + ",[null,'" + (drCaseName["cCaseName"].ToString()).Trim() + "',null,'_self',null";

						//�إ�Visit
						strSQL = "SELECT MAX(sClinicNum) AS 'MaxClinic' FROM UserLevelPresent WHERE cUserLevel = '"+strUserLevel+"' AND cCaseID = '"+strCaseID+"' AND cDivisionID = '"+strDivisionID+"' ";
						DataSet dsVisit = sqldb.getDataSet(strSQL);
						if(dsVisit.Tables[0].Rows.Count > 0)
						{
							for(int i=0 ; i<dsVisit.Tables[0].Rows.Count ; i++)
							{
								//get visit number
								int intVisit = 1;
								try
								{
									intVisit = Convert.ToInt32(dsVisit.Tables[0].Rows[i]["sClinicNum"]);
								}
								catch
								{
								}

								NodeStr = NodeStr + ",[null,'" + intVisit.ToString() + "',null,'_self',null";

								//�إ�Section
								strSQL = "SELECT cSectionName FROM UserLevelPresent WHERE cUserLevel = '"+strUserLevel+"' AND cCaseID = '"+strCaseID+"' AND cDivisionID = '"+strDivisionID+"' AND sClinicNum = '"+intVisit.ToString()+"' ORDER BY sSectionSeq ";
								DataSet dsSection = sqldb.getDataSet(strSQL);
								if(dsSection.Tables[0].Rows.Count > 0)
								{
									for(int j=0 ; j<dsSection.Tables[0].Rows.Count ; j++)
									{
										//get section name
										string strSectionName = "";
										try
										{
											strSectionName = dsSection.Tables[0].Rows[j]["cSectionName"].ToString();
										}
										catch
										{
										}

										NodeStr = NodeStr + ",[null,'" + strSectionName + "',null,'_self',null";

										NodeStr = NodeStr + "]";
									}
								}
								dsSection.Dispose();

								NodeStr = NodeStr + "]";
							}
						}
						dsVisit.Dispose();

						NodeStr = NodeStr + "]";
					}
					dsCase.Dispose();
					//�����إ�CaseNode
					NodeStr = NodeStr + "]";
				}
				dsDivision.Dispose();
				//�����إ�DivisionNode
				NodeStr = NodeStr + "]";

			}
			dsDepartment.Dispose();

			//�����NTreeNode�[��TreeView��
			string NodePut = "<Font size='26'><script language='JavaScript'>var myMenu=" + NodeStr + "]]</script></Font>";
			NodePut=NodePut + "<DIV ID=myMenuID  align='left' ></DIV>";
			NodePut = NodePut + "<SCRIPT LANGUAGE='JavaScript'>ctDraw ('myMenuID', myMenu, ctThemeXP2, '../Translate tree/ThemeXP', 1, 3);</SCRIPT>";
			Page.RegisterStartupScript("NodePut",NodePut);
		}

		private void setupCaseTree()
		{
			//�إ�tree�����njavascript
			string NodeSc="<SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/JSCookTree.js'></SCRIPT><link REL='stylesheet' HREF='../Translate tree/theme.css' TYPE='text/css'><SCRIPT LANGUAGE='JavaScript' SRC='../Translate tree/theme.js'></SCRIPT>";
			Page.RegisterStartupScript("NodeSc",NodeSc);

			//�إ�tree��string
			string NodeStr="[";

			//---------------------------------------���oCaseTree���C�@��Node----------------------------------------
			string strSQL = "";

			//-----------------------------�̷Ӥ��P��ViewType�ӨM�wCaseTree���n��ܪ����e----------------------------
			strSQL = "SELECT DISTINCT Department.cDepartmentName , Department.cDepartmentNameIndex, Division.cDepartmentID FROM Department INNER JOIN Division ON Department.cDepartmentID = Division.cDepartmentID INNER JOIN PresentServer ON Division.cDivisionID = PresentServer.cDivisionID ORDER BY cDepartmentNameIndex";
			
			//-------------------------------------------�NCaseTreeView����Node�[��TreeView��------------------------------------

			//-----------------------------------------------�إߤ@��Hospital Node-----------------------------------------------
			NodeStr = NodeStr + "[null,'"+"Hospital"+"',null,'_self',null";
			
			//--------------------------------------�إ�Hospital���lNode�ADepartmentNode-------------------------------------
			DataSet dsDepartment=sqldb.getDataSet(strSQL);
			int DepartmentCounter = 0;
			string strDepartmentCounter = "";
			foreach (DataRow drDepartmentName in dsDepartment.Tables[0].Rows)
			{
				DepartmentCounter = DepartmentCounter + 1;
				strDepartmentCounter = DepartmentCounter.ToString();
				NodeStr = NodeStr +",[null,'" + drDepartmentName["cDepartmentName"].ToString().Trim() + "',null,null,null";

				//�إ�Department���lNode�ADivisionNode				
				string strDivisionSQL = "";
				strDivisionSQL = "SELECT Distinct Division.cDivisionName AS DivisionName , Division.cDivisionID AS DivisionID FROM Division INNER JOIN PresentServer ON Division.cDivisionID = PresentServer.cDivisionID INNER JOIN UserDivision ON Division.cDivisionID = UserDivision.cDivisionID INNER JOIN HintsUser ON UserDivision.cUserID = HintsUser.cUserID WHERE PresentServer.sUseLevel IN (0,'1') AND Division.cDepartmentID ='" + drDepartmentName["cDepartmentID"].ToString() + "' ORDER BY DivisionName";
				DataSet dsDivision=sqldb.getDataSet(strDivisionSQL);
				foreach (DataRow drDivisionName in dsDivision.Tables[0].Rows)
				{
					NodeStr = NodeStr + ",[null,'" + drDivisionName[0].ToString().Trim() + "',null,'_self',null";
					//�إ�Division���lNode�ACaseNode
					string strCaseSQL = "SELECT DISTINCT TeachingCase.cCaseID, TeachingCase.cCaseName, DivIDtoURL.cURL AS cServer, TeachingCase.cURL FROM PresentServer INNER JOIN TeachingCase ON PresentServer.cCaseID = TeachingCase.cCaseID INNER JOIN DivIDtoURL ON PresentServer.cDivisionID = DivIDtoURL.cDivisionID WHERE PresentServer.cDivisionID='" + drDivisionName["DivisionID"].ToString() + "' ORDER BY cCaseName";
					DataSet dsCase=sqldb.getDataSet(strCaseSQL);
					
					foreach (DataRow drCaseName in dsCase.Tables[0].Rows)
					{
						//strCaseID = drCaseName["cCaseID"].ToString();
						//strCaseName = drCaseName["cCaseName"].ToString();
						//strDivisionID = drDivisionName["DivisionID"].ToString();
						
						NodeStr = NodeStr + ",[null,'" + drCaseName["cCaseName"].ToString().Trim() + "','SectionTreeView.aspx?CaseID=" + drCaseName["cCaseID"].ToString().Trim() + "&DivisionID=" + drDivisionName["DivisionID"].ToString().Trim() + "','frmMain',null";
						NodeStr = NodeStr + "]";
					}
					dsCase.Dispose();
					//�����إ�CaseNode
					NodeStr = NodeStr + "]";
				}
				dsDivision.Dispose();
				//�����إ�DivisionNode
				NodeStr = NodeStr + "]";

			}
			dsDepartment.Dispose();

			//�����NTreeNode�[��TreeView��
			string NodePut = "<Font size='26'><script language='JavaScript'>var myMenu=" + NodeStr + "]]</script></Font>";
			NodePut=NodePut + "<DIV ID=myMenuID  align='left' ></DIV>";
			NodePut = NodePut + "<SCRIPT LANGUAGE='JavaScript'>ctDraw ('myMenuID', myMenu, ctThemeXP2, '../Translate tree/ThemeXP', 1, 3);</SCRIPT>";
			Page.RegisterStartupScript("NodePut",NodePut);
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
