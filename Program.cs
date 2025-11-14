

using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Json.Serialization;


// object to turn valid row to JSON
class CustomerRecord
{
    [JsonPropertyName("customer_id")]
    public int CustomerId { get; set; }

    [JsonPropertyName("first_name")]
    public required string FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public required string LastName { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("phone_number")]
    public required string PhoneNumber { get; set; }

    [JsonPropertyName("address")]
    public required string Address { get; set; }

    [JsonPropertyName("city")]
    public required string City { get; set; }

    [JsonPropertyName("state")]
    public required string State { get; set; }

    [JsonPropertyName("postal_code")]
    public required string PostalCode { get; set; }

    [JsonPropertyName("car_make")]
    public required string CarMake { get; set; }

    [JsonPropertyName("car_model")]
    public required string CarModel { get; set; }

    [JsonPropertyName("car_year")]
    public int CarYear { get; set; }

    [JsonPropertyName("license_plate")]
    public required string LicensePlate { get; set; }

    [JsonPropertyName("purchase_date")]
    public DateTime PurchaseDate { get; set; }

    [JsonPropertyName("purchase_price")]
    public decimal PurchasePrice { get; set; }
}

class CSVtoJSON
{
    // the Main first makes sure a file is passed in as an argument and if so will read the lines of the file before
    // calling functions to find the vaild entries and convert it to a JSON
    static void Main(string[] args)
    {
        // JSON Settings
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: CSVtoJSON <input-file-path>");
            return;
        }
        string infile = args[0];
        string[] lines = File.ReadAllLines(infile);
        List<CustomerRecord> validLines = findValid(lines);
        string json = JsonSerializer.Serialize(validLines, jsonOptions);
        string outfile = args.Length > 1 ? args[1] : "output.json";
        File.WriteAllText(outfile, json);
    }

    // findValid goes through all the lines from the CSV and split it so that each of the colums content can be viewed
    // if there is a incomplete row it writes out to the console
    // the row is checked for errors if errors are present each is written out to the cosle else the row is saved as a valid row
    private static List<CustomerRecord> findValid(string[] lines)
    {
        List<CustomerRecord> validRows = new List<CustomerRecord>();

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            var fields = line.Split(',');

            if (fields.Length != 15)
            {
                Console.WriteLine($"Row {i}: Invalid number of columns ({fields.Length})");
                continue;
            }

            List<string> errors = ValidateRow(fields);

            if (errors.Count > 0)
            {
                foreach (var err in errors)
                    Console.WriteLine($"Row {i}: {err}");
                continue;
            }

            CustomerRecord record = ParseValidRecord(fields);
            validRows.Add(record);
        }
        return validRows;
    }

    // ValidateRow will take one row of the CSV and check each value in that row, 
    // all errors will be saved individually in an array 
    static List<string> ValidateRow(string[] f)
    {
        var errors = new List<string>();

        if (!ValidatePositiveInt(f[0]))
            errors.Add("customer_id must be a positive integer");

        if (!ValidateString(f[1]))
            errors.Add("first_name must be a string");

        if (!ValidateString(f[2]))
            errors.Add("last_name must be a string");

        if (!ValidateEmail(f[3]))
            errors.Add("email is not a valid email address");

        if (!ValidatePhone(f[4]))
            errors.Add("phone_number must be XXX-XXX-XXXX");

        if (!ValidateString(f[5]))
            errors.Add("address must be a string");

        if (!ValidateString(f[6]))
            errors.Add("city must be a string");

        if (!ValidateState(f[7]))
            errors.Add("state must be a valid 2-letter US state code");

        if (!ValidatePostalCode(f[8]))
            errors.Add("postal_code must be 1–5 digits");

        if (!ValidateString(f[9]))
            errors.Add("car_make must be a string");

        if (!ValidateString(f[10]))
            errors.Add("car_model must be a string");

        if (!ValidateCarYear(f[11]))
            errors.Add("car_year must be between 1900 and 2025");

        if (!ValidateLicensePlate(f[12]))
            errors.Add("license_plate must be ABC123 or 123ABC");

        if (!ValidatePurchaseDate(f[13]))
            errors.Add("purchase_date must be MM/DD/YYYY and between year 2000–present");

        if (!ValidateDecimal(f[14]))
            errors.Add("purchase_price must be a decimal number");

        return errors;
    }


    // functions that test for the diffrent possible errors described in the spec returning a bool true if pass, false if fail
    static bool ValidatePositiveInt(string s)
    {
        return int.TryParse(s, out int n) && n > 0;
    }

    static bool ValidateString(string s)
    {
        return !string.IsNullOrWhiteSpace(s);
    }

    static bool ValidateEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
    }

    static bool ValidatePhone(string phone)
    {
        return Regex.IsMatch(phone, @"^\d{3}-\d{3}-\d{4}$");
    }

    static readonly HashSet<string> US_STATES = new HashSet<string>
    {
        "AL","AK","AZ","AR","CA","CO","CT","DE","FL","GA","HI","ID","IL","IN",
        "IA","KS","KY","LA","ME","MD","MA","MI","MN","MS","MO","MT","NE","NV",
        "NH","NJ","NM","NY","NC","ND","OH","OK","OR","PA","RI","SC","SD","TN",
        "TX","UT","VT","VA","WA","WV","WI","WY"
    };

    static bool ValidateState(string s)
    {
        return US_STATES.Contains(s.ToUpper());
    }

    static bool ValidatePostalCode(string s)
    {
        return Regex.IsMatch(s, @"^\d{1,5}$");
    }

    static bool ValidateCarYear(string s)
    {
        return int.TryParse(s, out int year) && year >= 1900 && year <= 2025;
    }

    static bool ValidateLicensePlate(string s)
    {
        return Regex.IsMatch(s, @"^[A-Za-z]{3}\d{3}$")  // ABC123
            || Regex.IsMatch(s, @"^\d{3}[A-Za-z]{3}$"); // 123ABC
    }

    static bool ValidatePurchaseDate(string s)
    {
        if (!DateTime.TryParseExact(s, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
            return false;

        return dt.Year >= 2000 && dt <= DateTime.Now;
    }

    static bool ValidateDecimal(string s)
    {
        return decimal.TryParse(s, out _);
    }

    // turns the valid record into an object ready to be turned into a JSON object 
    static CustomerRecord ParseValidRecord(string[] f)
    {
        return new CustomerRecord
        {
            CustomerId = int.Parse(f[0]),
            FirstName = f[1],
            LastName = f[2],
            Email = f[3],
            PhoneNumber = f[4],
            Address = f[5],
            City = f[6],
            State = f[7],
            PostalCode = f[8],
            CarMake = f[9],
            CarModel = f[10],
            CarYear = int.Parse(f[11]),
            LicensePlate = f[12],
            PurchaseDate = DateTime.ParseExact(f[13], "MM/dd/yyyy", null),
            PurchasePrice = decimal.Parse(f[14])
        };
    }

}

