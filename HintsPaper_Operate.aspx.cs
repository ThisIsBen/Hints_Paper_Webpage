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
	/// HintsPaper_Operate ���K�n�y�z�C
	/// </summary>
    public partial class HintsPaper_Operate : AuthoringTool_BasicForm_BasicForm
	{
		string strCaseID , strDivisionID , strStartTime , strOperationTime , strClinicNum , strSectionName;
		//string strClinicNum , strSectionName ;
		string strUserID;
		string strPaperID;
		string strPresentType = "";

		protected string strPaperName = "";
		string strObjective = "";
		string strEditMethod = "";
		string strGenerationMethod = "";
		string strPresentMethod = "";

		int intTestDuration = 0;
		int intQuestionNum = 0;//���D���ƶq�A�n�[�JHeader����줤�C
		int intQuestionIndex = 0;

		//�إ�SqlDB����
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
		DataReceiver myReceiver = new DataReceiver();
		RandomSelect myQuestionSelect = new RandomSelect();
		SQLString mySQLString = new SQLString();
		//HintsUtility.DBAccelerator dba = new HintsUtility.DBAccelerator(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);


		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();
            
			//�����Ѽ�(UserID , StartTime , CaseID , DivisionID , PaperID)
			this.getParameter();

			//�ˬd���ϥΪ̫إ�strOperationTime�r��
			if(this.IsPostBack != true)
			{
				strOperationTime = myReceiver.getNowTime();
				hiddenOperationTime.Value = strOperationTime;
			}

			//get the Paper Header from the database
			string strSQL = "";
			strSQL = "SELECT * FROM Paper_Header WHERE cPaperID = '" + strPaperID + "' ";

			DataSet dsHeader = sqldb.getDataSet(strSQL);
			if(dsHeader.Tables[0].Rows.Count != 0)
			{
				if(dsHeader.Tables[0].Rows[0]["cPaperName"] != DBNull.Value)
				{
					strPaperName = dsHeader.Tables[0].Rows[0]["cPaperName"].ToString();
				}

				if(dsHeader.Tables[0].Rows[0]["cObjective"] != DBNull.Value)
				{
					strObjective = dsHeader.Tables[0].Rows[0]["cObjective"].ToString();
				}

				if(dsHeader.Tables[0].Rows[0]["cEditMethod"] != DBNull.Value)
				{
					strEditMethod = dsHeader.Tables[0].Rows[0]["cEditMethod"].ToString();
				}

				if(dsHeader.Tables[0].Rows[0]["cGenerationMethod"] != DBNull.Value)
				{
					strGenerationMethod = dsHeader.Tables[0].Rows[0]["cGenerationMethod"].ToString();
				}

				if(dsHeader.Tables[0].Rows[0]["cPresentMethod"] != DBNull.Value)
				{
					strPresentMethod = dsHeader.Tables[0].Rows[0]["cPresentMethod"].ToString();
				}

				if(dsHeader.Tables[0].Rows[0]["sTestDuration"] != DBNull.Value)
				{
					intTestDuration = Convert.ToInt32(dsHeader.Tables[0].Rows[0]["sTestDuration"]);
				}
				
				if(strEditMethod == "System" && strGenerationMethod == "Present")
				{
					if(dsHeader.Tables[0].Rows[0]["sQuestionNum"] != DBNull.Value)
					{
						intQuestionNum = myReceiver.getQuestionCountFromGroupingQuestion(strPaperID);
					}
				}
				else
				{
					intQuestionNum = myReceiver.getPaperQuestionCount(strPaperID);
				}
			}
			dsHeader.Dispose();

			this.setupHintsQuestionTable();

			//Hints���ݨ��t��
            HtmlInputButton btnHints = new HtmlInputButton();
			btnHints.ID = "btnHints";
			phSubmit.Controls.Add(btnHints);
            btnHints.Value = this.GetMultiLanguageString("SubmitPaper");
            btnHints.ServerClick += new EventHandler(btnHints_ServerClick);
		}

		private void setupHintsQuestionTable()
		{
			Table table = new Table();
			
			table.CellSpacing = 0;
			table.CellPadding = 2;
			table.BorderStyle = BorderStyle.Solid;
			table.BorderWidth=Unit.Pixel(1);
			table.BorderColor = System.Drawing.Color.Black;
			table.GridLines = GridLines.None;
			table.Width = Unit.Percentage(100);

			intQuestionIndex = 0;

			//���X���ݨ�������D���e
			string strSQL = mySQLString.getPaperSelectionContent(strPaperID);
			DataSet dsQuestionList = sqldb.getDataSet(strSQL);
			if(dsQuestionList.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i<dsQuestionList.Tables[0].Rows.Count ; i++)
				{
					//���oQuestionType
					string strQuestionType = "1";
					try
					{
						strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();
					}
					catch
					{
					}

					//���oQID
					string strQID = "";
					try
					{
						strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
					}
					catch
					{
					}

					//�]�wSelection row��CSS�ҳ]�w���ܼ�
					int intQuestionCount = 0;
					
					//���o���D��SQL
					strSQL = mySQLString.getSingleQuestionInformation(strQID);
					DataSet dsQuestion = sqldb.getDataSet(strSQL);

					if(dsQuestion.Tables[0].Rows.Count > 0)
					{

						//�إ߰��D�����e
						TableRow trQuestion = new TableRow();
						table.Rows.Add(trQuestion);

						intQuestionIndex += 1;
						
						//���D���D��
						string strQuestionNum = "Q" + intQuestionIndex.ToString() + ": ";

						//���D�����e
						string strQuestion = "";
						try
						{
							strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();
						}
						catch
						{
						}
						TableCell tcQuestion = new TableCell();
						trQuestion.Cells.Add(tcQuestion);
						tcQuestion.Text = strQuestionNum + strQuestion;
						double dQuestionWidth = tcQuestion.Width.Value;

						//�إ߰��D��CSS
						trQuestion.Attributes.Add("Class","TableTitle");
						trQuestion.ForeColor = System.Drawing.Color.DarkRed;
						trQuestion.Font.Bold = true;

						//�إ߿ﶵ
						TableRow trSelection = new TableRow();
						table.Rows.Add(trSelection);
			
						int intColSpan = 0;
						strSQL = mySQLString.getAllSelections(strQID);
						DataSet dsSelection = sqldb.getDataSet(strSQL);
						if(dsSelection.Tables[0].Rows.Count > 0)
						{
							for(int j=0 ; j<dsSelection.Tables[0].Rows.Count ; j++)
							{
								intQuestionCount += 1;

								//SelectionID
								string strSelectionID = "";
								try
								{
									strSelectionID = dsSelection.Tables[0].Rows[j]["cSelectionID"].ToString();
								}
								catch
								{
								}

								//Seq
								string strSeq = "";
								try
								{
									strSeq = dsSelection.Tables[0].Rows[j]["sSeq"].ToString();
								}
								catch
								{
								}

								//Selection
								string strSelection = "";
								try
								{
									strSelection = dsSelection.Tables[0].Rows[j]["cSelection"].ToString();
								}
								catch
								{
								}

								//bCaseSelect
								bool bCaseSelect = false;
								try
								{
									bCaseSelect = Convert.ToBoolean(dsSelection.Tables[0].Rows[j]["bCaseSelect"]);
								}
								catch
								{
								}
								
								//�ﶵ���e�PRadioButton
								TableCell tcSelection = new TableCell();
								trSelection.Cells.Add(tcSelection);

								RadioButton rbSelection = new RadioButton();
								tcSelection.Controls.Add(rbSelection);
								rbSelection.Text = strSelection;
								rbSelection.GroupName = strQID;
								string strID = "rb-" + strQID + "-" + strSelectionID;
								rbSelection.ID = strID;
								try
								{
									if(Request.Form[strQID] != null)
									{
										if(Request.Form[strQID].ToString() == strID)
										{
											rbSelection.Checked = true;
										}
									}
								}
								catch
								{
								}
								intColSpan += 1;
							}
							tcQuestion.ColumnSpan = intColSpan;
						}
						else
						{
							//�����D�S���ﶵ
						}
						dsSelection.Dispose();
					}
					else
					{
						//�����D�S���ﶵ
					}
					dsQuestion.Dispose();
				}
			}
			else
			{
				//���ݨ��S������D�����p
			}
			dsQuestionList.Dispose();

			int intMaxColSpan = 0;

			//�վ�table��TableCell�e��
			int intSelectionIndex = 0;
			
			for(int i=0 ; i<table.Rows.Count ; i++)
			{
				if(i % 2 != 0)
				{
					for(int j=0 ; j<table.Rows[i].Cells.Count ; j++)
					{
						double dPercentage = Convert.ToDouble(100) / Convert.ToDouble(table.Rows[i].Cells.Count);
						//�N�C��SelectionCell���e�׳]��QuestionCell���e��/tcSelection���Ӽ�
						table.Rows[i].Cells[j].Width = Unit.Percentage(dPercentage);

						if(table.Rows[i].Cells.Count > intMaxColSpan)
						{
							intMaxColSpan = table.Rows[i].Cells.Count;
						}
					}
					//�إ߿ﶵ��CSS
					if((Convert.ToInt32(intSelectionIndex) % 2) != 0)
					{
						table.Rows[i].Attributes.Add("Class","TableOddRow");	
					}
					else
					{
						table.Rows[i].Attributes.Add("Class","TableEvenRow");	
					}
					intSelectionIndex += 1;
				}
			}

			//�إ��ݩ󦹰ݨ����ݵ��D
			strSQL = mySQLString.getPaperTextContent(strPaperID);
			DataSet dsTextList = sqldb.getDataSet(strSQL);
			int intTextCount = 0;
			if(dsTextList.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i<dsTextList.Tables[0].Rows.Count ; i++)
				{
					intQuestionIndex += 1;

					//���o�����D��QID
					string strQID = "";
					try
					{
						strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
					}
					catch
					{
					}

					TableRow trQuestion = new TableRow();
					table.Rows.Add(trQuestion);
					
					//Question number
					string strQuestionNum = "Q" + intQuestionIndex.ToString() + ": ";

					//Question
					TableCell tcQuestion = new TableCell();
					trQuestion.Cells.Add(tcQuestion);
					tcQuestion.ColumnSpan = intMaxColSpan;
					string strQuestion = "";
					try
					{
						strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
					}
					catch
					{
					}
					tcQuestion.Text = strQuestionNum + strQuestion;

					//�[�JCSS
					intTextCount += 1;
					trQuestion.Attributes.Add("Class","TableTitle");
					trQuestion.ForeColor = System.Drawing.Color.DarkRed;
					trQuestion.Font.Bold = true;

					//�[�J����TableRow
					TableRow trAnswer = new TableRow();
					table.Rows.Add(trAnswer);


					TableCell tcAnswer = new TableCell();
					trAnswer.Cells.Add(tcAnswer);
					tcAnswer.ColumnSpan = intMaxColSpan;

					//�[�JTextArea
					HtmlTextArea txtAnswer = new HtmlTextArea();
					tcAnswer.Controls.Add(txtAnswer);
					string strTxtID = "txt-" + strQID;
					txtAnswer.ID = strTxtID;
					txtAnswer.Cols = 80;
					txtAnswer.Rows = 5;
					txtAnswer.Style.Add("WIDTH","100%");
					try
					{
						if(Request.Form[strTxtID] != null)
						{
							txtAnswer.Value = Request.Form[strTxtID].ToString();
						}
					}
					catch
					{
					}

					hiddenTextID.Value += strTxtID + ";";
					hiddenTextCount.Value = Convert.ToString(Convert.ToInt32(hiddenTextCount.Value) + 1);

					//�M��CSS
					if((intTextCount % 2) != 0)
					{
						trAnswer.Attributes.Add("Class","TableOddRow");
					}
					else
					{
						trAnswer.Attributes.Add("Class","TableEvenRow");
					}
				}
			}
			else
			{
				//���ݨ��S���ݵ��D������
			}
			dsTextList.Dispose();

			//�[�JEmpty TableRow
			int a = 0;
			for(int i=table.Rows.Count-1 ; i>=1 ; i--)
			{
				if(a == 1)
				{
					TableRow tr1 = new TableRow();
					table.Rows.AddAt(i,tr1);

					TableCell tc1 = new TableCell();
					tr1.Cells.Add(tc1);
					tc1.ForeColor = System.Drawing.Color.Transparent;
					tc1.Font.Size = FontUnit.Parse("0");
					tc1.Text = "1";

					TableRow tr2 = new TableRow();
					table.Rows.AddAt(i,tr2);

					TableCell tc2 = new TableCell();
					tr2.Cells.Add(tc2);
					tc2.ForeColor = System.Drawing.Color.Transparent;
					tc2.Font.Size = FontUnit.Parse("0");
					tc2.Text = "1";

					a = 0;
				}
				else
				{
					a += 1;
				}
			}

			phQuestion.Controls.Clear();
			phQuestion.Controls.Add(table);
		}		

		private void getParameter()
		{
			//display=&UserID=wyt&UserLevel=1&StartTime=20051020124216&CaseID=gait001Case200401061006167084912&DivisionID=7501&ClinicNum=1&SectionName=Examination&CaseSelect=
			//�HQuery string �u��
			//UserID
			if(Request.QueryString["UserID"] != null)
			{
				strUserID = Request.QueryString["UserID"].ToString();
			}
			else
			{
				if(usi.UserID != null)
				{
                    strUserID = usi.UserID;
				}
				else
				{
					strUserID = "swakevin";
				}
			}
			

			//StartTime
			if(Request.QueryString["StartTime"] != null)
			{
				strStartTime = Request.QueryString["StartTime"].ToString();
			}
			else
			{
				if(usi.StartTime != null)
				{
                    strStartTime = usi.StartTime;
				}
				else
				{
					strStartTime = "20050906171257";
				}
			}

			//CaseID kyhCase200505301448128593750
			if(Request.QueryString["CaseID"] != null)
			{
				strCaseID = Request.QueryString["CaseID"].ToString();
			}
			else
			{
				if(usi.CaseID != null)
				{
                    strCaseID = usi.CaseID;
				}
				else
				{
					strCaseID = "kyhCase200505301448128593750";
				}
			}

			//Division 9801
			if(Request.QueryString["DivisionID"] != null)
			{
				strDivisionID = Request.QueryString["DivisionID"].ToString();
			}
			else
			{
				if(usi.Division != null)
				{
                    strDivisionID = usi.Division;
				}
				else
				{
					strDivisionID = "9801";
				}
			}

			//ClinicNum
			if(Request.QueryString["ClinicNum"] != null)
			{
				strClinicNum = Request.QueryString["ClinicNum"].ToString();
			}
			else
			{
				if(usi.ClinicNum != null)
				{
                    strClinicNum = usi.ClinicNum.ToString();
				}
				else
				{
					strClinicNum = "1";
				}
			}

			//SectionName
			if(usi.Section != null)
			{
				strSectionName = Request.QueryString["SectionName"].ToString();
			}
			else
			{
                if (usi.Section != null)
				{
                    strSectionName = usi.Section;
				}
				else
				{
					strSectionName = "Examination";
				}
			}

			//PresentType
			if(Request.QueryString["PresentType"] != null)
			{
				strPresentType = Request.QueryString["PresentType"].ToString();

				//�̷�PresentType���o�������ݨ�ID
				strPaperID = myReceiver.getPaperIDByPresentType(strPresentType , strCaseID , strDivisionID , strClinicNum , strSectionName);
			}
			else
			{
				//�p�G�S��PresentType�A�N�qSession���oPaperID
				if(Request.QueryString["PaperID"] != null)
				{
					strPaperID = Request.QueryString["PaperID"].ToString();
					if(usi.PaperID != null)
					{
                        usi.PaperID = strPaperID;
					}
				}
				else
				{
                    if (usi.PaperID != null)
					{
                        strPaperID = usi.PaperID;
					}
					else
					{
						strPaperID = "swakevin20050907120001";
					}
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

        void btnHints_ServerClick(object sender, EventArgs e)
        {
            //Hints���ݨ��t��

            //�ˬd���ݨ��O���O�C�Ӱ��D�����Q�^��
            bool bEmptyCheck = true;

            //���X���ݨ�������D���e
            string strSQL = mySQLString.getPaperSelectionContent(strPaperID);
            DataSet dsQuestionCheck = sqldb.getDataSet(strSQL);
            if (dsQuestionCheck.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsQuestionCheck.Tables[0].Rows.Count; i++)
                {
                    //���oQID
                    string strQID = "";
                    try
                    {
                        strQID = dsQuestionCheck.Tables[0].Rows[i]["cQID"].ToString();
                    }
                    catch
                    {
                    }

                    try
                    {
                        if (Request.Form[strQID] == null)
                        {
                            int intEmptyIndex = i + 1;

                            bEmptyCheck = false;

                            //�����D�S���Q����A���ĵ�i����ĵ�i�ϥΪ̡C
                            string strScript = "<script language='javascript'>\n";
                            strScript += "AlertSelectionNull('" + intEmptyIndex + "');\n";
                            strScript += "</script>\n";
                            Page.RegisterStartupScript("AlertSelectionNull", strScript);
                        }
                    }
                    catch
                    {
                        int intEmptyIndex = i + 1;

                        bEmptyCheck = false;

                        //�����D�S���Q����A���ĵ�i����ĵ�i�ϥΪ̡C
                        string strScript = "<script language='javascript'>\n";
                        strScript += "AlertSelectionNull('" + intEmptyIndex + "');\n";
                        strScript += "</script>\n";
                        Page.RegisterStartupScript("AlertSelectionNull", strScript);
                    }
                }
            }
            dsQuestionCheck.Dispose();

            //�p�G�C�@�Ӱ��D�����Q�^�����ܡA�h�N�C�Ӱ��D���^���s�J��Ʈw��
            if (bEmptyCheck == true)
            {
                //�إ�FinishTime
                string strFinishTime = "";
                try
                {
                    strFinishTime = myReceiver.getNowTime();
                }
                catch
                {
                }

                //���ѨϥΪ̬O�_���I�惡�ݨ���������@�Ӷ���
                bool bPaperCheck = false;

                //���X���ݨ�������D���e
                strSQL = mySQLString.getPaperSelectionContent(strPaperID);
                DataSet dsQuestionList = sqldb.getDataSet(strSQL);
                if (dsQuestionList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQuestionList.Tables[0].Rows.Count; i++)
                    {
                        //���oQuestionType
                        string strQuestionType = "1";
                        try
                        {
                            strQuestionType = dsQuestionList.Tables[0].Rows[i]["cQuestionType"].ToString();
                        }
                        catch
                        {
                        }

                        //���oQID
                        string strQID = "";
                        try
                        {
                            strQID = dsQuestionList.Tables[0].Rows[i]["cQID"].ToString();
                        }
                        catch
                        {
                        }

                        //StandardScore
                        int intStandardScore = 0;
                        try
                        {
                            intStandardScore = Convert.ToInt32(dsQuestionList.Tables[0].Rows[i]["sStandardScore"]);
                        }
                        catch
                        {
                        }

                        //QuestionMode
                        string strQuestionMode = "";
                        try
                        {
                            strQuestionMode = dsQuestionList.Tables[0].Rows[i]["cQuestionMode"].ToString();
                        }
                        catch
                        {
                        }

                        //Seq
                        string strQuestionSeq = "";
                        try
                        {
                            strQuestionSeq = dsQuestionList.Tables[0].Rows[i]["sSeq"].ToString();
                        }
                        catch
                        {
                        }

                        //���o���D��SQL
                        strSQL = mySQLString.getSingleQuestionInformation(strQID);
                        DataSet dsQuestion = sqldb.getDataSet(strSQL);

                        if (dsQuestion.Tables[0].Rows.Count > 0)
                        {
                            intQuestionIndex += 1;

                            //���D�����e
                            string strQuestion = "";
                            try
                            {
                                strQuestion = dsQuestion.Tables[0].Rows[0]["cQuestion"].ToString();
                            }
                            catch
                            {
                            }

                            //Question Level
                            string strQuestionLevel = "";
                            try
                            {
                                strQuestionLevel = dsQuestion.Tables[0].Rows[0]["sLevel"].ToString();
                            }
                            catch
                            {
                            }

                            //QuestionGroupID
                            string strQuestionGroupID = "";
                            try
                            {
                                strQuestionGroupID = dsQuestion.Tables[0].Rows[0]["cQuestionGroupID"].ToString();
                            }
                            catch
                            {
                            }

                            //�ˬd�����D�O���O���Q�I��
                            bool bQuestionSelect = false;

                            strSQL = mySQLString.getAllSelections(strQID);
                            DataSet dsSelection = sqldb.getDataSet(strSQL);
                            if (dsSelection.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsSelection.Tables[0].Rows.Count; j++)
                                {
                                    //SelectionID
                                    string strSelectionID = "";
                                    try
                                    {
                                        strSelectionID = dsSelection.Tables[0].Rows[j]["cSelectionID"].ToString();
                                    }
                                    catch
                                    {
                                    }

                                    //Seq
                                    string strSelectionSeq = "";
                                    try
                                    {
                                        strSelectionSeq = dsSelection.Tables[0].Rows[j]["sSeq"].ToString();
                                    }
                                    catch
                                    {
                                    }

                                    //Selection
                                    string strSelection = "";
                                    try
                                    {
                                        strSelection = dsSelection.Tables[0].Rows[j]["cSelection"].ToString();
                                    }
                                    catch
                                    {
                                    }

                                    //bCaseSelect
                                    bool bCaseSelect = false;
                                    try
                                    {
                                        bCaseSelect = Convert.ToBoolean(dsSelection.Tables[0].Rows[j]["bCaseSelect"]);
                                    }
                                    catch
                                    {
                                    }

                                    string strCaseSelect = "0";
                                    if (bCaseSelect == false)
                                    {
                                        strCaseSelect = "0";
                                    }
                                    else
                                    {
                                        strCaseSelect = "1";
                                    }

                                    //SelectState
                                    string strSelectState = "";

                                    bool bCheck = false;
                                    try
                                    {
                                        bCheck = ((RadioButton)(this.FindControl("Form1").FindControl("rb-" + strQID + "-" + strSelectionID))).Checked;
                                    }
                                    catch
                                    {
                                    }

                                    if (bCheck == true)
                                    {
                                        strSelectState = "1";
                                    }
                                    else
                                    {
                                        strSelectState = "0";
                                    }


                                    //�p�G�ϥΪ̦��I��h�h���O�_������o�@���D��
                                    //�P�_�o�@�ӿﶵ�ϥΪ̬O�_������
                                    if (strCaseSelect == "0")
                                    {
                                        if (strCaseSelect == strSelectState)
                                        {
                                            //���O��ĳ�ﶵ�A���O�ϥΪ̤]�S���I��A�ҥH���x�s��TempLog
                                        }
                                        else
                                        {
                                            //�ϥΪ̦��I�惡����
                                            //�N��Ʀs�JTempLog_PaperSelectionAnswer
                                            strSelectState = "1";
                                            bQuestionSelect = true;
                                            bPaperCheck = true;

                                            //���Ʀs�JTempLog_PaperSelectionAnswer
                                            mySQLString.SaveToTempLog_PaperSelectionAnswer(strPaperID, strStartTime, strUserID, strQID, strSelectionID, strSelectionSeq, strSelection, strCaseSelect);
                                        }
                                    }
                                    else
                                    {
                                        if (strCaseSelect != strSelectState)
                                        {
                                            //�O��ĳ�ﶵ�A���O�ϥΪ̨S���I��A�ҥHSelectState = 2
                                            //�N��Ʀs�JTempLog_PaperSelectionAnswer
                                            strSelectState = "2";
                                        }
                                        else
                                        {
                                            //�O��ĳ�ﶵ�A�ϥΪ̦��I��A�ҥHSelectState = 1
                                            //�N��Ʀs�JTempLog_PaperSelectionAnswer
                                            strSelectState = "1";
                                            bQuestionSelect = true;
                                            bPaperCheck = true;

                                            //���Ʀs�JTempLog_PaperSelectionAnswer
                                            mySQLString.SaveToTempLog_PaperSelectionAnswer(strPaperID, strStartTime, strUserID, strQID, strSelectionID, strSelectionSeq, strSelection, strCaseSelect);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //�����D�S���ﶵ
                            }
                            dsSelection.Dispose();

                            if (bQuestionSelect == true)
                            {
                                //���Ʀs�JTempLog_PaperSelectionQuestion
                                mySQLString.SaveToTempLog_PaperSelectionQuestion(strPaperID, strStartTime, strUserID, strQID, strQuestionLevel, strQuestion);
                            }
                        }
                        else
                        {
                            //�����D�S���ﶵ
                        }
                        dsQuestion.Dispose();

                        //�p�G�ϥΪ̦��I�惡�ݨ������@�Ӷ��ءA�h�s�W�@����Ʀ�TempLog_PaperHeader
                        if (bPaperCheck == true)
                        {
                            mySQLString.SaveToTempLog_PaperHeader(strPaperID, strStartTime, strUserID, hiddenOperationTime.Value, strFinishTime);
                        }

                    }
                }
                else
                {
                    //���ݨ��S������D�����p
                }
                dsQuestionList.Dispose();


                //�إ��ݩ󦹰ݨ����ݵ��D
                strSQL = mySQLString.getPaperTextContent(strPaperID);
                DataSet dsTextList = sqldb.getDataSet(strSQL);
                if (dsTextList.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsTextList.Tables[0].Rows.Count; i++)
                    {
                        //���o�����D��QID
                        string strQID = "";
                        try
                        {
                            strQID = dsTextList.Tables[0].Rows[i]["cQID"].ToString();
                        }
                        catch
                        {
                        }

                        //Question number
                        string strQuestionNum = "Q" + intQuestionIndex.ToString() + ": ";

                        //Question
                        string strQuestion = "";
                        try
                        {
                            strQuestion = dsTextList.Tables[0].Rows[i]["cQuestion"].ToString();
                        }
                        catch
                        {
                        }

                        //���oTextArea�����e
                        string strAnswer = "";
                        try
                        {
                            strAnswer = ((HtmlTextArea)(this.FindControl("Form1").FindControl("txt-" + strQID))).InnerText;
                        }
                        catch
                        {
                        }

                        //StandardScore
                        int intStandardScore = 0;
                        try
                        {
                            intStandardScore = Convert.ToInt32(dsTextList.Tables[0].Rows[i]["sStandardScore"]);
                        }
                        catch
                        {
                        }

                        //�p�GTextArea�����e���O�Ū��ܡA�h�s�J��Ʈw�C
                        if (strAnswer.Length > 0)
                        {
                            bPaperCheck = true;

                            //���Ʀs�JTempLog_PaperText
                            mySQLString.SaveToTempLog_PaperTextQuestion(strPaperID, strStartTime, strUserID, strQID, strQuestion, strAnswer);
                        }
                    }
                }
                else
                {
                    //���ݨ��S���ݵ��D������
                }
                dsTextList.Dispose();

                //�p�G�ϥΪ̦��I�惡�ݨ������@�Ӷ��ءA�h�s�W�@����Ʀ�TempLog_PaperHeader
                if (bPaperCheck == true)
                {
                    mySQLString.SaveToTempLog_PaperHeader(strPaperID, strStartTime, strUserID, hiddenOperationTime.Value, strFinishTime);
                }

                //��������
                string strScript = "<script language='javascript'>\n";
                strScript += "closeWindow();\n";
                strScript += "</script>\n";
                Page.RegisterStartupScript("closeWindow", strScript);
            }
            else
            {
                //�����D�S���^��������
                //phQuestion.Controls.Clear();
                this.setupHintsQuestionTable();
            }
        }
	}
}
