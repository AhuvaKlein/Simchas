using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simchas.data
{
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CellNumber { get; set; }
        public DateTime DateJoined { get; set; }
        public bool AlwaysJoin { get; set; }

        public decimal Balance { get; set; }
    }

    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public decimal? TotalContributions { get; set; }
        public int TotalContributors { get; set; }
    }

    public class Deposit
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }

    public class Contributions
    {
        public int ContributorId { get; set; }
        public int SimchaId { get; set; }
        public decimal Amount { get; set; }
    }

    public class SimchaContributor
    {
        public int ContributorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysJoin { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public bool Contributed { get; set; }
    }
    
    public class ContributorHistory
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class Manager
    {
        private string _connectionString;

        public Manager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Simcha> GetSimchas()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas";
            conn.Open();
            List<Simcha> simchas = new List<Simcha>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simchas.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                    TotalContributions = GetSimchaTotalContributions((int)reader["Id"]),
                    TotalContributors = GetSimchaTotalContributors((int)reader["Id"])
                });
            }
            conn.Close();
            conn.Dispose();
            return simchas;
        }

        public void AddSimcha(Simcha simcha)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Simchas VALUES (@name, @date)";
            cmd.Parameters.AddWithValue("@name", simcha.Name);
            cmd.Parameters.AddWithValue("@date", simcha.Date);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public IEnumerable<Contributor> GetContributors()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            conn.Open();
            List<Contributor> simchas = new List<Contributor>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                simchas.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    DateJoined = (DateTime)reader["DateJoined"],
                    AlwaysJoin = (bool)reader["AlwaysJoin"],
                    Balance = GetContributorBalance((int)reader["Id"])
                });
            }
            conn.Close();
            conn.Dispose();
            return simchas;
        }

        public int AddContributor(Contributor c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors VALUES (@firstName, @lastName, @cellNumber, @dateJoined, @alwaysJoin) SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
            cmd.Parameters.AddWithValue("@dateJoined", c.DateJoined);
            cmd.Parameters.AddWithValue("@alwaysJoin", c.AlwaysJoin);
            conn.Open();
            return (int)(decimal)cmd.ExecuteScalar();

        }

        public void AddDeposit(Deposit deposit)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposits VALUES (@date, @amount, @contributorId)";
            cmd.Parameters.AddWithValue("@date", deposit.Date);
            cmd.Parameters.AddWithValue("@amount", deposit.Amount);
            cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public decimal GetContributorDepositTotal(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Deposits WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetContributionTotal(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Contributions WHERE ContributorId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();

            return (decimal)cmd.ExecuteScalar();
        }

        public decimal GetContributorBalance(int id)
        {
            return GetContributorDepositTotal(id) - GetContributionTotal(id);
        }

        public IEnumerable<ContributorHistory> GetDepositHistory(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Deposits WHERE ContributorId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<ContributorHistory> deposits = new List<ContributorHistory>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                deposits.Add(new ContributorHistory
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Action = "Deposit"
                });
            }

            conn.Close();
            conn.Dispose();
            return deposits;


        }

        public Contributor GetContributor(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    CellNumber = (string)reader["CellNumber"],
                    DateJoined = (DateTime)reader["DateJoined"],
                    AlwaysJoin = (bool)reader["AlwaysJoin"]
                };
            }

            return null;
        }

        public void EditContributor(Contributor c)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Contributors SET FirstName=@firstName, LastName=@lastName, CellNumber=@cellNumber, DateJoined=@dateJoined, AlwaysJoin=@alwaysJoin WHERE id=@id";
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.Parameters.AddWithValue("@firstName", c.FirstName);
            cmd.Parameters.AddWithValue("@lastName", c.LastName);
            cmd.Parameters.AddWithValue("@cellNumber", c.CellNumber);
            cmd.Parameters.AddWithValue("@dateJoined", c.DateJoined);
            cmd.Parameters.AddWithValue("@alwaysJoin", c.AlwaysJoin);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }

        public decimal GetTotalDeposits()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Amount), 0) FROM Deposits";                               
            conn.Open();
            decimal total = (decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public decimal GetTotalContributions()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText= "SELECT ISNULL(SUM(Amount), 0) FROM Contributions";
            conn.Open();
            decimal total = (decimal)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public decimal GetTotalBalance()
        {
            return GetTotalDeposits() - GetTotalContributions();
        }

        public IEnumerable<SimchaContributor> GetContributorsThatContributed(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions c JOIN Contributors co ON co.id=c.ContributorId WHERE SimchaId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<SimchaContributor> contributors = new List<SimchaContributor>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributors.Add(new SimchaContributor
                {
                    ContributorId = (int)reader["ContributorId"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysJoin = (bool)reader["AlwaysJoin"],
                    Balance = GetContributorBalance((int)reader["contributorId"]),
                    Amount = (decimal)reader["Amount"],
                    Contributed = true
                });
            }
            conn.Close();
            conn.Dispose();
            return contributors;
        }

        public Simcha GetSimcha(int id)
        {

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simchas WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Simcha
                {
                    Id = (int)reader["Id"],
                    Name = (string)reader["Name"],
                    Date = (DateTime)reader["Date"],
                };
            }

            conn.Close();
            conn.Dispose();
            return null;


        }

        public void UpdateContributions(IEnumerable<SimchaContributor> contributions, int simchaId)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions VALUES(@contributorId, @simchaId, @amount)";
            conn.Open();
            foreach (SimchaContributor sc in contributions)
            {
                if (sc.Contributed)
                {
                    cmd.Parameters.AddWithValue("@contributorId", sc.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", sc.Amount);
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.ExecuteNonQuery();
                }
            }

            conn.Close();
            conn.Dispose();
        }

        public void DeleteContributions(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public decimal? GetSimchaTotalContributions(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            decimal? total;
            if(cmd.ExecuteScalar()!=DBNull.Value)
            {
                total = (decimal)cmd.ExecuteScalar();
            }
            else
            {
                total = null;
            }
            conn.Close();
            conn.Dispose();
            return total;
        }

        public int GetSimchaTotalContributors(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributions WHERE simchaId = @id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            int total = (int)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public int GetTotalContributors()
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Contributors";
            conn.Open();
            int total = (int)cmd.ExecuteScalar();
            conn.Close();
            conn.Dispose();
            return total;
        }

        public IEnumerable<ContributorHistory> GetContributionHistory(int id)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions c JOIN Simchas s ON s.Id=c.SimchaId WHERE c.ContributorId=@id";
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            List<ContributorHistory> contributions = new List<ContributorHistory>();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                contributions.Add(new ContributorHistory
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Action = $"Contribution for {(string)reader["Name"]} Simcha"
                });
            }

            conn.Close();
            conn.Dispose();
            return contributions;
        }

    }

}
