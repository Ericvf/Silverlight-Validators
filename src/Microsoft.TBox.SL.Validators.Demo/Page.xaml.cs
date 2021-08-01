using System.Windows;
using System.Windows.Controls;
using Microsoft.Tbox.SL.Validators;

namespace Microsoft.TBox.SL.Validators.Demo
{
    /// <summary>
    /// Main page for the Demo application
    /// </summary>
    public partial class Page : UserControl
    {
        /// <summary>
        /// Initializes the page
        /// </summary>
        public Page()
        {
            // Init UI
            InitializeComponent();

            // Attach events
            Loaded += new RoutedEventHandler(Page_Loaded);
        }

        /// <summary>
        /// Loads the PAge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Create a custom validation script
            ValidationDelegate customValidation = new ValidationDelegate(this.validateSocialSecurityNo);

            // Set the custom validation on the UIElement
            this.customVal.SetCustomValidation(customValidation);
        }

        /// <summary>
        /// Validates the SocialSecurityNoField for 10 digits
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        bool validateSocialSecurityNo(UIElement uiElement)
        {
            // Check for exactly 10 digits
            return (customVal.Text.Length == 10);
        }

        /// <summary>
        /// Handles Validation #01 button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidationGroup1_Click(object sender, RoutedEventArgs e)
        {
            // Validate the form
            ValidatorService.Validate("Section 1");
        }

        /// <summary>
        /// Handles Validation #02 button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValidationGroup2_Click(object sender, RoutedEventArgs e)
        {
            // Validate the form
            ValidatorService.Validate("Section 2");
        }
    }
}
