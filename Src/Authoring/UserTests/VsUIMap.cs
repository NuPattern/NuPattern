using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace NuPattern.Authoring.UserTests
{
    public class VsUIMap
    {
        private UIVsMainWindow mUIVsMainWindow;
        private UINewProjectWindow mUINewProjectWindow;
        private UINewPatternToolkitWindow mUINewPatternToolkitWindow;

        public UIVsMainWindow UIVsMainWindow
        {
            get
            {
                if ((this.mUIVsMainWindow == null))
                {
                    this.mUIVsMainWindow = new UIVsMainWindow();
                }
                return this.mUIVsMainWindow;
            }
        }

        public UINewProjectWindow UINewProjectWindow
        {
            get
            {
                if ((this.mUINewProjectWindow == null))
                {
                    this.mUINewProjectWindow = new UINewProjectWindow();
                }
                return this.mUINewProjectWindow;
            }
        }

        public UINewPatternToolkitWindow UINewPatternToolkitWindow
        {
            get
            {
                if ((this.mUINewPatternToolkitWindow == null))
                {
                    this.mUINewPatternToolkitWindow = new UINewPatternToolkitWindow();
                }
                return this.mUINewPatternToolkitWindow;
            }
        }
    }

    public class UIVsMainWindow : WpfWindow
    {
        public UIVsMainWindow()
        {
            #region Search Criteria
#if VSVER11
            this.SearchProperties[WpfTitleBar.PropertyNames.AutomationId] = "VisualStudioMainWindow";
#endif
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.Name, "- Microsoft Visual Studio - Experimental Instance", PropertyExpressionOperator.Contains));
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.WindowTitles.Add("Start Page - Microsoft Visual Studio - Experimental Instance");
            #endregion
        }
    }
    public class UINewProjectWindow : WpfWindow
    {
        private WpfEdit mUINameEdit;
        private WpfButton mUIOKButton;
        private WpfButton mUICancelButton;

        public UINewProjectWindow()
        {
            #region Search Criteria
            this.SearchProperties[WpfWindow.PropertyNames.Name] = "New Project";
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.WindowTitles.Add("New Project");
            #endregion
        }

        public WpfEdit UINameEdit
        {
            get
            {
                if ((this.mUINameEdit == null))
                {
                    this.mUINameEdit = new WpfEdit(this);
                    #region Search Criteria
                    this.mUINameEdit.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "txt_Name";
                    this.mUINameEdit.WindowTitles.Add("New Project");
                    #endregion
                }
                return this.mUINameEdit;
            }
        }

        public WpfButton UIOKButton
        {
            get
            {
                if ((this.mUIOKButton == null))
                {
                    this.mUIOKButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUIOKButton.SearchProperties[WpfButton.PropertyNames.AutomationId] = "btn_OK";
                    this.mUIOKButton.WindowTitles.Add("New Project");
                    #endregion
                }
                return this.mUIOKButton;
            }
        }

        public WpfButton UICancelButton
        {
            get
            {
                if ((this.mUICancelButton == null))
                {
                    this.mUICancelButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUICancelButton.SearchProperties[WpfButton.PropertyNames.AutomationId] = "btn_Cancel";
                    this.mUICancelButton.WindowTitles.Add("New Project");
                    #endregion
                }
                return this.mUICancelButton;
            }
        }
    }
    public class UINewPatternToolkitWindow : WpfWindow
    {
        private WpfButton mUINextButton;
        private WpfButton mUIBackButton;
        private WpfButton mUICancelButton;
        private WpfButton mUIFinishButton;
        private WpfRadioButton mUICreateanewtoolkitRadioButton;
        private WpfEdit mUIToolkitNameEditorEdit;
        private WpfEdit mUIPART_EditEdit;
        private WpfEdit mUIPART_EditEdit1;
        private WpfEdit mUIPART_EditEdit2;
        private WpfEdit mUIPART_EditEdit3;

        public UINewPatternToolkitWindow()
        {
            #region Search Criteria
            this.SearchProperties[WpfWindow.PropertyNames.Name] = "New Pattern Toolkit";
            this.SearchProperties.Add(new PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains));
            this.WindowTitles.Add("New Pattern Toolkit");
            #endregion
        }

        public WpfButton UIFinishButton
        {
            get
            {
                if ((this.mUIFinishButton == null))
                {
                    this.mUIFinishButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUIFinishButton.SearchProperties[WpfButton.PropertyNames.Name] = "Finish";
                    this.mUIFinishButton.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIFinishButton;
            }
        }

        public WpfButton UINextButton
        {
            get
            {
                if ((this.mUINextButton == null))
                {
                    this.mUINextButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUINextButton.SearchProperties[WpfButton.PropertyNames.AutomationId] = "BrowseForward";
                    this.mUINextButton.SearchProperties[WpfButton.PropertyNames.Instance] = "2";
                    this.mUINextButton.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUINextButton;
            }
        }

        public WpfButton UIBackButton
        {
            get
            {
                if ((this.mUIBackButton == null))
                {
                    this.mUIBackButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUIBackButton.SearchProperties[WpfButton.PropertyNames.AutomationId] = "BrowseBack";
                    this.mUIBackButton.SearchProperties[WpfButton.PropertyNames.Instance] = "2";
                    this.mUIBackButton.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIBackButton;
            }
        }

        public WpfButton UICancelButton
        {
            get
            {
                if ((this.mUICancelButton == null))
                {
                    this.mUICancelButton = new WpfButton(this);
                    #region Search Criteria
                    this.mUICancelButton.SearchProperties[WpfButton.PropertyNames.Name] = "Cancel";
                    this.mUICancelButton.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUICancelButton;
            }
        }

        public WpfRadioButton UICreateanewtoolkitRadioButton
        {
            get
            {
                if ((this.mUICreateanewtoolkitRadioButton == null))
                {
                    this.mUICreateanewtoolkitRadioButton = new WpfRadioButton(this);
                    #region Search Criteria
                    this.mUICreateanewtoolkitRadioButton.SearchProperties[WpfRadioButton.PropertyNames.AutomationId] = "NewRadio";
                    this.mUICreateanewtoolkitRadioButton.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUICreateanewtoolkitRadioButton;
            }
        }

        public WpfEdit UIToolkitNameEditorEdit
        {
            get
            {
                if ((this.mUIToolkitNameEditorEdit == null))
                {
                    this.mUIToolkitNameEditorEdit = new WpfEdit(this);
                    #region Search Criteria
                    this.mUIToolkitNameEditorEdit.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "ToolkitNameEditor";
                    this.mUIToolkitNameEditorEdit.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIToolkitNameEditorEdit;
            }
        }

        public WpfEdit UIPART_EditEdit
        {
            get
            {
                if ((this.mUIPART_EditEdit == null))
                {
                    this.mUIPART_EditEdit = new WpfEdit(this);
                    #region Search Criteria
                    this.mUIPART_EditEdit.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "PART_Edit";
                    this.mUIPART_EditEdit.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIPART_EditEdit;
            }
        }

        public WpfEdit UIPART_EditEdit1
        {
            get
            {
                if ((this.mUIPART_EditEdit1 == null))
                {
                    this.mUIPART_EditEdit1 = new WpfEdit(this);
                    #region Search Criteria
                    this.mUIPART_EditEdit1.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "PART_Edit";
                    this.mUIPART_EditEdit1.SearchProperties[WpfEdit.PropertyNames.Instance] = "2";
                    this.mUIPART_EditEdit1.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIPART_EditEdit1;
            }
        }

        public WpfEdit UIPART_EditEdit2
        {
            get
            {
                if ((this.mUIPART_EditEdit2 == null))
                {
                    this.mUIPART_EditEdit2 = new WpfEdit(this);
                    #region Search Criteria
                    this.mUIPART_EditEdit2.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "PART_Edit";
                    this.mUIPART_EditEdit2.SearchProperties[WpfEdit.PropertyNames.Instance] = "3";
                    this.mUIPART_EditEdit2.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIPART_EditEdit2;
            }
        }

        public WpfEdit UIPART_EditEdit3
        {
            get
            {
                if ((this.mUIPART_EditEdit3 == null))
                {
                    this.mUIPART_EditEdit3 = new WpfEdit(this);
                    #region Search Criteria
                    this.mUIPART_EditEdit3.SearchProperties[WpfEdit.PropertyNames.AutomationId] = "PART_Edit";
                    this.mUIPART_EditEdit3.SearchProperties[WpfEdit.PropertyNames.Instance] = "4";
                    this.mUIPART_EditEdit3.WindowTitles.Add("New Pattern Toolkit");
                    #endregion
                }
                return this.mUIPART_EditEdit3;
            }
        }
    }
}
