using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Tips.ApiMessage.Handlers;
using Tips.ApiMessage.TodoItems.Context;
using Tips.ApiMessage.TodoItems.CreateTodoItems;
using Tips.ApiMessage.TodoItems.DeleteTodoItems;
using Tips.ApiMessage.TodoItems.GetTodoItem;
using Tips.ApiMessage.TodoItems.GetTodoItems;

namespace Tips.ApiMessage
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoList"));
            ConfigureDependencies(services);
            services.AddControllers();
        }

        private static void ConfigureDependencies(IServiceCollection services)
        {
            //services.AddScoped(typeof(IActionResultHandler<,>), typeof(ActionResultHandler<,>));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemsRequest, GetTodoItemsResponse>), typeof(GetTodoItemsRequestHandler));
            services.AddScoped(typeof(IRequestHandler<GetTodoItemRequest, GetTodoItemResponse>), typeof(GetTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<CreateTodoItemRequest, CreateTodoItemResponse>), typeof(CreateTodoItemRequestHandler));
            services.AddScoped(typeof(IRequestHandler<DeleteTodoItemRequest, DeleteTodoItemResponse>), typeof(DeleteTodoItemRequestHandler));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
