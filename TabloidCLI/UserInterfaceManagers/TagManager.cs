using System;
using System.Collections.Generic;
using TabloidCLI.Models;
using TabloidCLI.Repositories;

namespace TabloidCLI.UserInterfaceManagers
{


    public class TagManager : IUserInterfaceManager
    {

        private TagRepository _tagRepository;
        private string _connectionString;
        private readonly IUserInterfaceManager _parentUI;

        public TagManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _tagRepository = new TagRepository(connectionString);
            _connectionString = connectionString;

        }

        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Tag Menu");
            Console.WriteLine(" 1) List Tags");
            Console.WriteLine(" 2) Add Tag");
            Console.WriteLine(" 3) Edit Tag");
            Console.WriteLine(" 4) Remove Tag");
            Console.WriteLine(" 0) Go Back");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    List();
                    return this;
                case "2":
                    Add();
                    return this;
                case "3":
                    Edit();
                    return this;
                case "4":
                    Remove();
                    return this;
                case "0":
                    return _parentUI;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }
        }

        private void List()
        {
            List<Tag> tags = _tagRepository.GetAll();
            foreach (Tag tag in tags)
            {
                Console.WriteLine(tag);
            }
        }

        private void Add()
        {
            Console.WriteLine("New Tag");
            Tag tag = new Tag();

            Console.WriteLine("Name: ");
            tag.Name = Console.ReadLine();
            
            _tagRepository.Insert(tag);
        }


        private Tag Choose(string prompt = null)
        //Displays a list of tags and prompts user to choose a tag which user would like to edit.
        {
            if (prompt == null)
            {
                prompt = "Please choose a Tag:";
            }

            Console.WriteLine(prompt);

            List<Tag> tags = _tagRepository.GetAll();

            for (int i = 0; i < tags.Count; i++)
            {
                Tag tag = tags[i];
                Console.WriteLine($" {i + 1}) {tag.Name}");
            }
            Console.Write("> ");

            string input = Console.ReadLine();
            try
            {
                int choice = int.Parse(input);
                return tags[choice - 1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("Invalid Selection");
                return null;
            }
        }

            private void Edit()
        {   //Choose is returning the tag that we want out of the list of tags
            Tag tagToEdit = Choose("Which tag would you like to edit?");
            if (tagToEdit == null)
            {
                return;
            }

            //if user doesn't leave blank or unchanged then the data the user entered will be stored in title and passed into database 
            Console.WriteLine();
            Console.WriteLine("New Tag (blank to leave unchanged: ");
            string name = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(name))
            {
                tagToEdit.Name = name;
            }
            




            _tagRepository.Update(tagToEdit);
        
    }

        private void Remove()
        {
            Tag tagToDelete = Choose("Which tag would you like to remove?");
            if (tagToDelete != null)
            {
                _tagRepository.Delete(tagToDelete.Id);
            }
        }
    }
}
