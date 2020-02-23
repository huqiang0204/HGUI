using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace huqiang.Data
{
    public enum CharacterValidation
    {
        None,
        Integer,
        Decimal,
        Alphanumeric,
        Name,
        numberAndName,
        EmailAddress,
        Custom
    }
    public class CharOperation
    {
        static readonly char[] Separators = { ' ', '.', ',', '\t', '\r', '\n' };
        const string EmailCharacters = "!#$%&'*+-/=?^_`{|}~";
        public static char Validate(CharacterValidation validat, string text, int pos, char ch)
        {
            if (validat == CharacterValidation.None)
                return ch;
            if (validat == CharacterValidation.Integer)
            {
                if (ch == '-')
                {
                    if (text == "")
                        return ch;
                    if (text.Length > 0)
                        return (char)0;
                }
                if (ch < '0' | ch > '9')
                    return (char)0;
                return ch;
            }
            else if (validat == CharacterValidation.Decimal)
            {
                if (ch >= '0' && ch <= '9')
                {
                    if (ch == '.')
                        if (text.IndexOf('.') < 0)
                            return ch;
                    return (char)0;
                }
                return ch;
            }
            else if (validat == CharacterValidation.Alphanumeric)
            {
                // All alphanumeric characters
                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.numberAndName)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
                if (ch >= '0' && ch <= '9') return ch;
            }
            else if (validat == CharacterValidation.Name)
            {
                if (char.IsLetter(ch))
                {
                    // Character following a space should be in uppercase.
                    if (char.IsLower(ch) && ((pos == 0) || (text[pos - 1] == ' ')))
                    {
                        return char.ToUpper(ch);
                    }

                    // Character not following a space or an apostrophe should be in lowercase.
                    if (char.IsUpper(ch) && (pos > 0) && (text[pos - 1] != ' ') && (text[pos - 1] != '\''))
                    {
                        return char.ToLower(ch);
                    }

                    return ch;
                }

                if (ch == '\'')
                {
                    // Don't allow more than one apostrophe
                    if (!text.Contains("'"))
                        // Don't allow consecutive spaces and apostrophes.
                        if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                              ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                            return ch;
                }

                if (ch == ' ')
                {
                    // Don't allow consecutive spaces and apostrophes.
                    if (!(((pos > 0) && ((text[pos - 1] == ' ') || (text[pos - 1] == '\''))) ||
                          ((pos < text.Length) && ((text[pos] == ' ') || (text[pos] == '\'')))))
                        return ch;
                }
            }
            else if (validat == CharacterValidation.EmailAddress)
            {

                if (ch >= 'A' && ch <= 'Z') return ch;
                if (ch >= 'a' && ch <= 'z') return ch;
                if (ch >= '0' && ch <= '9') return ch;
                if (ch == '@' && text.IndexOf('@') == -1) return ch;
                if (EmailCharacters.IndexOf(ch) != -1) return ch;
                if (ch == '.')
                {
                    char lastChar = (text.Length > 0) ? text[UnityEngine.Mathf.Clamp(pos, 0, text.Length - 1)] : ' ';
                    char nextChar = (text.Length > 0) ? text[UnityEngine.Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n';
                    if (lastChar != '.' && nextChar != '.')
                        return ch;
                }
            }
            return (char)0;
        }
        public static string Validate(CharacterValidation validat, string text, int count)
        {
            if (count > 0)
                if (count > text.Length)
                    text = text.Substring(0, count);
            switch (validat)
            {
                case CharacterValidation.Integer:
                    return ValidateInteger(text);
                case CharacterValidation.Decimal:
                    return ValidateDecimal(text);
                case CharacterValidation.Alphanumeric:
                    return ValidateAlphanumeric(text);
                case CharacterValidation.EmailAddress:
                    return ValidateEmailAddress(text);
            }
            return text;
        }
        public static string ValidateInteger(string str)
        {
            int len = str.Length;
            for(int i=0;i<str.Length;i++)
            {
                if (str[i] < '0' | str[i] > '9')
                {
                    len = i;
                    break;
                }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0,len);
        }
        public static string ValidateDecimal(string str)
        {
            bool point = false;
            int len = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                {
                    if (!point)
                        if (str[i] == '.')
                        {
                            point = true;
                            continue;
                        }
                    len = i;
                    break;
                }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
        public static string ValidateAlphanumeric(string str)
        {
            int len = str.Length;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                    if (str[i] < 'a' | str[i] > 'z')
                        if (str[i] < 'A' | str[i] > 'Z')
                        {
                            len = i;
                            break;
                        }
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
        public static string ValidateEmailAddress(string str)
        {
            int len = str.Length;
            bool point = false;
            bool at = false;
            char chr= str[0];
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] < '0' | str[i] > '9')
                    if (str[i] < 'a' | str[i] > 'z')
                        if (str[i] < 'A' | str[i] > 'Z')
                        {
                            if (str[i] == '@')
                            {
                                if (i == 0)
                                    break;
                                if (!at)
                                {
                                   if(!point)
                                    {
                                        at = true;
                                        continue;
                                    }
                                }

                            }
                            else if (str[i] == '.')
                            {
                                if (chr == '@')
                                    break;
                                if (!point)
                                {
                                    if(at)
                                    {
                                        point = true;
                                        continue;
                                    }
                                }
                            }
                            len = i;
                            break;
                        }
                chr = str[i];
            }
            if (len == str.Length)
                return str;
            if (len == 0)
                return "";
            return str.Substring(0, len);
        }
    }
}
