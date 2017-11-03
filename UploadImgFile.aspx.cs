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
	/// UploadImgFile ���K�n�y�z�C
	/// </summary>
    public partial class UploadImgFile : AuthoringTool_BasicForm_BasicForm
	{
		string strCaseID , strDivisionID , strClinicNum , strSectionName , strUserID , strPaperID;

		//Setup objects
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
		DataReceiver myReceiver = new DataReceiver();

	
		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			//�����Ѽ�
			this.getParameter();

			//�[�J�ƥ�
            btnUpload.ServerClick += new EventHandler(btnUpload_ServerClick);
		}

		private void getParameter()
		{
			//UserID
			if(Session["UserID"] != null)
			{
				strUserID = Session["UserID"].ToString();
			}
			//strUserID = "swakevin";
			
			//CaseID
			if(Session["CaseID"] != null)
			{
				strCaseID = Session["CaseID"].ToString();
			}
			
			//Division
			if(Session["DivisionID"] != null)
			{
				strDivisionID = Session["DivisionID"].ToString();
			}
			
			//ClinicNum
			if(Session["ClinicNum"] != null)
			{
				strClinicNum = Session["ClinicNum"].ToString();
			}
			
			//SectionName
			if(Session["SectionName"] != null)
			{
				strSectionName = Session["SectionName"].ToString();
			}

			//PaperID
			if(Session["PaperID"] != null)
			{
				strPaperID = Session["PaperID"].ToString();
			}
			else
			{
				SQLString mySQL = new SQLString();
				//strPaperID = mySQL.getPaperIDFromCase(strCaseID , strClinicNum.ToString() , strSectionName);
			}
			//strPaperID = "wyt20060510150619";
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

        void btnUpload_ServerClick(object sender, EventArgs e)
        {
            if (fileinput.PostedFile != null)
            {
                //�T�{�W�Ǫ��ɮ׫��A�O�w����Image(bmp,jpg,png......)

                //�إ��ɦW
                string strFileName = strUserID + "-" + myReceiver.getNowTime();

                //���oCase�����
                string strSQL = "";
                SQLString mySQL = new SQLString();
                strSQL = mySQL.getCaseFolder(strCaseID);
                DataSet dsCase = sqldb.getDataSet(strSQL);
                if (dsCase.Tables[0].Rows.Count > 0)
                {

                    //�ɮ��x�s��Ƨ������|
                    //string strFolder = @"C:\WebShare\HintsData\HintsCase\VideoFileOfPathologyCut\";
                    string strFolder = "";
                    try
                    {
                        strFolder = dsCase.Tables[0].Rows[0]["cURL"].ToString();
                    }
                    catch
                    {
                    }

                    //��Case���ݪ�Division

                    try
                    {
                        strDivisionID = dsCase.Tables[0].Rows[0]["cDivisionID"].ToString();
                    }
                    catch
                    {
                    }

                    //���o��Case��Server
                    string strServer = DataReceiver.getDomainNameBySplitingURL(this);

                    dsCase.Dispose();

                    //���oContentType
                    string strContentType = "";
                    try
                    {
                        strContentType = fileinput.PostedFile.ContentType;
                    }
                    catch
                    {
                    }

                    //�̷�ContentType�إߤ��P��������|
                    switch (strContentType)
                    {
                        case "video/x-ms-wmv":
                            strFileName += ".wmv";
                            break;
                        case "audio/wav":
                            strFileName += ".wav";
                            break;
                        case "video/mpeg":
                            strFileName += ".mpeg";
                            break;
                        case "video/avi":
                            strFileName += ".avi";
                            break;
                        case "video/x-ms-asf":
                            strFileName += ".asf";
                            break;
                        case "image/gif":
                            strFileName += ".gif";
                            break;
                        case "image/jpg":
                            strFileName += ".jpg";
                            break;
                        case "image/jpeg":
                            strFileName += ".jpg";
                            break;
                        case "image/pjpeg":
                            strFileName += ".jpg";
                            break;
                        case "image/bmp":
                            strFileName += ".bmp";
                            break;
                        case "image/x-png":
                            strFileName += ".png";
                            break;
                        default:
                            strFileName += ".jpg";
                            break;
                    }

                    //�إߧ����ɮ�URL******�`�N!!�U����|����WebShare�b�W�Ǧ�Server�ݮɭn�O�o�ק令Web_Share******
                    string strFileURL = "";
                    try
                    {
                        strFileURL = @"C:\Web_Share\HintsData" + strFolder + @"\" + strFileName;
                        fileinput.PostedFile.SaveAs(strFileURL);
                    }
                    catch
                    {
                        strFileURL = "";
                        strFileURL = @"D:\Web_Share\HintsData" + strFolder + @"\" + strFileName;
                        fileinput.PostedFile.SaveAs(strFileURL);
                    }

                    //��Server�ݪ��ɮצ�m�ഫ���������۹��m
                    string strImgSrc = "";
                    strFolder = strFolder.Replace(@"\", "/");
                    strImgSrc = "http://" + strServer + strFolder + "/" + strFileName;

                    strImgSrc = "<IMG src=" + strImgSrc + ">";

                    //�����������A�ñN�W�Ǫ��Ϥ��[�JMMD��imgDiv���C(�ǤJServer��m�����})
                    string strScript = "<script language='javascript'>\n";
                    strScript += "closePage('" + strImgSrc + "')\n";
                    strScript += "</script>\n";
                    Page.RegisterStartupScript("closePage", strScript);

                }
                else
                {
                    //��Case�S�����
                }
            }
        }
	}
}
