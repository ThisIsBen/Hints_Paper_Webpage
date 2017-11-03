﻿using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Hints.DB;
using Hints.DB.QuestionGroup;
using Hints.HintsUtility;
using Hints;
using System.Drawing;
using ORCS.DB;
using Hints.DB.Administrator;
using System.Collections.Generic;


public partial class AuthoringTool_CaseEditor_Paper_QuestionGroupTree_QGroupTreeNew : AuthoringTool_BasicForm_BasicForm, ICallbackEventHandler
{
    protected clsHintsDB hintsDB = new clsHintsDB();
    protected string strSQL = "";
    protected string strSpanIDPrefix = "";
    private string message = "";    // for callback
    protected string sCallBackFunctionInvocation = "";

    //目前模式資料
    //post
    private string strPresentType = "";
    private string strEditMode = "";
    private string strGroupID = "";
    //pre
    private string strOpener = "";
    private string strQuestionMode = "";
    private string strModifyType = "";
    
    protected void Page_Load(object sender, EventArgs e)
    {
       


        this.initPage();        
        if (usi.UsingSystem == "")
        {
            usi.UsingSystem = "HINTS";
        }
        if (!this.IsPostBack)
        {
            //畫面開始先隱藏主要Panel 朱君2012/10/24
            Panel1.Visible = false;
            //畫面開始先隱藏主要Pane2 老詹2015/05/26
            Panel2.Visible = false;
            //初始化ddlSelectQuestion下拉式選單
            setddlSelectQuestion();
            
            /* 教授要求非對應課程也能選其他題庫 所以先註解
            //編輯考卷 檢查若在上課且有題庫對應到上課名稱將直接導至題庫頁面
            if (strModifyType == "Paper" && strPresentType == "Edit")
            {
               //如果正在上課會取得CourseID 否則為空
               string strCourseID = CheckClassState();
               //如果正在上課會檢查課程有無再題庫中
               if (!strCourseID.Equals(""))
               {
                   //取得課程名稱
                   string strCourseName = clsEditGroup.ORCS_ClassGroup_SELECT_by_ClassGroupID(strCourseID).Rows[0]["cClassGroupName"].ToString();
                   //取得所有題庫ID
                   List<string> QuestionbankIDs = new List<string>();
                   DataTable dtSelectQuestion = new DataTable();
                   if (Request.QueryString["Career"] != null && Request.QueryString["Career"].ToString() != "")
                   {
                       string strSQL_SelectQuestion = "SELECT * FROM QuestionList ORDER BY qId";
                       dtSelectQuestion = hintsDB.getDataSet(strSQL_SelectQuestion).Tables[0];
                   }
                   else
                   {
                       string strSQL_SelectQuestion = "SELECT * FROM QuestionList WHERE qId!='25' ORDER BY qId";
                       dtSelectQuestion = hintsDB.getDataSet(strSQL_SelectQuestion).Tables[0];
                   }
                   //紀錄所有題庫ID
                   for (int i = 0; i < dtSelectQuestion.Rows.Count; i++)
                   {
                       QuestionbankIDs.Add(dtSelectQuestion.Rows[i]["qId"].ToString());
                   }

                   foreach(string strQuestionbankID in QuestionbankIDs)
                   {
                        //取得題庫底下節點資料
                        string strSQL_NodeID = "SELECT * FROM QuestionGroupTree WHERE qId LIKE '" + strQuestionbankID + "'";
                        DataTable dtNodeData = hintsDB.getDataSet(strSQL_NodeID).Tables[0];
                        if(dtNodeData.Rows.Count > 0)
                        {
                            foreach(DataRow drNodeData in dtNodeData.Rows)
                            {
                                //如果題庫名稱符合課程名稱跳過此頁面直接導向該題庫
                                if(strCourseName.IndexOf(drNodeData["cNodeName"].ToString()) != -1)
                                {
                                    string nextPage = "../Paper_SelectQuestion.aspx?GroupID=" + drNodeData["cNodeID"].ToString() + "&SearchMode=Group&cCourseID=" + strCourseID;
                                    Response.Redirect(nextPage);
                                }
                            }
                        }
                   }
               }
            }
             */
            //若Career參數有值，則選單預設職業問答樹選項  老詹 2015/06/27         
            if (Request.QueryString["Career"] != null && Request.QueryString["Career"].ToString() != "")
            {
                hf_Career.Value = Request.QueryString["Career"].ToString();
                if (Request.QueryString["SelectedGroup"] != null)
                {
                    Session["SelectedGroupForRecover"] = Request.QueryString["SelectedGroup"].ToString();
                }
                ddlSelectQuestion.SelectedIndex = 5;
                SetSelectQuestion();
            }



            
        }

        string selectNodeID = "";
        if (Session["QuestionBankID"] != null)
        {
            selectNodeID = Session["QuestionBankID"].ToString();
            if (tvQuestionGroup.Nodes.Count > 0)
                foreach (TreeNode tnnode in tvQuestionGroup.Nodes)
                {
                    SearchNode(tnnode, selectNodeID);
                }
        }

        sCallBackFunctionInvocation = ClientScript.GetCallbackEventReference(this, "message", "show_callback", null);
        Ajax.Utility.RegisterTypeForAjax(typeof(AuthoringTool_CaseEditor_Paper_QuestionGroupTree_QGroupTreeNew));





        
    }

