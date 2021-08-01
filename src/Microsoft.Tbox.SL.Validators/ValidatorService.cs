using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Microsoft.Tbox.SL.Validators
{
    /// <summary>
    /// Defines the type of validators
    /// </summary>
    public enum ValidatorTypes
    {
        Required,
        RegularExpression,
        Range,
        Compare,
        Custom,
        None
    }

    /// <summary>
    /// Create a delegate type that performs validation for an element
    /// </summary>
    /// <returns></returns>
    public delegate bool ValidationDelegate(UIElement element);

    /// <summary>
    /// The validator service class
    /// 
    /// This class defines a couple of Attached Dependency Properties that allow you to register validation behavior 
    /// for a range of input controls. E.g. TextBox, ComboBox, ListBox, DatePicker and RadioGroup.
    /// 
    /// As soon as Silverlight parses a XAML file and finds these properties the class will store references to those
    /// controls internally. As soon as the ValidatorService.Validate() method is called, all the controls will be 
    /// validated. In some cases this class listens to events on the input controls, allowing them to be validated 
    /// when they are changed.
    /// </summary>
    public static class ValidatorService
    {
        #region Attached Dependency Properties (RegisterValidator, Validate, ValidatorType, Validator, ValidatorParameter, ValidationGroup)

        #region RegisterValidator Property

        private static readonly DependencyProperty RegisterValidatorProperty = DependencyProperty.RegisterAttached("RegisterValidator",
            typeof(string),
            typeof(ValidatorService),
            new PropertyMetadata(RegisterValidatorPropertyChanged));

        public static string GetRegisterValidator(UIElement element)
        {
            return element.GetValue(ValidatorService.RegisterValidatorProperty).ToString();
        }

        public static void SetRegisterValidator(UIElement element, string value)
        {
            element.SetValue(ValidatorService.RegisterValidatorProperty, value);
        }

        private static void RegisterValidatorPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Unbox as a UIelement
            UIElement dependencyObject = o as UIElement;

            // Find the key of the validator
            string validatorKey = dependencyObject.GetValue(ValidatorService.RegisterValidatorProperty).ToString();

            // Add the validator to the list
            if (!ValidatorService.validators.ContainsKey(validatorKey))
                ValidatorService.validators.Add(validatorKey, dependencyObject);
        }

        #endregion

        #region Validate Property

        public static readonly DependencyProperty ValidateProperty = DependencyProperty.RegisterAttached("Validate",
            typeof(ValidationDelegate),
            typeof(ValidatorService),
            null);

        public static ValidationDelegate GetValidate(UIElement element, ValidationDelegate value)
        {
            // Returns the value of the dependancy property 
            return (ValidationDelegate)element.GetValue(ValidatorService.ValidateProperty);
        }

        public static void SetValidate(UIElement element, ValidationDelegate value)
        {
            // Sets the value on the dependancy propery
            element.SetValue(ValidatorService.ValidateProperty, value);
        }

        #endregion

        #region ValidatorType Property

        public static readonly DependencyProperty ValidatorTypeProperty = DependencyProperty.RegisterAttached("ValidatorType",
            typeof(ValidatorTypes),
            typeof(ValidatorService),
            new PropertyMetadata(ValidatorTypes.None, ValidatorTypePropertyChanged));

        public static ValidatorTypes GetValidatorType(UIElement element, ValidatorTypes value)
        {
            // Returns the value of the dependancy property 
            return (ValidatorTypes)element.GetValue(ValidatorService.ValidatorTypeProperty);
        }

        public static void SetValidatorType(UIElement element, ValidatorTypes value)
        {
            // Sets the value on the dependancy propery
            element.SetValue(ValidatorService.ValidatorTypeProperty, value);
        }

        private static void ValidatorTypePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Unbox as a UIelement
            UIElement dependencyObject = o as UIElement;

            // Check for enable and attach 
            if (!uiElements.Contains(dependencyObject))
            {
                // Initialize the element
                InitializeElement(dependencyObject);

                // Add the element to the list of validating elements
                uiElements.Add(dependencyObject);
            }
        }

        #endregion

        #region Validator Property

        private static readonly DependencyProperty ValidatorProperty = DependencyProperty.RegisterAttached("Validator",
            typeof(string),
            typeof(ValidatorService),
            new PropertyMetadata(ValidatorPropertyChanged));

        public static string GetValidator(UIElement element, string value)
        {
            // Returns the value of the dependancy property 
            return element.GetValue(ValidatorService.ValidatorProperty).ToString();
        }

        public static void SetValidator(UIElement element, string value)
        {
            // Sets the value on the dependancy propery
            element.SetValue(ValidatorService.ValidatorProperty, value);
        }

        private static void ValidatorPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            // Unbox as a UIelement
            UIElement dependencyObject = o as UIElement;
        }

        #endregion

        #region ValidatorParameter Property

        private static readonly DependencyProperty ValidatorParameterProperty = DependencyProperty.RegisterAttached("ValidatorParameter",
           typeof(string),
           typeof(ValidatorService),
           null);

        public static string GetValidatorParameter(UIElement element, string value)
        {
            // Returns the value of the dependancy property 
            return element.GetValue(ValidatorService.ValidatorParameterProperty).ToString();
        }

        public static void SetValidatorParameter(UIElement element, string value)
        {
            // Sets the value on the dependancy propery
            element.SetValue(ValidatorService.ValidatorParameterProperty, value);
        }

        #endregion

        #region ValidationGroup

        private static readonly DependencyProperty ValidationGroupProperty = DependencyProperty.RegisterAttached("ValidationGroup",
           typeof(string),
           typeof(ValidatorService),
           null);

        public static string GetValidationGroup(UIElement element)
        {
            return element.GetValue(ValidatorService.ValidationGroupProperty).ToString();
        }

        public static void SetValidationGroup(UIElement element, string value)
        {
            element.SetValue(ValidatorService.ValidationGroupProperty, value);
        }

        #endregion

        #endregion

        /// <summary>
        /// A collection of UIElements that have registered for validation
        /// </summary>
        private static List<UIElement> uiElements = new List<UIElement>();

        /// <summary>
        /// A collection of IValidators that have been registered for validation
        /// </summary>
        private static Dictionary<string, UIElement> validators = new Dictionary<string, UIElement>();

        /// <summary>
        /// Initializes an element
        /// </summary>
        /// <param name="dependencyObject"></param>
        private static void InitializeElement(UIElement uiElement)
        {
            // Retrieve the type of validator from the element
            var validatorType = (ValidatorTypes)uiElement.GetValue(ValidatorService.ValidatorTypeProperty);

            // Switch of the types
            switch (validatorType)
            {
                case ValidatorTypes.Required:
                    InitializeRequired(uiElement);
                    break;

                case ValidatorTypes.RegularExpression:
                    InitializeRegularExpression(uiElement);
                    break;

                case ValidatorTypes.Range:
                    InitializeRange(uiElement);
                    break;

                case ValidatorTypes.Compare:
                    InitializeCompare(uiElement);
                    break;

                default:
                    break;
            }

            // Attach validation events to the uiElement
            AttachValidationEvents(uiElement);

            // Attach validation group
            AttachValidationGroup(uiElement);
        }

        /// <summary>
        /// Detects if the UIElement is part of a validation group
        /// </summary>
        /// <param name="uiElement"></param>
        private static void AttachValidationGroup(UIElement uiElement)
        {
            // Cast the uiElement to a FrameworkElement
            var frameworkElement = uiElement as FrameworkElement;

            // Check if the uiElement is a FrameworkElement
            if (frameworkElement != null)
            {
                // Attach a Loaded event handler that initializes the group
                frameworkElement.Loaded += delegate(object sender, RoutedEventArgs e)
                {
                    // Initialize the validation group in the Loaded event, so we can traverse the Visual Tree
                    InitializeValidationGroup(uiElement);
                };
            }
        }

        /// <summary>
        /// Initializes the validationgroup for a UIElement
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeValidationGroup(UIElement uiElement)
        {
            // Find the parent of this element
            var parentElement = uiElement;

            // Loop through all the parents
            while (parentElement != null)
            {
                // Check for a ValidationGroup parameter
                var validationGroup = parentElement.GetValue(ValidatorService.ValidationGroupProperty);

                // Set the validation group on the original uiElement
                if (validationGroup != null)
                {
                    // Set the group on the uiElement
                    uiElement.SetValue(ValidatorService.ValidationGroupProperty, validationGroup.ToString());
                    break;
                }

                // Find the parent of the parent
                parentElement = VisualTreeHelper.GetParent(parentElement) as UIElement;
            }
        }

        /// <summary>
        /// Attaches validation events to the uiElement. (e.g. TextBox.TextChanged)
        /// </summary>
        /// <param name="uiElement"></param>
        private static void AttachValidationEvents(UIElement uiElement)
        {
            // Create an event that keeps on validating
            if (uiElement is TextBox)
            {
                // Cast the uiElement
                TextBox tbUIElement = uiElement as TextBox;

                // Attach a textchanged event to the validator
                tbUIElement.TextChanged += delegate(object sender, TextChangedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
            if (uiElement is PasswordBox)
            {
                // Cast the uiElement
                PasswordBox tbUIElement = uiElement as PasswordBox;

                // Attach a textchanged event to the validator
                tbUIElement.PasswordChanged += delegate(object sender, RoutedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
            else if (uiElement is Selector)
            {
                // Cast the uiElement
                Selector selectorElement = uiElement as Selector;

                // Attach a changed event
                selectorElement.SelectionChanged += delegate(object sender, SelectionChangedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
            else if (uiElement is DatePicker)
            {
                // Cast the uiElement
                DatePicker dpElement = uiElement as DatePicker;

                // Attach a changed event
                dpElement.SelectedDateChanged += delegate(object sender, SelectionChangedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
            else if (uiElement is RadioGroup)
            {
                // Find the radiogroup
                RadioGroup radioGroup = uiElement as RadioGroup;

                // Attach a changed event
                radioGroup.SelectionChanged += delegate(object sender, SelectionChangedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
            else if (uiElement is CheckBox)
            {
                // Find the radiogroup
                CheckBox checkBox = uiElement as CheckBox;

                // Attach a changed event
                checkBox.Checked += delegate(object sender, RoutedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };

                // Attach a changed event
                checkBox.Unchecked += delegate(object sender, RoutedEventArgs e)
                {
                    ValidatorService.ValidateElement(uiElement);
                };
            }
        }

        /// <summary>
        /// Initializes the ValidationParameter
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeValidationParameter(UIElement uiElement)
        {
            // Find the validation parameter
            var validationParameter = uiElement.GetValue(ValidatorService.ValidatorParameterProperty);

            // Check if the parameter is correct
            if (validationParameter != null && string.IsNullOrEmpty(validationParameter.ToString()))
                throw new ArgumentNullException("ValidationParameter for UIElement has not been set.");
        }

        /// <summary>
        /// Initialize an element with an RequiredValidator
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeRequired(UIElement uiElement)
        {
            // Attach the RequiredValidator
            uiElement.SetValue(ValidatorService.ValidateProperty, new ValidationDelegate(ValidateRequired));
        }

        /// <summary>
        ///  Initialize an element with an RegularExpressionValidator
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeRegularExpression(UIElement uiElement)
        {
            // Attach the RequiredValidator
            uiElement.SetValue(ValidatorService.ValidateProperty, new ValidationDelegate(ValidateRegularExpression));

            // Check if the UIElement has ValidationParameters
            InitializeValidationParameter(uiElement);
        }

        /// <summary>
        /// Initialize an element with an RangeValidator
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeRange(UIElement uiElement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialize an element with an CompareValidator
        /// </summary>
        /// <param name="uiElement"></param>
        private static void InitializeCompare(UIElement uiElement)
        {
            // Attach the RequiredValidator
            uiElement.SetValue(ValidatorService.ValidateProperty, new ValidationDelegate(ValidateCompare));

            // Check if the UIElement has ValidationParameters
            InitializeValidationParameter(uiElement);
        }

        /// <summary>
        /// Validates an element
        /// </summary>
        private static bool ValidateElement(UIElement uiElement)
        {
            // Retrieve the ValidateDelegate of the uiElement
            ValidationDelegate validate = (ValidationDelegate)uiElement.GetValue(ValidatorService.ValidateProperty);

            // Handle null
            if (validate == null)
                return true;

            // Invoke the validator
            bool validated = validate(uiElement);

            // Find the validatorkey
            string validatorKey = uiElement.GetValue(ValidatorService.ValidatorProperty).ToString();

            // Find the IValidator for the key
            if (ValidatorService.validators.ContainsKey(validatorKey))
            {
                // Find the IValidator instance
                IValidator validator = ValidatorService.validators[validatorKey] as IValidator;

                // Handle null
                if (validator != null)
                {
                    // Enable the validator
                    if (!validated) validator.Show();
                    else validator.Hide();
                }
            }

            return validated;
        }

        /// <summary>
        /// Validates all elements
        /// </summary>
        /// <param name="validationGroup"></param>
        /// <returns></returns>
        public static bool Validate()
        {
            return Validate(null);
        }

        /// <summary>
        /// Validates all elements in a given group
        /// </summary>
        public static bool Validate(string validationGroup)
        {
            // Create a list of uiElements that didn't validate
            List<UIElement> notValidatedElements = new List<UIElement>();

            // Element to focus
            UIElement focusElement = null;
            
            // Validate each element
            foreach (var uiElement in uiElements)
            {
                // Check for the correct validationgroup
                if (validationGroup != null)
                {
                    // Find the group of the element
                    var validationGroupElement = uiElement.GetValue(ValidatorService.ValidationGroupProperty);

                    // Continue with other elements if the groups doesn't match
                    if (validationGroupElement == null || !validationGroupElement.ToString().Equals(validationGroup))
                        continue;
                }

                // Validate the element
                if (!ValidateElement(uiElement))
                {
                    // Add the element to the collection
                    notValidatedElements.Add(uiElement);

                    // Check for first element
                    if (focusElement == null)
                        focusElement = uiElement;
                }
            }

            // Check for focusElement
            if (focusElement != null && focusElement is Control)
                ((Control)focusElement).Focus();

            // Return a bool that indicated if all items were validated
            return !(notValidatedElements.Count > 0);
        }

        #region Validate Types (Required, RegularExpression, etc...)

        /// <summary>
        /// Validates a RequiredValidator
        /// </summary>
        /// <returns></returns>
        private static bool ValidateRequired(UIElement uiElement)
        {
            // Create a return value
            bool returnValue = false;

            // Validate the TextBox type
            if (uiElement is TextBox)
            {
                // Check for empty string
                returnValue = !string.IsNullOrEmpty(((TextBox)uiElement).Text);
            }
            // Validate the TextBox type
            else if (uiElement is PasswordBox)
            {
                // Check for empty string
                returnValue = !string.IsNullOrEmpty(((PasswordBox)uiElement).Password);
            }
            // Validate the Selector type (ListBox, ComboBox, ...)
            else if (uiElement is Selector)
            {
                // Check for selected the first index
                returnValue = ((Selector)uiElement).SelectedIndex > 0;
            }
            // Validate the DatePicker type
            else if (uiElement is DatePicker)
            {
                // Check for a selected date
                returnValue = ((DatePicker)uiElement).SelectedDate.HasValue;
            }
            // Validate a StackPanel filled with radio buttons
            else if (uiElement is RadioGroup)
            {
                // Try to see if the StackPanel contains radiobuttons
                returnValue = (uiElement as RadioGroup).HasSelection();
            }
            // Validate a Checkbox
            else if (uiElement is CheckBox)
            {
                // Try to see if the StackPanel contains radiobuttons
                returnValue = (uiElement as CheckBox).IsChecked.HasValue ? (uiElement as CheckBox).IsChecked.Value : false;
            }
            else
            {
                throw new NotImplementedException();
            }

            return returnValue;
        }

        /// <summary>
        /// Validates a RegularExpressionValidator
        /// </summary>
        /// <returns></returns>
        private static bool ValidateRegularExpression(UIElement uiElement)
        {
            // Create a return value
            bool returnValue = false;

            // ValidationValue
            string controlValue = string.Empty;

            // Validate the TextBox type
            if (uiElement is TextBox)
            {
                // Read the text property from the element
                controlValue = ((TextBox)uiElement).Text;
            }
            else
            {
                throw new NotImplementedException();
            }

            try
            {
                // Find the validatorparameter
                var validatorParameter = uiElement.GetValue(ValidatorService.ValidatorParameterProperty).ToString();

                // Create a regex match for the control
                Match match = Regex.Match(controlValue, validatorParameter);

                // Check for a successful and (non-partial) match
                returnValue = (match.Success && (match.Index == 0)) && (match.Length == controlValue.Length);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("The RegularExpression could not be executed.", ex);
            }

            // Return
            return returnValue;
        }

        /// <summary>
        /// Validates a RangeValidator
        /// </summary>
        /// <returns></returns>
        private static bool ValidateRange(UIElement uiElement)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates a CompareValidator
        /// </summary>
        /// <returns></returns>
        private static bool ValidateCompare(UIElement uiElement)
        {
            // Find the element to compare
            var compareElementName = uiElement.GetValue(ValidatorService.ValidatorParameterProperty).ToString();

            // Find the parent
            var parentElement = VisualTreeHelper.GetParent(uiElement) as FrameworkElement;

            // Find a reference to the element
            var compareElement = parentElement.FindName(compareElementName) as FrameworkElement;

            // Create a return value
            bool returnValue = false;

            // Check for null
            if (compareElement != null)
            {
                // Compare textboxes
                if (uiElement is TextBox && compareElement is TextBox)
                    returnValue = ((TextBox)uiElement).Text.CompareTo(((TextBox)compareElement).Text) > -1;

                // Compare textboxes
                else if (uiElement is PasswordBox && compareElement is PasswordBox)
                    returnValue = ((PasswordBox)uiElement).Password.CompareTo(((PasswordBox)compareElement).Password) > -1;
            }

            // Return
            return returnValue;
        }

        #endregion
    }
}