using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
// ...

namespace WindowsFormsApplication122 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e) {
            XtraReport report = new XtraReport();
            XRDesignForm form = new XRDesignForm();
            form.DesignMdiController.AddService(typeof(IMenuCreationService),
                new CustomMenuCreationService(form.DesignMdiController));
            form.OpenReport(report);
            form.ShowDialog();
        }
    }
    class CustomMenuCreationService : IMenuCreationService {
        XRDesignMdiController controller;
        XtraReport ActiveReport {
            get {
                return controller.ActiveDesignPanel != null ? controller.ActiveDesignPanel.Report : null;
            }
        }
        CommandID showGridCommandID;
        CommandID hideGridCommandID;
        public CustomMenuCreationService(XRDesignMdiController controller) {
            this.controller = controller;
            Guid guid = Guid.NewGuid();
            showGridCommandID = new CommandID(guid, 0);
            hideGridCommandID = new CommandID(guid, 1);
        }
        public void ProcessMenuItems(MenuKind menuKind, MenuItemDescriptionCollection items) {
            if (menuKind == MenuKind.Selection) {
                int index = items.IndexOf(ReportCommand.Copy) - 1;
                if (index < 0)
                    return;
                MenuItemDescription itemShow = new MenuItemDescription("Show Grid", null, showGridCommandID);
                MenuItemDescription itemHide = new MenuItemDescription("Hide Grid", null, hideGridCommandID);
                items.Insert(index++, MenuItemDescription.Separator);
                items.Insert(index++, itemShow);
                items.Insert(index++, itemHide);
                items.Insert(index++, MenuItemDescription.Separator);
            }
        }
        public MenuCommandDescription[] GetCustomMenuCommands() {
            return new MenuCommandDescription[] { new MenuCommandDescription(showGridCommandID, OnHandleShowGrid, OnStatusShowGrid),
                new MenuCommandDescription(hideGridCommandID, OnHandleHideGrid, OnStatusHideGrid)};
        }
        void OnHandleShowGrid(object sender, CommandExecuteEventArgs e) {
            SetDrawGrid(true);
        }
        void OnHandleHideGrid(object sender, CommandExecuteEventArgs e) {
            SetDrawGrid(false);
        }
        void SetDrawGrid(bool value) {
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(XtraReport))["DrawGrid"];
            property.SetValue(controller.ActiveDesignPanel.Report, value);
        }
        void OnStatusShowGrid(object sender, EventArgs e) {
            MenuCommand command = sender as MenuCommand;
            command.Supported = command.Enabled = ActiveReport != null && !ActiveReport.DrawGrid;
        }
        void OnStatusHideGrid(object sender, EventArgs e) {
            MenuCommand command = sender as MenuCommand;
            command.Supported = command.Enabled = ActiveReport != null && ActiveReport.DrawGrid;
        }
    }
}
