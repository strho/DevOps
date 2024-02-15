using Microsoft.EntityFrameworkCore;

public class BugContext : DbContext
{
    public DbSet<Bug> Bugs { get; set; }

    public BugContext(DbContextOptions<BugContext> options) : base(options)
    { }
}