    #region swakevin function
    protected void initPage()
    {
        string strUserID = usi.UserID;
        //UserID
        if (Request.QueryString["UserID"] != null)
        {
            usi.UserID = Request.QueryString["UserID"].ToString();
        }

        //Opener
        if (Request.QueryString["Opener"] != null)
        {
            hiddenOpener.Value = Request.QueryString["Opener"].ToString();
        }else if (Session["Opener"] != null)
        {
            hiddenOpener.Value = Session["Opener"].ToString();
        }

        //Setup opener
        if (Session["Opener"] != null)
        {
            Session["Opener"] = "./QuestionGroupTree/QuestionGroupTree";
        }
        else
        {
            Session.Add("Opener", "./QuestionGroupTree/QuestionGroupTree");
        }

        //PresentType
        if (Session["PresentType"] != null)
        {
            hiddenPresentType.Value = Session["PresentType"].ToString();
        }

        //QuestionMode
        if (Session["QuestionMode"] != null)
        {
            hiddenQuestionMode.Value = Session["QuestionMode"].ToString();
        }

        //QuestionType
        if (Session["QuestionType"] != null)
        {
            hiddenQuestionType.Value = Session["QuestionType"].ToString();
        }

        //EditMode
        if (Session["EditMode"] != null)
        {
            hiddenEditMode.Value = Session["EditMode"].ToString();
        }

        //ModifyType
        if (Request.QueryString["ModifyType"] != null)
        {
            hiddenModifyType.Value = Request.QueryString["ModifyType"].ToString();
            Session["ModifyType"] = hiddenModifyType.Value;
        }else if (Session["ModifyType"] != null)
        {
            hiddenModifyType.Value = Session["ModifyType"].ToString();
        }

        //QuestionFunction
        if (Session["QuestionFunction"] != null)
        {
            hiddenQuestionFunction.Value = Session["QuestionFunction"].ToString();
        }

        //PreOpener
        if (Session["PreOpener"] != null)
        {
            if (Request.QueryString["Opener"] != null)
            {
                if (Request.QueryString["Opener"].ToString() == "Paper_QuestionType")
                    hiddenPreOpener.Value = Session["PreOpener"].ToString();
                else if(Request.QueryString["Opener"].ToString() == "SelectPaperModeAddANewQuestion")//課堂練習編輯考卷新增題目
                {
                    Session["SystemFunction"] = "EditQuestion";
                    hiddenPreOpener.Value = "SelectPaperModeAddANewQuestion";
                    Session["PreOpener"] = "SelectPaperModeAddANewQuestion";
                }
            }
            if (Session["PreOpener"].ToString().Equals("SelectPaperModeAddANewQuestion"))//課堂練習編輯考卷新增題目
            {
                hiddenPreOpener.Value = "SelectPaperModeAddANewQuestion";
            }
        }

        //以下將取得的Session存入後端變數
        if (hiddenPresentType.Value != null)
        {
            strPresentType = hiddenPresentType.Value;
        }

        if (hiddenEditMode.Value != null)
        {
            strEditMode = hiddenEditMode.Value;
        }

        if (strCurrentNodeID.Value != null)
        {
            strGroupID = strCurrentNodeID.Value;
        }

        if (hiddenModifyType.Value != null)
        {
            strModifyType = hiddenModifyType.Value;
        }
        if (hiddenOpener.Value != null)
        {
            strOpener = hiddenOpener.Value;
        }
        if (hiddenQuestionMode.Value != null)
        {
            strQuestionMode = hiddenQuestionMode.Value;
        }
        if (hiddenModifyType.Value != null)
        {
            strModifyType = hiddenModifyType.Value;
        }
        if (Session["QuestionBankID"] != null)
        {
            strCurrentNodeID.Value = Session["QuestionBankID"].ToString();
            hiddenQuestionBankID.Value = Session["QuestionBankID"].ToString();
        }

    }
    protected void initFeatureSession() 
    {
        //讓一開始記錄在session的FeatureDDL個數的數值歸零
        Session["FeatureDDLNum"] = "0";
        
        //初始化Session所記錄的datatable
        DataTable dtFeatureItem = new DataTable();
        dtFeatureItem.Columns.Add(new DataColumn("FaetureSetID", typeof(string)));
        dtFeatureItem.Columns.Add(new DataColumn("FaetureSetName", typeof(string)));
        dtFeatureItem.Columns.Add(new DataColumn("FaetureItemID", typeof(string)));
        dtFeatureItem.Columns.Add(new DataColumn("FaetureItemName", typeof(string)));
        //增加一個自動編號的欄位
        DataColumn COL = new DataColumn("Number");
        COL.AutoIncrement = true;
        COL.AutoIncrementSeed = 1;
        COL.AutoIncrementStep = 1;
        dtFeatureItem.Columns.Add(COL);
        //增加欄位，紀錄要使用AND模式或是OR模式
        dtFeatureItem.Columns.Add(new DataColumn("FaetureSearchMode", typeof(string)));
        dtFeatureItem.Columns.Add(new DataColumn("FaetureSearchModeValue", typeof(string)));
        
        Session["dtSelectedFeatureItem"] = dtFeatureItem;

        //紀錄題庫代號到Session
        Session["QuestionListID"] = ddlSelectQuestion.SelectedValue.ToString(); 
    }
    #endregion

    #region question group tree construction
    protected void ConstructQuestionGroupTree()
    {

        //先清空原有的題庫樹
        tvQuestionGroup.Nodes.Clear();
        tvMoveGroup.Nodes.Clear();
        strSpanIDPrefix = "display";

        //從QuestionList資料表讀取Root的資訊
        DataTable dtQuestionRoot = new DataTable();
        string strSQL_QuestionRoot = "SELECT * FROM QuestionList WHERE qName='" + ddlSelectQuestion.SelectedItem.ToString() + "'";
        dtQuestionRoot = hintsDB.getDataSet(strSQL_QuestionRoot).Tables[0];

        tvQuestionGroup.Nodes.Add(getTree(dtQuestionRoot.Rows[0]["qRootName"].ToString(), dtQuestionRoot.Rows[0]["qName"].ToString()));
        tvQuestionGroup.CollapseAll();

        TreeNode tnRoot = new TreeNode();
        TreeNode tnNode = new TreeNode();

        tnRoot = tvQuestionGroup.Nodes[0];
        tnRoot.Expanded = true;

        for (int nIdx = 0; nIdx < tnRoot.ChildNodes.Count; nIdx++)
        {
            tnNode = tvQuestionGroup.Nodes[0].ChildNodes[nIdx];
            tnNode.Expand();
        }

    }

    private void SearchNode(TreeNode tnnode , string selectNodeID){
        if(tnnode.ChildNodes.Count > 0){
            foreach (TreeNode tnchildnode in tnnode.ChildNodes)
                SearchNode(tnchildnode, selectNodeID);
        }
        if (tnnode.Value.Equals(selectNodeID))
        {
            tnnode.Selected = true;
        }
    }

