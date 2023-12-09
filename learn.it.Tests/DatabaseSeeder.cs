using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using learn.it.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace learn.it.Tests
{
    public static class DatabaseSeeder
    {

        public static (List<Permission> Permissions, List<User> Users, List<Achievement> Achievements, List<Group> Groups) Prepare(LearnitDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var f = Seed(context);
            return f;
        }

        public static (List<Permission> Permissions, List<User> Users, List<Achievement> Achievements, List<Group> Groups) Seed(LearnitDbContext context)
        {
            var hasher = new PasswordHasher<User>();

            var permissions = new List<Permission>
            {
                new()
                {
                    Name = "Admin"
                },
                new()
                {
                    Name = "User"
                }
            };

            var users = new List<User>
            {
                new()
                {
                    Username = "testAdmin",
                    Email = "admin@admin.com",
                    Permissions = permissions[0],
                    CreateTime = DateTime.UtcNow,
                    UserPreferences = new UserPreferences(),
                    UserStats = new UserStats()
                },

                new()
                {
                    Username = "testUser1",
                    Email = "user1@user.com",
                    Permissions = permissions[1],
                    CreateTime = DateTime.UtcNow,
                    UserPreferences = new UserPreferences(),
                    UserStats = new UserStats()
                },

                new()
                {
                    Username = "testUser2",
                    Email = "user2@user.com",
                    Permissions = permissions[1],
                    CreateTime = DateTime.UtcNow,
                    UserPreferences = new UserPreferences(),
                    UserStats = new UserStats()
                },

                new()
                {
                    Username = "testUser3",
                    Email = "user3@user.com",
                    Permissions = permissions[1],
                    CreateTime = DateTime.UtcNow,
                    UserPreferences = new UserPreferences(),
                    UserStats = new UserStats()
                }
            };

            users[0].Password = hasher.HashPassword(users[0], "testAdmin1");
            users[1].Password = hasher.HashPassword(users[1], "testUser1");
            users[2].Password = hasher.HashPassword(users[2], "testUser2");
            users[3].Password = hasher.HashPassword(users[3], "testUser3");

            var achievements = new List<Achievement>
            {
                new()
                {
                    Name = "1 Total Login",
                    ImagePath = "testPath1.jpg",
                    Description = "testDescription1",
                    Predicate = "TotalLoginDays >= 1"
                },

                new()
                {
                    Name = "1 Consecutive Login",
                    ImagePath = "testPath2.jpg",
                    Description = "testDescription2",
                    Predicate = "ConsecutiveLoginDays >= 1"
                },

                new()
                {
                    Name = "1 Flashcard Added",
                    ImagePath = "testPath3.jpg",
                    Description = "testDescription3",
                    Predicate = "TotalFlashcardsAdded >= 1"
                },

                new()
                {
                    Name = "1 Set Added",
                    ImagePath = "testPath4.jpg",
                    Description = "testDescription4",
                    Predicate = "TotalSetsAdded >= 1"
                },

                new()
                {
                    Name = "1 Flashcard Mastered",
                    ImagePath = "testPath5.jpg",
                    Description = "testDescription5",
                    Predicate = "TotalFlashcardsMastered >= 1"
                },

                new()
                {
                    Name = "1 Set Mastered",
                    ImagePath = "testPath6.jpg",
                    Description = "testDescription6",
                    Predicate = "TotalSetsMastered >= 1"
                }
            };

            var groups = new List<Group>
            {
                new()
                {
                    Creator = users[1], //testUser1
                    Name = "testGroup1",
                    Users = new List<User>
                    {
                        users[1], //testUser1
                        users[2] //testUser2
                    }
                }
            };
            users[1].GroupCreator = new List<Group> { groups[0] };
            users[1].Groups = new List<Group> { groups[0] };
            users[2].Groups = new List<Group> { groups[0] };

            context.Permissions.AddRange(permissions);
            context.Users.AddRange(users);
            context.Achievements.AddRange(achievements);
            context.Groups.AddRange(groups);
            context.SaveChanges();

            users[0].Password = "testAdmin1";
            users[1].Password = "testUser1";
            users[2].Password = "testUser2";
            users[3].Password = "testUser3";
            return (Permissions: permissions, Users: users, Achievements: achievements, Groups: groups);
        }
    }
}
