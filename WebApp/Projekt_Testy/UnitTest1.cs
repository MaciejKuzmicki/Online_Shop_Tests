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
        public async Task UserDataEdit_Number3()
        {
            // Arrange
            var optionsUser = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "UserDataBase3")
                .Options;
            Userdb userdb = new Userdb(optionsUser);
            var optionsItem = new DbContextOptionsBuilder<Itemdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase3")
                .Options;
            Itemdb itemdb = new Itemdb(optionsItem);
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;

            User user = new User
            {
                Email = "user@example.com",
                Password = "Password123",
                Name = "Jan",
                Surname = "Kowalski",
                PhoneNumber = "123456789",
                Id = 1,
                Items = "",
                Role = "client",
                ObservedCategory = ""
            };

            userdb.Users.Add(user);
            userdb.SaveChanges();

            string email = "newusermail@example.com";
            string name = "Adam";
            string surname = "Nowak";
            string phoneNumber = "987654321";
            var pageModel = new Projekt.Pages.Users.AboutMeModel(userdb, itemdb)
            { 
                user = user,
                Email = email,
                Name = name,
                Surname = surname,
                PhoneNumber = phoneNumber,
                ObservedCategory = ""
            };
            pageModel.PageContext = new PageContext { HttpContext = httpContext };

            //Act
            var result = await pageModel.OnPostAsync(1, "profile");

            // Assert
            Assert.IsType<PageResult>(result);
            User? userAssert = userdb.Users.FirstOrDefault(x => x.Id == 1);
            Assert.NotNull(userAssert);
            Assert.Equal(email, userAssert.Email);
            Assert.Equal(name, userAssert.Name);
            Assert.Equal(surname, userAssert.Surname);
            Assert.Equal(phoneNumber, userAssert.PhoneNumber);
        }

        [Fact]
        public async Task ShowAddedItems_Number4()
        {
            // Arrange
            var optionsUser = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "UserDataBase4")
                .Options;
            Userdb userdb = new Userdb(optionsUser);
            var optionsItem = new DbContextOptionsBuilder<Itemdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase4")
                .Options;
            Itemdb itemdb = new Itemdb(optionsItem);
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;

            int idItem = 1;
            string nameItem = "Test";
            string descriptionItem = "Description";
            double priceItem = 1.12;
            string categoryItem = "Books";
            Item item = new Item
            {
                Id = idItem,
                Name = nameItem,
                Description = descriptionItem,
                Price = priceItem,
                Category = categoryItem,
                State = "New",
                ImageData = ""
            };
            itemdb.Itemos.Add(item);
            itemdb.SaveChanges();

            int userId = 1;
            User user = new User
            {
                Email = "user@example.com",
                Password = "Password123",
                Name = "Jan",
                Surname = "Kowalski",
                PhoneNumber = "123456789",
                Id = userId,
                Items = $"{idItem}",
                Role = "client",
                ObservedCategory = ""
            };
            userdb.Users.Add(user);
            userdb.SaveChanges();

            var pageModel = new Projekt.Pages.Users.MyItemsModel(itemdb, userdb)
            {
                Category = "Any",
                PageContext = new PageContext { HttpContext = httpContext }
            };

            //Act
            await pageModel.OnGetAsync(user.Items);

            //Assert
            Assert.True(pageModel.Item.Contains(item)); 
        }

        [Fact]
        public async Task AddItem_Number5()
        {
            // Arrange
            var optionsUser = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "UserDataBase5")
                .Options;
            Userdb userdb = new Userdb(optionsUser);
            var optionsItem = new DbContextOptionsBuilder<Itemdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase5")
                .Options;
            Itemdb itemdb = new Itemdb(optionsItem);
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;

            int userId = 1;
            string email = "user@example.com";
            User user = new User
            {
                Email = email,
                Password = "Password123",
                Name = "Jan",
                Surname = "Kowalski",
                PhoneNumber = "123456789",
                Id = userId,
                Items = "",
                Role = "client",
                ObservedCategory = ""
            };
            userdb.Users.Add(user);
            userdb.SaveChanges();

            int itemId = 1;
            string itemName = "Test";
            string itemDescription = "Description";
            string itemState = "New";
            double itemPrice = 1.23;
            string itemCategory = "Books";
            Item item = new Item
            {
                Id = itemId,
                Name = itemName,
                Description = itemDescription,
                State = itemState,
                Price = itemPrice,
                Category = itemCategory,
                ImageData = "",
            };
            var pageModel = new Projekt.CRUD.CreateModel(itemdb, userdb)
            {
                Item = item,
                PageContext = new PageContext { HttpContext = httpContext }
            };

            //Act
            IActionResult result = await pageModel.OnPostAsync(email);

            //Assert
            Item? itemAssert = itemdb.Itemos.FirstOrDefault(x => x.Id == itemId);
            Assert.NotNull(itemAssert);
            Assert.Equal(itemAssert, item);
        }

        [Fact]
        public async Task EditItem_Number6()
        {
            // Arrange
            var optionsUser = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "UserDataBase6")
                .Options;
            Userdb userdb = new Userdb(optionsUser);
            var optionsItem = new DbContextOptionsBuilder<Itemdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase6")
                .Options;
            Itemdb itemdb = new Itemdb(optionsItem);
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;

            int idItem = 1;
            string nameItem = "Test";
            string descriptionItem = "Description";
            double priceItem = 1.12;
            string categoryItem = "Books";
            Item item = new Item
            {
                Id = idItem,
                Name = nameItem,
                Description = descriptionItem,
                Price = priceItem,
                Category = categoryItem,
                State = "New",
                ImageData = ""
            };
            itemdb.Itemos.Add(item);
            itemdb.SaveChanges();

            int userId = 1;
            User user = new User
            {
                Email = "user@example.com",
                Password = "Password123",
                Name = "Jan",
                Surname = "Kowalski",
                PhoneNumber = "123456789",
                Id = userId,
                Items = $"{idItem}",
                Role = "client",
                ObservedCategory = ""
            };
            userdb.Users.Add(user);
            userdb.SaveChanges();

            var pageModel = new Projekt.CRUD.EditModel(itemdb)
            {
                PageContext = new PageContext { HttpContext = httpContext }
            };
            IActionResult result1 = await pageModel.OnGetAsync(idItem);

            Assert.Equal(item, pageModel.Item);

            //Act

            string name = "Nowa nazwa";
            pageModel.Item.Name = name;
            IActionResult result2 = await pageModel.OnPostAsync();

            //Assert
            Item? itemAssert = itemdb.Itemos.FirstOrDefault(x => x.Id == idItem);
            Assert.NotNull(itemAssert);
            Assert.Equal(name, itemAssert.Name);
        }

        [Fact]
        public async Task RemoveItem_Number7()
        {
            // Arrange
            var optionsUser = new DbContextOptionsBuilder<Userdb>()
                .UseInMemoryDatabase(databaseName: "UserDataBase7")
                .Options;
            Userdb userdb = new Userdb(optionsUser);
            var optionsItem = new DbContextOptionsBuilder<Itemdb>()
                .UseInMemoryDatabase(databaseName: "ItemDataBase7")
                .Options;
            Itemdb itemdb = new Itemdb(optionsItem);
            var httpContext = new DefaultHttpContext();

            httpContext.Session = new Mock<ISession>().Object;

            int idItem = 1;
            string nameItem = "Test";
            string descriptionItem = "Description";
            double priceItem = 1.12;
            string categoryItem = "Books";
            Item item = new Item
            {
                Id = idItem,
                Name = nameItem,
                Description = descriptionItem,
                Price = priceItem,
                Category = categoryItem,
                State = "New",
                ImageData = ""
            };
            itemdb.Itemos.Add(item);
            itemdb.SaveChanges();

            int userId = 1;
            User user = new User
            {
                Email = "user@example.com",
                Password = "Password123",
                Name = "Jan",
                Surname = "Kowalski",
                PhoneNumber = "123456789",
                Id = userId,
                Items = $"{idItem}",
                Role = "client",
                ObservedCategory = ""
            };
            userdb.Users.Add(user);
            userdb.SaveChanges();

            var pageModel = new Projekt.CRUD.DeleteModel(itemdb, userdb)
            {
                PageContext = new PageContext { HttpContext = httpContext }
            };

            //Act
            IActionResult result1 = await pageModel.OnPostAsync(idItem);

            //Assert
            Item? itemAssert = itemdb.Itemos.FirstOrDefault(x => x.Id == idItem);
            Assert.Null(itemAssert);
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


    }
}