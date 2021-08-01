using System.Windows;
using System.Windows.Controls;

namespace Microsoft.TBox.SL.Validators.Demo
{
    public partial class Validator : UserControl, IValidator
    {
        /// <summary>
        /// Keeps track of the state of the control
        /// </summary>
        bool isShown = true;

        /// <summary>
        /// Initializes the validator
        /// </summary>
        public Validator()
        {
            // Init UI
            this.InitializeComponent();

            // Attach events
            this.Loaded += new RoutedEventHandler(Validator_Loaded);
        }

        /// <summary>
        /// Loads the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Validator_Loaded(object sender, RoutedEventArgs e)
        {
            // Hide the tooltip
            this.Hide();
        }

        /// <summary>
        /// Shows the control
        /// </summary>
        public void Show()
        {
            // Check if the control isn't already shown
            if (!this.isShown)
            {
                // Start the show animation
                this.ShowAnimation.Begin();
                this.isShown = true;
            }
        }

        /// <summary>
        /// Hides the control
        /// </summary>
        public void Hide()
        {
            // Check if the control is showing
            if (this.isShown)
            {
                // Start the hide anumation
                this.HideAnimation.Begin();
                this.isShown = false;
            }
        }
    }
}
