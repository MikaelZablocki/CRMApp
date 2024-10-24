
namespace Api
{
    public class Program
    {

        public static void Main(string[] args)
        {

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";  // Define a string constant for CORS policy

            var builder = WebApplication.CreateBuilder(args);  // Create a new instance of WebApplicationBuilder

            // Add CORS services to the service collection
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:5500")  // Allow requests from specified origin
                                             .AllowAnyOrigin()  // Allow requests from any origin
                                             .AllowAnyHeader()  // Allow any header in the request
                                             .AllowAnyMethod();  // Allow any HTTP method
                                  });
            });

        
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
