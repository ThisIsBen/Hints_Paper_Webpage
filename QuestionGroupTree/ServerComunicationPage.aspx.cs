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

namespace WebApplication1
{
	/// <summary>
	/// �������B�z"Mt_Favorite.aspx"���Ʈw���������q
	/// </summary>
	public partial class ServerComunicationPage : System.Web.UI.Page
	{
		private string HandleType = "";//�n�B�z���Ʊ����A
	    private string NodeID = "";//�n�B�z���`�IID
		private string Parameter = "";//�n�B�z���`�I�Ѽ�,���n�B�z���Ʊ����A���P�Ӧ����P���N�q,�Ҧp�Y�n�ק�@�Ӹ�Ƨ�(�`�I)�W��,�hParameter�N�O�s�W��
        private SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
		private string Directory_DataTable = "QuestionGroupTree";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			init();
			ProcessHandleType();
		}

		/// <summary>
		/// �NQueryString�]�w�������������O�ݩ�
		/// </summary>
		private void init()
		{
			HandleType = Request.QueryString["HandleType"] + "";
			NodeID = Request.QueryString["NodeID"] + "";
			Parameter = Request.QueryString["Parameter"] + "";
		}

		/// <summary>
		/// �B�zHandleType�n�@���Ʊ�
		/// </summary>
		private void ProcessHandleType()
		{
			switch(HandleType)
			{
				case "Update_Folder_Name":
					Update_Folder_Name();
					break;
			}
		}

		/// <summary>
		/// ��s��Ƨ����W��
		/// </summary>
		private void Update_Folder_Name()
		{
			string strSQL = "SELECT * FROM " + Directory_DataTable + " WHERE cNodeID='"+NodeID+"'";
			DataTable dt = sqldb.getDataSet(strSQL).Tables[0];
			DataRow dr = dt.Rows[0];
			dr["cNodeName"] = Parameter;
			sqldb.Update(dt,"SELECT * FROM " + Directory_DataTable);
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
