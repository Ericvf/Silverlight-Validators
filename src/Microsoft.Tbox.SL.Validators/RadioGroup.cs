using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Tbox.SL.Validators
{
    /// <summary>
    /// RadioGroup 
    /// </summary>
    public class RadioGroup : StackPanel
    {
        /// <summary>
        /// Event that notifies listeners for changes in radiobuttons
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Keeps a list of all local radio buttons
        /// </summary>
        private List<RadioButton> radioButtons = new List<RadioButton>();

        /// <summary>
        /// Gets the RadioButtons
        /// </summary>
        public List<RadioButton> RadioButtons
        {
            get
            {
                return this.radioButtons;
            }
        }

        /// <summary>
        /// Initializes the RadioGroup
        /// </summary>
        public RadioGroup()
        {
            // Attach events
            this.Loaded += new System.Windows.RoutedEventHandler(RadioGroup_Loaded);
        }

        /// <summary>
        /// Loads the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioGroup_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Loop through all the children and see if they're radiobuttons
            foreach (var childElement in this.Children)
            {
                // Check the type
                if (!(childElement is RadioButton))
                    throw new ArgumentException("Children of RadioGroup must be of type RadioButton.");

                // Add the Checked event to the RadioButton
                (childElement as RadioButton).Checked += new System.Windows.RoutedEventHandler(RadioButton_Checked);

                // Add the radiobutton to the children collection
                this.radioButtons.Add(childElement as RadioButton);
            }
        }

        /// <summary>
        /// Handles the checked event for each radiobutton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            // Raise the selectionChanged event
            if (this.SelectionChanged != null)
                this.SelectionChanged(sender, null);
        }

        /// <summary>
        /// Checks of at least one radiobutton has been selected
        /// </summary>
        /// <returns></returns>
        public bool HasSelection()
        {
            // Create a returnvalue
            bool returnValue = false;

            // Loop through all the items
            foreach (var radioButton in this.RadioButtons)
            {
                // See if the button is checked
                if (radioButton.IsChecked.HasValue && radioButton.IsChecked.Value)
                {
                    returnValue = true;
                    break;
                }
            }

            // Return
            return returnValue;
        }
    }
}
