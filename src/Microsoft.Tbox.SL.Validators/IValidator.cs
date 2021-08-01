/// <summary>
/// Interface that describes a visual validator
/// </summary>
public interface IValidator
{
    /// <summary>
    /// Shows the validator after the control has been invalidated
    /// </summary>
    void Show();

    /// <summary>
    /// Hides the validator after the control has been validated
    /// </summary>
    void Hide();
}