using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Projekt;
using Projekt.Authentication;
using System;
using System.Net.Http;

namespace Projekt_Testy
{
    public class UnitTest1
    {

        public UnitTest1()
        {

        }


        [Fact]
        public async Task OnPost_WithCorrectCredentials_RedirectsToIndex_Number2()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase2") // jak nie chcecie miec problemow z testami to kazda nazwa bd powinna byc inna
                .Options;
            var httpContext = new DefaultHttpContext();
            
            httpContext.Session = new Mock<ISession>().Object;

            using (var context = new Userdb(options))
            {
                context.Users.Add(new User
                {
                    Email = "existinguser@example.com",
                    Password = "Password123",
                    Name = "jan",
                    Surname = "Kowalski",
                    PhoneNumber = "1234567890",
                    Id = 1,
                    Items = "",
                    Role="client",
                    ObservedCategory=""

                });
                context.SaveChanges();
            }

            var pageModel = new Projekt.Pages.Users.LoginModel(new Userdb(options))
            {
                Email = "existinguser@example.com",
                Password = "Password123"
            };
            pageModel.PageContext = new PageContext { HttpContext = httpContext };

            // Act
            var result = await pageModel.OnPost();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectResult.PageName);
        }

        [Fact]
        public async Task CreatingAccount_Number1()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase1")
                .Options;
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;
            User user = new User
            {
                Email = "existinguser@example.com",
                Password = "Password123",
                Name = "jan",
                Surname = "Kowalski",
                PhoneNumber = "1234567890",
                ObservedCategory = ""
            };

            var pageModel = new Projekt.Pages.Users.CreateModel(new Userdb(options))
            {
                User = user,
            };
            pageModel.PageContext = new PageContext { HttpContext = httpContext };
            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = Assert.IsAssignableFrom<RedirectToPageResult>(result);
            Assert.Equal("./Index", redirectResult.PageName);
        }

        [Fact]
        public async Task CreatingAccountWhileThereExistsAccountWithTheSameEmail_Number13()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase13")
                .Options;

            User user = new User
            {
                Email = "existinguser@example.com",
                Password = "Password123",
                Name = "jan",
                Surname = "Kowalski",
                PhoneNumber = "1234567890",
                ObservedCategory = "",
                Id = 1,
                Role = "client",
                Items = ""
            };

            User user2 = new User
            {
                Email = "existinguser@example.com",
                Password = "Password123",
                Name = "jan",
                Surname = "Kowalski",
                PhoneNumber = "1234567890",
                ObservedCategory = ""
            };

            using (var context = new Userdb(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            var pageModel = new Projekt.Pages.Users.CreateModel(new Userdb(options))
            {
                User = user2
            };
            // Act
            var result = await pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);

        }

        [Fact]
        public async Task TryToLoginWithIncorrectData_number14()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase14") // jak nie chcecie miec problemow z testami to kazda nazwa bd powinna byc inna
                .Options;

            using (var context = new Userdb(options))
            {
                context.Users.Add(new User
                {
                    Email = "existinguser@example.com",
                    Password = "Password123",
                    Name = "jan",
                    Surname = "Kowalski",
                    PhoneNumber = "1234567890",
                    Id = 1,
                    Items = "",
                    Role = "client",
                    ObservedCategory = ""

                });
                context.SaveChanges();
            }

            var pageModel = new Projekt.Pages.Users.LoginModel(new Userdb(options))
            {
                Email = "existinguser@example.com",
                Password = "Password1234"
            };
            pageModel.PageContext = new PageContext { };

            // Act
            var result = await pageModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);

        }

        [Fact]
        public async Task UnauthorizedUserTriesToDoTheThingsThatHeCannot_Number12() // aktualnie nie dziala
        {
            // Arrange
            var options = new DbContextOptionsBuilder<Userdb>()
               .UseInMemoryDatabase(databaseName: "ItemDataBase121") // jak nie chcecie miec problemow z testami to kazda nazwa bd powinna byc inna
               .Options;

            var options2 = new DbContextOptionsBuilder<Itemdb>()
               .UseInMemoryDatabase(databaseName: "ItemDataBase122") // jak nie chcecie miec problemow z testami to kazda nazwa bd powinna byc inna
               .Options;
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;
            //httpContext.Session.SetString("Role", "client"); // -- problem z odczytaniem danej z sesji przez co zawsze zwroci jakby uzytkownik nie byl zautoryzowany
            
            var routeData = new RouteData();
            routeData.Values["page"] = "/CRUD/Create"; // Replace with your page path

            var pageContext = new PageContext(
                new ActionContext(httpContext, routeData, new PageActionDescriptor(), new ModelStateDictionary())

            );
            var pageModel = new Projekt.CRUD.CreateModel(new Itemdb(options2), new Userdb(options));
            pageModel.PageContext = pageContext;
            var attribute = new RequireAuthAttribute { RequiredRole = "client" };
            var executingContext = new PageHandlerExecutingContext(
                pageModel.PageContext,
                new List<IFilterMetadata>(),
                new HandlerMethodDescriptor(),
                new Dictionary<string, object>(),
                pageModel
            );
            // Act
            attribute.OnPageHandlerExecuting(executingContext);
            
            Assert.IsType<RedirectToPageResult>(executingContext.Result); //dla uzytkownika (client) powinno zwrocic RedirectToPage a nie RedirectResult
            


        }


    }
}