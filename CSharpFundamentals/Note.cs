namespace CSharpFundamentals
{
    public class Note
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Content { get; set; }
        public NoteStatus Status { get; set; }

        public override string ToString()
        {
            return $"""
                    [ID: {Id}] [{CreatedAt}] [Status: {Status}]
                    {Content}
                    ----------------------------------
                """;
        }


    }
}
