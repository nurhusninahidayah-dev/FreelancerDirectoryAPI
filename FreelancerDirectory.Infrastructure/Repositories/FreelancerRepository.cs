using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using FreelancerDirectory.Application.Interfaces;
using FreelancerDirectory.Domain.Entities;

namespace FreelancerDirectory.Infrastructure.Repositories;

public class FreelancerRepository : IFreelancerRepository
{
    private readonly string _connectionString;

    public FreelancerRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<IEnumerable<Freelancer>> GetAllAsync(bool includeArchived = false)
    {
        var sql = @"SELECT * FROM Freelancers 
                    WHERE (@includeArchived = 1 OR IsArchived = 0)";

        using var connection = CreateConnection();
        var freelancers = await connection.QueryAsync<Freelancer>(sql, new { includeArchived });

        foreach (var f in freelancers)
        {
            f.Skillsets = (await connection.QueryAsync<SkillSet>(
                "SELECT * FROM SkillSets WHERE FreelancerId = @Id", new { f.Id })).ToList();
            f.Hobbies = (await connection.QueryAsync<Hobby>(
                "SELECT * FROM Hobbies WHERE FreelancerId = @Id", new { f.Id })).ToList();
        }
        return freelancers;
    }

    public async Task<Freelancer?> GetByIdAsync(int id)
    {
        using var connection = CreateConnection();
        var freelancer = await connection.QueryFirstOrDefaultAsync<Freelancer>(
            "SELECT * FROM Freelancers WHERE Id = @id", new { id });
        if (freelancer == null) return null;

        freelancer.Skillsets = (await connection.QueryAsync<SkillSet>(
            "SELECT * FROM SkillSets WHERE FreelancerId = @id", new { id })).ToList();
        freelancer.Hobbies = (await connection.QueryAsync<Hobby>(
            "SELECT * FROM Hobbies WHERE FreelancerId = @id", new { id })).ToList();

        return freelancer;
    }

    public async Task<int> CreateAsync(Freelancer freelancer)
    {
        const string sql = @"INSERT INTO Freelancers (Username, Email, PhoneNumber, IsArchived)
                             VALUES (@Username, @Email, @PhoneNumber, 0);
                             SELECT CAST(SCOPE_IDENTITY() as int)";
        using var connection = CreateConnection();
        var id = await connection.ExecuteScalarAsync<int>(sql, freelancer);

        foreach (var skill in freelancer.Skillsets)
        {
            await connection.ExecuteAsync(
                "INSERT INTO SkillSets (FreelancerId, Skill) VALUES (@id, @Skill)",
                new { id, skill.Skill });
        }
        foreach (var hobby in freelancer.Hobbies)
        {
            await connection.ExecuteAsync(
                "INSERT INTO Hobbies (FreelancerId, HobbyName) VALUES (@id, @HobbyName)",
                new { id, HobbyName = hobby.HobbyName });
        }
        return id;
    }

    public async Task<bool> UpdateAsync(Freelancer freelancer)
    {
        const string sql = @"UPDATE Freelancers 
                             SET Username=@Username, Email=@Email, PhoneNumber=@PhoneNumber 
                             WHERE Id=@Id AND IsArchived=0";
        using var connection = CreateConnection();
        var rows = await connection.ExecuteAsync(sql, freelancer);

        await connection.ExecuteAsync("DELETE FROM SkillSets WHERE FreelancerId = @Id", new { freelancer.Id });
        foreach (var skill in freelancer.Skillsets)
        {
            await connection.ExecuteAsync(
                "INSERT INTO SkillSets (FreelancerId, Skill) VALUES (@Id, @Skill)",
                new { freelancer.Id, skill.Skill });
        }
        await connection.ExecuteAsync("DELETE FROM Hobbies WHERE FreelancerId = @Id", new { freelancer.Id });
        foreach (var hobby in freelancer.Hobbies)
        {
            await connection.ExecuteAsync(
                "INSERT INTO Hobbies (FreelancerId, HobbyName) VALUES (@Id, @HobbyName)",
                new { freelancer.Id, HobbyName = hobby.HobbyName });
        }
        return rows > 0;
    }

    public async Task<bool> ArchiveAsync(int id)
    {
        using var connection = CreateConnection();
        var rows = await connection.ExecuteAsync(
            "UPDATE Freelancers SET IsArchived = 1 WHERE Id = @id", new { id });
        return rows > 0;
    }

    public async Task<bool> UnarchiveAsync(int id)
    {
        using var connection = CreateConnection();
        var rows = await connection.ExecuteAsync(
            "UPDATE Freelancers SET IsArchived = 0 WHERE Id = @id", new { id });
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = CreateConnection();
        var rows = await connection.ExecuteAsync("DELETE FROM Freelancers WHERE Id = @id", new { id });
        return rows > 0;
    }

    public async Task<IEnumerable<Freelancer>> SearchAsync(string keyword)
    {
        var sql = @"SELECT * FROM Freelancers 
                    WHERE (Username LIKE @keyword OR Email LIKE @keyword) 
                    AND IsArchived = 0";
        using var connection = CreateConnection();
        var freelancers = await connection.QueryAsync<Freelancer>(sql, new { keyword = $"%{keyword}%" });
        foreach (var f in freelancers)
        {
            f.Skillsets = (await connection.QueryAsync<SkillSet>(
                "SELECT * FROM SkillSets WHERE FreelancerId = @Id", new { f.Id })).ToList();
            f.Hobbies = (await connection.QueryAsync<Hobby>(
                "SELECT * FROM Hobbies WHERE FreelancerId = @Id", new { f.Id })).ToList();
        }
        return freelancers;
    }
}