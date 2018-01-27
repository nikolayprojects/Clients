using System;
using System.Collections.Generic;
using System.Linq;
using ClientsManagement.DAL;
using ClientsManagement.DTO;
using ClientsManagement.Models;
using ClientsManagement.UnitTest.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientsManagement.UnitTest
{
    [TestClass]
    public class ClientsModelTests
    {
        [TestMethod]
        public void LoadAllTest()
        {
            var (model, unitOfWork) = MakeTestData(false);
            unitOfWork.LoadTestData();

            List<ClientTypeDTO> clientsTypesDTO = new List<ClientTypeDTO>()
            {
                new ClientTypeDTO() { ID=1, Name="Физ.лицо"},
                new ClientTypeDTO() { ID=2, Name="Юр.лицо"}
            };

            List<ClientDTO> clientsDTO = new List<ClientDTO>()
            {
                new ClientDTO(){
                    ID=1,
                    INN ="111111111110",
                    Name ="Новый клиент 1",
                    DateContract = new DateTime(2018, 01, 10),
                    Contacts="Нижний Новгород Ивлева",
                    Type=clientsTypesDTO[0] },

                    new ClientDTO(){
                    ID=2,
                    INN ="111111111111",
                    Name ="Новый клиент 2",
                    DateContract = new DateTime(2018, 01, 10),
                    Contacts="Нижний Новгород Ванеева",
                    Type=clientsTypesDTO[1] }
            };

            model.LoadAllAsync().Wait();

            Assert.IsTrue(clientsDTO.SequenceEqual(model.ClientsList),
                    "Списки клиентов не совпадают.");
            Assert.IsTrue(clientsTypesDTO.SequenceEqual(model.ClientsTypesList),
                "Списки типов клиентов не совпадают.");
        }

        [TestMethod]
        public void AddClientAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData();

            ClientDTO clientDTO = new ClientDTO()
            {
                ID = 3,
                INN = "111111111110",
                Name = "Новый клиент 3",
                DateContract = new DateTime(2018, 01, 10),
                Contacts = "Нижний Новгород Гагарина",
                Type = new ClientTypeDTO() { ID = 1, Name = "Физ.лицо" }
            };

            model.AddClientAsync(clientDTO).Wait();

            Clients entityActual = unitOfWork.ClientsRepository.Get().
                Where(x => x.ID == clientDTO.ID).
                FirstOrDefault();

            bool compare = clientDTO.ID == entityActual?.ID &&
                clientDTO.INN == entityActual?.INN &&
                clientDTO.Name == entityActual?.Name &&
                clientDTO.DateContract == entityActual?.DateContract &&
                clientDTO.Contacts == entityActual?.Contacts &&
                clientDTO.Type.ID == entityActual?.ClientsTypes.ID &&
                clientDTO.Type.Name == entityActual?.ClientsTypes.Name;

            Assert.IsTrue(compare);
        }

        [TestMethod()]
        public void RemoveClientAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData();
            int id = model.ClientsList[0].ID;

            model.RemoveClientAsync(model.ClientsList[0]).Wait();

            ClientDTO clientDTOActual = model.ClientsList.Where(x => x.ID == id).FirstOrDefault();
            Assert.IsNull(clientDTOActual, "Клиент не был удалён из коллекции.");

            Clients entityActual = unitOfWork.ClientsRepository.FindByIDAsync(id).Result;
            Assert.IsNull(entityActual, "Клиент не был удалён из репозитория.");
        }

        [TestMethod()]
        public void UpdateClientAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData();

            ClientDTO clientDTO = model.ClientsList[0];
            clientDTO.Contacts = "Нижний Новгород Высоковский";
            clientDTO.INN = "123456789012";

            model.UpdateClientAsync(clientDTO).Wait();

            Clients entityActual = unitOfWork.ClientsRepository.FindByIDAsync(clientDTO.ID).Result;

            bool compare = entityActual.INN == "123456789012" &&
                entityActual.Contacts == "Нижний Новгород Высоковский";

            Assert.IsTrue(compare);
        }

        [TestMethod]
        public void AddClientTypeAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData(false);
            ClientTypeDTO clientTypeDTO = new ClientTypeDTO() { Name = "Прочие" };

            model.AddClientTypeAsync(clientTypeDTO).Wait();

            ClientsTypes entityActual = unitOfWork.ClientsTypesRepository.Get().FirstOrDefault();

            bool compare = entityActual?.Name == "Прочие";
            Assert.IsTrue(compare);
        }

        [TestMethod]
        public void RemoveClientTypeAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData();
            int id = model.ClientsTypesList[0].ID;

            model.RemoveClientTypeAsync(model.ClientsTypesList[0]).Wait();

            int count = model.ClientsList.Where(x => x.ID == id).Count();
            Assert.AreEqual(0, count, "Клиенты соответствующие данному типу не были удалены из коллекции.");

            ClientsTypes entityActual = unitOfWork.ClientsTypesRepository.FindByIDAsync(id).Result;
            Assert.IsNull(entityActual, "Данный тип клиента не был удалён из репозитория.");
        }

        [TestMethod]
        public void UpdateClientTypeAsyncTest()
        {
            var (model, unitOfWork) = MakeTestData();
            ClientTypeDTO clientTypeDTO = model.ClientsTypesList[0];
            clientTypeDTO.Name = "Новый тип";

            model.UpdateClientTypeAsync(clientTypeDTO).Wait();

            ClientsTypes entityActual = unitOfWork.ClientsTypesRepository.FindByIDAsync(clientTypeDTO.ID).Result;

            bool compare = entityActual?.Name == "Новый тип";
            Assert.IsTrue(compare);
        }

        static (ClientsModel model, FakeClientsUnitOfWork unitOfWork) MakeTestData(bool loadTestData = true)
        {
            FakeClientsUnitOfWork unitOfWork = new FakeClientsUnitOfWork();
            ClientsModel model = new ClientsModel(unitOfWork, new FakeCollectionWrapperFactory());

            if (loadTestData)
            {
                unitOfWork.LoadTestData();
                model.LoadAllAsync().Wait();
            }

            return (model, unitOfWork);
        }
    }
}