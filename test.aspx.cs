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
	/// test ���K�n�y�z�C
	/// </summary>
    public partial class test : AuthoringTool_BasicForm_BasicForm
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			//CONTOURMANIPULATELib.ContourManipulateClass b = new ContourManipulateClass();
			//b.AboutBox();
			
//			try
//			{
//				int i = b.TestCall();
//			}
//			catch(System.Runtime.InteropServices.COMException ex)
//			{
//				ex.HelpLink.ToString();
////				(System.Runtime.InteropServices.COMException(e))
//			}
			//b.GradeContours("0,0;0,9;9,9;9,0;|","0,0;0,9;9,9;9,0;|",1,1);
			// �b�o�̩�m�ϥΪ̵{���X�H��l�ƺ���
			CheckBox ch = new CheckBox();
			this.FindControl("Form1").Controls.Add(ch);
			ch.Text = "Test";
			ch.ID = "ch-Test";
			
			Button btnSubmit = new Button();
			this.FindControl("Form1").Controls.Add(btnSubmit);
			btnSubmit.ID = "btnSubmit";
			btnSubmit.Text = "Submit";
			btnSubmit.Click+=new EventHandler(btnSubmit_Click);
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

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			bool bCheckBox = ((CheckBox)(this.FindControl("Form1").FindControl("ch-Test"))).Checked;
		}
	}
}
