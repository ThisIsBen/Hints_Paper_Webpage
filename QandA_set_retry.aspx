<%@ Reference Page="~/AuthoringTool/BasicForm/BasicForm.aspx" %>
<%@ Page language="c#" Inherits="PaperSystem.QandA_set_retry" CodeFile="QandA_set_retry.aspx.cs" %>

<HTML>
	<HEAD runat="server">
		<title>QandA_set_retry</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body id="body_content">
		<form id="Form1" method="post" runat="server">
			Case ID:<%=cCaseID%>
			�E���G<%=CurrentTerm%>
			�D���G<%=cQID%>
			SectionName:<%=SectionName%>
			<TABLE id="Table1" style="WIDTH: 1016px; HEIGHT: 248px" cellSpacing="1" cellPadding="1"
				width="1016" border="1">
				<TR>
					<TD><FONT face="�s�ө���">���D���@�\��]�w</FONT></TD>
				</TR>
				<TR>
					<TD><asp:RadioButton id="Radio1" runat="server" Text="1." Width="24px" AutoPostBack="True" GroupName="RG1" oncheckedchanged="Radio1_CheckedChanged"></asp:RadioButton>�����z�|�A�����i�J�U�@�D�C</TD>
				</TR>
				<TR>
					<TD><asp:RadioButton id="Radio2" runat="server" Text="2." Width="24px" AutoPostBack="True" GroupName="RG1" oncheckedchanged="Radio2_CheckedChanged"></asp:RadioButton>�i�����׿��~�Ʀ������������ܡA�Y�b�F��
						<asp:TextBox id="TextBox1" AutoPostBack="True" runat="server" Width="24px" ontextchanged="TextBox1_TextChanged"></asp:TextBox>���ɤ��M�����N�����z�|�A�i�J�U�@�D�F�p�G���@���ᵪ��i�H�o��
						<asp:TextBox id="TextBox2" runat="server" Width="28px" ontextchanged="TextBox2_TextChanged"></asp:TextBox>�H�����ơC</TD>
				<TR>
					<TD><asp:RadioButton id="Radio3" runat="server" Text="3." Width="24px" AutoPostBack="True" GroupName="RG1" oncheckedchanged="Radio3_CheckedChanged"></asp:RadioButton>�i�����׿��~�B�������ܡA�Y�b�F��
						<asp:TextBox id="TextBox3" AutoPostBack="True" runat="server" Width="24px" ontextchanged="TextBox3_TextChanged"></asp:TextBox>���ɤ��M�����N�����z�|�A�i�J�U�@�D�F�p�G���@���ᵪ��i�H�o�쪺
						<asp:TextBox id="TextBox4" AutoPostBack="True" runat="server" Width="28px" ontextchanged="TextBox4_TextChanged"></asp:TextBox>%���ơC</TD>
				</TR>
				<TR>
					<TD><asp:RadioButton id="Radio4" runat="server" Text="4." Width="24px" AutoPostBack="True" GroupName="RG1" oncheckedchanged="Radio4_CheckedChanged"></asp:RadioButton>�i�����׿��~�A���������������ܡA����ǲߪ̿�ܥ��T���סA�A�i�J�U�@�D�C</TD>
				</TR>
				<TR>
					<TD><asp:RadioButton id="Radio5" runat="server" Text="5." Width="24px" AutoPostBack="True" GroupName="RG1" oncheckedchanged="Radio5_CheckedChanged"></asp:RadioButton>�i�����׿��~�A�P�ɵ����������ܡA����ǲߪ̿�ܥ��T���סA�A�i�J�U�@�D�C</TD>
				</TR>
			</TABLE>
			<br />
			&nbsp;<asp:button id="Button1" runat="server" Text="�T�w" onclick="Button1_Click" CssClass="button_continue"></asp:button></form>
	</body>
</HTML>
