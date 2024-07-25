using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using MyCommonStructure.Services;

WebHost.CreateDefaultBuilder().
ConfigureServices(s =>
{
    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    s.AddSingleton<register>();
    s.AddSingleton<login>();
    s.AddSingleton<changePassword>();
    s.AddSingleton<resetPassword>();
    s.AddSingleton<editProfile>();
    s.AddSingleton<deleteProfile>();
    s.AddSingleton<feedbacks>();
    s.AddSingleton<cars>();
    s.AddSingleton<users>();
    s.AddSingleton<orders>();
    s.AddSingleton<wishlists>();
    s.AddSingleton<wishListCars>();

    s.AddCors();
    s.AddControllers();
    s.AddAuthorization();
    s.AddAuthentication("SourceJWT").AddScheme<SourceJwtAuthenticationSchemeOptions, SourceJwtAuthenticationHandler>("SourceJWT", options =>
        {
            options.SecretKey = appsettings["jwt_config:Key"].ToString();
            options.ValidIssuer = appsettings["jwt_config:Issuer"].ToString();
            options.ValidAudience = appsettings["jwt_config:Audience"].ToString();
            options.Subject = appsettings["jwt_config:Subject"].ToString();
        });
}).Configure(app =>
{
    app.UseCors(options =>
             options.WithOrigins("https://localhost:5001", "http://localhost:5002")
            .AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
    app.UseRouting();
    app.UseStaticFiles();

    app.UseAuthorization();
    app.UseAuthentication();

    app.UseEndpoints(e =>
    {
        var register = e.ServiceProvider.GetRequiredService<register>();
        e.MapPost("/registration",
        [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await register.Registration(rData)); // register
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await register.GetUserByEmail(rData)); // get users details via email
        });

        var login = e.ServiceProvider.GetRequiredService<login>();
        e.MapPost("/login",
        [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await login.Login(rData)); // login
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await login.Logout(rData)); // logout
        });

        var changePassword = e.ServiceProvider.GetRequiredService<changePassword>();
        e.MapPost("/changePassword", [Authorize] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await changePassword.ChangePassword(rData)); // change Password
        }).RequireAuthorization();

        var resetPassword = e.ServiceProvider.GetRequiredService<resetPassword>();
        e.MapPost("/resetPassword", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await resetPassword.ResetPassword(rData)); // reset Password
        });

        var editProfile = e.ServiceProvider.GetRequiredService<editProfile>();
        e.MapPost("/editProfile", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await editProfile.EditProfile(rData)); // edit profile
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await editProfile.EditProfilePic(rData)); // edit pic
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await editProfile.DeleteProfilePic(rData)); // delete pic
        });

        var deleteProfile = e.ServiceProvider.GetRequiredService<deleteProfile>();
        e.MapPost("/deleteProfile", [Authorize] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfileByUser(rData)); // delete profile by user
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await deleteProfile.DeleteProfileByAdmin(rData)); // delete profile by admin
        }).RequireAuthorization();

        var feedbacks = e.ServiceProvider.GetRequiredService<feedbacks>(); // for feedback details
        e.MapPost("/feedbacks", [AllowAnonymous] async (HttpContext http) =>
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await feedbacks.ContactUs(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await feedbacks.DeleteFeedbackById(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await feedbacks.GetFeedbackById(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await feedbacks.GetAllFeedbacks(rData));
        });


        var cars = e.ServiceProvider.GetRequiredService<cars>();
        e.MapPost("/cars", [AllowAnonymous] async (HttpContext http) => // for cars details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await cars.AddCar(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await cars.EditCar(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await cars.DeleteCar(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await cars.GetCar(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await cars.GetAllCars(rData));
        });

        var users = e.ServiceProvider.GetRequiredService<users>();
        e.MapPost("/users", [AllowAnonymous] async (HttpContext http) => // for admin to accesss user details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await users.GetAllUsers(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await users.GetUserById(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await users.DeleteUserById(rData));
        });

        var orders = e.ServiceProvider.GetRequiredService<orders>();
        e.MapPost("/orders", [Authorize] async (HttpContext http) => // for users order details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await orders.CreateOrder(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await orders.EditOrder(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await orders.DeleteOrder(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await orders.GetOrderById(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await orders.GetAllOrders(rData));
        }).RequireAuthorization();

        var wishlists = e.ServiceProvider.GetRequiredService<wishlists>();
        e.MapPost("/wishlists", [Authorize] async (HttpContext http) => // for users wishlists details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await wishlists.CreateWishList(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await wishlists.EditWishList(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await wishlists.DeleteWishList(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await wishlists.GetWishListById(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await wishlists.GetAllWishLists(rData));
        }).RequireAuthorization();

        var wishListCars = e.ServiceProvider.GetRequiredService<wishListCars>();
        e.MapPost("/wishListCars", [Authorize] async (HttpContext http) => // for wishlist cars details
        {
            var body = await new StreamReader(http.Request.Body).ReadToEndAsync();
            requestData rData = JsonSerializer.Deserialize<requestData>(body);
            if (rData.eventID == "1001") await http.Response.WriteAsJsonAsync(await wishListCars.AddToWishList(rData));
            if (rData.eventID == "1002") await http.Response.WriteAsJsonAsync(await wishListCars.UpdateInWishList(rData));
            if (rData.eventID == "1003") await http.Response.WriteAsJsonAsync(await wishListCars.RemoveFromWishList(rData));
            if (rData.eventID == "1004") await http.Response.WriteAsJsonAsync(await wishListCars.GetAWishListCar(rData));
            if (rData.eventID == "1005") await http.Response.WriteAsJsonAsync(await wishListCars.GetAllWishListCars(rData));
        }).RequireAuthorization();

        e.MapGet("/bing",
          async c => await c.Response.WriteAsJsonAsync("{'Name':'Akash','Age':'25','Project':'CarsHeaven'}"));
    });
}).Build().Run();

public record requestData
{
    [Required]
    public string eventID { get; set; }
    [Required]
    public IDictionary<string, object> addInfo { get; set; }
}

public record responseData
{
    public responseData()
    {
        eventID = "";
        rStatus = 0;
        rData = new Dictionary<string, object>();
    }
    [Required]
    public int rStatus { get; set; } = 0;
    public string eventID { get; set; }
    public IDictionary<string, object> addInfo { get; set; }
    public IDictionary<string, object> rData { get; set; }
}