    //取得所有的題庫
    public DataTable getQuestionGroupNodeForAll(object cNodeID, object cParentNodeID)
    {
        DataTable dtResult = new DataTable();
        strSQL = "SELECT * FROM QuestionGroupTree WHERE cNodeID LIKE '" + cNodeID + "' AND cParentID LIKE '" + cParentNodeID + "' ORDER BY  cNodeID asc";
        try
        {
            dtResult = hintsDB.getDataSet(strSQL).Tables[0];
        }
        catch
        {
            dtResult = new DataTable();
        }
        return dtResult;
    }

    public TreeNode getTree(string nodeID, string nodeName)
    {
        //取得node的mode與作者
        DataTable dtNodeID = new DataTable();
        string strSQL_NodeID = "SELECT * FROM QuestionGroupTree WHERE cNodeID LIKE '" + nodeID + "' AND cNodeName LIKE '" + nodeName + "' ORDER BY cNodeID ASC";
        dtNodeID = hintsDB.getDataSet(strSQL_NodeID).Tables[0];
        string strAuthor = "";
        string strNodeName = "";

        TreeNode node = new TreeNode();
        if (dtNodeID.Rows.Count > 0)
        {
            strAuthor = dtNodeID.Rows[0]["cAuthor"].ToString();
            strNodeName = dtNodeID.Rows[0]["cNodeName"].ToString();
            node.Value = nodeID;
            node.Text = "<span id='" + strSpanIDPrefix + "_" + nodeID + "'>" + nodeName + "</span>";
            //node.NavigateUrl = "javascript:SelectNode('" + nodeID + "','" + strAuthor + "')";
        }
        else
        {
            node.Value = nodeID;
            node.Text = "<span id='" + strSpanIDPrefix + "_" + nodeID + "'>" + nodeName + "</span>";
            //node.NavigateUrl = "javascript:SelectNode('" + nodeID + "','" + strAuthor + "')";
        }
   

        //判斷node是否還有children node
        DataTable dtChildren = this.getQuestionGroupNodeForAll("%", nodeID);
        if (dtChildren.Rows.Count > 0)
        {
            foreach (DataRow drData in dtChildren.Rows)
            {
                node.ChildNodes.Add(getTree(drData["cNodeID"].ToString(), drData["cNodeName"].ToString()));
            }
        }
        return node;
    }
    protected void initDDLFeatureSet()
    {
        DDLFeatureSet.Items.Clear();
        DataTable dtFeatureSetList = new DataTable();
        string strSQL_NodeID = "SELECT A.cNodeID, A.iFeatureSetID, B.cNodeName FROM FeatureSetItem AS A INNER JOIN QuestionList AS C ON A.iFeatureSetID = C.iFeatureSetID INNER JOIN FeaturevalueTree AS B ON A.cNodeID = B.cNodeID WHERE (C.qId='" + ddlSelectQuestion.SelectedValue.ToString() + "')";
        dtFeatureSetList = hintsDB.getDataSet(strSQL_NodeID).Tables[0];
        for(int i=0;i<dtFeatureSetList.Rows.Count;i++)
        {
            DDLFeatureSet.Items.Add(new ListItem(dtFeatureSetList.Rows[i]["cNodeName"].ToString(), dtFeatureSetList.Rows[i]["cNodeID"].ToString()));
        }
    }
    protected void setDDLFeatureItem()
    {
        DDLFeatureItem.Items.Clear();
        DataTable dtFeatureSetItem = new DataTable();
        string strSQL_ItemID = "SELECT iFeatureNum,sFeatureName FROM FeaturevalueItem WHERE cNodeID='" + DDLFeatureSet.SelectedValue.ToString() + "'";
        dtFeatureSetItem = hintsDB.getDataSet(strSQL_ItemID).Tables[0];
        for (int i = 0; i < dtFeatureSetItem.Rows.Count; i++)
        {
            DDLFeatureItem.Items.Add(new ListItem(dtFeatureSetItem.Rows[i]["sFeatureName"].ToString(), dtFeatureSetItem.Rows[i]["iFeatureNum"].ToString()));
        }
    
    }
    protected void initDDLSearchMode()
    {
        DDLSearchMode.Items.Clear();
        DDLSearchMode.Items.Add(new ListItem("AND","1"));
        DDLSearchMode.Items.Add(new ListItem("OR", "2"));
    }
    protected void initFeatureItemTable()
    {
        DataTable dtSessionFeatureItem = (DataTable)Session["dtSelectedFeatureItem"];
        for (int i = 1; i <= Convert.ToInt32(Session["FeatureDDLNum"]); i++)
        {

            //創造出特徵表格格式
            //若是第一個則不顯示Search模式，因為沒意義
            Table tbl = creatFeatureTable(i);
            Label lbFeatureSetTittle = new Label();
            Label lbFeatureItem = new Label();
            Label lbFeatureSearchMode = new Label();

            //讀取session紀錄
            lbFeatureSetTittle.Text = dtSessionFeatureItem.Rows[i - 1]["FaetureSetName"].ToString();
            lbFeatureItem.Text = dtSessionFeatureItem.Rows[i - 1]["FaetureItemName"].ToString();
            lbFeatureSearchMode.Text = dtSessionFeatureItem.Rows[i - 1]["FaetureSearchMode"].ToString();
            
            tbl.Rows[0].Cells[0].Controls.Add(lbFeatureSetTittle);
            tbl.Rows[0].Cells[1].Controls.Add(lbFeatureItem);
            //若是第一個則不顯示Search模式，因為沒意義
            if (i != 1)
            tbl.Rows[0].Cells[2].Controls.Add(lbFeatureSearchMode);

            PanelFeatureItem.Controls.Add(tbl);
        }
    }
    //創造特徵物件的表格
    protected Table creatFeatureTable(int count)
    {
        Table tbl = new Table(); ;
        tbl.Width = Unit.Parse("100%");
        tbl.BorderColor = Color.DimGray;
        tbl.BorderWidth = Unit.Parse("1px");
        TableRow trDDL = new TableRow();

        TableCell tcFeaturesetTittle = new TableCell();
        tcFeaturesetTittle.Width = 120;
        tcFeaturesetTittle.BorderColor = Color.DimGray;
        tcFeaturesetTittle.BorderWidth = Unit.Parse("1px");
        tcFeaturesetTittle.HorizontalAlign = HorizontalAlign.Center;
        
        TableCell tcFeaturevalueValue = new TableCell();
        tcFeaturevalueValue.BorderColor = Color.DimGray;
        tcFeaturevalueValue.BorderWidth = Unit.Parse("1px");

        TableCell tcFeatureSearchMode = new TableCell();
        tcFeatureSearchMode.Width = 120;
        tcFeatureSearchMode.BorderColor = Color.DimGray;
        tcFeatureSearchMode.BorderWidth = Unit.Parse("1px");
        tcFeatureSearchMode.HorizontalAlign = HorizontalAlign.Center;

        tbl.Rows.Add(trDDL);
        trDDL.Cells.Add(tcFeaturesetTittle);
        trDDL.Cells.Add(tcFeaturevalueValue);
        //若是第一個則不顯示Search模式，因為沒意義
        if(count!=1)
        trDDL.Cells.Add(tcFeatureSearchMode);
        return tbl;
    }
    

