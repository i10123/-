namespace LABS_3_4
{
    public class Word : Token
    {
        public override TokenType Type => TokenType.Word;
        public Word() { }
        public Word(string value) : base(value) { }

        public int Length => Value != null ? Value.Length : 0;

        public bool CheckForSoglasny()
        {
            if (string.IsNullOrEmpty(Value)) 
                return false;

            char firstChar = char.ToLower(Value[0]);
            string glasnie = "aeiouаеёиоуыэюя";
            return char.IsLetter(firstChar) && !glasnie.Contains(firstChar);
        }

        public override string ToString() => Value ?? "";

    }
}