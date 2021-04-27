using AuthApp.Models;
using kyrsiv.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace kyrsiv
{
  public static class SampleData
        {
            public static void Initialize(UserContext context)
            {
                if (!context.Users.Any())
                {
                context.Users.AddRange(
                    new User
                    {
                        Email = "Admin",
                        Password = "neadmin",
                        Role = "Admin"
                    },
                    new User
                    {

                        Email = "Moder1",
                        Password = "moder",
                        Role = "Moderator"
                    }); 
                    context.SaveChanges();
                }

            }
        }
    }
