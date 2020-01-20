using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Runner
{
    class Program
    {
        private static IConfigurationRoot config;
        static void Main(string[] args)
        {
            Initialize();

            //Single Value Crud Operations 
            //Get_all_shoud_return_6_results();

            //Single and Multiple Entity Return
            //var id = Insert_should_assign_identity_to_new_entity();
            //Find_should_retrieve_existing_entity(id);
            //Modify_should_update_existing_entity(id);
            //Delete_should_remove_entity(id);

            //1 to Many Address for a contact
            //var repository = CreateRepository();
            //var mj = repository.GetFullContact(1);
            //mj.Output();

            //List_support_should_produce_correct_results();
            //Dynamic_List_support_should_produce_correct_results();

            //Bulk_insert_should_insert_4_rows();

            GetIllinoisAddresses();

            Console.ReadKey();
        }

        static void GetIllinoisAddresses()
        {
            // arrange
            var repository = CreateRepositoryEX();

            // act
            var addresses = repository.GetAddressesByState(17);

            // assert
            Debug.Assert(addresses.Count == 2);
            addresses.Output();
        }


        static void Bulk_insert_should_insert_4_rows()
        {
            // arrange
            var repository = CreateRepositoryEX();
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Charles", LastName = "Barkley" },
                new Contact { FirstName = "Scottie", LastName = "Pippen" },
                new Contact { FirstName = "Tim", LastName = "Duncan" },
                new Contact { FirstName = "Patrick", LastName = "Ewing" }
            };

            // act
            var rowsAffected = repository.BulkInsertContacts(contacts);

            // assert
            Console.WriteLine($"Rows inserted: {rowsAffected}");
            Debug.Assert(rowsAffected == 4);
        }

        static void List_support_should_produce_correct_results()
        {
            //arrange 
            var repository = CreateRepositoryEX();

            //act
            var contacts = repository.GetContactsById(1, 2, 4);

            //assert
            Debug.Assert(contacts.Count == 3);
            contacts.Output();
        }

        static void Dynamic_List_support_should_produce_correct_results()
        {
            //arrange 
            var repository = CreateRepositoryEX();

            //act
            var contacts = repository.GetDynamicContactsById(1, 2, 4);

            //assert
            Debug.Assert(contacts.Count == 3);
            Console.WriteLine($"First FirstName is : { contacts.First().FirstName} " );
            contacts.Output();
        }


        static void Delete_should_remove_entity(int id)
        {
            // arrange
            IContactRepository repository = CreateRepository();

            // act
            repository.Remove(id);

            // create a new repository for verification purposes
            IContactRepository repository2 = CreateRepository();
            var deletedEntity = repository2.Find(id);

            // assert
            Debug.Assert(deletedEntity == null);
            Console.WriteLine("*** Contact Deleted ***");
        }



        static void Modify_should_update_existing_entity(int id)
        {
            //arrange 
            IContactRepository repository = CreateRepository();
            //act
            //Single Operation
            //var contact = repository.Find(id);
            var contact = repository.GetFullContact(id);
            contact.FirstName = "Bob";
            contact.Addresses[0].StreetAddress = "456 Main Street";

            //Single Operation 
            //repository.Update(contact);
            repository.Save(contact);

            //create a new repo for verification purposes
            IContactRepository repository2 = CreateRepository();
            //Single Operation
            //var modifiedContact = repository2.Find(id);

            var modifiedContact = repository2.GetFullContact(id);

            
            //assert
            Console.WriteLine("*** Contact Modified ***");
            modifiedContact.Output();

            Debug.Assert(modifiedContact.FirstName == "Bob");
            Debug.Assert(modifiedContact.Addresses.First().StreetAddress == "456 Main Street");
        }


        static void Find_should_retrieve_existing_entity(int id)
        {
            // arrange
            IContactRepository repository = CreateRepository();

            // act
            //var contact = repository.Find(id);
            var contact = repository.GetFullContact(id);



            // assert
            Console.WriteLine("*** Get Contact ***");
            contact.Output();
            Debug.Assert(contact.FirstName == "Joe");
            Debug.Assert(contact.LastName == "Blow");
            Debug.Assert(contact.Addresses.Count == 1);
            Debug.Assert(contact.Addresses.First().StreetAddress == "123 Main Street");
        }

        static int Insert_should_assign_identity_to_new_entity()
        {
            //arrange
            IContactRepository repository = CreateRepository();
            var contact = new Contact
            {
                FirstName = "Joe",
                LastName = "Blow",
                Email = "joe.blow@mgmail.com",
                Company = "Microsoft",
                Title = "Developer"
            };
            var address = new Address
            { 
                AddressType = "Home",
                StreetAddress = "123 Main Street",
                City = "Baltimore",
                StateId = 1,
                PostalCode= "22222"
            };

            contact.Addresses.Add(address);

            //act
            //Single Individual Row 
            //repository.Add(contact);
            //Works on Complex object
            repository.Save(contact);

            //assert
            Debug.Assert(contact.Id != 0);
            Console.WriteLine("***Contact Inserted***");
            Console.WriteLine($"New ID :{contact.Id}");
            return contact.Id;


        }
                          

        static void Get_all_shoud_return_6_results()
        {
            //arrange
            var repository = CreateRepository();

            //act
            var contacts = repository.GetAll();

            //assert
            Console.WriteLine($"Count: {contacts.Count}");
            Debug.Assert(contacts.Count == 6);
            contacts.Output();

        }

        private static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(@"C:\Users\jaffe\source\repos\Dapper\02\demos\demos\after\DapperDemo\Runner")
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            config = builder.Build();
        }

        private static IContactRepository CreateRepository()
        {
            //return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
            return new ContactRepositorySP(config.GetConnectionString("DefaultConnection"));
        }

        private static ContactRepositoryEX CreateRepositoryEX()
        {
            return new ContactRepositoryEX(config.GetConnectionString("DefaultConnection"));
        }
    }
}
