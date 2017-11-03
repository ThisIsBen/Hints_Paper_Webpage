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
using Ajax;

namespace AuthoringTool.CommonQuestionEdit
{
	/// <summary>
	/// RemotingScripting ���K�n�y�z�C
	/// </summary>
	public partial class RemotingScripting : System.Web.UI.Page
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
		}

		[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public void ChangeTextBoxContentForQuestion(string strQID,string strQuestionText)
		{
            ((QuestionAccessor)this.Page.Session["QuestionAccessor"]).modifyQuestionText(strQID, strQuestionText);
		}

		[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public void chkbox_CheckedChanged(string strControlID,bool IsCheck)
		{
			string tmpQID =	strControlID.Split('@')[1].Split('#')[0];  					//���o�n�ﶵ���ݪ����D��ID
			string tmpSelectionID = strControlID.Split('@')[1].Split('#')[1];           //���o�ﶵID
			((QuestionSelectionAccessor)Session["QuestionSelectionAccessor"]).modifySelectionCorrect(tmpQID,tmpSelectionID,IsCheck);
		}

		[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public void ChangeTextBoxContentForSelection(string strSelectionID,string strSelectionText)
		{
			string strQID = ((QuestionSelectionAccessor)this.Page.Session["QuestionSelectionAccessor"]).GetQIDBySelectionID(strSelectionID);
			((QuestionSelectionAccessor)this.Page.Session["QuestionSelectionAccessor"]).modifySelectionText(strQID,strSelectionID,strSelectionText);
		}

		[Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
		public void ChangeTextBoxContentForSelectionRespons(string strSelectionID,string strResponseText)
		{
			string strQID = ((QuestionSelectionAccessor)this.Page.Session["QuestionSelectionAccessor"]).GetQIDBySelectionID(strSelectionID);
			((QuestionSelectionAccessor)this.Page.Session["QuestionSelectionAccessor"]).modifySelectionResponseText(strQID,strSelectionID,strResponseText);
		}

        [Ajax.AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public void ChangeTextBoxContentForKeyWords(string strQID, string strKeyWordsText)
        {
            ((QuestionAccessor)this.Page.Session["QuestionAccessor"]).modifyKeyWordsText(strQID, strKeyWordsText);
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
