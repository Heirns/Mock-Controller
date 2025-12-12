using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mock.depart.Controllers;
using mock.depart.Models;
using mock.depart.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.depart.Controllers.Tests
{
    [TestClass()]
    public class CatsControllerTests
    {
        [TestMethod]
        public void Delete_CatNotFound()
        {
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(value: null);
            // ou serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns((Cat?)null);
            controller.Setup(c => c.UserId).Returns("1");

            var actionResult = controller.Object.DeleteCat(0);

            var result = actionResult.Result as NotFoundResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Delete_WrongOwner()
        {
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            CatOwner co = new CatOwner()
            {
                Id = "11111"
            };
            Cat c = new Cat()
            {
                Id = 1,
                Name = "Tite Dali",
                CatOwner = co,
                CuteLevel = Cuteness.Amazing
            };

            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(c);
            controller.Setup(c => c.UserId).Returns("1");

            var actionResult = controller.Object.DeleteCat(0);

            var result = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is not yours", result.Value);
        }

        [TestMethod]
        public void Delete_TooCute()
        {
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            CatOwner co = new CatOwner()
            {
                Id = "11111"
            };
            Cat c = new Cat()
            {
                Id = 1,
                Name = "Tite Dali",
                CatOwner = co,
                CuteLevel = Cuteness.Amazing
            };

            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(c);
            controller.Setup(c => c.UserId).Returns("11111");

            var actionResult = controller.Object.DeleteCat(0);

            var result = actionResult.Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Cat is too cute", result.Value);
        }

        [TestMethod]
        public void Delete_Ok()
        {
            Mock<CatsService> serviceMock = new Mock<CatsService>();
            Mock<CatsController> controller = new Mock<CatsController>(serviceMock.Object) { CallBase = true };

            CatOwner co = new CatOwner()
            {
                Id = "11111"
            };
            Cat c = new Cat()
            {
                Id = 1,
                Name = "Chat 1",
                CatOwner = co,
                CuteLevel = Cuteness.BarelyOk
            };

            serviceMock.Setup(s => s.Get(It.IsAny<int>())).Returns(c);
            serviceMock.Setup(s => s.Delete(It.IsAny<int>())).Returns(c);
            controller.Setup(c => c.UserId).Returns("11111");

            var actionResult = controller.Object.DeleteCat(0);

            var result = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(result);

            Cat? catresult = (Cat?)result!.Value;
            Assert.AreEqual(c.Id, catresult!.Id);
        }
    }
}