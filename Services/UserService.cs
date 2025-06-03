using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

public class UserService
{
    private static List<User> _users = new List<User>();
    private static int _nextId = 1;

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUserById(int id)
    {
        return _users.FirstOrDefault(user => user.Id == id);
    }

    public User AddUser(User user)
    {
        if (string.IsNullOrEmpty(user.Name))
        {
            throw new ArgumentException("Le nom d'utilisateur est requis.");
        }
        if (user.Id == 0)
        {
            user.Id = _nextId++;
        }
        else if (_users.Any(u => u.Id == user.Id))
        {
            throw new ArgumentException($"Un utilisateur avec l'ID {user.Id} existe déjà.");
        }
        _users.Add(user);
        return user;
    }

    public bool DeleteUserById(int id)
    {
        User? user = _users.FirstOrDefault(user => user.Id == id);
        if (user != null)
        {
            _users.Remove(user);
            return true;
        }
        else
        {
            Console.WriteLine($"The ID {id} does not exist in the database");
            return false;
        }
    }
}