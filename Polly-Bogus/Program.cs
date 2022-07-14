using Bogus;
using Polly;
using Polly.Retry;

var persons = new Faker<Person>()
    .RuleFor(bp => bp.Id, f => f.IndexFaker)
    .RuleFor(bp => bp.Name, f => f.Name.FullName())
    .RuleFor(bp => bp.Email, (f, bp) => f.Internet.Email(bp.Name).ToLower())
    .RuleFor(bp => bp.CreatedDate, f => f.Date.Past())
    .Generate(10)
    .ToList();

foreach (var item in persons)
{
    Console.WriteLine(item.Id + " - " + item.Name + " - " + item.Email + " skapad: " + item.CreatedDate.ToString());
}

AsyncRetryPolicy<bool> retryPolicyNeedsTrueResponse = Policy
    .HandleResult<bool>(result => !result)
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));

bool result = await retryPolicyNeedsTrueResponse.ExecuteAsync(() => IsConnected());

async Task<bool> IsConnected()
{
    Console.WriteLine("trying...");
    await Task.Delay(1);
    return new Random().Next(2) == 1;
}


Console.WriteLine("resultat = " + result);

public class Person
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedDate { get; set; }
}
