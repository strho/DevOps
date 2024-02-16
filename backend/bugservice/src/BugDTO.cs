public class BugDTO
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public int? AssignedTo { get; set; }

    public BugDTO() { }
    public BugDTO(Bug bug)
    {
        Id = bug.Id;
        Title = bug.Title;
        Description = bug.Description;
        Status = bug.Status;
        AssignedTo = bug.AssignedTo;
    }

}