    #endregion

    #region Question group tree operation

    #region Modify question group
    protected void btModifyGroupSubmit_Click(object sender, EventArgs e)
    {
        strSQL = "UPDATE QuestionGroupTree SET cNodeName='" + tNodeName.Value + "' WHERE cNodeID='" + strCurrentNodeID.Value + "'";
        try
        {
            hintsDB.ExecuteNonQuery(strSQL);
        }
        catch
        {
            this.RegisterClientScriptBlock("", "<script> alert('error'); </script>");
        }

        this.ConstructQuestionGroupTree();
        //Response.Redirect("QGroupTree.aspx");
    }
    #endregion
    #region add question group
    protected void btAddGroupSubmit_Click(object sender, EventArgs e)
    {
         string[] strTmpArr = tNewNodeName.Text.Split(',');
         for (int i = 0; i < strTmpArr.Length; i++)
         {
             if (strTmpArr[i] != "")
             {
                 DataTable dtQuestionNewNode = new DataTable();
                 string strSQL_QuestionNewNode = "SELECT * FROM QuestionGroupTree WHERE cNodeID='" + strCurrentNodeID.Value + "'";
                 dtQuestionNewNode = hintsDB.getDataSet(strSQL_QuestionNewNode).Tables[0];

                 strSQL = "INSERT INTO QuestionGroupTree VALUES('" + this.getNewNodeID("") + "','" + strCurrentNodeID.Value + "','" + strTmpArr[i] + "', '" + dtQuestionNewNode.Rows[0]["cNodeType"].ToString() + "', '" + usi.UserID + "','" + dtQuestionNewNode.Rows[0]["qId"].ToString() + "')";
                 try
                 {
                     hintsDB.ExecuteNonQuery(strSQL);
                 }
                 catch
                 {
                     this.RegisterClientScriptBlock("", "<script> alert('error'); </script>");
                 }
             }
         }

        this.ConstructQuestionGroupTree();
        //Response.Redirect("QGroupTree.aspx");
    }

    private string getNewNodeID(string folder_id_to_add)
    {
        string strNewID;
        int intTemp = 0;
        DateTime dtNow = DateTime.Now;
        while (dtNow.AddSeconds(0.1) < DateTime.Now)
            intTemp++;
        strNewID = "Group_" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        return (strNewID);
    }
    #endregion
    #region delete question group
    protected void btDeletNodeSubmit_Click(object sender, EventArgs e)
    {
        this.Delete_Node(strCurrentNodeID.Value);
        this.ConstructQuestionGroupTree();
        //Response.Redirect("QGroupTree.aspx");
    }

    public void Delete_Node(string nodeID_to_delete)
    {
        strSQL = "DELETE QuestionGroupTree WHERE cNodeID = '" + nodeID_to_delete + "'";

        hintsDB.ExecuteNonQuery(strSQL);
        //刪除子節點
        strSQL = "SELECT cNodeID FROM QuestionGroupTree WHERE cParentID = '" + nodeID_to_delete + "' ";
        DataTable dt = hintsDB.getDataSet(strSQL).Tables[0];
        foreach (DataRow dr in dt.Rows)
        {
            Delete_Node(dr["cNodeID"].ToString());
        }
    }
    #endregion

    #endregion

    #region Callback event
    public void RaiseCallbackEvent(string eventArg)
    {
        this.message = eventArg;
    }
    public string GetCallbackResult()
    {
        string nodeName = "";
        try
        {
            DataTable dtNode = this.getQuestionGroupNodeForAll(message, "%");
            nodeName = dtNode.Rows[0]["cNodeName"].ToString();
        }
        catch
        {
            nodeName = "";
        }
        return nodeName;
    }
    #endregion


    protected void btBack_Click(object sender, EventArgs e)
    {
        if (Session["QuestionBankID"] != null)
        {
            Session.Remove("QuestionBankID");
        }
        string prePage = "";
        //PreOpener
        if (hiddenPreOpener.Value == "SelectPaperMode")
        {
            prePage = "../Paper_QuestionTypeNew.aspx?Opener=Paper_MainPage";
        }
        else if (hiddenPreOpener.Value == "SelectPaperModeAddANewQuestion")
        {
            /*
            ////use JS alert() in C#
            ScriptManager.RegisterStartupScript(
             this,
             typeof(Page),
             "Alert",
            "<script>alert('" + Request.UrlReferrer.ToString() + "');</script>",
             false);
             */
            if (Session["PreOpener"] != null)
            {
                Session["PreOpener"] = "SelectPaperMode";
            }

            //關閉視窗
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "close", "<script> window.close(); </script>");
            
            //Response.Redirect(Session["PreviousPageURL"].ToString());
            return;




        }
        else if (strModifyType == "Paper")
        {
            //編輯考卷
            if (strOpener == "Paper_QuestionMode")
            {
                prePage = "../Paper_QuestionMode.aspx";
            }
            else if (strOpener == "Paper_OtherQuestion")
            {
                prePage = "../Paper_OtherQuestion.aspx";
            }
            else if (strOpener == "Paper_PresentMethod")
            {
                prePage = "../Paper_PresentMethod.aspx";
            }
            else if (strOpener == "Paper_EditMethod")
            {
                prePage = "../Paper_EditMethod.aspx";
            }
            else if (strOpener == "Paper_MainPage")
            {
                prePage = "../Paper_MainPage.aspx";
            }
            else if (strOpener == "Paper_QuestionMain")
            {
                prePage = "../Paper_QuestionMain.aspx";
            }
            else
            {
                prePage = "../Paper_QuestionMode.aspx";
            }
        }
        else
        {
            //編輯題目
            prePage = "../Paper_QuestionMainNew.aspx?Career=";
        }

