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
	/// HintsPaper_StatisticPresent ���K�n�y�z�C
	/// </summary>
    public partial class HintsPaper_StatisticPresent : AuthoringTool_BasicForm_BasicForm
	{	
		//�إ�SqlDB����
		SqlDB sqldb = new SqlDB(System.Configuration.ConfigurationSettings.AppSettings["connstr"]);
		DataReceiver myReceiver = new DataReceiver();
		SQLString mySQL = new SQLString();

		string strPaperID , strFunction , strCaseID , strAuthorID , strClass , strGroup;
		int intQuestionNum = 0;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            this.Initiate();

			//�����Ѽ�
			this.getParameter();

			//�إ߿���D��table
			Table table = new Table();
			tdSelectionTable.Controls.Add(table);
			table.Attributes.Add("Class","header1_table");
			table.Style.Add("WIDTH","100%");
            table.GridLines = GridLines.Vertical;

			//�إ߿���D���t�@�اe�{�覡�����
			Table tableOutput = new Table();
			tcOutputTable.Controls.Add(tableOutput);
			tableOutput.Attributes.Add("Class","header1_table");
			tableOutput.Style.Add("WIDTH","100%");

			//�إ�Output table ��Title
			TableRow trTitle = new TableRow();
			tableOutput.Rows.Add(trTitle);
			trTitle.CssClass = "header1_table_first_row";

			//Question
			TableCell tcQuestionTitle = new TableCell();
			trTitle.Cells.Add(tcQuestionTitle);
			tcQuestionTitle.Text = "Questions";

			//�D�`�P�N
			TableCell tcTitle1 = new TableCell();
			trTitle.Cells.Add(tcTitle1);
			tcTitle1.Text = "�D�`�P�N";

			//�P�N
			TableCell tcTitle2 = new TableCell();
			trTitle.Cells.Add(tcTitle2);
			tcTitle2.Text = "�P�N";

			//���q
			TableCell tcTitle3 = new TableCell();
			trTitle.Cells.Add(tcTitle3);
			tcTitle3.Text = "���q";

			//���P�N
			TableCell tcTitle4 = new TableCell();
			trTitle.Cells.Add(tcTitle4);
			tcTitle4.Text = "���P�N";

			//�D�`���P�N
			TableCell tcTitle5 = new TableCell();
			trTitle.Cells.Add(tcTitle5);
			tcTitle5.Text = "�D�`���P�N";

			//�b��椤�إ߿���D
			setupSelectionContent(table , tableOutput);
            tableOutput.GridLines = GridLines.Vertical;

			//�إ߰ݵ��D��Table
			Table tableText = new Table();
			tdTextTable.Controls.Add(tableText);
			tableText.Attributes.Add("Class","header1_table");
			tableText.Style.Add("WIDTH","100%");
            tableText.GridLines = GridLines.Vertical;

			//�b��椤�إ߰ݵ��D
			setupTextContent(tableText);
			
			//�إ�Duration��Table
			Table tableDuration = new Table();
			tdDurationTable.Controls.Add(tableDuration);
			tableDuration.Attributes.Add("Class","header1_table");
			tableDuration.Style.Add("WIDTH","100%");
            tableDuration.GridLines = GridLines.Vertical;

			//�b��椤�إ�Duration
			setupDurationContent(tableDuration);
		}

		private void setupDurationContent(Table table)
		{
			//�̷Ӥ��PFunction�I�s���P��Function
			string strSQL = "";
			switch (strFunction)
			{
				case "0":
					//Author,Case , Class
					strSQL = mySQL.getCaseClassDuration(strPaperID , strCaseID , strClass);
					break;
				case "1":
					//Author , Case , Group
					strSQL = mySQL.getCaseGroupDuration(strPaperID , strCaseID , strGroup);
					break;
				case "2":
					//Author , Case
					strSQL = mySQL.getCaseDuration(strPaperID , strCaseID);
					break;
				case "3":
					//Author , Class
					//strSQL = mySQL.getSelectionSummaryByAuthorClass(strPaperID , strQID , strSelectionID , strAuthorID , strClass);
					break;
				case "4":
					//Author , Group
					//strSQL = mySQL.getSelectionSummaryByAuthorGroup(strPaperID , strQID , strSelectionID , strAuthorID , strGroup);
					break;
				case "5":
					//Author
					//strSQL = mySQL.getSelectionSummaryByAuthor(strPaperID , strQID , strSelectionID , strAuthorID);
					break;
				case "6":
					//Case , Class
					strSQL = mySQL.getCaseClassDuration(strPaperID , strCaseID , strClass);
					break;
				case "7":
					//Case , Group
					strSQL = mySQL.getCaseGroupDuration(strPaperID , strCaseID , strGroup);
					break;
				case "8":
					//Case
					strSQL = mySQL.getCaseDuration(strPaperID , strCaseID);
					break;
				case "9":
					//Class
					//strSQL = mySQL.getSelectionSummaryByClass(strPaperID , strQID , strSelectionID , strClass);
					break;
				case "10":
					//Group
					//strSQL = mySQL.getSelectionSummaryByGroup(strPaperID , strQID , strSelectionID , strGroup);
					break;
			}

			if(strSQL.Trim().Length == 0)
			{
				tdDurationTable.Controls.Clear();
			}
			else
			{
				if(strFunction == "0" || strFunction == "1" || strFunction == "2" || strFunction == "6" || strFunction == "7" || strFunction == "8")
				{
					//�u����@�Юת�����
					setupSingleCaseDuration(strSQL , table);
				}
				else
				{
					//�h�ӱЮת�����
				}
			}
		}

		private void setupSingleCaseDuration(string strSQL , Table table)
		{
			//Title
			TableRow trTitle = new TableRow();
			table.Rows.Add(trTitle);
			trTitle.Attributes.Add("Class","header1_table_first_row");
			trTitle.ForeColor = System.Drawing.Color.DarkRed;
			trTitle.Font.Bold = true;

			//UserName
			TableCell tcUserNameTitle = new TableCell();
			trTitle.Cells.Add(tcUserNameTitle);
			tcUserNameTitle.Text = "User name";
			tcUserNameTitle.Width = System.Web.UI.WebControls.Unit.Percentage(50);
			tcUserNameTitle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

			//Duration
			TableCell tcDurationTitle = new TableCell();
			trTitle.Cells.Add(tcDurationTitle);
			tcDurationTitle.Text = "Learning duration";
			tcDurationTitle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;

			DataSet dsDuration = sqldb.getDataSet(strSQL);
			if(dsDuration.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i<dsDuration.Tables[0].Rows.Count ; i++)
				{
					TableRow tr = new TableRow();
					table.Rows.Add(tr);
					if(i % 2 == 0)
					{
						tr.Attributes.Add("Class","header1_tr_odd_row");
					}
					else
					{
						tr.Attributes.Add("Class","header1_tr_even_row");
					}

					//User name
					TableCell tcUserName = new TableCell();
					tr.Cells.Add(tcUserName);
					tcUserName.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
					try
					{
						tcUserName.Text = dsDuration.Tables[0].Rows[i]["cUserID"].ToString();
					}
					catch
					{
					}

					//Duration
					string strStartTime = "";
					string strUserID = "";
					try
					{
						strStartTime = dsDuration.Tables[0].Rows[i]["cStartTime"].ToString();
						strUserID = dsDuration.Tables[0].Rows[i]["cUserID"].ToString();
					}
					catch
					{
					}

					//�qSummary_Header��X�o�@�����
					string strUserLevel = "";
					string strCaseID = "";
					string strDivisionID = "";
					strSQL = "SELECT * FROM Summary_Header H WHERE cUserID = '"+strUserID+"' AND cStartTime = '"+strStartTime+"' ORDER BY cStartTime ";
					DataSet dsHeader = sqldb.getDataSet(strSQL);
					if(dsHeader.Tables[0].Rows.Count > 0)
					{
						try
						{
							strUserLevel = dsHeader.Tables[0].Rows[0]["cUserLevel"].ToString();
							strCaseID = dsHeader.Tables[0].Rows[0]["cCaseID"].ToString();
							strDivisionID = dsHeader.Tables[0].Rows[0]["cDivisionID"].ToString();
						}
						catch
						{
						}
					}
					dsHeader.Dispose();

					//��X�Ĥ@���ާ@���Юת�StartTime
					strSQL = "SELECT MIN(cStartTime) AS 'cStartTime' FROM Summary_Header H WHERE cUserLevel = '"+strUserLevel+"' AND cUserID = '"+strUserID+"' AND cCaseID = '"+strCaseID+"' AND cDivisionID = '"+strDivisionID+"' ";
					DataSet dsStartTime = sqldb.getDataSet(strSQL);
					if(dsStartTime.Tables[0].Rows.Count > 0)
					{
						try
						{
							strStartTime = dsStartTime.Tables[0].Rows[0]["cStartTime"].ToString();
						}
						catch
						{
						}
					}
					dsStartTime.Dispose();

					string strDuration = "";
					try
					{
						strDuration = myReceiver.calculateSectionDuration(strCaseID , strDivisionID , strUserLevel , strUserID , strStartTime);
					}
					catch
					{
					}

					
					TableCell tcDuration = new TableCell();
					tr.Cells.Add(tcDuration);
					tcDuration.ForeColor = System.Drawing.Color.Blue;
					tcDuration.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Center;
					try
					{
						tcDuration.Text = strDuration;
					}
					catch
					{
					}

				}
			}
			dsDuration.Dispose();
		}

		private void setupSelectionContent(Table table , Table tableOutput)
		{
			//���o���ݨ����Ҧ�����D
			string strSQL = "";
			strSQL = mySQL.getPaperSelectionContent(strPaperID);
			DataSet dsQuestion = sqldb.getDataSet(strSQL);
			if(dsQuestion.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i<dsQuestion.Tables[0].Rows.Count ; i++)
				{
					//���o�����D��QID
					string strQID = "";
					try
					{
						strQID = dsQuestion.Tables[0].Rows[i]["cQID"].ToString();
					}
					catch
					{
					}

					//���o�����D�����e
					string strQuestion = "";
					try
					{
						strQuestion = dsQuestion.Tables[0].Rows[i]["cQuestion"].ToString();
                        if (strQuestion == "") {	// get from QuestionIndex table
                            // added by dolphin @ 2007-03-22
                            strSQL = "SELECT * FROM QuestionIndex WHERE cQID='" + strQID + "'";
                            DataTable dtQuestion = sqldb.getDataSet(strSQL).Tables[0];
                            if (dtQuestion.Rows.Count > 0)
                            {
                                strQuestion = dtQuestion.Rows[0]["cQuestion"].ToString();
                            }
                        }
					}
					catch
					{
					}

					//�����D���D��
					intQuestionNum = intQuestionNum + 1;

					//------------------�إ�TableRow--------------------
					//���D��TableRow
					TableRow trQuestion = new TableRow();
					table.Rows.Add(trQuestion);

					//�إ߰��D��CSS
					trQuestion.Attributes.Add("Class","header1_table_first_row");
					trQuestion.ForeColor = System.Drawing.Color.DarkRed;
					trQuestion.Font.Bold = true;

					//�ﶵ��TableRow
					TableRow trSelection = new TableRow();
					table.Rows.Add(trSelection);
					trSelection.Attributes.Add("Class","header1_tr_odd_row");

					//�H�ƪ�TableRow
					TableRow trNum = new TableRow();
					table.Rows.Add(trNum);
					trNum.Attributes.Add("Class","header1_tr_even_row");

					//�D�`�P�N+�P�N���ʤ���
					TableRow trPer = new TableRow();
					table.Rows.Add(trPer);
					trPer.CssClass = "header1_tr_odd_row";

					//���j��TableRow
					TableRow trEmpty = new TableRow();
					table.Rows.Add(trEmpty);

					TableCell tcEmpty = new TableCell();
					trEmpty.Cells.Add(tcEmpty);

					Label lbEmpty = new Label();
					tcEmpty.Controls.Add(lbEmpty);
					lbEmpty.Text = "";
					lbEmpty.Style.Add("WIDTH","0px");

					//------------------�إ�TableCell-------------------
					//�إߦ����D��TableCell
					TableCell tcQuestion = new TableCell();
					trQuestion.Cells.Add(tcQuestion);
					tcQuestion.Text = intQuestionNum.ToString() + ". " + strQuestion;

					//�إ�Output table �����e
					TableRow trOutput = new TableRow();
					tableOutput.Rows.Add(trOutput);
					TableCell tcOutput = new TableCell();
					if(i % 2 == 0)
					{
						trOutput.CssClass = "header1_tr_even_row";
					}
					else
					{
						trOutput.CssClass = "header1_tr_odd_row";
					}
					trOutput.Cells.Add(tcOutput);
					tcOutput.Text = strQuestion;

					//�����D�ﶵ��TableCell
					this.setupSelectionAndNumRow(tcQuestion , trSelection , trNum , trPer , strQID , trOutput);
				}
			}
			else
			{
				//���ݨ��S������D
			}
			dsQuestion.Dispose();
		}

		private void setupSelectionAndNumRow(TableCell tcQuestion , TableRow trSelection , TableRow trNum , TableRow trPer , string strQID , TableRow trOutput)
		{
			//��X�����D���ﶵ
			string strSQL = "";
			strSQL = mySQL.getQuestionAndSelection(strQID);
			DataSet dsSelection = sqldb.getDataSet(strSQL);
			if(dsSelection.Tables[0].Rows.Count > 0)
			{
				//�������ﶵ�`�@���X�ӤH��
				double dTotalNum = 0;

				for(int i=0 ; i<dsSelection.Tables[0].Rows.Count ; i++)
				{
					//���o�ﶵ�����e
					string strSelection = "";
					try
					{
						strSelection = dsSelection.Tables[0].Rows[i]["cSelection"].ToString();
					}
					catch
					{
					}

					//���o�ﶵ��SelectionID
					string strSelectionID = "";
					try
					{
						strSelectionID = dsSelection.Tables[0].Rows[i]["cSelectionID"].ToString();
					}
					catch
					{
					}

					//�btrSelection�إ߿ﶵ�����e
					TableCell tcSelection = new TableCell();
					trSelection.Cells.Add(tcSelection);
					tcSelection.Text = strSelection;
					tcSelection.HorizontalAlign = HorizontalAlign.Center;

					//�btrSelection�إߦ��ﶵ���H��
					TableCell tcNum = new TableCell();
					trNum.Cells.Add(tcNum);
					
					//�̷Ӥ��PFunction�I�s���P��Function
					switch (strFunction)
					{
						case "0":
							//Author,Case , Class
							strSQL = mySQL.getSelectionSummaryByCaseClass(strPaperID , strQID , strSelectionID , strCaseID , strClass);
							break;
						case "1":
							//Author , Case , Group
							strSQL = mySQL.getSelectionSummaryByCaseGroup(strPaperID , strQID , strSelectionID , strCaseID , strGroup);
							break;
						case "2":
							//Author , Case
							strSQL = mySQL.getSelectionSummaryByCase(strPaperID , strQID , strSelectionID , strCaseID);
							break;
						case "3":
							//Author , Class
							strSQL = mySQL.getSelectionSummaryByAuthorClass(strPaperID , strQID , strSelectionID , strAuthorID , strClass);
							break;
						case "4":
							//Author , Group
							strSQL = mySQL.getSelectionSummaryByAuthorGroup(strPaperID , strQID , strSelectionID , strAuthorID , strGroup);
							break;
						case "5":
							//Author
							strSQL = mySQL.getSelectionSummaryByAuthor(strPaperID , strQID , strSelectionID , strAuthorID);
							break;
						case "6":
							//Case , Class
							strSQL = mySQL.getSelectionSummaryByCaseClass(strPaperID , strQID , strSelectionID , strCaseID , strClass);
							break;
						case "7":
							//Case , Group
							strSQL = mySQL.getSelectionSummaryByCaseGroup(strPaperID , strQID , strSelectionID , strCaseID , strGroup);
							break;
						case "8":
							//Case
							strSQL = mySQL.getSelectionSummaryByCase(strPaperID , strQID , strSelectionID , strCaseID);
							break;
						case "9":
							//Class
							strSQL = mySQL.getSelectionSummaryByClass(strPaperID , strQID , strSelectionID , strClass);
							break;
						case "10":
							//Group
							strSQL = mySQL.getSelectionSummaryByGroup(strPaperID , strQID , strSelectionID , strGroup);
							break;
					}

					//�]�w�䤺�e
					DataSet dsNum = sqldb.getDataSet(strSQL);

					//���D���H��
					tcNum.Text = dsNum.Tables[0].Rows.Count.ToString();
					//�N�H�Ʋ֥[�i�`�H��
					dTotalNum += dsNum.Tables[0].Rows.Count;
					//�]�wtableOutput���H��
					TableCell tcOutput = new TableCell();
					trOutput.Cells.Add(tcOutput);
					tcOutput.Text = dsNum.Tables[0].Rows.Count.ToString();
					tcOutput.HorizontalAlign = HorizontalAlign.Center;
					
					tcNum.ForeColor = System.Drawing.Color.Blue;
					dsNum.Dispose();

					tcNum.HorizontalAlign = HorizontalAlign.Center;
				}

				
				//�N�w�g���ͪ��H�Ƹ�Ʀ汽�y�@�M
				long intSelectPer1 = 0;
                long intSelectPer2 = 0;         
				for(int i=0 ; i<trNum.Cells.Count ; i++)
				{
					//�N�H�ƪ�����[�J�ʤ���

					//���X�H��
                    double dNum = Convert.ToDouble(trNum.Cells[i].Text);

					//�N�H��/�`�H��=�ʤ���
                    long intPer;
                    if(dTotalNum==0)//�קK���s
                    {
                        intPer = 0;
                    }else{
                        intPer = Convert.ToInt64((dNum / dTotalNum) * 100);
                    } 
                    
                    
					//�����D�`�P�N�M�P�N���ʤ���
					if(i==0)
					{
						intSelectPer1 = intPer;
					}
					else if(i==1)
					{
						intSelectPer2 = intPer;
					}

					//�N�ʤ���[�JTableCell
					trNum.Cells[i].Text = trNum.Cells[i].Text + "  " + "(" + intPer.ToString() + "%)";
					
					//�]�w�D�`�P�N�P�P�N���ʤ���
					if(i==2)
					{
						TableCell tcPer = new TableCell();
						trPer.Cells.Add(tcPer);
						tcPer.ColumnSpan = 2;
						tcPer.HorizontalAlign = HorizontalAlign.Center;

						long intAgree = intSelectPer1 + intSelectPer2;
						//tcPer.Text = "�D�`�P�N(%) + �P�N(%) = " + intAgree.ToString() + "(%)";
						tcPer.Text = intAgree.ToString() + "(%)";

						TableCell tcEmpty = new TableCell();
						trPer.Cells.Add(tcEmpty);
						tcEmpty.ColumnSpan = 3;
					}
				}

				//�վ�tcQuestion��ColSpan�ݩ�
				tcQuestion.ColumnSpan = dsSelection.Tables[0].Rows.Count;
			}
			else
			{
				//�����D�S���ﶵ
			}
			dsSelection.Dispose();
		}

		private void setupTextContent(Table table)
		{
			//���o���ݨ����Ҧ��ݵ��D
			string strSQL = "";
			strSQL = mySQL.getPaperTextContent(strPaperID);
			DataSet dsText = sqldb.getDataSet(strSQL);
			if(dsText.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i<dsText.Tables[0].Rows.Count ; i++)
				{
					//�����D���D��
					intQuestionNum = intQuestionNum + 1;

					//���o�����D�����e
					string strQuestion = "";
					try
					{
						strQuestion = dsText.Tables[0].Rows[i]["cQuestion"].ToString();
					}
					catch
					{
					}

					//���o�����D��QID
					string strQID = "";
					try
					{
						strQID = dsText.Tables[0].Rows[i]["cQID"].ToString();
					}
					catch
					{
					}

					//�إ߰��D��TableRow
					TableRow trQuestion = new TableRow();
					table.Rows.Add(trQuestion);

					//�إ߰��D��TableCell
					TableCell tcQuestion = new TableCell();
					trQuestion.Cells.Add(tcQuestion);
					tcQuestion.Text = intQuestionNum.ToString() + ". " + strQuestion;
					//***�p�G�n��ܾǥͩm�W�Ч令2
					tcQuestion.ColumnSpan = 1;

					//�إ߰��D��CSS
					trQuestion.Attributes.Add("Class","header1_table_first_row");
					trQuestion.ForeColor = System.Drawing.Color.DarkRed;
					trQuestion.Font.Bold = true;

					//�إߦ����D���ǥ͵���
					this.setupTextAnswer(table , strQID);

					//���j��TableRow
					if(i != (dsText.Tables[0].Rows.Count - 1))
					{
						TableRow trEmpty = new TableRow();
						table.Rows.Add(trEmpty);

						TableCell tcEmpty = new TableCell();
						trEmpty.Cells.Add(tcEmpty);

						Label lbEmpty = new Label();
						tcEmpty.Controls.Add(lbEmpty);
						lbEmpty.Text = "";
						lbEmpty.Style.Add("WIDTH","0px");
					}
				}
			}
			else
			{
				//���ݨ��S���ݵ��D
			}
			dsText.Dispose();
		}

		private void setupTextAnswer(Table table , string strQID)
		{
			//�إ߾ǥ͵��ת�function
			string strSQL = "";
			//�̷Ӥ��PMODE�I�s���P��Function
			switch(strFunction)
			{
				case "0":
					//Author,Case , Class
					strSQL = mySQL.getTextSummaryByCaseClass(strPaperID , strQID , strCaseID , strClass);
					break;
				case "1":
					//Author , Case , Group
					strSQL = mySQL.getTextSummaryByCaseGroup(strPaperID , strQID , strCaseID , strGroup);
					break;
				case "2":
					//Author , Case
					strSQL = mySQL.getTextSummaryByCase(strPaperID , strQID , strCaseID);
					break;
				case "3":
					//Author , Class
					strSQL = mySQL.getTextSummaryByAuthorClass(strPaperID , strQID , strAuthorID , strClass);
					break;
				case "4":
					//Author , Group
					strSQL = mySQL.getTextSummaryByAuthorGroup(strPaperID , strQID , strAuthorID , strGroup);
					break;
				case "5":
					//Author
					strSQL = mySQL.getTextSummaryByAuthor(strPaperID , strQID ,strAuthorID);
					break;
				case "6":
					//Case , Class
					strSQL = mySQL.getTextSummaryByCaseClass(strPaperID , strQID , strCaseID , strClass);
					break;
				case "7":
					//Case , Group
					strSQL = mySQL.getTextSummaryByCaseGroup(strPaperID , strQID , strCaseID , strGroup);
					break;
				case "8":
					//Case
					strSQL = mySQL.getTextSummaryByCase(strPaperID , strQID , strCaseID);
					break;
				case "9":
					//Class
					strSQL = mySQL.getTextSummaryByClass(strPaperID , strQID , strClass);
					break;
				case "10":
					//Group
					strSQL = mySQL.getTextSummaryByGroup(strPaperID , strQID , strGroup);
					break;
			}

			//�]�w�䤺�e
			
			DataSet dsAnswer = sqldb.getDataSet(strSQL);
			if(dsAnswer.Tables[0].Rows.Count > 0)
			{
				for(int i=0 ; i< dsAnswer.Tables[0].Rows.Count ; i++)
				{
					//UsrID
					string strUserID = "";
					try
					{
						strUserID = dsAnswer.Tables[0].Rows[i]["cUserID"].ToString();
					}
					catch
					{
					}

					//User name
					string strUserName = "";
					try
					{
						strUserName = myReceiver.getUserName(strUserID);
					}
					catch
					{
					}

					//Answer
					string strAnswer = "";
					try
					{
						strAnswer = dsAnswer.Tables[0].Rows[i]["cAnswer"].ToString();
					}
					catch
					{
					}

					TableRow trAnswer = new TableRow();
					table.Rows.Add(trAnswer);
					//�إ�CSS
					if((i % 2) != 0)
					{
						trAnswer.Attributes.Add("Class","header1_tr_odd_row");	
					}
					else
					{
						trAnswer.Attributes.Add("Class","header1_tr_even_row");	
					}
					/***�p�G�n��ܾǥͩm�W�в�������
					//�ǥͩm�W
					TableCell tcName = new TableCell();
					trAnswer.Cells.Add(tcName);
					tcName.Text = "";//strUserName;
					tcName.Width = Unit.Pixel(0);
					*/
					//����
					TableCell tcAnswer = new TableCell();
					trAnswer.Cells.Add(tcAnswer);
					tcAnswer.Text = strAnswer;
					tcAnswer.Style.Add("FONT-WEIGHT","bold");
					tcAnswer.Style.Add("FONT-SIZE","16px");
					tcAnswer.ForeColor = System.Drawing.Color.Blue;
				}
			}
			dsAnswer.Dispose();
		}


		private void getParameter()
		{
			//PaperID
			if(usi.PaperID != "")
			{
                strPaperID = usi.PaperID;
			}
			else
			{
                // modified @ 2007-03-22 by dolphin, modified the Query string check
                if (Request["PaperID"] != null && Request.QueryString["PaperID"] != "")
                {
                    strPaperID = Request.QueryString["PaperID"];
                }
			}

			//Function
			if(Request.QueryString["Function"] != null)
			{
				strFunction = Request.QueryString["Function"].ToString();
			}
			else
			{
				strFunction = "0";
			}

			//CaseID
			if(usi.CaseID != null)
			{
                strCaseID = usi.CaseID;
			}
			else
			{
				if(Request.QueryString["CaseID"] != null)
				{
					strCaseID = Request.QueryString["CaseID"].ToString();
				}
			}

			//AuthorID
			if(Session["Author"] != null)
			{
				strAuthorID = Session["Author"].ToString();
			}
			else
			{
				if(Request.QueryString["AuthorID"] != null)
				{
					strAuthorID = Request.QueryString["AuthorID"].ToString();
				}
			}
			

			//Class
			if(Session["Class"] != null)
			{
				strClass = Session["Class"].ToString();
			}
			else
			{
				if(Request.QueryString["Class"] != null)
				{
					strClass = Request.QueryString["Class"].ToString();
				}
			}
			
			//Group
			if(Session["Group"] != null)
			{
				strGroup = Session["Group"].ToString();
			}
			else
			{
				if(Request.QueryString["Group"] != null)
				{
					strGroup = Request.QueryString["Group"].ToString();
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
