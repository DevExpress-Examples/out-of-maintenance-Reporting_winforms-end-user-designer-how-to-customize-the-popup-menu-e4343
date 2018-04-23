Imports System
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports DevExpress.XtraReports.Design
Imports DevExpress.XtraReports.Design.Commands
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
' ...

Namespace E4343
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
		End Sub
		Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles button1.Click
			Dim report As New XtraReport()
			Dim form As New XRDesignForm()
			form.DesignMdiController.AddService(GetType(IMenuCreationService), New CustomMenuCreationService(form.DesignMdiController))
			form.OpenReport(report)
			form.ShowDialog()
		End Sub
	End Class
	Friend Class CustomMenuCreationService
		Implements IMenuCreationService

		Private controller As XRDesignMdiController
		Private ReadOnly Property ActiveReport() As XtraReport
			Get
				Return If(controller.ActiveDesignPanel IsNot Nothing, controller.ActiveDesignPanel.Report, Nothing)
			End Get
		End Property
		Private showGridCommandID As CommandID
		Private hideGridCommandID As CommandID
		Public Sub New(ByVal controller As XRDesignMdiController)
			Me.controller = controller
			Dim guid As Guid = System.Guid.NewGuid()
			showGridCommandID = New CommandID(guid, 0)
			hideGridCommandID = New CommandID(guid, 1)
		End Sub
		Public Sub ProcessMenuItems(ByVal menuKind As MenuKind, ByVal items As MenuItemDescriptionCollection) Implements IMenuCreationService.ProcessMenuItems
			If menuKind = DevExpress.XtraReports.UserDesigner.MenuKind.Selection Then
				Dim index As Integer = Math.Max(0, items.IndexOf(ReportCommand.Cut))
				Dim itemShow As New MenuItemDescription("Show Grid", Nothing, showGridCommandID)
				Dim itemHide As New MenuItemDescription("Hide Grid", Nothing, hideGridCommandID)
				items.Insert(index, MenuItemDescription.Separator)
				index += 1
				items.Insert(index, itemShow)
				index += 1
				items.Insert(index, MenuItemDescription.Separator)
				index += 1
				items.Insert(index, itemHide)
				index += 1
				items.Insert(index, MenuItemDescription.Separator)
				index += 1
			End If
		End Sub
		Public Function GetCustomMenuCommands() As MenuCommandDescription() Implements IMenuCreationService.GetCustomMenuCommands
            Return New MenuCommandDescription() {
                New MenuCommandDescription(showGridCommandID, AddressOf OnHandleShowGrid, AddressOf OnStatusShowGrid),
                New MenuCommandDescription(hideGridCommandID, AddressOf OnHandleHideGrid, AddressOf OnStatusHideGrid)
            }
        End Function
        Private Sub OnHandleShowGrid(ByVal sender As Object, ByVal e As CommandExecuteEventArgs)
            SetDrawGrid(True)
        End Sub
        Private Sub OnHandleHideGrid(ByVal sender As Object, ByVal e As CommandExecuteEventArgs)
            SetDrawGrid(False)
        End Sub
        Private Sub SetDrawGrid(ByVal value As Boolean)
            Dim [property] As PropertyDescriptor = TypeDescriptor.GetProperties(GetType(XtraReport))("DrawGrid")
            [property].SetValue(controller.ActiveDesignPanel.Report, value)
        End Sub
        Private Sub OnStatusShowGrid(ByVal sender As Object, ByVal e As EventArgs)
            Dim command As MenuCommand = TryCast(sender, MenuCommand)
            command.Enabled = ActiveReport IsNot Nothing AndAlso Not ActiveReport.DrawGrid
            command.Supported = command.Enabled
        End Sub
        Private Sub OnStatusHideGrid(ByVal sender As Object, ByVal e As EventArgs)
            Dim command As MenuCommand = TryCast(sender, MenuCommand)
            command.Enabled = ActiveReport IsNot Nothing AndAlso ActiveReport.DrawGrid
            command.Supported = command.Enabled
        End Sub
    End Class
End Namespace