        Response.Redirect(prePage);
    }

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        //如果為空字串 則提醒使用者請先選擇題庫
        if (strGroupID.Equals("")) 
        {
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "alert", "<script>alert('請先選擇左方題庫選項，再點擊Submit按鈕!');</script>");
            return;
        }

        string nextPage = "";
        if (strModifyType == "Paper")
        {
           
            //編輯考卷
            if (strPresentType == "Edit")
            {
                if (strEditMode == "Automatic")
                {
                    nextPage = "../Paper_RandomSelect.aspx?GroupID=" + strGroupID;
                }
                else
                {
                    if (Request.QueryString["CaseID"] != null)
                        nextPage = "../Paper_SelectQuestion.aspx?GroupID=" + strGroupID + "&SearchMode=Group" + "&CaseID=" + Request.QueryString["CaseID"].ToString();
                    else
                        nextPage = "../Paper_SelectQuestion.aspx?GroupID=" + strGroupID + "&SearchMode=Group";
                    
                }
            }
            else
            {
                nextPage = "../Paper_RandomSelect.aspx?GroupID=" + strGroupID;
            }
        }
        else
        {
            
            //編輯題目
            string strQuestionFunction = "";
            if (hiddenQuestionFunction.Value != null)
            {
                strQuestionFunction = hiddenQuestionFunction.Value;
            }
            
            //if (strQuestionFunction == "New")
            //{
                /*
                //Ben check
                //use JS alert() in C#
                ScriptManager.RegisterStartupScript(
                 this,
                 typeof(Page),
                 "Alert",
                 "<script>alert('" + hiddenQuestionType.Value + "');</script>",
                 false);
                /////
                 */

                //Ben comment the what the original strQuestionFunction == "New" would do.

                /*
                //新編題目
                if (hiddenQuestionType.Value == "1")
                {
                    //選擇題
                    nextPage = "../CommonQuestionEdit/Page/ShowQuestion.aspx?GroupID=" + strGroupID;
                }
                else
                {
                    //判斷目前選擇的群組的root 是否為 SubjectiveRoot ObjectiveRoot AssessmentRoot PlanRoot
                    //是則用新的問答題資料表的新頁面 否則用舊的
                    hfRoot.Value = "false";
                    QuestionGroupTree_SELECT_Root(strGroupID);
                    if (hfRoot.Value == "true")
                    {
                        //Response.Write("<script>alert('hfRoot-true QA');</script>");
                        //問答題
                        nextPage = "../Paper_TextQuestionEditorNew.aspx?GroupID=" + strGroupID;
                    }
                    else
                    {
                        //Response.Write("<script>alert('hfRoot-false QA'');</script>");
                        //問答題
                        //nextPage = "../Paper_TextQuestionEditor.aspx?GroupID=" + strGroupID;
                        nextPage = "../Paper_TextQuestionEditorNew.aspx?GroupID=" + strGroupID;
                    }
                }
                 */
               // nextPage = "../Paper_QuestionTypeNew.aspx?Opener=./QuestionGroupTree/QGroupTreeNew&bModify=False&GroupID=" + strGroupID;
               // Session["QuestionFunction"] = "afterNew";//reset session QuestionFunction

               // Response.Redirect(nextPage);
           // }
            


            //判斷目前選擇的群組的root 是否為 SubjectiveRoot ObjectiveRoot AssessmentRoot PlanRoot
            //是則用新的問答題資料表的新頁面 否則用舊的
            hfRoot.Value = "false";
            QuestionGroupTree_SELECT_Root(strGroupID);

             
            if (hfRoot.Value == "true")
            {
                  
                if (Request.QueryString["CaseID"] != null)
                    nextPage = "../Paper_QuestionViewNew.aspx?GroupID=" + strGroupID + "&CaseID=" + Request.QueryString["CaseID"].ToString();
                else
                    nextPage = "../Paper_QuestionViewNew.aspx?GroupID=" + strGroupID;

                    
            }
            else
            {
                //編輯或刪除題目
                //nextPage = "../Paper_QuestionView.aspx?GroupID=" + strGroupID;

                    

                if (Request.QueryString["CaseID"] != null)
                    nextPage = "../Paper_QuestionViewNew.aspx?GroupID=" + strGroupID + "&CaseID=" + Request.QueryString["CaseID"].ToString();
                else
                    nextPage = "../Paper_QuestionViewNew.aspx?GroupID=" + strGroupID;
                    
            }
            


            
            
        }

        Response.Redirect(nextPage);

        //test 編輯或刪除題目 page


    }
    
    protected void DDLFeatureSet_SelectedIndexChanged(object sender, EventArgs e)
    {
        setDDLFeatureItem();
    }
    
    private void QuestionGroupTree_SELECT_Root(string strNodeID)
    {
        string strRootName = "";
        DataTable dtQuestionGroupTree = new DataTable();
        string strSQL_QuestionGroupTree = "SELECT * FROM QuestionGroupTree WHERE cNodeID = '" + strNodeID + "' ";
        dtQuestionGroupTree = hintsDB.getDataSet(strSQL_QuestionGroupTree).Tables[0];
        if (dtQuestionGroupTree.Rows.Count > 0)
        {
            strRootName = dtQuestionGroupTree.Rows[0]["cNodeName"].ToString();
            if (strRootName == "SubjectiveRoot" || strRootName == "ObjectiveRoot" || strRootName == "AssessmentRoot" || strRootName == "PlanRoot")
            {
                hfRoot.Value = "true";
            }
            else
            {
                string strParentID = "";
                strParentID = dtQuestionGroupTree.Rows[0]["cParentID"].ToString();
                QuestionGroupTree_SELECT_Root(strParentID);
            }
        }
    }

    //初始化ddlSelectQuestion下拉式選單 朱君 2012/10/24
    protected void setddlSelectQuestion()     
    {
        clsORCSDB ORCSDB = new clsORCSDB();
        //將物件放入ddlSelectQuestion下拉式選單
        DataTable dtSelectQuestion = new DataTable();
        //取得課程系級名稱
        string strDepartmentName = "";
        if (Request.QueryString["Career"] != null && Request.QueryString["Career"].ToString() != "") //Career有值表示為"職業問答樹"的題庫  老詹2015/08/27
        {
            string strSQL_SelectQuestion = "SELECT * FROM QuestionList ORDER BY qId";
            dtSelectQuestion = hintsDB.getDataSet(strSQL_SelectQuestion).Tables[0];

            //若沒在上課 檢查使用者屬於哪個系
            if (strDepartmentName == "")
            {
                string strSQL = "SELECT * FROM ORCS_MemberCourseTeacher MCT JOIN ORCS_ClassGroup CG ON MCT.iGroupID = CG.iClassGroupID JOIN ORCS_SchoolGroup SG ON CG.iSchoolGroupID = SG.iSchoolGroupID WHERE MCT.cUserID = '" + usi.UserID + "'";
                strDepartmentName = ORCSDB.GetDataSet(strSQL).Tables[0].Rows[0]["cSchoolGroupName"].ToString();
            }

            bool bIsNoHasDepartmentName = true;
            //檢查題庫是否有符合部門名稱 有的話預設為選此選項 沒有則預設為第一個
            if (strDepartmentName != "")
            {
                foreach (DataRow drSelectQuestion in dtSelectQuestion.Rows)
                {
                    //比對系級名稱前兩個字
                    if (drSelectQuestion["qName"].ToString().IndexOf(strDepartmentName.Substring(0, 2)) >= 0)
                        bIsNoHasDepartmentName = false;
                }
            }
            for (int i = 0; i < dtSelectQuestion.Rows.Count; i++)
            {
                ListItem li = new ListItem(dtSelectQuestion.Rows[i]["qName"].ToString(), dtSelectQuestion.Rows[i]["qId"].ToString());
                if ((i == 0 && bIsNoHasDepartmentName) || (!bIsNoHasDepartmentName && dtSelectQuestion.Rows[i]["qName"].ToString().IndexOf(strDepartmentName.Substring(0, 2)) >= 0))
                {
                    li.Selected = true;
                }
                ddlSelectQuestion.Items.Add(li);
            }
        }
        else
        {
            string strSQL_SelectQuestion = "SELECT * FROM QuestionList WHERE qId!='25' ORDER BY qId";
            dtSelectQuestion = hintsDB.getDataSet(strSQL_SelectQuestion).Tables[0];
            //如果正在上課會取得CourseID 否則為空
            string strCourseID = CheckClassState();
            if (!strCourseID.Equals(""))
            {
                //取得DepartmentID
                string strSQL = "SELECT * FROM ORCS_ClassGroup OC JOIN ORCS_SchoolGroup OS ON OC.iSchoolGroupID = OS.iSchoolGroupID WHERE OC.iClassGroupID ='" + strCourseID + "'";
                strDepartmentName = ORCSDB.GetDataSet(strSQL).Tables[0].Rows[0]["cSchoolGroupName"].ToString();
            }

            string strDepartmentNameTwo = "";
            string strSQLSG = "SELECT * FROM ORCS_MemberCourseTeacher MCT JOIN ORCS_ClassGroup CG ON MCT.iGroupID = CG.iClassGroupID JOIN ORCS_SchoolGroup SG ON CG.iSchoolGroupID = SG.iSchoolGroupID WHERE MCT.cUserID = '" + usi.UserID + "'";
            DataTable dtSchool = ORCSDB.GetDataSet(strSQLSG).Tables[0];
            if (dtSchool.Rows.Count >= 1)
            {
                foreach (DataRow drSchoolGroupName in dtSchool.Rows)
                    strDepartmentNameTwo += drSchoolGroupName["cSchoolGroupName"].ToString() + ",";
            }

            bool bIsNoHasDepartmentName = true;
            bool bIsNoHasDepartmentNameTwo = true;
            //檢查題庫是否有符合部門名稱 有的話預設為選此選項 沒有則預設為第一個
            if (strDepartmentName != "")
            {
                foreach (DataRow drSelectQuestion in dtSelectQuestion.Rows)
                {
                    //比對系級名稱前兩個字
                    if (drSelectQuestion["qName"].ToString().IndexOf(strDepartmentName.Substring(0, 2)) >= 0)
                        bIsNoHasDepartmentName = false;
                }
            }
            //代表沒找到 改成找使用者所屬
            if (bIsNoHasDepartmentName == true)
            {
                foreach (DataRow drSelectQuestion in dtSelectQuestion.Rows)
                {
                    //比對系級名稱前兩個字
                    if (strDepartmentNameTwo.IndexOf(drSelectQuestion["qName"].ToString().Substring(0, 2)) >= 0)
                        bIsNoHasDepartmentNameTwo = false;
                }
            }
            bool bIsSelected = false;
            for (int i = 0; i < dtSelectQuestion.Rows.Count; i++)
            {
                ListItem li = new ListItem(dtSelectQuestion.Rows[i]["qName"].ToString(), dtSelectQuestion.Rows[i]["qId"].ToString());
                if (!bIsSelected && ((i == 0 && bIsNoHasDepartmentName && bIsNoHasDepartmentNameTwo) || (!bIsNoHasDepartmentName && dtSelectQuestion.Rows[i]["qName"].ToString().IndexOf(strDepartmentName.Substring(0, 2)) >= 0)
                    || (!bIsNoHasDepartmentNameTwo && strDepartmentNameTwo.IndexOf(dtSelectQuestion.Rows[i]["qName"].ToString().Substring(0, 2)) >= 0)))
                {
                    bIsSelected = true;
                    li.Selected = true;
                }
                ddlSelectQuestion.Items.Add(li);
            }
        }
        //預設第一個題庫後的初始化
        SetSelectQuestion();
    }

    //按下特徵條件後，動態增加特徵條件
    protected void btnAddFeature_Click(object sender, EventArgs e)
    {
        btnFeatureDelete.Visible = true;
        
        //將session的值+1
        int ddlCount = Convert.ToInt32(Session["FeatureDDLNum"]);
        ddlCount += 1;
        Session["FeatureDDLNum"] = ddlCount.ToString();
        
        //將使用者目前所選擇的特徵集合與特徵物件存入session
        addFeatureSetValueToSession(DDLFeatureSet.SelectedValue.ToString());
        //建構特徵物件表格
        initFeatureItemTable();
        uppanelFeatureItem.Update();
    }
    
    //刪除所選擇的特徵物件
    protected void btnFeatureDelete_Click(object sender, EventArgs e)
    {
        //初始化特徵session
        initFeatureSession();
        btnFeatureDelete.Visible = false;
        uppanelFeatureItem.Update();

    }

    //將使用者目前所選擇的特徵集合存入session
    //將session裡的datatable取出，把選擇的物件資訊放入row後，再存回session中
    protected void addFeatureSetValueToSession(string strFeatureSetValue)
    {
        DataTable dtFeatureItem = (DataTable)Session["dtSelectedFeatureItem"];
        DataRow drFeatureRow = dtFeatureItem.NewRow();
        drFeatureRow["FaetureSetID"] = DDLFeatureSet.SelectedValue;
        drFeatureRow["FaetureSetName"] = DDLFeatureSet.SelectedItem.Text;
        drFeatureRow["FaetureItemID"] = DDLFeatureItem.SelectedValue;
        drFeatureRow["FaetureItemName"] = DDLFeatureItem.SelectedItem.Text;
        drFeatureRow["FaetureSearchMode"] = DDLSearchMode.SelectedItem.Text;
        drFeatureRow["FaetureSearchModeValue"] = DDLSearchMode.SelectedValue;
        dtFeatureItem.Rows.Add(drFeatureRow);
        Session["dtSelectedFeatureItem"] = dtFeatureItem;  
    }

    protected void btFeatureSearch_Click(object sender, EventArgs e)
    {
        //先將使用者所選擇的特徵屬性設為條件置資料庫搜尋符合的問題編號
        //儲存結果的DataTable
        DataTable dtQuestionResult = new DataTable();
        //題庫的編號 
        string QuestionListID=ddlSelectQuestion.SelectedValue.ToString(); 
        //從Session讀取使用者所選擇的FeatureItem
        DataTable dtFeatureItem = (DataTable)Session["dtSelectedFeatureItem"];
        //利用使用者所選擇的特徵值到資料庫搜尋相對應的題目與題目種類
        Session["dtSelectedFeatureItemResult"] = this.FeatureSearchResult(dtFeatureItem, QuestionListID); 
        //判斷目前是在什麼模式，決定按下search的頁面
        string nextPage = "";
        if (strModifyType == "Paper")
        {
            //編輯考卷
            if (strPresentType == "Edit")
            {
                if (!(strEditMode == "Automatic"))
                {
                    
                    nextPage = "../Paper_SelectQuestion.aspx?GroupID=" + strGroupID + "&SearchMode=Feature";
                    Response.Redirect(nextPage);
                }
            }
        }
        else
        {
            ////use JS alert() in C#
            ScriptManager.RegisterStartupScript(
             this,
             typeof(Page),
             "Alert",
            "<script>alert('strModifyType: " + Session["ModifyType"].ToString() + "');</script>",
             false);
            //Response.Write("<script>alert('Hi else OpenFeatureSearchResult');</script>");
            Page.RegisterStartupScript("js", "<script language=\"javascript\">OpenFeatureSearchResult()</script>");
            //建構特徵物件表格，因為按下Search按鈕postback後，特徵條件會消失，因此在建構一次
            initFeatureItemTable();
        }
    }
    
    //利用使用者所選擇的特徵值到資料庫搜尋相對應的題目與題目種類
    protected DataTable FeatureSearchResult(DataTable dtFeatureItem ,string QuestionListID)
    {
        //Ben test SQL query
        //Response.Write("<script>alert('Hi kID');</script>");
        //end Ben

        DataTable dtQuestionResult = new DataTable();

        if (dtFeatureItem.Rows.Count > 0)
        {
            /*
            //Ben test SQL query
            ////use JS alert() in C#
            ScriptManager.RegisterStartupScript(
             this,
             typeof(Page),
             "Alert",
            "<script>alert('dtFeatureItem.Rows.Count: " + dtFeatureItem.Rows.Count + "');</script>",
             false);
            //end Ben
            */

            clsHintsDB HintsDB = new clsHintsDB();
            string strSQL_QuestionResult = "SELECT DISTINCT A.strQuestionID AS cQID FROM FeatureForSelect A , QuestionMode B";
            //若沒有特徵條件，則不執行搜尋

            //透過特徵條建比對選擇題，且要在目前題庫下的題目才被搜尋到
            for (int i = 0; i < dtFeatureItem.Rows.Count; i++)
            {
                if (i == 0)
                    strSQL_QuestionResult += " WHERE A.strQuestionID=B.cQID AND";
                else
                {
                    //判斷Session中儲存的Mode為AND還是OR
                    if (dtFeatureItem.Rows[i]["FaetureSearchModeValue"].ToString() == "1")
                        strSQL_QuestionResult += " AND";
                    else
                        strSQL_QuestionResult += " OR";
                }
                strSQL_QuestionResult += " (A.strQuestionID IN (SELECT strQuestionID FROM FeatureForSelect AS FeatureForSelect_1 WHERE (iFeatureNum = '" + Convert.ToInt32(dtFeatureItem.Rows[i]["FaetureItemID"].ToString()) + "') AND (cNodeID  = '" + dtFeatureItem.Rows[i]["FaetureSetID"].ToString() + "'))) ";
            }
            //判斷是否在同一題庫下
            strSQL_QuestionResult += " AND (A.strQuestionID IN (SELECT A.cQID FROM  QuestionMode AS A INNER JOIN QuestionGroupTree AS B ON A.cQuestionGroupID = B.cNodeID WHERE (B.qId = '" + Convert.ToInt32(QuestionListID) + "')))";
            //Ben test SQL query
            //Response.Write("<script>console.log('" + strSQL_QuestionResult + "');</script>");//有進
            //end Ben
            
            dtQuestionResult = HintsDB.getDataSet(strSQL_QuestionResult).Tables[0];

            //Ben test SQL query
            //Response.Write("<script>console.log('" + dtQuestionResult.Rows.Count + "');</script>");
            //end Ben


            
        }
        /*
        ////use JS alert() in C#
        ScriptManager.RegisterStartupScript(
              this,
              typeof(Page),
              "Alert",
             "<script>alert('LOL: " + dtQuestionResult.Rows.Count + "');</script>",
              false);
        */



        //將上列搜尋到的結果為條件，進入資料庫配對相對應的題目類型
        dtQuestionResult.Columns.Add(new DataColumn("cQuestionType", typeof(string)));
        for (int i = 0; i < dtQuestionResult.Rows.Count; i++)
        {
            clsHintsDB HintsDB = new clsHintsDB();
            string strSQL = "SELECT DISTINCT cQuestionType FROM QuestionMode WHERE cQID='" + dtQuestionResult.Rows[i]["cQID"].ToString() + "'";
            dtQuestionResult.Rows[i]["cQuestionType"] = HintsDB.getDataSet(strSQL).Tables[0].Rows[0][0].ToString();
        }
        //Ben test SQL query

        //Response.Write("<script>alert('AIII');</script>");
        /*
        ////use JS alert() in C#
        ScriptManager.RegisterStartupScript(
              this,
              typeof(Page),
              "Alert",
             "<script>alert('LOL: " + dtQuestionResult.Rows.Count + "');</script>",
              false);
       */
        //end Ben
        
        return dtQuestionResult;  
    }

    protected void ddlSelectQuestion_SelectedIndexChanged(object sender, EventArgs e)
    {
        SetSelectQuestion();
    }

    //選擇題庫的下拉式選單事件
    private void SetSelectQuestion()
    {
        //創造題庫樹
        this.ConstructQuestionGroupTree();
        //顯示主要panel  
        if (ddlSelectQuestion.SelectedItem.Text == "職業問答樹") // 新增判斷Career參數是否為空，若非空則顯示編輯問診樹頁面  老詹 2015/05/26
        {
            Panel1.Visible = false;
            Panel2.Visible = true;
            ifPanel2.Attributes["src"] = "QGroupTreeNew_VoiceInquiry.aspx?Career=" + hf_Career.Value.ToString();
        }
        else
        {
            Panel1.Visible = true;
            Panel2.Visible = false;
        }
        //初始化特徵條件下拉選單
        initDDLFeatureSet();
        setDDLFeatureItem();
        initDDLSearchMode();
        //初始化特徵session和紀錄題庫代號到Session
        initFeatureSession();        
    }


    //檢查ORCS是否為上課狀態並回傳課程ID
    protected string CheckClassState()
    {
        clsORCSDB myDb = new clsORCSDB(); //呼叫ORCS資料庫
        string strClassID = ""; //定義正在上課的課程ID
        //抓取使用者的課程ID
        string strSQL = "SELECT * FROM " + clsGroup.TB_ORCS_MemberCourseTeacher + " WHERE cUserID = '" + usi.UserID + "'";
        DataTable dtGroupMember = new DataTable();
        dtGroupMember = myDb.GetDataSet(strSQL).Tables[0];
        if (dtGroupMember.Rows.Count > 0)
        {
            //抓取正在上課的課程ID
            foreach (DataRow drGroupMember in dtGroupMember.Rows)
            {
                strSQL = "SELECT * FROM ORCS_SystemControl WHERE iClassGroupID = '" + drGroupMember["iGroupID"].ToString() + "' AND cSysControlName = 'SystemControl'";
                DataTable dtSystemControl = new DataTable();
                dtSystemControl = myDb.GetDataSet(strSQL).Tables[0];
                if (dtSystemControl.Rows.Count > 0)
                {
                    if (dtSystemControl.Rows[0]["iSysControlParam"].ToString() != "0") //判斷該課程是否上課("0":非上課,"1":上課,"2":上課遲到)
                        strClassID = dtSystemControl.Rows[0]["iClassGroupID"].ToString();
                }
            }
        }
        return strClassID;//回傳課程ID
    }
    protected void tvQuestionGroup_SelectedNodeChanged(object sender, EventArgs e)
    {
        string strselect = tvQuestionGroup.SelectedNode.Value;
        strCurrentNodeID.Value = strselect;
        hiddenQuestionBankID.Value = strselect;
        Session["QuestionBankID"] = strselect;
        tdNodeID.InnerText = strselect;
        tdParentNodeName.InnerHtml = tvQuestionGroup.SelectedNode.Text;
        string strTemp = tvQuestionGroup.SelectedNode.Text;
        int iStart = strTemp.IndexOf('>');
        strTemp = strTemp.Substring(iStart+1);
        int iEnd =strTemp.IndexOf('<');
        tNodeName.Value = strTemp.Substring(0, iEnd);
        tNewNodeName.Text = "";

        if (strOpener == "Paper_QuestionType")
        {
            //show the name of the selected problem database
            hiddenQuestionBankName.Value = tNodeName.Value;

           // show modal_featureSearch
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "openModal();", true);
        }
       
    }
}
