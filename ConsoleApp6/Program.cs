using Microsoft.EntityFrameworkCore;
using System.Windows.Input;

//static void Main(string[] args)
{
    // добавление данных
    using (ApplicationContext db = new ApplicationContext())
    {
        /////Пытался исправить ошибки в OnModelCreating (строки сейчас закоментированы)
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        // создаем два объекта User
        Company comp1 = new Company { Name = "Pit" };
        Company comp2 = new Company { Name = "Simpl" };
        User user1 = new User { Name = "Tom", Age = 47, Company = comp1 };
        User user2 = new User { Name = "Alice", Age = 29, Company = comp2 };

        // добавляем их в бд
        db.Users.AddRange(user1, user2);
        db.Companies.AddRange(comp1, comp2);
        db.SaveChanges();

        foreach (User u in db.Users.ToList())
        {
            Console.WriteLine($"{u.Id}.{u.Name} - {u.Age} - {u.Company?.Name}");
        }
    }
    // получение данных
    // Retrieve users and companies from the database and display user information
    using (ApplicationContext db = new ApplicationContext())
    {
        // Retrieve all companies from the database
        //var comps = db.Companies.ToList();

        // Retrieve all users from the database
        var users = db.Users.Include(u => u.Company).ToList();
       
        // Display the list of users
        Console.WriteLine("Users list:");

        // Iterate through each user and display their information
        foreach (User u in users)
        {
            // Find the company associated with the current user
            //var company = comps.FirstOrDefault(c => c.Id == u.CompanyId);

            // Display the user's information, including their company name
            Console.WriteLine($"{u.Id}.{u.Name} - {u.Age} - {u.Company?.Name}");
        }
    }

    Console.ReadKey();
}

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Company> Companies { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=app6db;Trusted_Connection=True;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Company>().Ignore(c => c.Users);        
        //modelBuilder.Entity<User>().Ignore(u => u.Company);
        //modelBuilder.Entity<User>().ToTable(t => t.HasCheckConstraint("ValidAge", "Age > 0 AND Age < 120"));

        ////Пытался исправить ошибки
        //modelBuilder.Entity<Company>().HasOne(c => (Country?)c.Country).WithMany().HasForeignKey(c => c.CountryId);
        //modelBuilder.Entity<User>().HasOne(u => (Company?)u.Company).WithMany(c => c.Users.OfType<User>()).HasForeignKey(u => u.CompanyId);
    }
}
public interface IUser<T,V>
{
    public T Id { get; set; }
    public string Name { get; set; }
    public V Age { get; set; }
    public T CompanyId { get; set; }      // внешний ключ
    public ICompany<T, V>? Company { get; set; }    // навигационное свойство
}
public class User: IUser<int, uint>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public uint Age { get; set; }
    public int CompanyId { get; set; }      // внешний ключ
    public ICompany<int, uint>? Company { get; set; }    // навигационное свойство
}

public interface ICompany<T, V>
{
    public T Id { get; set; }
    public string Name { get; set; }
    public int? CountryId { get; set; }      // внешний ключ
    public ICountry<T>? Country { get; set; }    // навигационное свойство
    public IEnumerable<IUser<T, V>>? Users { get; set; }
}

public class Company: ICompany<int, uint>
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? CountryId { get; set; }      // внешний ключ
    public ICountry<int>? Country { get; set; }
    public IEnumerable<IUser<int, uint>>? Users { get; set; }
}

public interface ICountry<T>
{
    public T Id { get; set; }
    public string Name { get; set; }
}
public class Country: ICountry<int>
{
    public int Id { get; set; }
    public required string Name { get; set; }
}
