using Microsoft.EntityFrameworkCore;
using NativaGlobalUsers.Models;
using NativaGlobalUsers.Repository;

namespace NativaGlobalUsers
{
    public class Program
    {
        // Given requirements:
        // A bot requires consulting and storing customer data.
        // For this, a REST API is being considered that implements a CRUD in the database to query, insert and update records. The fields to store are: Name, e-mail, and password. All fields are required and password must be validated to a minimum of 8 characters, alphanumeric, upper and lower case, and at least one special character.
        //    A) Develop the architecture of the solution, including the necessary database and endpoints.
        //    B) Justify the architecture, endpoint structure, and database.
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("SQLconnection"));
            });

            builder.Services.AddScoped<IRepository<User>, UserRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}