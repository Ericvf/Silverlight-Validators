# Silverlight-Validators
Silverlight Validation control for forms

Quick and simple XAML declarations:
```xaml
<validators:RadioGroup Orientation="Horizontal" Grid.Column="1" Grid.Row="4" VerticalAlignment="Center"
    validators:ValidatorService.Validator="gender" 
    validators:ValidatorService.ValidatorType="Required"
> 
    <RadioButton Content="Male" Margin="5,0,0,0" />
    <RadioButton Content="Female" Margin="5,0,0,0" />
</validators:RadioGroup>

<TextBox  Grid.Column="1" Grid.Row="5" TextWrapping="Wrap" Name="customVal"
    validators:ValidatorService.Validator="custom" 
    validators:ValidatorService.ValidatorType="Custom" Style="{StaticResource FormTextBox}" 
 />

<CheckBox Content="Yes / No"  Grid.Column="1" Grid.Row="6" VerticalAlignment="Center"
    validators:ValidatorService.Validator="terms" 
    validators:ValidatorService.ValidatorType="Required"
/>
```


![Screenshot](https://github.com/Ericvf/Silverlight-Validators/blob/54632594a7416feb6b16e034b4da817db08a0cf1/screenshot.PNG)
