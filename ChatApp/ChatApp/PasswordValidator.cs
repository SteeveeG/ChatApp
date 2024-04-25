using System.Text.RegularExpressions;

namespace ChatApp;

public class PasswordValidator
{
    private static Regex upperCaseRegex = new("^[A-Z]+$");
    private static Regex lowerCaseRegex = new("^[a-z]+$");
    private static Regex alphabeticRegex = new("^[A-Za-z]+$");
    private static Regex numbersRegex = new("^[0-9]+$");
    private static Regex numbersLowerCaseRegex = new("^[a-z0-9]+$");
    private static Regex numbersUpperCaseRegex = new("^[A-Z0-9]+$");
    private static Regex specialCharactersRegex = new("^[a-zA-Z0-9]+$");

    private static string ruleSet = "Rules: minimum 8 characters \nthere must be lower case letters," +
                                    "\nupper case letters, \nNumbers and special characters\nYour Mistake:";

    public static Tuple<bool, string> ValidatePassword(string password)
    {
        switch (password.Length)
        {
            case < 8:
                return new Tuple<bool, string>(false, $"{ruleSet}too short");
        }
        
        switch (password)
        {
            case { } s when upperCaseRegex.IsMatch(password):
                return new Tuple<bool, string>(false,  $"{ruleSet}Missing Numbers and lowercase");
            case { } s when lowerCaseRegex.IsMatch(password):
                return new Tuple<bool, string>(false,$"{ruleSet}Missing Numbers and uppercase");
            case { } s when alphabeticRegex.IsMatch(password):
                return new Tuple<bool, string>(false,$"{ruleSet}Missing Numbers");
            case { } s when numbersRegex.IsMatch(password):
                return new Tuple<bool, string>(false, $"{ruleSet}Missing lowercase and uppercase");
            case { } s when numbersLowerCaseRegex.IsMatch(password):
                return new Tuple<bool, string>(false,  $"{ruleSet}Missing uppercase");
            case { } s when numbersUpperCaseRegex.IsMatch(password):
                return new Tuple<bool, string>(false,  $"{ruleSet}Missing lowercase");
            case { } s when specialCharactersRegex.IsMatch(password):
                return new Tuple<bool, string>(false, $"{ruleSet}Special Character Missing");
        }
        return new Tuple<bool, string>(true, password);
    }
}